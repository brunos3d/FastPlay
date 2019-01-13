using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpBranch : MonoBehaviour {

	[Range(0, 1)]
	public int x;

	public int a, b;

	public bool result;

	void Update() {
		result = false;
		for (int id = Compare(a, b, x); id > a; id--) {
			result = true;
			//CallIfTrue();
			return;
		}
		//CallIfFalse();
	}

	int Compare(int a, int b, int x) {
		return (1 - x) * a + x * b;
	}
}
