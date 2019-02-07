using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FastPlay.Editor;

namespace FastPlay.Runtime {
	[HideInList]
	public class ConstructorNode<T> : Node, IValueNode, IRegisterDefaultPorts where T : struct {

		[NonSerialized]
		List<IInputValue> parameters = new List<IInputValue>();

		public Type valueType { get { return typeof(T); } }

		public ConstructorNode() { }

		public void OnRegisterDefaultPorts() {
			RegisterOutputValue<T>("Get", OnGetValue);

			parameters = new List<IInputValue>();
			foreach (ConstructorInfo constructor in valueType.GetConstructors()) {
				foreach (ParameterInfo parameter in constructor.GetParameters()) {
					parameters.Add((IInputValue)RegisterInputValue(parameter.ParameterType, parameter.Name.AddSpacesToSentence()));
				}
				break;
			}
		}

		public void SetType(Type type) {
			
		}

		public object GetValue() {
			return OnGetValue();
		}

		public virtual T OnGetValue() {
			object[] args = parameters.Select(i => i.GetValue()).ToArray();
			return (T)Activator.CreateInstance(valueType, args);
		}

		public void SetValue(object value) { }
	}
}
