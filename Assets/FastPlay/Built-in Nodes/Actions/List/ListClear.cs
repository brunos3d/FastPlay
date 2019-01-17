using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Title("Clear")]
	[Subtitle("List")]
	[Path("Actions/List/Clear<T>")]
	public class ListClear<T> : ActionNode, IRegisterPorts {

		public InputValue<List<T>> list;

		public void OnRegisterPorts() {
			list = RegisterInputValue<List<T>>("list");
		}

		public override void OnExecute() {
			list.value.Clear();
		}
	}
}
