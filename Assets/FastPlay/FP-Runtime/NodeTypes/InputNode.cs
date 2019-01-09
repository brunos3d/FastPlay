using System;
using System.Collections.Generic;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Icon("InputNode Icon")]
	[Title("Input")]
	[Path("Graph/Input")]
	public class InputNode : Node, IRegisterDefaultPorts {

		[NonSerialized]
		public OutputAction output;

		[NonSerialized]
		public Dictionary<int, OutputAction> output_acts = new Dictionary<int, OutputAction>();

		[NonSerialized]
		public List<IOutputValue> output_values = new List<IOutputValue>();

		public void OnRegisterDefaultPorts() {
			output = RegisterExitPort("Out");

			int id = 0;
			output_values = new List<IOutputValue>();
			output_acts = new Dictionary<int, OutputAction>();
			foreach (Parameter parameter in graph.inputParameters) {
				if (parameter is ParameterInput) {
					output_acts[parameter.id] = RegisterExitPort(parameter.name);
				}
				else {
					output_values.Add((IOutputValue)RegisterOutputValue(parameter.valueType, parameter.name, () => { return macro.input_values[parameter.id].GetValue(); }));
				}
			}
			id++;
		}

		public virtual void OnExecute() {
			Call(output);
		}

		public void CallAction(int key) {
			Call(output_acts[key]);
		}
	}
}
