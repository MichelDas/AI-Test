using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EnemyUI : MonoBehaviour {

	public bool show = true;
	public GameObject enUIprefab;
	GameObject enUI;
	Text textUI;
	Text morale;
	Text suppresion;
	EnemyAI enAI;

	// Use this for initialization
	void Start () {
		enAI = GetComponent<EnemyAI> ();
		enUI = Instantiate (enUIprefab, transform.position, Quaternion.identity) as GameObject;
		enUI.transform.SetParent (GameObject.FindGameObjectWithTag ("Canvas").transform);

		Text[] texts = enUI.GetComponentsInChildren<Text> ();
		textUI = texts [0];
		morale = texts[1];
;
		suppresion = texts [2];

		//textUI = enUI.GetComponentInChildren<Text> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (show)
		{
			enUI.gameObject.SetActive (true);

			string info = enAI.aiStates.ToString ();

			textUI.text = info;

			morale.text = "morale " + enAI.charStats.morale.ToString ();

			suppresion.text = "suppresion " + enAI.charStats.suppresionLevel.ToString ();

			Vector2 screenPoint = Camera.main.WorldToScreenPoint ( transform.position);
			enUI.transform.position = screenPoint;
		}
		else
		{
			enUI.gameObject.SetActive (false);
		}
	}

	public void EnableDisableUI()
	{
		show = !show;
	}
}
