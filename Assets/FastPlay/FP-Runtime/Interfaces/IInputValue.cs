namespace FastPlay.Runtime {
	public interface IInputValue : IValuePort, IInputPort {

		GetFilter GetFilterMode();

		object GetDefaultValue();

		void SetDefaultValue(object value);
	}
}
