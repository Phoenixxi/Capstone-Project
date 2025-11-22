using UnityEngine;

public class UIPlayerSwap : MonoBehaviour
{
    [SerializeField] private GameObject zoomImage;
    [SerializeField] private GameObject boomImage;
    [SerializeField] private GameObject gloomImage;
    [SerializeField] private GameObject zoomHealthRing;
    [SerializeField] private GameObject boomHealthRing;
    [SerializeField] private GameObject gloomHealthRing;

    [Header("Locations")]
    [SerializeField] private Vector3 middleTransform;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        middleTransform = zoomImage.transform.position;
    }

    public void swapLoc()
    {
        boomImage.transform.position = middleTransform;
    }
}
