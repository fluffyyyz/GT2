using UnityEngine;

public class Goblin : MonoBehaviour
{
    public float speed = 2f;  // Movement speed
    public Transform house;   // Reference to the house
    public Transform houseTarget; // Reference to a specific stopping point at the house
    public GameObject goldPrefab; // Reference to the gold prefab for dropping gold
    private GameObject targetGold;  // The gold the goblin is moving toward
    private bool hasGold = false;   // Whether the goblin is holding gold

    private enum GoblinState { Idle, MovingToGold, MovingToHouse }
    private GoblinState currentState = GoblinState.Idle;  // Default state is Idle

    public Animator animator;  // Reference to the Animator component
    private Vector3 lastPosition;  // Store last position to calculate movement

    void Start()
    {
        // Start in Idle state
        currentState = GoblinState.Idle;

        // Initialize last position
        lastPosition = transform.position;
    }

    void Update()
    {
        // Calculate horizontal movement
        float horizontalMove = (transform.position - lastPosition).magnitude / Time.deltaTime;

        // Update the animator with the movement speed
        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

        // Update the sprite direction
        UpdateSpriteDirection();

        // Update last position
        lastPosition = transform.position;

        // State machine to handle goblin's behavior
        switch (currentState)
        {
            case GoblinState.Idle:
                CheckForGold();
                break;

            case GoblinState.MovingToGold:
                FindAndMoveToGold();
                break;

            case GoblinState.MovingToHouse:
                MoveToHouse();
                break;
        }
    }

    // Check if there's any gold in the scene and start moving towards it
    void CheckForGold()
    {
        targetGold = FindClosestGold();
        if (targetGold != null)
        {
            currentState = GoblinState.MovingToGold;  // Change state to moving to gold
            Debug.Log("Goblin found gold and is moving towards it.");
        }
    }

    // Find the nearest gold and move towards it
    void FindAndMoveToGold()
    {
        if (targetGold == null)
        {
            currentState = GoblinState.Idle;  // No gold found, go to idle state
            Debug.Log("No gold found. Returning to Idle state.");
            return;
        }

        // Calculate the target position slightly below the gold's center
        Vector3 goldPosition = targetGold.transform.position;
        Vector3 targetPosition = new Vector3(goldPosition.x, goldPosition.y - 0.5f, goldPosition.z);  // Adjust the offset as needed

        // Move towards the gold using MoveTowards for smoother motion
        MoveTowards(targetPosition);

        // If the goblin is close enough to the base of the gold, pick it up
        if (Vector2.Distance(transform.position, targetPosition) < 0.1f)  // Reduced threshold for smoother pickup
        {
            PickUpGold();
        }
    }

    // Move to the house to drop off gold
    void MoveToHouse()
    {
        // Move towards the specific house target using MoveTowards
        if (houseTarget != null)
        {
            MoveTowards(houseTarget.position);

            // If close enough to the house target, deposit the gold
            if (Vector2.Distance(transform.position, houseTarget.position) < 0.1f)  // Reduced threshold for smoother deposit
            {
                DepositGold();
            }
        }
        else
        {
            // Fallback to house position if houseTarget is not assigned
            MoveTowards(house.position);

            // If close enough to the house, deposit the gold
            if (Vector2.Distance(transform.position, house.position) < 0.1f)  // Reduced threshold for smoother deposit
            {
                DepositGold();
            }
        }
    }

    // Helper function to move the goblin using MoveTowards for smooth motion
    void MoveTowards(Vector3 target)
    {
        // Smooth movement using MoveTowards
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
    }

    // Update the sprite direction based on movement
    void UpdateSpriteDirection()
    {
        Vector3 direction = transform.position - lastPosition;

        // Store the original scale to ensure we don't alter the y and z values unintentionally
        Vector3 originalScale = transform.localScale;

        // Check for horizontal movement direction
        if (direction.x > 0.01f)  // Moving right
        {
            transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);  // Face right
        }
        else if (direction.x < -0.01f)  // Moving left
        {
            transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);  // Face left (flipped horizontally)
        }
    }

    // Pick up the gold
    void PickUpGold()
    {
        hasGold = true;
        Destroy(targetGold);  // Remove the gold from the scene
        Debug.Log("Goblin picked up gold!");
        currentState = GoblinState.MovingToHouse;  // Change state to moving to house
    }

    // Deposit the gold at the house
    void DepositGold()
    {
        hasGold = false;
        House houseScript = house.GetComponent<House>();
        if (houseScript != null)
        {
            houseScript.AddGold();
            Debug.Log("Gold deposited!");
        }
        else
        {
            Debug.LogWarning("No House script found on the house object.");
        }

        targetGold = null;  // Reset gold target for the next round
        currentState = GoblinState.Idle;  // Go back to idle state
    }

    // Drop the gold if hit by an arrow and find new gold
    public void DropGold()
    {
        if (hasGold)
        {
            hasGold = false;

            // Instantiate a new gold prefab at the goblin's current position
            Instantiate(goldPrefab, transform.position, Quaternion.identity);
            Debug.Log("Goblin dropped the gold!");

            // Immediately find new gold after dropping the current one
            CheckForGold();  // Check for new gold after dropping the current one

            // Change state to idle to allow moving towards new gold
            currentState = GoblinState.Idle;
        }
    }

    // Find the closest gold object
    GameObject FindClosestGold()
    {
        GameObject[] goldObjects = GameObject.FindGameObjectsWithTag("Gold");
        GameObject closestGold = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject gold in goldObjects)
        {
            float distance = Vector2.Distance(transform.position, gold.transform.position);
            if (distance < closestDistance)
            {
                closestGold = gold;
                closestDistance = distance;
            }
        }

        return closestGold;
    }

    // Called when new gold is spawned
    public void OnGoldSpawned()
    {
        if (currentState == GoblinState.Idle)
        {
            CheckForGold();  // Check for gold again when new gold spawns
        }
    }

    // This method will be called when the goblin is hit by an arrow
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Arrow"))
        {
            Debug.Log("Goblin hit by an arrow!");
            DropGold();  // Call the DropGold method when hit by an arrow
        }
    }
}
