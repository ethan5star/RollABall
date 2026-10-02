using UnityEngine;

public class CollectibleController : MonoBehaviour
{
    // Particle effect prefab to spawn upon collection$$$
    [SerializeField] private GameObject collectParticlePrefab;

    // Controls how fast the object rotates on its Y-axis in degrees per second
    [SerializeField] private float rotationSpeed = 90f;

    // Reference to the GameManager script handling game state
    private GameManager gameManager;

    private AudioClip collectSound;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // FindObjectOfType provides backward and forward compatibility across Unity versions
        gameManager = Object.FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        // Keeps Update clean by calling a separate helper method for rotation logic
        RotateObject();
    }

    // Rotates the object continuously around its vertical Y-axis over time
    private void RotateObject()
    {
        // Rotates on the Y-axis scaled by frame time to maintain frame-rate independent speed
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
    }

    // Called when this object collides with another collider set as a trigger
    private void OnTriggerEnter(Collider other)
    {
        // Only executes if the colliding object is tagged as Player
        if (other.CompareTag("Player"))
        {
            // Verifies gameManager exists before calling methods to prevent NullReferenceException
            if (gameManager != null)
            {
                gameManager.UpdateRemaining();
            }

            // Checks if sound clip exists before playing audio
            if (collectSound != null)
            {
                AudioSource.PlayClipAtPoint(collectSound, transform.position);
            }

            // Checks if particle prefab exists before spawning
            if (collectParticlePrefab != null)
            {
                Instantiate(collectParticlePrefab, transform.position, Quaternion.identity);
            }

            // Destroys this collectible object
            Destroy(gameObject);
        }
    }
}
