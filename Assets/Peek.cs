using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Peek : MonoBehaviour
{
    private float _peekTime;

	// Update is called once per frame
	void Update () {
		transform.Translate(transform.up * 16, Space.Self);
        Debug.Log(GetComponent<RectTransform>().position.x);
	    if (_peekTime >= 1f)
	    {
	        Destroy(this);
	    }
	    _peekTime += Time.deltaTime;
	}
}
