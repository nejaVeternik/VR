using UnityEngine;

public class GameController : MonoBehaviour
{
    void Update()
    {
        if (OVRInput.GetUp(OVRInput.Button.Two))
        {
            float headsetStability = StabilityTracker.Instance.CalculateHeadsetStability();
            float leftControllerStability = StabilityTracker.Instance.CalculateLeftControllerStability();
            float rightControllerStability = StabilityTracker.Instance.CalculateRightControllerStability();

            Debug.Log($"Headset Stability: {headsetStability}");
            Debug.Log($"Left Controller Stability: {leftControllerStability}");
            Debug.Log($"Right Controller Stability: {rightControllerStability}");
        }
    }
}
