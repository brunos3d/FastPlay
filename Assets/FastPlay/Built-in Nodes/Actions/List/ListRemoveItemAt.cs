using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Name("RemoveAt")]
	[Path("Actions/List/RemoveAt")]
	[Body("RemoveAt", "List")]
	public class ListRemoveItemAt<T> : ActionNode, IRegisterPorts {

		public InputValue<List<T>> list;
		public InputValue<int> index;

		public void OnRegisterPorts() {
			list = RegisterInputValue<List<T>>("list");
			index = RegisterInputValue<int>("index");
		}

		public override void OnExecute() {
			list.value.RemoveAt(index.value);
		}
	}
}
