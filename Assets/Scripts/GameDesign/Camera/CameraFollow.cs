using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;  // Tekne objesi
    public float smoothSpeed = 0.125f; // Kameran�n yumu�ak hareket h�z�
    public Vector3 offset;  // Kameran�n tekneye g�re ofset pozisyonu
    public float rotationSpeed = 100f; // Mouse ile d�nd�rme h�z�
    private float currentYaw = 0f; // Y ekseni etraf�nda d�n�� a��s�

    void Update()
    {
        if (target == null) return; // E�er hedef yoksa ��k

        // Farenin hareketine ba�l� olarak d�n�� a��s�n� hesapla
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
        currentYaw += mouseX; // Y ekseni etraf�nda d�n�� a��s�n� g�ncelle

        // Kamera pozisyonunu belirle
        Quaternion rotation = Quaternion.Euler(0, currentYaw, 0); // Y ekseninde d�nd�rme
        Vector3 desiredPosition = target.position + rotation * offset;

        // Kamera pozisyonunu yumu�ak�a hareket ettir
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Kamera her zaman hedefe bakmal�
        transform.LookAt(target);
    }
}
