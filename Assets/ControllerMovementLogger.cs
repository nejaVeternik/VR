using UnityEngine;

public class ControllerMovementLogger : MonoBehaviour
{
    public bool loggingEnabled = false;
    public OVRInput.Controller controller; // Select the controller (e.g., OVRInput.Controller.RTouch)
    public int bufferSize = 1000; // Set the size of the circular buffer
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
        // Get the local controller velocity
        Vector3 controllerVelocity = OVRInput.GetLocalControllerVelocity(controller);

        // Check if the velocity is not zero
        if (controllerVelocity != Vector3.zero)
        {
            // If the buffer is full, subtract the speed of the element being overwritten
            if (validSampleCount >= bufferSize)
            {
                totalSpeed -= movementDataBuffer[bufferIndex].velocity.magnitude;
            }
            else
            {
                validSampleCount++;
            }

            // Create a new movement data entry
            MovementData data = new MovementData
            {
                timestamp = Time.time,
                velocity = controllerVelocity
            };

            // Add the data entry to the circular buffer
            movementDataBuffer[bufferIndex] = data;
            totalSpeed += controllerVelocity.magnitude;

            // Move to the next index in the buffer
            bufferIndex = (bufferIndex + 1) % bufferSize;

            if (loggingEnabled) Debug.Log($"Controller Velocity: {controllerVelocity.magnitude} m/s");
        }
        if (loggingEnabled) DisplayMovementStats();
    }

    // Class to store movement data
    private class MovementData
    {
        public float timestamp;
        public Vector3 velocity;
    }

    // Method to analyze or display stored movement data
    public void DisplayMovementStats()
    {
        float averageSpeed = validSampleCount > 0 ? totalSpeed / validSampleCount : 0f;
        Debug.Log($"Total Valid Samples: {validSampleCount}");
        Debug.Log($"Average Speed: {averageSpeed} m/s");
    }
}
