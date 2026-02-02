#ifndef ALL_LIGHTS_INCLUDED
#define ALL_LIGHTS_INCLUDED

void AllLightsDiffuse_float(float3 Position, float3 Normal, out float3 Color)
{

#if defined(SHADERGRAPH_PREVIEW)
   Color = float3(0.5f, 0.5f, 0.5f);
#else
    float3 normalWS = normalize(Normal);
    float3 totalDiffuse = 0;

   // Main Light
#if SHADOWS_SCREEN
   float4 clipPos = TransformWorldToHClip(Position);
   float4 shadowCoord = ComputeScreenPos(clipPos);
#else
    float4 shadowCoord = TransformWorldToShadowCoord(Position);
#endif
    Light mainLight = GetMainLight(shadowCoord);
    float mainAtten = mainLight.distanceAttenuation * mainLight.shadowAttenuation;
    float mainDiffuse = max(0.0f, dot(normalWS, mainLight.direction));
    totalDiffuse += mainLight.color * mainAtten * mainDiffuse;

   // Additional Lights
    half4 shadowMask = half4(1, 1, 1, 1);
    uint meshRenderingLayers = GetMeshRenderingLayer();
    InputData inputData = (InputData) 0;
    float4 screenPos = ComputeScreenPos(TransformWorldToHClip(Position));
    inputData.normalizedScreenSpaceUV = screenPos.xy / screenPos.w;
    inputData.positionWS = Position;

#if USE_CLUSTER_LIGHT_LOOP
   for (uint lightIndex = 0; lightIndex < min(URP_FP_DIRECTIONAL_LIGHTS_COUNT, MAX_VISIBLE_LIGHTS); lightIndex++) {
      CLUSTER_LIGHT_LOOP_SUBTRACTIVE_LIGHT_CHECK
      Light light = GetAdditionalLight(lightIndex, Position, shadowMask);
#ifdef _LIGHT_LAYERS
      if (IsMatchingLightLayer(light.layerMask, meshRenderingLayers))
#endif
      {
         float atten = light.distanceAttenuation * light.shadowAttenuation;
         float diffuse = max(0.0f, dot(normalWS, light.direction));
         totalDiffuse += light.color * atten * diffuse;
      }
   }
#endif

    uint pixelLightCount = GetAdditionalLightsCount();
    LIGHT_LOOP_BEGIN(pixelLightCount)

    Light light = GetAdditionalLight(lightIndex, Position, shadowMask);
#ifdef _LIGHT_LAYERS
      if (IsMatchingLightLayer(light.layerMask, meshRenderingLayers))
#endif
      {
        float atten = light.distanceAttenuation * light.shadowAttenuation;
        float diffuse = max(0.0f, dot(normalWS, light.direction));
        totalDiffuse += light.color * atten * diffuse;
    }
    LIGHT_LOOP_END

   Color = totalDiffuse;
   
#endif
}

#endif