using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControllerManager : MonoBehaviour
{
    public GameObject menu;
    public Transform ovrCameraRig; 
    public float menuDistance = 1.5f; 
    public Vector3 torusCenter = Vector3.zero; 
    public float torusRadius = 1.8f; 

    private bool isPaused = false;
    public Spawner spawner;

    private void Start()
    {
        spawner = Spawner.Instance;
    }

    void Update()
    {
        if ((OVRInput.GetUp(OVRInput.Button.One) || OVRInput.GetUp(OVRInput.Button.Start)))
        {
            if (!isPaused)
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0;
        PositionMenuInFrontOfPlayer(menu);
        menu.SetActive(true);
        if (spawner != null)
        {
            spawner.DisableAllObjects();
        }
    }

    public void ContinueGame()
    {
        if (isPaused)
        {
            Time.timeScale = 1;
            menu.SetActive(false);
            isPaused = false;
        }
        if (spawner != null)
        {
            spawner.EnableAllObjects();
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

    public void PositionMenuInFrontOfPlayer(GameObject targetMenu)
    {
        if (ovrCameraRig != null && targetMenu != null)
        {
            Vector3 forward = ovrCameraRig.forward;
            forward.y = 0; 
            forward.Normalize();
            Vector3 newPosition = ovrCameraRig.position + forward * menuDistance;

            newPosition = ClampPositionWithinTorus(newPosition);

            targetMenu.transform.position = newPosition;

            targetMenu.transform.LookAt(new Vector3(ovrCameraRig.position.x, targetMenu.transform.position.y, ovrCameraRig.position.z));
            targetMenu.transform.Rotate(0, 180, 0); 
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
