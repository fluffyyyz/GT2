using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    public GameObject arrowPrefab; // The arrow prefab with the FollowGoblinArrow script attached
    public Transform bulletPos; // The position from where the arrow will be shot
    public Animator animator; // Reference to the Animator component
    public float shootingInterval = 1f; // Interval between shots

    private float timer; // Timer to track shooting intervals
    private GameObject player; // Reference to the player (Goblin)

    void Start()
    {
        // Find the player with the tag "Goblin"
        player = GameObject.FindGameObjectWithTag("Goblin");

        if (player == null)
        {
            Debug.LogWarning("Player with tag 'Goblin' not found. Please assign the correct tag to the player object.");
            return;
        }

        // Ensure animator is assigned, if not, get the Animator component from the GameObject
        if (animator == null)
        {
            animator = GetComponent<Animator>();

            if (animator == null)
            {
                Debug.LogError("Animator component not found! Please assign it in the Inspector or attach an Animator component to the GameObject.");
                return;
            }
        }
    }

    void Update()
    {
        if (player == null || animator == null)
            return;

        // Calculate the distance between the archer and the goblin (player)
        float distance = Vector2.Distance(transform.position, player.transform.position);

        // If the goblin is within range, aim and shoot
        if (distance < 8)
        {
            // Set the shooting animation
            animator.SetBool("isShooting", true);

            // Increment the timer based on elapsed time
            timer += Time.deltaTime;

            // Shoot an arrow at specified intervals
            if (timer > shootingInterval)
            {
                timer = 0;
                Shoot(); // Call the Shoot method
            }
        }
        else
        {
            // Stop shooting animation if the goblin is out of range
            animator.SetBool("isShooting", false);
        }
    }

    // Method to shoot an arrow directly towards the goblin
    void Shoot()
    {
        // Instantiate the arrow at the specified position
        Instantiate(arrowPrefab, bulletPos.position, Quaternion.identity);

        // Play shooting animation
        animator.SetTrigger("shoot");

        Debug.Log("Archer shoots an arrow at the goblin!");
    }
}
