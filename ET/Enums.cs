using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EntityType
{
	EntityType_Spirit = 0,
	EntityType_Heavy,
	EntityType_Light,
	EntityType_Neutral,

	EntityType_Total
};

public enum PlayerState
{
	MOVESTATE_IDLE = 0,
	MOVESTATE_MOVING,
	MOVESTATE_SWITCHING,
	MOVESTATE_TRANSITION,

	MOVESTATE_TOTAL
};

public enum BlockType
{
	Floor_3Way_0 = 0,
	Floor_3Way_1,
	Floor_Center_1,
	Floor_Corner_3,
	Floor_Corner_4,
	Floor_Edge_1,
	Floor_Edge_2,
	Floor_End_0,
	Floor_Middle_0,
	Obstacle_BreakableWall,
	Obstacle_Water,
	Obstacle_NarrowWall,
	Obstacle_Door,
	Wall_Prototype,
	Wall_1,
	Wall_2,
	Wall_3,
	BlockType_Total
};
