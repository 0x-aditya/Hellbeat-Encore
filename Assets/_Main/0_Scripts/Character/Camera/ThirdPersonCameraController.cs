using UnityEngine;
using Unity.Cinemachine;

public class ThirdPersonCameraController : MonoBehaviour
{
    public CinemachineCamera cinemachineCamera;

    [Header("Mouse")]
    public float mouseSensitivity = 2f;

    [Header("Vertical Limits")]
    public float minPitch = -20f;
    public float maxPitch = 60f;

    private CinemachineOrbitalFollow orbitalFollow;

    private float yaw;
    private float pitch = 15f;

    void Start()
    {
        orbitalFollow =
            cinemachineCamera.GetComponent<CinemachineOrbitalFollow>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        yaw += mouseX * mouseSensitivity;
        pitch -= mouseY * mouseSensitivity;

        pitch = Mathf.Clamp(
            pitch,
            minPitch,
            maxPitch
        );

        orbitalFollow.HorizontalAxis.Value = yaw;
        orbitalFollow.VerticalAxis.Value = pitch;
    }
}