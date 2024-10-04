using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    public enum SpawnState { SPAWNING, WAITING, COUNTING };

    [System.Serializable]
    public class Wave
    {
        public string name;
        public Transform enemy;
        public int count;
        public float rate;
    }

    public Wave[] waves;
    private int nextWave = 0;

    public Transform spawnPoint;

    public float timeBetweenWaves = 5f;
    private float waveCountdown;

    public float timeBetweenSearches = 1f;
    private float searchCountdown = 1f;

    public TextMeshProUGUI waveCountdownText;

    private SpawnState state = SpawnState.COUNTING;

    void Start()
    {
        waveCountdown = timeBetweenWaves;
    }

    void Update()
    {
        if (state == SpawnState.WAITING)
        {
            // Check if enemies are still alive
            if(!EnemyIsAlive()) 
            {
                // Begin new round
                WaveCompleted();
            } 
            else { return; }
        }

        if (waveCountdown <= 0f)
        {
            if (state != SpawnState.SPAWNING)
            {
                StartCoroutine(SpawnWave(waves[nextWave]));
                waveCountdown = timeBetweenWaves;
            } 
        }
        else
        {
            waveCountdown -= Time.deltaTime;
        }

        waveCountdownText.text = (Mathf.Round(waveCountdown) + 1).ToString();
        
    }

    bool EnemyIsAlive()
    {
        searchCountdown -= Time.deltaTime;
        if(searchCountdown <= 0f)
        {
            searchCountdown = timeBetweenSearches;
            if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
            {
                return false;
            }
        }
        return true;
    }

    void WaveCompleted()
    {
        Debug.Log("Wave Completed!");

        state = SpawnState.COUNTING;
        waveCountdown = timeBetweenWaves;

        if(nextWave + 1 > waves.Length - 1)
        {
            nextWave = 0;
            Debug.Log("ALL WAVES COMPLETE! looping.");
        } else
        {
            nextWave++;
        }
    }

    IEnumerator SpawnWave(Wave _wave)
    {
        Debug.Log("Spawning Wave: " + _wave.name);
        state = SpawnState.SPAWNING;

        for (int i = 0; i < _wave.count; i++)
        {
            SpawnEnemy(_wave.enemy);
            yield return new WaitForSeconds( 1f/_wave.rate );
        }

        state = SpawnState.WAITING;

        yield break;
    }

    void SpawnEnemy(Transform _enemy)
    {
        //Spawn Enemy
        Debug.Log("Spawning Enemy: " + _enemy.name);

        if (spawnPoint == null)
        {
            Debug.Log("WaveSpawner ERROR: No Spawn Point Set.");
        }
        Instantiate(_enemy, spawnPoint.position, spawnPoint.rotation);
    }
}
