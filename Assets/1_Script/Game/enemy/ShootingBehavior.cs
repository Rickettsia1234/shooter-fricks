using System.Collections;
using UnityEngine;
using Cysharp.Threading.Tasks;

[RequireComponent(typeof(EnemyBase))]
public class ShootingBehavior : MonoBehaviour
{
    [SerializeField] private float fireInterval = 2f;
    [SerializeField] private float attackRange = 10f;
    [SerializeField] private BulletBase bulletPrefab;
    [SerializeField] private float bulletSpeed = 15f;
    [SerializeField] private int bulletDamage = 1;
    [SerializeField] private TargetType targetType = TargetType.Player;
    [SerializeField] private float prepareTime = 0.5f;
    [SerializeField] private GameObject warningIcon;
    [SerializeField] protected AudioSource audioSource;
    [SerializeField] private AudioClip sfxFire;

    private EnemyBase enemyData;
    private float fireTimer;
    private bool isShooting;
    private Vector3 warningLocalOffset;
    private Animator warningAnimator;

    private void Awake()
    {
        enemyData = GetComponent<EnemyBase>();
        warningLocalOffset = warningIcon.transform.localPosition;
        if (warningIcon != null)
        {
            warningAnimator = warningIcon.GetComponent<Animator>();
        }
    }

    private void Update()
    {
        if (enemyData.player == null || isShooting) return;

        float distanceToPlayer = Vector2.Distance(transform.position, enemyData.player.position);
        if (distanceToPlayer > attackRange) return;

        fireTimer += Time.deltaTime;
        if (fireTimer >= fireInterval)
        {
            fireTimer = 0f;
            ShootRoutine().Forget();
        }
    }

    private async UniTaskVoid ShootRoutine()
    {
        isShooting = true;

        if (warningAnimator != null)
        {
            warningAnimator.Rebind();
        }

        await UniTask.Delay(System.TimeSpan.FromSeconds(prepareTime));

        Vector2 fireDir = transform.up;

        if (targetType == TargetType.Player)
        {
            fireDir = (enemyData.player.position - transform.position).normalized;
        }

        BulletBase bullet = GameManager.Instance.SpawnBullet(bulletPrefab, transform.position, fireDir);
        if (bullet != null)
        {
            bullet.Setup(bulletSpeed, bulletDamage, targetType, enemyData.player);
        }

        audioSource.PlayOneShot(sfxFire);

        isShooting = false;
    }
    
    private void LateUpdate()
    {
        warningIcon.transform.position = transform.position + warningLocalOffset;
        warningIcon.transform.rotation = Quaternion.identity;
    }
}