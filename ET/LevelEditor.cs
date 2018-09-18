using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Treaxen
{
	public class LevelEditor : MonoBehaviour
	{
		public GameObject[] blockList 		= new GameObject[(int)BlockType.BlockType_Total];

		public BlockType	blockType		= 0;
		public Vector3 		startPos		= new Vector3(0,0,0);
		public Vector3		endPos			= new Vector3(0,0,0);
		public Transform	level			= null;

		public Vector3		blockScale
		{
			get {return blockList[(int)blockType].transform.localScale;}
		}

		public Vector3		blockSize
		{
			get
			{
				Vector3 tempSize;

				tempSize = blockList[(int)blockType].GetComponent<BoxCollider>().size;

//				if (blockList[(int)blockType].GetComponent<MeshFilter>() != null)
//				{
//					tempSize = blockList[(int)blockType].GetComponent<MeshFilter>().sharedMesh.bounds.size;
//				}
//				else
//				{
//					tempSize = blockList[(int)blockType].GetComponentInChildren<MeshFilter>().sharedMesh.bounds.size;
//				}

				tempSize.x = Calculation.CeilToTenth (tempSize.x) * blockScale.x;
				tempSize.y = Calculation.CeilToTenth (tempSize.y) * blockScale.y;
				tempSize.z = Calculation.CeilToTenth (tempSize.z) * blockScale.z;

				return tempSize;
			}
		}
			
//		public Vector3 		objectScale 	= new Vector3(1,1,1);

//		public Vector3		RealPosition
//		{
//			get 
//			{
//				return objectPosition + (objectScale/2);
//			}
//		}

		public void CreateObject()
		{
			TileBlocks(blockList[(int)blockType]);
		}

		public void TileBlocks(GameObject block)
		{
			Debug.Log ("Tiling Starting");

			int xRange = Mathf.CeilToInt(Mathf.Abs(endPos.x - startPos.x)) + 1;
			int yRange = Mathf.CeilToInt(Mathf.Abs(endPos.y - startPos.y)) + 1;
			int zRange = Mathf.CeilToInt(Mathf.Abs(endPos.z - startPos.z)) + 1;

//			if(xRange <= 0) xRange = 1;
//			if(yRange <= 0) yRange = 1;
//			if(zRange <= 0) zRange = 1;


			Debug.Log ("xRange : " + xRange + " yRange : " + yRange + " zRange : " + zRange);

			for (int i = 0; i < yRange; i++)
			{
				for (int j = 0; j < zRange; j++)
				{
					for (int k = 0; k < xRange; k++)
					{
						Debug.Log (" x size of block : " + blockSize.x);
						Vector3 spawnPos = new Vector3(startPos.x + (k*blockSize.x), startPos.y + (i*blockSize.y), startPos.z + (j*blockSize.z));
						Debug.Log ("Spawning at : " + spawnPos);
						DeleteIfOccupied(spawnPos);
						GameObject obj = Instantiate(block,spawnPos,block.transform.rotation);
						AddToLevel(obj);
					}
				}
			}

			Debug.Log ("Tiling Completed");
		}

		public void PlaceBlock(Ray ray)
		{
			RaycastHit hit;

			if (Physics.Raycast (ray, out hit,1000))
			{
				BoxCollider box = hit.collider as BoxCollider;

				if (box != null)
				{
					Vector3 objPos = hit.transform.position;
					Vector3 objExtent = hit.transform.GetComponent<BoxCollider>().size;
					Vector3 blockExtent = blockList[(int)blockType].GetComponent<BoxCollider>().size;
					Vector3 offset = Vector3.zero;
					Vector3 spawnPos = Vector3.zero;

					if (hit.normal == Vector3.up || hit.normal == Vector3.down)
					{
						offset = new Vector3 (0,objExtent.y * hit.normal.y,0);
						spawnPos = objPos + offset + new Vector3 (0,blockExtent.y* hit.normal.y,0);
					}
					else if (hit.normal == Vector3.forward || hit.normal == Vector3.back)
					{
						offset = new Vector3 (0,0,objExtent.z * hit.normal.z);
						Debug.Log ("offset : "+ offset);
						spawnPos = objPos + offset + new Vector3 (0,0,blockExtent.z* hit.normal.z);
					}
					else if (hit.normal == Vector3.left || hit.normal == Vector3.right)
					{
						offset = new Vector3 (objExtent.x * hit.normal.x,0,0);
						spawnPos = objPos + offset + new Vector3 (blockExtent.x* hit.normal.x,0,0);
					}

					GameObject obj = Instantiate(blockList[(int)blockType],spawnPos,blockList[(int)blockType].transform.rotation);
					AddToLevel(obj);
				}
				else
				{
					Debug.Log("No box colliders!");
				}
			}
		}

		public void DestroyBlock(Ray ray)
		{
			RaycastHit hit;

			if (Physics.Raycast (ray, out hit,1000))
			{
				BoxCollider box = hit.collider as BoxCollider;

				if (box != null)
				{
					GameObject.DestroyImmediate(hit.transform.gameObject);
				}
				else
				{
					Debug.Log("No box colliders!");
				}
			}
		}

		public void RotateBLock(Ray ray)
		{
			RaycastHit hit;

			if (Physics.Raycast (ray, out hit, 1000))
			{
				BoxCollider box = hit.collider as BoxCollider;

				if (box != null)
				{
					Debug.Log ("Zap");
					int rotation = 0;
					rotation = (int)hit.transform.localRotation.eulerAngles.y + 90;
					Debug.Log ("Self Rotation : " + hit.transform.localRotation);
					Debug.Log ("Target Rotation : " + rotation);
					if (rotation >= 360)
					{
						rotation = 0;
					}

					hit.transform.localRotation = Quaternion.Euler(new Vector3(0,rotation,0));
				}
				else
				{
					Debug.Log("No box colliders!");
				}
			}
		}

		private void DeleteIfOccupied(Vector3 pos)
		{
			Collider[] hitCollider = Physics.OverlapSphere (pos, 0.01f);

			Debug.Log ("Hit count : "+ hitCollider.Length);

			if (hitCollider.Length > 0)
			{
				for (int i = 0; i < hitCollider.Length; i++)
				{
					GameObject.DestroyImmediate(hitCollider[i].gameObject);
				}
			}
		}

		private void AddToLevel(GameObject block)
		{
			block.transform.parent = level;
		}

	}
}
