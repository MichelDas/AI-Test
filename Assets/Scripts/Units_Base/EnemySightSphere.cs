using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySightSphere : MonoBehaviour {

	EnemyAI enAI;	//  The EnemyAI attached to this enemy
	CharacterStates charStats;  // reference of characterStates of this enemy

	List<CharacterStates> trackingTargets = new List<CharacterStates>();  // enemies in the sight area

	// Use this for initialization
	void Start () 
	{
		enAI = GetComponentInParent<EnemyAI> ();
		charStats = GetComponentInParent<CharacterStates> ();  
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		for (int i = 0; i < trackingTargets.Count; i++) 
		{
			if (trackingTargets [i] != enAI.target) {
				Vector3 direction = trackingTargets [i].transform.position - transform.position;
				float angleTowardsTarget = Vector3.Angle (transform.parent.forward, direction.normalized);

				if (angleTowardsTarget < charStats.viewAngleLimit) {
					enAI.target = trackingTargets [i];
				}
			} 
			else
			{
				continue;
			}
		}
		
	}

	void OnTriggerEnter(Collider col)
	{
		if (col.GetComponent<CharacterStates> ()) 
		{
			CharacterStates otherStats = col.GetComponent<CharacterStates> ();

			if ( otherStats.team != charStats.team )
			{
				if ( !trackingTargets.Contains (otherStats) )
				{
					trackingTargets.Add (otherStats);
				}
			}
			else
			{
				EnemyAI otherAI = otherStats.transform.GetComponent<EnemyAI> ();

				if ( otherAI != enAI )
				{
					if ( !enAI.alliesNear.Contains (otherAI) )
					{
						enAI.alliesNear.Add (otherAI);
					}
				}
			}
		}
	}

	void OnTriggerExit(Collider col)
	{
		if (col.GetComponent<CharacterStates> ()) 
		{
			CharacterStates leavingTarget = col.GetComponent<CharacterStates> ();

			if (trackingTargets.Contains (leavingTarget)) 
			{
				trackingTargets.Remove (leavingTarget);
			}

			if ( leavingTarget.transform.GetComponent<EnemyAI> () )
			{
				EnemyAI otherAI = leavingTarget.transform.GetComponent<EnemyAI> ();

				if ( otherAI != enAI )
				{
					if ( !enAI.alliesNear.Contains (otherAI) )
					{
						enAI.alliesNear.Remove (otherAI);
					}
				}
			}
		}
	}


}
