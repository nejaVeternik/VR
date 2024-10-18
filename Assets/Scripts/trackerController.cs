using System.Collections.Generic;
using UnityEngine;

public class TrackerController : MonoBehaviour
{
    public static TrackerController Instance;
    public bool loggingEnabled = false;

    private List<float> reactionTimes = new List<float>();

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

    public void RecordReactionTime(float time)
    {
        reactionTimes.Add(time);
        if (loggingEnabled) Debug.Log("Reaction Time Recorded: " + time);
    }

    public List<float> GetReactionTimes()
    {
        return reactionTimes;
    }

    // You can add more methods to process the reaction times if needed
}
