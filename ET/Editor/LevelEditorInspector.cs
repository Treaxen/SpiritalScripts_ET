using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Treaxen
{
	[CustomEditor(typeof(LevelEditor))]
	public class LevelEditorInspector : Editor 
	{
		private static bool tilingModeOn = false;

		public override void OnInspectorGUI()
		{
			LevelEditor editor = (LevelEditor)target;

			DrawDefaultInspector();
		
			if (GUILayout.Button("Add Object"))
			{
				editor.CreateObject();
			}
		}
			
		private void OnSceneGUI()
		{
			LevelEditor editor = (LevelEditor)target;

			if (tilingModeOn)
			{
				HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
				switch (Event.current.type)
				{
				case EventType.MouseDown:
					{
						if (Event.current.button == 1)
						{
							Vector2 guiPos = Event.current.mousePosition;
							Ray ray = HandleUtility.GUIPointToWorldRay(guiPos);
							editor.PlaceBlock(ray);
						}

						else if (Event.current.button == 0)
						{
							Vector2 guiPos = Event.current.mousePosition;
							Ray ray = HandleUtility.GUIPointToWorldRay(guiPos);
							editor.DestroyBlock(ray);
						}

						else if (Event.current.button == 2)
						{
							Vector2 guiPos = Event.current.mousePosition;
							Ray ray = HandleUtility.GUIPointToWorldRay(guiPos);
							editor.RotateBLock(ray);
						}
						Event.current.Use();
					}
					break;
				}
			}

			if (Event.current.type == EventType.KeyDown)
			{
				if (Event.current.keyCode == KeyCode.Space)
				{
					Debug.Log ("Tiling Enabled");
					tilingModeOn = true;
				}
			}

			if (Event.current.type == EventType.KeyUp)
			{
				if (Event.current.keyCode == KeyCode.Space)
				{
					Debug.Log ("Tiling Disabled");
					tilingModeOn = false;
				}
			}
		}
	}

}