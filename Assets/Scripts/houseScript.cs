using UnityEngine;

public class House : MonoBehaviour
{
    private int goldCount = 0;

    public void AddGold()
    {
        goldCount++;
        Debug.Log("Total Gold in the House: " + goldCount);
    }
}
