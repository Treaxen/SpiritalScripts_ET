using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;
using Treaxen;


namespace Andrew
{
	public class Obstacle : Block 
	{
		public enum PathDirection
		{
			X_DIR = 0,
			Z_DIR,
			TOTAL_DIR
		}

		private NavMeshObstacle navMeshObstacle = null;
		public NavMeshObstacle _navMeshObstacle
		{
			get{return navMeshObstacle;}
		}

		public Vector3 enterVector;
		public Vector3 exitVector;

		public void Start()
		{
			if (this.GetComponent<NavMeshObstacle>() != null)
				navMeshObstacle = this.GetComponent<NavMeshObstacle>();
		}

		public virtual void OnEntityTypeSwitch(EntityType entityType){}

		void OnEnable()
		{
			Player.OnEntityTypeSwitch += OnEntityTypeSwitch;
		}
		void OnDisable()
		{
			Player.OnEntityTypeSwitch -= OnEntityTypeSwitch;
		}

		public void DirectionChecker(PathDirection checkingDirection)
		{
			Vector3 playerPos = Player.instance.CurBody.transform.position;

			if(checkingDirection == PathDirection.Z_DIR)
			{
				if(playerPos.z > transform.position.z)
				{
					enterVector = new Vector3(transform.position.x, transform.position.y, transform.position.z + 1);
					exitVector = new Vector3(transform.position.x, transform.position.y, transform.position.z - 1);
				}
				else if(playerPos.z  < transform.position.z)
				{
					enterVector = new Vector3(transform.position.x, transform.position.y, transform.position.z - 1);
					exitVector = new Vector3(transform.position.x, transform.position.y, transform.position.z + 1);
				}
			}
			else if(checkingDirection == PathDirection.X_DIR)
			{
				if(playerPos.x > transform.position.x)
				{
					enterVector = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z);
					exitVector = new Vector3(transform.position.x - 1, transform.position.y, transform.position.z);
				}
				else if(playerPos.x  < transform.position.x)
				{
					enterVector = new Vector3(transform.position.x - 1, transform.position.y, transform.position.z);
					exitVector = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z);
				}
			}

			enterVector = new Vector3(enterVector.x, 1.0f, enterVector.z);
			exitVector = new Vector3(exitVector.x, 1.0f, exitVector.z);

			if(Player.instance.Navigator.CalculatePath(enterVector).status == NavMeshPathStatus.PathPartial
				&& Player.instance.Navigator.CalculatePath(exitVector).status == NavMeshPathStatus.PathComplete)
			{
				Vector3 tempVector = enterVector;
				enterVector = exitVector;
				exitVector = tempVector;
			}
		}
	}
}
