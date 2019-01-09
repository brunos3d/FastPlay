#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Linq;
using System.Threading;
using System.Reflection;
using System.Collections.Generic;
using FastPlay.Runtime;
using System.Diagnostics;
using ThreadState = System.Threading.ThreadState;
using Debug = UnityEngine.Debug;

namespace FastPlay.Editor {
	public class ThreadLoopInstance : ScriptableObject {

		private Thread thread;

		private bool stop_thread;

		public int thread_id;

		[NonSerialized]
		public int variable_count;

		[NonSerialized]
		public ContextItem context;

		[NonSerialized]
		public bool is_ready;

		[NonSerialized]
		public int loop_length;

		[NonSerialized]
		public int loop_index;

		[NonSerialized]
		public string thread_info;

		[NonSerialized]
		public Vector2 spawn_pos;

		void OnDisable() {
			StopThread();
		}

		public void StartThread() {
			if (thread == null) {
				Process current_process = Process.GetCurrentProcess();
				foreach (Thread t in current_process.Threads) {
					if (t.ManagedThreadId.Equals(thread_id)) {
						thread = t;
						break;
					}
				}
				StopThread();
				CreateThread();
				thread.Start();
			}
			else {
				switch (thread.ThreadState) {
					case ThreadState.Running:
						//Debug.Log("Thread already is running");
						break;
					case ThreadState.Unstarted:
						thread.Start();
						break;
					case ThreadState.Stopped:
						if (!is_ready) {
							CreateThread();
							thread.Start();
						}
						break;
				}
			}
		}

		public void StopThread() {
			stop_thread = true;
			if (thread != null) {
				thread.Abort();
			}
		}

		public void CreateThread() {
			is_ready = false;
			stop_thread = false;
			context = new ContextItem();
			List<Parameter> var_parameters = GraphEditor.graph.variableParameters;
			List<Type> built_in_nodes = typeof(Node).Assembly.GetTypes().Where(t => typeof(Node).IsAssignableFrom(t) && !t.IsAbstract && !t.HasAttribute<HideInListAttribute>(false)).ToList();
			List<Type> current_types = EditorHandler.GetConstantTypesCurrentInstance().current_types;
			Dictionary<Type, Texture> icons = new Dictionary<Type, Texture>();
			var_parameters.ForEach(p => icons[p.valueType] = GUIReferrer.GetTypeIcon(p.valueType));
			current_types.ForEach(t => icons[t] = GUIReferrer.GetTypeIcon(t));
			built_in_nodes.ForEach(t => icons[t] = GUIReferrer.GetTypeIcon(t));

			loop_index = 0;
			thread_info = string.Empty;
			loop_length = var_parameters.Count + built_in_nodes.Count + current_types.Count;

			//Código que será executado em paralelo ao resto do código
			thread = new Thread(() => {
				is_ready = false;
				variable_count = var_parameters.Count;
				foreach (Parameter param in var_parameters) {
					if (stop_thread) {
						//Debug.Log("Bye");
						stop_thread = false;
						return;
					}
					else {
						//Debug.Log("Run");
					}
					loop_index++;
					Texture icon = icons[param.valueType];
					thread_info = string.Format("{0} ({1}%)", param.name, Mathf.CeilToInt(((float)loop_index / loop_length) * 100));

					object[] args = new object[] { typeof(VariableNode<>).MakeGenericType(param.valueType), param };
					context.AddItem(new GUIContent(string.Format("Local Variables/{0} : {1}", param.name, param.valueType.GetTypeName()), icon), AddCustomNode, args);
				}

				foreach (Type type in built_in_nodes) {
					if (stop_thread) {
						//Debug.Log("Bye");
						stop_thread = false;
						return;
					}
					else {
						//Debug.Log("Run");
					}
					loop_index++;
					Texture icon = icons[type];
					string type_name = type.GetTypeName();
					string path = type_name;
					PathAttribute path_attribute = type.GetAttribute<PathAttribute>(false);

					thread_info = string.Format("{0} ({1}%)", type_name, Mathf.CeilToInt(((float)loop_index / loop_length) * 100));

					if (type.IsGenericType) {
						foreach (Type t in current_types) {
							Type type_gen = type.MakeGenericType(t);
							string type_gen_name = type_gen.GetTypeName();
							if (path_attribute == null) {
								if (typeof(ActionNode).IsAssignableFrom(type_gen) || typeof(ValueNode).IsAssignableFrom(type_gen)) {
									path = "Actions/" + type_gen_name;
								}
								else if (typeof(EventNode).IsAssignableFrom(type_gen)) {
									path = "Events/" + type_gen_name;
								}
								else {
									path = "Others/" + type_gen_name;
								}
							}
							else {
								path = string.Format("{0}/{1}", path_attribute.path, type_gen.GetTypeName(false, true));
							}
							if (type_gen.HasAttribute<BuiltInNodeAttribute>(false)) {
								context.AddItem(new GUIContent(path, icon), AddNode, type_gen);
							}
							else {
								context.AddItem(new GUIContent("References/" + path, icon), AddNode, type_gen);
							}
						}
					}
					else {
						if (path_attribute == null) {
							if (typeof(ActionNode).IsAssignableFrom(type) || typeof(ValueNode).IsAssignableFrom(type)) {
								path = "Actions/" + type_name;
							}
							else if (typeof(EventNode).IsAssignableFrom(type)) {
								path = "Events/" + type_name;
							}
							else {
								path = "Others/" + type_name;
							}
						}
						else {
							path = path_attribute.path;
						}
						if (type.HasAttribute<BuiltInNodeAttribute>(false)) {
							context.AddItem(new GUIContent(path, icon), AddNode, type);
						}
						else {
							context.AddItem(new GUIContent("References/" + path, icon), AddNode, type);
						}
					}
				}

				//reflected nodes
				foreach (Type type in current_types) {
					if (stop_thread) {
						//Debug.Log("Bye");
						stop_thread = false;
						return;
					}
					else {
						//Debug.Log("Run");
					}
					loop_index++;

					Texture icon = icons[type];
					string type_name = type.GetTypeName();
					thread_info = string.Format("{0} ({1}%)", type_name, Mathf.CeilToInt(((float)loop_index / loop_length) * 100));
					if (type.IsGenericType) {
						foreach (Type t in current_types) {
							Type type_gen = type.MakeGenericType(t);
							string type_gen_name = type_gen.GetTypeName();
							thread_info = string.Format("{0} ({1}%)", type_gen_name, Mathf.CeilToInt(((float)loop_index / loop_length) * 100));

							MethodInfo[] methods = type_gen.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static).Where(m => m.GetGenericArguments().Length <= 1).ToArray();
							//methods.Where(m => m.IsSpecialName == false && m.DeclaringType != type)
							//methods.Where(m => m.IsSpecialName)
							//methods.Where(m => m.IsSpecialName == false && m.DeclaringType == type)
							foreach (MethodInfo method in methods.Where(m => m.IsSpecialName == false && m.DeclaringType != type_gen)) {
								context.AddItem(new GUIContent(string.Format("Codebase/{0}/{1}/Inherited/{2}", type_name, type_gen_name, method.GetSignName()), icon), AddReflectedNode, method);
							}
							foreach (MethodInfo method in methods.Where(m => m.IsSpecialName)) {
								context.AddItem(new GUIContent(string.Format("Codebase/{0}/{1}/Properties/{2}", type_name, type_gen_name, method.GetSignName()), icon), AddReflectedNode, method);
							}

							//Literal Nodes
							if (!(type_gen.IsAbstract && type_gen.IsSealed)) {
								Type literal_node_type = typeof(LiteralNode<>).MakeGenericType(type_gen);
								context.AddItem(new GUIContent(string.Format("Codebase/{0}/{1}/Literal {1}", type_name, type_gen_name), icon), AddNode, literal_node_type);
							}

							foreach (MethodInfo method in methods.Where(m => m.IsSpecialName == false && m.DeclaringType == type_gen)) {
								context.AddItem(new GUIContent(string.Format("Codebase/{0}/{1}/{2}", type_name, type_gen_name, method.GetSignName()), icon), AddReflectedNode, method);
							}
						}
					}
					else {
						MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static).Where(m => m.GetGenericArguments().Length <= 1).ToArray();
						//methods.Where(m => m.IsSpecialName == false && m.DeclaringType != type)
						//methods.Where(m => m.IsSpecialName)
						//methods.Where(m => m.IsSpecialName == false && m.DeclaringType == type)
						foreach (MethodInfo method in methods.Where(m => m.IsSpecialName == false && m.DeclaringType != type)) {
							if (method.IsGenericMethod) {
								foreach (Type t in current_types) {
									MethodInfo method_gen = method.MakeGenericMethod(t);
									object[] args = new object[] { method_gen, t };
									context.AddItem(new GUIContent(string.Format("Codebase/{0}/Inherited/{1}/{2}", type_name, method.GetSignName(), method_gen.GetSignName()), icon), AddReflectedGenericNode, args);
								}
							}
							else {
								context.AddItem(new GUIContent(string.Format("Codebase/{0}/Inherited/{1}", type_name, method.GetSignName()), icon), AddReflectedNode, method);
							}
						}
						foreach (MethodInfo method in methods.Where(m => m.IsSpecialName)) {
							if (method.IsGenericMethod) {
								foreach (Type t in current_types) {
									MethodInfo method_gen = method.MakeGenericMethod(t);
									object[] args = new object[] { method_gen, t };
									context.AddItem(new GUIContent(string.Format("Codebase/{0}/Properties/{1}/{2}", type_name, method.GetSignName(), method_gen.GetSignName()), icon), AddReflectedGenericNode, args);
								}
							}
							else {
								context.AddItem(new GUIContent(string.Format("Codebase/{0}/Properties/{1}", type_name, method.GetSignName()), icon), AddReflectedNode, method);
							}
						}

						//Literal Nodes
						if (!(type.IsAbstract && type.IsSealed)) {
							Type literal_node_type = typeof(LiteralNode<>).MakeGenericType(type);
							context.AddItem(new GUIContent(string.Format("Codebase/{0}/Literal {0}", type_name), icon), AddNode, literal_node_type);
						}

						foreach (MethodInfo method in methods.Where(m => m.IsSpecialName == false && m.DeclaringType == type)) {
							if (method.IsGenericMethod) {
								foreach (Type t in current_types) {
									MethodInfo method_gen = method.MakeGenericMethod(t);
									object[] args = new object[] { method_gen, t };
									context.AddItem(new GUIContent(string.Format("Codebase/{0}/{1}/{2}", type_name, method.GetSignName(), method_gen.GetSignName()), icon), AddReflectedGenericNode, args);
								}
							}
							else {
								context.AddItem(new GUIContent(string.Format("Codebase/{0}/{1}", type_name, method.GetSignName()), icon), AddReflectedNode, method);
							}
						}
					}
				}
				is_ready = true;
			});
			thread_id = thread.ManagedThreadId;
		}

		public void AddNode(object obj) {
			Node instance = GraphEditorWindow.AddNode((Type)obj, this.spawn_pos - GraphEditor.scroll);
			instance.Validate();
			instance.OnGraphAdd();
		}

		public void AddCustomNode(object obj) {
			object[] args = (object[])obj;
			Node instance = GraphEditorWindow.AddCustomNode((Type)args[0], this.spawn_pos - GraphEditor.scroll, args[1]);
			instance.Validate();
			instance.OnGraphAdd();
		}

		public void AddReflectedNode(object obj) {
			ReflectedNode instance = GraphEditorWindow.AddNode<ReflectedNode>(this.spawn_pos - GraphEditor.scroll, false);
			instance.SetMethod((MethodInfo)obj);
			instance.Validate();
			instance.OnGraphAdd();
		}

		public void AddReflectedGenericNode(object obj) {
			ReflectedNode instance = GraphEditorWindow.AddNode<ReflectedNode>(this.spawn_pos - GraphEditor.scroll, false);
			object[] args = (object[])obj;
			instance.SetMethod((MethodInfo)args[0], (Type)args[1]);
			instance.Validate();
			instance.OnGraphAdd();
		}
	}
}
#endif