//Hardcoded version of the 3x3 boxblur kernel://
//1 , 1 , 1//
//1 , 1 , 1//
//1 , 1 , 1//

Shader "HvA_PostFX/BoXBlur"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}

		_Width ("TextureSize", Float) = 128	
		_Height ("TextureSize", Float) = 128
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

			uniform float _Width;
			uniform float _Height;

			uniform float3 _KernelLine1;
			uniform float3 _KernelLine2;
			uniform float3 _KernelLine3;

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

				//textureSize to UV coords (0 to 1)//
				float w = 1 / _Width;
				float h = 1 / _Height;

				//texture sampling
				// tl , tc , tr
				// cl , cc , cr
				// bl , bc , br
				
				float3 tl = tex2D(_MainTex, uv + fixed2(-w, -h));	// Top Left
				float3 tc = tex2D(_MainTex, uv + fixed2( 0, -h));	// Top Centre
				float3 tr = tex2D(_MainTex, uv + fixed2(+w, -h));	// Top Right

				float3 cl = tex2D(_MainTex, uv + fixed2(-w, 0));	// Centre Left
				float3 cc = tex2D(_MainTex, uv);					// Centre Centre
				float3 cr = tex2D(_MainTex, uv + fixed2(+w, 0));	// Centre Right

				float3 bl = tex2D(_MainTex, uv + fixed2(-w, +h));	// Bottom Left
				float3 bc = tex2D(_MainTex, uv + fixed2( 0, +h));	// Bottom Centre
				float3 br = tex2D(_MainTex, uv + fixed2(+w, +h));	// Bottom Right

				//combine results
				float3 result = 
				tl + tc + tr +
				cl + cc + cr +
				bl + bc + br;

				result = result / 9;//surrounding pixels average for blur

				return float4(result.r, result.g, result.b, 1);
			}
			ENDCG
		}
	}
}
