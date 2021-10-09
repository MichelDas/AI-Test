// .............. Michel Das .......................

using UnityEngine;
using System.Collections;
using CnControls;

namespace AI
{

	public class GameManager : MonoBehaviour
	{

		public CharacterStates selectedUnit;	// is the player unit selected or enemy
		public int playerTeam;				// the number which indicates team number of player
		public GameObject unitControls;		// The Panal that contains UI buttons to control player
		public bool doubleClick;		// indicates that player will run
		public bool overUIElement;		// Is UI buttons working 
		public GameObject cameraMover;		// The object that holds camera
		public float cameraSpeed = 0.3f;     // camera move speed

		public bool ignorePlayer;

		// Update is called once per frame
		void Update () 
		{


			if ( !ignorePlayer )
			{
				// if UI buttons are not  working then click on a place to move the player
				if (!overUIElement) 
				{
					HandleSelection ();
				}

				bool hasUnit = selectedUnit;
				unitControls.SetActive (hasUnit);	// activate UI buttons

				HandleCameraMovement ();	// move Camera

			}


		}

		// Move Player At the Clicked area
		void HandleSelection()
		{
			if (Input.GetMouseButtonUp (0)) 
			{
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				RaycastHit hit;

				if (Physics.Raycast (ray, out hit, 100)) {
					CheckHit (hit);
				}
			
			}
		}

		/*
		 * selects player
		 * deselectes player
		 * make player run and walk
		 * */
		void CheckHit(RaycastHit hit)
		{
			if (hit.transform.GetComponent<CharacterStates> ()) 
			{
				CharacterStates hitStats = hit.transform.GetComponent<CharacterStates> ();

				if (hitStats.team == playerTeam) 
				{
					if (selectedUnit == null)
					{
						selectedUnit = hitStats;
						selectedUnit.selected = true;
					} 
					else 
					{
						selectedUnit.selected = false;
						selectedUnit = hitStats;
						selectedUnit.selected = true;
					}
				
				} 
				else 
				{
					if (selectedUnit == null) 
					{
						// Add something
					}
				}
			} 
			else 
			{
				if (selectedUnit) 
				{		
					// player will run
					if (doubleClick) 
					{
						selectedUnit.alert = true;
						selectedUnit.run = true;
					}
					else
					{		// player will walk
						doubleClick = true;
						StartCoroutine ("closeDoubleClick");
					}

					selectedUnit.MoveToPosition (hit.point);
				}
			}
		}

		IEnumerator closeDoubleClick()
		{
			yield return new WaitForSeconds (1);
			doubleClick = false;
		}

		// Move camera
		void HandleCameraMovement()
		{
			float hor =  Input.GetAxis ("Horizontal");
			float vert = Input.GetAxis ("Vertical");

			float hor1 = CnInputManager.GetAxis ("Horizontal");
			float ver1 = CnInputManager.GetAxis ("Vertical");


			Vector3 newPos = new Vector3 (hor, 0, vert) * cameraSpeed;
			Vector3 newPos1 = new Vector3 (hor1, 0, ver1) * cameraSpeed;
			cameraMover.transform.position += newPos;

			cameraMover.transform.position += newPos1;

			
		}

		// Activate UI Buttons
		public void EnterUIElement()
		{
			overUIElement = true;
		}

		// DeActivate UI Buttons
		public void ExitUIElement()
		{
			overUIElement = false;
		}

		// If running then Stop, if standing then crouch, if crouching then Stand
		public void ChangeStance()
		{
			if (selectedUnit) {
				if ( Input.GetKeyDown (KeyCode.C) )
				{
					overUIElement = true;
					selectedUnit.run = false;
					selectedUnit.crouch = !selectedUnit.crouch;
					overUIElement = false;
				}
			}
		}
	}
}
