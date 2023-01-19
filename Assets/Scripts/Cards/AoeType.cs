using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AoeType
{
    // a single effect only has 1 AOE Type, no combinations
	Circle = 0, // works as cicle around self
	Line = 1, // line in front of player
	HalveCircleFront = 2, // halve circle in front of player
	QuarterCircleFront = 3, // quarter circle in front of player
}
