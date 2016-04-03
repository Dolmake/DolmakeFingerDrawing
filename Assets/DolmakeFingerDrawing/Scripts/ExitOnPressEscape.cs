using UnityEngine;
using System.Collections;

public class ExitOnPressEscape : MonoBehaviour {

	
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            this.enabled = false;
            Application.Quit();
        }
	
	}
}
