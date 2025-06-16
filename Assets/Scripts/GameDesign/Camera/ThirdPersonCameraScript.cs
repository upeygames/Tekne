using UnityEngine;

public class ThirdPersonCameraScript : MonoBehaviour
{
    public Transform target;
    public float distance = 10f;
    public float minDistance = 5f;
    public float maxDistance = 20f;
    public float zoomSpeed = 2f;
    public float rotationSpeed = 100f;

    private float currentYaw = 0f;
    private float currentPitch = 10f;
    public float minPitch = 0f;
    public float maxPitch = 45f;

    void Update()
    {
        if (target == null) return;

        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

        currentYaw += mouseX;
        currentPitch -= mouseY;
        currentPitch = Mathf.Clamp(currentPitch, minPitch, maxPitch);

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        distance -= scroll * zoomSpeed;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);

        Quaternion rotation = Quaternion.Euler(currentPitch, currentYaw, 0);
        Vector3 offset = new(0, 2f, 0); // IDE0090: new Vector3(...) basitleþtirildi

        transform.position = target.position + offset - (rotation * Vector3.forward * distance);
        transform.LookAt(target.position + offset);
    }
}
