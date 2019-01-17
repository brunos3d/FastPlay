using System;
using UnityEngine;

namespace FastPlay.Runtime {
	[BuiltInNode]
	[Title("OnRenderImage")]
	[Path("Events/Rendering/OnRenderImage")]
	[Summary("OnRenderImage is called after all rendering is complete to render image.")]
	public class OnRenderImageEvent : EventNode, IRegisterEvents, IRegisterPorts {

		public RenderTexture source;

		public RenderTexture destination;

		public void OnRegisterEvents() {
			Current.controller.DoRenderImage += OnRenderImage;
		}

		public void OnRemoveEvents() {
			Current.controller.DoRenderImage -= OnRenderImage;
		}

		public void OnRegisterPorts() {
			RegisterOutputValue<RenderTexture>("source", () => { return source; });
			RegisterOutputValue<RenderTexture>("destination", () => { return destination; });
		}

		public void OnRenderImage(RenderTexture source, RenderTexture destination) {
			this.source = source;
			this.destination = destination;
			Call(output);
		}
	}
}
