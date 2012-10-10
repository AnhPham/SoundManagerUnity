using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum UsedFrequent
{
	High,
	Low
}

public class ObjectCallback
{
	public GameObject gameObject;
	public string callback;
	
	public ObjectCallback(GameObject go, string cb)
	{
		gameObject = go;
		callback = cb;
	}
}

/// <summary>
/// Freq audio clip: This class contains 1 audio clip & its using frequency. 
/// If high frequency, resource load 1 time at the begining and unload only when quit application.
/// If low frequency, resource load when need to play the sound and unload after finishing to play.
/// </summary>
public class FreqAudioClip : MonoBehaviour
{
	#region Const
	private const float END_SOUND_CALLBACK_DELAY = 0.5f;
	#endregion
	
	#region Properties
	private string path;
	private AudioClip audioClip;
	private UsedFrequent freq;
	private List<ObjectCallback> targets = new List<ObjectCallback>();
	#endregion

	#region Public
	/// <summary>
	/// Gets the frequency of this sound
	/// </summary>
	/// return UsedFrequent
	/// The freq.
	/// </value>
	public UsedFrequent Freq { get { return freq; } }
	
	/// <summary>
	/// Creates the Game Object, add this script to it as 1 component.
	/// </summary>
	/// <returns>
	/// The freq audio clip.
	/// </returns>
	/// <param name='parent'>
	/// Transform Parent.
	/// </param>
	/// <param name='n'>
	/// Name
	/// </param>
	/// <param name='p'>
	/// Path
	/// </param>
	/// <param name='f'>
	/// Use Frequecy
	/// </param>
	public static FreqAudioClip CreateFreqAudioClip(Transform parent, string n, string p, UsedFrequent f)
	{
		GameObject go = new GameObject(n);
		go.transform.parent = parent;
		go.transform.localPosition = new Vector3(0f, 0f, 0f);
		FreqAudioClip fac = go.AddComponent<FreqAudioClip>();
		fac.Set(n, p, f);
		return fac;	
	}
	
	public void Play(float volume, float pitch, GameObject target, string endCallback)
	{
		if (target != null && !string.IsNullOrEmpty(endCallback)) {
			targets.Add(new ObjectCallback(target, endCallback));
		} else {
			targets.Add(new ObjectCallback(null, string.Empty));
		}
		
		CancelInvoke("UnloadClip");
		LoadClip();
		
		if (audioClip != null){
			if (Debug.isDebugBuild) Debug.Log("[Play Clip] [OK] " + path);
			SoundTool.PlaySound(audioClip, volume, pitch);
			
			if (freq == UsedFrequent.Low) {
				Invoke("UnloadClip", audioClip.length + END_SOUND_CALLBACK_DELAY);
			} else if (freq == UsedFrequent.High) {
				Invoke("CallBack", audioClip.length + END_SOUND_CALLBACK_DELAY);
			}
		}
	}
	
	public void Play(GameObject target, string endCallback)
	{
		if (target != null && !string.IsNullOrEmpty(endCallback)) targets.Add(new ObjectCallback(target, endCallback));
		CallBack();
	}
	
	public void UnloadAll()
	{
		if (this == null || this.gameObject == null) return;
		
		RemoveAllCallback();
		if (audioClip != null) {
			Resources.UnloadAsset(audioClip);
			audioClip = null;
			if (Debug.isDebugBuild) Debug.Log("[UnloadClip] [OK] " + path);
		}
	}
	
	public void SetHighFrequent(UsedFrequent f)
	{
		freq = f;
		
		if (freq == UsedFrequent.High) {
			LoadClip();
		}
	}
	
	/// <summary>
	/// Play this sound with specified volume and pitch.
	/// </summary>
	/// <param name='volume'>
	/// Volume.
	/// </param>
	/// <param name='pitch'>
	/// Pitch.
	/// </param>
	public void Play(float volume, float pitch)
	{
		Play(volume, pitch, null, null);
	}
	
	/// <summary>
	/// Play this sound with 'volume', pitch 1f
	/// </summary>
	/// <param name='volume'>
	/// Volume.
	/// </param>
	public void Play(float volume)
	{
		Play(volume, 1f);
	}
	
	/// <summary>
	/// Play this sound with volume 1f, pitch 1f
	/// </summary>
	public void Play()
	{
		Play(1f, 1f);
	}
	#endregion
	
	#region Private
	
	/// <summary>
	/// Raises the destroy event.
	/// </summary>
	private void OnDestroy()
	{
		UnloadAll();
	}
	
	/// <summary>
	/// Set the specified n, p and f.
	/// </summary>
	/// <param name='n'>
	/// Name
	/// </param>
	/// <param name='p'>
	/// Path
	/// </param>
	/// <param name='f'>
	/// Use Frequecy
	/// </param>
	private void Set(string n, string p, UsedFrequent f)
	{
		path = p;
		freq = f;
		
		if (freq == UsedFrequent.High) {
			LoadClip();
		}
	}
	
	/// <summary>
	/// Loads the clip from resource.
	/// </summary>
	private void LoadClip()
	{
		if (audioClip == null) {
			audioClip = Resources.Load(path) as AudioClip;
			if (audioClip == null) {
				if (Debug.isDebugBuild) Debug.LogWarning("[LoadClip] Sound not found: " + path);
			} else {
				if (Debug.isDebugBuild) Debug.Log("[LoadClip] [OK] " + path);
			}
		} else {
			//if (Debug.isDebugBuild) Debug.Log("[LoadClip] [OK] This sound was loaded already: " + path);
		}	
	}
	
	/// <summary>
	/// Unloads the clip resource to release memory
	/// </summary>
	private void UnloadClip()
	{
		CallBack();
		if (audioClip != null) {
			Resources.UnloadAsset(audioClip);
			audioClip = null;
			if (Debug.isDebugBuild) Debug.Log("[UnloadClip] [OK] " + path);
		}
	}
	
	private void CallBack()
	{
		if (targets.Count == 0) return;
		if (targets[0].gameObject == null) {
			if (targets.Count > 0) targets.RemoveAt(0);
			return;
		}
		
		GameObject curTarget = targets[0].gameObject;
		curTarget.SendMessage(targets[0].callback, this, SendMessageOptions.DontRequireReceiver);
		
		if (targets.Count == 0) return;
		targets.RemoveAt(0);
	}
	
	private void RemoveAllCallback()
	{
		int len = targets.Count;
		for (int i = 0; i < len ; i++) {
			targets.RemoveAt(0);
		}
		//if (Debug.isDebugBuild) Debug.Log("[RemoveAllCallback] Removed all call back: " + len.ToString());
	}
	#endregion
}
