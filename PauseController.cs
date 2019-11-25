using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseController : MonoBehaviour {
    public GameObject pause;
    public void TogglePause()
    {
        pause.SetActive(!pause.activeSelf);
        if (!pause.activeSelf)
            Time.timeScale = 1;
        else
            Time.timeScale = 0;
    }

    void Update()
    {
        if (Input.GetKeyDown("escape"))
            TogglePause();
    }
}
