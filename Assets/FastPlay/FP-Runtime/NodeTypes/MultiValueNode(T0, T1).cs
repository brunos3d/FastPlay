using System;

namespace FastPlay.Runtime {
	[HideInList]
	public abstract class MultiValueNode<T0, T1> : ValueNode, IRegisterDefaultPorts {

		public override Type valueType {
			get {
				return typeof(T0);
			}
		}

		public override Type[] valueTypes {
			get {
				return new Type[] { typeof(T0), typeof(T1) };
			}
		}

		public virtual string[] portNames {
			get {
				return new string[] { "Get 0", "Get 1" };
			}
		}

		public MultiValueNode() { }

		public void OnRegisterDefaultPorts() {
			RegisterOutputValue<T0>(portNames[0], Get_0);
			RegisterOutputValue<T1>(portNames[1], Get_1);
		}

		public virtual T0 Get_0() { return default(T0); }

		public virtual T1 Get_1() { return default(T1); }
	}
}
