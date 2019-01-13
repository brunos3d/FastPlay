using System.Collections;

namespace FastPlay.Runtime {
	public static class WaitFor {

		public static IEnumerator NextFrame() {
			return Frames(1);
		}

		public static IEnumerator Frames(int frame_count) {
			while (frame_count > 0) {
				frame_count--;
				yield return null;
			}
		}
	}
}
