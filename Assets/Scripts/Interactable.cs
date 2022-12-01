using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField] private DialogueObject interactionDialogue;
    [SerializeField] private float indicatorHeight = 1.0f;

    public float radius = 2f;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    public DialogueObject InteractionDialogue => interactionDialogue;
    public float IndicatorHeight => indicatorHeight;
}
