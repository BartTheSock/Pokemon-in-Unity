using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapArea))]
public class MapAreaEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        float totalChanceGrass = serializedObject.FindProperty("totalChanceGrass").floatValue;
        float totalChanceWater = serializedObject.FindProperty("totalChanceWater").floatValue;
        float totalChanceCave = serializedObject.FindProperty("totalChanceCave").floatValue;
        float totalChanceRock = serializedObject.FindProperty("totalChanceRock").floatValue;
        
        if (totalChanceGrass != 100f && totalChanceGrass != -1f)
            EditorGUILayout.HelpBox($"The total chance of Pokémon in the grass is {totalChanceGrass}%, not 100!", MessageType.Error);

        if (totalChanceWater != 100f && totalChanceWater != -1f)
            EditorGUILayout.HelpBox($"The total chance of Pokémon in the water is {totalChanceWater}%, not 100!", MessageType.Error);

        if (totalChanceCave != 100f && totalChanceCave != -1f)
            EditorGUILayout.HelpBox($"The total chance of Pokémon in the caves is {totalChanceCave}%, not 100!", MessageType.Error);

        if (totalChanceRock != 100f && totalChanceRock != -1f)
            EditorGUILayout.HelpBox($"The total chance of Pokémon hiding under rocks is {totalChanceRock}%, not 100!", MessageType.Error);
    }
}