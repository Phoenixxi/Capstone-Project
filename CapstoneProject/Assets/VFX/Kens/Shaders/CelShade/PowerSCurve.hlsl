#ifndef POWER_S_CURVE_INCLUDED
#define POWER_S_CURVE_INCLUDED

void PowerSCurve_float(float In, float Contrast, out float Out)
{
#if defined(SHADERGRAPH_PREVIEW)
   Out = 0.5f;
#else
    float r = clamp(Contrast, 0.0f, 0.99f);
    float p = 1.0f / (1.0f - r);
    float x = In;
    float mask = step(0.5f, x);
    float a = pow(2.0f * x, p);
    float b = pow(2.0f * (1.0f - x), p);
    float low = 0.5f * a;
    float high = 1.0f - 0.5f * b;
    Out = lerp(low, high, mask);
#endif
}

#endif