using UnityEngine;

public class RotateAround : MonoBehaviour
{
    [SerializeField] private Vector3 centerPoint = Vector3.zero;
    [SerializeField] private float rotationSpeed = 30.0f; // deg/s

    private void Update()
    {
        transform.RotateAround(centerPoint, Vector3.up, rotationSpeed * Time.deltaTime);
    }
}