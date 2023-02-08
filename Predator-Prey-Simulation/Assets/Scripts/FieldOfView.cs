using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public List<GameObject> visibleObjects = new();
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.name != transform.parent.name && collider.gameObject.name != transform.name)
        {
            visibleObjects.Add(collider.gameObject);
        }
    }
    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.name != transform.parent.name && collider.gameObject.name != transform.name)
        {
            visibleObjects.Remove(collider.gameObject);
        }
    }
}
