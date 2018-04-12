using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour {

	[SerializeField]
	public float m_level;

	[SerializeField]
	Lasp.FilterType m_filterType = Lasp.FilterType.Bypass;

	public int m_historyLength = 4096;
	private Queue<Vector3> history;
	private Vector3[] historyArray;
	
	private LineRenderer m_lineRenderer;
	private AudioServer m_audioServer;


	void Start () {
	 	history = new Queue<Vector3>();
		
		while(history.Count < m_historyLength) {
			history.Enqueue(Vector3.zero);
		}

		m_lineRenderer = GetComponent<LineRenderer>();
		m_lineRenderer.positionCount = m_historyLength;

		AudioServer[] servers = FindObjectsOfType(typeof(AudioServer)) as AudioServer[];
		m_audioServer = servers[0];
	}
	

	void Update () {

		// m_level = Lasp.AudioInput.CalculateRMS(m_filterType);
		m_level = m_audioServer.getLevel(m_filterType);

		Vector3 pos = new Vector3(0, m_level, 0);
		history.Enqueue(pos);

		while(history.Count > m_historyLength) {
			history.Dequeue();
		}

		historyArray = history.ToArray();
		for ( int i = 0; i < historyArray.Length; i ++)
		{
			historyArray[i].x = (float)i/(float)m_historyLength- 0.5f;
		}

		m_lineRenderer.SetPositions(historyArray);
	}
}
