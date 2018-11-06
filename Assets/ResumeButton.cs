using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResumeButton : MonoBehaviour {
    PauseButton btn;
    public GameObject pauseMenuUI;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnClick() {
        Debug.Log("押された");
        pauseMenuUI = GameObject.Find("PauseMenu");
        if (pauseMenuUI)
        {
            Debug.Log(pauseMenuUI);
        }
        else
        {
            Debug.Log("No game object called wibble found");
        }
        pauseMenuUI.transform.Translate(566f, 0, 0, 0);
    }
}
