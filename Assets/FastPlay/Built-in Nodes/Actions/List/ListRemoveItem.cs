using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Title("RemoveItem")]
	[Subtitle("List")]
	[Path("Actions/List/RemoveItem")]
	public class ListRemoveItem<T> : ActionNode, IRegisterPorts {

		public InputValue<List<T>> list;
		public InputValue<T> item;

		public void OnRegisterPorts() {
			list = RegisterInputValue<List<T>>("list");
			item = RegisterInputValue<T>("item");
		}

		public override void OnExecute() {
			list.value.Remove(item.value);
		}
	}
}
