// StylizedPostFX.hlsl

// 1. Calculate Luminance (Standard helper)
void GetLuminance_float(float3 Color, out float Luminance)
{
    Luminance = dot(Color, float3(0.2126, 0.7152, 0.0722));
}

void GetBrightness_float(float3 Color, out float Brightness)
{
    Brightness = max(Color.r, max(Color.g, Color.b));
}

// Sample SSAO
void SampleSSAO_float(float2 UV, out float Occlusion)
{
    float2 uv = UnityStereoTransformScreenSpaceTex(UV);
    Occlusion = SAMPLE_TEXTURE2D_X(_ScreenSpaceOcclusionTexture, sampler_ScreenSpaceOcclusionTexture, uv).r;
}

void GetOutline_float(float2 UV, float2 TexelSize, float2 Thickness, float DepthThreshold, float NormalThreshold, 
float MinDist, float MaxDist, out float Out)
{
    // Inputs: UV, TexelSize, Thickness (float2), DepthThreshold, NormalThreshold, MinDist, MaxDist
// Output: Out (float)

#if defined(SHADERGRAPH_PREVIEW)
    Out = 0;
#else
    // 1. Scene Depth Sampling
    // We intentionally DO NOT scale UVs by distance here yet.
    // Changing sampling coordinates dynamically on a Point-Filtered texture causes "Swimming" pixels.
    float2 uv0 = UV + float2(-TexelSize.x, -TexelSize.y) * Thickness;
    float2 uv1 = UV + float2(TexelSize.x, TexelSize.y) * Thickness;
    float2 uv2 = UV + float2(TexelSize.x, -TexelSize.y) * Thickness;
    float2 uv3 = UV + float2(-TexelSize.x, TexelSize.y) * Thickness;

    // 2. Fetch & Linearize Depth
    float d0 = SHADERGRAPH_SAMPLE_SCENE_DEPTH(uv0);
    float d1 = SHADERGRAPH_SAMPLE_SCENE_DEPTH(uv1);
    float d2 = SHADERGRAPH_SAMPLE_SCENE_DEPTH(uv2);
    float d3 = SHADERGRAPH_SAMPLE_SCENE_DEPTH(uv3);

    float ld0 = LinearEyeDepth(d0, _ZBufferParams);
    float ld1 = LinearEyeDepth(d1, _ZBufferParams);
    float ld2 = LinearEyeDepth(d2, _ZBufferParams);
    float ld3 = LinearEyeDepth(d3, _ZBufferParams);

    // 3. Roberts Cross (Depth)
    // We calculate the difference
    float depthDiff0 = ld1 - ld0;
    float depthDiff1 = ld3 - ld2;
    float edgeDepth = sqrt(pow(depthDiff0, 2) + pow(depthDiff1, 2));

    // 4. SOFT THRESHOLD (Solves "Fireflies")
    // Instead of (edge > Threshold), we use smoothstep.
    // This creates a soft transition. If edge is slightly below threshold, it just fades out
    // rather than vanishing instantly.
    // We multiply threshold by depth to keep line width consistent across distances.
    float depthThresholdDynamic = DepthThreshold * ld0;
    float depthResult = smoothstep(depthThresholdDynamic, depthThresholdDynamic * 1.5, edgeDepth);

    // 5. Normal Sampling
    float3 n0 = SHADERGRAPH_SAMPLE_SCENE_NORMAL(uv0);
    float3 n1 = SHADERGRAPH_SAMPLE_SCENE_NORMAL(uv1);
    float3 n2 = SHADERGRAPH_SAMPLE_SCENE_NORMAL(uv2);
    float3 n3 = SHADERGRAPH_SAMPLE_SCENE_NORMAL(uv3);

    // 6. Roberts Cross (Normal)
    float3 normalDiff0 = n1 - n0;
    float3 normalDiff1 = n3 - n2;
    float edgeNormal = sqrt(dot(normalDiff0, normalDiff0) + dot(normalDiff1, normalDiff1));
    
    // Soft Threshold for Normals
    float normalResult = smoothstep(NormalThreshold, NormalThreshold + 0.1, edgeNormal);

    // 7. Distance Fade Logic (Stable)
    // Instead of shrinking the line (which causes jitter), we just fade the Opacity.
    float closestDepth = min(min(ld0, ld1), min(ld2, ld3));
    float distanceFade = 1 - saturate((closestDepth - MinDist) / (MaxDist - MinDist));

    // 8. Combine
    // We take the max of Depth or Normal, then multiply by the Fade.
    Out = max(depthResult, normalResult) * distanceFade;
#endif
}