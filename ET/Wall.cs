using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Treaxen
{
	public class Wall : Block 
	{
		private void Start()
		{
			isInteractable = false;
			isHighlightable = false;
			entityType = EntityType.EntityType_Neutral;
		}
	}

}