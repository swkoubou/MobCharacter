using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopArrow : SelectArrow {

	
	new void Update () {
        base.Update();

        //実験用
        if (Input.GetKeyDown(KeyCode.T))
        {
            StopSelect();
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            StartSelect();
        }
    }
}
