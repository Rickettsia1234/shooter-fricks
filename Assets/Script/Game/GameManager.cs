using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Unity.Cinemachine;

[System.Serializable]
public struct EnemySpawnData
{
    [SerializeField] private float minSpawnTime;
    [SerializeField] private float maxSpawnTime;
    [SerializeField] private int minScore;
    [SerializeField] private int maxScore;
    [SerializeField] private float intervalTime;
    [SerializeField] private int maxCount;
    [SerializeField] private bool isDespawnable;
    [SerializeField] private EnemyBase prefab;
    [HideInInspector] public float lastSpawnTime;

    public readonly float MinSpawnTime => minSpawnTime;
    public readonly float MaxSpawnTime => maxSpawnTime;
    public readonly int MinScore => minScore;
    public readonly int MaxScore => maxScore;
    public readonly float IntervalTime => intervalTime;
    public readonly int MaxCount => maxCount;
    public readonly bool IsDespawnable => isDespawnable;
    public readonly EnemyBase Prefab => prefab;
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private PlayerBase playerPrefab;
    [SerializeField] private CinemachineCamera virtualCamera;
    [SerializeField] private float despawnDistance = 20f;
    [SerializeField] private float spawnRadius = 15f;
    [SerializeField] private int maxBulletCount = 10;
    [SerializeField] private int score = 0;
    [SerializeField] private List<EnemySpawnData> enemySpawnDataList;
    [SerializeField] private DrawManager drawManager;

    private Transform player;
    private readonly List<EnemyBase> activeEnemies = new();
    private readonly List<BulletBase> activeBullets = new();
    private readonly Dictionary<EnemyBase, IObjectPool<EnemyBase>> enemyPools = new();
    private readonly Dictionary<EnemyBase, EnemyBase> enemyInstanceMap = new();
    private readonly Dictionary<BulletBase, IObjectPool<BulletBase>> bulletPools = new();
    private readonly Dictionary<BulletBase, BulletBase> bulletInstanceMap = new();
    private Transform enemyContainer;
    private Transform bulletContainer;
    private float elapsedTime;
    private float lastTime;
    private bool isQuitting;

    private void Awake()
    {
        Instance = this;
        PlayerBase playerObj = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        player = playerObj.transform;
        virtualCamera.Follow = player;

        enemyContainer = new GameObject("EnemyContainer").transform;
        bulletContainer = new GameObject("BulletContainer").transform;
    }

    private void Start()
    {
        GameSystem.Instance.SetState(GameState.Game);
        StartCoroutine(SpawnAndDespawnRoutine());
    }

    private void OnApplicationQuit()
    {
        isQuitting = true;
    }

    private IObjectPool<EnemyBase> GetEnemyPool(EnemyBase prefab, int maxCount)
    {
        if (prefab == null) return null;

        if (!enemyPools.TryGetValue(prefab, out var pool))
        {
            pool = new ObjectPool<EnemyBase>(
                () => Instantiate(prefab.gameObject, enemyContainer).GetComponent<EnemyBase>(),
                e => { if (e != null) { e.gameObject.SetActive(true); activeEnemies.Add(e); } },
                e => { if (e != null) { e.gameObject.SetActive(false); activeEnemies.Remove(e); } },
                e => { if (e != null) { if (!isQuitting && Application.isPlaying) Destroy(e.gameObject); else if (!isQuitting) DestroyImmediate(e.gameObject); } },
                maxSize: maxCount
            );
            enemyPools[prefab] = pool;
        }
        return pool;
    }

    private IObjectPool<BulletBase> GetBulletPool(BulletBase prefab)
    {
        if (prefab == null) return null;

        if (!bulletPools.TryGetValue(prefab, out var pool))
        {
            pool = new ObjectPool<BulletBase>(
                () => Instantiate(prefab.gameObject, bulletContainer).GetComponent<BulletBase>(),
                b => { if (b != null) { b.gameObject.SetActive(true); activeBullets.Add(b); } },
                b => { if (b != null) { b.gameObject.SetActive(false); activeBullets.Remove(b); } },
                b => { if (b != null) { if (!isQuitting && Application.isPlaying) Destroy(b.gameObject); else if (!isQuitting) DestroyImmediate(b.gameObject); } },
                maxSize: maxBulletCount
            );
            bulletPools[prefab] = pool;
        }
        return pool;
    }

    private IEnumerator SpawnAndDespawnRoutine()
    {
        WaitForSeconds waitOneSecond = new WaitForSeconds(1f);
        while (true)
        {
            yield return waitOneSecond;

            DespawnFarObjects(activeEnemies, despawnDistance);
            DespawnFarObjects(activeBullets, despawnDistance);

            SpawnEnemiesByInterval();
        }
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;
        int totalSeconds = Mathf.FloorToInt(elapsedTime);

        if (totalSeconds != lastTime)
        {
            lastTime = totalSeconds;
            UpdateTime();
        }
    }

    private void SpawnEnemiesByInterval()
    {
        for (int i = 0; i < enemySpawnDataList.Count; i++)
        {
            var data = enemySpawnDataList[i];

            bool isMinSatisfied = elapsedTime >= data.MinSpawnTime || score >= data.MinScore;
            bool isMaxExceeded = elapsedTime > data.MaxSpawnTime || score > data.MaxScore;

            if (isMinSatisfied && !isMaxExceeded)
            {
                if (elapsedTime - data.lastSpawnTime >= data.IntervalTime)
                {
                    int currentPrefabCount = 0;
                    for (int j = 0; j < activeEnemies.Count; j++)
                    {
                        if (enemyInstanceMap.TryGetValue(activeEnemies[j], out var p) && p == data.Prefab)
                        {
                            currentPrefabCount++;
                        }
                    }

                    if (currentPrefabCount < data.MaxCount)
                    {
                        SpawnEnemyPrefab(data.Prefab, data.MaxCount);
                        data.lastSpawnTime = elapsedTime;
                        enemySpawnDataList[i] = data;
                    }
                }
            }
        }
    }

    private void SpawnEnemyPrefab(EnemyBase prefab, int maxCount)
    {
        Vector2 randomDir = Random.insideUnitCircle.normalized;
        Vector3 spawnPosition = player.position + (Vector3)(randomDir * spawnRadius);

        EnemyBase enemy = GetEnemyPool(prefab, maxCount).Get();
        enemyInstanceMap[enemy] = prefab;

        enemy.transform.position = spawnPosition;
        enemy.Initialize(player);
    }

    private void DespawnFarObjects<T>(List<T> activeList, float distance) where T : MonoBehaviour
    {
        float sqrDespawnDistance = distance * distance;
        for (int i = activeList.Count - 1; i >= 0; i--)
        {
            T obj = activeList[i];
            if (obj is EnemyBase enemy)
            {
                if (enemyInstanceMap.TryGetValue(enemy, out var prefab))
                {
                    bool canDespawn = true;
                    for (int j = 0; j < enemySpawnDataList.Count; j++)
                    {
                        if (enemySpawnDataList[j].Prefab == prefab)
                        {
                            canDespawn = enemySpawnDataList[j].IsDespawnable;
                            break;
                        }
                    }
                    if (!canDespawn) continue;
                }
            }

            if ((obj.transform.position - player.position).sqrMagnitude > sqrDespawnDistance)
            {
                if (obj is EnemyBase e) DespawnEnemy(e);
                else if (obj is BulletBase b) DespawnBullet(b);
            }
        }
    }

    public BulletBase SpawnBullet(BulletBase prefab, Vector3 position, Vector2 direction)
    {
        if (activeBullets.Count >= maxBulletCount) return null;

        BulletBase bullet = GetBulletPool(prefab).Get();
        bulletInstanceMap[bullet] = prefab;
        
        bullet.transform.position = position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        bullet.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        return bullet;
    }

    public void DespawnEnemy(EnemyBase enemy)
    {
        if (enemyInstanceMap.TryGetValue(enemy, out var prefab))
        {
            enemyPools[prefab].Release(enemy);
        }
    }

    public void DespawnBullet(BulletBase bullet)
    {
        if (bulletInstanceMap.TryGetValue(bullet, out var prefab))
        {
            bulletPools[prefab].Release(bullet);
        }
    }

    public void AddScore(int amount)
    {
        score += amount;
        drawManager.DrawScore(score);
    }

    public void UpdateTime()
    {
        drawManager.DrawTime((int)elapsedTime);
    }

    public void PlayerDead()
    {
        GameSystem.Instance.SetScore(score);
        GameSystem.Instance.ChangeScene(SceneName.Dead);
    }
}