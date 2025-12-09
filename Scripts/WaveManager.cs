using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaveManager : MonoBehaviour
{
    public bool freezeWave;

    [Header("Current Wave")]
    public TextMeshProUGUI waveCount;
    public int currentWave;

    [Header("Spawn Points")]
    public Transform left_Spawn;
    public Transform right_Spawn;
    private int spawnAmount = 6; // Add 2 every wave increase
    public float spawnDelay = 10; // 10 seconds, -0.5s every wave
    public float timer;

    [Header("Enemies")]
    public float enemiesSpawn_SoFar = 0;
    public GameObject enemy_one;

    private void Start()
    {
        NextWave(); // I KNOW its better to make a new functions called first wave or match start, with this the values will be 8 and 9.5 off start, but im ok with that
    }

    private void Update()
    {
        if (freezeWave)
            return;

        if (enemiesSpawn_SoFar < spawnAmount)
        {
            if (timer >= spawnDelay)
            {
                // delay is up
                // have not spawned all the enemies
                Instantiate(enemy_one, left_Spawn);
                Instantiate(enemy_one, right_Spawn);
                enemiesSpawn_SoFar += 2;
                timer = 0;
            }
            else
            {
                // delay not up
                timer += Time.deltaTime;
            }
        }
        else
        {
            // if all enemies spawned so far
            // check to see if any enemies are alive with the tag enemy
            // if so return
            // if nothing is alive with the enemy tag then NextWave()

            // Check if ANY enemies are still alive
            GameObject[] aliveEnemies = GameObject.FindGameObjectsWithTag("Enemy");

            // If ANY enemies exist, stop here
            if (aliveEnemies.Length > 0)
                return;

            // If we reach this point, NO enemies are alive so we start next wave
            NextWave();
        }
    }

    private void NextWave()
    {
        spawnAmount += 2;

        if (spawnDelay > 0)
            spawnDelay -= 0.5f;
        else
            spawnDelay = 0;

        currentWave++;
        waveCount.text = currentWave.ToString();
    }
}
