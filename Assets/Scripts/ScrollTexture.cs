using UnityEngine;

public class ScrollTexture : MonoBehaviour
{
    public Vector2 scrollSpeed;
    private Renderer myRenderer;

    void Start()
    {
        myRenderer = GetComponent<Renderer>();
    }

    void FixedUpdate()
    {
        if (!TaskLogic.isBalloonStopped)
        {
            Vector2 offset = Time.time * scrollSpeed;
            myRenderer.material.SetTextureOffset("_BaseMap", offset);
        }
    }
}
