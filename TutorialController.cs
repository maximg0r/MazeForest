using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour {
    public GameObject tutorial_screen;

	void Start () {
		if (GameData.show_tutorial)
        {
            ShowTutorial();
        }
	}

    public void ShowTutorial()
    {
        Time.timeScale = 0;
        tutorial_screen.SetActive(true);
    }

    public void CloseTutorial()
    {
        tutorial_screen.SetActive(false);
        Time.timeScale = 1;
        GameData.show_tutorial = false;
    }
}
