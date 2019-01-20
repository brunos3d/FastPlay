using UnityEditor;

public static class ChangeEditorSkin {

	[MenuItem("Tools/Change Skin")]
	static void Init() {
		UnityEditorInternal.InternalEditorUtility.SwitchSkinAndRepaintAllViews();
		FastPlay.Editor.FPSkin.UpdateSkin();
	}
}
