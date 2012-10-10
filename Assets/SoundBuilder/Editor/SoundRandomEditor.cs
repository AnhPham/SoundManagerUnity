using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(SoundRandomBuilder))]
public class SoundRandomEditor : Editor
{
	public override void OnInspectorGUI()
	{
		SoundRandomBuilder gen = (SoundRandomBuilder)target;
		
		if (gen.randomGroup == null) {
			if (GUILayout.Button("Read / Create")) gen.Read();
		} else {
			GUILayout.Label("Groups");
			
			foreach (string s in gen.groups) {
				if (GUILayout.Button(s)) gen.ReadGroup(s);
			}
			
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Add Group")) gen.AddGroup();
			if (GUILayout.Button("Delete Selected Group")) gen.DeleteSelectedGroup();
			GUILayout.EndHorizontal();
	
			
			DrawDefaultInspector();
			
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Set Selected Sounds To Group")) gen.SetSelectedSoundToGroup();
			if (GUILayout.Button("Save all")) gen.UpdateGroup();
			GUILayout.EndHorizontal();
		}
	}
}
