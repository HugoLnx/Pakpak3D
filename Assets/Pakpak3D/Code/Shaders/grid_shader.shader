Shader "Unlit/GridShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _TexBorderUVSize ("Texture Border UV Size", Float) = 0.4
        _RenderBorderWSSize ("Rendering Border WS Size", Float) = 0.1
        _Size ("Size", Vector) = (10, 1, 5, 0)
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

            static const fixed3 RED = fixed3(1, 0, 0);
            static const fixed3 GREEN = fixed3(0, 1, 0);
            static const fixed3 BLUE = fixed3(0, 0, 1);
            static const fixed3 WHITE = fixed3(1, 1, 1);
            static const fixed3 BLACK = fixed3(0, 0, 0);
            static const fixed3 YELLOW = fixed3(1, 1, 0);
            static const fixed3 CYAN = fixed3(0, 1, 1);
            static const fixed3 MAGENTA = fixed3(1, 0, 1);

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Size;
            float _TexBorderUVSize;
            float _RenderBorderWSSize;

            int stepBetween(float minEdge, float maxEdge, float value) {
                return step(minEdge, value) * step(value, maxEdge);
            }

            float3 ToTestVec(float3 vec) {
                float3 absVec = abs(vec);
                float3 testVec = absVec;
                testVec.x -= step(vec.x, 0.) * 0.95;
                testVec.y -= step(vec.y, 0.) * 0.95;
                testVec.z -= step(vec.z, 0.) * 0.95;
                return testVec;
            }

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 tangent : TANGENT;
                float3 normal : NORMAL;
            };

            float3 InxToColor(float inx) {
                int isMiddle = stepBetween(0.45, 0.55, inx);
                int isBorder = stepBetween(0.9, 1.0, inx);
                int isBeyondEdge = step(1.0, inx);
                int isSomething = isMiddle | isBorder | isBeyondEdge;
                int isNothing = 1 - isSomething;

                float cinx = 1 - inx;
                fixed3 col = fixed3(0, 0, 0);
                col += isMiddle * cinx * GREEN;
                col += isBorder * cinx * BLUE;
                col += isBeyondEdge * cinx * RED;
                col += isNothing * cinx * WHITE;
                return col;
            }

            struct v2f
            {
                float2 uv2 : TEXCOORD0;
                // UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float2 sizeUvS : TEXCOORD1;
                // float3 normal : TEXCOORD1;
                // float3 tangent : TANGENT;
                // float3 bitangent : TEXCOORD2;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                float2 uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv2 = uv - .5;
                // o.normal = UnityObjectToWorldNormal(v.normal);
                // o.tangent = UnityObjectToWorldDir(v.tangent.xyz);
                // o.bitangent = cross(o.normal, o.tangent) * v.tangent.w;
                float3 normal = v.normal;
                float3 tangent = v.tangent.xyz;
                float3 bitangent = cross(normal, tangent) * v.tangent.w;
                float3 size = _Size.xyz;

                float3x3 rotationToUvSpace = float3x3(tangent, bitangent, normal);
                o.sizeUvS = abs(mul(rotationToUvSpace, size).xy);
                return o;
            }

                // float3 vec = float3(sizeUvS, 0);
                // float3 testVec = ToTestVec(vec);
                // return float4(testVec, 1);

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv2 = i.uv2;
                float2 sizeUvS = i.sizeUvS;
                float2 halfSizeUvS = sizeUvS * 0.5;

                // Calculate base variables
                float centerUvDist =  max(abs(uv2.x), abs(uv2.y));
                float edgeUvDist = 0.5 - centerUvDist;

                float2 centerDistWS2d = abs(uv2 * sizeUvS);
                float2 edgeDistWS2d = halfSizeUvS - centerDistWS2d;
                float edgeDistWS = min(edgeDistWS2d.x, edgeDistWS2d.y);

                // Detect border or middle
                int isBorder = step(edgeDistWS, _RenderBorderWSSize);
                int isMiddle = 1 - isBorder;


                // fixed4 col = tex2D(_MainTex, i.uv);

                fixed3 col = fixed3(0, 0, 0);
                col += isBorder * edgeUvDist * BLUE;
                col += isMiddle * edgeUvDist * WHITE;

                return fixed4(col, 1);
            }
            ENDCG
        }
    }
}
