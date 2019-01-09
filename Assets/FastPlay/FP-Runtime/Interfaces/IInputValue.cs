namespace FastPlay.Runtime {
	public interface IInputValue : IValuePort, IInputPort {

		object GetDefaultValue();

		void SetDefaultValue(object value);
	}
}
