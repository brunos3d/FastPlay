using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("New Bounds")]
	[Path("Functions/Constructors/New Bounds")]
	[Body("New Bounds")]
	public class NewBounds : ValueNode<Bounds>, IRegisterPorts {

		public InputValue<Vector3> center, _size;

		public void OnRegisterPorts() {
			Bounds defaultBounds = default(Bounds);
			center = RegisterInputValue<Vector3>("center", defaultBounds.center);
			_size = RegisterInputValue<Vector3>("size", defaultBounds.size);
		}

		public override Bounds OnGetValue() {
			return new Bounds(center.value, _size.value);
		}
	}
}
