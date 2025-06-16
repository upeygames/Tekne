using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BoatScript : MonoBehaviour
{
    [Header("Hareket Ayarları")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float turnSpeed = 50f;

    [Header("Kenar Duvar Ayarları")]
    [SerializeField] private float wallHeight = 1.5f;
    [SerializeField] private float wallThickness = 0.1f;

    private Rigidbody boatRb;
    private bool isTouchingTerrain = false;

    private const float originalLinearDamping = 0.5f;
    private const float terrainLinearDamping = 5f;

    private void Start()
    {
        boatRb = GetComponent<Rigidbody>();

        boatRb.mass = 1000f;
        boatRb.interpolation = RigidbodyInterpolation.Interpolate;
        boatRb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        boatRb.linearDamping = originalLinearDamping;
        boatRb.angularDamping = 10f;
        boatRb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        AddEdgeColliders();
    }

    private void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        float moveInput = Input.GetAxis("Vertical");
        float turnInput = Input.GetAxis("Horizontal");

        if (isTouchingTerrain)
        {
            float rotation = turnSpeed * turnInput * Time.deltaTime;
            Quaternion turnRotation = Quaternion.AngleAxis(rotation, Vector3.up) * boatRb.rotation;
            boatRb.MoveRotation(turnRotation);
            return;
        }

        Vector3 move = transform.forward * (moveSpeed * moveInput * Time.deltaTime);
        boatRb.MovePosition(boatRb.position + move);

        float rot = turnSpeed * turnInput * Time.deltaTime;
        if (moveInput < 0f) rot *= -1f;

        Quaternion rotationQuat = Quaternion.AngleAxis(rot, Vector3.up) * boatRb.rotation;
        boatRb.MoveRotation(rotationQuat);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Terrain"))
        {
            isTouchingTerrain = true;
            boatRb.linearDamping = terrainLinearDamping;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Terrain"))
        {
            isTouchingTerrain = false;
            boatRb.linearDamping = originalLinearDamping;
        }
    }

    public bool IsTouchingTerrain() => isTouchingTerrain;

    // Kenar ekleyen sistem
    private void AddEdgeColliders()
    {
        Transform cube = transform.Find("Küp");
        if (cube == null) return;

        foreach (Transform yuzey in cube)
        {
            BoxCollider baseCollider = yuzey.GetComponent<BoxCollider>();
            if (baseCollider == null) continue;

            CreateBordersAround(yuzey, baseCollider);
        }
    }

    private void CreateBordersAround(Transform surfaceTransform, BoxCollider baseCollider)
    {
        Vector3 size = baseCollider.size;
        Vector3 center = baseCollider.center;

        Vector3[] positions = new Vector3[]
        {
        new Vector3(0, 0,  size.z / 2),
        new Vector3(0, 0, -size.z / 2),
        new Vector3(-size.x / 2, 0, 0),
        new Vector3( size.x / 2, 0, 0)
        };

        Vector3[] scales = new Vector3[]
        {
        new Vector3(size.x, wallHeight, wallThickness),
        new Vector3(size.x, wallHeight, wallThickness),
        new Vector3(wallThickness, wallHeight, size.z),
        new Vector3(wallThickness, wallHeight, size.z)
        };

        for (int i = 0; i < 4; i++)
        {
            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name = $"Kenar_{i}";
            wall.transform.SetParent(surfaceTransform);
            wall.transform.localPosition = center + positions[i] + Vector3.up * (wallHeight / 2f);
            wall.transform.localRotation = Quaternion.identity;
            wall.transform.localScale = scales[i];

            DestroyImmediate(wall.GetComponent<MeshRenderer>());

            BoxCollider col = wall.GetComponent<BoxCollider>();

            // 🔷 Yuzey1 - tüm Kenarlar
            if (surfaceTransform.name == "Yuzey1")
            {
                col.center = new Vector3(0f, 2.85f, 0f);
                col.size = new Vector3(1f, 5.7f, 0.9999999f);
            }

            // 🔷 Yuzey2 - Kenar_2 özel
            if (surfaceTransform.name == "Yuzey2" && wall.name == "Kenar_2")
            {
                col.center = new Vector3(-1.035241f, -1.719662f, -0.0251088f);
                col.size = new Vector3(1f, 2.716504f, 0.925404f);
            }

            // 🔷 Yuzey2 - Kenar_3 özel
            if (surfaceTransform.name == "Yuzey2" && wall.name == "Kenar_3")
            {
                col.center = new Vector3(-5.476006f, -1.719662f, -0.0251088f);
                col.size = new Vector3(1f, 2.716504f, 0.925404f);
            }

            // 🔷 Yuzey3 - Kenar_1 özel
            if (surfaceTransform.name == "Yuzey3" && wall.name == "Kenar_1")
            {
                col.center = new Vector3(-0.0116953f, -2.533223f, -0.00007850017f);
                col.size = new Vector3(0.7293102f, 1f, 0.9999999f);
            }

            // 🔷 Yuzey3 - Kenar_2 özel
            if (surfaceTransform.name == "Yuzey3" && wall.name == "Kenar_2")
            {
                col.center = new Vector3(-0.0116953f, -2.533223f, -0.00007850017f);
                col.size = new Vector3(0.7293102f, 1f, 0.9999999f);
            }

            // 🔷 Yuzey3 - Kenar_3 özel
            if (surfaceTransform.name == "Yuzey3" && wall.name == "Kenar_3")
            {
                col.center = new Vector3(-0.0116953f, -2.533223f, -0.00007850017f);
                col.size = new Vector3(0.7293102f, 1f, 0.9999999f);
            }

            // 💠 Fizik materyali (tutarlılık için her kenara uygulanır)
            var mat = new PhysicsMaterial();
            mat.frictionCombine = PhysicsMaterialCombine.Minimum;
            mat.bounceCombine = PhysicsMaterialCombine.Minimum;
            mat.dynamicFriction = 0f;
            mat.staticFriction = 0f;
            mat.bounciness = 0f;
            col.material = mat;
        }
    }

}
