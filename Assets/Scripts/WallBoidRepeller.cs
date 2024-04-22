using UnityEngine;

public class WallBoidRepeller : MonoBehaviour
{
    private Vector2 _size;

    private void Start()
    {
        _size = new Vector2(transform.localScale.x, transform.localScale.y) * 4f;
    }

    private void FixedUpdate()
    {
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll((Vector2) transform.position,
            _size, transform.eulerAngles.z);
        
        Debug.Log("Colliders hit: " + hitColliders.Length);

        foreach (var collider in hitColliders)
        {
            if (!collider.TryGetComponent(out SheepController sheep)) continue;

            sheep.TendToPlaceScaled(transform.position);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, _size);
    }
}
