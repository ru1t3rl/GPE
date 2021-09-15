Shader "Daniel/WaterOfLife_amazing_prop"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Pixels ("TextureSize", Float) = 128	
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

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;

			uniform float4 _MainTex_ST;

			uniform float4 _Color;

			uniform float _Pixels;
			uniform float _Dampner;

			v2f vert (appdata v)
			{
				v2f o;

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{

				fixed4 col = tex2D(_MainTex, i.uv);
				fixed2 uv = i.uv;

				float s = 1 / _Pixels;
				//holes

				// tl , tc , tr
				// cl , cc , cr
				// bl , bc , br
				
				float cl = tex2D(_MainTex, uv + fixed2(-s, 0)).r; // Centre Left
				float tc = tex2D(_MainTex, uv + fixed2(-0, -s)).r; // Top Centre
				float cc = tex2D(_MainTex, uv).a; // Centre Centre
				float bc = tex2D(_MainTex, uv + fixed2(0, +s)).r; // Bottom Centre
				float cr = tex2D(_MainTex, uv + fixed2(+s, 0)).r; // Centre Right

				float test = (tc + bc + cr + cl) / 2 - cc;

				//test *= _Dampner;

				return float4(test, test, test, 1);
			}
			ENDCG
		}
	}
}
