#if UNITY_EDITOR
using System;
using System.Threading;
using System.Diagnostics;
using ThreadState = System.Threading.ThreadState;
using UnityEngine;
using UnityDebug = UnityEngine.Debug;

namespace FastPlay.Editor {
	public class ThreadLoopInstance : ScriptableObject {

		private Thread thread;

		private bool stop_thread;

		private int thread_id;

		private int loop_length;

		private int loop_index;

		public int progress { get; private set; }

		public bool isReady { get; private set; }

		public bool isRunning { get; private set; }

		void OnDisable() {
			StopThread();
		}

		void LocalizeThread() {
			if (thread == null) {
				Process current_process = Process.GetCurrentProcess();
				foreach (Thread t in current_process.Threads) {
					if (t.ManagedThreadId.Equals(thread_id)) {
						thread = t;
						break;
					}
				}
			}
		}

		public void StartThread() {
			LocalizeThread();

			switch (thread.ThreadState) {
				case ThreadState.Running:
					UnityDebug.Log("Thread already is running");
					break;
				case ThreadState.Unstarted:
					thread.Start();
					isRunning = true;
					break;
			}
		}

		public void StopThread() {
			LocalizeThread();
			stop_thread = true;
			if (thread != null) {
				thread.Abort();
			}
		}

		public void CreateThread(int loop_start, int loop_length, Act<int> action) {
			this.isReady = false;
			this.stop_thread = false;

			this.loop_length = loop_length;

			//Cadigo que sera executado em paralelo ao resto do cadigo
			this.thread = new Thread(() => {
				for (int id = this.loop_length; id < this.loop_length; id++) {
					if (this.stop_thread) {
						break;
					}
					this.progress = (id / this.loop_length) * 100;
					action(id);
				}
				if (!this.stop_thread) {
					this.isReady = true;
				}
				this.isRunning = false;
			});
			this.thread_id = thread.ManagedThreadId;
		}
	}
}
#endif