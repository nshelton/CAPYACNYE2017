using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TunnelTek : MonoBehaviour {

    #region Private Constants
    
    private const float INITIAL_SCALE = 0.12f;
    private const float INITIAL_OFFSET = 3.0f;
    private const float INITIAL_SPEED = 0.1f;
    private const float INITIAL_TUNNEL_LENGTH = 50;

    private const int INITIAL_SIDES = 4;
    private const int INITIAL_TUNNEL_SEGMENTS = 4;
    private const int INITIAL_COLOR_BANDS = 5;
    
    #endregion

    #region Public Enumerations
    
    enum RepeatMode 
    {
        Rings = 0,
        Helix = 1
    }

    #endregion

    #region Public Properties

    [SerializeField]
    private Material m_material;

    public Material Material
    {
        get { return m_material; }
        set { m_material = value; }
    }

    [SerializeField]
    private Mesh m_mesh;

    public Mesh Mesh
    {
        get { return m_mesh; }
        set { m_mesh = value; }
    }

    [Header("Primitive")]
    [SerializeField]
    private Vector3 m_scaleBias = Vector3.one * INITIAL_SCALE;

    public Vector3 ScaleBias
    {
        get { return m_scaleBias; }
        set { m_scaleBias = value; }
    }

    [SerializeField]	
    private Vector3 m_scaleSineAmp;

    public Vector3 ScaleSineAmp
    {
        get { return m_scaleSineAmp; }
        set { m_scaleSineAmp = value; }
    }

    [SerializeField]
    private Vector3 m_scaleSineFreq;

    public Vector3 ScaleSineFreq
    {
        get { return m_scaleSineFreq; }
        set { m_scaleSineFreq = value; }
    }

    [SerializeField]
    private Vector3 m_scaleTimePhase;

    public Vector3 ScaleTimePhase
    {
        get { return m_scaleTimePhase; }
        set { m_scaleTimePhase = value; }
    }

    [Header("Offset")]
    [SerializeField]
    private Vector3 m_offsetBias = Vector3.one * INITIAL_OFFSET;

    public Vector3 OffsetBias
    {
        get { return m_offsetBias; }
        set { m_offsetBias = value; }
    }
        
    [SerializeField]	
    private Vector3 m_offsetSineAmp;

    public Vector3 OffsetSineAmp
    {
        get { return m_offsetSineAmp; }
        set { m_offsetSineAmp = value; }
    }

    [SerializeField]
    private Vector3 m_offsetSineFreq;

    public Vector3 OffsetSineFreq
    {
        get { return m_offsetSineFreq; }
        set { m_offsetSineFreq = value; }
        }

    [SerializeField]
    private Vector3 m_offsetTimePhase;

    public Vector3 OffsetTimePhase
    {
        get { return m_offsetTimePhase; }
        set { m_offsetTimePhase = value; }
    }

    [Header("Rotation")]
    [SerializeField]
    private float m_rotationAmp;

    public float RotationAmp
    {
        get { return m_rotationAmp; }
        set { m_rotationAmp = value; }
    }

    [SerializeField]
    private float m_rotationFreq;

    public float RotationFreq
    {
        get { return m_rotationFreq; }
        set { m_rotationFreq = value; }
    }

    [SerializeField]
    private float m_rotationSpeed;

    public float RotationSpeed
    {
        get { return m_rotationSpeed; }
        set { m_rotationSpeed = value; }
    }

    [Header("Colors")]
    [SerializeField]
    [ColorUsageAttribute(true,true,0f,8f,0.125f,3f)] //HDR
    private Color m_Color = Color.white;

    public Color Color1
    {
        get { return m_Color; }
        set { m_Color = value; }
    }

    [SerializeField]
    [ColorUsageAttribute(true,true,0f,8f,0.125f,3f)] //HDR
    private Color m_Color2;

    public Color Color2
    {
        get { return m_Color2; }
        set { m_Color2 = value; }
    }

    [SerializeField]
    private float m_colorCycles = INITIAL_COLOR_BANDS;

    public float ColorCycles
    {
        get { return m_colorCycles; }
        set { m_colorCycles = value; }
    }
        
    [SerializeField]
    private float m_emissionAmount;

    public float EmissionAmount
    {
        get { return m_emissionAmount; }
        set { m_emissionAmount = value; }
    }

    [Header("Mesh Dimensions (requires reset resources)")]
    [SerializeField]
    private int m_numSides = INITIAL_SIDES;

    public int NumSides
    {
        get { return m_numSides; }
        set { m_numSides = value; }
    }

    [SerializeField]
    private int m_numSegments = INITIAL_TUNNEL_SEGMENTS;
 
    public int NumSegments
    {
        get { return m_numSegments; }
        set { m_numSegments = value; }
    }

    [Header("Misc Settings")]
    [SerializeField]
    private RepeatMode m_repeatMode;
 
    [SerializeField]
    private float m_scrollSpeed = INITIAL_SPEED;

    public float ScrollSpeed
    {
        get { return m_scrollSpeed; }
        set { m_scrollSpeed = value; }
    }
 
    [SerializeField]
    private float m_tunnelLength = INITIAL_TUNNEL_LENGTH;

    public float TunnelLength
    {
        get { return m_tunnelLength; }
        set { m_tunnelLength = value; }
    }

    #endregion

    #region Private Variables
    
    private TunnelTekMergedMesh m_bulkMesh;
   
    private MaterialPropertyBlock m_props;

    private float m_scrollPosition = 0;

    #endregion

    #region Private Methods

    [ContextMenu("Reset Resources")]
    void ResetResources()
    {
        if(m_mesh == null)
        {
            Debug.LogError("m_mesh == null");
            return;
        }

        m_bulkMesh = new TunnelTekMergedMesh(m_mesh, m_numSegments, m_numSides);
    }

    #endregion

    #region Public Methods

    public void Awake()
    {
        ResetResources();
    }
    
    public void Start () 
    {
        if (m_mesh == null)
        {
            Debug.LogError("m_mesh == null. Disabling component");
            enabled = false;
            return;
        }

        ResetResources();
    }
    
    public void Update () {

        if ( m_props == null)
        {
            m_props = new MaterialPropertyBlock();
        }

        if (m_bulkMesh == null || m_bulkMesh.Mesh == null)
        {
            return;
        }

        m_props.SetVector("_InstanceScaleBias", m_scaleBias);
        m_props.SetVector("_InstanceScaleSineAmp", m_scaleSineAmp);
        m_props.SetVector("_InstanceScaleSineFreq", m_scaleSineFreq);
        m_props.SetVector("_InstanceScaleTimePhase", m_scaleTimePhase);

        m_props.SetVector("_OffsetBias", m_offsetBias);
        m_props.SetVector("_OffsetSineAmp", m_offsetSineAmp);
        m_props.SetVector("_OffsetSineFreq", m_offsetSineFreq);
        m_props.SetVector("_OffsetTimePhase", m_offsetTimePhase);

        m_props.SetVector("_Color", m_Color);
        m_props.SetVector("_Color2", m_Color2);
        
        m_scrollPosition += m_scrollSpeed * Time.smoothDeltaTime;

        m_props.SetVector("_TunnelParam", new Vector4(
            m_tunnelLength, m_numSides, m_numSegments, m_scrollPosition));
        
        m_props.SetVector("_Rotation",  new Vector4(m_rotationAmp, m_rotationFreq, m_rotationSpeed, 0));
        m_props.SetVector("_Config",  new Vector4((int)m_repeatMode, m_colorCycles, m_emissionAmount, 0));

        Graphics.DrawMesh(
            m_bulkMesh.Mesh,
            transform.position,
            transform.rotation,
            m_material, 0, null, 0, 
            m_props, false, false);
    }

    #endregion
}
