using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(SoundSceneBuilder))]
public class SoundSceneEditor : Editor
{
	public override void OnInspectorGUI()
	{
		SoundSceneBuilder gen = (SoundSceneBuilder)target;
		
		if (gen.sceneGroup == null) {
			if (GUILayout.Button("Read / Create")) gen.Read();
		} else {
			GUILayout.Label("Scenes");
			
			foreach (string s in gen.scenes) {
				if (GUILayout.Button(s)) gen.ReadGroup(s);
			}
			
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Add Scene")) gen.AddGroup();
			if (GUILayout.Button("Delete Selected Scene")) gen.DeleteSelectedGroup();
			GUILayout.EndHorizontal();
	
			
			DrawDefaultInspector();
			
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Set Selected Sounds To Scene")) gen.SetSelectedSoundToGroup();
			if (GUILayout.Button("Save all")) gen.UpdateGroup();
			GUILayout.EndHorizontal();
		}
	}
}
