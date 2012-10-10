using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;

/// <summary>
/// Test sound util.
/// </summary>
public class TestSoundUtil : ScriptableObject {
	
	#region Const
	protected const string SOUND_LIST_FILENAME_PATH = "Sounds/_SoundList";
	protected const string HIGH_FREQ_FILENAME_PATH = "Sounds/_HighFreqList";
	protected const string RANDOM_GROUP_LIST_PATH = "Sounds/_RandGroupList";	
	#endregion

	#region Properties
	protected static Dictionary<string, string> soundPaths = new Dictionary<string, string>();
	protected static Dictionary<string, List<string>> randomGroup = new Dictionary<string, List<string>>();
	protected static Dictionary<string, List<string>> highGroup = new Dictionary<string, List<string>>();
	protected static Dictionary<string, FreqAudioClip> _sounds;	
	#endregion
	
	#region Public
	/// <summary>
	/// Checks all source code to find wrong Play() or PlayRandom() of SoundUtil
	/// </summary>
	[MenuItem("Tools/Sound Utils/Test/Check All Source Code")]
	public static void CheckAllSourceCode() 
    { 
		Init();
		
		bool goodFlag = true;	
		int normalPlay = 0;
		int randomPlay = 0;
		
		Debug.Log("---------- Start to test source files----------");
		
		Object[] sources = Selection.GetFiltered(typeof(TextAsset), SelectionMode.DeepAssets); 
		
		foreach (Object obj in sources) {
			TextAsset textAsset = (TextAsset)obj;
			StringReader reader = new StringReader(textAsset.text);
			
			string s;
			while ((s = reader.ReadLine()) != null) {
				string key;
				if (s.Contains("SoundUtil.Instance.Play("))
				{
					key = GetKeyFromLineCodeNormal(s);
					normalPlay++;
					if (!soundPaths.ContainsKey(key)) {
						Debug.LogWarning("Sound key not found: [" + key + "] in " + textAsset.name);
						goodFlag = false;
					}						
				}
				else if (s.Contains("SoundUtil.Instance.PlayRandom(")) {
					key = GetKeyFromLineCodeRandom(s);
					randomPlay++;
					if (!randomGroup.ContainsKey(key)) {
						Debug.LogWarning("Random group key not found: [" + key + "] in " + textAsset.name);
						goodFlag = false;
					}
				}
			}
		}
        
		Debug.Log("Source files checked: " + sources.Length.ToString());
		Debug.Log("Normal Play checked: " + normalPlay.ToString());
		Debug.Log("Random Play checked: " + randomPlay.ToString());
		
		string r = "GOOD"; if (!goodFlag) r = "BAD";
		Debug.Log("---------- Finish to test source files---------- RESULT: " + r);
    }

	/// <summary>
	/// Checks all input sound file for finding missing sound or wrong key
	/// </summary>
	[MenuItem("Tools/Sound Utils/Test/Check All Input Sound File")]
    public static void CheckAllInputSoundFile() { 
		Init();
		
		bool goodFlag = true;		
		Debug.Log("---------- Start to test input files----------");
		
		Debug.Log("--Test exist sounds [" + SOUND_LIST_FILENAME_PATH + "]: ");
		foreach (KeyValuePair<string, string> pair in soundPaths) {
			if (!ExistSound(pair.Key)) goodFlag = false;
		}
		
		Debug.Log("--Test exist key sound[" + RANDOM_GROUP_LIST_PATH + "]: ");
		foreach (KeyValuePair<string, List<string>> pair in randomGroup) {
			foreach (string s in pair.Value) {
				if (!ExistSound(s)) goodFlag = false;
			}
		}
		
		Debug.Log("--Test exist key sound[" + HIGH_FREQ_FILENAME_PATH + "]: ");	
		foreach (KeyValuePair<string, List<string>> pair in highGroup) {
			foreach (string s in pair.Value) {
				if (!ExistSound(s)) goodFlag = false;
			}
		}
		
		string r = "GOOD"; if (!goodFlag) r = "BAD";
		Debug.Log("---------- Finish to test input files---------- RESULT: " + r);
    }	
	#endregion
	
	#region Private
	/// <summary>
	/// Check the sound file or sound key exist or not
	/// </summary>
	/// <returns>
	/// The sound.
	/// </returns>
	/// <param name='key'>
	/// If set to <c>true</c> key.
	/// </param>
	private static bool ExistSound(string key)
	{
		if (!soundPaths.ContainsKey(key)) {
			Debug.LogWarning("Key not exist: " + key);
			return false;
		}
		Object obj = Resources.Load(soundPaths[key]);
		if (obj == null) {
			Debug.LogWarning("Sound file not exist: " + soundPaths[key]);
			return false;
		} else {
			Resources.UnloadAsset(obj);
			return true;
		}
	}
	
	/// <summary>
	/// Reads the sound list from list file, then add to the Dictionary named "soundPaths"
	/// </summary>
	private static void ReadSoundList()
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
	private static void ReadHighFreqList()
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
	private static void ReadRandomGroupList()
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
	private static void Init()
	{
		ReadSoundList();
		ReadHighFreqList();
		ReadRandomGroupList();
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
	private static StringReader ReadFromResources(string path)
	{		
		StringReader reader = null; 
		TextAsset textAsset = (TextAsset)Resources.Load(path, typeof(TextAsset));
		if (textAsset == null) {
			Debug.LogWarning(path + " not found");
			return null;
		}
		
		reader = new StringReader(textAsset.text);
		if ( reader == null ) {
			textAsset = null;
		   	Debug.LogWarning(path + " not readable");
		}
		else {
			return reader;
		}
		return null;
	}
	
	/// <summary>
	/// Gets the key from line code.
	/// </summary>
	/// <returns>
	/// The key from line code.
	/// </returns>
	/// <param name='line'>
	/// Line.
	/// </param>
	private static string GetKeyFromLineCodeNormal(string line)
	{
		return GetKeyFromLineCode(line, "SoundUtil.Instance.Play(");
	}
	
	private static string GetKeyFromLineCodeRandom(string line)
	{
		return GetKeyFromLineCode(line, "SoundUtil.Instance.PlayRandom(");
	}
	
	private static string GetKeyFromLineCode(string line, string s)
	{
		string r = line.Substring(line.LastIndexOf(s));
		string[] arr = r.Split('\"');
		return arr[1];
	}
		
	
	#endregion
}
