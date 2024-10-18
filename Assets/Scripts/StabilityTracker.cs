using System.Collections.Generic;
using UnityEngine;

public class StabilityTracker : MonoBehaviour
{
    public static StabilityTracker Instance;

    public Transform headset;
    public Transform leftController;
    public Transform rightController;

    private List<Vector3> headsetPositions = new List<Vector3>();
    private List<Vector3> leftControllerPositions = new List<Vector3>();
    private List<Vector3> rightControllerPositions = new List<Vector3>();

    private float trackingInterval = 0.1f; // Track positions every 0.1 seconds
    private float trackingTimer = 0f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        trackingTimer += Time.deltaTime;

        if (trackingTimer >= trackingInterval)
        {
            TrackPositions();
            trackingTimer = 0f;
        }
    }

    private void TrackPositions()
    {
        if (headset != null)
        {
            headsetPositions.Add(headset.position);
        }

        if (leftController != null)
        {
            leftControllerPositions.Add(leftController.position);
        }

        if (rightController != null)
        {
            rightControllerPositions.Add(rightController.position);
        }
    }

    public float CalculateHeadsetStability()
    {
        return CalculateStability(headsetPositions);
    }

    public float CalculateLeftControllerStability()
    {
        return CalculateStability(leftControllerPositions);
    }

    public float CalculateRightControllerStability()
    {
        return CalculateStability(rightControllerPositions);
    }

    private float CalculateStability(List<Vector3> positions)
    {
        if (positions.Count < 2)
            return 0f;

        Vector3 mean = Vector3.zero;
        foreach (var pos in positions)
        {
            mean += pos;
        }
        mean /= positions.Count;

        float variance = 0f;
        foreach (var pos in positions)
        {
            variance += (pos - mean).sqrMagnitude;
        }
        variance /= positions.Count;

        return Mathf.Sqrt(variance); // Standard deviation as a measure of stability
    }
}
