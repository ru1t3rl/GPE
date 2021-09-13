#include "UnityCG.cginc"
uniform float4 _LightColor0; 

uniform float4 _BaseColor;
uniform sampler2D _BaseMap;
uniform float4 _BaseMap_ST;

uniform float4 _UnlitColor;
uniform float _DiffuseThreshold;
uniform float4 _OutlineColor;
uniform float _LitOutlineThickness;
uniform float _UnlitOutlineThickness;
uniform float4 _SpecColor;
uniform float _Smoothness;

struct appdata {
    float4 vertex: POSITION;
    float3 normal: NORMAL;
    float2 uv: TEXCOORD0;
};

struct v2f {
    float4 pos: SV_POSITION;
    float2 uv: TEXCOORD0;
    float4 posWorld: TEXCOORD1;
    float3 normalDir: TEXCOORD2;
};

v2f vert (appdata i) {
    v2f o;

    float4x4 modelMatrix = unity_ObjectToWorld;
    float4x4 modelMatrixInverse = unity_WorldToObject;

    o.uv = TRANSFORM_TEX(i.uv, _BaseMap);

    o.posWorld = mul(modelMatrix, i.vertex);
    o.normalDir = normalize(mul(float4(i.normal, 0.0), modelMatrixInverse).xyz);
    o.pos = UnityObjectToClipPos(i.vertex);  

    return o;        
}

float4 frag(v2f i) : COLOR {
    float3 normalDir = normalize(i.normalDir);
    float3 viewDir = normalize(_WorldSpaceCameraPos - i.posWorld.xyz);
    float3 lightDir;
    float attenuation;

    if(_WorldSpaceLightPos0.w == 0.0) {
        // Directional Light
        attenuation = 1.0;
        lightDir = normalize(_WorldSpaceLightPos0.xyz);
        } else {
        // Point or Spot Light
        float3 vertexToLightSource = _WorldSpaceLightPos0.xyz - i.posWorld.xyz;
        float distance = length(vertexToLightSource);
        attenuation = 1.0/ distance;
        lightDir = normalize(vertexToLightSource);
    }

    // default: unlit
    float3 fragmentColor = _UnlitColor.rgb;

    // Low priority: diffuse illumination
    if(attenuation * max(0.0, dot(normalDir, lightDir)) >= _DiffuseThreshold) {
        fragmentColor = _LightColor0.rgb * _OutlineColor.rgb;
    }

    // highest priority: highlights
    // first check if the light source is on the right
    // and if it has more than half light intensity
    if(dot(normalDir, lightDir) > 0.0 &&
    attenuation * pow(max(0.0, dot(reflect(-lightDir, normalDir), viewDir)), _Smoothness) > 0.5) {
        fragmentColor = _SpecColor.a * _LightColor0.rgb * _SpecColor.rgb 
        + (1.0 - _SpecColor.a) * fragmentColor;
    }

    return (float4(_BaseColor * fragmentColor, 1));
}