using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public class BlendShapeIndicators : MonoBehaviour
{
    SkinnedMeshRenderer skinnedMeshRenderer;
    Mesh skinnedMesh;
    int blendShapeCount;

    [SerializeField] private float timeTaken;
    
    private float currentBlendShapeValue = 0f;

    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        
        skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        skinnedMesh = GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh;
        blendShapeCount = skinnedMesh.blendShapeCount;

        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (currentBlendShapeValue > 0f)
        {
            float blendShapeChange = (Time.deltaTime / timeTaken) * 100f;

            currentBlendShapeValue -= blendShapeChange;

            skinnedMeshRenderer.SetBlendShapeWeight(0, currentBlendShapeValue);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    void OnEnable()
    {
        StartIndicator();
    }

    public void SetTimeTaken(float timeTaken)
    {
        this.timeTaken = timeTaken;
    }

    public void StartIndicator()
    {
        currentBlendShapeValue = 100f;
    }
}
