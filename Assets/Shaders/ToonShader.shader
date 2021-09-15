/* Sources: 
*  https://en.wikibooks.org/wiki/Cg_Programming/Unity/Toon_Shading (Basic Toon Shading)
*  https://catlikecoding.com/unity/tutorials/rendering/part-5/ (Placing the code in a cginc file)
*/

Shader "Custom/Toon Shader"
{
    Properties
    {
        _LightColorInfluence ("Light Color Influence", Float) = 1
        _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        _BaseMap("Base Map", 2D) = "white" {}
        _UnlitColor("Unlit Base Color", Color) = (0.5, 0.5, 0.5, 1)
        _DiffuseThreshold("Threshold for base colors", Range(0, 1)) = 0.1
        
        _OutlineColor("Outline Color", Color) = (1, 1, 1, 1)
        _LitOutlineThickness("Lit Outline Thickness", Range(0, 1)) = .1
        _UnlitOutlineThickness("Unlit Outline Thickness", Range(0, 1)) = .4

        _SpecColor("Specular Color", Color) = (1, 1, 1, 1)
        _Smoothness ("Smoothness", Float) = 10
    }
    SubShader
    {
        Pass
        {
            // Pass for the ambient light and first light source
            Tags {
                "LightMode" = "ForwardBase"
            }

            Cull Off

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "ToonShading.cginc"

            ENDCG
        }
        Pass {
            // Pass to process aditional light sources
            Tags {"LightMode"="ForwardAdd"}
            
            // Blend specular hightlights over framebuffer
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off

            CGPROGRAM

            #pragma vertex vert  
            #pragma fragment frag 

            #include "ToonShading.cginc"

            ENDCG
        }
    }
    Fallback "Specular"
}
