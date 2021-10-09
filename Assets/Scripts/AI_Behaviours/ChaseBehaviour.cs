using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
	public class ChaseBehaviour : MonoBehaviour {

		EnemyAI enAI_main;

		public float delayTillNewBehaviour = 3;	// time taken before changing the state
		float _timerTillNewBehaviour;

		// Use this for initialization
		void Start () 
		{
			enAI_main = GetComponent<EnemyAI> ();
		}
		
		/*
		 * how he will chase if he does not
		 *  */
		public void ChaseBehavior()
		{
			if (enAI_main.target == null)
			{
				if (!enAI_main.goToPos) 
				{					// if we have no position to go to
					enAI_main.charStats.MoveToPosition (enAI_main.lastKnownPosition);
					enAI_main.charStats.run = true;
					enAI_main.goToPos = true; 
				}
			}
			else 
			{
				enAI_main.charStats.MoveToPosition (enAI_main.target.transform.position);
				enAI_main.charStats.run = true;
			}

			if (!enAI_main.SightRaycasts ()) 
			{
				if ( enAI_main.target )	// we have a target but we cant see him
				{
					enAI_main.lastKnownPosition = enAI_main.target.transform.position;
					enAI_main.target = null;
				}
				else
				{	// we dont have a target and cant see anyone then we go to searching him
					float distanceFromTargetPosition = Vector3.Distance (transform.position, enAI_main.lastKnownPosition);

					if ( distanceFromTargetPosition < 2 )
					{
						_timerTillNewBehaviour += Time.deltaTime;

						if ( _timerTillNewBehaviour > delayTillNewBehaviour )
						{
							enAI_main.AI_State_Search ();
							_timerTillNewBehaviour = 0;
						}
					}
				}
			}
		}
	}
}