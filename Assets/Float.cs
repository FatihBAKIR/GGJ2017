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
    void Start()
    {
        _water = FindObjectOfType<Water>();
        rotCache = transform.rotation.x;
    }

    private float _curSpeed = 0.25f;

    // Update is called once per frame
    void Update()
    {
        Vector3 posInWater = _water.transform.InverseTransformPoint(transform.position);
        transform.position = new Vector3(transform.position.x, -_water.GetHeight(posInWater) / 6, transform.position.z);
        var currentRot = transform.rotation.x;

        var currChange = currentRot - rotCache;
        var speed = currChange > 0.05 || currChange < -0.05;

        if (isAdd)
        {
            transform.Rotate(new Vector3(speed ? 2.0f : _curSpeed, 0, 0));
            //_curSpeed *= 0.98f;
        }
        else
        {
            transform.Rotate(new Vector3(speed ? -2.0f : _curSpeed, 0, 0));
            //_curSpeed *= 0.98f;
        }

        if (rotCache + 0.05f < currentRot)
        {
            isAdd = false;
            _curSpeed = -0.25f;
        }
        else if (rotCache - 0.05f > currentRot)
        {
            isAdd = true;
            _curSpeed = 0.25f;
        }

        if (ApplyForces)
        {
            var force = _water.GetForce(posInWater);
            transform.Translate(force * Time.deltaTime, Space.World);
            transform.Rotate(new Vector3(force.x * Time.deltaTime * 360, 0, 0));
        }
    }
}
