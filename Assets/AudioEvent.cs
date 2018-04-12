using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AudioEvent : MonoBehaviour {

    [System.Serializable] public class FloatEvent : UnityEvent<float> {}

	[SerializeField]
	public float m_level;

	[SerializeField]
	Lasp.FilterType m_filterType = Lasp.FilterType.Bypass;

	[SerializeField]
    public AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);

	[SerializeField]
	[Range(0,1)]
	private float m_attack = 1f;

	[SerializeField]
	[Range(0,1)]
	private float m_decay = 1f;

	[SerializeField]
	private FloatEvent floatEvent;

	private AudioServer m_audioServer;

	void Start () {
		AudioServer[] servers = FindObjectsOfType(typeof(AudioServer)) as AudioServer[];
		m_audioServer = servers[0];
	}
	
	// Update is called once per frame
	void Update () {

		float input = m_audioServer.getLevel(m_filterType);

      	if (m_attack < 1.0f)
        {
            var coeff = Mathf.Pow(m_attack, 2.3f) * -128;
            input -= (input - m_level) * Mathf.Exp(coeff * Time.deltaTime);
        }

        var speed = m_decay < 1.0f ? m_decay * 10 + 0.5f : 100.0f;
        m_level = Mathf.Max(input, m_level - Time.deltaTime * speed);

		var o = curve.Evaluate(m_level);
		floatEvent.Invoke(o);



	}
}
