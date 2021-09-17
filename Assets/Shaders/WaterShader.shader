/* Sources:
*   https://answers.unity.com/questions/1402029/how-to-calculate-screen-space-texcoords-for-surfac.html
*   http://tinkering.ee/unity/asset-unity-refractive-shader/
*   https://halisavakis.com/my-take-on-shaders-stylized-water-shader/
*   https://catlikecoding.com/unity/tutorials/flow/texture-distortion/ 
*   https://catlikecoding.com/unity/tutorials/flow/looking-through-water/
*   https://gamedev.stackexchange.com/questions/129139/how-do-i-calculate-uv-space-from-world-space-in-the-fragment-shader 
*/
Shader "Custom/Water"
{
    Properties
    {
        [Header(Colors)]
        _Depth ("Depth", Float) = 1
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _ShallowWater ("Shallow Water", Color) = (.50, .78, .81, 1)
        _DeepWater("Deep Water", Color) = (0.32, 0.63, 0.59, 1)

        [Header(Reflection)]
        [Toggle] _UseReflections ("Use Reflections", Float) = 1
        _ReflectionIntensity("ReflectionIntensity", Float) = 1
        _ReflectionRefractionStrength("Reflection Refraction Strength", Range(0, 0.1)) = 0.05
        _ReflectionFade("Reflection Fade", Range(0, 1)) = 1
        

        [Header(Foam)]
        [Toggle] _UseFoam ("Use Foam", Float) = 1
        _FoamColor ("Foam Color", Color) = (1, 1, 1, 1)
        _FoamWidth ("Foam Width", Float) = 1
        _FoamThreshold ("Foam Threshold", Range(0, 1)) = 0.5
        [NoScaleOffset] _FoamNoise ("Foam Noise", 2D) = "white" {}
        _FoamTiling ("Foam Tiling", Float) = 1        

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
        _RefractionIntensity("Refraciton Intensity", Range(0, 1)) = 1
        _RefractionStrength("Refraction Strength", Range(0, 0.1)) = 0.05
        _RefractionDepthInfluence("Refraction Depth Influence", Range(0, 1)) = .75

        [Header(Other Settigns)]
        [NoScaleOffset] _DerivHeightMap ("Deriv (AG) Height (B)", 2D) = "black" {}        
        _NormalIntensity ("Normal Strength", Range(0, 2)) = 1        
        _Smoothness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0 
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" 
               "Queue"="Transparent" }
        //Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        LOD 200

        GrabPass { }

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

        // Reflections
        float _UseReflections, 
            _ReflectionRefractionStrength, 
            _ReflectionIntensity,
            _ReflectionFade;

        // Foam
        float _UseFoam, _FoamTiling, _FoamWidth, _FoamThreshold;
        float4 _FoamColor;
        sampler2D _FoamNoise;

        // Flow 
        sampler2D _MainTex, _FlowMap;
        float _UJump, _VJump, _Tiling, _Speed, _FlowStrength, _FlowOffset;

        // Refraction
        float _UseRefraction, 
            _RefractionStrength, 
            _RefractionDepthInfluence, 
            _RefractionIntensity;

        // Other Vars
        half _Smoothness;
        half _Metallic;
        float _NormalIntensity;
        sampler2D _DerivHeightMap;

        uniform sampler2D _WorldReflectionTexture;
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
            // Get depth behind current pixel, linear depth
            float depth = tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPos)).r;
            depth = LinearEyeDepth(depth);

            // depthDelta between water surface and pixel behind
            float depthDelta = depth - i.screenPos.w;
            
            // Set color based on depth
            fixed4 c = lerp(_ShallowWater, _DeepWater, (1 - _ReflectionFade) * saturate(depthDelta / _Depth));

            // Flow vectors
            float3 flow = tex2D(_FlowMap, i.uv_MainTex).rgb;
            flow.xy = flow.xy * 2 - 1;
            flow *= _FlowStrength;
            float noise = tex2D(_FlowMap, i.uv_MainTex).a;
            float time = _Time.y * _Speed + noise;
            float2 jump = float2(_UJump, _VJump);

            // Calculate uv's
            float3 uvwA = FlowUVW(i.uv_MainTex, flow.xy, jump, _FlowOffset, _Tiling, time, false);
            float3 uvwB = FlowUVW(i.uv_MainTex, flow.xy, jump, _FlowOffset, _Tiling, time, true);

            // Unpack normal's/height's
            float3 dhA =
            UnpackDerivativeHeight(tex2D(_DerivHeightMap, uvwA.xy)) * (uvwA.z * _NormalIntensity);
            float3 dhB =
            UnpackDerivativeHeight(tex2D(_DerivHeightMap, uvwB.xy)) * (uvwB.z * _NormalIntensity); 
            float3 finalNormal = normalize(float3(-(dhA.xy + dhB.xy), 1));

            // Map the main texture
            fixed4 texA = tex2D(_MainTex, uvwA.xy) * uvwA.z;
            fixed4 texB = tex2D(_MainTex, uvwB.xy) * uvwB.z;
            fixed4 finalTex = texA + texB;

            if(_UseReflections == 1) {
                // Set through a script made by Unity
                float4 reflection = tex2D(_WorldReflectionTexture, i.screenPos.xy/i.screenPos.w + (finalNormal.xy * _ReflectionRefractionStrength));
                reflection = lerp(c, reflection * c.a, c.a);
                c += (reflection * _ReflectionIntensity);
            }

            if(_UseRefraction == 1) 
            {   
                // Calculate UV offset based on the normal's             
                float2 uvOffset = finalNormal.xy * _RefractionStrength;
                
                float2 uv;
                if(depthDelta/_Depth > 0) {
                    uv= i.screenPos.xy / i.screenPos.w + uvOffset;
                    } else {
                    uv = i.screenPos.xy / i.screenPos.w;
                }
                float4 refraction = tex2D(_GrabTexture, uv).rgba;                
                
                c += (lerp(refraction * (1 - c.a), c, saturate(depthDelta/_Depth) * _RefractionDepthInfluence) * _RefractionIntensity);
            }

            if(_UseFoam == 1) {
                // Calculate uv's
                uvwA = FlowUVW(i.uv_MainTex, flow.xy, jump, _FlowOffset, _FoamTiling, time, false);
                uvwB = FlowUVW(i.uv_MainTex, flow.xy, jump, _FlowOffset, _FoamTiling, time, true);

                float3 foamColor = (tex2D(_FoamNoise, uvwA.xy) * uvwA.z 
                + tex2D(_FoamNoise, uvwB.xy) * uvwB.z ) 
                * _FoamColor;

                // To get cartoon like hard edge
                foamColor = foamColor > _FoamThreshold ? float3(0, 0, 0) : _FoamColor;

                if(_FoamWidth >= depthDelta/_Depth) {
                    c = float4(foamColor, 1) + c;
                }
            }

            o.Normal = finalNormal;
            o.Albedo = c;
            o.Metallic = _Metallic;
            o.Smoothness = _Smoothness;          
        }
        ENDCG
    }
    //FallBack "Diffuse"
}
