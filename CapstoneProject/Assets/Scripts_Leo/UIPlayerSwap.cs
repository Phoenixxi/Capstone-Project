using UnityEngine;
using lilGuysNamespace;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
using ElementType = lilGuysNamespace.EntityData.ElementType;
using UnityEngine.AI;
using System;

public class UIPlayerSwap : MonoBehaviour
{
    [SerializeField] private GameObject zoomIcon;
    [SerializeField] private GameObject boomIcon;
    [SerializeField] private GameObject gloomIcon;
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

    private Image zoomImage;
    private Image boomImage;
    private Image gloomImage;

    private bool zoomDead = false;
    private bool boomDead = false;
    private bool gloomDead = false;

    Color blackColor = new Color(0.25f, 0.25f, 0.25f);

    private int currentState = 1;

    void Start()
    {
        zoomImage = zoomIcon.GetComponent<Image>();
        boomImage = boomIcon.GetComponent<Image>();
        gloomImage = gloomIcon.GetComponent<Image>();
    }

    
    public void AllPlayersHealed()
    {
        zoomDead = false;
        zoomImage.color = Color.white;
        boomDead = false;
        boomImage.color = Color.white;
        gloomDead = false;
        gloomImage.color = Color.white;
    }

    public void zoomDied()
    {
        zoomDead = true;
        zoomImage.color = blackColor;
    }

    public void boomDied()
    {
        boomDead = true;
        boomImage.color = blackColor;
    }

    public void gloomDied()
    {
        gloomDead = true;
        gloomImage.color = blackColor;
    }

    public void swapImageLocation(int charNum)
    {
        
        switch (currentState)
        {
            case 1:
                if(charNum == -1 && !gloomDead)
                    stateThree();
                else if(charNum == 1 && !boomDead)
                    stateTwo();
                break;
            case 2:
                if(charNum == -1 && !zoomDead)
                    stateOne();
                else if(charNum == 1 && !gloomDead)
                    stateThree();
                break;
            case 3:
                if(charNum == -1 && !boomDead)
                    stateTwo();
                else if(charNum == 1 && !zoomDead)
                    stateOne();
                break;
        }
    }

    public void stateOne()
    {
        currentState = 1;
        gloomIcon.transform.position = topTransform.transform.position;
        zoomIcon.transform.position = middleTransform.transform.position;
        boomIcon.transform.position = bottomTransform.transform.position;
    }

    public void stateTwo()
    {
        currentState = 2;
        zoomIcon.transform.position = topTransform.transform.position;
        boomIcon.transform.position = middleTransform.transform.position;
        gloomIcon.transform.position = bottomTransform.transform.position;
    }

    public void stateThree()
    {
        currentState = 3;
        boomIcon.transform.position = topTransform.transform.position;
        gloomIcon.transform.position = middleTransform.transform.position;
        zoomIcon.transform.position =  bottomTransform.transform.position;
    }
}
