using UnityEngine;

public class TestTrigger : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Object entered trigger: " + other.name);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("Object exited trigger: " + other.name);
    }
}
