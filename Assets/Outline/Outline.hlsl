TEXTURE2D(_CameraColorTexture);
SAMPLER(sampler_CameraColorTexture);
float4 _CameraColorTexture_TexelSize;

TEXTURE2D(_CameraDepthTexture);
SAMPLER(sampler_CameraDepthTexture);

TEXTURE2D(_CameraIDColorsTexture);
SAMPLER(sampler_CameraIDColorsTexture);

TEXTURE2D(_CameraDepthNormalsTexture);
SAMPLER(sampler_CameraDepthNormalsTexture);
 
float3 DecodeNormal(float4 enc)
{
    float kScale = 1.7777;
    float3 nn = enc.xyz*float3(2*kScale,2*kScale,0) + float3(-kScale,-kScale,1);
    float g = 2.0 / dot(nn.xyz,nn.xyz);
    float3 n;
    n.xy = g*nn.xy;
    n.z = g-1;
    return n;
}

void Outline_float(float2 UV, float OutlineThickness, float NormalThickness, float NormalsSensitivity, float DepthSensitivity, float4 OutlineColor, float2 Noise, float zFlipped, out float4 Out)
{
    float2 Texel = (1.0) / float2(_CameraColorTexture_TexelSize.z, _CameraColorTexture_TexelSize.w);

    float2 uvSamples[4];
    float2 uvSamplesNormal[4];
    float depthSamples[4];
    float3 normalSamples[4];
    float3 colorSamples[4];

    uvSamples[0] = UV - float2(Texel.x, Texel.y) * OutlineThickness * 0.5 + Noise;
    uvSamples[1] = UV + float2(Texel.x, Texel.y) * OutlineThickness * 0.5 + Noise;
    uvSamples[2] = UV + float2(Texel.x * OutlineThickness * 0.5, -Texel.y * OutlineThickness * 0.5) + Noise;
    uvSamples[3] = UV + float2(-Texel.x * OutlineThickness * 0.5, Texel.y * OutlineThickness * 0.5) + Noise;
    uvSamplesNormal[0] = UV - float2(Texel.x, Texel.y) * NormalThickness * 0.5 + Noise;  
    uvSamplesNormal[1] = UV + float2(Texel.x, Texel.y) * NormalThickness * 0.5 + Noise;
    uvSamplesNormal[2] = UV + float2(Texel.x * NormalThickness * 0.5, -Texel.y * NormalThickness * 0.5) + Noise;
    uvSamplesNormal[3] = UV + float2(-Texel.x * NormalThickness * 0.5, Texel.y * NormalThickness * 0.5) + Noise;

    for(int i = 0; i < 4 ; i++)
    {
        depthSamples[i] = SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, uvSamples[i]);
        depthSamples[i] = lerp(depthSamples[i], 1-depthSamples[i], zFlipped);
        normalSamples[i] = DecodeNormal(SAMPLE_TEXTURE2D(_CameraDepthNormalsTexture, sampler_CameraDepthNormalsTexture, uvSamplesNormal[i]));
        colorSamples[i] = SAMPLE_TEXTURE2D(_CameraIDColorsTexture, sampler_CameraIDColorsTexture, uvSamples[i]);
    }
    //Depth
    // float depthFiniteDifference0 = depthSamples[1] - depthSamples[0];
    // float depthFiniteDifference1 = depthSamples[3] - depthSamples[2];
    float currDepth = SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, UV);
    currDepth = lerp(currDepth, 1-currDepth, zFlipped);
    float depthFiniteDifference0 = (depthSamples[1] - currDepth) - (currDepth - depthSamples[0]);
    float depthFiniteDifference1 = (depthSamples[3] - currDepth) - (currDepth - depthSamples[2]);

    float edgeDepth = sqrt(pow(depthFiniteDifference0, 2) + pow(depthFiniteDifference1, 2)) * 100;
    float depthThreshold = (1/DepthSensitivity) * (depthSamples[0]+1);
    edgeDepth = edgeDepth > depthThreshold ? 1 : 0;

    // Normals
    float3 normalFiniteDifference0 = normalSamples[1] - normalSamples[0];
    float3 normalFiniteDifference1 = normalSamples[3] - normalSamples[2];
    float edgeNormal = sqrt(dot(normalFiniteDifference0, normalFiniteDifference0) + dot(normalFiniteDifference1, normalFiniteDifference1));
    edgeNormal = edgeNormal > (1/NormalsSensitivity) ? 1 : 0;

    //Color
    float3 colorFiniteDifference0 = colorSamples[1] - colorSamples[0];
    float3 colorFiniteDifference1 = colorSamples[3] - colorSamples[2];
    float edgeColor = dot(colorFiniteDifference0, colorFiniteDifference0) + dot(colorFiniteDifference1, colorFiniteDifference1);
	edgeColor = edgeColor > 0.000001 ? 1 : 0;
    
    float edge = max(max(edgeNormal, edgeColor),edgeDepth);
    float4 original = SAMPLE_TEXTURE2D(_CameraColorTexture, sampler_CameraColorTexture, UV);	
    Out = ((1 - edge) * original) + (edge * lerp(original, OutlineColor,  OutlineColor.a));
    //Out = SAMPLE_TEXTURE2D(_CameraIDColorsTexture, sampler_CameraIDColorsTexture, UV);
    //Out = max(edgeNormal,edgeDepth);;
    // if(UV[0] > .5f){
    // Out = SAMPLE_TEXTURE2D(_CameraDepthNormalsTexture, sampler_CameraDepthNormalsTexture, UV).w;
    //     if(UV[1] > .5f){
    //        Out = SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, UV);
    //     }
    // } else {
    //     if(UV[1] > .5f){
    //        Out = SAMPLE_TEXTURE2D(_CameraIDColorsTexture, sampler_CameraIDColorsTexture, UV);
    //     }
    // }
}