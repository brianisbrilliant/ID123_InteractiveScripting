using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public EnemyScript enemyPrefab;
    public Transform player;
    public LayerMask enemyLayer;

    public int desiredEnemies = 10;
    public float spawnInterval = 3;

    private int enemiesCreated = 0;
    bool canSpawn = true;

    private void Update()
    {
        if(canSpawn && enemiesCreated < desiredEnemies && !checkForClear())
        {
            canSpawn = false;
            spawnEnemy();
            StartCoroutine(spawnDelayer());
        }
    }

    bool checkForClear()
    {
        return Physics.CheckSphere(transform.position, 1f, enemyLayer);
    }

    IEnumerator spawnDelayer()
    {
        yield return new WaitForSeconds(spawnInterval);
        canSpawn = true;
    }

    void spawnEnemy()
    {
        EnemyScript enemy = Instantiate(enemyPrefab, transform.position + (Vector3.up * 2), Quaternion.identity);
        enemy.playerTrans = player;
        enemiesCreated++;
    }
}
