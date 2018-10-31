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
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
        //if (Input.touchCount > 0) {
        //    Touch touch = Input.GetTouch(0);
        //    if (touch.phase == TouchPhase.Ended) {
        //        if (GameIsPaused) {
        //            Resume();
        //        }else{
        //            Pause();

        //        }
        //    }
        //}
    }

    public void Resume() {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void Pause() {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

}
