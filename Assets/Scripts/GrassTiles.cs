using UnityEngine;

public class GrassTiles : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    public int sheepCount = 0;

    [SerializeField] private Color grassColor;
    [SerializeField] private Color dirtColor;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetTileToGrass()
    {
        spriteRenderer.color = grassColor;
    }

    public void SetTileToDirt()
    {
        spriteRenderer.color = dirtColor;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.TryGetComponent(out SheepController sheepController))
            return;

        sheepCount++;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.gameObject.TryGetComponent(out SheepController sheepController))
            return;

        sheepCount--;
    }
}
