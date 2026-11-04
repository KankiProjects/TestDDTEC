using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovingPlatform : MonoBehaviour, ISmashable
{
    [SerializeField, Min(0f)] private float distance = 4f;
    [SerializeField, Min(0f)] private float speed = 2f;
    [SerializeField] private Vector3 direction = Vector3.right;

    private Rigidbody body;
    private Vector3 startPosition;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        body.isKinematic = true;
        startPosition = transform.position;
        direction = direction.sqrMagnitude > 0f ? direction.normalized : Vector3.right;
    }

    private void FixedUpdate()
    {
        float offset = Mathf.PingPong(Time.time * speed, distance);
        body.MovePosition(startPosition + direction * offset);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Smash(collision);
    }

    public void Smash(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
        {
            return;
        }

        // The contact normal on this collider points down when the player lands on top.
        for (int i = 0; i < collision.contactCount; i++)
        {
            if (collision.GetContact(i).normal.y < -0.5f)
            {
                Destroy(gameObject);
                return;
            }
        }
    }
}
