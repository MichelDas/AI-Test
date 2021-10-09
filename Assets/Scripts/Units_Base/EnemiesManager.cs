using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemiesManager : MonoBehaviour {

	public List<CharacterStates> AllEnemies = new List<CharacterStates> ();				// list of all enemies
	public List<CharacterStates> EnemiesAvailableToChase = new List<CharacterStates>(); // enemies that can chase if there is a point of interest
	public List<CharacterStates> EnemiesOnPatrol = new List<CharacterStates>();			// list of enemies those are patroling

	public bool showBehaviour;		
	public bool resetAll;
	public bool universalAlert;
	public bool everyoneWhoCanChase;
	public bool patrolsOnly;
	public Transform debugPOI;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (resetAll) 	// this makes all enemies go to normal state
		{
			for (int i = 0; i < AllEnemies.Count; i++) 
			{
				AllEnemies [i].ChangeToNormal ();
			}

			resetAll = false;
		}

		if (universalAlert) 	// this makes all enemies go to alert state
		{
			for (int i = 0; i < AllEnemies.Count; i++) 
			{
				AllEnemies [i].ChangeToAlert (debugPOI.position);
			}

			universalAlert = false;
		}

		if (everyoneWhoCanChase)  // make chase the enemies who can chase
		{
			for (int i = 0; i < EnemiesAvailableToChase.Count; i++) 
			{
				EnemiesAvailableToChase [i].ChangeToAlert (debugPOI.position);
			}
			everyoneWhoCanChase = false;
		}

		if (patrolsOnly) {		
			for (int i = 0; i < EnemiesOnPatrol.Count; i++) 
			{
				EnemiesOnPatrol [i].ChangeToAlert (debugPOI.position);
			}

			patrolsOnly = false;
		}

		if (showBehaviour) {
			for (int i = 0; i < AllEnemies.Count; i++) 
			{
				AllEnemies [i].GetComponent<EnemyUI> ().EnableDisableUI ();	
			}
			showBehaviour = false;
		}
	}


	/*
	 * enemies who can chase add them to the lise enemiesAvailableToChase
	 *  */
	public void UpdateListOfChaseEnemies()
	{
		if (AllEnemies.Count > 0) 
		{
			for (int i = 0; i < AllEnemies.Count; i++) 
			{
				if (AllEnemies [i].GetComponent<EnemyAI> ().canChase) 
				{
					if (!EnemiesAvailableToChase.Contains (AllEnemies [i])) 
					{
						EnemiesAvailableToChase.Add (AllEnemies [i]);
					}
				}
			}
		}
	}
}
