using UnityEngine;


public class SmoothCamera : MonoBehaviour
{

    public Transform target;

    public float smoothSpeed = 10f;

    public Vector3 offset;


    void LateUpdate()
    {

        Vector3 desiredPosition =
            target.position + offset;


        transform.position =
            Vector3.Lerp(
                transform.position,
                desiredPosition,
                smoothSpeed * Time.deltaTime
            );


        transform.LookAt(target);

    }
}