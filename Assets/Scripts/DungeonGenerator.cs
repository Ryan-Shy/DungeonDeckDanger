using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum DungeonState {inactive, generatingMain, genreatingBranches, cleanup, completed}

public class DungeonGenerator : MonoBehaviour
{
    [SerializeField] GameObject[] startPrefabs;
    [SerializeField] GameObject[] tilePrefabs;
    [SerializeField] GameObject[] blockedPrefabs;
    [SerializeField] GameObject[] exitPrefabs;
    [SerializeField] GameObject[] doorPrefabs;

    [Header("Debugging Options")] 
    [SerializeField] bool useBoxColliders;
    [SerializeField] bool useLightsforDebugging;
    [SerializeField] bool restoreLightsAfterDebugging;
    
    
    [Header("Keybinding options")]
    public KeyCode reloadKey = KeyCode.Backspace;
    public KeyCode toogleMapKey = KeyCode.M;
    
    [Header("Generation Limits")]
    [Range(0, 1f)] [SerializeField] float constructionDelay = 0.5f;
    [Range(2, 100)] [SerializeField] int mainLength;
    [Range(0, 50)] [SerializeField] int branchLength = 5;
    [Range(0, 25)] [SerializeField] int numBranch = 10;
    [Range(0, 100)] [SerializeField] int doorPercent = 25;
    
    [Header("Available at Runtime")]
    [HideInInspector] public DungeonState dungeonState = DungeonState.inactive;
    public List<Tile> generatedTiles = new List<Tile>();
	[HideInInspector] public System.Random generatorRandom;

    private GameObject goCamera, goPlayer;
    private List<Connector> availableConnectors = new List<Connector>();
    private Color startLightColor = Color.white;
    private Transform tileFrom, tileTo, tileRoot, container;
    private int attempts;
    private int maxAttempts = 50;
    
    void Start()
    {
        goCamera = GameObject.Find("OverheadCamera");
        goPlayer = GameObject.FindWithTag("Player");
        if(Saves.current != null && Saves.current.HasKey("genSeed") && Saves.current.HasKey("floor"))
		{
			int baseSeed = Saves.current.GetInt("genSeed");
			int floor = Saves.current.GetInt("floor");
			generatorRandom = new System.Random(baseSeed + floor);
			Debug.Log("Generating with seed: " + (baseSeed + floor));
		}
		else
		{
			// unseeded rng
			Debug.Log("Generating without seed");
			generatorRandom = new System.Random(0);
		}
        StartCoroutine(DungeonBuild());
    }
    
    void Update()
    {
        if (Input.GetKeyDown(reloadKey))
        {
            SceneManager.LoadScene(5);
        }

        if (Input.GetKeyDown(toogleMapKey))
        {
            goCamera.SetActive(!goCamera.activeInHierarchy);
            goPlayer.SetActive(!goPlayer.activeInHierarchy);
        }
    }

    IEnumerator DungeonBuild()
    {
        goCamera.SetActive(true);
        goPlayer.SetActive(false);
        GameObject goContainer = new GameObject("Main Path");
        container = goContainer.transform;
        container.SetParent(transform);
        tileRoot = CreateStartTile();
        DebugRoomLightning(tileRoot, Color.cyan);
        tileTo = tileRoot;
        dungeonState = DungeonState.generatingMain;
        for (int i = 0; i < mainLength - 1; i++)
        {
            yield return new WaitForSeconds(constructionDelay);
            tileFrom = tileTo;
            tileTo = CreateTile();
            DebugRoomLightning(tileTo, Color.yellow);
            ConnectTiles();
            CollisionCheck(false);
            if (attempts >= maxAttempts)
            {
                break;
            }
        }
		// Final Room generation
		tileFrom = tileTo;
		tileTo = CreateExitTile();
		DebugRoomLightning(tileTo, Color.yellow);
		ConnectTiles();
		CollisionCheck(true);

		// Get all connectors within container that not already connected
		foreach (Connector connector in container.GetComponentsInChildren<Connector>())
        {
            if (!connector.isConnected)
            {
                if (!availableConnectors.Contains(connector))
                {
                    availableConnectors.Add(connector);
                }
            }
        }
        
        // Branching
        dungeonState = DungeonState.genreatingBranches;
        for (int b = 0; b < numBranch; b++)
        {
            if (availableConnectors.Count > 0)
            {
                goContainer = new GameObject("Branch " + (b + 1));
                container = goContainer.transform;
                container.SetParent(transform);
                int availableIndex = generatorRandom.Next(0, availableConnectors.Count);
                tileRoot = availableConnectors[availableIndex].transform.parent.parent;
                availableConnectors.RemoveAt(availableIndex);
                tileTo = tileRoot;
                for (int i = 0; i < branchLength - 1; i++)
                {
                    yield return new WaitForSeconds(constructionDelay);
                    tileFrom = tileTo;
                    tileTo = CreateTile();
                    DebugRoomLightning(tileTo, Color.green);
                    ConnectTiles();
                    CollisionCheck(false);
                    if (attempts >= maxAttempts)
                    {
                        break;
                    }
                }
            }
            else { break; }
        }

        dungeonState = DungeonState.cleanup;
        lightRestoration();
        CleanUpBoxCollider();
        BLockedPassages();
        SpawnDoors();
        dungeonState = DungeonState.completed;
        yield return null; // to wait one frame for spawning objects
        goCamera.SetActive(false);
        goPlayer.SetActive(true);
    }

    void SpawnDoors()
    {
        if (doorPercent > 0)
        {
            Connector[] allConnectors = transform.GetComponentsInChildren<Connector>();

            for (int i = 0; i < allConnectors.Length; i++)
            {
                Connector myConnector = allConnectors[i];

                if (myConnector.isConnected)
                {
                    // random chance of spawning a door
                    int roll = generatorRandom.Next(1, 101);

                    if (roll <= doorPercent)
                    {
                        Vector3 halfExtents = new Vector3(myConnector.size.x, 1f, myConnector.size.x);
                        Vector3 pos = myConnector.transform.position;
                        Vector3 offset = Vector3.up * 0.5f;
                        Collider[] hits = Physics.OverlapBox(pos + offset, halfExtents, Quaternion.identity,
                            LayerMask.GetMask("Door"));
                        if (hits.Length == 0)
                        {
                            int doorIndex = generatorRandom.Next(0, doorPrefabs.Length);
                            GameObject goDoor = Instantiate(doorPrefabs[doorIndex], pos, myConnector.transform.rotation,
                                myConnector.transform) as GameObject;
                            goDoor.name = doorPrefabs[doorIndex].name;
							// give door an id
							ToogleDoor td = goDoor.GetComponentInChildren<ToogleDoor>();
							if(td != null)
							{
								td.SetID(generatorRandom.Next().ToString());
							}
						}
                    }
                }
            }
        }
    }
    void BLockedPassages()
    {
        foreach (Connector connector in transform.GetComponentsInChildren<Connector>())       
        {
            if (!connector.isConnected)
            {
                Vector3 pos = connector.transform.position;
                int wallIndex = generatorRandom.Next(0,blockedPrefabs.Length);
                GameObject goWall = Instantiate(blockedPrefabs[wallIndex], pos, connector.transform.rotation,
                    connector.transform) as GameObject;
                goWall.name = blockedPrefabs[wallIndex].name;
            }
        }
    }
    
    void CollisionCheck(bool isBossRoom)
    {
        BoxCollider box = tileTo.GetComponent<BoxCollider>();
        if (box == null)
        {
            box = tileTo.gameObject.AddComponent<BoxCollider>();
            box.isTrigger = true;
        }

        Vector3 offset = (tileTo.right * box.center.x) + (tileTo.up * box.center.y) + (tileTo.forward * box.center.z);
        Vector3 halfExtents = box.bounds.extents;
        List<Collider> hits = Physics
            .OverlapBox(tileTo.position + offset, halfExtents, Quaternion.identity, 
                LayerMask.GetMask("Tile")).ToList();

        if (hits.Count > 0)
        {
            if (hits.Exists(x =>x.transform != tileFrom && x.transform != tileTo))
            {
                // hit something other than tileFrom or tileTo
                attempts++;
                int toIndex = generatedTiles.FindIndex(x => x.tile == tileTo);
                if (generatedTiles[toIndex].connector != null)
                {
                    generatedTiles[toIndex].connector.isConnected = false;
                }
                generatedTiles.RemoveAt(toIndex);
                DestroyImmediate(tileTo.gameObject);
                
                // Backtracking
                if (attempts >= maxAttempts)
                {
                    int fromIndex = generatedTiles.FindIndex(x => x.tile == tileFrom);
                    Tile myTileFrom = generatedTiles[fromIndex];
                    
                    // Ensures to not destroy a root tile 
                    if (tileFrom != tileRoot)
                    {
                        if (myTileFrom.connector != null)
                        {
                            myTileFrom.connector.isConnected = false;
                        }
                        
                        availableConnectors.RemoveAll(x => x.transform.parent == tileFrom);
                        generatedTiles.RemoveAt(fromIndex);
                        DestroyImmediate(tileFrom.gameObject);
                        
                        if (myTileFrom.origin != tileRoot)
                        {
                            tileFrom = myTileFrom.origin;
                        }
                        else if (container.name.Contains("Main"))
                        {
                            if (myTileFrom.origin != null)
                            {
                                tileRoot = myTileFrom.origin;
                                tileFrom = tileRoot;
                            }
                        }
                        else if (availableConnectors.Count > 0)
                        {
                            int availIndex = generatorRandom.Next(0, availableConnectors.Count);
                            tileRoot = availableConnectors[availIndex].transform.parent.parent;
                            availableConnectors.RemoveAt(availIndex);
                            tileFrom = tileRoot;
                        }
                        else
                        {
                            return;
                        }
                    }
                    else if (container.name.Contains("Main"))
                    {
                        if (myTileFrom.origin != null)
                        {
                            tileRoot = myTileFrom.origin;
                            tileFrom = tileRoot;
                        }
                    }
                    else if (availableConnectors.Count > 0)
                    {
                        int availIndex = generatorRandom.Next(0, availableConnectors.Count);
                        tileRoot = availableConnectors[availIndex].transform.parent.parent;
                        availableConnectors.RemoveAt(availIndex);
                        tileFrom = tileRoot;
                    }
                    else
                    {
                        return;
                    }
                }
                
                // retry
                if (tileFrom != null)
                {
					if (isBossRoom)
					{
						tileTo = CreateExitTile();
					}
					else
					{
						tileTo = CreateTile();
					}
                    Color retryColor = container.name.Contains("Branch") ? Color.green : Color.yellow;
                    DebugRoomLightning(tileTo, retryColor * 2f);
                    ConnectTiles();
                    CollisionCheck(isBossRoom); // Recursion
                }
            }
            else
            {   
                // nothing other then tileTo and tileFrom was hit (restore attempts to zero)
                attempts = 0;
            }
        }
    }
    
    void lightRestoration()
    {
        if (useLightsforDebugging && restoreLightsAfterDebugging && Application.isEditor)
        {
            Light[] lights = transform.GetComponentsInChildren<Light>();

            foreach (Light light in lights)
            {
                light.color = startLightColor;
            }
        }
    }

    void CleanUpBoxCollider()
    {
        if (!useBoxColliders)
        {
            foreach (Tile myTile in generatedTiles)
            {
                BoxCollider box = myTile.tile.GetComponent<BoxCollider>();
                if (box != null)
                {
                    Destroy(box);
                }
            }
        }
    }
    
    void DebugRoomLightning(Transform tile, Color lightColor)
    {
        if (useLightsforDebugging && Application.isEditor)
        {
            Light[] lights = tile.GetComponentsInChildren<Light>();
            if (lights.Length > 0)
            {
                if (startLightColor == Color.white)
                {
                    startLightColor = lights[0].color;
                }

                foreach (Light light in lights)
                {
                    light.color = lightColor;
                }
            }
        }
    }
    
    Transform CreateStartTile()
    {
        // Generate random number as an index to select the start room from the start room GO array
        int index = generatorRandom.Next(0, startPrefabs.Length);
        
        // Instantiate the chosen start room at origin
        GameObject goTile = Instantiate(startPrefabs[index], Vector3.zero, Quaternion.identity, container) as GameObject;
        
        // Apply a random rotation to the start room
        float yRot = generatorRandom.Next(0, 4) * 90f;
        goTile.transform.Rotate(0, yRot, 0);
        
        // Ensures the player looks at the doorway in the start room
        goPlayer.transform.LookAt(goTile.GetComponentInChildren<Connector>().transform.position);
        
        // Add to generatedTiles list
        generatedTiles.Add(new Tile(goTile.transform, null));
        return goTile.transform;
    }

    Transform CreateTile()
    {
        // Generate random number as an index to select the tile from the tilePrefabs GO array
        int index = generatorRandom.Next(0, tilePrefabs.Length);
        
        // Instantiate the chosen start room at origin
        GameObject goTile = Instantiate(tilePrefabs[index], Vector3.zero, Quaternion.identity, container) as GameObject;
        
        // Name the tile in the hierarchy
        goTile.name = tilePrefabs[index].name;

        Transform origin = generatedTiles[generatedTiles.FindIndex(x => x.tile == tileFrom)].tile;
        generatedTiles.Add(new Tile(goTile.transform, origin));
        return goTile.transform;
    }

	Transform CreateExitTile()
	{
		// Generate random number as an index to select the tile from the tilePrefabs GO array
		int index = generatorRandom.Next(0, exitPrefabs.Length);

		// Instantiate the chosen start room at origin
		GameObject goTile = Instantiate(exitPrefabs[index], Vector3.zero, Quaternion.identity, container) as GameObject;

		// Name the tile in the hierarchy
		goTile.name = exitPrefabs[index].name;

		Transform origin = generatedTiles[generatedTiles.FindIndex(x => x.tile == tileFrom)].tile;
		generatedTiles.Add(new Tile(goTile.transform, origin));
		return goTile.transform;
	}

    void ConnectTiles()
    {
        Transform connectFrom = GetRandomConnector(tileFrom);
        if (connectFrom == null) { return; }

        Transform connectTo = GetRandomConnector(tileTo);
        if (connectTo == null) { return; }
        
        // parent the connector of tileTo to the connector of tileFrom
        connectTo.SetParent(connectFrom);
        
        // parent the tileTo to the connector of tileTo
        tileTo.SetParent(connectTo);
        
        // reset position and rotation of connector of tileTo
        connectTo.localPosition = Vector3.zero;
        connectTo.localRotation = Quaternion.identity;
        
        // rotate the connector of tileTo 180deg around the y axis
        connectTo.transform.Rotate(0,180f,0);
        
        // re-parent tileTo to the Generator object
        tileTo.SetParent(container);
        
        // re-parent the connector of tileTo to connector inside tileTo
        connectTo.SetParent(tileTo.Find("Connectors"));

        generatedTiles.Last().connector = connectFrom.GetComponent<Connector>();
    }

    Transform GetRandomConnector(Transform tile)
    {
        if (tile == null) { return null; }
        
        // Find all connectors in the tile which are not connected
        List<Connector> connectorList =
            tile.GetComponentsInChildren<Connector>().ToList().FindAll(x => x.isConnected == false);

        if (connectorList.Count > 0)
        {
            int index = generatorRandom.Next(0, connectorList.Count);
            connectorList[index].isConnected = true;
            if (tile == tileFrom)
            {
                BoxCollider box = tile.GetComponent<BoxCollider>();
                if (box == null)
                {
                    box = tile.gameObject.AddComponent<BoxCollider>();
                    box.isTrigger = true;
                }
            }
            return connectorList[index].transform;
        }
        return null;
    }
}
