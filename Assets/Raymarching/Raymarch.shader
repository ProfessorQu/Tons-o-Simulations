// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/Raymarch"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Centre ("Center", Vector) = (0, 0, 0, 0)
        _Radius ("Radius", Float) = 1
        _WorldSpace ("World Space", Range(0, 1)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            #define MAX_STEPS 64
            #define MIN_DIST .1

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;

                float3 worldPos : TEXCOORD1;
                float3 objectPos : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float3 _Centre;
            float _Radius;

            float _WorldSpace;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.objectPos = v.vertex;
                return o;
            }

            float signedSphereDistance(float3 p) {
                return distance(p, _Centre) - _Radius;
            }

            float raymarch(float3 position, float3 direction)
            {
                for (int i = 0; i < MAX_STEPS; i++) {
                    float distance = signedSphereDistance(position);
                    if (distance < MIN_DIST)
                        //return (i + 1) / (float) MAX_STEPS;
                        return fixed4(1, 1, 1, 1);

                    position += distance * direction;
                }

                return fixed4(0, 0, 0, 1);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 viewDirection = 0;

                float3 pos = 0;

                if (_WorldSpace > 0.5) {
                    float4 worldSpaceOrigin = float4(_WorldSpaceCameraPos, 1);

                    viewDirection = normalize(i.worldPos - worldSpaceOrigin);
                    pos = i.worldPos;
                }
                else {
                    float4 objectSpaceOrigin = mul(unity_WorldToObject, float4(_WorldSpaceCameraPos, 1));

                    viewDirection = normalize(i.objectPos - objectSpaceOrigin);
                    pos = i.objectPos;
                }

                return raymarch(pos, viewDirection);
            }

            ENDCG
        }
    }
}
