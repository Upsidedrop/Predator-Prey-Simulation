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
    // Start is called before the first frame update
    void Start()
    {
        if (isPrey)
        {
        StartCoroutine(IsMoving());
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

    }
    IEnumerator IsMoving()
    {
        yield return new WaitForSeconds(0.1f);
        //Checks if AI moved on it's last turn
        if (lastPos == transform.position)
        {
            progressToReproduce += 0.1f;
        }
        else
        {
            progressToReproduce = 0;
        }
        lastPos = transform.position;
        StartCoroutine(IsMoving());
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
                print(collision.gameObject.name);
                Destroy(collision.gameObject);
                progressToReproduce+= 1;
            }
        }
    }
}
