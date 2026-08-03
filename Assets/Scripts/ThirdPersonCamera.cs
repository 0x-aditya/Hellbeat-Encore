using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;

    public float distance = 5f;
    public float mouseSensitivity = 200f;

    public float minY = -20f;
    public float maxY = 60f;


    private float yaw;
    private float pitch;


    void LateUpdate()
    {
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        pitch = Mathf.Clamp(pitch, minY, maxY);


        Quaternion rotation =
            Quaternion.Euler(pitch, yaw, 0);


        Vector3 position =
            target.position - rotation * Vector3.forward * distance;


        transform.position = position;

        transform.rotation = rotation;
    }
}