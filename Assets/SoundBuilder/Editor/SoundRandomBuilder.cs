using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class SoundRandomBuilder : Editor {
	
	const string UNITY_SOUND_FOLDER = "/Assets/SoundBuilder/Resources/Sounds";
	const string SOUND_LIST_FILENAME = "_SoundList.txt";
	const string RANDOM_GROUP_LIST_FILENAME = "_RandGroupList.txt";
	const string HIGH_FREQ_FILENAME = "_HighFreqList.txt";	
	
	public Dictionary<string, List<string>> randomGroup;
	
	[HideInInspector]
	public List<string> groups;
	
	public string selectedGroup;
	private string selectedGroupTemp;
	
	public List<string> sounds;
	
	StringReader ReadFromResources(string path)
	{		
		StringReader reader = null; 
		TextAsset textAsset = (TextAsset)Resources.Load(path, typeof(TextAsset));
		if (textAsset == null) {
			//if (Debug.isDebugBuild) Debug.LogWarning(path + " not found");
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
	
	public void MakeEmptyFile()
	{
		string file = Directory.GetCurrentDirectory() + UNITY_SOUND_FOLDER + "/" +  RANDOM_GROUP_LIST_FILENAME;
		StreamWriter sw = File.CreateText(file);
		sw.Close();
		AssetDatabase.Refresh();
		
		randomGroup = new Dictionary<string, List<string>>();
		Write();
		
		AssetDatabase.Refresh();
	}
	
	public void Read()
	{
		string s = "Sounds/" + RANDOM_GROUP_LIST_FILENAME;
		s = s.Replace(".txt", string.Empty);
		StringReader reader = ReadFromResources(s);
		
		if (reader == null) {
			MakeEmptyFile();
			reader = ReadFromResources(s);
		}
		
		string json = reader.ReadLine();
		randomGroup = LitJson.JsonMapper.ToObject<Dictionary<string, List<string>>> (json);
		
		if (randomGroup.Count == 0) {
			groups.Clear();
			sounds.Clear();
			selectedGroup = string.Empty;
			selectedGroupTemp = string.Empty;
		}
		
		groups.Clear();	
		bool isFirst = false;
		foreach (KeyValuePair<string, List<string>> pair in randomGroup) {
			groups.Add(pair.Key);
			if (!isFirst) {
				isFirst = true;
				ReadGroup(pair.Key);
			}
		}
		
		reader.Close();
	}
	
	public void ReadGroup(string key)
	{
		sounds.Clear();
		
		selectedGroup = key;
		selectedGroupTemp = key;
		
		List<string> list = randomGroup[key];
		foreach (string s in list) {
			sounds.Add(s);
		}
	}

	public void Write()
	{
		string file = Directory.GetCurrentDirectory() + UNITY_SOUND_FOLDER + "/" +  RANDOM_GROUP_LIST_FILENAME;
		
		TextWriter tw;
		tw = new StreamWriter(file);
		
		string json = LitJson.JsonMapper.ToJson (randomGroup);
		tw.Write(json);
		tw.Close();
		
		Debug.Log("Saved random group list to file: " + file);
		AssetDatabase.Refresh();
	}
	
	public void AddGroup()
	{
		
		string newGroup = "new group " + GetUniqueNumber();
		randomGroup.Add(newGroup, new List<string>());
		groups.Add(newGroup);
		ReadGroup(newGroup);
	}
	
	public void DeleteSelectedGroup()
	{
		randomGroup.Remove(selectedGroupTemp);
		groups.Remove(selectedGroupTemp);
		
		Write();
		Read();
	}
	
	public void SetSelectedSoundToGroup()
	{
		sounds.Clear();
		foreach (Object obj in Selection.objects) {
			sounds.Add(obj.name);
		}
	}
	
	public void UpdateGroup()
	{
		randomGroup.Remove(selectedGroupTemp);
		groups.Remove(selectedGroupTemp);
		
		randomGroup.Add(selectedGroup, new List<string>());
		foreach (string obj in sounds) {
			randomGroup[selectedGroup].Add(obj);
		}
		groups.Add(selectedGroup);
		
		selectedGroupTemp = selectedGroup;
		
		Write();
	}
	
	static int counter;
	public static int GetUniqueNumber()
	{ 
	    return counter++; 
	}
}
