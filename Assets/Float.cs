using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class Float : MonoBehaviour
{
    public bool ApplyForces;
    private Water _water;
	private float rotCache = 0.0f;
	private bool isAdd = true;
	// Use this for initialization
	void Start ()
	{
	    _water = FindObjectOfType<Water>();
		rotCache = transform.rotation.x;
	}

	// Update is called once per frame
	void Update ()
	{
	    Vector3 posInWater = _water.transform.InverseTransformPoint(transform.position);
	    transform.position = new Vector3(transform.position.x, -_water.GetHeight(posInWater) / 6, transform.position.z);
		var currentRot = transform.rotation.x;
		float currChange = currentRot - rotCache;
		bool speed= currChange>0.05||currChange<-0.05?true:false;

		if (isAdd) {
			Debug.Log (speed);
			transform.Rotate(new Vector3(speed?2.0f:0.25f,0, 0));

		}
		else {
			
			transform.Rotate(new Vector3(speed?-2.0f:-0.25f,0, 0));

		}

		if (rotCache+0.05f<currentRot  ) {
			isAdd =false;
		}
		else if (rotCache-0.05f>currentRot ) {
			isAdd =true;

		}
	    if (ApplyForces)
	    {
	        var force = _water.GetForce(posInWater);
	        transform.Translate(force * Time.deltaTime, Space.World);
			transform.Rotate(new Vector3(force.x * Time.deltaTime*360,0,0));
	    }
	}
}
