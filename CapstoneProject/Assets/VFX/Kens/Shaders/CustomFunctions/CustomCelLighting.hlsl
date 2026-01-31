#ifndef CUSTOM_CEL_LIGHTING_INCLUDED
#define CUSTOM_CEL_LIGHTING_INCLUDED

// Helper function to calculate the cel intensity
float GetCelIntensity(float3 normal, float3 lightDir, float threshold, float smoothness)
{
    // Dot product between Normal and Light Direction (Lambert)
    float NdotL = dot(normal, lightDir);
    
    // Remap NdotL from [-1, 1] to [0, 1] for easier control (optional, but often helps)
    float litVal = NdotL * 0.5 + 0.5;
    
    // Apply the smoothstep to create the hard/soft edge
    // smoothstep(min, max, x) returns 0 if x < min, 1 if x > max, and interpolates in between
    return smoothstep(threshold - smoothness, threshold + smoothness, NdotL);
}

void CalculateCelShading_float(
    float3 Position,
    float3 Normal,
    float3 BaseColor,
    float3 ShadowColor,
    float Threshold,
    float EdgeSmoothness,
    out float3 FinalColor)
{
    // Initialize color
    float3 totalColor = float3(0, 0, 0);

#if defined(SHADERGRAPH_PREVIEW)
        // In the graph preview, lighting data isn't available. 
        // We return a dummy visualization to prevent errors.
        float3 lightDir = normalize(float3(0.5, 0.5, 0.25));
        float intensity = GetCelIntensity(Normal, lightDir, Threshold, EdgeSmoothness);
        totalColor = lerp(ShadowColor, BaseColor, intensity);
#else
        // -----------------------
        // 1. MAIN LIGHT CALCULATION
        // -----------------------
        
        // Calculate Shadow Coordinates
#if SHADOWS_SCREEN
            float4 clipPos = TransformWorldToHClip(Position);
            float4 shadowCoord = ComputeScreenPos(clipPos);
#else
    float4 shadowCoord = TransformWorldToShadowCoord(Position);
#endif

    Light mainLight = GetMainLight(shadowCoord);
        
        // Calculate intensity for main light
    float mainIntensity = GetCelIntensity(Normal, mainLight.direction, Threshold, EdgeSmoothness);
        
        // Combine: (CelRamp * ShadowAtten * DistanceAtten) * LightColor
    float mainAtten = mainLight.shadowAttenuation * mainLight.distanceAttenuation;
    float3 mainLightResult = lerp(ShadowColor, BaseColor, mainIntensity * mainAtten) * mainLight.color;
        
    totalColor += mainLightResult;

        // -----------------------
        // 2. ADDITIONAL LIGHTS LOOP (Point, Spot)
        // -----------------------
        
        // Get the number of extra lights affecting this pixel
    int pixelLightCount = GetAdditionalLightsCount();
        
    for (int i = 0; i < pixelLightCount; ++i)
    {
            // Get light data (Position, Color, Attenuation, Shadows)
        Light light = GetAdditionalLight(i, Position);
            
        float addIntensity = GetCelIntensity(Normal, light.direction, Threshold, EdgeSmoothness);
            
            // Calculate attenuation (falloff + shadows)
        float addAtten = light.distanceAttenuation * light.shadowAttenuation;
            
            // For additional lights, we usually add the light color purely additively
            // rather than lerping shadow color, to prevent washing out the scene.
            // However, we apply the Step function to the light visibility.
        float3 addLightResult = BaseColor * light.color * (addIntensity * addAtten);
            
        totalColor += addLightResult;
    }
#endif

    FinalColor = totalColor;
}

#endif