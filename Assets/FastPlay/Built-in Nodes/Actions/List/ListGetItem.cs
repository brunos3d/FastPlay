using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Title("GetItem")]
	[Subtitle("List")]
	[Path("Actions/List/GetItem<T>")]
	public class ListGetItem<T> : ValueNode<T>, IRegisterPorts {

		public InputValue<List<T>> list;
		public InputValue<int> index;

		public ListGetItem() { }

		public void OnRegisterPorts() {
			list = RegisterInputValue<List<T>>("list");
			index = RegisterInputValue<int>("index");
		}

		public override T OnGetValue() {
			return list.value[index];
		}
	}
}
