Shader "Unlit/GridShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BorderSizeTexUVS ("Border Size in Texture UV", Float) = 0.4
        _BorderSizeWS ("Border Size in World Space", Float) = 0.1
        _LoopScale ("Scale Texture Looping Area", Float) = 1
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
            float _BorderSizeTexUVS;
            float _BorderSizeWS;
            float _LoopScale;

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
                float2 sizeWS : TEXCOORD1;
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
                o.sizeWS = abs(mul(rotationToUvSpace, size).xy);
                return o;
            }

            float VecMax(float2 vec) {
                return max(vec.x, vec.y);
            }

            float VecMin(float2 vec) {
                return min(vec.x, vec.y);
            }

            int IsNot(int v) { return 1 - v; }
            int2 IsNot(int2 v) { return 1 - v; }
            int And(int a, int b) { return a * b; }
            int And(int a, int b, int c) { return a * b * c; }
            int2 And(int2 a, int2 b) { return a * b; }
            int2 And(int2 a, int2 b, int2 c) { return a * b * c; }
            int Or(int a, int b) { return max(a, b); }
            int Or(int a, int b, int c) { return max(max(a, b), c); }
            int2 Or(int2 a, int2 b) { return max(a, b); }
            int2 Or(int2 a, int2 b, int2 c) { return max(max(a, b), c); }
            int Xor(int a, int b) { return (a - b) * (a - b); }
            int2 Xor(int2 a, int2 b) { return (a - b) * (a - b); }

                // float3 vec = float3(sizeWS, 0);
                // float3 testVec = ToTestVec(vec);
                // return float4(testVec, 1);

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv2 = i.uv2;
                float2 sizeWS = i.sizeWS;
                float2 halfSizeWS = sizeWS * 0.5;

                // Calculate base variables
                float2 uv2WS = uv2 * sizeWS;
                float2 uv2AbsWS = abs(uv2WS);
                float2 borderLimitWS = halfSizeWS - _BorderSizeWS;
                float2 borderLimitTexUV = 0.5 - _BorderSizeTexUVS;
                float2 wsToTexUv2 = _BorderSizeTexUVS / _BorderSizeWS;
                float2 texSizeWS = _BorderSizeWS / _BorderSizeTexUVS;
                float2 halfTexSizeWS = texSizeWS * 0.5;
                // float2 texUv2 = (abs(uv2WS) - borderLimitWS) * wsToTexUv2 * sign(uv2WS);
                int2 isBorder = step(borderLimitWS, uv2AbsWS);
                int2 isNotBorder = IsNot(isBorder);
                float2 texUv2 = 0;

                float2 borderTexUvInx = (uv2AbsWS - borderLimitWS) / _BorderSizeWS;
                texUv2 += isBorder * lerp(borderLimitTexUV, 0.5, borderTexUvInx) * sign(uv2WS);

                float2 middleTexSizeWS = (texSizeWS - _BorderSizeWS * 2);
                float2 middleTexUvInx = fmod((uv2WS + halfSizeWS - _BorderSizeWS) * _LoopScale, middleTexSizeWS) / middleTexSizeWS;
                texUv2 += isNotBorder * lerp(-borderLimitTexUV, borderLimitTexUV, middleTexUvInx);

                // // Detect border or middle
                // int isBorder = step(edgeDistWS, _BorderSizeWS);
                // int isMiddle = 1 - isBorder;

                // float2 texUv2 = uv2WS * wsToTexUv2;

                float2 texUv = texUv2 + 0.5;
                fixed3 texCol = tex2D(_MainTex, texUv);

                // int isBorderX = step(borderLimitWS.x, abs(uv2WS.x));
                // int isBorderY = step(borderLimitWS.y, abs(uv2WS.y));
                // int isBorder = Or(isBorderX, isBorderY);
                int isBorderOnly = Xor(isBorder.x, isBorder.y);
                int isBorder1d = Or(isBorder.x, isBorder.y);
                fixed3 col = texCol;
                // fixed3 col = BLACK;
                // col += isBorder1d * texCol;
                // return fixed4(texCol, 1);

                // fixed3 col = fixed3(0, 0, 0);
                // col += isBorder * edgeUvDist * BLUE;
                // col += isMiddle * edgeUvDist * WHITE;

                return fixed4(col, 1);
            }
            ENDCG
        }
    }
}
