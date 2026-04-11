using System.Collections.Generic;
using UnityEngine;

public class Spawner : OverworldObject
{
    [SerializeField] List<GameObject> spawnPoints;
    [SerializeField] List<SpawnPhase> phases = new();
    int phaseIndex = 0;
    int enemiesSpawnedThisPhase = 0;
    [SerializeField] int enemiesPerPhase = 5;       // Ahora mismo todas las fases tienen la misma longitud pero se podr�a cambiar
    [SerializeField] PlayerController player;
    float timer = 0;

    SpawnPhase currentPhase;

    private void Awake()
    {
        currentPhase = phases[phaseIndex];
    }

    void Update()
    {
        if (!isPaused) timer += Time.deltaTime;

        if (!isPaused && timer >= currentPhase.startingTime && timer % currentPhase.spawnInterval <= Time.deltaTime)
        {
            if (enemiesSpawnedThisPhase < enemiesPerPhase)
            {
                int spawnPointIndex = Random.Range(0, spawnPoints.Count);
                print(spawnPointIndex);
                Instantiate(currentPhase.enemyPrefab, spawnPoints[spawnPointIndex].transform.position, Quaternion.identity);
                enemiesSpawnedThisPhase++;
                if (currentPhase.spawnInterval > currentPhase.minSpawnInterval) currentPhase.spawnInterval -= currentPhase.spawnTimeIncrement;
            } else
            {
                timer = 0;
                phaseIndex = (++phaseIndex) % phases.Count;
                currentPhase = phases[phaseIndex];
                print("Cambiando a la fase " + phaseIndex + "...");
                enemiesSpawnedThisPhase = 0;
            }
        }
    }


    [System.Serializable]       // Poner esto es obligatorio porque si no lo de abajo no aparece para editarlo en ningn sitio
    public class SpawnPhase
    {
        [SerializeField] public GameObject enemyPrefab;
        [SerializeField] public float spawnInterval;
        [SerializeField] public float minSpawnInterval;
        [SerializeField] public float spawnTimeIncrement;
        [SerializeField] public float startingTime;
    }
}
