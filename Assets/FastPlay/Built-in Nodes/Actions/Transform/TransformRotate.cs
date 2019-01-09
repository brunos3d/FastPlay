using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("Rotate")]
	[Path("Actions/Transform/Rotate")]
	[Body("Rotate", "Transform", "Transform Icon")]
	public class TransformRotate : ActionNode, IRegisterPorts {

		public InputValue<Transform> target;
		public InputValue<Vector3> euler_angles;

		public void OnRegisterPorts() {
			target = RegisterInputValue<Transform>("target");
			euler_angles = RegisterInputValue<Vector3>("eulerAngles");
		}

		public override void OnExecute() {
			target.value.Rotate(euler_angles.value);
			Call(output);
		}
	}
}
