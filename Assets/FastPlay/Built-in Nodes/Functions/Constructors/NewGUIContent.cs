using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("New GUIContent")]
	[Path("Functions/Constructors/New GUIContent")]
	[Body("New GUIContent")]
	public class NewGUIContent : ValueNode<GUIContent>, IRegisterPorts {

		public InputValue<string> text;
		public InputValue<Texture> image;
		public InputValue<string> tooltip;

		public void OnRegisterPorts() {
			text = RegisterInputValue<string>("text");
			image = RegisterInputValue<Texture>("image");
			tooltip = RegisterInputValue<string>("tooltip");
		}

		public override GUIContent OnGetValue() {
			return new GUIContent(text.value, image.value, tooltip.value);
		}
	}
}
