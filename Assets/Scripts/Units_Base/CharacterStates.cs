//.................. Michel Das ...............................

using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class CharacterStates : MonoBehaviour {

	public float health;	// health of the character
	public int morale = 100;		//
	public int suppresionLevel = 20;
	public int unitRank = 0;

	public float viewAngleLimit = 50;	// in what angle does the character see(only for enemies)
	public int alertLevel;		// what is the alert level
	public bool aim;		// does he aiming
	public bool shooting;	// is he shooting
	public bool dead;		// is he dead
	public int team;		// which team does he belong to
	public bool selected;   // if it is a player then it can be selected
	public bool run;		// is he running
	public bool crouch;		// is he crouching
	public bool alert = true;  // Indicates The character is alert or not
	public bool hasCover;

	public int ReduceAmount;

	public Transform alertDebugCube; // The cube that indicates the Alert level
	public GameObject selectCube;  // This is used to indicate selected

	EnemyAI enAI;		// reference to the EnemyAi attached to this enemy


	PlayerControl plControl;		//reference to the player

	// Use this for initialization
	void Start () 
	{

		health = 100;

		plControl = GetComponent<PlayerControl> ();

		if (GetComponent<EnemyAI> ()) {
			enAI = GetComponent<EnemyAI> ();
		}
	}
	
	// Update is called once per frame
	void Update () 
	{		
		selectCube.SetActive (selected);

		if (run) 
		{
			crouch = false;
		}

		// Adjust the size of alert Cube accordin to the alertLevel
		if (alertDebugCube) 
		{
			float scale = alertLevel * 0.05f;
			alertDebugCube.localScale = new Vector3 (scale, scale, scale);
		}

		if ( morale < 0 )
		{
			morale = 0;
		}
		if ( health <= 0 )
		{
			health = 0;

			if ( !dead )
			{
				dead = true;

				if ( enAI )
				{
					// decrease Morale
				}

				KillCharacter ();
			}
		}

	}

	public void MoveToPosition(Vector3 position)
	{
		plControl.moveToPosition = true;
		plControl.destPosition = position;
	}

	public void StopMoving()
	{
		plControl.moveToPosition = false;
	}

	public void CallFunctionWithString(string functionIdentifier, float delay)
	{
		Invoke (functionIdentifier, delay);
	}

	void ChangeStance()
	{
		crouch = !crouch;
	}

	void AlertPhase()
	{
		alert = !alert;
	}

	public void ChangeToNormal()
	{
		enAI.ChangeAIBehaviour ("AI_State_Normal", 0);
		alert = false;
		crouch = false;
		run = false;
	}

	public void ChangeToAlert(Vector3 poi)
	{
		alert = true;
		plControl.moveToPosition = false;
		enAI.GoOnAlert (poi);
	}

	// kill the character
	private void KillCharacter()
	{
		//plControl.RagdollCharacter ();

		GetComponent<Collider> ().enabled = false;
		GetComponent<Rigidbody> ().isKinematic = true;
		GetComponent<NavMeshAgent> ().enabled = false;
		plControl.Die ();

		if ( enAI )
		{
			enAI.commonBehaviour.enabled = false;
			enAI.chaseBehaviour.enabled = false;
			enAI.searchBehaviour.enabled = false;
			enAI.attackBehaviour.enabled = false;
			enAI.alliesBehavior.enabled = false;
			enAI.alertBehaviour.enabled = false;
			enAI.enabled = false;
		}

	}
}
