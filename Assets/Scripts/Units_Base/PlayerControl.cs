using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerControl : MonoBehaviour {

	Animator anim;

	UnityEngine.AI.NavMeshAgent agent;

	CharacterStates charstates;

	public float stopDistance = 1;
	public bool moveToPosition;
	public Vector3 destPosition;

	public bool run;
	public bool crouch;

	public float walkSpeed = 1;
	public float runSpeed = 2;
	public float crouchSpeed = 0.8f;

	public float maxStance = 0.9f;
	public float minStance = 0.1f;
	float targetStance;
	float stance;
	public float TestStance, TestForward;

	List<Rigidbody> ragdollBones = new List<Rigidbody>();

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
		SetupAnimator ();
		agent = GetComponent<UnityEngine.AI.NavMeshAgent> ();
		charstates = GetComponent<CharacterStates> ();
		agent.stoppingDistance = stopDistance - 0.1f;

		agent.updateRotation = true;
		agent.angularSpeed = 500;
		agent.autoBraking = false;
		InitRagdoll ();

		if (GetComponentInChildren<EnemySightSphere> ()) 
		{
			GetComponentInChildren<EnemySightSphere> ().gameObject.layer = 2;
		}

	}

	// Update is called once per frame
	void Update () {

		if ( !charstates.dead )
		{
			run = charstates.run;

			if (moveToPosition) {

				agent.Resume ();
				agent.updateRotation = true;
				agent.SetDestination (destPosition);

				float distanceToTarget = Vector3.Distance (transform.position, destPosition);

				if (distanceToTarget <= stopDistance) {
					moveToPosition = false;
					charstates.run = false;
				}
			}
			else
			{
				agent.Stop ();
				agent.updateRotation = false;
			}

			HandleSpeed ();
			HandleAiming ();
			HandleAnimation ();
			HandleStates ();
		}

	}

	void HandleAiming()
	{
		anim.SetBool ("Aim", charstates.aim);

		if ( charstates.shooting )
		{
			anim.SetTrigger ("Shoot");
			charstates.shooting = false;
		}
	}

	void SetupAnimator()
	{
		// this is a ref to the animator component on the root
		anim = GetComponent<Animator>();

		// we use avatar from a child animator component if present
		//this is to enable easy swapping of the character model as a child node
		foreach (var childAnimator in GetComponentsInChildren<Animator>()) 
		{
			if (childAnimator != anim) 
			{
				anim.avatar = childAnimator.avatar;
				Destroy (childAnimator);
				break;//if you find the first animator, stop searching
			}

		}
	}

	void HandleSpeed()
	{
		if (!run)
		{
			if (!crouch) 
			{
				agent.speed = walkSpeed;
			} 
			else 
			{
				agent.speed = crouchSpeed;
			}
		} 
		else 
		{
			agent.speed = runSpeed;
		}
	}

	void HandleAnimation()
	{
		Vector3 relativeDirection = (transform.InverseTransformDirection (agent.desiredVelocity)).normalized;
		float animValue = relativeDirection.z;

		if (!run) {
			animValue = Mathf.Clamp (animValue, 0, 0.5f);
		}
		TestForward = animValue;
		anim.SetFloat ("Forward", animValue, 0.3f, Time.deltaTime);
	}

	void HandleStates()
	{
		if (charstates.run) 
		{
			targetStance = minStance;
		}
		else
		{
			if (charstates.crouch) 
			{
				targetStance = maxStance;
			}
			else
			{
				targetStance = minStance;
			}
		}

		stance = Mathf.Lerp (stance, targetStance, Time.deltaTime * 3);
		TestStance = stance;
		anim.SetFloat ("Stance", stance);
		anim.SetBool ("Alert", charstates.alert);
	}

	void InitRagdoll()
	{
		Rigidbody[] rigB = GetComponentsInChildren<Rigidbody> ();
		Collider[] cols = GetComponentsInChildren<Collider> ();

		for (int i = 0; i < rigB.Length; i++) 
		{
			rigB [i].isKinematic = true;
		}

		for (int i = 0; i < cols.Length; i++) 
		{
			if (i != 0)
				cols [i].gameObject.layer = 10;		//layer 10 is Ragdoll 

			cols [i].isTrigger = true;
		}

	}

	public void RagdollCharacter()
	{
		
	}

	public void Die()
	{
		anim.SetTrigger ("Die");
		anim.SetBool ("Dead", true);
	}
}
