using System.Collections;
using UnityEngine;

public class DeactivateAfterDelay : MonoBehaviour
{
    public GameObject targetObject; // 👈 Assign this GameObject in the Inspector
    public float delay = 2.5f; // Delay before deactivation (in seconds)

    private void Start()
    {
        StartCoroutine(DeactivateAfterTime());
    }

    private IEnumerator DeactivateAfterTime()
    {
        yield return new WaitForSeconds(delay);

        if (targetObject != null)
        {
            targetObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("⚠️ No target object assigned to deactivate.");
        }
    }
}
