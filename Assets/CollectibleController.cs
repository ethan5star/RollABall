/************************************************************
* COMPONENT OF: Collectible Prefabs
* REQUIRED DEPENDENCIES: Rigidbody Component
* DESCRIPTION: It listens for WASD and Arrow key presses to set 
*              vertical and horizontal directions. It uses those
*              to push the player's Rigidbody in that direction
*              at a preset force.
* AUTHOR: EEdward
* DATE WRITTEN: Sep 16 2026
* VERSION: 1.0
*************************************************************/



using UnityEngine;

public class CollectibleController : MonoBehaviour
{
   [SerializeField] private AudioClip collectSound;
   [SerializeField] private GameObject collectParticlePrefab;
   private GameManager gameManager;
   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

// Call with this object collides with a trigger
    private void OnTriggerEnter(Collider other)
    {
        // Only executes if the collision was with the Player
        if (other.CompareTag("Player"))
        {
            gameManager.UpdateRemaining();
            
            // Spawn audio at the collectible's position (auto-destroys)
            AudioSource.PlayClipAtPoint(collectSound, transform.position);

            // Spawn particles (auto-destroys if Stop Action is set to Destroy)
            Instantiate(collectParticlePrefab, transform.position, Quaternion.identity);

            // Safely destroy the collectible immediately
            Destroy(gameObject);
        }
    }

}


