using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public bool spawnEnabled = false;
    [SerializeField] int maxEnemies = 5;
    [SerializeField] float minPositionX = -10;
    [SerializeField] float maxPositionX = 10;
    [SerializeField] float minPosisionZ = -10;
    [SerializeField] float maxPositionZ = 10;
    [SerializeField] float minSpawnInterval = 1;
    [SerializeField] float maxSpawnIterval = 3;
    [SerializeField] int mutantSpawnRound = 5;
    [SerializeField] GameObject[] enemyPrefabs;
    [SerializeField] GameManager gameManager;

    bool spawning = false;
    public int enemySpawnCount = 0;
    public int mutantSpawnCount = 0;
    int spawnPosNum = 0;
    int count = 0;
    public List<GameObject> EnemiesTargetOfPlayer = new List<GameObject>();
    public List<GameObject> EnemiesTargetOfWarehouse = new List<GameObject>();
    public List<GameObject> Mutants = new List<GameObject>();
    void Start()
    {
        spawnEnabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnEnabled)
        {
            StartCoroutine(SpawnTimer());
        }
    }

    IEnumerator SpawnTimer()
    {
        if (!spawning)
        {
            if (SpawnEnemy())
            {
                spawning = true;

                float interval = Random.Range(minSpawnInterval, maxSpawnIterval);
                yield return new WaitForSeconds(interval);

                spawning = false;
            }
            else
                yield return null;
        }
        yield return null;
    }

    bool SpawnEnemy()
    {
        maxEnemies = gameManager.GetmaxKill();
        if (enemySpawnCount >= maxEnemies)
            return false;
        else
        {
            int choosedIndex;
            if (gameManager.round >= mutantSpawnRound && mutantSpawnCount * mutantSpawnRound < gameManager.round)
            {
                choosedIndex = Random.Range(0, enemyPrefabs.Length);
                if(choosedIndex==2)
                    mutantSpawnCount++;
            }
            else
                choosedIndex = Random.Range(0, enemyPrefabs.Length-1);
            if(choosedIndex == 0)
            {
                foreach(GameObject EnemyTargetOfPlayer in EnemiesTargetOfPlayer)
                {
                    if (EnemyTargetOfPlayer.activeSelf == false)
                    {
                        EnemyTargetOfPlayer.transform.position = RandomPos();
                        EnemyTargetOfPlayer.SetActive(true);
                        count = 0;
                        break;
                    }
                    else
                        count++;
                }
                if (count == EnemiesTargetOfPlayer.Count || EnemiesTargetOfPlayer.Count == 0)
                {
                    GameObject EnemyTargetOfPlayer = Instantiate(enemyPrefabs[0], RandomPos(), Quaternion.identity);
                    EnemiesTargetOfPlayer.Add(EnemyTargetOfPlayer);
                    count = 0;
                }
            }
            if (choosedIndex == 1)
            {
                foreach (GameObject EnemyTargetOfWarehouse in EnemiesTargetOfWarehouse)
                {
                    if (EnemyTargetOfWarehouse.activeSelf == false)
                    {
                        EnemyTargetOfWarehouse.transform.position = RandomPos();
                        EnemyTargetOfWarehouse.SetActive(true);
                        count = 0;
                        break;
                    }
                    else
                        count++;
                }
                if (count == EnemiesTargetOfWarehouse.Count || EnemiesTargetOfWarehouse.Count == 0)
                {
                    GameObject EnemyTargetOfWarehouse = Instantiate(enemyPrefabs[1], RandomPos(), Quaternion.identity);
                    EnemiesTargetOfWarehouse.Add(EnemyTargetOfWarehouse);
                    count = 0;
                }
            }
            if (choosedIndex == 2)
            {
                foreach (GameObject Mutant in Mutants)
                {
                    if (Mutant.activeSelf == false)
                    {
                        Mutant.transform.position = RandomPos();
                        Mutant.SetActive(true);
                        count = 0;
                        break;
                    }
                    else
                        count++;
                }
                if (count == Mutants.Count || Mutants.Count == 0)
                {
                    GameObject Mutant = Instantiate(enemyPrefabs[2], RandomPos(), Quaternion.identity);
                    Mutants.Add(Mutant);
                    count = 0;
                }
            }
            enemySpawnCount++;
            return true;
        }
    }

    Vector3 RandomPos()
    {
        float diffPositionX = Random.Range(minPositionX, maxPositionX);
        float diffPositionZ = Random.Range(minPosisionZ, maxPositionZ);
        Vector3 position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        spawnPosNum = Random.Range(1, 7);
        if (spawnPosNum == 1)
            position = new Vector3(145 + diffPositionX, transform.position.y, 90 + diffPositionZ);
        else if (spawnPosNum == 2)
            position = new Vector3(120 + diffPositionX, transform.position.y, 90 + diffPositionZ);
        else if (spawnPosNum == 3)
            position = new Vector3(90 + diffPositionX, transform.position.y, 90 + diffPositionZ);
        else if (spawnPosNum == 4)
            position = new Vector3(93 + diffPositionX, transform.position.y, 116 + diffPositionZ);
        else if (spawnPosNum == 5)
            position = new Vector3(115 + diffPositionX, transform.position.y, 116 + diffPositionZ);
        else if (spawnPosNum == 6)
            position = new Vector3(147 + diffPositionX, transform.position.y, 126 + diffPositionZ);
        return position;
    }
}
