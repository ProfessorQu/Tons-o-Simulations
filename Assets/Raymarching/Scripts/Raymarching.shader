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

            uniform float4 sphere;
            uniform float4 box;
            uniform float4 color;
            uniform float blendStrength;

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

            float DistanceField(float3 p) {
                //float plane1 = dot(p, normalize(float3(0, 1, 0)));
                
                float sphere1 = sdSphere(p - sphere.xyz, sphere.w);
                float box1 = sdBox(p - box.xyz, box.w);

                return OpUS(sphere1, box1, blendStrength);
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
                    float d = DistanceField(p);

                    if (d < 0.01){ // We have hit something
                        // Shading!
                        float3 n = GetNormal(p);
                        float light = dot(-_LightDir, n);

                        result = fixed4(color.rgb * light, 1);
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
