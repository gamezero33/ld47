using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	public AudioListener listener;

	public AudioSource[] gnarlyScratches;
	public AudioSource[] ambience;

	[SerializeField] private float destroyDelay = 1;
	private List<AudioSource> playingSources;
	private AudioSource aDeckSource;
	private AudioSource bDeckSource;

	private List<AudioSource> ambientSources;
	private float ambienceTimer = 0;
	private float nextAmbienceChange = 0;
	private bool playingAmbience = true;

	public AudioSource Play (AudioSource audioSource, bool instantiate=true, int loops=0, int deck=-1, bool sync=false, float pause=0) {
		if (playingSources == null) playingSources = new List<AudioSource>();
		if (instantiate) {
			AudioSource source = Instantiate(audioSource, transform);
			playingSources.Add(source);
		} else {
			audioSource.transform.SetParent(transform);
			playingSources.Add(audioSource);
		}

		AudioSource lastSrc = playingSources.Last();

		lastSrc.Play();

		if (pause > 0) {
			aDeckSource?.Pause();
			bDeckSource?.Pause();
			Invoke("Resume", pause);
		}

		if (deck == 0) {
			if (sync) {
				if (aDeckSource != null) lastSrc.timeSamples = SyncSamples(lastSrc, aDeckSource);
			}
			if (loops > 0) {
				aDeckSource = lastSrc;
				if (sync) {
					if (bDeckSource) aDeckSource.timeSamples = bDeckSource.timeSamples;
				}
			}
		}
		if (deck == 1) {
			if (sync) {
				if (bDeckSource != null) lastSrc.timeSamples = SyncSamples(lastSrc, bDeckSource);
			}
			if (loops > 0) {
				bDeckSource = lastSrc;
				if (sync) {
					if (aDeckSource) bDeckSource.timeSamples = aDeckSource.timeSamples;
				}
			}
		}
		
		if (!lastSrc.loop)
			StartCoroutine(DestroySource(lastSrc, lastSrc.clip.length + destroyDelay));

		return lastSrc;
	}
	
	private void Resume () {
		aDeckSource?.Play();
		bDeckSource?.Play();
	}
	
	private int SyncSamples (AudioSource sourceA, AudioSource sourceB) {
		return sourceA.timeSamples > 0 ? sourceB.timeSamples % sourceA.timeSamples : 0;
	}

	public void Stop (AudioSource source) {
		StartCoroutine(FadeSource(source, 2));
	}

	public void PlayGnarlyScratch () {
		Play(gnarlyScratches.Random());
	}

	public void PlayAmbience () {
		if (ambientSources == null) ambientSources = new List<AudioSource>();
		playingAmbience = true;
	}

	private void Update () {
		if (playingAmbience) {
			if (ambientSources == null) ambientSources = new List<AudioSource>();
			if (ambienceTimer > nextAmbienceChange) {
				AudioSource source = ambience.Random();
				ambientSources.Add(Play(source));
				nextAmbienceChange = Random.Range(0, source.clip.length / 2);
				ambienceTimer = 0;
			} else {
				ambienceTimer += Time.deltaTime;
			}
		}
	}

	public void StopAmbience () {
		playingAmbience = false;
		foreach (AudioSource source in ambientSources) {
			StartCoroutine(FadeSource(source, 5));
		}
	}

	private IEnumerator FadeSource (AudioSource source, float time, bool destroy=true) {
		if (source == null) yield break;
		float startVol = source.volume;
		float f = 1f / time;
		float t = 0;
		while (t < 1) {
			t += Time.deltaTime * f;
			source.volume = Mathf.Lerp(startVol, 0, t);
			yield return null;
		}
		ambientSources.Remove(source);
		if (destroy)
			DestroySource(source, 0.1f);
	}

	private IEnumerator DestroySource (AudioSource source, float delay) {
		
		yield return new WaitForSeconds(delay);
		if (source == null) yield break;
		if (playingSources.Contains(source)) playingSources.Remove(source);
		if (source.gameObject) Destroy(source.gameObject);
	}
	
}
