using System;
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
    Node[][] nodes = new Node[5][];
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
                    * UnityEngine.Random.Range(-0.1f, 0.1f)
                    + Vector3.right
                    * UnityEngine.Random.Range(-0.1f, 0.1f),
                    Quaternion.identity, SpawnOGAnimals.emptyPrey);
            }
            if (isPredator)
            {
                Instantiate(SpawnOGAnimals.predator,
                    transform.position
                    + Vector3.up
                    * UnityEngine.Random.Range(-0.1f, 0.1f)
                    + Vector3.right
                    * UnityEngine.Random.Range(-0.1f, 0.1f),
                    Quaternion.identity, SpawnOGAnimals.emptyPredator);
            }

        }
        AssignInputs();
        AssignNodes();
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
        CreateNodes();
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

    // This method calculates the hypotenuse of a right triangle using the Pythagorean theorem
    float PythagoreanTheorem(float input1, float input2)
    {
        // Calculate the hypotenuse using the Pythagorean theorem
        return Mathf.Sqrt(
            (input1 * input1) + (input2 * input2)
            );
    }

    // This method calculates the shortest distance of visible gameobjects from the current gameobject
    float ShortestDistanceOfVisible()
    {
        // Create a list to store the results of the Pythagorean theorem for each visible gameobject
        List<float> results = new();

        // Initialize a variable to hold the index of the closest visible gameobject
        int closestDistanceIndex = -1;

        // Loop through each visible gameobject
        for (int i = 0; i < FieldOfView.visibleObjects.Count; i++)
        {
            // Calculate the distance from the current gameobject to the visible gameobject using the Pythagorean theorem
            results.Add(PythagoreanTheorem(
                Mathf.Abs(FieldOfView.visibleObjects[i].transform.position.x - transform.position.x),
                Mathf.Abs(FieldOfView.visibleObjects[i].transform.position.y - transform.position.y)));
        }

        // Loop through the results to find the index of the closest visible gameobject
        for (int i = 0; i < results.Count; i++)
        {
            if (Mathf.Min(results.ToArray()) == results[i])
            {
                closestDistanceIndex = i;
            }
        }

        // If there is a closest visible gameobject
        if (closestDistanceIndex > -1)
        {
            // Set the closest gameobject to the closest visible gameobject
            closestGameobject = FieldOfView.visibleObjects[closestDistanceIndex];
        }
        else
        {
            // If there is no closest visible gameobject, set the closest gameobject to null
            closestGameobject = null;
        }

        // Return the shortest distance from the current gameobject to the visible gameobjects
        return Mathf.Min(results.ToArray());
    }

    // This method calculates the direction from the current gameobject to the closest gameobject
    float DirectionToGameobject()
    {
        // If there is a closest gameobject
        if (closestGameobject != null)
        {
            // Calculate the direction from the current gameobject to the closest gameobject
            Vector3 direction = (closestGameobject.transform.position - transform.position).normalized;

            // Calculate the angle between the x-axis and the direction
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Return the angle
            return angle;
        }

        // If there is no closest gameobject, return 0
        return 0;
    }


    // This method assigns values to the nodes in a neural network
    void AssignNodes()
    {
        // Create an array to store the values of the current layer's nodes
        float[] values = new float[7];

        // Create an array to store the values of the last layer's nodes
        float[] lastValues = new float[7];

        // Loop through each layer of the neural network
        for (int i = 0; i < 5; i++)
        {
            // Loop through each node in the current layer
            for (int r = 0; r < 7; r++)
            {
                // Store the value of the current node
                values[r] = nodes[i][r].value;

                // If we're not at the first layer of the network
                if (i != 0)
                {
                    // Set the value of the current node to the sum of the last layer's node values
                    nodes[i][r].value = Sum(lastValues);
                }
                // If we're at the first layer of the network
                else
                {
                    // Set the value of the current node to the sum of the input node values
                    nodes[i][r].value = Sum(inputs);
                }
            }

            // Copy the values of the current layer's nodes into the lastValues array
            Array.Copy(values, lastValues, values.Length);
        }
    }

    void CreateNodes()
    {
        for (int i = 0; i < 5; i++)
        {
            nodes[i] = new Node[7];
            for (int r = 0; r < 7; r++)
            {
                nodes[i][r].layer = i;
            }
        }
    }
    // This method calculates the sum of the elements in a float array
    float Sum(float[] arguments)
    {
        // Initialize a variable to hold the sum
        float sum = 0;

        // Loop through each element in the array
        foreach (float argument in arguments)
        {
            // Add the current element to the sum
            sum += argument;
        }

        // Return the sum of the elements in the array
        return sum;
    }

}
struct Node
{
    public float[] inputBiases;
    public float[] inputWeights;
    public float value;
    public int layer;
}
