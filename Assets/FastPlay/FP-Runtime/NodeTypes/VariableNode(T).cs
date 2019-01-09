using UnityEngine;

namespace FastPlay.Runtime {
	[HideInList]
	public class VariableNode<T> : ValueNode<T>, IVariable {

		protected override bool useSet { get { return true; } }

		private VariableObject<T> variable_cache;

		public int variable_key;

		public Parameter<T> parameter;

		public VariableNode() { }

		public VariableNode(Parameter<T> parameter) {
			this.parameter = parameter;
			this.variable_key = parameter.GetInstanceID();
		}

		public override T OnGetValue() {
			return (variable_cache ?? (variable_cache = Current.GetVariable<T>(variable_key))).value;
		}

		public override void OnSetValue(T value) {
			(variable_cache ?? (variable_cache = Current.GetVariable<T>(variable_key))).value = value;
		}

		public int GetVariableKey() {
			return parameter.id;
		}

		public string GetVariableName() {
			return parameter.name;
		}
	}
}
