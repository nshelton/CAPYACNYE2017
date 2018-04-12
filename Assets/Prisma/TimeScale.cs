using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeScale : MonoBehaviour {

	public float Scale
	{
		set {  Time.timeScale = value;}
	}
}
