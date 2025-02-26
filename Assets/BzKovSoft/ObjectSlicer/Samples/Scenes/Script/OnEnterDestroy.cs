using UnityEngine;

public class OnEnterDestroy : MonoBehaviour
{
    [Header("Tag Filtering (Optional)")]
    [Tooltip("If set, only objects with this tag will be destroyed.")]
    public string targetTag;

    private void OnCollisionEnter(Collision collision)
    {
        TryDestroy(collision.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        TryDestroy(other.gameObject);
    }

    private void TryDestroy(GameObject other)
    {
        if (!string.IsNullOrEmpty(targetTag) && !other.CompareTag(targetTag))
        {
            return; // Skip objects that don't match the tag
        }

        Destroy(other);
    }
}
