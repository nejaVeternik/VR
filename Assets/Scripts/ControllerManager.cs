using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControllerManager : MonoBehaviour
{
    public GameObject menu;
    private bool isPaused = false;

    // Update is called once per frame
    void Update()
    {
        if ((OVRInput.GetUp(OVRInput.Button.One) || OVRInput.GetUp(OVRInput.Button.Start)))
        {
            if (isPaused == false) {
                pauseGame();
            }
    
            //OVRInput.SetControllerVibration(1, 0.6f, OVRInput.Controller.RTouch);
        }
    }

    public void pauseGame() {
        isPaused = true;
        Time.timeScale = 0;
        menu.SetActive(true);
    }

    public void continueGame() {
        if (isPaused)
        {
            Time.timeScale = 1;
            menu.SetActive(false);
            isPaused = false;
        }
    }

    public void toMainMenu() {
        Time.timeScale = 1;
        Destroy(gameObject);
        SceneManager.LoadScene("Menu");
    }
}