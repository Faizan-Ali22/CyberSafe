using UnityEngine;
using System.Collections;   
[RequireComponent(typeof(Collider))]
public class VirusDamageDealer : MonoBehaviour
{
   public float damageAmount = 20f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            MazeGameManager.Instance?.DamagePlayer(damageAmount);
        }
    }
}
