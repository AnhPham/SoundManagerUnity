using UnityEngine;
using System.Collections;

public class SoundTest : MonoBehaviour {
	
	bool isHide;

	void Start () {
		SoundUtil.Instance.LoadScene("/top/template");
	}
	
	void OnDestroy() {
		SoundUtil.Instance.UnloadScene("/top/template");
	}
	
	void OnGUI() {
		
		if (isHide) return;
		
		if (GUI.Button(new Rect(0, 0, 300, 100), "MIGI (usually be used in this scene)")) 	SoundUtil.Instance.Play("MIGI_TEST");
		if (GUI.Button(new Rect(0, 100, 300, 100), "HIDARI (rarely be used in this scene)")) 	SoundUtil.Instance.Play("HIDARI_TEST");
		if (GUI.Button(new Rect(0, 200, 300, 100), "Random MIGI or HIDARI")) SoundUtil.Instance.PlayRandom("TEMPLATE");
		if (GUI.Button(new Rect(0, 300, 300, 100), "MIGI then wait")) {
			Debug.Log("Hide");
			isHide = true;
			SoundUtil.Instance.Play("MIGI_TEST", gameObject, "Show");
		}
		
		if (GUI.Button(new Rect(350, 0, 100, 100), "Mute")) SoundUtil.Instance.Mute = true;
		if (GUI.Button(new Rect(350, 100, 100, 100), "UnMute")) SoundUtil.Instance.Mute = false;
	}
	
	void Show() {
		Debug.Log("Show");
		isHide = false;
	}
}
