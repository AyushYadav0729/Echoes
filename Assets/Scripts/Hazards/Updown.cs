using UnityEngine;

public class UpDownMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float distance = 3f;
    public float speed = 2f;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        float offset = Mathf.Sin(Time.time * speed) * distance;

        transform.position = startPosition + Vector3.up * offset;
    }
}