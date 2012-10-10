using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class SoundSceneBuilder : Editor {
	
	const string UNITY_SOUND_FOLDER = "/Assets/SoundBuilder/Resources/Sounds";
	const string SOUND_LIST_FILENAME = "_SoundList.txt";
	const string RANDOM_GROUP_LIST_FILENAME = "_RandGroupList.txt";
	const string HIGH_FREQ_FILENAME = "_HighFreqList.txt";	
	
	public Dictionary<string, List<string>> sceneGroup;
	
	[HideInInspector]
	public List<string> scenes;
	
	public string selectedScene;
	private string selectedSceneTemp;
	
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
		string file = Directory.GetCurrentDirectory() + UNITY_SOUND_FOLDER + "/" +  HIGH_FREQ_FILENAME;
		StreamWriter sw = File.CreateText(file);
		sw.Close();
		AssetDatabase.Refresh();
		
		sceneGroup = new Dictionary<string, List<string>>();
		Write();
		
		AssetDatabase.Refresh();
	}
	
	public void Read()
	{
		string s = "Sounds/" + HIGH_FREQ_FILENAME;
		s = s.Replace(".txt", string.Empty);
		StringReader reader = ReadFromResources(s);
		
		if (reader == null) {
			MakeEmptyFile();
			reader = ReadFromResources(s);
		}
		
		string json = reader.ReadLine();
		sceneGroup = LitJson.JsonMapper.ToObject<Dictionary<string, List<string>>> (json);
		
		if (sceneGroup.Count == 0) {
			scenes.Clear();
			sounds.Clear();
			selectedScene = string.Empty;
			selectedSceneTemp = string.Empty;
		}
		
		scenes.Clear();	
		bool isFirst = false;
		foreach (KeyValuePair<string, List<string>> pair in sceneGroup) {
			scenes.Add(pair.Key);
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
		
		selectedScene = key;
		selectedSceneTemp = key;
		
		List<string> list = sceneGroup[key];
		foreach (string s in list) {
			sounds.Add(s);
		}
	}

	public void Write()
	{
		string file = Directory.GetCurrentDirectory() + UNITY_SOUND_FOLDER + "/" +  HIGH_FREQ_FILENAME;
		
		TextWriter tw;
		tw = new StreamWriter(file);
		
		string json = LitJson.JsonMapper.ToJson (sceneGroup);
		tw.Write(json);
		tw.Close();
		
		Debug.Log("Saved random group list to file: " + file);
		AssetDatabase.Refresh();
	}
	
	public void AddGroup()
	{
		
		string newGroup = "new scene " + GetUniqueNumber();
		sceneGroup.Add(newGroup, new List<string>());
		scenes.Add(newGroup);
		ReadGroup(newGroup);
	}
	
	public void DeleteSelectedGroup()
	{
		sceneGroup.Remove(selectedSceneTemp);
		scenes.Remove(selectedSceneTemp);
		
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
		sceneGroup.Remove(selectedSceneTemp);
		scenes.Remove(selectedSceneTemp);
		
		sceneGroup.Add(selectedScene, new List<string>());
		foreach (string obj in sounds) {
			sceneGroup[selectedScene].Add(obj);
		}
		scenes.Add(selectedScene);
		
		selectedSceneTemp = selectedScene;
		
		Write();
	}
	
	static int counter;
	public static int GetUniqueNumber()
	{ 
	    return counter++; 
	}
}
