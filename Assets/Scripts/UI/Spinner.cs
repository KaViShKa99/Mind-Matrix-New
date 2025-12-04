using UnityEngine;

public class Spinner : MonoBehaviour
{
    public float speed = 350f; // degrees per second

    void Update()
    {
        transform.Rotate(0, 0, -speed * Time.deltaTime);
    }
}
