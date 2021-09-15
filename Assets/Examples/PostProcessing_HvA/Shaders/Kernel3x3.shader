//Author: Daniel van der Meulen, 2018//
//Shader to use and play with 3x3 Kernels; see https://en.wikipedia.org/wiki/Kernel_(image_processing) for inspiration//

Shader "HvA_PostFX/Kernel3x3"
{
	Properties
	{
		_MainTex ("Texture (Screen)", 2D) = "white" {}

		_Width ("Texture Size Width", Float) = 128	
		_Height ("Texture Size Height", Float) = 128	

		//only uses first 3 numbers
		_KernelLine1 ("Kernel Line 1", Vector) = (1,1,1,1)
		_KernelLine2 ("Kernel Line 2", Vector) = (1,1,1,1)
		_KernelLine3 ("Kernel Line 3", Vector) = (1,1,1,1)

		_Divisor ("Divisor for the kernel", Float) = 0
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

			uniform float _Divisor;

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

				float w = 1 / _Width;
				float h = 1 / _Height;

				// Sample the texture for the kernel
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

				//apply Kernel
				tl *= _KernelLine3.z;
				tc *= _KernelLine3.y;
				tr *= _KernelLine3.x;

				cl *= _KernelLine2.z;
				cc *= _KernelLine2.y;
				cr *= _KernelLine2.x;

				bl *= _KernelLine1.z;
				bc *= _KernelLine1.y;
				br *= _KernelLine1.x;

				//combine results
				float3 result = 
				tl + tc + tr +
				cl + cc + cr +
				bl + bc + br;

				if(_Divisor != 0) //dont devide by zero =D (try it for some fun results tho)
					result = result / _Divisor;

				return float4(result.r, result.g, result.b, 1);
			}
			ENDCG
		}
	}
}
