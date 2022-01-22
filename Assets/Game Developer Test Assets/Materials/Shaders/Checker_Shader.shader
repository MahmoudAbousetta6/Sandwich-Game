Shader "Unlit/Checker"
{
    Properties
    {
        // _MainTex ("Texture", 2D) = "white" {}
        _Color1 ("Color 1", Color) = (.85, .85, .85, 1)
        _Color2 ("Color 2", Color) = (.75, .75, .75, 1)
        _GridColor ("Grid Color", Color) = (.5, .5, .5, 1)
        _Thickness ("Thickness", float) = .01
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

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float3 worldPos : TEXCOORD2;
                float4 vertex : SV_POSITION;
            };

            float4 _Color1, _Color2, _GridColor;
            float _Thickness;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.uv = v.uv;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float num =  ceil(i.worldPos.x) + ceil(i.worldPos.z + 1);
                float4 base = num % 2 ? _Color1 : _Color2;

                float hor = step(frac(i.worldPos.x + _Thickness / 2) , _Thickness);
                float ver = step(frac(i.worldPos.z + _Thickness / 2), _Thickness);
                float grid = saturate(hor + ver);

                return grid ? _GridColor : base;
            }
            ENDCG
        }
    }
}
