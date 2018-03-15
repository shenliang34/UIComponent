using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(RadarChart))]
public class RadarChartEditor : Editor {
	SerializedProperty maxPointsProp, color, raycast, material;
	SerializedProperty percents;

	string[] names ;
	void OnEnable() {
		maxPointsProp = serializedObject.FindProperty("maxPoints");
		percents = serializedObject.FindProperty("percents");

		color = serializedObject.FindProperty("m_Color");
		raycast = serializedObject.FindProperty("m_RaycastTarget");
		material = serializedObject.FindProperty("m_Material");
		names = new string[6] {
			"Center", "ATK", "HP", "ASS", "REV", "CON" 
		};
	}

	bool showMaxProp = false, showPercent = true;
	public override void OnInspectorGUI() {
		serializedObject.Update ();

		showMaxProp = EditorGUILayout.Foldout(showMaxProp, "Max Points");
		if(showMaxProp) {
			int size = maxPointsProp.arraySize;
			for(int i=0; i<size; i++) {
				SerializedProperty p = maxPointsProp.GetArrayElementAtIndex(i);
				EditorGUILayout.PropertyField(p, new GUIContent(names[i], ""));
			}
		}

		EditorGUILayout.PropertyField(raycast);
		EditorGUILayout.PropertyField(material);
		EditorGUILayout.PropertyField(color);

		EditorGUILayout.Space();

		showPercent = EditorGUILayout.Foldout(showPercent, "percents");
		if(showPercent) {
			int size = percents.arraySize;
			for(int i=0; i<size; i++) {
				SerializedProperty p = percents.GetArrayElementAtIndex(i);
				EditorGUILayout.Slider(p, 0, 1, new GUIContent(names[i+1], ""));
			}
		}

		serializedObject.ApplyModifiedProperties ();
	}

}
