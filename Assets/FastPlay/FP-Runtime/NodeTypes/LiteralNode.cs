using System;
using FastPlay.Editor;

namespace FastPlay.Runtime {
	[HideInList]
	public class LiteralNode : Node {

		public virtual Type valueType { get { return default(Type); } }

		public LiteralNode() { }

		public virtual object GetValue() { return null; }
	}
}
