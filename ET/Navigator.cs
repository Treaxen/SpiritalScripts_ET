/*
 * Navigator Source Code (No need to attach to any objects)
 * Ng E-Tjing
 * 
 * Basic use:
 * 1. declare a class instance and refer the Agent property to the respective component
 * 2. To set destination, call the function CalculateRoute(Vector3 point)
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Andrew;

namespace Treaxen
{
	public class Navigator
	{
		public NavMeshPath path = null;
		private bool isCheckingPos = false;
		private bool isRecentering = false;
		private bool isStillMoving = false;
		private bool isPathComplete = false;
		private Floor curGroundBlock = null;
		private Task moveToDestination = null;

		private NavMeshAgent agent	= null;

		private Vector3 preCorner;
		private Vector3 curCorner;
		private Vector3 targetDir;
		private Vector3 localForward;

		public NavMeshAgent Agent
		{
			get{return agent;}
			set{agent = value;}
		}

		public Floor CurGroundBlock
		{
			get {return curGroundBlock;}
			set {curGroundBlock = value;}
		}

		public Vector3 CurPos
		{
			get{return Agent.gameObject.transform.position;}
			set{Agent.gameObject.transform.position = value;}
		}

		public Quaternion CurRot
		{
			get{return agent.gameObject.transform.rotation;}
			set{agent.gameObject.transform.rotation = value;}
		}

		public Vector3 CurCorner
		{
			get{return curCorner;}
		}

		public Vector3 LocalPos
		{
			get{return agent.gameObject.transform.localPosition;}
			set{agent.gameObject.transform.localPosition = value;}
		}

		public bool IsStillMoving
		{
			get {return isStillMoving;}
		}

		public bool IsPathComplete
		{
			get {return isPathComplete;}
		}
			
		public NavMeshPath CalculatePath(Vector3 point)
		{
			if (agent != null)
			{
				if (path == null)
				{
					path = new NavMeshPath();
				}
				agent.CalculatePath(point, path);
			}

			return path;
		}

		//Mainlhy used to set travel destination
		public void CalculateRoute(Vector3 point)
		{
			if (agent != null)
			{
				if (path == null)
				{
					path = new NavMeshPath(); //create a new navmeshpath instance if it's not available
				}
				agent.CalculatePath(point, path); //calculate the path, the corners of the path would be used for the rest of the processes

				if (path.status != NavMeshPathStatus.PathComplete)
					isPathComplete = false;
				else
					isPathComplete = true;
			}

			ResetAgentPath();//If this function is called when the agent is moving, stop it and reset everything

			isStillMoving = true;
			moveToDestination = new Task (Navigate(), true); // Call the coroutine to move the character

			//Task rotateBeforeMove;
			//rotateBeforeMove = new Task(RotateBeforeMove(point), true);
		}

		public void ObstacleCollide ()
		{
			if (!isCheckingPos)
			{
				Debug.Log ("ouch!");
				isCheckingPos = true;

				Task checkIfStationary;
				checkIfStationary = new Task (CheckIfStationary(), true);
			}
		}

		private void ResetAgentPath()
		{
			if (moveToDestination != null) 
			{
				moveToDestination.EndTask();
				moveToDestination = null;
				if (agent.hasPath)
				{
					agent.velocity = Vector3.zero;
					agent.isStopped = true;
					agent.ResetPath();
				}
			}
		}

		private IEnumerator Navigate()
		{
			if (path.corners.Length > 0)
			{
				preCorner = path.corners[0];
			}

			while(true)
			{
				int i = 1;
//				Debug.Log("Corners : " + path.corners.Length);
				while (i < path.corners.Length) //keep runnng as long as destination is not reached
				{
//					if (agent.path != null)
//					{
//						Debug.Log ("Path distance : " + agent.remainingDistance);
//						if (agent.remainingDistance > 0.01f) //If already moving during iteration, don't do anything unless destination is reached
//						{
//							Debug.Log ("Still Walking");
//							yield return null;
//						}
//					}

					agent.updateRotation = false; //prevent the agent from controlling the character's rotation

					curCorner = path.corners[i];
					targetDir = curCorner - CurPos;
					targetDir.y = 0;
//					Debug.Log ("Cur Corner : " + curCorner);
//					Debug.Log ("CurPos : " + CurPos);
//					Debug.Log ("Target Dir : " + targetDir);

					if (targetDir == Vector3.zero)
					{
						i++;
						continue;
					}

					localForward = agent.transform.forward; //get the front direction of the body, ignoring y axis
					localForward.y = 0;

					float angle = Vector3.Angle(targetDir, localForward); //get the remaining angle to rotate

					while (angle > 2.0f) //keep rotating and do nothing else when the angle is >2. didn't put >0 because rotation is not 100% accurate
					{
						Quaternion rotation = Quaternion.LookRotation(targetDir, Vector3.up);
						agent.gameObject.transform.localRotation = Quaternion.Slerp(agent.gameObject.transform.localRotation, rotation, Time.deltaTime * 10.0f);

						localForward = agent.transform.forward;
						localForward.y = 0;

						angle = Vector3.Angle(targetDir, localForward);
						yield return null;
					}

					agent.updateRotation = true;

					agent.ResetPath();
					agent.destination = path.corners[i]; //re-enable update and set agent path to the next corner of the path

					if (agent.pathPending)
					{
						yield return null;
					}

//					Debug.Log ("Path distance : " + agent.remainingDistance);
					while (agent.remainingDistance > 0.01f) //If already moving during iteration, don't do anything unless destination is reached
					{
//						Debug.Log ("Still Walking");
						yield return null;
					}

					preCorner = curCorner;
					i++;

					yield return null;
				}

				while (agent.remainingDistance > 0.1f)
				{
					yield return null;
				}

				Debug.Log("Path Completed");
				isStillMoving = false;
				yield break;
			}
		}

		private IEnumerator RecenterPlayer()
		{
			Vector3 targetPosition = new Vector3 (curGroundBlock.transform.position.x, CurPos.y, curGroundBlock.transform.position.z);
			while (isRecentering)
			{
				this.CurPos = Vector3.Lerp(this.CurPos,targetPosition, 2.0f *Time.deltaTime);

				if (Vector3.Distance(this.CurPos,targetPosition) < 0.01f)
				{
					Debug.Log ("Recentered");
					isRecentering = false;
					yield break;
				}

				if (agent.hasPath)
				{
					isRecentering = false;
					yield break;
				}

				yield return null;
			}
		}

		private IEnumerator CheckIfStationary()
		{
			float timer = 0.0f;
			Vector3 startPos = this.CurPos;
			Quaternion startRot = this.CurRot;

			while (true)
			{
				if (timer < 0.1f)
				{
					if (Vector3.Distance(startPos,this.CurPos) > 0.2f) //if >0.2f, means character is still moving
					{
						isCheckingPos = false;
						yield break;
					}

					else if (Quaternion.Angle(startRot, this.CurRot ) > 5.0f )//if angle >5, means character is trying to turn around
					{
						isCheckingPos = false;
						yield break;
					}
				}
				else
				{
					break;
				}

				timer += Time.deltaTime;

				yield return null;
			}

			Debug.Log ("Stops");

			Player.instance.State = PlayerState.MOVESTATE_IDLE;
			ResetAgentPath();
			CalculateRoute(curGroundBlock.transform.position);
			isCheckingPos = false;
		}

//		IEnumerator RotateBeforeMove(Vector3 point)
//		{
//			if (agent != null)
//			{
//				if (path == null)
//				{
//					path = new NavMeshPath();
//				}
//				agent.CalculatePath(point, path);
//			}
//
//			//Transform curTransform = agent.transform.GetComponentInParent<Transform>();
//
//			/*while(Vector3.Dot(curTransform.forward, (point - curTransform.position).normalized) > 0.8f)
//			{
//				//Vector3.RotateTowards(curTransform.position, point, 0.1f, 0.1f);
//				yield return null;
//			}*/
//
//			Agent.path = path;
//
//			yield break;
//		}
	}
}
