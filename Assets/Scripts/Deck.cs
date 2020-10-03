using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Deck : MonoBehaviour {

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
	[SerializeField] private AudioSource[] scratchClips;
	[SerializeField] private Vector2 scratchTiming = new Vector2(0, 2);
	[SerializeField, ColorUsage(false)] private Color[] recordColors;
	[SerializeField] private Gradient progressColors;
	[SerializeField] private float progressAlpha = 0.3f;
	[SerializeField] private Image[] eventNodes;


	private float turntableVelocity = 0;
	private float scratchTime = 0;
	private float scratchTimer = 0;


	private IEnumerator moveArmToStart (float delay = 0) {
		
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
		
		float ft = 1f / recordLength;
		float fr = 360f / revSpeed * recordLength * ft;
		float r = 0;
		float t = 0;
		while (true) {
			t += Time.deltaTime * ft;
			r = Mathf.SmoothDamp(r, fr, ref turntableVelocity, turntableAccel);
			turntable.Rotate(Vector3.up, r * Time.deltaTime);
			armJoint.eulerAngles = Vector3.Lerp(startEulers, completeEulers, t);
			recordProgress.fillAmount = t;
			recordProgress.color = progressColors.Evaluate(t).ToAlpha(progressAlpha);
			if (t > 1) {
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
		if (turntableVelocity == 0) {
			changeRecord();
			StartCoroutine(moveArmToStart(1));
			yield break;
		}
		//GnarlyScratch();
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

	public void ChangeRecord () {

		StopAllCoroutines();
		StartCoroutine(stopRecord());
	}

	private void changeRecord () {

		turntable.localEulerAngles = Vector3.zero;
		recordA.localEulerAngles = Vector3.zero;
		animator.SetTrigger("Change Record");
	}

	public void ChangeRecordColor () {

		recordProgress.fillAmount = 0;
		recordBLabel.material.color = recordALabel.material.color;
		recordALabel.material.color = recordColors.Random().ToAlpha(1);
	}


	public void GnarlyScratch() {

		scratchClips.Random().Play();
	}

}
