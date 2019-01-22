using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

namespace FastPlay {
	[Serializable]
	public class ObjectBase {

		[SerializeField]
		private int m_id;

		public string name;

		private static int clock;

		private Dictionary<string, List<MethodInfo>> m_methods = new Dictionary<string, List<MethodInfo>>();

		private static List<int> m_ids = new List<int>();

		private static Dictionary<int, ObjectBase> m_objects = new Dictionary<int, ObjectBase>();

		public int id {
			get {
				return this.m_id;
			}
			protected set {
				this.m_id = value;
			}
		}

		public Type type {
			get {
				return GetType();
			}
		}

		public ObjectBase() {
			this.m_id = GenerateUniqueID();
			m_objects[this.m_id] = this;
		}

		public ObjectBase(string name) {
			this.name = name;
			this.m_id = GenerateUniqueID();
			m_objects[this.m_id] = this;
		}

		/// <summary>
		/// Changes the Object id to avoid instance errors
		/// </summary>
		public int ChangeID() {
			this.m_id = GenerateUniqueID();
			m_objects[m_id] = this;
			return this.m_id;
		}

		/// <summary>
		/// Calls the method named method_name on this object.
		/// </summary>
		public object SendMessage(string method_name, params object[] args) {
			if (m_methods.Count == 0) {
				m_methods = new Dictionary<string, List<MethodInfo>>();
				MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
				foreach (MethodInfo method in methods) {
					List<MethodInfo> l1;
					if (m_methods.TryGetValue(method.Name, out l1)) {
						l1.Add(method);
					}
					else {
						m_methods[method.Name] = new List<MethodInfo>() { method };
					}
				}
			}
			List<MethodInfo> l2;
			if (m_methods.TryGetValue(method_name, out l2)) {
				if (l2.Count > 0) {
					foreach (MethodInfo method in l2) {
						bool do_invoke = true;
						ParameterInfo[] parameters = method.GetParameters();
						if (args.Length == parameters.Length) {
							if (args.Length > 0 && parameters.Length > 0) {
								Type[] arg_types = args.Select(a => a.GetType()).ToArray();
								Type[] param_types = parameters.Select(p => p.ParameterType).ToArray();
								if (arg_types.Length == param_types.Length) {
									for (int id = 0; id < arg_types.Length; id++) {
										if (!param_types[id].IsAssignableFrom(arg_types[id])) {
											do_invoke = false;
											break;
										}
									}
								}
								else {
									do_invoke = false;
									continue;
								}
							}
						}
						else {
							do_invoke = false;
							continue;
						}
						if (do_invoke) {
							return method.Invoke(this, args);
						}
					}
				}
			}
			return null;
		}

		/// <summary>
		/// Returns the instance id of the object.
		/// </summary>
		public int GetInstanceID() {
			return m_id;
		}

		/// <summary>
		/// Generates a unique id.
		/// </summary>
		public static int GenerateUniqueID() {
			int id = int.MaxValue + (int.MinValue / (2 + clock++)) - (clock - 1000 * DateTime.Now.Millisecond * DateTime.Now.Minute * clock + System.DateTime.Now.Second - System.DateTime.Now.Day - clock + System.DateTime.Now.Year);
			if (m_ids.Count < m_objects.Count) {
				m_ids = m_objects.Select(o => o.Value.id).ToList();
			}
			if (m_ids.Contains(id)) {
				return GenerateUniqueID();
			}
			m_ids.Add(id);
			return id;
		}

		/// <summary>
		/// Translates an instance ID to a reference to an object.
		/// </summary>
		public static ObjectBase InstanceIDToObject(int id) {
			ObjectBase o1;
			if (m_objects.TryGetValue(id, out o1)) {
				return o1;
			}
			return m_objects.Values.First(o2 => o2.m_id == id);
		}

		public static List<ObjectBase> GetAllInstances() {
			return m_objects.Values.ToList();
		}

		/// <summary>
		/// Creates an instance of a ObjectBase.
		/// </summary>
		public static T CreateInstance<T>(params object[] args) where T : ObjectBase {
			return (T)CreateInstance(typeof(T), args);
		}

		/// <summary>
		/// Creates a generic instance based on "generic_type" using the "type_argument" argument
		/// </summary>
		public static ObjectBase CreateGenericInstance(Type generic_type, Type type_argument, params object[] args) {
			return CreateInstance(generic_type.MakeGenericType(type_argument), args);
		}

		/// <summary>
		/// Creates a generic instance based on "generic_type" using the "type_args" arguments
		/// </summary>
		public static ObjectBase CreateGenericInstance(Type generic_type, Type[] type_args, params object[] args) {
			return CreateInstance(generic_type.MakeGenericType(type_args), args);
		}

		/// <summary>
		/// The type of the ObjectBase to create, as a System.Type instance
		/// </summary>
		public static ObjectBase CreateInstance(Type type, params object[] args) {
			ObjectBase instance = (ObjectBase)Activator.CreateInstance(type, args);
			instance.m_id = GenerateUniqueID();
			return m_objects[instance.m_id] = instance;
		}

		public override int GetHashCode() {
			return m_id;
		}

		/// <summary>
		/// Compares two object references to see if they refer to the same object.
		/// </summary>
		public override bool Equals(object other) {
			ObjectBase rhs = other as ObjectBase;
			if (((rhs == null) && (other != null)) && !(other is ObjectBase)) {
				return false;
			}
			return this.id == rhs.id;
		}

		/// <summary>
		/// Does the object exist?
		/// </summary>
		public static implicit operator bool(ObjectBase exists) {
			return !Equals(exists, null);
		}

		/// <summary>
		/// Compares two object references to see if they refer to the same object.
		/// </summary>
		public static bool CompareBaseObjects(ObjectBase x, ObjectBase y) {
			if (x && y) {
				return x.id == y.id;
			}
			if (!x && !y) {
				return true;
			}
			return false;
		}

		/// <summary>
		/// Compares two object references to see if they refer to the same object.
		/// </summary>
		public static bool operator ==(ObjectBase x, ObjectBase y) {
			return CompareBaseObjects(x, y);
		}

		/// <summary>
		/// Compares if two objects refer to a different object.
		/// </summary>
		public static bool operator !=(ObjectBase x, ObjectBase y) {
			return !CompareBaseObjects(x, y);
		}
	}
}
