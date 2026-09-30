using UnityEngine;

[ExecuteInEditMode]
public class CopyPosition : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;
    [SerializeField] private bool rotate;
        
    void Update()
    {
        transform.position = target.position  + offset;
        if (rotate)
            transform.rotation = Quaternion.Euler(target.eulerAngles);
    }
}
