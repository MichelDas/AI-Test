using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverPosition : MonoBehaviour {

	public List<Coverbase> FrontPositions = new List<Coverbase>();
	public List<Coverbase> BackPositions = new List<Coverbase> ();

	void Start()
	{
		for (int i = 0; i < BackPositions.Count ; i++)
		{
			BackPositions [i].backPos = true;
		}
	}
}

[System.Serializable]
public class Coverbase
{
	public bool occupied;
	public Transform positionObject;
	public bool backPos;
}
