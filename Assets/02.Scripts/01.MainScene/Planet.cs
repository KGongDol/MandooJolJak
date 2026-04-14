using UnityEngine;

public class Planet : MonoBehaviour
{
    public float speed = 10;

    void Update()
    {
        transform.Rotate(Vector3.up * speed * Time.deltaTime, Space.Self);
    }
}
