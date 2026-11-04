using UnityEngine;

public enum EnemyState
{
    Wandering,
    Fleeing
}

[RequireComponent(typeof(Rigidbody))]
public class EnemyStateMachine : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField, Min(0f)] private float fleeRadius = 4f;
    [SerializeField, Min(0f)] private float wanderRadius = 5f;
    [SerializeField, Min(0f)] private float wanderSpeed = 1.5f;
    [SerializeField, Min(0f)] private float fleeSpeed = 4f;
    [SerializeField, Min(0.01f)] private float destinationTolerance = 0.2f;
    [SerializeField] private Color wanderingColor = new Color(0.2f, 0.8f, 0.3f);
    [SerializeField] private Color fleeingColor = Color.red;

    private Rigidbody body;
    private Renderer[] enemyRenderers;
    private Vector3 wanderCenter;
    private Vector3 wanderDestination;
    private EnemyState currentState;

    public EnemyState CurrentState => currentState;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        body.isKinematic = true;
        body.useGravity = false;
        wanderCenter = body.position;
        enemyRenderers = GetComponentsInChildren<Renderer>();

        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
            else
            {
                Debug.LogError("EnemyStateMachine needs a Player reference or an object tagged Player.", this);
            }
        }

        if (enemyRenderers.Length == 0)
        {
            Debug.LogError("EnemyStateMachine needs a Renderer on this object or one of its children.", this);
        }

        ChooseWanderDestination();
        SetState(EnemyState.Wandering);
    }

    private void Update()
    {
        if (player == null)
        {
            return;
        }

        Vector3 offset = player.position - body.position;
        offset.y = 0f;
        bool playerIsClose = offset.sqrMagnitude <= fleeRadius * fleeRadius;

        if (currentState == EnemyState.Wandering && playerIsClose)
        {
            SetState(EnemyState.Fleeing);
        }
        else if (currentState == EnemyState.Fleeing && !playerIsClose)
        {
            SetState(EnemyState.Wandering);
            ChooseWanderDestination();
        }
    }

    private void FixedUpdate()
    {
        switch (currentState)
        {
            case EnemyState.Wandering:
                Wander();
                break;
            case EnemyState.Fleeing:
                Flee();
                break;
        }
    }

    private void Wander()
    {
        Vector3 direction = wanderDestination - body.position;
        direction.y = 0f;

        if (direction.sqrMagnitude <= destinationTolerance * destinationTolerance)
        {
            ChooseWanderDestination();
            return;
        }

        direction.Normalize();
        body.MovePosition(body.position + direction * wanderSpeed * Time.fixedDeltaTime);
    }

    private void Flee()
    {
        if (player == null)
        {
            return;
        }

        Vector3 direction = body.position - player.position;
        direction.y = 0f;
        if (direction.sqrMagnitude < 0.0001f)
        {
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            direction = new Vector3(randomDirection.x, 0f, randomDirection.y);
        }

        body.MovePosition(body.position + direction.normalized * fleeSpeed * Time.fixedDeltaTime);
    }

    private void ChooseWanderDestination()
    {
        Vector2 offset = Random.insideUnitCircle * wanderRadius;
        wanderDestination = wanderCenter + new Vector3(offset.x, 0f, offset.y);
    }

    private void SetState(EnemyState newState)
    {
        currentState = newState;
        Color stateColor = currentState == EnemyState.Wandering ? wanderingColor : fleeingColor;

        foreach (Renderer enemyRenderer in enemyRenderers)
        {
            enemyRenderer.material.color = stateColor;
        }
    }
}
