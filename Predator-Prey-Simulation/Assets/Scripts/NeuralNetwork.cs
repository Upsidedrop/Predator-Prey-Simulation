using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuralNetwork : MonoBehaviour
{
    [SerializeField]
    float progressToReproduce;
    Vector3 lastPos;
    SpawnOGAnimals SpawnOGAnimals;
    bool isPrey = false;
    bool isPredator = false;
    Rigidbody2D Rigidbody2D;
    int thrust;
    float[] inputs = new float[5];
    [SerializeField]
    GameObject closestGameobject;
    FieldOfView FieldOfView;
    Node[][] node = new Node[5][];
    // Start is called before the first frame update
    void Start()
    {
        if (isPrey)
        {
            StartCoroutine(IsMoving());
        }
        if (isPredator)
        {
            inputs = new float[6];
        }
        thrust = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (progressToReproduce >= 3)
        {
            progressToReproduce = 0;
            if (isPrey)
            {
                Instantiate(SpawnOGAnimals.prey,
                    transform.position
                    + Vector3.up
                    * Random.Range(-0.1f, 0.1f)
                    + Vector3.right
                    * Random.Range(-0.1f, 0.1f),
                    Quaternion.identity, SpawnOGAnimals.emptyPrey);
            }
            if (isPredator)
            {
                Instantiate(SpawnOGAnimals.predator,
                    transform.position
                    + Vector3.up
                    * Random.Range(-0.1f, 0.1f)
                    + Vector3.right
                    * Random.Range(-0.1f, 0.1f),
                    Quaternion.identity, SpawnOGAnimals.emptyPredator);
            }

        }
        AssignInputs();
    }
    IEnumerator IsMoving() //coroutine to check if AI is moving
    {
        yield return new WaitForSeconds(0.1f);
        //Checks if AI moved on it's last turn
        if (lastPos == transform.position)
        {
            progressToReproduce += 0.1f; //increment progress if not moving
        }
        else
        {
            progressToReproduce = 0; //reset progress if moving
        }
        lastPos = transform.position;
        StartCoroutine(IsMoving()); //restart coroutine
    }
    private void Awake()
    {
        if (gameObject.name == "Prey(Clone)")
        {
            isPrey = true;
        }
        if (gameObject.name == "Predator(Clone)")
        {
            isPredator = true;
        }
        SpawnOGAnimals = GameObject.Find("Script Manager")
                                   .GetComponent<SpawnOGAnimals>();
        Rigidbody2D = GetComponent<Rigidbody2D>();
        FieldOfView = GetComponentInChildren<FieldOfView>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isPredator)
        {
            if (collision.gameObject.name == "Prey(Clone)")
            {
                
                Destroy(collision.gameObject);
                progressToReproduce += 1;

            }
        }
    }
    void AssignInputs()
    {
        inputs[0] = thrust;
        inputs[1] = progressToReproduce;
        inputs[2] = FieldOfView.visibleObjects.Count;
        inputs[3] = ShortestDistanceOfVisible();
        inputs[4] = DirectionToGameobject();
    }

    float PythagoreanTheorem(float input1, float input2)
    {
        return Mathf.Sqrt(
            (input1 * input1) + (input2 * input2)
            );
    }
    float ShortestDistanceOfVisible()
    {
        List<float> results = new();
        int closestDistanceIndex = -1;
        for (int i = 0; i < FieldOfView.visibleObjects.Count; i++)
        {
            results.Add(PythagoreanTheorem(
                Mathf.Abs(FieldOfView.visibleObjects[i].transform.position.x - transform.position.x),
                Mathf.Abs(FieldOfView.visibleObjects[i].transform.position.y - transform.position.y)));
        }
        for (int i = 0; i < results.Count; i++)
        {
            if (Mathf.Min(results.ToArray()) == results[i])
            {
                closestDistanceIndex = i;
            }
        }
        if (closestDistanceIndex > -1)
        {
            closestGameobject = FieldOfView.visibleObjects[closestDistanceIndex];
        }
        else
        {
            closestGameobject = null;
        }
        return Mathf.Min(results.ToArray());
    }
    float DirectionToGameobject()
    {
        if (closestGameobject != null)
        {
            Vector3 direction = (closestGameobject.transform.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            return angle;
        }
        return 0;
    }
}
   struct Node
   {
       public float[] inputBiases;
       public float[] inputWeights;
       public float value;
       public int layer;
   }
