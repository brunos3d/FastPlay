using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FastPlay.Runtime {
	// GraphController MonoBehaviour Events Manager
	public partial class GraphController {

		#region Coroutines

		public List<string> coroutines = new List<string>();

		public void StopAllCoroutines2() {
			foreach (string coroutine in coroutines) {
				StopCoroutine(coroutine);
			}
			StopAllCoroutines();
		}

		public void WaitForNextFrameToCall(Act action) {
			coroutines.Add(action.Method.Name);
			StartCoroutine(CoroutineAction(action, WaitFor.NextFrame()));
		}

		public void WaitForFramesToCall(Act action, int frameCount) {
			coroutines.Add(action.Method.Name);
			StartCoroutine(CoroutineAction(action, WaitFor.Frames(frameCount)));
		}

		public void WaitForSecondsToCall(Act action, float seconds) {
			coroutines.Add(action.Method.Name);
			StartCoroutine(CoroutineWaitForSeconds(action, seconds));
		}

		private IEnumerator CoroutineAction(Act action, IEnumerator coroutine) {
			yield return StartCoroutine(coroutine);
			action();
		}

		private IEnumerator CoroutineWaitForSeconds(Act action, float seconds) {
			yield return new WaitForSeconds(seconds);
			action();
		}

		#endregion

		#region Functions

		public void SetThisCurrent() {
#if UNITY_EDITOR
			if (!Application.isPlaying) return;
#endif
			Current.SetCurrent(this);
		}

		public void ClearEvents() {
			DoAnimatorIK = null;
			DoAnimatorMove = null;
			DoApplicationFocus = null;
			DoApplicationPause = null;
			DoApplicationQuit = null;
			DoAudioFilterRead = null;
			DoBecameInvisible = null;
			DoBecameVisible = null;
			DoBeforeTransformParentChanged = null;
			DoCanvasGroupChanged = null;
			DoCollisionEnter = null;
			DoCollisionEnter2D = null;
			DoCollisionExit = null;
			DoCollisionExit2D = null;
			DoCollisionStay = null;
			DoCollisionStay2D = null;
			DoControllerColliderHit = null;
			DoDestroy = null;
			DoFixedUpdate = null;
			DoGUI = null;
			DoJointBreak = null;
			DoJointBreak2D = null;
			DoLateUpdate = null;
			DoMouseDown = null;
			DoMouseDrag = null;
			DoMouseEnter = null;
			DoMouseExit = null;
			DoMouseOver = null;
			DoMouseUp = null;
			DoParticleCollision = null;
			DoParticleTrigger = null;
			DoPostRender = null;
			DoPreCull = null;
			DoPreRender = null;
			DoRectTransformDimensionsChange = null;
			DoRectTransformRemoved = null;
			DoRenderImage = null;
			DoRenderObject = null;
			DoTransformChildrenChanged = null;
			DoTransformParentChanged = null;
			DoTriggerEnter = null;
			DoTriggerEnter2D = null;
			DoTriggerExit = null;
			DoTriggerExit2D = null;
			DoTriggerStay = null;
			DoTriggerStay2D = null;
			DoUpdate = null;
			DoWillRenderObject = null;
			DoMouseUpAsButton = null;

#if !(UNITY_2018 || UNITY_2018_OR_NEWER || UNITY_2018_2)
			DoServerInitialized = null;
			DoConnectedToServer = null;
			DoDisconnectedFromMasterServer = null;
			DoDisconnectedFromServer = null;
			DoFailedToConnect = null;
			DoFailedToConnectToMasterServer = null;
			DoMasterServerEvent = null;
			DoNetworkInstantiate = null;
			DoPlayerConnected = null;
			DoPlayerDisconnected = null;
			DoSerializeNetworkView = null;
#endif
		}

		#endregion

		#region MonoBehaviour

		public event Action DoEnable;

		public event Action DoDisable;

		public event Action DoDestroy;

		public event Action DoStart;

		public event Action DoUpdate;

		public event Action DoFixedUpdate;

		public event Action DoLateUpdate;

		public bool destroying { get; private set; }

		public void OnEnable() {
#if UNITY_EDITOR
			if (!Application.isPlaying) return;
#endif
			ClearEvents();
			OnControllerEnable();
			if (DoEnable != null) {
				SetThisCurrent();
				DoEnable();
			}
		}

		public void OnDisable() {
#if UNITY_EDITOR
			if (!Application.isPlaying) return;
#endif
			if (DoDisable != null) {
				SetThisCurrent();
				DoDisable();
			}
			OnControllerDisable();
		}

		public void OnDestroy() {
#if UNITY_EDITOR
			if (!Application.isPlaying) return;
#endif
			destroying = true;
			if (DoDestroy != null) {
				SetThisCurrent();
				DoDestroy();
			}
			OnControllerDestroy();
			ClearEvents();
		}

		public void Start() {
#if UNITY_EDITOR
			if (!Application.isPlaying) return;
#endif
			OnControllerStart();
			if (DoStart != null) {
				SetThisCurrent();
				DoStart();
			}
		}

		public void Update() {
#if UNITY_EDITOR
			if (!Application.isPlaying) return;
#endif
			OnControllerUpdate();
			if (DoUpdate != null) {
				SetThisCurrent();
				DoUpdate();
			}
		}

		public void FixedUpdate() {
#if UNITY_EDITOR
			if (!Application.isPlaying) return;
#endif
			if (DoFixedUpdate != null) {
				SetThisCurrent();
				DoFixedUpdate();
			}
		}

		public void LateUpdate() {
#if UNITY_EDITOR
			if (!Application.isPlaying) return;
#endif
			if (DoLateUpdate != null) {
				SetThisCurrent();
				DoLateUpdate();
			}
		}

		public event Action<int> DoAnimatorIK;

		public event Action DoAnimatorMove;

		public event Action<bool> DoApplicationFocus;

		public event Action<bool> DoApplicationPause;

		public event Action DoApplicationQuit;

		public event Action<float[], int> DoAudioFilterRead;

		public event Action DoBecameInvisible;

		public event Action DoBecameVisible;

		public event Action DoBeforeTransformParentChanged;

		public event Action DoCanvasGroupChanged;

		public event Action<Collision> DoCollisionEnter;

		public event Action<Collision2D> DoCollisionEnter2D;

		public event Action<Collision> DoCollisionExit;

		public event Action<Collision2D> DoCollisionExit2D;

		public event Action<Collision> DoCollisionStay;

		public event Action<Collision2D> DoCollisionStay2D;

		public event Action<ControllerColliderHit> DoControllerColliderHit;

		public event Action DoGUI;

		public event Action<float> DoJointBreak;

		public event Action<Joint2D> DoJointBreak2D;

		public event Action DoMouseDown;

		public event Action DoMouseDrag;

		public event Action DoMouseEnter;

		public event Action DoMouseExit;

		public event Action DoMouseOver;

		public event Action DoMouseUp;

		public event Action DoMouseUpAsButton;

		public event Action<GameObject> DoParticleCollision;

		public event Action DoParticleTrigger;

		public event Action DoPostRender;

		public event Action DoPreCull;

		public event Action DoPreRender;

		public event Action DoRectTransformDimensionsChange;

		public event Action DoRectTransformRemoved;

		public event Action<RenderTexture, RenderTexture> DoRenderImage;

		public event Action DoRenderObject;

		public event Action DoTransformChildrenChanged;

		public event Action DoTransformParentChanged;

		public event Action<Collider> DoTriggerEnter;

		public event Action<Collider2D> DoTriggerEnter2D;

		public event Action<Collider> DoTriggerExit;

		public event Action<Collider2D> DoTriggerExit2D;

		public event Action<Collider> DoTriggerStay;

		public event Action<Collider2D> DoTriggerStay2D;

		public event Action DoWillRenderObject;

#if !(UNITY_2018_OR_NEWER || UNITY_2018 || UNITY_2018_2)

		public event Action DoServerInitialized;

		public event Action DoConnectedToServer;

		public event Action<NetworkDisconnection> DoDisconnectedFromMasterServer;

		public event Action<NetworkDisconnection> DoDisconnectedFromServer;

		public event Action<NetworkConnectionError> DoFailedToConnect;

		public event Action<NetworkConnectionError> DoFailedToConnectToMasterServer;

		public event Action<MasterServerEvent> DoMasterServerEvent;

		public event Action<NetworkMessageInfo> DoNetworkInstantiate;

		public event Action<NetworkPlayer> DoPlayerConnected;

		public event Action<NetworkPlayer> DoPlayerDisconnected;

		public event Action<BitStream, NetworkMessageInfo> DoSerializeNetworkView;

		public void OnServerInitialized() {
#if UNITY_EDITOR
			if (!Application.isPlaying) return;
#endif
			if (DoServerInitialized != null) {
				SetThisCurrent();
				DoServerInitialized();
			}
		}

		public void OnConnectedToServer() {
#if UNITY_EDITOR
			if (!Application.isPlaying) return;
#endif
			if (DoConnectedToServer != null) {
				SetThisCurrent();
				DoConnectedToServer();
			}
		}

		public void OnDisconnectedFromMasterServer(NetworkDisconnection info) {
#if UNITY_EDITOR
			if (!Application.isPlaying) return;
#endif
			if (DoDisconnectedFromMasterServer != null) {
				SetThisCurrent();
				DoDisconnectedFromMasterServer(info);
			}
		}

		public void OnDisconnectedFromServer(NetworkDisconnection info) {
#if UNITY_EDITOR
			if (!Application.isPlaying) return;
#endif
			if (DoDisconnectedFromServer != null) {
				SetThisCurrent();
				DoDisconnectedFromServer(info);
			}
		}

		public void OnFailedToConnect(NetworkConnectionError error) {
#if UNITY_EDITOR
			if (!Application.isPlaying) return;
#endif
			if (DoFailedToConnect != null) {
				SetThisCurrent();
				DoFailedToConnect(error);
			}
		}

		public void OnFailedToConnectToMasterServer(NetworkConnectionError error) {
#if UNITY_EDITOR
			if (!Application.isPlaying) return;
#endif
			if (DoFailedToConnectToMasterServer != null) {
				SetThisCurrent();
				DoFailedToConnectToMasterServer(error);
			}
		}

		public void OnMasterServerEvent(MasterServerEvent msEvent) {
#if UNITY_EDITOR
			if (!Application.isPlaying) return;
#endif
			if (DoMasterServerEvent != null) {
				SetThisCurrent();
				DoMasterServerEvent(msEvent);
			}
		}

		public void OnNetworkInstantiate(NetworkMessageInfo info) {
#if UNITY_EDITOR
			if (!Application.isPlaying) return;
#endif
			if (DoNetworkInstantiate != null) {
				SetThisCurrent();
				DoNetworkInstantiate(info);
			}
		}

		public void OnPlayerConnected(NetworkPlayer player) {
#if UNITY_EDITOR
			if (!Application.isPlaying) return;
#endif
			if (DoPlayerConnected != null) {
				SetThisCurrent();
				DoPlayerConnected(player);
			}
		}

		public void OnPlayerDisconnected(NetworkPlayer player) {
#if UNITY_EDITOR
			if (!Application.isPlaying) return;
#endif
			if (DoPlayerDisconnected != null) {
				SetThisCurrent();
				DoPlayerDisconnected(player);
			}
		}

		public void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
#if UNITY_EDITOR
			if (!Application.isPlaying) return;
#endif
			if (DoSerializeNetworkView != null) {
				SetThisCurrent();
				DoSerializeNetworkView(stream, info);
			}
		}
#endif


		public void OnAnimatorIK(int layerIndex) {
#if UNITY_EDITOR
			if (!Application.isPlaying) return;
#endif
			if (DoAnimatorIK != null) {
				SetThisCurrent();
				DoAnimatorIK(layerIndex);
			}
		}

		public void OnAnimatorMove() {
#if UNITY_EDITOR
			if (!Application.isPlaying) return;
#endif
			if (DoAnimatorMove != null) {
				SetThisCurrent();
				DoAnimatorMove();
			}
		}

		public void OnApplicationFocus(bool focus) {
#if UNITY_EDITOR
			if (!Application.isPlaying) return;
#endif
			if (DoApplicationFocus != null) {
				SetThisCurrent();
				DoApplicationFocus(focus);
			}
		}

		public void OnApplicationPause(bool pause) {
#if UNITY_EDITOR
			if (!Application.isPlaying) return;
#endif
			if (DoApplicationPause != null) {
				SetThisCurrent();
				DoApplicationPause(pause);
			}
		}

		public void OnApplicationQuit() {
#if UNITY_EDITOR
			if (!Application.isPlaying) return;
#endif
			if (DoApplicationQuit != null) {
				SetThisCurrent();
				DoApplicationQuit();
			}
			ClearEvents();
		}

		public void OnAudioFilterRead(float[] data, int channels) {
			/*
#if UNITY_EDITOR
			if (!Application.isPlaying) return;
#endif
			*/
			if (DoAudioFilterRead != null) {
				SetThisCurrent();
				DoAudioFilterRead(data, channels);
			}
		}

		public void OnBecameInvisible() {
#if UNITY_EDITOR
			if (!Application.isPlaying) return;
#endif
			if (DoBecameInvisible != null) {
				SetThisCurrent();
				DoBecameInvisible();
			}
		}

		public void OnBecameVisible() {
#if UNITY_EDITOR
			//if (!Application.isPlaying) return;
#endif
			if (DoBecameVisible != null) {
				SetThisCurrent();
				DoBecameVisible();
			}
		}

		public void OnBeforeTransformParentChanged() {
#if UNITY_EDITOR
			if (!Application.isPlaying) return;
#endif
			if (DoBeforeTransformParentChanged != null) {
				SetThisCurrent();
				DoBeforeTransformParentChanged();
			}
		}

		public void OnCanvasGroupChanged() {
#if UNITY_EDITOR
			if (!Application.isPlaying) return;
#endif
			if (DoCanvasGroupChanged != null) {
				SetThisCurrent();
				DoCanvasGroupChanged();
			}
		}

		public void OnCollisionEnter(Collision collision) {
#if UNITY_EDITOR
			if (!Application.isPlaying) return;
#endif
			if (DoCollisionEnter != null) {
				SetThisCurrent();
				DoCollisionEnter(collision);
			}
		}

		public void OnCollisionEnter2D(Collision2D collision) {
#if UNITY_EDITOR
			if (!Application.isPlaying) return;
#endif
			if (DoCollisionEnter2D != null) {
				SetThisCurrent();
				DoCollisionEnter2D(collision);
			}
		}

		public void OnCollisionExit(Collision collision) {
#if UNITY_EDITOR
			if (!Application.isPlaying) return;
#endif
			if (DoCollisionExit != null) {
				SetThisCurrent();
				DoCollisionExit(collision);
			}
		}

		public void OnCollisionExit2D(Collision2D collision) {
#if UNITY_EDITOR
			if (!Application.isPlaying) return;
#endif
			if (DoCollisionExit2D != null) {
				SetThisCurrent();
				DoCollisionExit2D(collision);
			}
		}

		public void OnCollisionStay(Collision collision) {
#if UNITY_EDITOR
			if (!Application.isPlaying) return;
#endif
			if (DoCollisionStay != null) {
				SetThisCurrent();
				DoCollisionStay(collision);
			}
		}

		public void OnCollisionStay2D(Collision2D collision) {
#if UNITY_EDITOR
			if (!Application.isPlaying) return;
#endif
			if (DoCollisionStay2D != null) {
				SetThisCurrent();
				DoCollisionStay2D(collision);
			}
		}

		public void OnControllerColliderHit(ControllerColliderHit hit) {
#if UNITY_EDITOR
			if (!Application.isPlaying) return;
#endif
			if (DoControllerColliderHit != null) {
				SetThisCurrent();
				DoControllerColliderHit(hit);
			}
		}

		public void OnGUI() {
#if UNITY_EDITOR
			if (!Application.isPlaying) return;
#endif
			if (DoGUI != null) {
				SetThisCurrent();
				DoGUI();
			}
		}
		public void OnJointBreak(float breakForce) {
#if UNITY_EDITOR
			if (!Application.isPlaying) return;
#endif
			if (DoJointBreak != null) {
				SetThisCurrent();
				DoJointBreak(breakForce);
			}
		}

		public void OnJointBreak2D(Joint2D joint) {
#if UNITY_EDITOR
			if (!Application.isPlaying) return;
#endif
			if (DoJointBreak2D != null) {
				SetThisCurrent();
				DoJointBreak2D(joint);
			}
		}

		public void OnMouseDown() {
#if UNITY_EDITOR
			if (!Application.isPlaying) return;
#endif
			if (DoMouseDown != null) {
				SetThisCurrent();
				DoMouseDown();
			}
		}

		public void OnMouseDrag() {
#if UNITY_EDITOR
			if (!Application.isPlaying) return;
#endif
			if (DoMouseDrag != null) {
				SetThisCurrent();
				DoMouseDrag();
			}
		}

		public void OnMouseEnter() {
#if UNITY_EDITOR
			if (!Application.isPlaying) return;
#endif
			if (DoMouseEnter != null) {
				SetThisCurrent();
				DoMouseEnter();
			}
		}

		public void OnMouseExit() {
#if UNITY_EDITOR
			if (!Application.isPlaying) return;
#endif
			if (DoMouseExit != null) {
				SetThisCurrent();
				DoMouseExit();
			}
		}

		public void OnMouseOver() {
#if UNITY_EDITOR
			if (!Application.isPlaying) return;
#endif
			if (DoMouseOver != null) {
				SetThisCurrent();
				DoMouseOver();
			}
		}

		public void OnMouseUp() {
#if UNITY_EDITOR
			if (!Application.isPlaying) return;
#endif
			if (DoMouseUp != null) {
				SetThisCurrent();
				DoMouseUp();
			}
		}

		public void OnMouseUpAsButton() {
#if UNITY_EDITOR
			if (!Application.isPlaying) return;
#endif
			if (DoMouseUpAsButton != null) {
				SetThisCurrent();
				DoMouseUpAsButton();
			}
		}

		public void OnParticleCollision(GameObject other) {
#if UNITY_EDITOR
			if (!Application.isPlaying) return;
#endif
			if (DoParticleCollision != null) {
				SetThisCurrent();
				DoParticleCollision(other);
			}
		}

		public void OnParticleTrigger() {
#if UNITY_EDITOR
			if (!Application.isPlaying) return;
#endif
			if (DoParticleTrigger != null) {
				SetThisCurrent();
				DoParticleTrigger();
			}
		}

		public void OnPostRender() {
#if UNITY_EDITOR
			if (!Application.isPlaying) return;
#endif
			if (DoPostRender != null) {
				SetThisCurrent();
				DoPostRender();
			}
		}

		public void OnPreCull() {
#if UNITY_EDITOR
			if (!Application.isPlaying) return;
#endif
			if (DoPreCull != null) {
				SetThisCurrent();
				DoPreCull();
			}
		}

		public void OnPreRender() {
#if UNITY_EDITOR
			if (!Application.isPlaying) return;
#endif
			if (DoPreRender != null) {
				SetThisCurrent();
				DoPreRender();
			}
		}

		public void OnRectTransformDimensionsChange() {
#if UNITY_EDITOR
			if (!Application.isPlaying) return;
#endif
			if (DoRectTransformDimensionsChange != null) {
				SetThisCurrent();
				DoRectTransformDimensionsChange();
			}
		}

		public void OnRectTransformRemoved() {
#if UNITY_EDITOR
			if (!Application.isPlaying) return;
#endif
			if (DoRectTransformRemoved != null) {
				SetThisCurrent();
				DoRectTransformRemoved();
			}
		}

		public void OnRenderImage(RenderTexture source, RenderTexture destination) {
#if UNITY_EDITOR
			if (!Application.isPlaying) return;
#endif
			if (DoRenderImage != null) {
				SetThisCurrent();
				DoRenderImage(source, destination);
			}
			else {
				Graphics.Blit(source, destination);
			}
		}

		public void OnRenderObject() {
#if UNITY_EDITOR
			if (!Application.isPlaying) return;
#endif
			if (DoRenderObject != null) {
				SetThisCurrent();
				DoRenderObject();
			}
		}

		public void OnTransformChildrenChanged() {
#if UNITY_EDITOR
			if (!Application.isPlaying) return;
#endif
			if (DoTransformChildrenChanged != null) {
				SetThisCurrent();
				DoTransformChildrenChanged();
			}
		}

		public void OnTransformParentChanged() {
#if UNITY_EDITOR
			if (!Application.isPlaying) return;
#endif
			if (DoTransformParentChanged != null) {
				SetThisCurrent();
				DoTransformParentChanged();
			}
		}

		public void OnTriggerEnter(Collider collision) {
#if UNITY_EDITOR
			if (!Application.isPlaying) return;
#endif
			if (DoTriggerEnter != null) {
				SetThisCurrent();
				DoTriggerEnter(collision);
			}
		}

		public void OnTriggerEnter2D(Collider2D collision) {
#if UNITY_EDITOR
			if (!Application.isPlaying) return;
#endif
			if (DoTriggerEnter2D != null) {
				SetThisCurrent();
				DoTriggerEnter2D(collision);
			}
		}

		public void OnTriggerExit(Collider collision) {
#if UNITY_EDITOR
			if (!Application.isPlaying) return;
#endif
			if (DoTriggerExit != null) {
				SetThisCurrent();
				DoTriggerExit(collision);
			}
		}

		public void OnTriggerExit2D(Collider2D collision) {
#if UNITY_EDITOR
			if (!Application.isPlaying) return;
#endif
			if (DoTriggerExit2D != null) {
				SetThisCurrent();
				DoTriggerExit2D(collision);
			}
		}

		public void OnTriggerStay(Collider collision) {
#if UNITY_EDITOR
			if (!Application.isPlaying) return;
#endif
			if (DoTriggerStay != null) {
				SetThisCurrent();
				DoTriggerStay(collision);
			}
		}

		public void OnTriggerStay2D(Collider2D collision) {
#if UNITY_EDITOR
			if (!Application.isPlaying) return;
#endif
			if (DoTriggerStay2D != null) {
				SetThisCurrent();
				DoTriggerStay2D(collision);
			}
		}

		public void OnWillRenderObject() {
#if UNITY_EDITOR
			if (!Application.isPlaying) return;
#endif
			if (DoWillRenderObject != null) {
				SetThisCurrent();
				DoWillRenderObject();
			}
		}

		#endregion
	}
}
