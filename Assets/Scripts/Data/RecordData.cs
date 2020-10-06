using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class RecordData : ScriptableObject
{

	public Sprite icon;
	[ColorUsage(false)] public Color color;
	public AudioSource audioSourcePrefab;
	public int loops = 16;
	public float eventSensitivity = 0.1f;

	public EventData[] events = new EventData[8];

}
