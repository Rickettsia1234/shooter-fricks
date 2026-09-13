using System.Collections;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    private static readonly int DieHash = Animator.StringToHash("die");
    private static readonly int IdleHash = Animator.StringToHash("idle");

    [SerializeField] protected float health = 100f;
    [SerializeField] protected float moveSpeed = 10f;
    [SerializeField] protected float maxTurnSpeed = 180f;
    [SerializeField] protected float minTurnSpeed = 30f;
    [SerializeField] protected int damage = 1;
    [SerializeField] protected int score = 1;

    [SerializeField] protected float dieAnimTime = 0.5f;
    [SerializeField] protected Animator animator;
    [SerializeField] protected Collider2D col;
    [SerializeField] protected Behaviour[] behaviorsToDisable;
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] protected AudioSource audioSource;
    [SerializeField] protected AudioClip sfxDie;
    
    public float MoveSpeed => moveSpeed;
    public float MaxTurnSpeed => maxTurnSpeed;
    public float MinTurnSpeed => minTurnSpeed;

    protected bool isDead;

    [HideInInspector] public Transform player;

    private void Awake()
    {
        if (behaviorsToDisable == null || behaviorsToDisable.Length == 0)
        {
            System.Collections.Generic.List<Behaviour> list = new System.Collections.Generic.List<Behaviour>();
            foreach (var b in GetComponents<MonoBehaviour>())
            {
                if (b != this) list.Add(b);
            }
            behaviorsToDisable = list.ToArray();
        }
    }

    public virtual void Initialize(Transform targetPlayer)
    {
        player = targetPlayer;
        isDead = false;

        if (col != null) col.enabled = true;
        if (rb != null) rb.simulated = true;

        if (animator != null)
        {
            animator.ResetTrigger(DieHash);
            animator.Play(IdleHash, 0, 0f);
        }

        if (behaviorsToDisable != null)
        {
            for (int i = 0; i < behaviorsToDisable.Length; i++)
            {
                if (behaviorsToDisable[i] != null) behaviorsToDisable[i].enabled = true;
            }
        }
    }

    public void Die()
    {
        if (isDead) return;
        StartCoroutine(DieRoutine());
    }

    private IEnumerator DieRoutine()
    {
        isDead = true;

        audioSource.PlayOneShot(sfxDie);

        if (col != null) col.enabled = false;
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.simulated = false;
        }

        if (behaviorsToDisable != null)
        {
            for (int i = 0; i < behaviorsToDisable.Length; i++)
            {
                if (behaviorsToDisable[i] != null) behaviorsToDisable[i].enabled = false;
            }
        }

        if (animator != null)
        {
            animator.SetTrigger(DieHash);
        }

        yield return new WaitForSeconds(dieAnimTime);

        GameManager.Instance.DespawnEnemy(this);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDead) return;

        if (collision.CompareTag("Player"))
        {
            GameManager.Instance.AddScore(1);
            Die();
        }
    }
}