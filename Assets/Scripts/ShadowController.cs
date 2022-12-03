using UnityEngine;

public class ShadowController : MonoBehaviour
{
    public Transform playerTransform;
    public LayerMask groundMask;
    public float maxSize = 3.4f;
    public float maxHeight = 10.0f;

    void LateUpdate()
    {
        bool groundCheckResult = Physics.SphereCast(playerTransform.position, 0.1f, Vector3.down, out RaycastHit groundHit, maxHeight, groundMask);

        if (groundCheckResult)
        {
            transform.position = groundHit.point + Vector3.up * 0.01f;
            float newScale = Mathf.Max(0, maxSize * (1.0f - (playerTransform.position.y - groundHit.point.y) / maxHeight));
            transform.localScale = new Vector3(newScale, newScale, newScale);
        }
        else
        {
            transform.localScale = new Vector3(0, 0, 0);
        }
    }
}
