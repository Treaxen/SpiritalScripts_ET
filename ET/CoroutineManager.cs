using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Treaxen
{
	public class CoroutineManager : MonoBehaviour 
	{
		private static CoroutineManager _instance = null;
		public static CoroutineManager instance
		{
			get
			{
				if (!_instance)
				{
					//see if the instance exists, if not create it
					_instance = FindObjectOfType (typeof(CoroutineManager)) as CoroutineManager;

					if (!_instance)
					{
						GameObject obj = new GameObject ("CoroutineManager");
						_instance = obj.AddComponent<CoroutineManager> ();
					}
				}

				return _instance;
			}
		}

		void OnApplicationQuit()
		{
			_instance = null;
		}
	}

	public class Task
	{
		public event System.Action<bool> taskComplete;

		public bool running;
		public bool paused;

		private IEnumerator _coroutine;
		private bool _taskEnded;

		//constructor
		public Task(IEnumerator coroutine, bool shouldStart)
		{
			_coroutine = coroutine;

			if (shouldStart)
				StartTask();
		}

		//creating a task
		public static Task make (IEnumerator coroutine, bool shouldStart)
		{
			return new Task (coroutine, shouldStart);
		}

		public void StartTask()
		{
			running = true;
			CoroutineManager.instance.StartCoroutine (DoWork ());
		}

		public void Pause()
		{
			paused = true;
		}

		public void Unpause()
		{
			paused = false;
		}

		public void EndTask()
		{
			_taskEnded = true;
			running = false;
			paused = false;
		}

		private IEnumerator DoWork()
		{
			//incase the coroutine was started at a paused state
			yield return null;

			while (running)
			{
				if (paused)
				{
					yield return null;
				}
				else
				{
					//run the next iteration of the coroutine and exit if done
					if (_coroutine.MoveNext ())
					{
						yield return _coroutine.Current;
					}
					else
					{
						running = false;
					}
				}

				if (taskComplete != null)
				{
					taskComplete (_taskEnded);
				}

			}

			CoroutineManager.instance.StopCoroutine(DoWork());
		}
	}

}

