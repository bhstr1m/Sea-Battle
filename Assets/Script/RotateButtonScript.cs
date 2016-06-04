using UnityEngine;
using System.Collections;

public class RotateButtonScript : MonoBehaviour {

    public GameObject parent = null;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnMouseDown() {
        if (parent != null)
        {
            parent.GetComponent<FieldScript>().Rotate();
        }
    }
}
