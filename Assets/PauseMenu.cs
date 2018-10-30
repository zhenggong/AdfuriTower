using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour {

    public static bool GameIsPaused = false;

    public GameObject pauseMenuUI;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.touchCount > 0) {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Ended) {
                if (GameIsPaused) {
                    Resume();
                }else{
                    Pause();

                }
            }
        }
	}

    private void Resume() {

    }

    private void Pause() {

    }

}
