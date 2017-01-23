using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class dock : MonoBehaviour
{
    public GameObject Target;
    public AudioClip Clap;
    public AudioClip Won;
    public bool Done { get; private set; }

    void Start()
    {
        var hint = transform.parent.FindChild("DockHint").gameObject;

        var sprite = Target.GetComponentInChildren<SpriteRenderer>();
        var mesh = Target.GetComponentInChildren<MeshFilter>();

        if (sprite)
        {
            var sr = hint.AddComponent<SpriteRenderer>();
            sr.sprite = sprite.sprite;
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0.25f);
            hint.transform.localScale = sprite.transform.localScale;
            hint.transform.rotation = sprite.transform.rotation;
        }
        else if (mesh)
        {
            var mr = hint.AddComponent<MeshRenderer>();
            var mf = hint.AddComponent<MeshFilter>();
            var mat = Resources.Load<Material>("ParkHere");
            mf.sharedMesh = mesh.sharedMesh;
            mr.material = mat;
            hint.transform.localScale = mesh.transform.localScale;
            hint.transform.rotation = mesh.transform.rotation;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == Target)
        {
            transform.parent.FindChild("DockHint").gameObject.SetActive(false);
            Target.GetComponentInChildren<Float>().ApplyForces = false;
            Debug.Log("Hurray");
            GetComponent<AudioSource>().PlayOneShot(Clap);
            Done = true;
            if (FindObjectsOfType<dock>().All(x => x.Done))
            {
                FindObjectOfType<Master>().PlayerWon();
                GetComponent<AudioSource>().PlayOneShot(Won);
            }
            // won
        }
    }
}
