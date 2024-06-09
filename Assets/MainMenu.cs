using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
        SceneManager.LoadScene("WhackAMmole");
    }
}
