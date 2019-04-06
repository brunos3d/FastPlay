using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Title("ContainsItem")]
	[Subtitle("List")]
	[Path("Actions/List/ContainsItem<T>")]
	public class ListContainsItem<T> : ValueNode<bool>, IRegisterPorts {

		public InputValue<List<T>> list;
		public InputValue<T> item;

		public ListContainsItem() { }

		public void OnRegisterPorts() {
			list = RegisterInputValue<List<T>>("list");
			item = RegisterInputValue<T>("item");
		}

		public override bool OnGetValue() {
			return list.value.Contains(item.value);
		}
	}
}
