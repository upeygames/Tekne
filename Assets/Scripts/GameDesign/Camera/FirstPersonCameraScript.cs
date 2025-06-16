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

        // Ba�lang��ta karakter y�n�ne g�re senkron ba�las�n
        yaw = playerTransform.eulerAngles.y;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        // Kamera hem sa�a/sola (yaw) hem yukar�/a�a�� (pitch) d�ner
        transform.rotation = Quaternion.Euler(pitch, yaw, 0);

        // Karakter sadece sa�a/sola (yaw) d�ner
        if (playerTransform != null)
        {
            playerTransform.rotation = Quaternion.Euler(0, yaw, 0);
            transform.position = playerTransform.position + new Vector3(0, 1.7f, 0);
        }
    }
}
