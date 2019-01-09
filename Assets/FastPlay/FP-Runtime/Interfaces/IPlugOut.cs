using System.Collections.Generic;

namespace FastPlay.Runtime {
	public interface IPlugOut : IPlug {

		List<IPlugIn> GetPluggedPorts();

		void AddPlug(IPlugIn port);

		void UnplugFrom(IPlugIn port);
	}
}
