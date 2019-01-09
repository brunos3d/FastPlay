using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("New WWW")]
	[Path("Functions/Constructors/New WWW")]
	[Body("New WWW")]
	public class NewWWW : ValueNode<WWW>, IRegisterPorts {

		public InputValue<string> url;

		public void OnRegisterPorts() {
			url = RegisterInputValue<string>("URL", string.Empty);
		}

		public override WWW OnGetValue() {
			return new WWW(url.value);
		}
	}
}
