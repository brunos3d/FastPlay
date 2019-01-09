using System;
using System.Collections.Generic;
using UnityEngine;

namespace FastPlay.Runtime {
	// Runtime Port 
	public abstract partial class Port : ObjectBase {

		[NonSerialized]
		public FlowState flow_state = FlowState.Idle;

		public bool display_port = true;

		public Node node;

		public Port() { }

		public virtual void Initialize() { }

		public virtual void Finish() { }
	}
}
