using UnityEngine;
using Oculus.Interaction;

public class Mover : MonoBehaviour
{
    public float verticalSpeed = 5f;  // Default constant vertical speed (upward movement)
    public float targetHeight = 4f;  // Height at which to stop accelerating
    public float horizontalMagnitude = 2.5f;  // Default maximum horizontal deviation
    public float rotationSpeed = 90f;  // Rotation speed in degrees per second (around the Z-axis)
    public float maxVerticalSpeed = 10f;  // Maximum vertical speed after acceleration
    public float smoothTime = 1.0f;  // Smooth time for horizontal movement
    public float removalHeight = 10f;  // Height at which to remove the object
    public GameObject animationPrefab;  // Animation prefab to spawn on poke
    public float trackHitHeight = 3f;  // Height from which to start tracking the hit time

    private float currentSpeed;  // Current upward speed of the object
    private float originalZ;  // Original Z position to use as the base for left/right movement
    private float horizontalSpeed = 0.0f;
    private PokeInteractable plate;
    public int basePoints = 10;  // Default points for normal objects
    public int extraPoints = 0;
    private ControllerManager controllerManager;
    private bool isPoked = false;
    private bool hasSlowedDown = false;  // Flag to check if the object has slowed down
    private float slowDownTime;  // Time when the object slowed down
    private bool isSpecial = false;  // Flag to check if the object is special
    private float trackStartTime;  // Time when the object reaches trackHitHeight
    private float hitHeight;  // Height at which the object was hit
    private LevelManager levelManager;

    void Awake()
    {
        plate = GetComponentInChildren<PokeInteractable>();
        if (plate == null)
        {
            Debug.LogError("No PokeInteractable found on the GameObject or its children!");
        }
    }

    private void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();
        controllerManager = FindObjectOfType<ControllerManager>();
        originalZ = transform.position.z;  // Remember the original Z position at start
        currentSpeed = verticalSpeed;  // Set the initial upward speed
        plate.WhenInteractorAdded.Action += OnPoked;  // Subscribe to the poke event
    }

    void Update()
    {
        if (controllerManager != null && controllerManager.IsPaused()) return;
        if (isPoked) return;  // Skip updates if the object is poked

        checkForRemoval();

        // Manage vertical movement
        if (transform.position.y < targetHeight)
        {
            currentSpeed = maxVerticalSpeed;
        }
        else
        {
            if (!hasSlowedDown)
            {
                hasSlowedDown = true;
                slowDownTime = Time.time;  // Record the time when the object slows down
            }
            currentSpeed = verticalSpeed;
        }
        transform.position += Vector3.up * currentSpeed * Time.deltaTime;

        // Record the time when the object reaches the trackHitHeight
        if (transform.position.y >= trackHitHeight && trackStartTime == 0f)
        {
            trackStartTime = Time.time;
        }

        // Continuous smooth random left and right movement using SmoothDamp
        float targetZPosition = originalZ + Random.Range(-horizontalMagnitude, horizontalMagnitude);
        float newPositionZ = Mathf.SmoothDamp(transform.position.z, targetZPosition, ref horizontalSpeed, smoothTime);
        transform.position = new Vector3(transform.position.x, transform.position.y, newPositionZ);

        // Continuous rotation around the Z-axis
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }

    void checkForRemoval()
    {
        if (transform.position.y > removalHeight)
        {
            Destroy(gameObject);  // Remove the object when it reaches the removal height
        }
    }

    private void OnPoked(PokeInteractor pokeInteractor)
    {
        if (levelManager != null) levelManager.OnObjectHit();
        
        if (controllerManager != null && controllerManager.IsPaused()) return;
        if (isPoked) return;

        isPoked = true;  // Set flag to indicate the object is poked

        float hitTime = 0f;
        hitHeight = transform.position.y;  // Record the height at which the object was hit

        if (ScoreManager.Instance != null)
        {
            int timeBonus = 0;

            if (!isSpecial)
            {
                if (trackStartTime == 0f)
                {
                    // Calculate hit time based on the height difference
                    float heightDifference = targetHeight - transform.position.y;
                    hitTime = heightDifference / maxVerticalSpeed;  // Assuming constant speed

                    // If the object is poked before it slows down, give maximum bonus points
                    timeBonus = 10;
                }
                else
                {
                    // Calculate the time taken to hit the target after it reached the trackHitHeight
                    hitTime = Time.time - trackStartTime;

                    // Calculate bonus points based on time taken to hit the target
                    if (hitTime <= 5f)
                    {
                        timeBonus = 10;
                    }
                    else if (hitTime <= 5.5f)
                    {
                        timeBonus = 5;
                    }
                }
                ScoreManager.Instance.UpdateAverageHitTime(hitTime);
            }

            // Add points to the score
            ScoreManager.Instance.AddPoints(basePoints + extraPoints + timeBonus);
        }

        ScoreManager.Instance.UpdateAverageHitHeight(hitHeight);

        //Debug.Log("Hit time: " + hitTime + "\n" + "Hit height: " + hitHeight);

        // Instantiate the animation GameObject and destroy it after 1 second
        if (animationPrefab != null)
        {
            var animationInstance = Instantiate(animationPrefab, transform.position, Quaternion.identity);
            Destroy(animationInstance, 1.0f);
        }

        Destroy(gameObject);
    }

    public void SetSpeed(float newSpeed)
    {
        verticalSpeed = newSpeed;
    }

    public void SetHorizontalMagnitude(float newHorizontalMagnitude)
    {
        horizontalMagnitude = newHorizontalMagnitude;
    }

    public void SetProperties(float newVerticalSpeed, float newMaxVerticalSpeed, float newHorizontalMagnitude)
    {
        verticalSpeed = newVerticalSpeed;
        maxVerticalSpeed = newMaxVerticalSpeed;
        horizontalMagnitude = newHorizontalMagnitude;
    }

    public void SetSpecial(bool special)
    {
        isSpecial = special;
    }
}
