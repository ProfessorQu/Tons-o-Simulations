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

            // Setup
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

            // Colors
            uniform float _ColorIntensity;
            uniform float _BlendStrength;

            // Repeating
            float3 _RepeatAxis;
            float3 _RepeatInterval;

            // Shapes
            float3 shapePositions[100];
            float3 shapeScales[100];

            int shapeTypes[100];
            int shapeOperations[100];

            float3 shapeColors[100];

            uniform int numShapes;


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

            float GetShapeDistance(float3 p,float3 s,int type) {
                float curShape = 1;

                if (type == 0) {
                    curShape = sdSphere(p, s);
                }
                else if (type == 1) {
                    curShape = sdBox(p, s);
                }
                else if (type == 2) {
                    curShape = sdTorus(p, s);
                }
                else if (type == 3) {
                    curShape = sdPlane(p, s);
                }

                return curShape;
            }

            float4 Combine(float4 d1, float4 d2, int op) {
                float4 combine;
                if (op == 0) {
                    combine = Unite(d1, d2);
                }
                else if (op == 1) {
                    combine = Subtract(d1, d2);
                }
                else if (op == 2) {
                    combine = Intersect(d1, d2);
                }
                else if (op == 3) {
                    combine = SmoothUnite(d1, d2, _BlendStrength);
                }
                else if (op == 4) {
                    combine = SmoothSubtract(d1, d2, _BlendStrength);
                }
                else if (op == 5) {
                    combine = SmoothIntersect(d1, d2, _BlendStrength);
                }
                
                return combine;
            }

            float4 DistanceField(float3 p) {
                float4 dist = 1;

                if (_RepeatAxis.x > 0) {
                    PositionMod(p.x, _RepeatInterval.x);
                }
                if (_RepeatAxis.y > 0) {
                    PositionMod(p.y, _RepeatInterval.y);
                }
                if (_RepeatAxis.z > 0) {
                    PositionMod(p.z, _RepeatInterval.z);
                }

                for (int i = 0; i < numShapes; i++) {
                    float3 pos = p - shapePositions[i];
                    float3 scale = shapeScales[i];

                    int type = shapeTypes[i];
                    int operation = shapeOperations[i];

                    float3 color = shapeColors[i];

                    float shapeDist = GetShapeDistance(pos, scale, type);
                    float4 shape = float4(color, shapeDist);

                    dist = Combine(shape, dist, operation);
                }

                return dist;
            }

            float3 GetNormal(float3 p) {
                const float2 offset = float2(0.001, 0);

                float3 n = float3(
                    DistanceField(p + offset.xyy).w - DistanceField(p - offset.xyy).w,
                    DistanceField(p + offset.yxy).w - DistanceField(p - offset.yxy).w,
                    DistanceField(p + offset.yyx).w - DistanceField(p - offset.yyx).w
                );

                return normalize(n);
            }

            float HardShadow(float3 ro, float3 rd, float minT, float maxT) {
                for (float t = minT; t < maxT;) {
                    float h = DistanceField(ro + rd * t).w;
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
                    float h = DistanceField(ro + rd * t).w;
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
                    ao += max(0.0, (dist - DistanceField(p + n * dist).w) / dist);
                }

                return 1 - ao * _AoIntensity;
            }

            float3 Shading(float3 p, float3 n, fixed3 c) {
                float3 result;
                float3 color = c.rgb * _ColorIntensity;
                // Directional Light
                float3 light = (_LightCol * dot(-_LightDir, n) * 0.5 + 0.5) * _LightIntensity;

                // Penumbra Shadows
                float shadow = SoftShadow(p, -_LightDir, _ShadowDistance.x, _ShadowDistance.y, _ShadowPenumbra) * 0.5 + 0.5;
                shadow = max(0.0, pow(shadow, _ShadowIntensity));

                // Ambient Occlusion
                float ao = AmbientOcclusion(p, n);

                result = color.rgb * color.rgb * light * shadow * ao;

                return result;
            }

            fixed4 Raymarching(float3 ro, float3 rd, float depth) {
                fixed4 result = 0;
                const int maxIterations = _MaxIterations;
                float t = 0; // distance travelled along the ray direction

                for (int i = 0; i < maxIterations; i++) {
                    if (t > _MaxDistance || t >= depth) {
                        // Environment
                        result = 0;
                        break;
                    }

                    float3 p = ro + rd * t;
                    // Check for hit
                    float4 d = DistanceField(p);

                    if (d.w < _Accuracy) {
                        // Shading!
                        float3 n = GetNormal(p);
                        float3 s = Shading(p, n, d.rgb);

                        result = fixed4(s, 1);
                        break;
                    }

                    t += d.w;
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
