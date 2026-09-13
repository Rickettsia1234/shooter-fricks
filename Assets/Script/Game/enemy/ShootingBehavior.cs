using System.Collections;
using UnityEngine;

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

    private EnemyBase enemyData;
    private float fireTimer;
    private bool isShooting;
    private Vector3 warningLocalOffset;

    private void Awake()
    {
        enemyData = GetComponent<EnemyBase>();
        warningLocalOffset = warningIcon.transform.localPosition;
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
            StartCoroutine(ShootRoutine());
        }
    }

    private IEnumerator ShootRoutine()
    {
        isShooting = true;

        warningIcon.GetComponent<Animator>().Rebind();

        yield return new WaitForSeconds(prepareTime);

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

        isShooting = false;
    }
    
    private void LateUpdate()
    {
        warningIcon.transform.position = transform.position + warningLocalOffset;
        warningIcon.transform.rotation = Quaternion.identity;
    }
}