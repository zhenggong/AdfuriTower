using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseButton : MonoBehaviour {
    PauseButton btn;
    public GameObject pauseMenuUI;
	// Use this for initialization
	void Start () {
        btn = GetComponent<PauseButton>();

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

    }
}
