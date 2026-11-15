using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SceneReference))]
public class SceneReferenceDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var sceneAssetProperty = property.FindPropertyRelative("sceneAsset");
        EditorGUI.PropertyField(position, sceneAssetProperty, label);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var sceneAssetProperty = property.FindPropertyRelative("sceneAsset");
        return EditorGUI.GetPropertyHeight(sceneAssetProperty, label);
    }
}