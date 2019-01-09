using System.Collections.Generic;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("AddItem")]
	[Path("Actions/List/AddItem")]
	[Body("AddItem", "List")]
	public class ListAddItem<T> : ActionNode, IRegisterPorts {

		public InputValue<List<T>> list;
		public InputValue<T> item;

		public void OnRegisterPorts() {
			list = RegisterInputValue<List<T>>("list");
			item = RegisterInputValue<T>("item");
		}

		public override void OnExecute() {
			list.value.Add(item.value);
		}
	}
}
