using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartGame : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{
	    GameObject.Find("StartButton").GetComponent<Button>().onClick.AddListener(() =>
	    {
	        SceneManager.LoadScene("level1");
	    });
	}
	
	// Update is called once per frame
	void Update () {

		
	}
}
