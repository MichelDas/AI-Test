using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace AI
{

	public class SearchBehaviour : MonoBehaviour {

		EnemyAI enAI_main;

		//Searching Variables
		public bool decideBehaviour;
		public float decideBehaviourThreshold = 5;
		public List<Transform> possibleHidingPlaces = new List<Transform> ();
		public List<Vector3> positionAroundUnit = new List<Vector3>();
		bool getPossibleHidingPositions;
		bool populateListofPositions;
		bool searchAtPositions;
		bool createSearchPositions;
		int indexSearchPositions;
		bool searchHidingSpots;
		Transform targetHidingSpot;

		public float delayTillNewBehaviour = 3;	// time taken before changing the state
		float _timerTillNewBehaviour;

		// Use this for initialization
		void Start () 
		{
			enAI_main = GetComponent<EnemyAI> ();
		}

		public void SearchBehavior ()
		{
			if ( !decideBehaviour )
			{
				int ranValue = Random.Range (0, 11);

				if ( ranValue < decideBehaviourThreshold )
				{
					searchAtPositions = true;
					Debug.Log ("Searching in position around unit");
				}
				else
				{
					searchHidingSpots = true;
					Debug.Log ("Searching in Hiding Spots");
				}

				decideBehaviour = true;
			}
			else
			{
				#region search for HidingSpots
				if(searchHidingSpots)
				{
					if(!populateListofPositions)
					{
						possibleHidingPlaces.Clear();

						Collider[] allColliders = Physics.OverlapSphere(transform.position,20);

						for(int i=0 ; i<allColliders.Length ; i++)
						{
							if(allColliders[i].GetComponent<HidingSpots>())
							{
								possibleHidingPlaces.Add(allColliders[i].transform);
							}
						}
						populateListofPositions = true;
					}
					else if(possibleHidingPlaces.Count > 0)
					{
						if(!targetHidingSpot)
						{
							int ranValue = Random.Range(0,possibleHidingPlaces.Count);

							targetHidingSpot = possibleHidingPlaces[ranValue];
						}
						else
						{
							enAI_main.charStats.MoveToPosition(targetHidingSpot.position);

							Debug.Log("Going to hiding spot");
							float distanceToTarget = Vector3.Distance(transform.position, targetHidingSpot.position);

							if(distanceToTarget < 2)
							{
								_timerTillNewBehaviour += Time.deltaTime;

								if(_timerTillNewBehaviour > delayTillNewBehaviour)
								{
									//do things and reset
									populateListofPositions = false;
									targetHidingSpot = null;
									decideBehaviour = false;
									_timerTillNewBehaviour = 0;
								}
							}
						}
					}
					else
					{
						// No hiding spot found near unit, search at position instead
						Debug.Log("No hiding spots found, cancedl it and search at positions instead");
						searchAtPositions = true;
						populateListofPositions = false;
						targetHidingSpot = null;
					}
				}

				#endregion

				if ( searchAtPositions )
				{
					if ( !createSearchPositions )
					{
						positionAroundUnit.Clear ();

						int ranValue = Random.Range (4, 10);

						for (int i = 0; i < ranValue; i++)
						{
							float offsetX = Random.Range (-10, 10);
							float offsetZ = Random.Range (-10, 10);

							Vector3 originPos = transform.position;
							originPos += new Vector3 (offsetX, 0, offsetZ);

							NavMeshHit hit;

							if ( NavMesh.SamplePosition (originPos, out hit, 5, NavMesh.AllAreas) )
							{
								positionAroundUnit.Add (hit.position);
							}
						}

						if ( positionAroundUnit.Count > 0 )
						{
							indexSearchPositions = 0;
							createSearchPositions = true;
						}
					}
					else
					{
						Vector3 targetPosition = positionAroundUnit [indexSearchPositions];

						Debug.Log ("Going To Position");

						enAI_main.charStats.MoveToPosition (targetPosition);

						float distanceToPosition = Vector3.Distance (transform.position, targetPosition);

						if ( distanceToPosition < 2 )
						{
							int ranVal = Random.Range (0, 11);
							decideBehaviour = (ranVal < 5);

							if ( indexSearchPositions < positionAroundUnit.Count - 1 )
							{
								_timerTillNewBehaviour += Time.deltaTime;

								if ( _timerTillNewBehaviour > delayTillNewBehaviour )
								{
									indexSearchPositions++;
									_timerTillNewBehaviour = 0;
								}
							}
							else
							{
								_timerTillNewBehaviour += Time.deltaTime;

								if ( _timerTillNewBehaviour > delayTillNewBehaviour )
								{
									indexSearchPositions = 0;
									_timerTillNewBehaviour = 0;
								}
							}

						}
					}
				}
			}
		}
	}
}
