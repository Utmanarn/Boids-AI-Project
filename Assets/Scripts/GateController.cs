using UnityEngine;

public class GateController : MonoBehaviour
{
    private new SpriteRenderer renderer;

    private void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();
    }

    public void OpenGate()
    {
        renderer.enabled = false;
    }

    public void CloseGate()
    {
        renderer.enabled = true;
    }
}
