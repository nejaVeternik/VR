using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControllerManager : MonoBehaviour
{
    public GameObject menu;
    public Transform ovrCameraRig; // Reference to the OVR Camera Rig
    public float menuDistance = 1.5f; // Distance in front of the player to place the menu
    public Vector3 torusCenter = Vector3.zero; // Center of the torus
    public float torusRadius = 1.8f; // Radius of the torus bounds

    private bool isPaused = false;

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
            Vector3 forward = ovrCameraRig.forward;
            forward.y = 0; // Ignore vertical component
            forward.Normalize();
            Vector3 newPosition = ovrCameraRig.position + forward * menuDistance;

            // Clamp the new position within the torus bounds (ignore Y for bounds)
            newPosition = ClampPositionWithinTorus(newPosition);

            menu.transform.position = newPosition;

            // Rotate the menu to face the player
            menu.transform.LookAt(ovrCameraRig);
            menu.transform.Rotate(0, 180, 0); // Rotate to face the player correctly
        }
    }

    private Vector3 ClampPositionWithinTorus(Vector3 position)
    {
        Vector3 offset = position - torusCenter;
        float horizontalDistance = new Vector3(offset.x, 0, offset.z).magnitude;

        if (horizontalDistance > torusRadius)
        {
            offset.x *= torusRadius / horizontalDistance;
            offset.z *= torusRadius / horizontalDistance;
            position.x = torusCenter.x + offset.x;
            position.z = torusCenter.z + offset.z;
        }

        return position;
    }
}
