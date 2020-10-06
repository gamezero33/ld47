using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdDynamics : MonoBehaviour
{

	[System.Serializable]
	public class CrowdDynamicsData {
		public float minHeight = 0.5f;
		public float maxHeight = 0.6f;
		public float speed = 6;
		public float gravity = 2;
	}


	public float satisfaction = 0;

	[SerializeField] private float minHeight = 0.5f;
	[SerializeField] private float maxHeight = 1f;
	[SerializeField] private float speed = 5;
	[SerializeField] private float gravity = 2;

	[SerializeField] private CrowdDynamicsData resting;
	[SerializeField] private CrowdDynamicsData dancing;

	private List<Vector3> startPos;
	private List<Vector3> jumpPos;
	private List<bool> down;
	private List<Vector3> velocity;



	public void StopDancing () {

		minHeight = resting.minHeight;
		maxHeight = resting.maxHeight;
		speed = resting.speed;
		gravity = resting.gravity;
	}

	public void StartDancing () {

		minHeight = dancing.minHeight;
		maxHeight = dancing.maxHeight;
		speed = dancing.speed;
		gravity = dancing.gravity;
	}

	

	private void Start () {
		startPos = new List<Vector3>();
		jumpPos = new List<Vector3>();
		down = new List<bool>();
		velocity = new List<Vector3>();
		foreach (Transform child in transform) {
			startPos.Add(child.position);
			jumpPos.Add(Vector3.up * Random.Range(minHeight, maxHeight) + child.position);
			down.Add(false);
		}
	}

	private void Update () {

		int i = 0;
		float delta = Time.deltaTime * speed;
		foreach (Transform child in transform) {
			child.position = Vector3.Lerp(child.position, down[i]?startPos[i]:jumpPos[i], delta * (down[i]?gravity:1));
			if (down[i]) {
				if (child.position.y < startPos[i].y + 0.1f) down[i] = false;
			} else {
				if (child.position.y > jumpPos[i].y - 0.1f) {
					down[i] = true;
					jumpPos[i] = Vector3.up * Random.Range(minHeight, maxHeight) + startPos[i];
					if (maxHeight > 0.5f)
						satisfaction += 0.002f;
					//else
						//satisfaction = Mathf.Max(0, satisfaction - 0.005f);
				}
			}
			i++;
		}
	}

}
