using UnityEngine;

public class RepelBoids : MonoBehaviour
{
    [SerializeField] private float repelRange = 1.5f;
    
    private void FixedUpdate()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, repelRange); // BUG: Figure out why it is not detecting collisions.
        
        Debug.Log(hitColliders.Length);

        foreach (var collider in hitColliders)
        {
            if (!collider.TryGetComponent(out SheepController sheep)) return;

            sheep.TendToPlace(transform.position);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, repelRange);
    }
}
