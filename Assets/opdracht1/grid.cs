using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grid : MonoBehaviour
{
	public GameObject cell;
	public GameObject alternativeCell;
	public Vector2 dimensions;
	public Vector3 startingPos;
	public Vector2 PFStart;
	public Vector2 PFEnd;
	public GameObject [,] objgrid;
    // Start is called before the first frame update
    void Start()
    {
	    Camera.main.orthographicSize = Mathf.Max(dimensions.x,dimensions.y) / 2;
	 startingPos = Camera.main.ScreenToWorldPoint(new Vector3(0,  Screen.height, 0));
	startingPos += new Vector3(0.5f,0.5f  -1 * dimensions.y, 0);
	    
	    bool [,] tmpgrid = new bool [(int)dimensions.x, (int)dimensions.y];
	    objgrid = new GameObject[(int)dimensions.x, (int) dimensions.y];
	    
	    int x = (int)PFStart.x, y = (int)PFStart.y;
	    int failSave = 0;
	    
	    while(failSave < 100)
	    {
		    failSave++;
		    Debug.Log(x + "," + y);
		    tmpgrid[x,y] = true;
		    if(Mathf.Abs(PFEnd.x - x) < Mathf.Abs(PFEnd.y - y)) {if(PFEnd.y < y)y--;else y++;}
		    else {if(PFEnd.x < x)x--;else x++;}
		    if(x >= dimensions.x || y >= dimensions.y || x < 0 || y < 0) break;
	    }
	    
        for(int i = 0; i < dimensions.x; i++)
	{
		for(int j =0; j < dimensions.y; j++)
		{
			Vector3 pos = startingPos + new Vector3(i, j, 10);
			if(tmpgrid[i,j]) objgrid[i,j] = Instantiate(alternativeCell, pos, Quaternion.Euler(0,0,0));
			else objgrid[i,j] = Instantiate(cell, pos, Quaternion.Euler(0,0,0));
			
		}
	}
	Debug.Log(objgrid);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
