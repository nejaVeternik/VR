using UnityEngine;
using Oculus.Interaction;

public class Mover : MonoBehaviour
{
    public float verticalSpeed = 5f;  // Constant vertical speed (upward movement)
    public float targetHeight = 4f;  // Height at which to stop accelerating
    public float horizontalMagnitude = 0.5f;  // Maximum horizontal deviation
    public float rotationSpeed = 90f;  // Rotation speed in degrees per second (around the Z-axis)
    public float maxVerticalSpeed = 10f;  // Maximum vertical speed after acceleration
    public float smoothTime = 1.0f;  // Smooth time for horizontal movement
    public float removalHeight = 10f;  // Height at which to remove the object
    public GameObject animationPrefab;  // Animation prefab to spawn on poke
    private float currentSpeed;  // Current upward speed of the object
    private float originalZ;  // Original Z position to use as the base for left/right movement
    private float horizontalSpeed = 0.0f;
    private PokeInteractable plate;
    public int basePoints = 10;  // Default points for normal objects
    public int extraPoints = 0;
    private ControllerManager controllerManager;
    private bool isPoked = false;

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
            Destroy(gameObject);  // Remove the object when it reaches the removal height
        }
    }

    private void OnPoked(PokeInteractor pokeInteractor)
    {
        if (controllerManager != null && controllerManager.IsPaused()) return;
        if (isPoked) return;

        isPoked = true;  // Set flag to indicate the object is poked

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddPoints(basePoints + extraPoints);  // Add points to the score
        }

        // Instantiate the animation GameObject and destroy it after 1 second
        if (animationPrefab != null)
        {
            var animationInstance = Instantiate(animationPrefab, transform.position, Quaternion.identity);
            Destroy(animationInstance, 1.0f);
        }

        Destroy(gameObject);
    }
}
