using UnityEngine;
using UnityEngine.UIElements;

public class BlendShapeLoop : MonoBehaviour
{
    SkinnedMeshRenderer skinnedMeshRenderer;
    Mesh skinnedMesh;
    int blendShapeCount;

    int playIndex = 0;

    const float frameRate = 1f / 24f;
    float timer;

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
        timer += Time.deltaTime;
        while (timer >= frameRate)
        {
            timer -= frameRate;
            Frame();
        }
    }

    void Frame()
    {
        if (playIndex > 0) skinnedMeshRenderer.SetBlendShapeWeight(playIndex - 1, 0f);
        if (playIndex == 0) skinnedMeshRenderer.SetBlendShapeWeight(blendShapeCount - 1, 0f);
        skinnedMeshRenderer.SetBlendShapeWeight(playIndex, 100f);
        playIndex++;
        if (playIndex > blendShapeCount - 1) playIndex = 0;
    }
}
