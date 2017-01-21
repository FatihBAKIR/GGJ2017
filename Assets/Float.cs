using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class Float : MonoBehaviour
{
    public bool ApplyForces;
    private Water _water;

	// Use this for initialization
	void Start ()
	{
	    _water = FindObjectOfType<Water>();
	}

	// Update is called once per frame
	void Update ()
	{
	    Vector3 posInWater = _water.transform.InverseTransformPoint(transform.position);
	    transform.position = new Vector3(transform.position.x, -_water.GetHeight(posInWater) / 6, transform.position.z);

	    if (ApplyForces)
	    {
	        var force = _water.GetForce(posInWater);
	        transform.Translate(force * Time.deltaTime, Space.World);
	    }
	}
}
