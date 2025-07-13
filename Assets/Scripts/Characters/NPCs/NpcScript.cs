using UnityEngine;

[RequireComponent(typeof(Animator))]
public class NpcScript : MonoBehaviour
{
    [Header("Gezinme Ayarları")]
    public float walkSpeed = 1f;
    public float idleTime = 3f;
    public float walkTime = 4f;

    [Header("Tekne Referansı")]
    public Transform boatTransform;

    private Animator animator;
    private float timer;
    private bool isWalking = false;
    private Vector3 walkDirection;
    private Vector3 offsetFromBoat;

    private void Start()
    {
        animator = GetComponent<Animator>();

        if (boatTransform != null)
        {
            // NPC'yi zeminle hizala (tekneye göre)
            if (Physics.Raycast(transform.position + Vector3.up * 1f, Vector3.down, out RaycastHit hit, 5f))
            {
                transform.position = hit.point;
            }

            // Offset'i HİZALANMIŞ pozisyona göre al
            offsetFromBoat = transform.position - boatTransform.position;
        }
        else
        {
            Debug.LogWarning("⚠ boatTransform atanmadı!");
        }

        timer = idleTime;
        animator.SetBool("isWalking", false);
    }

    private void Update()
    {
        if (boatTransform == null) return;

        // NPC pozisyonunu tekneye göre sabitle
        transform.position = boatTransform.position + offsetFromBoat;

        timer -= Time.deltaTime;

        if (isWalking)
        {
            offsetFromBoat += walkDirection * walkSpeed * Time.deltaTime;

            if (timer <= 0f)
            {
                isWalking = false;
                animator.SetBool("isWalking", false);
                timer = idleTime;
            }
        }
        else
        {
            if (timer <= 0f)
            {
                isWalking = true;
                animator.SetBool("isWalking", true);
                timer = walkTime;

                walkDirection = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;
                transform.rotation = Quaternion.LookRotation(walkDirection);
            }
        }
    }

    public void Greet()
    {
        animator?.SetTrigger("Talk");
    }
}
