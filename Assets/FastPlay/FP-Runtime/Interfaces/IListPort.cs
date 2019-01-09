namespace FastPlay.Runtime {
	public interface IListPort {

		int portCount { get; set; }

		void AddPort();

		void RemovePort();
	}
}
