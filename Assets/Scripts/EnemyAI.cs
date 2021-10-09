//............. Michel Das ...................

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

[RequireComponent(typeof(AI.CommonBehaviour))]
[RequireComponent(typeof(AI.AttackBehaviour))]
[RequireComponent(typeof(AI.SearchBehaviour))]
[RequireComponent(typeof(AI.ChaseBehaviour))]
[RequireComponent(typeof(AI.AlertBehaviour))]
[RequireComponent(typeof(AI.AlliesBehavior))]

public class EnemyAI : MonoBehaviour {

	// Change the Ragdoll colliders layer to ignore raycasts

	public CharacterStates target;		// 
	public float sightDistance = 20;
	public bool goToPos;
	public Vector3 lastKnownPosition;
	public Vector3 pointOfInterest;
	public float alertTimer;
	public float alertTimerIncrement;
	public float alertMultiplier;

	public bool onPatrol;
	public bool canChase;
	public List<EnemyAI> alliesNear = new List<EnemyAI>();

	public enum AIstates	// states of the enemy
	{
		patrol,
		chase,
		alert,
		attack,
		search,
		cover,
		deciding,
		onAlertBehaviour,
		hastarget
	}
		
	// Components
	public PlayerControl plControl;
	public AIstates aiStates;
	public CharacterStates charStats; 
	EnemiesManager enManager;
	Quaternion targetRot;

	[HideInInspector] public AI.CommonBehaviour commonBehaviour;
	[HideInInspector] public AI.ChaseBehaviour chaseBehaviour;
	[HideInInspector] public AI.AlertBehaviour alertBehaviour;
	[HideInInspector] public AI.AttackBehaviour attackBehaviour;
	[HideInInspector] public AI.SearchBehaviour searchBehaviour;
	[HideInInspector] public AI.AlliesBehavior alliesBehavior;


	// Use this for initialization
	void Start ()
	{
		plControl = GetComponent<PlayerControl> ();
		charStats = GetComponent<CharacterStates> ();

		charStats.alert = false;


		enManager = GameObject.FindGameObjectWithTag ("GameController").GetComponent<EnemiesManager> ();
		enManager.AllEnemies.Add (charStats);

		commonBehaviour = GetComponent<AI.CommonBehaviour> ();
		chaseBehaviour = GetComponent<AI.ChaseBehaviour> ();
		alertBehaviour = GetComponent<AI.AlertBehaviour> ();
		attackBehaviour = GetComponent<AI.AttackBehaviour> ();
		searchBehaviour = GetComponent<AI.SearchBehaviour> ();
		alliesBehavior = GetComponent<AI.AlliesBehavior> ();


		if (onPatrol) 
		{
			canChase = true;
			enManager.EnemiesOnPatrol.Add (charStats);
		}

		if (canChase) 
		{
			enManager.EnemiesAvailableToChase.Add (charStats);
		}

		sightDistance = GetComponentInChildren<EnemySightSphere> ().GetComponent<SphereCollider> ().radius;

	}
	 
	// Update is called once per frame
	void Update () {

		switch (aiStates) 
		{
			case AIstates.patrol:
				alertMultiplier = 1;
				commonBehaviour.DecreaseAlerLevels ();
				commonBehaviour.PatrolBehavior ();
				TargetAvailable ();
				break;
			case AIstates.alert:
				TargetAvailable ();
				alertBehaviour.AlertBehaviourMain ();
				break;
			case AIstates.onAlertBehaviour:
				TargetAvailable ();
				alertBehaviour.OnAlertExtraBehaviours ();
				break;
			case AIstates.chase:
				TargetAvailable ();
				chaseBehaviour.ChaseBehavior ();
				break;
			case AIstates.search:
				alertMultiplier = 0.3f;
				TargetAvailable ();
				commonBehaviour.DecreaseAlerLevels ();
				searchBehaviour.SearchBehavior ();
				break;
			case AIstates.cover:
				attackBehaviour.CoverBehavior ();
				break;
			case AIstates.hastarget:
				attackBehaviour.HasTargetBehaviour ();
				break;
			case AIstates.deciding:
				attackBehaviour.DecideByStats ();
				break;
			case AIstates.attack:
				if ( !charStats.hasCover )
					attackBehaviour.AttackBehavior ();
				else
					attackBehaviour. AttackFromCover ();
				break;
		}
	}

	/*
	 * This is used for changing AI Behaviours.
	 *  */
	public void ChangeAIBehaviour(string behaviour, float delay)
	{
		Invoke (behaviour, delay);
	}

	// Go to patrol State
	public void AI_State_Normal()
	{
		aiStates = AIstates.patrol;
		goToPos = false;
		alertBehaviour.lookAtPOI = false;
		commonBehaviour._initCheck = false;
		target = null;
		charStats.alert = false;
		charStats.hasCover = false;
	}

	// Go to Chase State
	public void AI_State_Chase()
	{
		aiStates = AIstates.chase;
		goToPos = false;
		alertBehaviour.lookAtPOI = false;
		commonBehaviour._initCheck = false;
		charStats.hasCover = false;
	}

	// decides which behaviours he will do when he is on alert
	public void AI_State_OnAlert_RunListOfBehaviour()
	{
		aiStates = AIstates.onAlertBehaviour;
		charStats.run = true;
		goToPos = false;
		alertBehaviour.lookAtPOI = false;
		commonBehaviour._initCheck = false;
		charStats.hasCover = false;
	}

	// Go to HasTargetState
	public void AI_State_HasTarget()
	{
		aiStates = AIstates.hastarget;
		charStats.alert = true;
		goToPos = false;
		alertBehaviour.lookAtPOI = false;
		commonBehaviour._initCheck = false;
		charStats.hasCover = false;
	}

	// Go to cover
	public void AI_State_Cover()
	{
		aiStates = AIstates.cover;
		charStats.run = true;
		goToPos = false;
		alertBehaviour.lookAtPOI = false;
		commonBehaviour._initCheck = false;
		charStats.hasCover = false;
	}

	// Go to the state that decides whether to attack or take cover
	public void AI_State_DecideByStats()
	{
		aiStates = AIstates.deciding;
		charStats.run = false;
		goToPos = false;
		alertBehaviour.lookAtPOI = false;
		commonBehaviour._initCheck = false;
		charStats.hasCover = false;
	}

	// Go to Attack State
	public void AI_State_Attack()
	{
		alliesBehavior.AlertAllies ();
		aiStates = AIstates.attack;
	}

	// Go to Search State
	public void AI_State_Search() 
	{
		aiStates = AIstates.search;
		target = null;
		goToPos = false;
		alertBehaviour.lookAtPOI = false;
		commonBehaviour._initCheck = false;
		charStats.hasCover = false;
	}

	//Go to Alert State
	public void GoOnAlert(Vector3 poi)
	{
		pointOfInterest = poi;
		aiStates = AIstates.alert;
		alertBehaviour.lookAtPOI = false;
	}
		

	public void LookAtTarget(Vector3 positionToLook)
	{
		Vector3 directionToLookTo = positionToLook - transform.position;
		directionToLookTo.y = 0;

		float angle = Vector3.Angle (transform.forward, directionToLookTo);

		if (angle > 0.1f) 
		{
			targetRot = Quaternion.LookRotation (directionToLookTo);
			transform.localRotation = Quaternion.Slerp (transform.localRotation, targetRot, Time.deltaTime * 3);
		}
	}
		
	/*
	 *  if sees the target changes to hasTargetBehaviour
	 *  */
	void TargetAvailable()
	{
		if (target) 
		{
			if (SightRaycasts ()) 
			{
				AI_State_HasTarget ();
			}
		}
	}
		
	/*
	 * 	returns true if he can see the target
	 * else returns false
	 *  */
	public bool SightRaycasts()
	{	
		//return value
		bool retVal = false;

		// There are two raycast, from top of body and from feet 
		RaycastHit hitTowardsLowerBody;
		RaycastHit hitTowardsUpperBody;

		// the ray is a pretty longer then the sight area.....
		float raycastDistance = sightDistance + (sightDistance * 0.5f);
		 
		//target's   position
		Vector3 targetPosition = lastKnownPosition;

		if (target) 
		{
			targetPosition = target.transform.position;
		}


		Vector3 raycastStart = transform.position + new Vector3 (0, 1.6f, 0);
		Vector3 direction = targetPosition - raycastStart;

		LayerMask excludeLayers = ~((1 << 9) | (1 << 10) | (1 << 2)); // Exculde Ragdoll layer and Enemies Layer
		 
		Debug.DrawRay (raycastStart, direction + new Vector3 (0, 1, 0));


		// checking lower raycast
		if (Physics.Raycast (raycastStart, direction + new Vector3 (0, 1, 0), out hitTowardsLowerBody, raycastDistance, excludeLayers)) 
		{
			if (hitTowardsLowerBody.transform.GetComponent<CharacterStates> ()) 
			{
				if (target) 
				{
					if (hitTowardsLowerBody.transform.GetComponent<CharacterStates> () == target) 
					{
						retVal = true;
						Debug.DrawRay (raycastStart, direction + new Vector3(0,1,0), Color.blue);
				
					}
				}
			}
		}

		//if lower raycast doesnt find enemy then check upper raycast
		if (retVal == false) 
		{
			direction += new Vector3 (0, 1.6f, 0);

			if(Physics.Raycast(raycastStart, direction, out hitTowardsUpperBody,raycastDistance,excludeLayers))
			{
				if (target) 
				{
					if (hitTowardsUpperBody.transform == target.transform) 
					{
						if (!target.crouch) 
						{
							retVal = true;
							Debug.DrawRay (raycastStart, hitTowardsUpperBody.transform.position, Color.yellow);
						}
					}
				}
			
			}
				
		}

		if (retVal) 
		{
			lastKnownPosition = target.transform.position;
		}

		return retVal;


	}

}

[System.Serializable]
public struct WaypointsBase
{
	public Transform targetDestination;
	public float waitTime;
	public bool lookTowards;
	public Transform targetToLookTo;
	public float speedTolook;
	public bool overrideAnimation;
	public string[] animationRoutines;
	public bool stopList;
}
