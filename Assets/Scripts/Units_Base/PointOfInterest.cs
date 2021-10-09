using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PointOfInterest : MonoBehaviour {

	public bool createPointOfInterest;

	public List<CharacterStates> affectedChars = new List<CharacterStates>();

	void Update () 
	{

		AlertEnemies (); 

	}

	// If enemies are in point of interest area then alert them
	void AlertEnemies()
	{
		if (createPointOfInterest) 
		{
			for (int i = 0; i < affectedChars.Count; i++) 
			{
				affectedChars [i].ChangeToAlert (transform.position);			
			}

			createPointOfInterest = false;
		}
	}

	// If It Enters in the Point of interest area add them in the affectedChars list
	void OnTriggerEnter(Collider other)  
	{
		if (other.gameObject.GetComponent<CharacterStates> ()) 
		{
			if (!affectedChars.Contains (other.GetComponent<CharacterStates> ()))
				affectedChars.Add (other.GetComponent<CharacterStates> ());
		}
	}

	// If It Exits the Point of interest area remove them from the affectedChars list
	void OnTriggerExit(Collider other)
	{
		if (other.GetComponent<CharacterStates> ()) 
		{
			if (affectedChars.Contains (other.GetComponent<CharacterStates> ()))
				affectedChars.Remove (other.GetComponent<CharacterStates> ());
		}
	}
}

