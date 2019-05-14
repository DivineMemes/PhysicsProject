using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour {

    [System.Serializable]
    private struct SpawnData
    {
        public GameObject prefab;
        public int firstSpawnCount;
        public int firstWave;
        public int increaseAmount;
        public int increaseInterval;
        public int spawnWaveInterval;
    }

    private struct SpawnTracker
    {
        public int spawnCount;
        public int addSpawnsCountdown;
        public int spawnCountdown;
        public int remainingToSpawn;
    }

    [SerializeField, Tooltip("The positions enemies will spawn at")]
    private Transform[] spawns;
    [SerializeField, Tooltip("The length of each wave in seconds")]
    private float waveLength;
    [SerializeField, Tooltip("The length time to wait after one set of spawns before continuing the wave, in seconds")]
    private float spawnClearanceWait;
    [SerializeField, Tooltip("The initial forward velocity of spawned enemies")]
    private float initialVelocity;
    [SerializeField, Tooltip("The spawning data for each enemy")]
    private SpawnData[] spawnData;

    private int wave = 0;
    private float timer;
    private SpawnTracker[] trackers;



    private void Start()
    {
        trackers = new SpawnTracker[spawnData.Length];

        for(int i = 0; i < trackers.Length; ++i)
        {
            trackers[i].spawnCount = spawnData[i].firstSpawnCount;
            trackers[i].addSpawnsCountdown = spawnData[i].increaseInterval;
            trackers[i].spawnCountdown = spawnData[i].firstWave;
        }

        SpawnWave();
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if(timer <= 0)
        {
            SpawnWave();
        }
    }

    private void SpawnWave()
    {
        timer = waveLength;
        ++wave;
        Debug.Log("Spawning wave " + wave);

        //Check all listed enemies to spawn
        for(int i = 0; i < spawnData.Length; ++i)
        {
            //Count down to spawn
            --trackers[i].spawnCountdown;

            if(trackers[i].spawnCountdown <= 0)
            {
                //Reset spawn countdown
                trackers[i].spawnCountdown = spawnData[i].spawnWaveInterval;

                //Count down to increasing spawn count & increase if ready
                if(wave != spawnData[i].firstWave)
                {
                    --trackers[i].addSpawnsCountdown;
                    if (trackers[i].addSpawnsCountdown <= 0)
                    {
                        trackers[i].addSpawnsCountdown = spawnData[i].increaseInterval;
                        trackers[i].spawnCount += spawnData[i].increaseAmount;
                    }
                }

                trackers[i].remainingToSpawn = trackers[i].spawnCount;
            }
        }

        //Begin rounds of spawning
        InvokeRepeating("Spawn", 0f, spawnClearanceWait);
    }

    private void Spawn()
    {
        bool doneSpawning = false;
        int spawner = 0;

        for(int i = 0; i < trackers.Length; ++i)
        {
            if (trackers[i].remainingToSpawn > 0)
            {
                int spawnCount = trackers[i].remainingToSpawn;
                for (int n = 0; n < spawnCount; ++n)
                {
                    //Spawn enemy
                    GameObject enemy = Instantiate(spawnData[i].prefab);
                    enemy.transform.position = spawns[spawner].position;
                    enemy.transform.rotation = spawns[spawner].rotation;
                    enemy.GetComponent<Rigidbody>().velocity = enemy.transform.forward * initialVelocity;

                    //Increment variables
                    ++spawner;
                    --trackers[i].remainingToSpawn;
                    if (spawner == spawns.Length)
                    {
                        break;
                    }
                }
                if (spawner == spawns.Length)
                {
                    break;
                }
            }
            else if (i == trackers.Length - 1)
                doneSpawning = true;
        }

        if (doneSpawning)
            CancelInvoke();
    }

    public void PopulateSpawnerArray()
    {
        spawns = new Transform[transform.childCount];
        for(int i = 0; i < spawns.Length; ++i)
        {
            spawns[i] = transform.GetChild(i);
        }
    }

}
