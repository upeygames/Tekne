using UnityEngine;

public class NpcScript : MonoBehaviour
{
    private Animator animator;
    private Transform player;

    [Header("Gezinme Ayarları")]
    [SerializeField] private float walkSpeed = 1f;
    [SerializeField] private float idleTime = 3f;
    [SerializeField] private float walkTime = 4f;

    private float stateTimer = 0f;
    private bool isWalking = false;
    private Vector3 walkDirection;

    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        stateTimer = idleTime;
        ChooseNewDirection();
    }

    void Update()
    {
        HandleWalkingCycle();
    }

    private void HandleWalkingCycle()
    {
        stateTimer -= Time.deltaTime;

        if (isWalking)
        {
            // Yönü sabit bir yöne doğru yürüt
            transform.Translate(walkDirection * walkSpeed * Time.deltaTime, Space.World);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(walkDirection), 5f * Time.deltaTime);

            if (stateTimer <= 0f)
                StopWalking();
        }
        else
        {
            if (stateTimer <= 0f)
                StartWalking();
        }
    }

    private void StartWalking()
    {
        isWalking = true;
        stateTimer = walkTime;
        animator.SetBool("isWalking", true);
        ChooseNewDirection();
    }

    private void StopWalking()
    {
        isWalking = false;
        stateTimer = idleTime;
        animator.SetBool("isWalking", false);
    }

    private void ChooseNewDirection()
    {
        Vector2 random2D = Random.insideUnitCircle.normalized;
        walkDirection = new Vector3(random2D.x, 0f, random2D.y);
    }

    // Bu zaten vardı, değiştirmiyoruz
    public void Greet()
    {
        if (player != null)
        {
            Vector3 lookPos = player.position;
            lookPos.y = transform.position.y;
            transform.LookAt(lookPos);
        }

        animator.SetTrigger("Greet");
        Debug.Log("👋 NPC: Selam animasyonu tetiklendi.");
    }
}
