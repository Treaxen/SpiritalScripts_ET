using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Treaxen
{
	[CustomEditor(typeof (AudioManager))]
	public class AudioManagerInspector : Editor 
	{
		public override void OnInspectorGUI()
		{
			AudioManager manager = (AudioManager)target;

			DrawDefaultInspector();

			if (GUILayout.Button("Reinitialize Audio Sources"))
			{
				manager.ResetAudioSource();
				manager.InitializeAudioSource();
			}
		}
	}
}
