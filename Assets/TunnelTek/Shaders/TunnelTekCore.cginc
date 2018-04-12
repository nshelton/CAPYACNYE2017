float4 _InstanceScaleBias;
float4 _InstanceScaleSineAmp;
float4 _InstanceScaleSineFreq;
float4 _InstanceScaleTimePhase;

float4 _OffsetBias;
float4 _OffsetSineAmp;
float4 _OffsetSineFreq;
float4 _OffsetTimePhase;

float4 _TunnelParam; //m_tunnelLength, m_mumSides, m_numSegments, m_scrollSpeed
float4 _Rotation; // m_rotationAmp, m_rotationFreq, m_rotationSpeed, 0

float4 _Config; // RepeatMode, ColorRepeat, Emission

// quaternions maybe better
float3x3 rotationMatrix(float3 axis, float angle)
{
    axis = normalize(axis);
    float s = sin(angle);
    float c = cos(angle);
    float oc = 1.0 - c;
    
    return float3x3(oc * axis.x * axis.x + c,           oc * axis.x * axis.y - axis.z * s,  oc * axis.z * axis.x + axis.y * s,
                oc * axis.x * axis.y + axis.z * s,  oc * axis.y * axis.y + c,           oc * axis.y * axis.z - axis.x * s,
                oc * axis.z * axis.x - axis.y * s,  oc * axis.y * axis.z + axis.x * s,  oc * axis.z * axis.z + c);
}

// cache these here vars
float3x3 g_rotation; 
float phi;
float helix;
float scroll;
float3 scale;


float3 distortNormal(float3 normal)
{
    return mul(g_rotation, normal);
}

float3 distortPos(float3 pos, float2 uv)
{
    float theta = uv.x * 2.0 * 3.141592;
    scroll = frac(_TunnelParam.a + uv.y);

    helix = _Config.x > 0.0 ? (_TunnelParam.x / _TunnelParam.z) *  uv.x : 0.0;
    
    // distance down the tunnel
    phi = (uv.y * 2.0 * 3.1415 + helix);
    
    float3 offs = float3(0, 0, _TunnelParam.x * scroll  + helix);
 
    scale = _InstanceScaleBias + 
        _InstanceScaleSineAmp * sin((_InstanceScaleTimePhase * _Time.g + _InstanceScaleSineFreq) * phi);

    theta += _Rotation.x * sin(phi * _Rotation.y + _Time.x * _Rotation.z);
    
    g_rotation = rotationMatrix(float3(0, 0, 1), theta);

    offs += _OffsetBias + 
        _OffsetSineAmp * sin(_OffsetTimePhase * _Time.g + _OffsetSineFreq * phi);

    return mul(g_rotation, pos * scale + offs);

}