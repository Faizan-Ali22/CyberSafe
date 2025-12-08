using UnityEngine;

public class ShieldPickup : MonoBehaviour
{
     public MazeGameManager manager;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Debug.Log("Shield collected by Player");
        manager?.OnShieldCollected(this);
        Destroy(gameObject);
    }
}
