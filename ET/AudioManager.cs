/*
Audio Manager
by Ng E-Tjing

To add a new audio clip, do the following (Do this to the Audio Manager in the hierachy, not the project folder):
1. Change the array size of Audio Clip Info List to current size + 1 (eg: if the size is one, change it to 2) at the inspector
2. Enter the Name of the clip (this is just for easy recognition)
3. Come to this script, add a new value to the AudioClipID enum type, BGM has a range of 0-99, and from there onwards would be SFX ID
4. At the inspector again, select the appropriate AudioClipID from the dropdown
5. Drag the inteded audio clip into the "Audio Clip" field
6. Set the wait instance, wait instances are basically how many audio sources that would be on standby to play the audio, by default set it as 1
7. After all audio files are added, press Reinitialize Audio.
8. Apply prefab

To play whatever, run the play functions (Prefixed with "Play") and pass in the parameters of the enum value
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Treaxen
{
	public enum AudioClipID
	{
		BGM_MainMenu = 0,
		BGM_Gameplay = 1,

		SFX_Impact = 101,
		SFX_DoorOpen = 102,
		SFX_ButtonPressed = 103,
		SFX_QuickSand = 104,
		SFX_SqueezeThrough = 105,

		TOTAL = 9001
	}

	[System.Serializable]
	public class AudioClipInfo
	{
		public string clipName;
		public AudioClipID audioClipID;
		public AudioClip audioClip;
		public int waitInstance = 1; //number of audio sources to be put on standby
		public List<AudioSource> audioSource = new List<AudioSource>(); //audio Sources to play clip
	}

	public class AudioManager : MonoBehaviour
	{
		private static AudioManager mInstance;

		public static AudioManager Instance
		{
			get
			{
				if(mInstance == null)
				{
					if(GameObject.FindObjectOfType<AudioManager>() != null)
					{
						mInstance = GameObject.FindObjectOfType<AudioManager>();
					}
					else 
					{
						GameObject obj = (GameObject)Instantiate(Resources.Load("Prefabs/AudioManager", typeof (GameObject)));
						mInstance = obj.GetComponent<AudioManager>();
					}
					DontDestroyOnLoad(mInstance.gameObject);
				}
				return mInstance;
			}
		}
		public static bool CheckInstanceExist()
		{
			return mInstance;
		}

		private float bgmVolume = 1.0f;
		private float sfxVolume = 1.0f;

		public List<AudioClipInfo> audioClipInfoList = new List<AudioClipInfo>();

		[SerializeField] private List<AudioSource> bgmAudioSource = new List<AudioSource>();
		[SerializeField] private List<AudioSource> sfxAudioSource = new List<AudioSource>();

		public float BgmVolume { get{return bgmVolume;} set{bgmVolume = value;}}
		public float SfxVolume { get{return sfxVolume;} set{sfxVolume = value;}}

		#region Utilities
		void Awake () 
		{
			/*if(SoundManager.CheckInstanceExist())
			{
				Destroy(this.gameObject);
			}*/
		}

		void Start()
		{
			//InitializeAudioSource();
			DontDestroyOnLoad(this.gameObject);
		}

		AudioClipInfo FindAudioClip(AudioClipID audioClipID)
		{
			for(int i=0; i<audioClipInfoList.Count; i++)
			{
				if(audioClipInfoList[i].audioClipID == audioClipID)
				{
					return audioClipInfoList[i];
				}
			}

			Debug.LogError("Cannot Find Audio Clip : " + audioClipID);

			return null;
		}

		AudioSource FindReadyAudioSource(AudioClipInfo audioClipInfo)
		{
			for (int i= 0; i< audioClipInfo.audioSource.Count;i++)
			{
				if (!audioClipInfo.audioSource[i].isPlaying)
				{
					Debug.Log("Found");
					return audioClipInfo.audioSource[i];
				}
			}

			Debug.Log("No Ready Audio Sources");
			return null;
		}

		public void ResetAudioSource()
		{
			int count = transform.childCount;

			for (int i = 0; i<count; i++)
			{
				if (Application.isEditor)
					DestroyImmediate(transform.GetChild(0).gameObject);
				else
					Destroy(transform.GetChild(0).gameObject);
			}

			for (int i = 0; i < audioClipInfoList.Count; i++)
			{
				audioClipInfoList[i].audioSource.Clear();
			}

			for (int i = 0; i < bgmAudioSource.Count; i++)
			{
				bgmAudioSource.Clear();
			}

			for (int i = 0; i < sfxAudioSource.Count; i++)
			{
				sfxAudioSource.Clear();
			}
		}

		public void InitializeAudioSource()
		{
			for (int i = 0; i< audioClipInfoList.Count; i++)
			{
				GameObject obj = new GameObject(audioClipInfoList[i].clipName);
				obj.transform.parent = this.transform;

				for (int j = 0; j < audioClipInfoList[i].waitInstance; j++)
				{
					GameObject ob = new GameObject ("AudioSource");
					ob.transform.parent = obj.transform;

					audioClipInfoList[i].audioSource.Add(ob.AddComponent<AudioSource>());
					ob.GetComponent<AudioSource>().playOnAwake = false;
					ob.GetComponent<AudioSource>().clip = audioClipInfoList[i].audioClip;

					if ((int)audioClipInfoList[i].audioClipID < 100)
					{
						bgmAudioSource.Add(ob.GetComponent<AudioSource>());
						Debug.Log ("Add to bgmList, track count : " + bgmAudioSource.Count);
					}
					else
					{
						sfxAudioSource.Add(ob.GetComponent<AudioSource>());
					}
				}
			}
		}

		#endregion

		#region Background Music
		public void PlayBGM(AudioClipID audioClipID)
		{
			AudioClipInfo clipToPlay = FindAudioClip(audioClipID);

			if(clipToPlay == null)
			{
				return;
			}

			AudioSource player = FindReadyAudioSource(clipToPlay);

			if (player != null)
			{
				player.volume = bgmVolume;
				player.loop = true;
				player.Play();
			}
		}

		public void PlayBGMWithFadeIn(AudioClipID audioClipID, float fadeInDuration)
		{
			AudioClipInfo clipToPlay = FindAudioClip(audioClipID);

			if(clipToPlay == null)
			{
				return;
			}

			AudioSource player = FindReadyAudioSource(clipToPlay);

			if (player != null)
			{
				player.loop = true;
				StartCoroutine(FadeIn(player, fadeInDuration, bgmVolume));
			}
		}

		public void PlayBGMWithFadeOutIn(AudioClipID audioClipID, float fadeOutDuration, float fadeInDuration)
		{
			AudioClipInfo clipToPlay = FindAudioClip(audioClipID);

			if(clipToPlay == null)
			{
				return;
			}

			AudioSource player = FindReadyAudioSource(clipToPlay);

			if (player != null)
			{
				player.loop = true;
				StartCoroutine(FadeOutIn(player, fadeOutDuration, fadeInDuration, bgmVolume));
			}
		}

		public void PauseAllBGM()
		{
			for (int i = 0; i < bgmAudioSource.Count; i++)
			{
				if (bgmAudioSource[i].isPlaying)
				{
					bgmAudioSource[i].Pause();
				}
			}
		}

		public void StopAllBGM()
		{
			for (int i = 0; i < bgmAudioSource.Count; i++)
			{
				if (bgmAudioSource[i].isPlaying)
				{
					bgmAudioSource[i].Stop();
				}
			}
		}

		public void FadeOutAllBGM(float fadeOutDuration)
		{
			StartCoroutine(FadeOutAll(bgmAudioSource, fadeOutDuration));
		}

		public void ChangeBgmVolume(float value)
		{
			bgmVolume = value;
			Debug.Log("Softer Bgm : " +  bgmAudioSource.Count);
			for (int i = 0; i < bgmAudioSource.Count; i++)
			{
				Debug.Log("Softer Bgm");
				bgmAudioSource[i].volume = value;
			}
		}
		#endregion

		#region Sound Effects
		public void PlaySFX(AudioClipID audioClipID)
		{
			AudioClipInfo clipToPlay = FindAudioClip(audioClipID);

			if (clipToPlay == null)
			{
				return;
			}

			AudioSource player = FindReadyAudioSource(clipToPlay);

			if (player != null)
			{
				player.PlayOneShot(clipToPlay.audioClip, sfxVolume);
			}
		}

		public void FadeOutAllSFX(float fadeOutDuration)
		{
			StartCoroutine(FadeOutAll(sfxAudioSource, fadeOutDuration));
		}

		public void ChangSfxVolume(float value)
		{
			sfxVolume = value;

			for (int i = 0; i < sfxAudioSource.Count; i++)
			{
				sfxAudioSource[i].volume = value;
			}
		}
		#endregion

		#region Coroutines
		IEnumerator FadeIn(AudioSource audioSource, float fadeInDuration, float maxVolume)
		{
			audioSource.volume = 0.0f;
			audioSource.Play();

			float fadeInTimer = 0.0f;
			float fadeInSpeed = maxVolume / fadeInDuration * Time.deltaTime;

			while(fadeInTimer < fadeInDuration)
			{
				fadeInTimer += Time.deltaTime;
				audioSource.volume += fadeInSpeed;
				yield return null;
			}
			audioSource.volume = maxVolume;
		}

		IEnumerator FadeOut(AudioSource audioSource, float fadeOutDuration)
		{
			float fadeOutTimer = 0.0f;
			float fadeOutSpeed = audioSource.volume / fadeOutDuration * Time.deltaTime;

			while(fadeOutTimer < fadeOutDuration)
			{
				fadeOutTimer += Time.deltaTime;
				audioSource.volume -= fadeOutSpeed;
				yield return null;
			}
			audioSource.volume = 0.0f;
			audioSource.Stop();
		}

		IEnumerator FadeOutIn(AudioSource audioSource, float fadeOutDuration, float fadeInDuration, float maxVolume)
		{
			float fadeOutTimer = 0.0f;
			float fadeOutSpeed = audioSource.volume / fadeOutDuration * Time.deltaTime;

			while(fadeOutTimer < fadeOutDuration)
			{
				fadeOutTimer += Time.deltaTime;
				audioSource.volume -= fadeOutSpeed;
				yield return null;
			}
			StartCoroutine(FadeIn(audioSource, fadeInDuration, maxVolume));
		}

		IEnumerator FadeOutAll(List<AudioSource> audioSourceList, float fadeOutDuration)
		{
			float fadeOutTimer = 0.0f;
			List<float> fadeOutSpeedList = new List<float>();

			for(int i=0; i<audioSourceList.Count; i++)
			{
				fadeOutSpeedList.Add(audioSourceList[i].volume / fadeOutDuration * Time.deltaTime);
			}

			while(fadeOutTimer < fadeOutDuration)
			{
				fadeOutTimer += Time.deltaTime;
				for(int i=0; i<audioSourceList.Count; i++)
				{
					audioSourceList[i].volume -= fadeOutSpeedList[i];
				}
				yield return null;
			}
			for(int i=0; i<audioSourceList.Count; i++)
			{
				audioSourceList[i].volume = 0.0f;
				audioSourceList[i].Stop();
			}
		}

		IEnumerator FadeInAll(List<AudioSource> audioSourceList, float fadeInDuration, List<float> maxVolumeList)
		{
			float fadeInTimer = 0.0f;
			List<float> fadeInSpeedList = new List<float>();

			for(int i=0; i<audioSourceList.Count; i++)
			{
				audioSourceList[i].volume = 0.0f;
				audioSourceList[i].Play();
				fadeInSpeedList.Add(maxVolumeList[i] / fadeInDuration * Time.deltaTime);
			}

			while(fadeInTimer < fadeInDuration)
			{
				fadeInTimer += Time.deltaTime;
				for(int i=0; i<audioSourceList.Count; i++)
				{				
					audioSourceList[i].volume += fadeInSpeedList[i];
				}
				yield return null;
			}
			for(int i=0; i<audioSourceList.Count; i++)
			{
				audioSourceList[i].volume = maxVolumeList[i];
			}
		}

		IEnumerator FadeOutInAll(List<AudioSource> audioSourceList, float fadeOutDuration, float fadeInDuration)
		{
			float fadeOutTimer = 0.0f;
			List<float> fadeOutSpeedList = new List<float>();
			List<float> maxVolumeList = new List<float>();

			for(int i=0; i<audioSourceList.Count; i++)
			{
				fadeOutSpeedList.Add(audioSourceList[i].volume / fadeOutDuration * Time.deltaTime);
				maxVolumeList.Add(audioSourceList[i].volume);
			}

			while(fadeOutTimer < fadeOutDuration)
			{
				fadeOutTimer += Time.deltaTime;
				for(int i=0; i<audioSourceList.Count; i++)
				{
					audioSourceList[i].volume -= fadeOutSpeedList[i];
				}
				yield return null;
			}
			StartCoroutine(FadeInAll(audioSourceList, fadeInDuration, maxVolumeList));
		}

		#endregion

	}

}