using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateLight : MonoBehaviour {

	// Update is called once per frame
	void Update () {
		transform.Rotate(new Vector3(0, Time.deltaTime * 90, 0));
	}
}
