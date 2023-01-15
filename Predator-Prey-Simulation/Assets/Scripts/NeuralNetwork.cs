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
    [SerializeField]
    float[] inputs = new float[4];
    List<GameObject> visibleObjects = new();
    // Start is called before the first frame update
    void Start()
    {
        if (isPrey)
        {
        StartCoroutine(IsMoving());
        }
        if (isPredator)
        {
            inputs = new float[5];
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
                Quaternion.identity);
            }
            if (isPredator)
            {
            Instantiate(SpawnOGAnimals.predator,
                transform.position
                + Vector3.up
                * Random.Range(-0.1f, 0.1f)
                + Vector3.right
                * Random.Range(-0.1f, 0.1f),
                Quaternion.identity);
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
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isPredator)
        {
            if (collision.gameObject.name == "Prey(Clone)")
            {
                Destroy(collision.gameObject);
                progressToReproduce+= 1;
            }
        }
    }
    void AssignInputs()
    {
        inputs[0] = thrust;
        inputs[1] = progressToReproduce;
        inputs[2] = visibleObjects.Count;
        inputs[3] = ShortestDistanceOfVisible();
    }
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.gameObject.name != gameObject.name)
        {
            visibleObjects.Add(collider.gameObject);
        }
    }
    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.name != gameObject.name)
        {
            visibleObjects.Remove(collider.gameObject);
        }
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
        for (int i = 0; i < visibleObjects.Count; i++)
        {
            results.Add(PythagoreanTheorem(
                Mathf.Abs(visibleObjects[i].transform.position.x - transform.position.x),
                Mathf.Abs(visibleObjects[i].transform.position.y - transform.position.y)));
        }
        return Mathf.Min(results.ToArray());
    }
}
