using UnityEngine;
using System.Collections;

public class CellImgScript : MonoBehaviour 
{

	public Sprite[] imgs;
	public int imgId;
    public bool HideCell = false;

	void ChangeImgs()
	{
        if (imgId < imgs.Length)
        {
            if (HideCell && (imgId == 1))
                GetComponent<SpriteRenderer>().sprite = imgs[0];
            else
                GetComponent<SpriteRenderer>().sprite = imgs[imgId];
        }
	}
	// Use this for initialization
	void Start () 
	{
		ChangeImgs();
	}
	
	// Update is called once per frame
	void Update () 
	{
		ChangeImgs();
	}
}
