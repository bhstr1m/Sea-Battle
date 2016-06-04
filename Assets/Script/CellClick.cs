using UnityEngine;
using System.Collections;

public class CellClick : MonoBehaviour {

    public GameObject parent = null;

    public int coordX, coordY;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnMouseDown() {
        if (parent != null)
        {
            parent.GetComponent<FieldScript>().Click(coordX, coordY);
        }
    }
}
