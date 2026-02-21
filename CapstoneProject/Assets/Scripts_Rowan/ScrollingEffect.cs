using UnityEngine;
using UnityEngine.UI;

public class ScrollingEffect : MonoBehaviour
{
    [SerializeField] private RawImage rawImage;
    [SerializeField] private float x, y;

    // Update is called once per frame
    void Update()
    {
        rawImage.uvRect = new Rect(rawImage.uvRect.position + new Vector2(x, y) * Time.deltaTime, rawImage.uvRect.size);
    }
}
