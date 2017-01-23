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
	private bool acc;

    // Use this for initialization
    void Start()
    {
        _water = FindObjectOfType<Water>();
        rotCache = transform.rotation.x;
    }

    private float _curSpeed = 0.25f;
    public bool Sunk { get; private set; }

    private float _lastScream = -5;

    // Update is called once per frame
    void Update()
    {
        Vector3 posInWater = _water.transform.InverseTransformPoint(transform.position);
        transform.position = new Vector3(transform.position.x, -_water.GetHeight(posInWater) / 6, transform.position.z);
        var currentRot = transform.rotation.x;

        var currChange = currentRot - rotCache;
        var speed = currChange > 0.05 || currChange < -0.05;

        if (currChange > 0.3f || currChange < -0.3f && ApplyForces)
        {
            if (Time.timeSinceLevelLoad - _lastScream >= GetComponent<Explode>().Scream.length + 5)
            {
                GetComponent<AudioSource>().PlayOneShot(GetComponent<Explode>().Scream, 0.33f);
                _lastScream = Time.timeSinceLevelLoad;
            }
        }

		if(currChange > 0.4f || currChange < -0.4f && ApplyForces){
			transform.Translate (new Vector3 (0, -0.5f, 0));
			rotCache = transform.rotation.x;
			ApplyForces = false;
		    Sunk = true;
		    FindObjectOfType<Master>().PlayerLost();
		}
        if (isAdd)
		{   _curSpeed += acc?0.002f:-0.002f;
            transform.Rotate(new Vector3(speed ? 2.0f : _curSpeed, 0, 0));
            //_curSpeed *= 0.98f;
        }
        else
        {
			_curSpeed -= acc?0.002f:-0.002f;
            transform.Rotate(new Vector3(speed ? -2.0f : _curSpeed, 0, 0));
            //_curSpeed *= 0.98f;
        }

		if (rotCache + 0.05f < currentRot) {
			isAdd = false;
			_curSpeed = 0.0f;
			acc = true;
		} else if (rotCache - 0.05f > currentRot) {
			isAdd = true;
			_curSpeed-= 0.0f;
			acc = true;
		} 
		if (acc&&((currentRot>0&&isAdd)||currentRot<0&&!isAdd)) {
			acc = false;
		}

        if (ApplyForces)
        {
            var force = _water.GetForce(posInWater);
            if (force.sqrMagnitude > 0.0001)
            {
                GetComponent<AudioSource>().volume = 1;
            }
            else
            {
                GetComponent<AudioSource>().volume = 0.4f;
            }
            transform.Translate(force * Time.deltaTime, Space.World);
            transform.Rotate(new Vector3(force.x * Time.deltaTime * 180, 0, 0));
        }
    }
}
