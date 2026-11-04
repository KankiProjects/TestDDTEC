using System;
using UnityEngine;

public class PlayerTouchable : MonoBehaviour
{
    public static event Action<PlayerTouchable> Touched;

    private void OnCollisionEnter(Collision collision)
    {
        NotifyIfPlayer(collision.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        NotifyIfPlayer(other.gameObject);
    }

    private void NotifyIfPlayer(GameObject other)
    {
        if (other.CompareTag("Player"))
        {
            Touched?.Invoke(this);
        }
    }
}
