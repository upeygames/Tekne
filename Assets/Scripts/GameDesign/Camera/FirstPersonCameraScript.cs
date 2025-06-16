using UnityEngine;

public class FirstPersonCameraScript : MonoBehaviour
{
    public Transform playerTransform;
    public float sensitivity = 2f;

    private float yaw = 0f;
    private float pitch = 0f;
    public float minPitch = -60f;
    public float maxPitch = 60f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        // Baþlangýçta karakter yönüne göre senkron baþlasýn
        yaw = playerTransform.eulerAngles.y;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        // Kamera hem saða/sola (yaw) hem yukarý/aþaðý (pitch) döner
        transform.rotation = Quaternion.Euler(pitch, yaw, 0);

        // Karakter sadece saða/sola (yaw) döner
        if (playerTransform != null)
        {
            playerTransform.rotation = Quaternion.Euler(0, yaw, 0);
            transform.position = playerTransform.position + new Vector3(0, 1.7f, 0);
        }
    }
}
