using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem; 

public class CameraScrollZoom : MonoBehaviour
{
    [SerializeField] CinemachineOrbitalFollow orbital;
    [SerializeField] float zoomSpeed = 0.01f;
    [SerializeField] float minRadius = 2f;
    [SerializeField] float maxRadius = 15f;

    void Reset() => orbital = GetComponent<CinemachineOrbitalFollow>();

    void Update()
    {
        float scroll = Mouse.current.scroll.ReadValue().y; 
        if (Mathf.Approximately(scroll, 0f)) return;

        if (orbital.OrbitStyle == CinemachineOrbitalFollow.OrbitStyles.Sphere)
        {
            orbital.Radius = Mathf.Clamp(orbital.Radius - scroll * zoomSpeed, minRadius, maxRadius);
        }
        else 
        {
            var axis = orbital.RadialAxis;
            axis.Value = Mathf.Clamp(axis.Value - scroll * zoomSpeed * 0.1f, axis.Range.x, axis.Range.y);
            orbital.RadialAxis = axis;
        }
    }
}