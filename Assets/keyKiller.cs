
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class keyKiller : MonoBehaviour {

	[SerializeField]
	private GameObject[] m_targets;

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown("space"))
		{
			for(int i = 0; i < m_targets.Length; i++)
				m_targets[i].SetActive(false);
		}

	}
}
