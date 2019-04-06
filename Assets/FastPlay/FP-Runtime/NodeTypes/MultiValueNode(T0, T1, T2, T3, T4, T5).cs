using System;

namespace FastPlay.Runtime {
	[HideInList]
	public abstract class MultiValueNode<T0, T1, T2, T3, T4, T5> : ValueNode, IRegisterDefaultPorts {

		public override Type valueType {
			get {
				return typeof(T0);
			}
		}

		public override Type[] valueTypes {
			get {
				return new Type[] { typeof(T0), typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5) };
			}
		}

		public virtual string[] portNames {
			get {
				return new string[] { "Get 0", "Get 1", "Get 2", "Get 3", "Get 4", "Get 5" };
			}
		}

		public MultiValueNode() { }

		public void OnRegisterDefaultPorts() {
			RegisterOutputValue<T0>(portNames[0], Get_0);
			RegisterOutputValue<T1>(portNames[1], Get_1);
			RegisterOutputValue<T2>(portNames[2], Get_2);
			RegisterOutputValue<T3>(portNames[3], Get_3);
			RegisterOutputValue<T4>(portNames[4], Get_4);
			RegisterOutputValue<T5>(portNames[5], Get_5);
		}

		public virtual T0 Get_0() { return default(T0); }

		public virtual T1 Get_1() { return default(T1); }

		public virtual T2 Get_2() { return default(T2); }

		public virtual T3 Get_3() { return default(T3); }

		public virtual T4 Get_4() { return default(T4); }

		public virtual T5 Get_5() { return default(T5); }
	}
}
