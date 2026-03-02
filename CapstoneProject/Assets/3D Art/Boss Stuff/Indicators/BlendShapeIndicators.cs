using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public class BlendShapeIndicators : MonoBehaviour
{
    SkinnedMeshRenderer skinnedMeshRenderer;
    Mesh skinnedMesh;
    int blendShapeCount;

    [SerializeField] private float timeTaken;
    
    private float currentBlendShapeValue = 100f;

    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        skinnedMesh = GetComponent<SkinnedMeshRenderer>().sharedMesh;
        blendShapeCount = skinnedMesh.blendShapeCount;
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
    }
}
