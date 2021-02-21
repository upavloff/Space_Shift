using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false; 

    public GameObject pauseMenuUi;
    public HealthCollision healthScipt;

    // Update is called once per frame
    /*void Update()
    {
        if (GameIsPaused){

        }
    }*/

    public void Pause(){
    	pauseMenuUi.SetActive(true);
    	Time.timeScale = 0f;
    	GameIsPaused = true;
    }

    public void Resume(){
		pauseMenuUi.SetActive(false);
    	Time.timeScale = 1f;
    	GameIsPaused = false;
    }

    public void QuitGame(){
    	healthScipt.Die(false /*to not send data*/);
    	Time.timeScale = 1f;
    	GameIsPaused = false;
    	pauseMenuUi.SetActive(false);
    }
}
