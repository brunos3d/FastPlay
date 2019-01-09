using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("AddForce")]
	[Path("Actions/Rigidbody/AddForce")]
	[Body("AddForce", "Rigidbody", "Rigidbody Icon")]
	public class RigidbodyAddForce : ActionNode, IRegisterPorts {

		public InputValue<Rigidbody> target;
		public InputValue<Vector3> force;
		public InputValue<ForceMode> force_mode;

		public void OnRegisterPorts() {
			target = RegisterInputValue<Rigidbody>("target");
			force = RegisterInputValue<Vector3>("force");
			force_mode = RegisterInputValue<ForceMode>("forceMode", ForceMode.Force);
		}

		public override void OnExecute() {
			target.value.AddForce(force.value, force_mode.value);
			Call(output);
		}
	}
}
