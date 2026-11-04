using System.Collections;
using UnityEngine;

public class RespawningSmashable : MonoBehaviour, ISmashable
{
    [SerializeField, Min(0f)] private float respawnDelay = 3f;

    private Collider[] colliders;
    private Renderer[] renderers;
    private bool isSmashed;

    private void Awake()
    {
        colliders = GetComponentsInChildren<Collider>();
        renderers = GetComponentsInChildren<Renderer>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Smash(collision);
    }

    public void Smash(Collision collision)
    {
        if (isSmashed || !collision.gameObject.CompareTag("Player"))
        {
            return;
        }

        // The contact normal on this collider points down when the player lands on top.
        for (int i = 0; i < collision.contactCount; i++)
        {
            if (collision.GetContact(i).normal.y < -0.5f)
            {
                StartCoroutine(RespawnRoutine());
                return;
            }
        }
    }

    private IEnumerator RespawnRoutine()
    {
        isSmashed = true;
        SetActive(false);
        yield return new WaitForSeconds(respawnDelay);
        SetActive(true);
        isSmashed = false;
    }

    private void SetActive(bool active)
    {
        foreach (Collider c in colliders)
        {
            c.enabled = active;
        }

        foreach (Renderer r in renderers)
        {
            r.enabled = active;
        }
    }
}
