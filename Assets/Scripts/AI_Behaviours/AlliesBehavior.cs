using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{

	public class AlliesBehavior : MonoBehaviour {

		EnemyAI enAI_main;

		// Use this for initialization
		void Start () 
		{
			enAI_main = GetComponent<EnemyAI> ();	
		}
		
		public void AlertAllies()
		{
			if ( enAI_main.alliesNear.Count > 0 )
			{
				for (int i = 0; i < enAI_main.alliesNear.Count; i++)
				{
					if ( enAI_main.alliesNear [i].aiStates == EnemyAI.AIstates.patrol )
					{
						enAI_main.alliesNear [i].AI_State_HasTarget ();
						enAI_main.alliesNear [i].target = enAI_main.target;
						enAI_main.alliesNear [i].charStats.alertLevel = 10;
					}
				}
			}
		}

		public void AlertEveryoneInsideRange(float range)
		{
			LayerMask mask = 1 << gameObject.layer;

			Collider[] cols = Physics.OverlapSphere (transform.position, range, mask);

			Debug.Log (cols.Length);

			for (int i = 0; i < cols.Length; i++)
			{

				if ( cols [i].transform.GetComponent<EnemyAI> () )
				{
					EnemyAI otherAi = cols [i].transform.GetComponent<EnemyAI> ();

					if ( otherAi.aiStates == EnemyAI.AIstates.patrol )
					{
						otherAi.AI_State_HasTarget ();
						otherAi.target = enAI_main.target;
						otherAi.charStats.alertLevel = 10;
					}
				}
			}
		}

		public void DecreaseAlliesMorale(int amount)
		{
			if ( enAI_main.alliesNear.Count > 0 )
			{
				for (int i = 0; i < enAI_main.alliesNear.Count; i++)
				{
					enAI_main.alliesNear [i].charStats.morale -= amount;
				}
			}
		}
	}

}
