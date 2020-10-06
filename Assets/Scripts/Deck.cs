using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Deck : MonoBehaviour {

	[SerializeField] private GameManager gameManager;
	[SerializeField] private AudioManager audioManager;
	[SerializeField] private DataManager dataManager;
	[SerializeField] private UIManager uiManager;
	[SerializeField] private int index = 0;

	[SerializeField] private Animator animator;
	[SerializeField] private Transform turntable;
	[SerializeField] private Transform armJoint;
	[SerializeField] private Transform recordA;
	[SerializeField] private Renderer recordALabel;
	[SerializeField] private Transform recordB;
	[SerializeField] private Renderer recordBLabel;
	[SerializeField] private Image recordProgress;

	[SerializeField] private Vector3 homeEulers = Vector3.zero;
	[SerializeField] private Vector3 startEulers = Vector3.up * 7;
	[SerializeField] private Vector3 completeEulers = Vector3.up * 45;
	[SerializeField] private AnimationCurve armStartCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
	[SerializeField] private float armStartDuration = 3;
	[SerializeField] private float revSpeed = 33f / 60f;
	[SerializeField] private float recordLength = 10;
	[SerializeField] private float turntableAccel = 0.5f;
	[SerializeField] private Vector2 scratchTiming = new Vector2(0, 2);
	[SerializeField, ColorUsage(false)] private Color[] recordColors;
	[SerializeField] private Gradient progressColors;
	[SerializeField] private float progressAlpha = 0.3f;
	[SerializeField] private Image[] eventNodes;
	[SerializeField] private GameObject[] fx;


	private float turntableVelocity = 0;
	private float scratchTime = 0;
	private float scratchTimer = 0;
	public AudioSource playingAudio;
	public RecordData recordData;


	private const float eighth = 1f / 8f;

	private IEnumerator moveArmToStart (float delay = 0) {

		ActivateFXs();
		yield return new WaitForSeconds(delay);
		float f = 1f / armStartDuration;
		float t = 0;
		Vector3 start = armJoint.eulerAngles;
		while (t < 1) {
			t += Time.deltaTime * f;
			Vector3 eulers = Vector3.Lerp(start, startEulers, t);
			eulers.x = armStartCurve.Evaluate(t);
			armJoint.eulerAngles = eulers;
			yield return null;
		}
		StartCoroutine(spinRecord());
	}

	private IEnumerator spinRecord () {

		recordB.gameObject.SetActive(false);
		gameManager.StartDancing(index);
		recordLength = recordData.audioSourcePrefab.clip.length * recordData.loops;
		playingAudio = audioManager.Play(recordData.audioSourcePrefab, true, recordData.loops, index, true);
		float ft = 1f / recordLength;
		float fr = 360f / revSpeed * recordLength * ft;
		float r = 0;
		float t = 0;
		while (true) {
			t += Time.deltaTime * ft;
			r = Mathf.SmoothDamp(r, fr, ref turntableVelocity, turntableAccel);
			turntable.Rotate(Vector3.up, r * Time.deltaTime);
			armJoint.eulerAngles = Vector3.Lerp(startEulers, completeEulers, t);

			UpdateProgress(t);

			if (t > 1) {
				gameManager.StopDancing(index);
				uiManager.ShowRecordChange(index);
				if (playingAudio) audioManager.Stop(playingAudio);
				if (scratchTimer > scratchTime) {
					GnarlyScratch();
					scratchTimer = 0;
					scratchTime = Random.Range(scratchTiming.x, scratchTiming.y);
				} else {
					scratchTimer += Time.deltaTime;
				}
			}
			yield return null;
		}
	}

	private IEnumerator stopRecord () {
		DeactivateFXs();
		gameManager.StopDancing(index);
		if (turntableVelocity == 0) {
			changeRecord();
			StartCoroutine(moveArmToStart(1));
			yield break;
		}
		if (playingAudio) audioManager.Stop(playingAudio);
		StartCoroutine(moveArmToStart(1.5f));
		float r = 360f / revSpeed * recordLength * (1f / recordLength);
		float t = 0;
		while (t < 1) {
			t += Time.deltaTime;
			r = Mathf.SmoothDamp(r, 0, ref turntableVelocity, turntableAccel);
			turntable.Rotate(Vector3.up, r * Time.deltaTime);
			yield return null;
		}
		changeRecord();
	}


	private void ActivateFXs () {
		foreach (GameObject fxobj in fx) {
			fxobj.SetActive(true);
		}
	}

	private void DeactivateFXs () {
		foreach (GameObject fxobj in fx) {
			fxobj.SetActive(false);
		}
	}


	private void UpdateProgress (float t) {

		recordProgress.color = progressColors.Evaluate(t).ToAlpha(progressAlpha);
		recordProgress.fillAmount = t;

		if (t < 1) {
			for (int i = 0; i < 8; i++) {
				if (t > i * eighth - (recordData.eventSensitivity * eighth) + (eighth / 2) && t < i * eighth + (recordData.eventSensitivity * eighth) + (eighth / 2))
					EventNodeOn(i);
				else
					EventNodeOff(i);
			}
		}
	}
	

	private void EventNodeOn (int nodeIndex) {
		if (eventNodes[nodeIndex].color.a != 1) {
			eventNodes[nodeIndex].color = eventNodes[nodeIndex].color.ToAlpha(1);
			if (recordData.events[nodeIndex])
				uiManager.ShowEventButton(index, recordData.events[nodeIndex]);
		}
	}

	private void EventNodeOff (int nodeIndex) {
		if (eventNodes[nodeIndex].color.a != 0.1f) {
			eventNodes[nodeIndex].color = eventNodes[nodeIndex].color.ToAlpha(0.1f);
			uiManager.HideEventButton(index);
		}
	}
	
	public void ChangeRecord () {

		for (int i = 0; i < eventNodes.Length; i++) {
			EventNodeOff(i);
		}
		uiManager.HideEventButton(index);
		StopAllCoroutines();
		StartCoroutine(stopRecord());
	}

	private void changeRecord () {

		audioManager.StopAmbience();
		turntable.localEulerAngles = Vector3.zero;
		recordA.localEulerAngles = Vector3.zero;
		recordBLabel.material.color = recordALabel.material.color;
		if (recordData != null) recordB.gameObject.SetActive(true);
		animator.SetTrigger("Change Record");
	}

	public void ChangeRecordColor () {

		recordData = dataManager.GetLoop(index);
		recordProgress.fillAmount = 0;
		recordALabel.material.color = recordData.color;// recordColors.Random().ToAlpha(1);
		recordA.gameObject.SetActive(true);
	}


	public void GnarlyScratch() {

		audioManager.PlayGnarlyScratch();
	}

}
