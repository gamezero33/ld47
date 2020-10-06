using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{

	[SerializeField] private Deck aDeck;
	[SerializeField] private Deck bDeck;
	public RecordData[] loops;

	public bool upgraded = false;

	private int level = 1;
	public int Level {
		get {
			return level;
		}
		set {
			level = Mathf.Min(loops.Length - 1, value);
		}
	}

	public RecordData GetLoop (int deckIndex) {
		RecordData data = loops[Random.Range(0, Mathf.Min(loops.Length - 1, level + 1))];
		int iterations = 0;
		if (upgraded) {
			data = loops[level];
			upgraded = false;
		} else if (deckIndex == 0) {
			while (data == bDeck.recordData && iterations++ < 1000)
				data = loops[Random.Range(0, Mathf.Min(loops.Length - 1, level + 1))];
		} else {
			while (data == aDeck.recordData && iterations++ < 1000)
				data = loops[Random.Range(0, Mathf.Min(loops.Length - 1, level + 1))];
		}
		return data;
	}

}
