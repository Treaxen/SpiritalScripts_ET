/*
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Andrew;

namespace Treaxen
{
	public class Floor : Entity 
	{
		public override void OnInteract()
		{
			//TapCountScript.Instance.AddCount();
			Player.instance.MoveToPoint(this.transform.position);
		}
	}
}