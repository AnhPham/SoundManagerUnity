using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// Add sound util.
/// </summary>
public class AddSoundUtil : ScriptableObject 
{	
	#region Const
	private const string UNITY_SOUND_FOLDER = "/Assets/SoundBuilder/Resources/Sounds";
	private const string SOUND_LIST_FILENAME = "_SoundList.txt";
	private const string RANDOM_GROUP_LIST_FILENAME = "_RandGroupList.txt";
	private const string HIGH_FREQ_FILENAME = "_HighFreqList.txt";	
	#endregion

	#region Properties
	public static string[] filePaths;	
	#endregion
	
	#region Menu Item
	/*
	[MenuItem("Tools/Sound Utils/Add Update/Copy Sounds And Gen File List")]
	public static void CopySoundAndGenListFile()
	{
		CopySoundFiles();
		GenListFile();
	}
	*/
	
	[MenuItem("Tools/Sound Utils/Add Update/Create sound list in selected folder")]
	public static void GenListFileOnly()
	{
		string soundDir = Directory.GetCurrentDirectory() + UNITY_SOUND_FOLDER;
		filePaths = Directory.GetFiles(soundDir, "*.wav");
		GenListFile();
	}
	
	[MenuItem("Tools/Sound Utils/Setting/Change Setting Small Audio")]
    public static void ChangeSettingSmallAudio() { 
		ChangeSettingAudio(false, AudioImporterLoadType.DecompressOnLoad, 64001);
    }
	
	[MenuItem("Tools/Sound Utils/Setting/Change Setting Big Audio")]
    public static void ChangeSettingBigAudio() { 
		ChangeSettingAudio(false, AudioImporterLoadType.StreamFromDisc, 128001);
    }
	
	#endregion
	
	#region Private
	
	/// <summary>
	/// Changes the setting of selected audio clip files.
	/// </summary>
	/// <param name='is3D'>
	/// Is3 d.
	/// </param>
	/// <param name='loadType'>
	/// Load type.
	/// </param>
	/// <param name='cb'>
	/// Compression Bitrate
	/// </param>
    private static void ChangeSettingAudio(bool is3D, AudioImporterLoadType loadType, int cb) { 
        Object[] clips = GetSelectedSounds(); 
        foreach (AudioClip clip in clips)  {
            string path = AssetDatabase.GetAssetPath(clip); 
            AudioImporter audioImporter = AssetImporter.GetAtPath(path) as AudioImporter; 
            audioImporter.threeD = is3D;
			audioImporter.format = AudioImporterFormat.Compressed;
			audioImporter.loadType = loadType;
			audioImporter.compressionBitrate = cb;
            AssetDatabase.ImportAsset(path); 
        }
    }
    
	/// <summary>
	/// Gets the selected sounds.
	/// </summary>
	/// <returns>
	/// The selected sounds.
	/// </returns>
    private static Object[] GetSelectedSounds() 
    { 
        return Selection.GetFiltered(typeof(AudioClip), SelectionMode.DeepAssets); 
    }
	
	/*
	private static void CopySoundFiles()
	{
		Debug.Log("---------- Start to copy files----------");
		
		string soundDir = ResolveRelativePath(Directory.GetCurrentDirectory(), SVN_SOUND_FOLDER);
		filePaths = GetFiles(soundDir, "*.wav");
		foreach (string file in filePaths) {
			string fname = Path.GetFileName(file);
			File.Copy(file, Directory.GetCurrentDirectory() + UNITY_SOUND_FOLDER + "/" +  fname, true);
			Debug.Log("Copied " + fname);
		}
		Debug.Log("Total " + filePaths.Length.ToString() + " Copied");
	}
	*/
	
	/// <summary>
	/// Generates the file which contain 'name' & 'path' of sound list
	/// </summary>
	private static void GenListFile()
	{
		Debug.Log("---------- Start to gen file----------");
		
		string file = Directory.GetCurrentDirectory() + UNITY_SOUND_FOLDER +  "/" + SOUND_LIST_FILENAME;
		
		TextWriter tw;
		tw = new StreamWriter(file);
		
		Dictionary<string, string> dict = new Dictionary<string, string>();

		Object[] objs = GetSelectedSounds();
		foreach (Object obj in objs) {
			string path = AssetDatabase.GetAssetPath(obj);
			string k = Path.GetFileNameWithoutExtension(path);
			string v = FindResoucePath(path);
			dict.Add(k, v);
		}
		
		string json = LitJson.JsonMapper.ToJson (dict);
		tw.Write(json);		
		tw.Close();
		
		Debug.Log("Saved name list to file: " + file);
		
		AssetDatabase.Refresh();
	}
	
	/// <summary>
	/// Resolves the relative path.
	/// </summary>
	/// <returns>
	/// The relative path.
	/// </returns>
	/// <param name='referencePath'>
	/// Reference path.
	/// </param>
	/// <param name='relativePath'>
	/// Relative path.
	/// </param>
	private static string ResolveRelativePath(string referencePath, string relativePath)   
	{   
	    return Path.GetFullPath(Path.Combine(referencePath, relativePath));
	}
	
	private static string[] GetFiles(string path, string searchPattern)
	{
	    string[] searchPatterns = searchPattern.Split('|');
	    List<string> files = new List<string>();
	    foreach (string sp in searchPatterns)
	        files.AddRange(System.IO.Directory.GetFiles(path, sp));
	    files.Sort();
	    return files.ToArray();
	}
	
	private static string FindResoucePath(string path)
	{
		string r = string.Empty;
		string res = "Resources/";
		
		int length = path.Length;
		
		int i  = path.LastIndexOf(res);
		int lastLength = length - (i + res.Length);
		
		int j = path.LastIndexOf('.');
		if (j >= 0) lastLength -= (length - j);
		
		r = path.Substring(i + res.Length, lastLength);
		return r;
	}
	
	#endregion
}
