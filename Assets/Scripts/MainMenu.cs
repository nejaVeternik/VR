using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public void loadGait() {
        SceneManager.LoadScene("Gait");
    }

    public void loadMemory() {
        SceneManager.LoadScene("Memory");
    }

    public void loadPark() {
        SceneManager.LoadScene("Park");
    }

    public void loadWhackAMole() {
        SceneManager.LoadScene("WhackAMole");
    }

    public void quitApplication() {
        Application.Quit();
    }
}
