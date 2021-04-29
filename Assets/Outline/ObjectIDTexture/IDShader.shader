Shader "IDShader"
{
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            fixed4 _IdColor;
            uniform float4 _Bulge;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                

                float4 worldSpacePosition = mul(unity_ObjectToWorld, v.vertex);
                
                float cutoff = 0.025;
                float scale = clamp(.04 / distance(_Bulge.xyz, worldSpacePosition), cutoff, 0.3) - cutoff;
                scale *= _Bulge.w;
                worldSpacePosition += float4(normalize(v.normal) * scale, 0);

                //world to clip
                o.vertex = mul(UNITY_MATRIX_VP, worldSpacePosition);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return _IdColor;
            }
            ENDCG
        }
    }
}
