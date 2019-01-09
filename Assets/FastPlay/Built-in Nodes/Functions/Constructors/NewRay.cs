using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("New Ray")]
	[Path("Functions/Constructors/New Ray")]
	[Body("New Ray")]
	public class NewRay : ValueNode<Ray>, IRegisterPorts {

		public InputValue<Vector3> origin, direction;

		public void OnRegisterPorts() {
			Ray defaultRay = default(Ray);
			origin = RegisterInputValue<Vector3>("origin", defaultRay.origin);
			direction = RegisterInputValue<Vector3>("direction", defaultRay.direction);
		}

		public override Ray OnGetValue() {
			return new Ray(origin.value, direction.value);
		}
	}
}
