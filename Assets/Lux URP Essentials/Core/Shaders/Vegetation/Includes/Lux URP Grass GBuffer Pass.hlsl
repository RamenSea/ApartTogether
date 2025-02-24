#ifndef LUX_LIT_GBUFFER_PASS_INCLUDED
#define LUX_LIT_GBUFFER_PASS_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/UnityGBuffer.hlsl"

#if defined(LOD_FADE_CROSSFADE)
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"
#endif


half4 SampleMetallicSpecGloss(float2 uv, half albedoAlpha)
{
    return 1;
}

struct Attributes
{
    float3 positionOS                   : POSITION;
    float3 normalOS                     : NORMAL;
    float4 tangentOS                    : TANGENT;
    float2 texcoord                     : TEXCOORD0;
    #ifdef LIGHTMAP_ON
        float2 staticLightmapUV         : TEXCOORD1;
    #endif
    #ifdef DYNAMICLIGHTMAP_ON
        float2 dynamicLightmapUV        : TEXCOORD2;
    #endif
    half4 color                         : COLOR;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varyings
{
    float2 uv                           : TEXCOORD0;
    float3 positionWS                   : TEXCOORD1;
    half3 normalWS                      : TEXCOORD2;
    #if defined(_NORMALMAP)
        half4 tangentWS                 : TEXCOORD3;
    #endif
    #ifdef _ADDITIONAL_LIGHTS_VERTEX
        half3 vertexLighting            : TEXCOORD4;
    #endif
    #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
        float4 shadowCoord              : TEXCOORD5;
    #endif
    //#if defined(REQUIRES_TANGENT_SPACE_VIEW_DIR_INTERPOLATOR)
    //    half3 viewDirTS                : TEXCOORD6;
    //#endif
    DECLARE_LIGHTMAP_OR_SH(staticLightmapUV, vertexSH, 7);
    #ifdef DYNAMICLIGHTMAP_ON
        float2  dynamicLightmapUV       : TEXCOORD8;
    #endif

    half2 fadeOcclusion                 : TEXCOORD9;

    #ifdef USE_APV_PROBE_OCCLUSION
        float4 probeOcclusion           : TEXCOORD10;
    #endif

    float4 positionCS                   : SV_POSITION;
        
    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
};

//--------------------------------------
//  Fragment shader and functions

// Include the surface function
#include "Includes/Lux URP Grass SurfaceData.hlsl"

void InitializeInputData(Varyings input, half3 normalTS, out InputData inputData)
{
    inputData = (InputData)0;
    inputData.positionWS = input.positionWS;
    inputData.positionCS = input.positionCS;

    half3 viewDirWS = GetWorldSpaceNormalizeViewDir(input.positionWS);

    #if defined(_NORMALMAP)
        float sgn = input.tangentWS.w; 
        float3 bitangent = sgn * cross(input.normalWS.xyz, input.tangentWS.xyz);
        inputData.normalWS = TransformTangentToWorld(normalTS, half3x3(input.tangentWS.xyz, bitangent.xyz, input.normalWS.xyz));
    #else
        inputData.normalWS = input.normalWS;
    #endif

//  Flip normals in view space
    #if defined _NORMALVS
        half3 normalVS = TransformWorldToViewDir(inputData.normalWS, false); 
        normalVS.z = abs(normalVS.z);
        inputData.normalWS = normalize( mul( (real3x3)UNITY_MATRIX_I_V, normalVS) );
    #endif

    inputData.normalWS = NormalizeNormalPerPixel(inputData.normalWS);
    inputData.viewDirectionWS = viewDirWS;

    #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
        inputData.shadowCoord = input.shadowCoord;
    #elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
        inputData.shadowCoord = TransformWorldToShadowCoord(inputData.positionWS);
    #else
        inputData.shadowCoord = float4(0, 0, 0, 0);
    #endif

    inputData.fogCoord = 0.0; // we don't apply fog in the guffer pass
    #ifdef _ADDITIONAL_LIGHTS_VERTEX
        inputData.vertexLighting = input.vertexLighting.xyz;
    #else
        inputData.vertexLighting = half3(0, 0, 0);
    #endif
    #if defined(DYNAMICLIGHTMAP_ON)
        inputData.bakedGI = SAMPLE_GI(input.staticLightmapUV, input.dynamicLightmapUV, input.vertexSH, inputData.normalWS);
    #elif !defined(LIGHTMAP_ON) && (defined(PROBE_VOLUMES_L1) || defined(PROBE_VOLUMES_L2))
        inputData.bakedGI = SAMPLE_GI(input.vertexSH,
            GetAbsolutePositionWS(inputData.positionWS),
            inputData.normalWS,
            inputData.viewDirectionWS,
            inputData.positionCS.xy,
            input.probeOcclusion,
            inputData.shadowMask);
    #else
        inputData.bakedGI = SAMPLE_GI(input.staticLightmapUV, input.vertexSH, inputData.normalWS);
        inputData.shadowMask = SAMPLE_SHADOWMASK(input.staticLightmapUV);
    #endif

    #if defined(LIGHTMAP_ON)
        inputData.bakedGI *= _LightMapBoost;
    #endif
    
    inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(input.positionCS);
}

// Used in Standard (Physically Based) shader
Varyings LitGBufferPassVertex(Attributes input)
{
    Varyings output = (Varyings)0;

    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_TRANSFER_INSTANCE_ID(input, output);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

    #define vocclusion input.color.r

    float3 positionWS;
    half3 normalWS;
    half2 fadeOcclusion;
    BendGrass(
        input.positionOS,
        input.normalOS,
        input.color,
        positionWS,
        normalWS,
        fadeOcclusion
    );

//  VertexPositionInputs
    VertexPositionInputs vertexInput;
    vertexInput.positionWS = positionWS;
    vertexInput.positionVS = TransformWorldToView(positionWS);
    vertexInput.positionCS = TransformWorldToHClip(positionWS);
    float4 ndc = vertexInput.positionCS * 0.5f;
    vertexInput.positionNDC.xy = float2(ndc.x, ndc.y * _ProjectionParams.x) + ndc.w;
    vertexInput.positionNDC.zw = vertexInput.positionCS.zw;

//  VertexNormalInputs
    VertexNormalInputs normalInput;
    normalInput.normalWS = NormalizeNormalPerVertex(normalWS);
    normalInput.tangentWS = TransformObjectToWorldDir(input.tangentOS.xyz);  

    output.normalWS = normalInput.normalWS;
    #if defined(_NORMALMAP)
        real sign = input.tangentOS.w * GetOddNegativeScale();
        half4 tangentWS = half4(normalInput.tangentWS.xyz, sign);
        output.tangentWS = tangentWS;
    #endif
    
    output.uv = input.texcoord; //TRANSFORM_TEX(input.texcoord, _BaseMap);
    output.fadeOcclusion = fadeOcclusion;
    output.fadeOcclusion.y = lerp(1.0h, vocclusion, _Occlusion);
    
    OUTPUT_LIGHTMAP_UV(input.staticLightmapUV, unity_LightmapST, output.staticLightmapUV);
    #ifdef DYNAMICLIGHTMAP_ON
        output.dynamicLightmapUV = input.dynamicLightmapUV.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
    #endif
    
    OUTPUT_SH4(vertexInput.positionWS, output.normalWS.xyz, GetWorldSpaceNormalizeViewDir(vertexInput.positionWS), output.vertexSH, output.probeOcclusion);
    
    #ifdef _ADDITIONAL_LIGHTS_VERTEX
        half3 vertexLight = VertexLighting(vertexInput.positionWS, normalInput.normalWS);
        output.vertexLighting = vertexLight;
    #endif

    #if defined(REQUIRES_WORLD_SPACE_POS_INTERPOLATOR)
        output.positionWS = vertexInput.positionWS;
    #endif

    #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
        output.shadowCoord = GetShadowCoord(vertexInput);
    #endif

    output.positionCS = vertexInput.positionCS;

    return output;
}

// Used in Standard (Physically Based) shader
FragmentOutput LitGBufferPassFragment(Varyings input)
{
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

    #ifdef LOD_FADE_CROSSFADE
        LODFadeCrossFade(input.positionCS);
    #endif

    SurfaceData surfaceData;
    InitializeSurfaceData(input, input.fadeOcclusion, surfaceData);

    InputData inputData;
    InitializeInputData(input, surfaceData.normalTS, inputData);

#ifdef _DBUFFER
    #if defined(_RECEIVEDECALS)
        ApplyDecalToSurfaceData(input.positionCS, surfaceData, inputData);
    #endif
#endif

    BRDFData brdfData;
    InitializeBRDFData(surfaceData.albedo, surfaceData.metallic, surfaceData.specular, surfaceData.smoothness, surfaceData.alpha, brdfData);

    Light mainLight = GetMainLight(inputData.shadowCoord, inputData.positionWS, inputData.shadowMask);
    MixRealtimeAndBakedGI(mainLight, inputData.normalWS, inputData.bakedGI, inputData.shadowMask);
    half3 color = GlobalIllumination(brdfData, inputData.bakedGI, surfaceData.occlusion, inputData.normalWS, inputData.viewDirectionWS);

    return BRDFDataToGbuffer(brdfData, inputData, surfaceData.smoothness, surfaceData.emission + color, surfaceData.occlusion);
}

#endif