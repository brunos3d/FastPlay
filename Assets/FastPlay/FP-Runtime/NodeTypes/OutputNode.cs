using System;
using System.Collections.Generic;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Icon("OutputNode Icon")]
	[Title("Output")]
	[Path("Graph/Output")]
	public class OutputNode : Node, IRegisterDefaultPorts {

		[NonSerialized]
		public InputAction input;

		[NonSerialized]
		public List<InputAction> input_acts = new List<InputAction>();

		[NonSerialized]
		public Dictionary<int, IInputValue> input_values = new Dictionary<int, IInputValue>();

		public void OnRegisterDefaultPorts() {
			input = RegisterEntryPort("In", OnExecute);

			input_acts = new List<InputAction>();
			input_values = new Dictionary<int, IInputValue>();
			foreach (Parameter parameter in graph.outputParameters) {
				if (parameter is ParameterOutput) {
					input_acts.Add(RegisterEntryPort(parameter.name, () => { Call(macro.output_acts[parameter.id]); }));
				}
				else {
					input_values[parameter.id] = (IInputValue)RegisterInputValue(parameter.valueType, parameter.name);
				}
			}
		}

		public virtual void OnExecute() {
			Call(macro.output);
		}

		public object GetInputValue(int key) {
			return input_values[key].GetValue();
		}
	}
}
