using UnityEngine;

public class NpcScript : MonoBehaviour
{
    private Animator animator;
    private Transform player;

    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    public void Greet()
    {
        if (player != null)
        {
            // Oyuncuya dön
            Vector3 lookPos = player.position;
            lookPos.y = transform.position.y;
            transform.LookAt(lookPos);
        }

        animator.SetTrigger("Greet");
        Debug.Log("👋 NPC animasyonu tetiklendi.");
    }
}
