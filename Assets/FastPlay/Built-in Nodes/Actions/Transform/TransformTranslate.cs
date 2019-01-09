using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("Translate")]
	[Path("Actions/Transform/Translate")]
	[Body("Translate", "Transform", "Transform Icon")]
	public class TransformTranslate : ActionNode, IRegisterPorts {

		public InputValue<Transform> target;
		public InputValue<Vector3> translation;

		public void OnRegisterPorts() {
			target = RegisterInputValue<Transform>("target");
			translation = RegisterInputValue<Vector3>("translation");
		}

		public override void OnExecute() {
			target.value.Translate(translation.value);
			Call(output);
		}
	}
}
