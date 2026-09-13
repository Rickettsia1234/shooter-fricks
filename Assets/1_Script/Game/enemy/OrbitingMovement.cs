using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(EnemyBase))]
public class OrbitingMovement : MonoBehaviour
{
    [SerializeField] private float orbitDistance = 5f;
    [SerializeField] private float blendDistance = 3f;

    private EnemyBase enemyData;
    private Rigidbody2D rb;
    private float targetAngle;
    private float currentAngularVelocity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        enemyData = GetComponent<EnemyBase>();
    }

    private void FixedUpdate()
    {
        if (enemyData.player == null) return;

        Vector2 dirToPlayer = enemyData.player.position - transform.position;
        float distance = dirToPlayer.magnitude;
        Vector2 normalizedDir = dirToPlayer.normalized;

        Vector2 rightTangent = Vector3.Cross(normalizedDir, Vector3.forward);
        Vector2 leftTangent = -rightTangent;
        Vector2 tangentDir = Vector2.Dot(transform.up, leftTangent) >= 0f ? leftTangent : rightTangent;

        float distanceOffset = distance - orbitDistance;
        float blendFactor = Mathf.Clamp(distanceOffset / blendDistance, -1f, 1f);

        float approachWeight = blendFactor;
        float tangentWeight = 1f - Mathf.Abs(blendFactor);

        Vector2 targetDirection = (tangentDir * tangentWeight + normalizedDir * approachWeight).normalized;

        targetAngle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg - 90f;

        float currentAngle = transform.eulerAngles.z;
        float nextAngle = Mathf.SmoothDampAngle(currentAngle, targetAngle, ref currentAngularVelocity, 0.1f, enemyData.MaxTurnSpeed);

        float deltaAngle = Mathf.DeltaAngle(currentAngle, targetAngle);
        if (Mathf.Abs(deltaAngle) > 0.01f && Mathf.Abs(currentAngularVelocity) < enemyData.MinTurnSpeed)
        {
            currentAngularVelocity = Mathf.Sign(deltaAngle) * enemyData.MinTurnSpeed;
            nextAngle = currentAngle + currentAngularVelocity * Time.fixedDeltaTime;
        }

        rb.MoveRotation(nextAngle);
        rb.linearVelocity = transform.up * enemyData.MoveSpeed;
    }
}