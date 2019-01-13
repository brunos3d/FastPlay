using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BindingSample : MonoBehaviour {

	public string GetSetMe { get; set; }

	public void CallMe() { }

	public bool CompareMe(int a, int b) {
		return a == b;
	}
}
