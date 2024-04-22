using UnityEngine;

public class RepelBoids : MonoBehaviour
{
    [SerializeField] private float repelRange = 1.5f;
    
    private void FixedUpdate()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, repelRange); // BUG: Figure out why it is not detecting collisions.

        foreach (var collider in hitColliders)
        {
            if (!collider.TryGetComponent(out SheepController sheep)) continue;

            sheep.TendToPlaceScaled(transform.position);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, repelRange);
    }
}
