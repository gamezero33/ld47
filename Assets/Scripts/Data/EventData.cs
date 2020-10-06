using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EventData : ScriptableObject {

	public Sprite icon;
	public AudioSource audioSource;
	public bool enableFX;
	public float reward = 10;
	public float pause = 0;
	public bool sync;

}
