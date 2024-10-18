using UnityEngine;
using Oculus.Interaction;

public class Mover : MonoBehaviour
{
    public float verticalSpeed = 5f;  
    public float targetHeight = 4f;  
    public float horizontalMagnitude = 2.5f;  
    public float rotationSpeed = 90f;  
    public float maxVerticalSpeed = 10f;  
    public float smoothTime = 1.0f;  
    public float removalHeight = 10f;  
    public GameObject animationPrefab;  
    public float trackHitHeight = 3f;  

    private float currentSpeed;  
    private float originalZ;  
    private float horizontalSpeed = 0.0f;
    private PokeInteractable plate;
    public int basePoints = 10;  
    public int extraPoints = 0;
    private ControllerManager controllerManager;
    private bool isPoked = false;
    private bool hasSlowedDown = false;  
    private float slowDownTime; 
    private bool isSpecial = false;  
    private float trackStartTime;  
    private float hitHeight;  
    private LevelManager levelManager;
     private bool isPaused = false; 

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
        originalZ = transform.position.z;  
        currentSpeed = verticalSpeed;  
        plate.WhenInteractorAdded.Action += OnPoked; 
    }

    void Update()
    {
        if (controllerManager != null && controllerManager.IsPaused()) return;
        if (isPoked || isPaused) return; 

        checkForRemoval();

        if (transform.position.y < targetHeight)
        {
            currentSpeed = maxVerticalSpeed;
        }
        else
        {
            if (!hasSlowedDown)
            {
                hasSlowedDown = true;
                slowDownTime = Time.time; 
            }
            currentSpeed = verticalSpeed;
        }
        transform.position += Vector3.up * currentSpeed * Time.deltaTime;

        if (transform.position.y >= trackHitHeight && trackStartTime == 0f)
        {
            trackStartTime = Time.time;
        }

        float targetZPosition = originalZ + Random.Range(-horizontalMagnitude, horizontalMagnitude);
        float newPositionZ = Mathf.SmoothDamp(transform.position.z, targetZPosition, ref horizontalSpeed, smoothTime);
        transform.position = new Vector3(transform.position.x, transform.position.y, newPositionZ);

        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }

    void checkForRemoval()
    {
        if (transform.position.y > removalHeight)
        {
            Destroy(gameObject);  
        }
    }

    private void OnPoked(PokeInteractor pokeInteractor)
    {
        if (levelManager != null) levelManager.OnObjectHit();
        
        if (controllerManager != null && controllerManager.IsPaused()) return;
        if (isPoked) return;

        isPoked = true; 

        float hitTime = 0f;
        hitHeight = transform.position.y; 

        if (ScoreManager.Instance != null)
        {
            int timeBonus = 0;

            if (!isSpecial)
            {
                if (trackStartTime == 0f)
                {
                    float heightDifference = targetHeight - transform.position.y;
                    hitTime = heightDifference / maxVerticalSpeed; 

                    timeBonus = 10;
                }
                else
                {
                    hitTime = Time.time - trackStartTime;

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

            ScoreManager.Instance.AddPoints(basePoints + extraPoints + timeBonus);
        }

        ScoreManager.Instance.UpdateAverageHitHeight(hitHeight);

        //Debug.Log("Hit time: " + hitTime + "\n" + "Hit height: " + hitHeight);

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

    public void Disable()
    {
        isPaused = true;
        gameObject.SetActive(false);
    }

    public void Enable()
    {
        isPaused = false;
        gameObject.SetActive(true);
    }
}

