using UnityEngine.Audio;
using System;
using UnityEngine;
/** Author: Sebastián Jiménez Fernández.
 * Class for controlling the eaudio in the game.
 * */
public class AudioManager : MonoBehaviour
{

	public static AudioManager instance;
	public static AudioManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = FindObjectOfType<AudioManager>();
				if (instance == null)
				{
					instance = new GameObject("AudioManager").AddComponent<AudioManager>();
				}
			}
			return instance;
		}
	}

	public AudioMixerGroup mixerGroup;

	public Sound[] sounds;

	void Awake()
	{

		if (instance != null && instance != this)
		{
			Destroy(this.gameObject);
		}
		else
		{
			DontDestroyOnLoad(this.gameObject);
			instance = this;
		}

		foreach (Sound s in sounds)
		{
			s.source = gameObject.AddComponent<AudioSource>();
			s.source.clip = s.clip;
			s.source.loop = s.loop;

			s.source.outputAudioMixerGroup = mixerGroup;
		}
	}

	//Recibe el nombre del sonido y lo reproduce.
    public void Play(string sound)
	{
		Sound s = Array.Find(sounds, item => item.name == sound);
		if (s == null)
		{
			Debug.LogWarning("Sound: " + name + " not found!");
			return;
		}

		s.source.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
		s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));

		s.source.Play();
	}

	//Recibe el nombre del sonido y lo reproduce directamente, pudiendo sonar varias veces(disparos por ejemplo).
	public void PlayOneShot(string sound)
	{
		Sound s = Array.Find(sounds, item => item.name == sound);
		if (s == null)
		{
			Debug.LogWarning("Sound: " + name + " not found!");
			return;
		}

		s.source.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
		s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));

		s.source.PlayOneShot(s.clip);
	}

	//Recibe el nombre del sonido y lo reproduce con un tiempo de delay.
	public void PlayDelayed(string sound, float time)
	{
		Sound s = Array.Find(sounds, item => item.name == sound);
		if (s == null)
		{
			Debug.LogWarning("Sound: " + name + " not found!");
			return;
		}

		s.source.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
		s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));

		s.source.PlayDelayed(time);
	}

	//Recibe el nombre del sonido y lo reproduce solo si no está sonando ya.
	public void PlayOneAtTime(string sound)
	{
		Sound s = Array.Find(sounds, item => item.name == sound);
		if (s == null)
		{
			Debug.LogWarning("Sound: " + name + " not found!");
			return;
		}

		s.source.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
		s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));

		if (!s.source.isPlaying)
			s.source.Play();
	}

	//Recibe el sonido y lo para.
	public void Stop(string sound)
    {
		Sound s = Array.Find(sounds, item => item.name == sound);
		if (s == null)
		{
			Debug.LogWarning("Sound: " + name + " not found!");
			return;
		}

		if (s.source.isPlaying)
			s.source.Stop();
	}
	//Para todos los sonidos que estén sonando.
	public void StopAll()
	{
        foreach (var sound in sounds)
		{
			if (sound.source.isPlaying)
				sound.source.Stop();
		}
	}

	#region Methods with volume parameter.

	//Recibe el nombre del sonido y lo reproduce.
	public void Play(string sound, float volume)
	{
		Sound s = Array.Find(sounds, item => item.name == sound);
		if (s == null)
		{
			Debug.LogWarning("Sound: " + name + " not found!");
			return;
		}

		s.source.volume = volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
		s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));

		s.source.Play();
	}

	public void PlayOneShot(string sound, float volume)
	{
		Sound s = Array.Find(sounds, item => item.name == sound);
		if (s == null)
		{
			Debug.LogWarning("Sound: " + name + " not found!");
			return;
		}

		s.source.volume = volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
		s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));

		s.source.PlayOneShot(s.clip);
	}
	//Recibe el nombre del sonido y lo reproduce solo si no está sonando ya, CON VOLUMEN de parámetro.
	public void PlayOneAtTime(string sound, float volume)
	{
		Sound s = Array.Find(sounds, item => item.name == sound);
		if (s == null)
		{
			Debug.LogWarning("Sound: " + name + " not found!");
			return;
		}

		s.source.volume = volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
		s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));

		if (!s.source.isPlaying)
			s.source.Play();
	}

	//Recibe el nombre del sonido y lo reproduce con un tiempo de delay.
	public void PlayDelayed(string sound, float time, float volume)
	{
		Sound s = Array.Find(sounds, item => item.name == sound);
		if (s == null)
		{
			Debug.LogWarning("Sound: " + name + " not found!");
			return;
		}

		s.source.volume = volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
		s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));

		s.source.PlayDelayed(time);
	}
	#endregion

}
