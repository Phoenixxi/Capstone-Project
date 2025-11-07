#ifndef ADDITIONAL_LIGHT_INCLUDED
#define ADDITIONAL_LIGHT_INCLUDED

void MainLight_float(float3 WorldPos, out float3 Direction, out float3 Color, out float Attenuation, out float ShadowAtten)
{
    #ifdef SHADERGRAPH_PREVIEW
        Direction = float3(0.5, 0.5, 0);
        Color = 1;
        Attenuation = 1;
        ShadowAtten = 1;
    #else

        float4 shadowCoord = TransformWorldToShadowCoord(WorldPos);
        
        Light maintLight = GetMainLight(shadowCoord);
        Direction = maintLight.direction;
        Color = maintLight.color;
        Attenuation = maintLight.distanceAttenuation;
        ShadowAtten = maintLight.shadowAttenuation;


    #endif
}

#endif // ADDITIONAL_LIGHT_INCLUDED