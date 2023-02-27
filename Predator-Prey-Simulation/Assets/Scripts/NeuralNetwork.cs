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
    float thrust;
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
            Reproduce();

        }

        Rigidbody2D.velocity = transform.up * thrust;
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
        inputs[0] = DirectionToGameobject();
        inputs[1] = progressToReproduce;
        inputs[2] = FieldOfView.visibleObjects.Count;
        inputs[3] = ShortestDistanceOfVisible();
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
        if (FieldOfView.visibleObjects.Count != 0)
        {
            // Loop through each visible gameobject
            for (int i = 0; i < FieldOfView.visibleObjects.Count; i++)
            {
                print(FieldOfView.visibleObjects.Count + " Objects");
                // Calculate the distance from the current gameobject to the visible gameobject using the Pythagorean theorem
                results.Add(PythagoreanTheorem(
                    Mathf.Abs(FieldOfView.visibleObjects[i].transform.position.x - transform.position.x),
                    Mathf.Abs(FieldOfView.visibleObjects[i].transform.position.y - transform.position.y)));
            }
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
        /*
        if (Mathf.Min(results.ToArray()) == float.PositiveInfinity)
        {
            Debug.Break();
        }
        */
        // Return the shortest distance from the current gameobject to the visible gameobjects
        if (closestDistanceIndex != -1)
        {
            return Mathf.Min(results.ToArray());
        }
        return 0;
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
        // Initialize two arrays of floating point numbers to hold the values of nodes and the values of nodes from the previous iteration
        float[] values = new float[7];
        float[] lastValues = new float[7];

        // Loop through each layer of nodes in the network
        for (int i = 0; i < 5; i++)
        {
            // Loop through each individual node in the current layer
            for (int r = 0; r < 7; r++)
            {
                // Store the current value of the node in an array
                values[r] = nodes[i][r].value;

                // If the current layer is not the input layer, calculate the value of the current node based on the previous layer's values
                if (i != 0)
                {
                    nodes[i][r].value = CalculateNodes(
                        lastValues,
                        nodes[i][r].inputWeights,
                        nodes[i][r].bias);
                }
                // If the current layer is the input layer, calculate the value of the current node based on the input values
                else
                {
                    nodes[i][r].value = CalculateNodes(
                        inputs,
                        nodes[i][r].inputWeights,
                        nodes[i][r].bias);
                }
            }

            // Copy the values of the current layer's nodes to the lastValues array to be used in the next iteration
            Array.Copy(values, lastValues, values.Length);
        }

        // Calculate the output value by summing the lastValues array
        float outputValues = Sum(lastValues);

        // Rotate the object using the output value and a smoothing factor
        transform.rotation = Quaternion.Euler(0, 0, Mathf.Lerp(transform.rotation.z, outputValues, 0.05f));

        // Set the thrust value based on the output value and a smoothing factor, clamping it between -3 and 5
        thrust = Mathf.Lerp(thrust, Mathf.Clamp(outputValues, -3, 5), 0.25f);
    }

    void CreateNodes()
    {
        for (int i = 0; i < 5; i++)
        {
            nodes[i] = new Node[7];
            for (int r = 0; r < 7; r++)
            {
                nodes[i][r].value = 0;
                nodes[i][r].inputWeights = new float[7];
                nodes[i][r].bias = 0;

            }
        }
    }
    // This method calculates the sum of the elements in a float array
    float Sum(float[] arguments)
    {
        if (arguments.Length == 0)
        {
            return 0;
        }

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
    void Reproduce()
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
    float CalculateNodes(float[] values, float[] weights, float bias)
    {
        float[] alteredValues = new float[values.Length];
        float result;
        for (int i = 0; i < values.Length; i++)
        {
            alteredValues[i] = values[i] * weights[i];
        }
        result = Sum(alteredValues) + bias;
        return result;
    }
}
struct Node
{
    public float bias;
    public float[] inputWeights;
    public float value;
}
