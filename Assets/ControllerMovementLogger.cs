using UnityEngine;

public class ControllerMovementLogger : MonoBehaviour
{
    public bool loggingEnabled = false;
    public OVRInput.Controller controller;
    public int bufferSize = 1000;
    private MovementData[] movementDataBuffer;
    private int bufferIndex = 0;
    private int validSampleCount = 0;
    private float totalSpeed = 0f;

    private void Start()
    {
        movementDataBuffer = new MovementData[bufferSize];
    }

    private void Update()
    {
        Vector3 controllerVelocity = OVRInput.GetLocalControllerVelocity(controller);

        if (controllerVelocity != Vector3.zero)
        {
            if (validSampleCount >= bufferSize)
            {
                totalSpeed -= movementDataBuffer[bufferIndex].velocity.magnitude;
            }
            else
            {
                validSampleCount++;
            }

            MovementData data = new MovementData
            {
                timestamp = Time.time,
                velocity = controllerVelocity
            };

            movementDataBuffer[bufferIndex] = data;
            totalSpeed += controllerVelocity.magnitude;

            bufferIndex = (bufferIndex + 1) % bufferSize;

            if (loggingEnabled) Debug.Log($"Controller Velocity: {controllerVelocity.magnitude} m/s");
        }
        if (loggingEnabled) DisplayMovementStats();
    }

    private class MovementData
    {
        public float timestamp;
        public Vector3 velocity;
    }

    public void DisplayMovementStats()
    {
        float averageSpeed = validSampleCount > 0 ? totalSpeed / validSampleCount : 0f;
        Debug.Log($"Total Valid Samples: {validSampleCount}");
        Debug.Log($"Average Speed: {averageSpeed} m/s");
    }
}
