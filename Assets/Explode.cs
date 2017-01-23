using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explode : MonoBehaviour
{
    public AudioClip Scream;

    private void OnTriggerEnter(Collider other)
    {
        if (other.name != "Target" && other.tag != "Water")
        {
            Debug.Log("boom: " + other.gameObject.name);
            GetComponent<Float>().ApplyForces = false;
            StartCoroutine(Sink());
            GetComponent<AudioSource>().PlayOneShot(Scream, 0.66f);
        }
    }

    IEnumerator Sink()
    {
        while (!GetComponent<Float>().Sunk)
        {
            transform.Rotate(new Vector3(3, 0, 0));
            yield return new WaitForSeconds(0.01f);
        }
        FindObjectOfType<Master>().PlayerLost();
        yield return null;
    }
}
