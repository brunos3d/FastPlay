using System;
using System.Linq;
using System.Collections.Generic;

namespace FastPlay.Runtime {
	[HideInList]
	public class MacroNode : Node, IRegisterDefaultPorts {

		[NonSerialized]
		private int validated_frame;

		// The asset
		public GraphAsset reference;

		public InputNode input_node;

		public OutputNode output_node;

		public InputAction input;

		public OutputAction output;

		[NonSerialized]
		public List<InputAction> input_acts = new List<InputAction>();

		[NonSerialized]
		public Dictionary<int, OutputAction> output_acts = new Dictionary<int, OutputAction>();

		[NonSerialized]
		public Dictionary<int, IInputValue> input_values = new Dictionary<int, IInputValue>();

		[NonSerialized]
		public List<IOutputValue> output_values = new List<IOutputValue>();

		public override void Validate() {
			//stack overflow prevent
			if (validated_frame++ > 1) return;
			if (reference) {
				macro = this;

				input_node = (InputNode)reference.graph.nodes.FirstOrDefault(n => n is InputNode);
				output_node = (OutputNode)reference.graph.nodes.FirstOrDefault(n => n is OutputNode);

				if (input_node) {
					input_node.macro = this;
				}
				if (output_node) {
					output_node.macro = this;
				}

				reference.Validate();
			}
			base.Validate();
			validated_frame = 0;
		}

		public override void Initialize() {
			if (!reference.isInstance) {
				reference = reference.GetClone();
			}
			reference.graph.macro = this;
			reference.Play(Current.controller);
			base.Initialize();
		}

		public void OnRegisterDefaultPorts() {
			if (reference) {
				name = "{ " + string.Format("{0}", reference.name) + " }";
				macro = this;

				input_node = (InputNode)reference.graph.nodes.FirstOrDefault(n => n is InputNode);
				output_node = (OutputNode)reference.graph.nodes.FirstOrDefault(n => n is OutputNode);

				if (input_node) {
					input_node.macro = this;
					input = RegisterEntryPort("In", OnExecute);

					if (output_node) {
						output_node.macro = this;
						output = RegisterExitPort("Out");
					}
				}

				if (macro) {
					if (input_node) {
						input_acts = new List<InputAction>();
						input_values = new Dictionary<int, IInputValue>();
						foreach (Parameter parameter in reference.graph.inputParameters) {
							if (parameter is ParameterInput) {
								input_acts.Add(RegisterEntryPort(parameter.name, () => { Call(input_node.output_acts[parameter.id]); }));
							}
							else {
								input_values[parameter.id] = (IInputValue)RegisterInputValue(parameter.valueType, parameter.name);
							}
						}
					}
					if (output_node) {
						output_acts = new Dictionary<int, OutputAction>();
						output_values = new List<IOutputValue>();
						foreach (Parameter parameter in reference.graph.outputParameters) {
							if (parameter is ParameterOutput) {
								output_acts[parameter.id] = RegisterExitPort(parameter.name);
							}
							else {
								output_values.Add((IOutputValue)RegisterOutputValue(parameter.valueType, parameter.name, () => { return output_node.input_values[parameter.id].GetValue(); }));
							}
						}
					}
				}
			}
		}

		public virtual void OnExecute() {
			input_node.OnExecute();
		}

		public object GetInputValue(int key) {
			return input_values[key].GetValue();
		}

		public void CallAction(int key) {
			Call(output_acts[key]);
		}
	}
}
