using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

	[SerializeField] private TMPro.TextMeshProUGUI crowdDisplay;
	[SerializeField] private AudioManager audioManager;
	[SerializeField] private DataManager dataManager;

	[SerializeField] private Deck aDeck;
	[SerializeField] private Deck bDeck;
	[SerializeField] private CrowdDynamics aCrowd;
	[SerializeField] private CrowdDynamics bCrowd;

	[SerializeField] private GameObject strobeLight;

	[SerializeField] private Button aDeckEventButton;
	[SerializeField] private Button bDeckEventButton;

	[SerializeField] private Sprite changeRecordSprite;

	[SerializeField] private Button upgradeButton;
	[SerializeField] private TMPro.TextMeshProUGUI ugBtnText;

	private Tweener[] aDeckTweeners;
	private Tweener[] bDeckTweeners;

	private EventData aDeckEvent;
	private EventData bDeckEvent;

	private bool[] showing = new bool[2];

	private int currency = 0;
	private int targetCurrency = 0;

	private void Awake () {
		aDeckTweeners = aDeckEventButton.GetComponents<Tweener>();
		bDeckTweeners = bDeckEventButton.GetComponents<Tweener>();
	}


	private void Update () {
		targetCurrency = Mathf.FloorToInt(aCrowd.satisfaction + bCrowd.satisfaction);
		if (currency > targetCurrency) currency--;
		if (currency < targetCurrency) currency++;
		crowdDisplay.text = string.Format("${0}", currency);

		if (targetCurrency > 100 && !upgradeButton.gameObject.activeSelf && dataManager.Level < dataManager.loops.Length - 1)
			ShowUpgradeButton();
	}


	public void ShowEventButton (int deckIndex, EventData eventData) {
		if (!showing[deckIndex]) {
			switch (deckIndex) {
				case 0:
					foreach (Tweener tweener in aDeckTweeners)
						tweener.Play();
					aDeckEventButton.image.sprite = eventData.icon;
					aDeckEvent = eventData;
					break;
				case 1:
					foreach (Tweener tweener in bDeckTweeners)
						tweener.Play();
					bDeckEventButton.image.sprite = eventData.icon;
					bDeckEvent = eventData;
					break;
			}
			showing[deckIndex] = true;
		}
	}

	public void HideEventButton (int deckIndex) {
		if (showing[deckIndex]) {
			switch (deckIndex) {
				case 0:
					foreach (Tweener tweener in aDeckTweeners)
						tweener.PlayReverse();
					aDeckEvent = null;
					break;
				case 1:
					foreach (Tweener tweener in bDeckTweeners)
						tweener.PlayReverse();
					bDeckEvent = null;
					break;
			}
			showing[deckIndex] = false;
		}
	}

	public void ShowRecordChange (int deckIndex) {
		if (!showing[deckIndex]) {
			switch (deckIndex) {
				case 0:
					foreach (Tweener tweener in aDeckTweeners)
						tweener.Play();
					aDeckEventButton.image.sprite = changeRecordSprite;
					break;
				case 1:
					foreach (Tweener tweener in bDeckTweeners)
						tweener.Play();
					bDeckEventButton.image.sprite = changeRecordSprite;
					break;
			}
			showing[deckIndex] = true;
		}
	}


	public void OnClick (int deckIndex) {
		bool dismiss = false;
		switch (deckIndex) {
			case 0:
				if (aDeckEventButton.image.sprite == changeRecordSprite) {
					aDeck.ChangeRecord();
					dismiss = true;
				}
				if (aDeckEvent?.audioSource) {
					audioManager.Play(aDeckEvent.audioSource, true, 0, 0, aDeckEvent.sync, aDeckEvent.pause);
					dismiss = true;
				}
				if (aDeckEvent && aDeckEvent.enableFX) {
					strobeLight.SetActive(true);
				}
				if (aDeckEvent) aCrowd.satisfaction += aDeckEvent.reward;
				if (dismiss) HideEventButton(deckIndex);
				break;
			case 1:
				if (bDeckEventButton.image.sprite == changeRecordSprite) {
					bDeck.ChangeRecord();
					dismiss = true;
				}
				if (bDeckEvent?.audioSource) {
					audioManager.Play(bDeckEvent.audioSource, true, 0, 1, bDeckEvent.sync, bDeckEvent.pause);
					dismiss = true;
				}
				if (bDeckEvent && bDeckEvent.enableFX) {
					strobeLight.SetActive(true);
				}
				if (bDeckEvent) bCrowd.satisfaction += bDeckEvent.reward;
				if (dismiss) HideEventButton(deckIndex);
				break;
		}
	}

	public void ShowUpgradeButton () {
		upgradeButton.gameObject.SetActive(true);
	}

	public void HideUpgradeButton () {
		upgradeButton.gameObject.SetActive(false);
	}

	public void Upgrade () {
		if (currency > 100) {
			targetCurrency -= 100;
			aCrowd.satisfaction -= 50;
			bCrowd.satisfaction -= 50;
			dataManager.Level++;
			dataManager.upgraded = true;
			HideUpgradeButton();
		}
	}

}
