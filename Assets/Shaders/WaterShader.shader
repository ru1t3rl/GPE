Shader "Custom/WaterShader"
{
    Properties
    {
        [Header(Colors)]
        _Depth ("Depth", Float) = 1
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _ShallowWater ("Shallow Water", Color) = (.50, .78, .81, 1)
        _DeepWater("Deep Water", Color) = (0.32, 0.63, 0.59, 1)

        [Header(Flow)]
        [NoScaleOffset] _FlowMap ("Flow (RG, A noise)", 2D) = "black" {}
        _UJump ("U jump per phase", Range(-0.25, 0.25)) = 0.25
        _VJump ("V jump per phase", Range(-0.25, 0.25)) = 0.25
        _Tiling ("Tiling", Float) = 1
        _Speed ("Speed", Float) = 1
        _FlowStrength ("Flow Strength", Float) = 1
        _FlowOffset ("Flow Offset", Float) = 0

        [Header(Refraction)]
        [Toggle] _UseRefraction("Use Refraction", Float) = 1
        _RefractionIntensity("Refraction Intensity", Float) = 1
        _RefractionSpeed("Refraction Speed", Float) = 0.1

        [Header(Other Settigns)]
        [NoScaleOffset] _NormalMap ("Normals", 2D) = "bump" {}
        _NormalIntensity ("Normal Strength", Range(0, 2)) = 1
        [NoScaleOffset] _DerivHeightMap ("Deriv (AG) Height (B)", 2D) = "black" {}
        _Smoothness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        //Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        LOD 200

        GrabPass {

        }

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows
        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        #include "Flow.cginc"

        struct Input
        {
            float2 uv_MainTex;
            float4 screenPos;
            float3 worldPos;
            float3 viewDir;
        };

        // Water Color
        half _Depth;
        fixed4 _ShallowWater, _DeepWater;

        // Flow 
        sampler2D _MainTex, _FlowMap;
        float _UJump, _VJump, _Tiling, _Speed, _FlowStrength, _FlowOffset;

        // Refraction
        float _UseRefraction;
        float _RefractionIntensity, _RefractionSpeed;

        // Other Vars
        half _Smoothness;
        half _Metallic;
        float _NormalIntensity;
        sampler2D _NormalMap, _DerivHeightMap;

        sampler2D _CameraDepthTexture;
        sampler2D _GrabTexture;

        float3 UnpackDerivativeHeight (float4 textureData) {
            float3 dh = textureData.agb;
            dh.xy = dh.xy * 2 - 1;
            return dh;
        }

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
        // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input i, inout SurfaceOutputStandard o)
        {
            // Flow vectors
            float3 flow = tex2D(_FlowMap, i.uv_MainTex).rgb;
			flow.xy = flow.xy * 2 - 1;
			flow *= _FlowStrength;
            float noise = tex2D(_FlowMap, i.uv_MainTex).a;
            float time = _Time.y * _Speed + noise;
            float2 jump = float2(_UJump, _VJump);

            float3 uvwA = FlowUVW(i.uv_MainTex, flow.xy, jump, _FlowOffset, _Tiling, time, false);
            float3 uvwB = FlowUVW(i.uv_MainTex, flow.xy, jump, _FlowOffset, _Tiling, time, true);

            float3 dhA =
				UnpackDerivativeHeight(tex2D(_DerivHeightMap, uvwA.xy)) * (uvwA.z * _NormalIntensity);
			float3 dhB =
				UnpackDerivativeHeight(tex2D(_DerivHeightMap, uvwB.xy)) * (uvwB.z * _NormalIntensity); 
            float3 finalNormal = normalize(float3(-(dhA.xy + dhB.xy), 1));

            fixed4 texA = tex2D(_MainTex, uvwA.xy) * uvwA.z;
            fixed4 texB = tex2D(_MainTex, uvwB.xy) * uvwB.z;
            fixed4 finalTex = texA + texB;

            // Get depth behind current pixel, linear depth
            float depth = tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPos)).r;
            depth = LinearEyeDepth(depth);

            // depthDelta between water surface and pixel behind
            float depthDelta = depth - i.screenPos.w;
            
            // Set color based on depth
            fixed4 c = lerp(_ShallowWater, _DeepWater, saturate(depthDelta / _Depth));
            //c *= lerp((1, 1, 1, 1), finalTex, c.a);

            if(_UseRefraction == 1) {
                float x = i.screenPos.x;
                float4 refraction = tex2Dproj(_GrabTexture, i.screenPos);
                refraction.x += sin(_Time.y * _RefractionSpeed) * _RefractionIntensity;
                c = lerp(refraction, c, c.a);
            }


            o.Normal = finalNormal;
            o.Albedo = c;
            o.Metallic = _Metallic;
            o.Smoothness = _Smoothness;
            o.Alpha = c.a;            
        }
        ENDCG
    }
    FallBack "Diffuse"
}
