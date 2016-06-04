using UnityEngine;
using System.Collections;

public class ButtonScript : MonoBehaviour {

    public Camera MainCamera;
    public GameObject field;
    public string buttonFunction;
	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
	
	}
        

    void OnMouseDown()
    {
        switch (buttonFunction)
        {
            case "rotate":
                field.GetComponent<FieldScript>().Rotate();
                break;
            case "random":
                field.GetComponent<FieldScript>().GenerateRandomField();
                break;
            case "clear":
                field.GetComponent<FieldScript>().FieldClear();
                break;
            case "quit":
                Application.Quit();
                break;
            case "battle":
                if (!field.GetComponent<FieldScript>().ShipsExist())
                {
                    MainCamera.transform.position = new Vector3(75, -5, -10);
                    field.GetComponent<FieldScript>().CopyField();
                }
                break;
            case "restart":
                MainCamera.transform.position = new Vector3(0, 0, -10);
                field.GetComponent<FieldScript>().FieldClear();
                field.GetComponent<FieldScript>().GenerateRandomField();
                break;
        }
    }
}
