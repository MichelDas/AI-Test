using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{

	public class AttackBehaviour : MonoBehaviour {

		EnemyAI enAI_main;

		// General Behaviour variables 
		public float delayTillNewBehaviour = 3;	// time taken before changing the state
		float _timerTillNewBehaviour;

		// Attack variables
		public bool startShooting;
		public float attackRate = 1.5f;
		public float shootRate = 0.3f;
		float shootR;
		float attackR;

		// Shooting variables
		float _delayAnim;
		bool validCover;
		int timesToShoot;
		int timesShot;
		bool onAimingAnimation;
		public ParticleSystem muzzleFire;
		public AudioSource audioSource;
		public Transform bulletSpawnPoint;

		// Cover variables
		bool findCoverPositions;
		public List<Transform> coverPositions = new List<Transform>();
		public List<Transform> ignorePositions = new List<Transform>();
		private ObjectDistanceComparer objectDistanceComparer;
		public Coverbase currentCoverPosition;
		public int maxTries = 3;
		public int _curTries;

		private CharacterStates charStates;

		// Use this for initialization
		void Start () {
			enAI_main = GetComponent<EnemyAI> ();
			audioSource = GetComponentInChildren<AudioSource> ();
			charStates = GetComponent<CharacterStates> ();
		}
		
		public void HasTargetBehaviour()
		{
			enAI_main.charStats.StopMoving ();

			if (enAI_main.SightRaycasts ())
			{
				
				if (enAI_main.charStats.alertLevel < 10)
				{
					
					float distanceToTarget = Vector3.Distance (transform.position, enAI_main.target.transform.position);
					float multiplier = 1 + (distanceToTarget * 0.1f);
					// How fast it recognises it's enemy is based on the distance
					// we can also add extra behaviour here

					enAI_main.alertTimer += Time.deltaTime * enAI_main.alertMultiplier;

					if (enAI_main.alertTimer > enAI_main.alertTimerIncrement) 
					{
						
						enAI_main.charStats.alertLevel++;
						enAI_main.alertTimer = 0;
					}
				}
				else
				{

					enAI_main.AI_State_DecideByStats();
				}

				enAI_main.LookAtTarget (enAI_main.lastKnownPosition);
			}
			else
			{
				if (enAI_main.charStats.alertLevel > 5) 
				{
					enAI_main.AI_State_Normal();
					enAI_main.pointOfInterest = enAI_main.lastKnownPosition;
				}
				else
				{
					_timerTillNewBehaviour += Time.deltaTime;

					if(_timerTillNewBehaviour > delayTillNewBehaviour)
					{
						enAI_main.AI_State_Normal();
						_timerTillNewBehaviour = 0;
					}
				}
			}
		}

		public void DecideByStats()
		{
			bool supPass = supressionPass ();
			bool morPass = moralPass ();

			if ( supPass && morPass )
			{
				enAI_main.AI_State_Attack ();
			}
			else
			{
				if ( !supPass )
				{
					enAI_main.AI_State_Cover ();
				}
			}

		}

		private bool supressionPass()
		{
			int ranValue = Random.Range (0, 101);

			int enemyAiming = 0;

			if ( enAI_main.target.aim )
			{
				enemyAiming = 10;
			}

			int modifiers = enemyAiming;

			ranValue += modifiers;

			if ( ranValue < enAI_main.charStats.suppresionLevel )
				return false;
			else
				return true;
		}

		private bool moralPass()
		{
			int ranValue = Random.Range (0, 101);

			int health = Mathf.RoundToInt (enAI_main.charStats.health / 10);

			int friendlies = 0;

			if ( enAI_main.alliesNear.Count > 0 )
			{
				friendlies = 10;

				for (int i = 0; i < enAI_main.alliesNear.Count; i++)
				{
					if ( enAI_main.alliesNear [i].charStats.unitRank > enAI_main.charStats.unitRank )
					{
						friendlies += 10;
					}
				}
			}

			int modifiers = health + friendlies;

			ranValue -= modifiers;

			if ( ranValue > enAI_main.charStats.morale )
				return false;
			else
				return true;
		}

		/*
		 * if he sees the target, Attack
		 * else seek the target
		 *  */ 
		public void AttackBehavior()
		{
			if (!startShooting) 
			{
				if (enAI_main.SightRaycasts ()) 
				{
					enAI_main.LookAtTarget (enAI_main.lastKnownPosition);
					enAI_main.charStats.aim = true;

					attackR += Time.deltaTime;

					if (attackR > attackRate) 
					{
						startShooting = true;
						timesShot = 0;
						timesToShoot = Random.Range (1, 5);
						attackR = 0;
					}
				} 
				else 
				{
					enAI_main.charStats.aim = false;
					enAI_main.AI_State_Chase();
				}
			}
			else 
			{
				ShootingBehaviour ();
			}

		}

		public void AttackFromCover()
		{
			if ( !startShooting )
			{
				enAI_main.LookAtTarget (enAI_main.lastKnownPosition);
				enAI_main.charStats.run = false;
				enAI_main.charStats.crouch = true;

				float attackRatePenalty = 0;

				attackRatePenalty = enAI_main.charStats.suppresionLevel * 0.01f;

				attackR += Time.deltaTime;

				if ( attackR > attackRate + attackRatePenalty )
				{
					ReEvaluateCover ();

					if ( validCover )
					{
						enAI_main.charStats.suppresionLevel -= 10;

						if ( enAI_main.charStats.suppresionLevel < 0 )
							enAI_main.charStats.suppresionLevel = 0;

						if ( supressionPass () )
						{
							enAI_main.charStats.crouch = false;
							startShooting = true;
							timesShot = 0;
							timesToShoot = Random.Range (1, 5);
							_delayAnim = -0;
						}

					}
					else
					{
						enAI_main.charStats.aim = false;
						findCoverPositions = false;
						_curTries = 0;
						currentCoverPosition.occupied = false;
						enAI_main.AI_State_Cover ();
					}

					attackR = 0;
				}
			}
			else
			{
				
				enAI_main.LookAtTarget (enAI_main.lastKnownPosition);
				enAI_main.charStats.aim = true;
				_delayAnim += Time.deltaTime;

				if ( _delayAnim > 1 )
				{
					if ( enAI_main.SightRaycasts () )
					{
						ShootingBehaviour ();
					}
					else
					{
						startShooting = false;
						enAI_main.charStats.aim = false;
						attackR = 0;
						timesShot = 0;
						enAI_main.AI_State_Chase ();

					}
				}
			}
		}

		void ShootingBehaviour ()
		{
			if ( timesShot < timesToShoot )
			{
				shootR += Time.deltaTime;

				if ( shootR > shootRate )
				{
					muzzleFire.Emit (1);
					audioSource.Play ();
					enAI_main.charStats.shooting = true;
					if ( enAI_main.SightRaycasts () )
					{
						enAI_main.target.health -= charStates.ReduceAmount;
					}
					timesShot++;
					shootR = 0; 

					if ( timesShot == timesToShoot - 1 )
					{
						enAI_main.alliesBehavior.AlertEveryoneInsideRange (50);
					}
				}
			}
			else
			{
				startShooting = false;
			}
		}



		public void CoverBehavior()
		{
			if ( !findCoverPositions )
			{
				FindCover ();
			}
			else
			{
				enAI_main.charStats.MoveToPosition (currentCoverPosition.positionObject.position);
				enAI_main.charStats.run = true;

				float distance = Vector3.Distance (transform.position, currentCoverPosition.positionObject.position);

				if ( distance < 1 )
				{
					enAI_main.charStats.hasCover = true;
					enAI_main.charStats.StopMoving ();
					enAI_main.AI_State_Attack ();
				}
			}
		}

		void ReEvaluateCover()
		{
			Vector3 targetPosition = enAI_main.lastKnownPosition;
			Transform validatePosition = currentCoverPosition.positionObject.parent.parent.transform;

			Vector3 directionOfTarget = targetPosition - validatePosition.position;
			Vector3 coverForward = validatePosition.TransformDirection (Vector3.forward);

			if ( Vector3.Dot (coverForward, directionOfTarget) > 0 )
			{
				if ( currentCoverPosition.backPos )
					validCover = false;
				else
					validCover = true;
			}
			else
			{
				if ( currentCoverPosition.backPos )
				{
					validCover = true;
				}
				else
					validCover = false;
			}
		}
			
		void FindCover()
		{
			if ( _curTries <= maxTries )
			{
				if ( !findCoverPositions )
				{
					findCoverPositions = true;
					_curTries++;

					Coverbase targetCoverPosition = null;
					float distanceToTarget = Vector3.Distance (transform.position, enAI_main.target.transform.position);

					coverPositions.Clear ();

					Vector3 targetPosition = enAI_main.target.transform.position;

					Collider[] colliders = Physics.OverlapSphere (transform.position, 20);

					for (int i = 0; i < colliders.Length; i++)
					{
						if ( colliders [i].GetComponent<CoverPosition> () )
						{
							if ( !ignorePositions.Contains (colliders [i].transform) )
							{
								float distanceToCandidate = Vector3.Distance (transform.position, colliders [i].transform.position);

								if ( distanceToCandidate < distanceToTarget )
								{
									coverPositions.Add (colliders [i].transform);
								}
							}
						}
					}

					if ( coverPositions.Count > 0 )
					{
						SortPositions (coverPositions);

						CoverPosition validatePosition = coverPositions [0].GetComponent<CoverPosition> ();

						Vector3 directionOfTarget = targetPosition - validatePosition.transform.position;
						Vector3 coverForward = validatePosition.transform.TransformDirection (Vector3.forward);

						if ( Vector3.Dot (coverForward, directionOfTarget) < 0 )
						{
							for (int i = 0; i < validatePosition.BackPositions.Count; i++)
							{
								if ( !validatePosition.BackPositions [i].occupied )
								{
									targetCoverPosition = validatePosition.BackPositions [i];
								}
							}
						}
						else
						{
							for (int i = 0; i < validatePosition.FrontPositions.Count; i++)
							{
								if ( !validatePosition.FrontPositions [i].occupied )
								{
									targetCoverPosition = validatePosition.FrontPositions [i];
								}
							}
						}
					}

					if ( targetCoverPosition == null )
					{
						findCoverPositions = false;

						if ( coverPositions.Count > 0 )
						{
							ignorePositions.Add (coverPositions [0]);
						}
					}
					else
					{
						targetCoverPosition.occupied = true;
						currentCoverPosition = targetCoverPosition;
					}
				}
			}
			else
			{
				Debug.Log ("Max tries reached! Changing behavior!");
				enAI_main.AI_State_Attack ();
			}

		}

		void SortPositions(List<Transform> position)
		{
			objectDistanceComparer = new ObjectDistanceComparer(this.transform);
			position.Sort (objectDistanceComparer);
		}

		private class ObjectDistanceComparer : IComparer<Transform>
		{
			private Transform referenceObject;

			public ObjectDistanceComparer(Transform reference)
			{
				referenceObject = reference;
			}

			public int Compare(Transform x, Transform y)
			{
				float distX = Vector3.Distance (x.position, referenceObject.position);

				float distY = Vector3.Distance (y.position, referenceObject.position);

				int retVal = 0;

				if ( distX < distY )
				{
					retVal = -1;
				}
				else if ( distX > distY )
				{
					retVal = 1;
				}

				return retVal;
			}
		}
	}

}
