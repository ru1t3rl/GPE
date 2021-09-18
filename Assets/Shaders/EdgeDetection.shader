/* Source:
*   https://danielilett.com/2019-05-11-tut1-4-smo-edge-detect/
*/
Shader "Custom/EdgeDetect"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_LineColor ("Line Color", Color) = (0, 0, 0, 1)
		_LineWeight ("Line Weight", Float) = 1
		_LineIntensity ("Line Intensity", Float) = 1
	}

	SubShader
	{
		Tags{"RenderType"="Overlay" "Queue"="Overlay"}
		Pass
		{		
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _CameraDepthNormalsTexture, 
					  _CameraDepthTexture, 
					  _CameraGBufferTexture0,
					  _CameraGBufferTexture1,
					  _CameraGBufferTexture2;
			float2 _MainTex_TexelSize;
			float4 _LineColor;
			float _LineWeight, _LineIntensity;

			float3 sobel(sampler2D tex, float2 uv)
			{
				float x = 0;
				float y = 0;

				float2 texelSize = _MainTex_TexelSize;

				x += tex2D(tex, uv + float2(-texelSize.x, -texelSize.y)) * -1.0 * _LineWeight;
				x += tex2D(tex, uv + float2(-texelSize.x,            0)) * -2.0 * _LineWeight;
				x += tex2D(tex, uv + float2(-texelSize.x,  texelSize.y)) * -1.0 * _LineWeight;

				x += tex2D(tex, uv + float2( texelSize.x, -texelSize.y)) *  1.0 * _LineWeight;
				x += tex2D(tex, uv + float2( texelSize.x,            0)) *  2.0 * _LineWeight;
				x += tex2D(tex, uv + float2( texelSize.x,  texelSize.y)) *  1.0 * _LineWeight;

				y += tex2D(tex, uv + float2(-texelSize.x, -texelSize.y)) * -1.0 * _LineWeight;
				y += tex2D(tex, uv + float2(           0, -texelSize.y)) * 0 * _LineWeight;
				y += tex2D(tex, uv + float2( texelSize.x, -texelSize.y)) * 1.0 * _LineWeight;

				y += tex2D(tex, uv + float2(-texelSize.x,  texelSize.y)) *  -1.0 * _LineWeight;
				y += tex2D(tex, uv + float2(           0,  texelSize.y)) *  0 * _LineWeight;
				y += tex2D(tex, uv + float2( texelSize.x,  texelSize.y)) *  1.0 * _LineWeight;

				return sqrt(x * x + y * y) * _LineIntensity;
			}

			// Credit: http://lolengine.net/blog/2013/07/27/rgb-to-hsv-in-glsl
			float3 rgb2hsv(float3 c)
			{
				float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
				float4 p = c.g < c.b ? float4(c.bg, K.wz) : float4(c.gb, K.xy);
				float4 q = c.r < p.x ? float4(p.xyw, c.r) : float4(c.r, p.yzx);

				float d = q.x - min(q.w, q.y);
				float e = 1.0e-10;
				return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
			}

			// Credit: http://lolengine.net/blog/2013/07/27/rgb-to-hsv-in-glsl
			float3 hsv2rgb(float3 c)
			{
				float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
				float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
				return c.z * lerp(K.xxx, saturate(p - K.xxx), c.y);
			}

			fixed4 frag(v2f_img i) : SV_Target
			{
				float4 col = tex2D(_MainTex, i.uv);                
				float3 normal;
				float depth;
				DecodeDepthNormal(tex2D(_CameraDepthNormalsTexture, i.uv), depth, normal);
				float4 depthEdge = float4(sobel(_CameraDepthTexture, i.uv), 1);
				float4 sceneEdge = float4(sobel(_MainTex, i.uv), 1);				

				float4 edgeValue = sceneEdge * depthEdge;			

				return col + (col * (edgeValue * _LineColor.rgba) *  (1 - tex2D(_CameraDepthTexture, i.uv).r));//(depth + length((normal.x + normal.y + normal.z)));
			}
			ENDCG
		}
	}
}
