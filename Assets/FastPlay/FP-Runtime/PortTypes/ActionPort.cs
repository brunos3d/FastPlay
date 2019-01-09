namespace FastPlay.Runtime {
	public abstract class ActionPort : Port {

		public ActionPort() { }

		public void Call() {
#if UNITY_EDITOR
			flow_state = FlowState.Active;
#endif
			OnCall();
		}

		public virtual void OnCall() { }
	}
}
