using UnityEngine;

public class ShieldPickup : MonoBehaviour
{
    public MazeGameManager manager;
    private bool consumed = false;

    private void OnTriggerEnter(Collider other)
    {
        if (consumed) return;
        if (!other.CompareTag("Player")) return;

        consumed = true;
        manager?.OnShieldCollected(this);
        Destroy(gameObject);
    }
}
