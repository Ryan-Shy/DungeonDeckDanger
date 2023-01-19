using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour {

    public float mouseSensitivity = 500f;

    Transform player;
    float xRot;

    void Start() {
        player = transform.parent;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update() {
		float sensitivity = mouseSensitivity;
		if(Saves.profile != null && Saves.profile.HasKey("MouseSensitivity"))
		{
			sensitivity = Saves.profile.GetFloat("MouseSensitivity");
		}
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
        xRot -= mouseY;
        xRot = Mathf.Clamp(xRot, -90f, 90f);
        transform.localRotation = Quaternion.Euler(xRot, 0, 0);
        player.Rotate(Vector3.up * mouseX);
    }

}
