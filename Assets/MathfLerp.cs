
using UnityEngine;
using FastPlay.Runtime;

[Title("Lerp")]
[Subtitle("Mathf")]
[Path("Generated/Mathf/Lerp(float, float, float) : float")]
public class MathfLerp : ValueNode<float>, IRegisterPorts {
	
	//#VARIABLES#
	public InputValue<float> a;
	public InputValue<float> b;
	public InputValue<float> t;

	public void OnRegisterPorts() {
		//#REGISTERPORTS#
		a = this.RegisterInputValue<float>("A");
		b = this.RegisterInputValue<float>("B");
		t = this.RegisterInputValue<float>("T");
	}

	public override float OnGetValue() {
		return UnityEngine.Mathf.Lerp(a.value,b.value,t.value);
	}
}