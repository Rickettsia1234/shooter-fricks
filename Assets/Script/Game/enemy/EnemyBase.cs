using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [SerializeField] protected float health = 100f;
    [SerializeField] protected float moveSpeed = 10f;
    [SerializeField] protected float maxTurnSpeed = 180f;
    [SerializeField] protected float minTurnSpeed = 30f;
    [SerializeField] protected int damage = 1;
    [SerializeField] protected int score = 1;
    
    public float MoveSpeed => moveSpeed;
    public float MaxTurnSpeed => maxTurnSpeed;
    public float MinTurnSpeed => minTurnSpeed;

    [HideInInspector] public Transform player;

    public virtual void Initialize(Transform targetPlayer)
    {
        player = targetPlayer;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.Instance.AddScore(1);
            GameManager.Instance.DespawnEnemy(this);
        }
    }
}