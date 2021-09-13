Shader "Custom/MyLighting"
{
    Properties
    {
        _ShadowTint ("Shadow Tint", Color) = (0,0,0,1)
        _BaseColor ("Color", Color) = (1,1,1,1)
        _BaseMap ("Texture", 2D) = "white" {}
        _Smoothness ("Smoothness", Range(0, 1)) = 0.5
        [Gamma] _Metallic ("Metallic", Range(0, 1)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Tags {
                "LightMode"="ForwardBase"
            }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityPBSLighting.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float3 normal: TEXCOORD1;
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD2;
            };

            sampler2D _BaseMap;
            float4 _BaseMap_ST;
            float4 _BaseColor, 
                   _ShadowTint;
            float _Smoothness, _Metallic;


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _BaseMap);
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                i.normal = normalize(i.normal);
                float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
                float3 lightDir = _WorldSpaceLightPos0.xyz;
                float3 lightColor = _LightColor0.rgb;    

                float3 albedo = tex2D(_BaseMap, i.uv).rgb * _BaseColor.rgb;

                float3 diffuse = lightColor * albedo * DotClamped(lightDir, i.normal);
                float3 shadowColor = (1 - DotClamped(lightDir, i.normal)) * _ShadowTint;
                diffuse = diffuse + shadowColor;
                
                return float4(diffuse + specular, 1);
            }
            ENDCG
        }
    }
}
