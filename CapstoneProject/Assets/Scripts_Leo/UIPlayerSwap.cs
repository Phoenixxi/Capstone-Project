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
    [SerializeField] private Transform topTransform;
    [SerializeField] private Transform middleTransform;
    [SerializeField] private Transform bottomTransform;
    
    // [Header("Scales")]
    // [SerializeField] private Transform topScale;
    // [SerializeField] private Transform middleScale;
    // [SerializeField] private Transform bottomScale;


    private int currentState = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // topTransform = gloomImage.transform;
        // middleTransform = zoomImage.transform;
        // bottomTransform = boomImage.transform;

    }

    public void swapImageLocation(int charNum)
    {
        
        switch (currentState)
        {
            case 1:
                Debug.Log("inside case 1");
                if(charNum == -1)
                    stateThree();
                else if(charNum == 1)
                    stateTwo();
                break;
            case 2:
                Debug.Log("inside case 2");
                if(charNum == -1)
                    stateOne();
                else if(charNum == 1)
                    stateThree();
                break;
            case 3:
                Debug.Log("inside case 3");
                if(charNum == -1)
                    stateTwo();
                else if(charNum == 1)
                    stateOne();
                break;
        }
    }


    private void stateOne()
    {
        Debug.Log("in state 1");
        currentState = 1;
        gloomImage.transform.position = topTransform.transform.position;
        zoomImage.transform.position = middleTransform.transform.position;
        boomImage.transform.position = bottomTransform.transform.position;
    }

    private void stateTwo()
    {
        Debug.Log("in state 2");
        currentState = 2;
        zoomImage.transform.position = topTransform.transform.position;
        boomImage.transform.position = middleTransform.transform.position;
        gloomImage.transform.position = bottomTransform.transform.position;
    }

    private void stateThree()
    {
        Debug.Log("in state 3");
        currentState = 3;
        boomImage.transform.position = topTransform.transform.position;
        gloomImage.transform.position = middleTransform.transform.position;
        zoomImage.transform.position =  bottomTransform.transform.position;
    }
}
