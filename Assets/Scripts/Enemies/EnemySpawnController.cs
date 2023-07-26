using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnController : MonoBehaviour
{
    [Space()]
    [Header("Config")]
    [SerializeField] float spawnAfter = 2f;

    float currentSpawnAfter = 0;

    EnemyScript enemy;
    EnemyManager enemyManager;

    private void Update()
    {
        this.currentSpawnAfter += Time.deltaTime;
        if (this.currentSpawnAfter >= this.spawnAfter)
        {
            if (this.enemyManager != null) this.enemyManager.SpawnEnemy(enemy, this.transform);
            Destroy(this.gameObject);
        }
    }

    public void SpawnEnemy(EnemyScript enemy, EnemyManager enemyManager)
    {
        this.enemy = enemy;
        this.enemyManager = enemyManager;
    }
}
