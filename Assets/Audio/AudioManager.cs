using UnityEngine.Audio;
using System;
using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{

	public static AudioManager instance;

	public AudioMixerGroup mixerGroup;

	public Sound[] sounds;


	public float targetVolume { get; private set; }
	IEnumerator volumeTransition;

	void Awake()
	{
		if (instance != null)
		{
			Destroy(gameObject);
		}
		else
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}

		foreach (Sound s in sounds)
		{
			s.source = gameObject.AddComponent<AudioSource>();
			s.source.clip = s.clip;
			s.source.loop = s.loop;

			s.source.outputAudioMixerGroup = mixerGroup;
		}

		//targetVolume = 
	}

	public void Play(string sound)
	{
		Sound s = Array.Find(sounds, item => item.name == sound);
		if (s == null)
		{
			Debug.LogWarning("Sound: " + name + " not found!");
			return;
		}

        if (!s.source.isPlaying)
        {
			s.source.Play();
		}
		VolumeTransition(sound, 1, 0.5f);
	}
    public void StopPlaying(string sound)
    {
        Sound s = Array.Find(sounds, item => item.name == sound);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

		if (s.source.isPlaying && s.volume == 0)
		{
			s.source.Stop();
		}
        else
        {
            VolumeTransition(sound, 0, 0.5f);
		}
	}

	public void VolumeTransition(string sound, float target, float duration)
	{
		// set new target pitch
		//targetVolume = target;
		// stop any pitch transition currently running
		if (volumeTransition != null)
		{
			StopCoroutine(volumeTransition);
		}
		// create and start a new transition
		volumeTransition = VolumeTransitionCoroutine(sound, target, duration);
		StartCoroutine(volumeTransition);
	}

	IEnumerator VolumeTransitionCoroutine(string sound, float target, float duration)
	{
		Sound s = Array.Find(sounds, item => item.name == sound);
		if (s == null)
		{
			Debug.LogWarning("Sound: " + name + " not found!");
		}

		// starting pitch of the audio
		float from = s.source.volume;
		// "counter" variable to track position within Lerp
		float progress = 0.0f;
		// loop until we reach the end of the linear interpolation
		while (progress < 1.0)
		{
			// increase the "counter" by the fraction of duration
			progress += Time.unscaledDeltaTime / duration;

			// linear interpolation from starting to target pitch
			s.source.volume = Mathf.Lerp(from, target, progress);
			// yield control back to the program
			yield return null;
		}
	}
}
