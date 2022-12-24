using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField] private DialogueObject interactionDialogue;
    [SerializeField] private float indicatorHeight = 1.0f;

    private Animator animator;

    public float radius = 2f;

    private Quaternion targetQuaternion;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    public DialogueObject InteractionDialogue => interactionDialogue;
    public float IndicatorHeight => indicatorHeight;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        targetQuaternion = transform.rotation;
    }

    private void Update()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, targetQuaternion, Time.deltaTime * 5.0f);
    }

    public void OnStartTalking(Vector3 playerPosition)
    {
        if (animator)
        {
            animator.SetBool("isTalking", true);
        }

        Vector3 interactDir = (playerPosition - transform.position).normalized;
        targetQuaternion = Quaternion.LookRotation(new Vector3(interactDir.x, 0f, interactDir.z));
    }

    public void OnStopTalking()
    {
        if (animator)
        {
            animator.SetBool("isTalking", false);
        }
    }
}
