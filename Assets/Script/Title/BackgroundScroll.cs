using UnityEngine;
using UnityEngine.UI;

public class BackgroundScroll : MonoBehaviour
{
    public float scrollSpeedX = 0.1f;
    public float scrollSpeedY = 0.0f;

    private RawImage targetRawImage;
    private Rect currentUVRect;

    void Start()
    {
        targetRawImage = GetComponent<RawImage>();
        currentUVRect = targetRawImage.uvRect;
    }

    void Update()
    {
        currentUVRect.x += scrollSpeedX * Time.deltaTime;
        currentUVRect.y += scrollSpeedY * Time.deltaTime;

        targetRawImage.uvRect = currentUVRect;
    }
}
