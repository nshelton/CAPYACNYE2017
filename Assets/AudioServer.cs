using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using NAudio.Dsp;

public class AudioServer : MonoBehaviour {

    private const int FFT_SIZE = 4098;
    private const int CONST_M = 12; 

	private AutoGainControl m_volumeControl;
	private AutoGainControl m_bassVolumeControl;
	private AutoGainControl m_trebVolumeControl;
	private AutoGainControl m_midVolumeControl;

	[SerializeField]
	private float m_autoGainDecay = 0.99f;
	private float m_outputLevel;
	private float m_outputBassLevel;
	private float m_outputTrebLevel;
	private float m_outputMidLevel;

    // FFt stuff
    private float[] m_rawBuffer;

	[SerializeField]
	private float m_FFTDecay = 0.99f;
    
    [SerializeField]
	private float m_freqLogScale = 0.99f;

    [SerializeField]
	private float m_freqScale = 0.99f;

    [SerializeField]
    private LineRenderer wavLine;

    [SerializeField]
    private LineRenderer fftLine;

    [SerializeField]
    private LineRenderer ifftLine;


    [SerializeField]
    private bool _debug;

    [SerializeField]
    private Material _debugMaterial;

    [SerializeField]
    private int m_historyLength;

    // private Complex[] data_n = new Complex[FFT_SIZE];
    // private Complex[] data_nminus1 = new Complex[FFT_SIZE];

    private Texture2D m_fftTexture;
    private AudioSource m_possibleAudioSource;    
    private int m_historyOffset = 0;

	void Start () {

        m_possibleAudioSource = GetComponent(typeof(AudioSource)) as AudioSource;

        m_volumeControl     = new AutoGainControl(m_autoGainDecay);
        m_bassVolumeControl = new AutoGainControl(m_autoGainDecay);
        m_trebVolumeControl = new AutoGainControl(m_autoGainDecay);
        m_midVolumeControl  = new AutoGainControl(m_autoGainDecay);

        m_rawBuffer = new float[FFT_SIZE];

        //fftLine.positionCount = FFT_SIZE/2;
        ifftLine.positionCount = FFT_SIZE/2;
        wavLine.positionCount = FFT_SIZE;

        m_fftTexture = new Texture2D( m_historyLength, FFT_SIZE/2, TextureFormat.RGBAFloat, true);

        _debugMaterial.SetTexture("_MainTex", m_fftTexture);

	}
	int frameNum = 0;
	// Update is called once per frame
	void Update () {
        m_outputLevel = m_volumeControl.NormalizeVal(Lasp.AudioInput.CalculateRMS(Lasp.FilterType.Bypass));
        m_outputBassLevel = m_bassVolumeControl.NormalizeVal(Lasp.AudioInput.CalculateRMS(Lasp.FilterType.LowPass));
        m_outputTrebLevel = m_trebVolumeControl.NormalizeVal(Lasp.AudioInput.CalculateRMS(Lasp.FilterType.HighPass));
        m_outputMidLevel = m_midVolumeControl.NormalizeVal(Lasp.AudioInput.CalculateRMS(Lasp.FilterType.BandPass));


        if ( m_possibleAudioSource == null)
        {
            Lasp.AudioInput.RetrieveWaveform(Lasp.FilterType.Bypass, m_rawBuffer);
        }
        else
        {
            m_possibleAudioSource.GetOutputData(m_rawBuffer, 0);
        }

        for(int i = 0; i < FFT_SIZE/2; i ++)
        {
            wavLine.SetPosition(i, new Vector3((float)i / (float)FFT_SIZE ,m_rawBuffer[i], 0f));
            
        }

      //  NAudio.Dsp.FastFourierTransform.FFT(true, CONST_M, data_n);
        m_historyOffset = frameNum++ % m_historyLength;
        int lastIndex = 0;


        float minFreqMap = 0;
        float maxFreqMap = 0;

        minFreqMap = Mathf.Log(2f, 0 )* m_freqScale;
        maxFreqMap = Mathf.Log(2f, (FFT_SIZE/2)) * m_freqScale;

        // Debug.LogFormat("min{0}, max{1}", minFreqMap, maxFreqMap);

        // for(int i = 0; i < FFT_SIZE/2; i ++)
        // {
        //     float xPos = Mathf.Log(i , 2f) * m_freqScale;
            
        //     // xPos = (xPos - minFreqMap) / (maxFreqMap - minFreqMap);
        //     float x = Mathf.Sqrt(data_n[i].X * data_n[i].X + data_n[i].Y *  data_n[i].Y);
        //     x = Mathf.Log(x + 1);
        //     x = Mathf.Abs(x);

        //     int maxWidth = lastIndex + 100;
        //     for( int p = lastIndex; p < Mathf.Min((int) (xPos * FFT_SIZE) , maxWidth); p++)
        //     {
        //         m_fftTexture.SetPixel(m_historyOffset,  p, new Color(x,x,x) * 256);
        //         lastIndex = p;
        //     }

        //     // m_fftTexture.SetPixel(m_historyOffset, (int) (xPos * FFT_SIZE),  new Color(x,1,x) * 256);
        //     // lastIndex++;

        //     x = m_FFTDecay * data_nminus1[i].X + (1f - m_FFTDecay) * x;

        //     fftLine.SetPosition(i, new Vector3(xPos, x, 0f));
        //     data_nminus1[i].X = x;

        //     //-------------------------------------------------------------------------------
            
        //     float y = data_n[i].Y;
        //     y = Mathf.Log(y + 1);
        //     y = Mathf.Abs(y);

        //     y = m_FFTDecay * data_nminus1[i].Y + (1f - m_FFTDecay) * y;
        //     ifftLine.SetPosition(i, new Vector3(xPos, y, 0f));
        //     data_nminus1[i].Y = y;
        // }

        // m_fftTexture.Apply();
        _debugMaterial.SetFloat("_HistoryOffset", (float)m_historyOffset / (float)m_historyLength);
	}

	public float getLevel(Lasp.FilterType filterType)
	{
		switch(filterType)
		{
			case Lasp.FilterType.LowPass :		return m_outputBassLevel;
			case Lasp.FilterType.HighPass :		return m_outputTrebLevel;
			case Lasp.FilterType.BandPass :		return m_outputMidLevel;
			case Lasp.FilterType.Bypass :		return m_outputLevel;
			default : Debug.Log("bad filterType"); return 0;
		}
	}



    void OnGUI()
    {
        float scale = 3f;
        if (_debug && Event.current.type.Equals(EventType.Repaint))
        {
            if (_debugMaterial && m_fftTexture)
            {
                var rect = new Rect(0, 0, 4 *  m_historyLength /scale, (FFT_SIZE/2) / scale);
                Graphics.DrawTexture(rect, m_fftTexture, _debugMaterial);
            }
        }
    }
    
    public struct AutoGainControl
    {
        // scale factor needed to normalize incoming values
        public float Gain;

        // peak of the raw values coming in (this decays over time)
        public float Peak;

        // speed of exponential decay
        public float Decay;

        public AutoGainControl(float decay)
        {
            this.Peak = 1.0f;
            this.Decay = decay;
            this.Gain = 1.0f;
        }

        public float NormalizeVal(float rawVal)
        {
            // NGS: prevent numerical instability
            if(rawVal < 0.000001f)
            {
                return 0 ;
            }

            float decayingPeak = this.Peak * Mathf.Exp(-this.Decay * Time.deltaTime);
            this.Peak = Mathf.Max(decayingPeak, rawVal);

            if ( this.Peak > 0.001f )
            {
                this.Gain = 1.0f / this.Peak;
            }

            return this.Gain * rawVal;

        }
    }
}
