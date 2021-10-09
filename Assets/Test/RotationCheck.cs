using UnityEngine;
using System.Collections;

public class RotationCheck : MonoBehaviour {

	public Transform Target;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 relativePos = Target.position - transform.position;

		relativePos.y = 0;
		Quaternion rotation = Quaternion.LookRotation (relativePos);

		transform.rotation = rotation;
	}
}
