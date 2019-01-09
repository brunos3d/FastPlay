using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("New Color")]
	[Path("Functions/Constructors/New Color")]
	[Body("New Color")]
	public class NewColor : ValueNode<Color>, IRegisterPorts {

		public InputValue<float> r, g, b, a;

		public void OnRegisterPorts() {
			Color defaultColor = default(Color);
			r = RegisterInputValue<float>("R", defaultColor.r);
			g = RegisterInputValue<float>("G", defaultColor.g);
			b = RegisterInputValue<float>("B", defaultColor.b);
			a = RegisterInputValue<float>("A", defaultColor.a);
		}

		public override Color OnGetValue() {
			return new Color(r.value, g.value, b.value, a.value);
		}
	}
}
