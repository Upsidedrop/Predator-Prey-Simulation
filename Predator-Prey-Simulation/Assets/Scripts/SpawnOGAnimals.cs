using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.VR;
using UnityEngine;

public class SpawnOGAnimals : MonoBehaviour
{
    public GameObject prey;
    public GameObject predator;
    private void Awake()
    {
        for (int i = 0; i < 20; i++)
        {
            Instantiate(prey,
                transform.position
                + Vector3.up
                * Random.Range(-0.1f, 0.1f)
                + Vector3.right
                * Random.Range(-0.1f, 0.1f),
                Quaternion.identity);
        }
        
        Instantiate(predator);
    }
}
