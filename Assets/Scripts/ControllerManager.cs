using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControllerManager : MonoBehaviour
{
    public GameObject menu;
    public Transform ovrCameraRig; // Reference to the OVR Camera Rig
    private bool isPaused = false;
    public float menuDistance = 0.5f; // Distance in front of the player to place the menu

    // Update is called once per frame
    void Update()
    {
        if ((OVRInput.GetUp(OVRInput.Button.One) || OVRInput.GetUp(OVRInput.Button.Start)))
        {
            if (isPaused == false)
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0;
        PositionMenuInFrontOfPlayer();
        menu.SetActive(true);
    }

    public void ContinueGame()
    {
        if (isPaused)
        {
            Time.timeScale = 1;
            menu.SetActive(false);
            isPaused = false;
        }
    }

    public void ToMainMenu()
    {
        Time.timeScale = 1;
        Destroy(gameObject);
        SceneManager.LoadScene("Menu");
    }

    public bool IsPaused()
    {
        return isPaused;
    }

    private void PositionMenuInFrontOfPlayer()
    {
        if (ovrCameraRig != null && menu != null)
        {
            // Calculate the new position in front of the player's view
            Vector3 newPosition = ovrCameraRig.position + ovrCameraRig.forward * menuDistance;
            menu.transform.position = newPosition;

            // Rotate the menu to face the player
            Quaternion newRotation = Quaternion.LookRotation(menu.transform.position - ovrCameraRig.position);
            menu.transform.rotation = newRotation;
        }
    }
}
