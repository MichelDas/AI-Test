using UnityEngine;
using System.Collections;


[ExecuteInEditMode]
public class EnableDisableRenderers : MonoBehaviour {

	public bool activate;
	public bool diActivate;

	public MeshRenderer[] meshRens;

	// Update is called once per frame
	void Update () {

		if (activate) {
			foreach (MeshRenderer ren in meshRens) {
				ren.enabled = true;
			}
			activate = false;
		}

		if (diActivate) {
			foreach (MeshRenderer ren in meshRens) {
				ren.enabled = false;
			}

			diActivate = false;
		}

	}
}
