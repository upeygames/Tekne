using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SimpleBuoyancy : MonoBehaviour
{
    [Header("Su Ayarları")]
    public Transform waterSurface;
    public float floatStrength = 30f;

    private Rigidbody rb;
    private BoatScript boatScript;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        boatScript = GetComponent<BoatScript>();
    }

    void FixedUpdate()
    {
        if (boatScript != null && boatScript.IsTouchingTerrain())
            return;

        float waterLevel = GetWaterHeightAtPosition(transform.position);
        float boatY = transform.position.y;
        float depth = waterLevel - boatY;

        if (depth > 0f)
        {
            float forceAmount = Mathf.Clamp(depth * floatStrength, 0f, floatStrength * 2f);
            rb.AddForce(Vector3.up * forceAmount, ForceMode.Acceleration);
        }
        else
        {
            rb.AddForce(Vector3.up * 1.5f, ForceMode.Acceleration);
        }
    }

    float GetWaterHeightAtPosition(Vector3 position)
    {
        return waterSurface != null ? waterSurface.position.y : 0f;
    }
}
