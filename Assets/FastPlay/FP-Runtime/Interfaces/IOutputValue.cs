namespace FastPlay.Runtime {
	public interface IOutputValue : IValuePort, IOutputPort {

		void SetAction(ActValue<object> action);
	}
}
