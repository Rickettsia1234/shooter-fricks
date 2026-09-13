using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyStraightBullet : BulletBase
{
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public override void Setup(float speed, int damage, TargetType target, Transform targetTransform)
    {
        base.Setup(speed, damage, target, targetTransform);
        rb.linearVelocity = transform.up * this.speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (target == TargetType.Player && collision.CompareTag("Player"))
        {
            if (collision.TryGetComponent<PlayerController>(out var player))
            {
                player.OnHit();
            }
            GameManager.Instance.DespawnBullet(this);
        }
    }
}