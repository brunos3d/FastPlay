using System.Collections;

namespace FastPlay.Runtime {
	public static class WaitFor {

		public static IEnumerator NextFrame() {
			return Frames(1);
		}

		public static IEnumerator Frames(int frameCount) {
			while (frameCount > 0) {
				frameCount--;
				yield return null;
			}
		}
	}
}
