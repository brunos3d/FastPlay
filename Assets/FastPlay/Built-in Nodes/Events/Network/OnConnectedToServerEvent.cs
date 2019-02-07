#if !(UNITY_2018_OR_NEWER || UNITY_2018 || UNITY_2018_2)
namespace FastPlay.Runtime {
	[BuiltInNode]
	[Icon("connect")]
	[Title("OnConnectedToServer")]
	[Path("Events/Network/OnConnectedToServer")]
	public class OnConnectedToServerEvent : EventNode, IRegisterEvents {

		public void OnRegisterEvents() {
			Current.controller.DoConnectedToServer += OnConnectedToServer;
		}

		public void OnRemoveEvents() {
			Current.controller.DoConnectedToServer -= OnConnectedToServer;
		}

		public void OnConnectedToServer() {
			Call(output);
		}
	}
}
#endif
