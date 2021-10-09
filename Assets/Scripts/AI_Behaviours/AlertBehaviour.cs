using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{

	public class AlertBehaviour : MonoBehaviour {


		public int indexBehaviour; // index of the behaviour
		public bool lookAtPOI;
		public float delayTillNewBehaviour = 3;	// time taken before changing the state
		float _timerTillNewBehaviour;
		Quaternion targetRot;

		public List<WaypointsBase> onAlertExtraBehaviours = new List<WaypointsBase> ();
		public string[] alertLogic;		// what he is going to do on alert

		EnemyAI enAI_main;

		// Use this for initialization
		void Start () 
		{
			enAI_main = GetComponent<EnemyAI> ();
		}

		public void AlertBehaviourMain()
		{
			if (!lookAtPOI) // if not looking at point of interest
			{
				Vector3 directionToLookTo = enAI_main. pointOfInterest - transform.position;
				directionToLookTo.y = 0;

				float angle = Vector3.Angle (transform.forward, directionToLookTo);
				if (angle > 0.1f) 
				{
					targetRot = Quaternion.LookRotation (directionToLookTo);
					transform.localRotation = Quaternion.Slerp (transform.localRotation, targetRot, Time.deltaTime);

				} 
				else 
				{
					lookAtPOI = true; 	// looking at point of interest
				}
			}

			_timerTillNewBehaviour += Time.deltaTime;

			if (_timerTillNewBehaviour > delayTillNewBehaviour) 
			{
				if (alertLogic.Length > 0) 
				{
					enAI_main.ChangeAIBehaviour (alertLogic [0], 0);
				}
				_timerTillNewBehaviour = 0;
			}
		}

		public void OnAlertExtraBehaviours()  // almost same as patrol behaviour
		{
			if (onAlertExtraBehaviours.Count > 0) 
			{
				WaypointsBase curBehaviour = onAlertExtraBehaviours [indexBehaviour];

				if (!enAI_main.goToPos)
				{ // if we dont have a position to go to
					enAI_main.charStats.MoveToPosition (curBehaviour.targetDestination.position); // we are setting a position to go to
					enAI_main.goToPos = true;
				}
				else
				{
					float distanceToTarget = Vector3.Distance (transform.position, curBehaviour.targetDestination.position);
					 
					if (distanceToTarget < enAI_main.plControl.stopDistance) 
					{
						enAI_main.commonBehaviour.CheckWaypoint (curBehaviour, 1);
						//CheckWaypoint (curBehaviour, 1); 	// 1 means onAlertExtraBehaviour list
					}
				}
			}

		} 
	}

}
