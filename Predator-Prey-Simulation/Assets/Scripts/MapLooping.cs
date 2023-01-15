using UnityEngine;

public class MapLooping : MonoBehaviour
{
    Camera Camera;
    // Update is called once per frame
    void Update()
    {
        if (Camera.WorldToScreenPoint(transform.position).x < 0)
        {
            transform.position = Camera.ScreenToWorldPoint(new Vector3(Camera.pixelWidth - 5, Camera.WorldToScreenPoint(transform.position).y)) - Camera.transform.position;
        }
        if (Camera.WorldToScreenPoint(transform.position).x > Camera.pixelWidth)
        {
            transform.position = Camera.ScreenToWorldPoint(new Vector3(0 + 5, Camera.WorldToScreenPoint(transform.position).y)) - Camera.transform.position;
        }
        if (Camera.WorldToScreenPoint(transform.position).y < 0)
        {
            transform.position = Camera.ScreenToWorldPoint(new Vector3(Camera.WorldToScreenPoint(transform.position).x, Camera.pixelHeight - 5)) - Camera.transform.position;
        }
        if (Camera.WorldToScreenPoint(transform.position).y > Camera.pixelHeight)
        {
            transform.position = Camera.ScreenToWorldPoint(new Vector3(Camera.WorldToScreenPoint(transform.position).x, 0 + 5)) - Camera.transform.position;
        }
    }
    private void Awake()
    {
        Camera = GameObject.Find("Main Camera").GetComponent<Camera>();
    }
}
