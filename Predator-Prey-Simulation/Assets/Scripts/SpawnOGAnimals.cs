using UnityEngine;

public class SpawnOGAnimals : MonoBehaviour
{
    public GameObject prey;
    public GameObject predator;

    public Transform emptyPrey;

    public Transform emptyPredator;
    private void Awake()
    {
        for (int i = 0; i < 30; i++)
        {
            Instantiate(prey,
                prey.transform.position
                + Vector3.up
                * Random.Range(-0.1f, 0.1f)
                + Vector3.right
                * Random.Range(-0.1f, 0.1f),
                Quaternion.identity, emptyPrey);
        }

        for (int i = 0; i < 30; i++)
        {
            Instantiate(predator,
                predator.transform.position
                + Vector3.up
                * Random.Range(-0.1f, 0.1f)
                + Vector3.right
                * Random.Range(-0.1f, 0.1f),
                Quaternion.identity, emptyPredator);
        }
    }
}
