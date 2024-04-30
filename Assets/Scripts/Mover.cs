using UnityEngine;
using Oculus.Interaction;

public class Mover : MonoBehaviour
{
    public float verticalSpeed = 5f;          // Constant vertical speed (upward movement)
    public float targetHeight = 4f;           // Height at which to stop accelerating
    public float horizontalMagnitude = 0.5f;  // Maximum horizontal deviation
    public float rotationSpeed = 90f;         // Rotation speed in degrees per second (around the Z-axis)
    public float maxVerticalSpeed = 10f;      // Maximum vertical speed after acceleration
    public float smoothTime = 1.0f;         
    public float removalHeight = 10f;
    public GameObject animationPrefab;        // Animation prefab to spawn on poke
    private float currentSpeed;               // Current upward speed of the object
    private float originalZ;                  // Original Z position to use as the base for left/right movement
    private float horizontalSpeed = 0.0f;
    private PokeInteractable plate;
    public int basePoints = 10;               // Default points for normal objects
    public int extraPoints = 0;

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
        originalZ = transform.position.z;     // Remember the original Z position at start
        currentSpeed = verticalSpeed;         // Set the initial upward speed
        plate.WhenInteractorAdded.Action += OnPoked; // Subscribe to the poke event
    }

    void Update()
    {
        checkForRemoval();
        // Manage vertical movement
        if (transform.position.y < targetHeight)
        {
            currentSpeed = maxVerticalSpeed;
        } else {
            currentSpeed = verticalSpeed;
        }
        transform.position += Vector3.up * currentSpeed * Time.deltaTime;

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
            
            if (ScoreManager.Instance.Score > 0)
            {
                ScoreManager.Instance.AddPoints(-20);
            }
            
            Destroy(gameObject);  // Remove the object when it reaches the removal height
        }
    }

    private void OnPoked(PokeInteractor pokeInteractor)
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddPoints(basePoints + extraPoints);  // Add points to the score
        }
        
        // Instantiate the animation GameObject and destroy it after 1 second
        if (animationPrefab != null)
        {
            Debug.Log("AAAAAAAAAAA");
            var animationInstance = Instantiate(animationPrefab, transform.position, Quaternion.identity);
            Destroy(animationInstance, 1.0f);
        }
        
        Destroy(gameObject);
    }
}
