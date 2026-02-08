using System;
using System.Collections.Generic;
using UnityEngine;

public class PropRandomizer : MonoBehaviour
{
    [SerializeField] GameObject[] props;    // El contenido del array de props lo asignamos manualmente porque es diferente en cada mapa
    GameObject[] propSpawnPoints;       // Esto se rellena solo (ver abajo)

    private void Awake()
    {
        // Si queremos que el array de props se rellene solo sería: props = GameObject.FindGameObjectsWithTag("Prop");
        // Pero es mejor de esta forma
        propSpawnPoints = GameObject.FindGameObjectsWithTag("PropSpawnPoint");
        print("Se han encontrado " + props.Length + " props y " + propSpawnPoints.Length + " puntos de spawn ");

        foreach(GameObject pt in propSpawnPoints)
        {
            int chosenPropIndex = UnityEngine.Random.Range(0, props.Length);
            Instantiate(props[chosenPropIndex], pt.transform.position, pt.transform.rotation);
        }
    }
}
