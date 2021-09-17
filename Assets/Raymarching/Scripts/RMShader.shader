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

            uniform float _MaxDistance;
            uniform float3 _LightDir;

            uniform int shape;

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

            float sinDist(float3 p) {
                return sin(p.x) * sin(p.y) * sin(p.z);
            }

            float4 DistanceField(float3 p) {
                if (shape == 0) {
                    return float4(1, 1, 1, sdSphere(p, 1));
                }
                else if (shape == 1) {
                    return float4(0, 0, 1, sdBox(p, 1));
                }
                else if (shape == 2) {
                    pMod(p.x, 6);
                    pMod(p.y, 6);
                    pMod(p.z, 6);

                    return float4(1, 0, 0, sdSphere(p, 1));
                }
                else if (shape == 3) {
                    pMod(p.x, 6);
                    pMod(p.y, 6);
                    pMod(p.z, 6);

                    return float4(0, 1, 0, sdBox(p, 1));
                }
                else if (shape == 4) {
                    pMod(p.x, 4);
                    pMod(p.y, 4);
                    pMod(p.z, 4);

                    float sphere = sdSphere(p, 2.5);
                    float box = sdBox(p, 2);

                    return float4(1, 1, 1, OpS(sphere, box));
                }
                else if (shape == 5) {
                    return float4(0, 1, 1, sin(p.x) + sin(p.y) + sin(p.z));
                }
                else {
                    return 1;
                }
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

            fixed4 raymarching(float3 ro, float3 rd, float depth) {
                fixed4 result = fixed4(1, 1, 1, 1);
                const int maxIteration = 1024;
                float t = 0; // distance travelled along the ray direction

                for (int i = 0; i < maxIteration; i++) {
                    if (t > _MaxDistance || t >= depth) {
                        // Environment
                        result = fixed4(rd, 0);
                        break;
                    }

                    float3 p = ro + rd * t;
                    // Check for hit
                    float4 info = DistanceField(p);

                    float3 color = info.rgb;
                    float d = info.w;

                    if (d < 0.01){ // We have hit something
                        // Shading!
                        float3 n = GetNormal(p);
                        float light = dot(-_LightDir, n);

                        result = fixed4(color * light, 1);
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

                fixed4 result = raymarching(rayOrigin, rayDirection, depth);

                return fixed4(col * (1.0 - result.w) + result.xyz * result.w, 1.0);
            }
            ENDCG
        }
    }
}
