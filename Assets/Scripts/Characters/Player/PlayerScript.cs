using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class PlayerScript : MonoBehaviour
{
    [Header("Hareket ve Zıplama Ayarları")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 6.5f;

    [Header("Havada Yerçekimi Gücü")]
    [SerializeField] private float fallGravityMultiplier = 3f;
    [SerializeField] private float lowJumpGravityMultiplier = 2f;

    [Header("Kameralar")]
    [SerializeField] private Camera firstPersonCamera;
    [SerializeField] private Camera thirdPersonCamera;

    [Header("Tekne ve Su Yüzeyi")]
    [SerializeField] private Transform boatTransform;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform waterSurface;

    private BoatScript boatScript;
    private Rigidbody playerRb;
    private Animator animator;
    private CapsuleCollider capsuleCollider;

    private bool isFPSMode = true;
    private bool isGrounded = true;
    private bool isJumping = false;
    private bool initialSpawnDone = false;

    private void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        boatScript = boatTransform.GetComponent<BoatScript>();

        playerRb.linearDamping = 5f;

        AdjustCapsuleColliderToModel();
        EnableFPSMode();
    }

    private void Update()
    {
        HandleModeSwitch();

        if (isFPSMode)
        {
            HandlePlayerMovement();
            HandleJumpInput();
            HandleNpcInteraction();
        }

        isGrounded = IsTouchingGround();
        animator.SetBool("isGrounded", isGrounded);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    private void FixedUpdate()
    {
        if (!isFPSMode) return;

        float velocityY = playerRb.linearVelocity.y;

        if (!isGrounded && velocityY < 0f)
        {
            Vector3 extraGravity = Vector3.down * 9.81f * fallGravityMultiplier * playerRb.mass;
            playerRb.AddForce(extraGravity, ForceMode.Force);
        }
        else if (!isGrounded && !Input.GetKey(KeyCode.Space) && velocityY > 0f)
        {
            Vector3 lowJumpGravity = Vector3.down * 9.81f * lowJumpGravityMultiplier * playerRb.mass;
            playerRb.AddForce(lowJumpGravity, ForceMode.Force);
        }
    }

    private void HandleModeSwitch()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && !isFPSMode)
            EnableFPSMode();

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (isFPSMode && IsTouchingBoat())
                EnableTPSMode();
            else if (isFPSMode)
                Debug.Log("⚠ Tekneye binmeden TPS moduna geçemezsin.");
        }
    }

    private void EnableFPSMode()
    {
        isFPSMode = true;
        boatScript.enabled = false;

        firstPersonCamera.gameObject.SetActive(true);
        thirdPersonCamera.gameObject.SetActive(false);
        transform.SetParent(null, true);

        playerRb.isKinematic = false;
        playerRb.useGravity = true;
        playerRb.mass = 10f;
        playerRb.linearDamping = 0f;
        playerRb.interpolation = RigidbodyInterpolation.Interpolate;
        playerRb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        playerRb.linearVelocity = Vector3.zero;
        playerRb.angularVelocity = Vector3.zero;

        AdjustCapsuleColliderToModel();

        if (!initialSpawnDone)
        {
            Vector3 rayOrigin = transform.position + Vector3.up * 1.5f;
            Vector3 rayDir = Vector3.down;

            if (Physics.Raycast(rayOrigin, rayDir, out RaycastHit hit, 5f, ~0, QueryTriggerInteraction.Ignore))
            {
                float offset = capsuleCollider.height / 2f + 0.05f;
                playerRb.position = hit.point + Vector3.up * offset;
                Debug.Log($"✅ Zemin raycast ile yerleştirildi. Hit: {hit.collider.name}");
            }
            else
            {
                Debug.LogWarning("⚠ Karakterin altında zemin bulunamadı.");
            }

            initialSpawnDone = true;
        }

        transform.SetParent(boatTransform, true);
    }

    private void EnableTPSMode()
    {
        isFPSMode = false;

        bool groundedNow = IsTouchingGround();

        if (groundedNow)
        {
            playerRb.isKinematic = true;
            playerRb.useGravity = false;
            playerRb.interpolation = RigidbodyInterpolation.None;
        }
        else
        {
            playerRb.isKinematic = false;
            playerRb.useGravity = true;
            playerRb.interpolation = RigidbodyInterpolation.Interpolate;
            transform.SetParent(null, true);
        }

        transform.rotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);

        thirdPersonCamera.gameObject.SetActive(true);
        firstPersonCamera.gameObject.SetActive(false);

        boatScript.enabled = true;

        if (waterSurface != null && transform.position.y < waterSurface.position.y)
        {
            Vector3 newPos = transform.position;
            newPos.y = waterSurface.position.y + 0.5f;
            transform.position = newPos;
        }
    }

    private void HandlePlayerMovement()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 inputDir = new Vector3(h, 0, v).normalized;

        if (inputDir != Vector3.zero)
        {
            Vector3 moveVelocity = transform.TransformDirection(inputDir) * moveSpeed;
            playerRb.linearVelocity = new Vector3(moveVelocity.x, playerRb.linearVelocity.y, moveVelocity.z);
        }
        else
        {
            playerRb.linearVelocity = new Vector3(0, playerRb.linearVelocity.y, 0);
            playerRb.angularVelocity = Vector3.zero;
        }

        animator.SetFloat("Vertical", inputDir.z);
        animator.SetFloat("Horizontal", inputDir.x);
    }

    private void HandleJumpInput()
    {
        bool groundedNow = IsTouchingGround();
        isGrounded = groundedNow;

        if (Input.GetKeyDown(KeyCode.Space) && groundedNow && !isJumping)
        {
            animator.SetTrigger("Jump");
            isJumping = true;

            playerRb.linearVelocity = new Vector3(playerRb.linearVelocity.x, 0f, playerRb.linearVelocity.z);
            playerRb.linearVelocity += Vector3.up * jumpForce;
        }
    }

    private bool IsTouchingGround()
    {
        Vector3 rayOrigin = transform.position + Vector3.up * 0.1f;
        float rayLength = 0.6f;

        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, rayLength, ~0, QueryTriggerInteraction.Ignore))
        {
            return hit.collider.CompareTag("Boat") || hit.collider.CompareTag("Ground") || hit.collider.CompareTag("Terrain");
        }

        return false;
    }

    private bool IsTouchingBoat()
    {
        Vector3 origin = transform.position + Vector3.up * 0.1f;
        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, 1.2f))
        {
            return hit.collider.CompareTag("Boat");
        }
        return false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if ((collision.gameObject.CompareTag("Ground") ||
             collision.gameObject.CompareTag("Boat") ||
             collision.gameObject.CompareTag("Terrain") ||
             collision.gameObject.GetComponent<Terrain>() != null) && isJumping)
        {
            isGrounded = true;
            isJumping = false;
            animator.SetBool("isGrounded", true);
            playerRb.linearVelocity = Vector3.zero;
        }

        if (!isFPSMode && collision.gameObject == boatTransform.gameObject)
        {
            transform.SetParent(boatTransform, true);
            boatScript.enabled = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (!isFPSMode && collision.gameObject == boatTransform.gameObject)
        {
            transform.SetParent(null, true);
            boatScript.enabled = false;
        }
    }

    private void AdjustCapsuleColliderToModel()
    {
        if (capsuleCollider == null) return;

        var meshes = GetComponentsInChildren<SkinnedMeshRenderer>();
        if (meshes.Length == 0)
        {
            Debug.LogWarning("⚠ SkinnedMeshRenderer yok.");
            return;
        }

        Bounds combinedBounds = meshes[0].bounds;
        for (int i = 1; i < meshes.Length; i++)
        {
            combinedBounds.Encapsulate(meshes[i].bounds);
        }

        float modelHeight = combinedBounds.size.y;
        float centerY = combinedBounds.center.y;
        float radius = Mathf.Min(combinedBounds.extents.x, combinedBounds.extents.z);

        capsuleCollider.height = modelHeight;
        capsuleCollider.center = new Vector3(0, centerY - transform.position.y, 0);
        capsuleCollider.radius = radius;
    }

    /// <summary>
    /// E tuşuyla bakılan NPC'ye Greet() tetikler
    /// </summary>
    private void HandleNpcInteraction()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (firstPersonCamera == null) return;

            Ray ray = new Ray(firstPersonCamera.transform.position, firstPersonCamera.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, 3f, ~0, QueryTriggerInteraction.Ignore))
            {
                NpcScript npc = hit.collider.GetComponent<NpcScript>();
                if (npc != null)
                {
                    npc.Greet();
                }
            }
        }
    }
}
