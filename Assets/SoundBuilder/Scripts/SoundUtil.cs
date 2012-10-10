using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// Sound util: This class create a Game Object named "SoundUtil" which contains Audio Clips for playing sounds in game. 
/// It can manage high/low using frequency sound files to decide the best time when Unload audio resources.
/// </summary>
public class SoundUtil : MonoBehaviour {
	
	#region Properties
	protected const string SOUND_LIST_FILENAME_PATH = "Sounds/_SoundList";
	protected const string HIGH_FREQ_FILENAME_PATH = "Sounds/_HighFreqList";
	protected const string RANDOM_GROUP_LIST_PATH = "Sounds/_RandGroupList";
	
	protected Dictionary<string, string> soundPaths = new Dictionary<string, string>();
	protected Dictionary<string, List<string>> randomGroup = new Dictionary<string, List<string>>();
	protected Dictionary<string, List<string>> highGroup = new Dictionary<string, List<string>>();
	
	protected Dictionary<string, FreqAudioClip> _sounds;

	private static SoundUtil instance;
	
	protected bool mute;
	public bool Mute
	{
		get { return mute; }
		set { mute = value; if (mute) StopAllSound(); }
	}
	#endregion

	
	#region Public
	
	/// <summary>
	/// Gets the instance.
	/// </summary>
	/// <value>
	/// The instance.
	/// </value>
	public static SoundUtil Instance {
		get {
			if (instance == null) {
				GameObject go = new GameObject ("SoundUtil");
				go.AddComponent<AudioListener>();
				go.AddComponent<AudioSource>();
				instance = go.AddComponent<SoundUtil> ();

				DontDestroyOnLoad (go);
			}
			return instance;
		}
	}
	
	/// <summary>
	/// This function do nothing. It's just for initting the Instance
	/// </summary>
	public void LoadData()
	{
	}
	
	/*
	public void OnGUI()
	{
		if (GUI.Button(new Rect(0, 300, 200, 100), "Mute")) Mute = true;
		if (GUI.Button(new Rect(0, 400, 200, 100), "UnMute")) Mute = false;
	}
	*/
	
	public void LoadScene(string sceneName)
	{
		if (Debug.isDebugBuild) Debug.Log("----------------------Load sound: " + sceneName + "----------------------");
		if (!highGroup.ContainsKey(sceneName)) {
			if (Debug.isDebugBuild) Debug.Log("[Load scene sound] Not contain scene name: " + sceneName);
			return;
		}
		
		List<string> highList = highGroup[sceneName];
		foreach (string key in highList) {
			_sounds[key].SetHighFrequent(UsedFrequent.High);
		}
	}
	
	public void UnloadScene(string sceneName)
	{
		/*if (!highGroup.ContainsKey(sceneName)) {
			if (Debug.isDebugBuild) Debug.Log("[Unload scene sound] Not contain scene name: " + sceneName);
			return;
		}
		
		List<string> highList = highGroup[sceneName];
		foreach (string key in highList) {
			_sounds[key].UnloadAll();
		}*/
		
		if (Debug.isDebugBuild) Debug.Log("----------------------Unload sound: " + sceneName + "----------------------");
		foreach (KeyValuePair<string, FreqAudioClip> pair in _sounds) {
			if (pair.Value != null && pair.Value.enabled && pair.Value.gameObject != null && !pair.Value.isDestroyed) {
				pair.Value.UnloadAll();
			}
		}
	}
	
	public void Play(string key, float volume, float pitch, GameObject target, string endCallback)
	{
		if (_sounds.ContainsKey(key)){
			if (!mute) {
				_sounds[key].Play(volume, pitch, target, endCallback);
			} else {
				_sounds[key].Play(target, endCallback);
			}
		} else {
			if (Debug.isDebugBuild) Debug.LogWarning("[Play] Key sound not found: " + key.ToString());
		}
	}
	
	public void Play(string key, GameObject target, string endCallback)
	{
		Play(key, 1f, 1f, target, endCallback);
	}
	
	/// <summary>
	/// Play the specified sound which has name 'key', with 'volume' and 'pitch'.
	/// </summary>
	/// <param name='key'>
	/// Key is File Name without the extention
	/// </param>
	/// <param name='volume'>
	/// Volume (0.0f ~ 1.0f)
	/// </param>
	/// <param name='pitch'>
	/// Pitch (0.0f ~ 1.0f)
	/// </param>
	public void Play(string key, float volume, float pitch)
	{
		Play(key, volume, 1f, null, null);
	}
	
	/// <summary>
	/// Play the specified sound which has name 'key', with 'volume', pitch 1.0f
	/// </summary>
	/// <param name='key'>
	/// Key is File Name without the extention
	/// </param>
	/// <param name='volume'>
	/// Volume (0.0f ~ 1.0f)
	/// </param>
	public void Play(string key, float volume)
	{
		Play(key, volume, 1f);
	}
	
	/// <summary>
	/// Play the specified sound which has name 'key', volume 1.0f, pitch 1.0f
	/// </summary>
	/// <param name='key'>
	/// Key is File Name without the extention
	/// </param>
	public void Play(string key)
	{
		Play(key, 1f, 1f);
	}	
	
	public void PlayRandom(string key, float volume, float pitch, GameObject target, string endCallback)
	{
		if (randomGroup.ContainsKey(key)){
			List<string> list = randomGroup[key];
			int r = Random.Range(0, list.Count);
			Play(list[r], volume, pitch, target, endCallback);
		} else {
			if (Debug.isDebugBuild) Debug.LogWarning("[PlayRandom] Random Group Key not found: " + key.ToString());
		}
	}
	
	/// <summary>
	/// Select 1 random sound in group to play
	/// </summary>
	/// <param name='key'>
	/// Key.
	/// </param>
	/// <param name='volume'>
	/// Volume.
	/// </param>
	/// <param name='pitch'>
	/// Pitch.
	/// </param>
	public void PlayRandom(string key, float volume, float pitch)
	{
		PlayRandom(key, volume, pitch, null, null);
	}
	
	public void PlayRandom(string key, GameObject target, string endCallback)
	{
		PlayRandom(key, 1f, 1f, target, endCallback);
	}
	
	/// <summary>
	/// Select 1 random sound in group to play
	/// </summary>
	/// <param name='key'>
	/// Key.
	/// </param>
	/// <param name='volume'>
	/// Volume.
	/// </param>
	public void PlayRandom(string key, float volume)
	{
		PlayRandom(key, volume, 1f);
	}
	
	/// <summary>
	/// Select 1 random sound in group to play
	/// </summary>
	/// <param name='key'>
	/// Key.
	/// </param>
	public void PlayRandom(string key)
	{
		PlayRandom(key, 1f, 1f);
	}
	
	public void StopAllSound()
	{
		SoundTool.StopSound();
	}
	
	#endregion

	#region Private
	
	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake ()
	{
		instance = this;
		Init();
	}
	
	/// <summary>
	/// Reads from resources.
	/// </summary>
	/// <returns>
	/// StringReader
	/// </returns>
	/// <param name='path'>
	/// Path.
	/// </param>
	StringReader ReadFromResources(string path)
	{		
		StringReader reader = null; 
		TextAsset textAsset = (TextAsset)Resources.Load(path, typeof(TextAsset));
		if (textAsset == null) {
			if (Debug.isDebugBuild) Debug.LogWarning(path + " not found");
			return null;
		}
		
		reader = new StringReader(textAsset.text);
		if ( reader == null ) {
			textAsset = null;
		   	if (Debug.isDebugBuild) Debug.LogWarning(path + " not readable");
		}
		else {
			return reader;
		}
		return null;
	}
	
	/// <summary>
	/// Reads the sound list from list file, then add to the Dictionary named "soundPaths"
	/// </summary>
	void ReadSoundList()
	{
		StringReader reader = ReadFromResources(SOUND_LIST_FILENAME_PATH);
		
		if (reader == null) return;
		
		string json = reader.ReadLine();
		soundPaths = LitJson.JsonMapper.ToObject<Dictionary<string, string>> (json);
		
		reader.Close();
	}
	
	/// <summary>
	/// Reads the list which contains high using frequency sound from list file, then add to the List named "highFreqList"
	/// </summary>
	void ReadHighFreqList()
	{
		StringReader reader = ReadFromResources(HIGH_FREQ_FILENAME_PATH);
		
		if (reader == null) return;
		
		string json = reader.ReadLine();
		highGroup = LitJson.JsonMapper.ToObject<Dictionary<string, List<string>>> (json);
		
		reader.Close();
	}
	
	/// <summary>
	/// Reads the random group list then add to the Dictionary named "randomGroup"
	/// </summary>
	void ReadRandomGroupList()
	{
		StringReader reader = ReadFromResources(RANDOM_GROUP_LIST_PATH);
		
		if (reader == null) return;
		
		string json = reader.ReadLine();
		randomGroup = LitJson.JsonMapper.ToObject<Dictionary<string, List<string>>> (json);
		
		reader.Close();
	}
	
	/// <summary>
	/// Init this instance.
	/// </summary>
	void Init()
	{
		ReadSoundList();
		ReadHighFreqList();
		ReadRandomGroupList();
		InitSounds();
	}
	
	/// <summary>
	/// Adds all audio clips to the Dictionary named "_sounds"
	/// </summary>
	void InitSounds()
	{	
		_sounds = new Dictionary<string, FreqAudioClip>();
		
		foreach (KeyValuePair<string, string> pair in soundPaths) {
			InitSound(pair.Key, pair.Value);
		}
	}
	
	/// <summary>
	/// Adds 1 audio clip to the Dictionary named "_sounds"
	/// </summary>
	/// <param name='key'>
	/// Key is File Name without the extention
	/// </param>
	/// <param name='path'>
	/// Path is path of the file in Resources folder
	/// </param>
	void InitSound(string key, string path) 
	{
		if (_sounds.ContainsKey(key)) {
			if (Debug.isDebugBuild) Debug.LogWarning("[Init] Key sound already added: " + key.ToString());
			return;
		}
		
		UsedFrequent f = UsedFrequent.Low;
		//if (highFreqList.Contains(key)) f = UsedFrequent.High;
		FreqAudioClip fac = FreqAudioClip.CreateFreqAudioClip(this.transform, key, path, f);
		
		_sounds.Add(key, fac);
	}
	#endregion
}
