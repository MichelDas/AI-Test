using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{

	public class CommonBehaviour : MonoBehaviour {

		EnemyAI enAI_main;

		//Checks for each WP
		[HideInInspector] public bool _initCheck;
		[HideInInspector] public bool _loodAtTarget;
		[HideInInspector] public bool _overrideAnimation;

		//Waypoints main
		public int indexWaypoint;
		public List<WaypointsBase> waypoints = new List<WaypointsBase> ();

		Quaternion targetRot; //Look Rotation

		//Wait time for WP
		public bool circularList;	// are the way points circular
		bool descendingList;		// or descending list	
		float _waitTime;			// standing time in every way point

		// Use this for initialization
		void Start () {
			enAI_main = GetComponent<EnemyAI> ();
		}
			
		public void DecreaseAlerLevels ()
		{
			if (enAI_main.charStats.alertLevel > 0)
			{
				enAI_main.alertTimer += Time.deltaTime * enAI_main.alertMultiplier;

				if (enAI_main.alertTimer > enAI_main.alertTimerIncrement * 2) 
				{
					enAI_main.charStats.alertLevel--;
					enAI_main.alertTimer = 0;
				}
			}

			if ( enAI_main.charStats.alertLevel == 0 )
			{
				if ( enAI_main.aiStates != EnemyAI.AIstates.patrol )
				{
					enAI_main.AI_State_Normal ();
				}
			}
		}

		public void PatrolBehavior()
		{
			if (waypoints.Count > 0)
			{
				WaypointsBase curWayPoint = waypoints [indexWaypoint];

				if (!enAI_main.goToPos)
				{
					enAI_main.charStats.MoveToPosition (curWayPoint.targetDestination.position);
					enAI_main.goToPos = true;
				} 
				else
				{
					float distanceToTarget = Vector3.Distance (transform.position, curWayPoint.targetDestination.position);

					if (distanceToTarget < enAI_main.plControl.stopDistance) 
					{
						CheckWaypoint (curWayPoint, 0);	 // zero means way points list			
					}
				}
			}

		}
		
		public void CheckWaypoint(WaypointsBase wp, int listCase)
		{
			#region InitCheck
			if(!_initCheck)
			{
				_loodAtTarget = wp.lookTowards;
				_overrideAnimation = wp.overrideAnimation;
				_initCheck = true;
			}

			#endregion

			//
			if (!wp.stopList) 
			{
				switch (listCase) 
				{
				case 0:
					WaitTimerForEachWP (wp, waypoints);
					break;
				case 1:
					WaitTimerForExtraBehaviour (wp, enAI_main.alertBehaviour.onAlertExtraBehaviours);
					break;

				}
			}

			#region WaitTime
			_waitTime += Time.deltaTime;

			if(_waitTime > wp.waitTime)
			{
				if(circularList)
				{
					if(waypoints.Count - 1 > indexWaypoint)
					{
						indexWaypoint++;
					}
					else 
					{
						indexWaypoint = 0;
					}
				}
				else
				{
					if(!descendingList)
					{
						if(waypoints.Count - 1 == indexWaypoint)
						{
							descendingList = true;
							indexWaypoint--;
						}	
						else{
							indexWaypoint++;
						}
					}
					else
					{
						if(indexWaypoint > 0)
						{
							indexWaypoint--;
						}
						else
						{
							descendingList = false;
							indexWaypoint++;
						}
					}
				}

				_initCheck = false;
				enAI_main.goToPos = false;
				_waitTime = 0;
			}

			#endregion

			#region LookTowards
			if(_loodAtTarget)
			{
				enAI_main.plControl.moveToPosition = false;

				float speedToRotate;

				if(wp.speedTolook < 0.1f)
				{
					speedToRotate = 2;
				}
				else{
					speedToRotate = wp.speedTolook;
				}

				Vector3 direction = wp.targetToLookTo.position - transform.position;
				direction.y = 0;

				float angle = Vector3.Angle(transform.forward, direction);

				if(angle > 0.1f)
				{
					targetRot = Quaternion.LookRotation(direction);
					transform.localRotation = Quaternion.Slerp(transform.localRotation,targetRot,Time.deltaTime);
				}
				else 
				{
					_loodAtTarget = false;
				}
			}

			#endregion

			#region AnimationOverride
			if(_overrideAnimation)
			{
				if(wp.animationRoutines.Length > 0)
				{
					for(int i=0 ; i< wp.animationRoutines.Length ; i++ )
					{
						enAI_main.charStats.CallFunctionWithString(wp.animationRoutines[i], 0);
					}
				}
				else
				{
					Debug.Log("Warning! Animation Override check but there's no routine assigned");	
				}
				_overrideAnimation = false;
			}
			#endregion
		}

		public void WaitTimerForEachWP(WaypointsBase wp, List<WaypointsBase> listOfWP)
		{
			if (listOfWP.Count > 1) 
			{
				#region WaitTime
				_waitTime += Time.deltaTime;

				if(_waitTime > wp.waitTime )
				{
					if(circularList)
					{
						if(listOfWP.Count - 1 > indexWaypoint)
							indexWaypoint++;
						else
							indexWaypoint = 0;
					}
					else
					{
						if(!descendingList)
						{
							if(listOfWP.Count - 1 == indexWaypoint)
							{
								descendingList = true;
								indexWaypoint--;
							}
							else
								indexWaypoint++;
						}
						else
						{
							if(indexWaypoint > 0)
								indexWaypoint--;
							else
							{
								descendingList = false;
								indexWaypoint++;
							}
						}
					}

					_initCheck = false;
					enAI_main.goToPos = false;
					_waitTime = 0;
				}
				#endregion
			}

		}

		public void WaitTimerForExtraBehaviour(WaypointsBase wp, List<WaypointsBase> listOfWp)
		{
			if (listOfWp.Count > 1) 
			{
				#region WaitTime
				_waitTime += Time.deltaTime;

				if(_waitTime > wp.waitTime )
				{
					if(circularList)
					{
						if(listOfWp.Count - 1 > enAI_main.alertBehaviour.indexBehaviour)
							enAI_main.alertBehaviour.indexBehaviour++;
						else
							enAI_main.alertBehaviour.indexBehaviour = 0;
					}
					else
					{
						if(!descendingList)
						{
							if(listOfWp.Count - 1 == enAI_main.alertBehaviour.indexBehaviour)
							{
								descendingList = true;
								enAI_main.alertBehaviour.indexBehaviour--;
							}
							else
								enAI_main.alertBehaviour.indexBehaviour++;
						}
						else
						{
							if(enAI_main.alertBehaviour.indexBehaviour > 0)
								enAI_main.alertBehaviour.indexBehaviour--;
							else
							{
								descendingList = false;
								enAI_main.alertBehaviour.indexBehaviour++;
							}
						}
					}

					_initCheck = false;
					enAI_main.goToPos = false;
					_waitTime = 0;
				}
				#endregion
			}
		}
	}
}
