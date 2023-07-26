using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] float spawnMinX = -23;
    [SerializeField] float spawnMaxX = 23;
    [SerializeField] float spawnRate = 3;
    [SerializeField] float spawnRateIncrease;
    [SerializeField] float spawnHeight;
    [SerializeField] EnemyScript enemyPrefab;
    [SerializeField] EnemySpawnController warningPrefab;
    [SerializeField] TMP_Text killCountText;
    [SerializeField] string killCountString;

    [SerializeField] private float spawnTimer;

    int killCount = 0;

    public List<EnemyScript> enemies = new List<EnemyScript>();

    void Start()
    {
        SpawnEnemy();
        UpdateKillCountText();
    }

    void Update()
    {
        spawnTimer += Time.deltaTime;
        if (spawnTimer > spawnRate) 
        {
            spawnTimer -= spawnRate;
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        EnemySpawnController spawnAlert;
        spawnAlert = Instantiate(this.warningPrefab, new Vector3(Random.Range(this.spawnMinX, this.spawnMaxX), spawnHeight, 0), Quaternion.identity);

        spawnAlert.SpawnEnemy(this.enemyPrefab, this);
    }

    public void SpawnEnemy(EnemyScript enemy, Transform dest)
    {
        EnemyScript newEnemy = Instantiate(enemyPrefab, dest.position, Quaternion.identity);
        newEnemy.enemyManager = this;
        enemies.Add(newEnemy);
    }

    public void KillEnemy(EnemyScript target)
    {
        if (target == null) return;
        enemies.Remove(target);
        Destroy(target.gameObject);
        killCount ++;
        UpdateKillCountText();

    }

    private void UpdateKillCountText()
    {
        killCountText.text = killCountString + killCount;
    }
}
