using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Treaxen
{
	public class Calculation
	{
		public static float CeilToTenth (float value)
		{
			value = value * 10;
			value = Mathf.CeilToInt (value);
			value = value * 0.1f;

			return value;
		}
	}

}