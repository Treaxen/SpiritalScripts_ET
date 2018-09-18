using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Treaxen
{
	public class Entity : MonoBehaviour 
	{
		public EntityType entityType = 0;

//		public virtual void InitializeItem(){} //to be called on start if any of the variables differ from the default

		#region Interaction
		public bool isInteractable = true;

		public virtual void OnInteract()
		{
			if(!isInteractable)
				return;
		}
		#endregion

		#region Highlight Function
		public bool isHighlightable = true;
		public bool CompareIfSameType(EntityType type)
		{
			if (type == entityType)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		#endregion
	}
}
