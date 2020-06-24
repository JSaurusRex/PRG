using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class background : MonoBehaviour
{
	
	//scales the background to cover all of the camera
    // Start is called before the first frame update
    void Start()
    {
        Vector3 zerozero = Camera.main.ScreenToWorldPoint(Vector3.zero);
	Vector3 screen = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height));
	
	Vector3 scale = screen - zerozero;
	transform.localScale = scale * 0.8f;
	transform.localPosition = new Vector3(0, 0, 15);
    }
}
