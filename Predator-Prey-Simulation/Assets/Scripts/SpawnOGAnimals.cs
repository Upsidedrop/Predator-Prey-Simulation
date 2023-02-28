using UnityEngine;

public class SpawnOGAnimals : MonoBehaviour
{
    public GameObject prey;
    public GameObject predator;
    Node[][] preset = new Node[6][];
    public Transform emptyPrey;

    public Transform emptyPredator;
    private void Awake()
    {
        for (int i = 0; i < 6; i++)
        {
            preset[i] = new Node[7];
            for (int r = 0; r < 7; r++)
            {
                preset[i][r].value = 0;
                preset[i][r].inputWeights = new float[]{0,0,0,0,0,0,0};
                preset[i][r].bias = 0;

            }
        }
        for (int i = 0; i < 30; i++)
        {
            GameObject discard;
            discard = Instantiate(prey,
                prey.transform.position
                + Vector3.up
                * Random.Range(-0.1f, 0.1f)
                + Vector3.right
                * Random.Range(-0.1f, 0.1f),
                Quaternion.identity, emptyPrey);

            discard.GetComponent<NeuralNetwork>().Mutate(preset);
        }

        for (int i = 0; i < 30; i++)
        {
            GameObject discard;
            discard = Instantiate(predator,
                predator.transform.position
                + Vector3.up
                * Random.Range(-0.1f, 0.1f)
                + Vector3.right
                * Random.Range(-0.1f, 0.1f),
                Quaternion.identity, emptyPredator);

            discard.GetComponent<NeuralNetwork>().Mutate(preset);
        }
    }
}
