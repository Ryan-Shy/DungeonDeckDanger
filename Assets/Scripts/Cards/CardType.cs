using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardType
{
	// base types
    Attack = 1,
	Defence = 2,
	Heal = 4,
	Movement = 8,
	// combined types
	AttackDefence = 3,
	AttackHeal = 5,
	AttackMovement = 9,
	AttackDefenceHeal = 7,
	AttackDefenceMovement = 11,
	AttackHealMovement = 13,
	DefenceHeal = 6,
	DefenceMovement = 10,
	DefenceHealMovement = 14,
	HealMovement = 12,
	// special cases
	None = 0,
	All = 15,
	Invalid = -1
}
