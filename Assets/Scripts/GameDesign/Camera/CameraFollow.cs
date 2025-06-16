using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;  // Tekne objesi
    public float smoothSpeed = 0.125f; // Kameranýn yumuþak hareket hýzý
    public Vector3 offset;  // Kameranýn tekneye göre ofset pozisyonu
    public float rotationSpeed = 100f; // Mouse ile döndürme hýzý
    private float currentYaw = 0f; // Y ekseni etrafýnda dönüþ açýsý

    void Update()
    {
        if (target == null) return; // Eðer hedef yoksa çýk

        // Farenin hareketine baðlý olarak dönüþ açýsýný hesapla
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
        currentYaw += mouseX; // Y ekseni etrafýnda dönüþ açýsýný güncelle

        // Kamera pozisyonunu belirle
        Quaternion rotation = Quaternion.Euler(0, currentYaw, 0); // Y ekseninde döndürme
        Vector3 desiredPosition = target.position + rotation * offset;

        // Kamera pozisyonunu yumuþakça hareket ettir
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Kamera her zaman hedefe bakmalý
        transform.LookAt(target);
    }
}
