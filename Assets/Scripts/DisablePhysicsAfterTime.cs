using System.Collections;
using UnityEngine;

public class DisablePhysicsAfterTime : MonoBehaviour
{
    public void Setup(float delay)
    {
        StartCoroutine(DisablePhysics(delay));
    }

    private IEnumerator DisablePhysics(float delay)
    {
        yield return new WaitForSeconds(delay);

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true; // Stops physics simulation
        }

        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false; // Optional: Turn off collider too if you don't need it
        }
    }
}
