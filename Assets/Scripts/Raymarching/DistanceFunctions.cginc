float sdSphere(float3 p, float s)
{
    return length(p) - s;
}

float sdBox(float3 p, float3 b)
{
	float3 d = abs(p) - b;
	return min(max(d.x, max(d.y, d.z)), 0.0) +
		length(max(d, 0.0));
}

float sdTorus(float3 p, float2 t)
{
    float2 q = float2(length(p.xz) - t.x, p.y);
    return length(q) - t.y;
}

float sdPlane(float3 p, float3 n)
{
    return dot(p, n.xyz) * 1;
}

// BOOLEAN OPERATORS //

// Union
float4 Unite(float4 d1, float4 d2)
{
    return (d1.w < d2.w) ? d1 : d2;
}

// Subtraction
float4 Subtract(float4 d1, float4 d2)
{
	return (-d1.w > d2.w) ? -d1 : d2;
}

// Intersection
float4 Intersect(float4 d1, float4 d2)
{
	return (d1.w > d2.w) ? d1 : d2;
}


// SMOOTH BOOLEAN OPERATORS //

// Union
float4 SmoothUnite(float4 d1, float4 d2, float k)
{
    float h = clamp(0.5 + 0.5 * (d2.w - d1.w) / k, 0.0, 1.0);
    float3 color = lerp(d2.rgb, d1.rgb, h);
    float dist = lerp(d2.w, d1.w, h) - k * h * (1.0 - h);
    
    return float4(color, dist);
}

// Subtraction
float4 SmoothSubtract(float4 d1, float4 d2, float k)
{
    float h = clamp(0.5 - 0.5 * (d2.w + d1.w) / k, 0.0, 1.0);
    float3 color = lerp(d2.rgb, d1.rgb, h);
    float dist = lerp(d2.w, -d1.w, h) + k * h * (1.0 - h);
    
    return float4(color, dist);
}

// Intersection
float4 SmoothIntersect(float4 d1, float4 d2, float k)
{
    float h = clamp(0.5 - 0.5 * (d2 - d1) / k, 0.0, 1.0);
    float3 color = lerp(d2.rgb, d1.rgb, h);
    float dist = lerp(d2.w, d1.w, h) + k * h * (1.0 - h);
    
    return float4(color, dist);
}

// Mod Position Axis
float PositionMod (inout float p, float size)
{
	float halfsize = size * 0.5;
	float c = floor((p+halfsize)/size);
	p = fmod(p+halfsize,size)-halfsize;
	p = fmod(-p+halfsize,size)-halfsize;
	return c;
}