using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

	public CrowdDynamics crowdA;
	public CrowdDynamics crowdB;
	public AudioManager audioManager;
	public DataManager dataManager;


	private void Start () {
		crowdA.StopDancing();
		crowdB.StopDancing();
	}

	public void StopDancing (int index) {
		switch (index) {
			case 0:
				crowdA.StopDancing();
				break;
			case 1:
				crowdB.StopDancing();
				break;
		}
	}

	public void StartDancing (int index) {
		switch (index) {
			case 0:
				crowdA.StartDancing();
				break;
			case 1:
				crowdB.StartDancing();
				break;
		}
	}

}
