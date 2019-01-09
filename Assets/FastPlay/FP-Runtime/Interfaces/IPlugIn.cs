namespace FastPlay.Runtime {
	public interface IPlugIn : IPlug {

		IPlugOut GetPluggedPort();

		bool CanPlug(IPlugOut port, bool overwrite = true);

		void PlugTo(IPlugOut port);
	}
}
