Shader "Raymarching/Raymarching"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "UnityCG.cginc"
            #include "DistanceFunctions.cginc"

            sampler2D _MainTex;
            uniform sampler2D _CameraDepthTexture;
            uniform float4x4 _CamFrustum, _CamToWorld;

            uniform int _MaxIterations;
            uniform float _Accuracy;
            uniform float _MaxDistance;

            // Lighting
            uniform float3 _LightDir, _LightCol;
            uniform float _LightIntensity;

            // Shading
            uniform float2 _ShadowDistance;
            uniform float _ShadowIntensity;
            uniform float _ShadowPenumbra;

            // Ambient Occlusion
            uniform float _AoStepsize;
            uniform int _AoIterations;
            uniform float _AoIntensity;

            uniform float3 color;

            /*
            uniform float4 sphere1;
            uniform float4 sphere2;
            uniform float4 box1;
            uniform float box1Round, sphereBoxSmooth, sphereIntersectSmooth;
            */

            uniform int numShapes;

            uniform float3 shapePositions[100];
            uniform int shapeTypes[100];
            uniform int shapeOperations[100];

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;

                float3 ray : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                half index = v.vertex.z;
                v.vertex.z = 0;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                o.ray = _CamFrustum[(int)index].xyz;
                o.ray /= abs(o.ray.z);
                o.ray = mul(_CamToWorld, o.ray);

                return o;
            }

            float GetShapeDistance(float3 p, int type) {
                float curShape = 1;

                if (type == 0) {
                    curShape = sdSphere(p, 1);
                }
                else if (type == 1) {
                    curShape = sdBox(p, 1);
                }
                else if (type == 2) {
                    curShape = sdPlane(p, float4(0, 1, 0, 1));
                }

                return curShape;
            }

            float Combine(float d1, float d2, int op) {
                if (op == 0) {
                    return OpU(d1, d2);
                }
                else if (op == 1) {
                    return OpS(d1, d2);
                }
                else if (op == 2) {
                    return OpI(d1, d2);
                }
                
                return 1;
            }

            float DistanceField(float3 p) {
                float dist = 1;

                for (int i = 0; i < numShapes; i++) {
                    float3 pos = p - shapePositions[i];
                    float d = GetShapeDistance(pos, shapeTypes[i]);

                    dist = Combine(d, dist, shapeOperations[i]);
                }

                return dist;

                /*  
                 *  float dist = 0
                 *  for object in scene objects:
                 *      if object.operation == Union:
                 *          dist = Union(dist, object.distance)
                 *      else if object.operation == Intersection:
                 *          dist = Intersection(dist, object.distance)
                 *      else if object.operation == Subtraction:
                 *          dist = Subtraction(dist, object.distance)
                 *
                 * return dist
                */
            }

            float3 GetNormal(float3 p) {
                const float2 offset = float2(0.001, 0);

                float3 n = float3(
                    DistanceField(p + offset.xyy) - DistanceField(p - offset.xyy),
                    DistanceField(p + offset.yxy) - DistanceField(p - offset.yxy),
                    DistanceField(p + offset.yyx) - DistanceField(p - offset.yyx)
                );

                return normalize(n);
            }

            float HardShadow(float3 ro, float3 rd, float minT, float maxT) {
                for (float t = minT; t < maxT;) {
                    float h = DistanceField(ro + rd * t);
                    if (h < 0.001) {
                        return 0.0;
                    }

                    t += h;
                }

                return 1.0;
            }

            float SoftShadow(float3 ro, float3 rd, float minT, float maxT, float k) {
                float result = 1.0;

                for (float t = minT; t < maxT;) {
                    float h = DistanceField(ro + rd * t);
                    if (h < 0.001) {
                        return 0.0;
                    }

                    result = min(result, k * h/t);

                    t += h;
                }

                return result;
            }

            float AmbientOcclusion(float3 p, float3 n) {
                float step = _AoStepsize;
                float ao = 0.0;
                float dist;

                for (int i = 1; i <= _AoIterations; i++) {
                    dist = step * i;
                    ao += max(0.0, (dist - DistanceField(p + n * dist))) / dist;
                }

                return 1 - ao * _AoIntensity;
            }

            float3 Shading(float3 p, float3 n) {
                float3 result;
                // Directional Light
                float3 light = (_LightCol * dot(-_LightDir, n) * 0.5 + 0.5) * _LightIntensity;

                // Penumbra Shadows
                float shadow = SoftShadow(p, -_LightDir, _ShadowDistance.x, _ShadowDistance.y, _ShadowPenumbra) * 0.5 + 0.5;
                shadow = max(0.0, pow(shadow, _ShadowIntensity));

                // Ambient Occlusion
                float ao = AmbientOcclusion(p, n);

                result = color.rgb * light * shadow * ao;

                return result;
            }

            fixed4 Raymarching(float3 ro, float3 rd, float depth) {
                fixed4 result = fixed4(1, 1, 1, 1);
                const int maxIterations = _MaxIterations;
                float t = 0; // distance travelled along the ray direction

                for (int i = 0; i < maxIterations; i++) {
                    if (t > _MaxDistance || t >= depth) {
                        // Environment
                        result = fixed4(rd, 0);
                        break;
                    }

                    float3 p = ro + rd * t;
                    // Check for hit
                    float d = DistanceField(p);

                    if (d < _Accuracy){ // We have hit something
                        // Shading!
                        float3 n = GetNormal(p);
                        float3 s = Shading(p, n);

                        result = fixed4(s, 1);
                        break;
                    }

                    t += d;
                }

                return result;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float depth = LinearEyeDepth(tex2D(_CameraDepthTexture, i.uv).r);
                depth *= length(i.ray);

                fixed3 col = tex2D(_MainTex, i.uv);
                
                float3 rayDirection = normalize(i.ray.xyz);
                float3 rayOrigin = _WorldSpaceCameraPos;

                fixed4 result = Raymarching(rayOrigin, rayDirection, depth);

                return fixed4(col * (1.0 - result.w) + result.xyz * result.w, 1.0);
            }
            ENDCG
        }
    }
}
