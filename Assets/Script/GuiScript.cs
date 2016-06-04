using UnityEngine;
using System.Collections;

public class GuiScript : MonoBehaviour {

    int GameMode;

    float CenterScreenX = Screen.width / 2;
    float CenterScreenY = Screen.height / 2;

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    void OnGUI()
    {
        GameMode = GetComponent<FieldScript>().GameState;


        switch (GameMode)
        {
            case 0:
                Rect menuLocation = new Rect(new Vector2(CenterScreenX - 450, CenterScreenY + 100), new Vector2(250, 300));
                GUI.Box(menuLocation, "");
                Rect buttonLocation = new Rect(new Vector2(menuLocation.x, menuLocation.y), new Vector2(200, 100));
                GUI.Button(buttonLocation, "RANCOM");
                    

                break;
            case 1:
                
                break;
        }

    }
}
