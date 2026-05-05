using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(InfiniteRotation))]
public class InfiniteRotationEditor : Editor
{
    static bool showParameters = true;
    public override void OnInspectorGUI()
    {
        InfiniteRotation r = (InfiniteRotation)target;

        base.OnInspectorGUI();

        // ROTATION AXIS

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.PrefixLabel("Rotation Axis:");

        GUI.enabled = !r.GetRandom();

        SerializedProperty propX = serializedObject.FindProperty("x");
        SerializedProperty propY = serializedObject.FindProperty("y");
        SerializedProperty propZ = serializedObject.FindProperty("z");

        float prev = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = 15.0f; // Replace this with any width
        EditorGUILayout.PropertyField(propX, true);
        EditorGUILayout.PropertyField(propY, true);
        EditorGUILayout.PropertyField(propZ, true);

        if (r.GetRandom())
        {
            Undo.RecordObject(target, "Randomize");
            r.SetX(true);
            r.SetY(true);
            r.SetZ(true);
        }

        EditorGUIUtility.labelWidth = prev; // Replace this with any width

        EditorGUILayout.EndHorizontal();

        GUI.enabled = true;

        // SPEED

        EditorGUILayout.Space();

        SerializedProperty propSpeed = serializedObject.FindProperty("speed");
        EditorGUILayout.PropertyField(propSpeed, true);

        // PARAMETERS

        EditorGUILayout.Space();

        showParameters = EditorGUILayout.BeginFoldoutHeaderGroup(showParameters, "Rotation Parameters");

        if (showParameters)
        {
            SerializedProperty propOnCreation = serializedObject.FindProperty("rotateOnCreation");
            SerializedProperty propInverted = serializedObject.FindProperty("inverted");
            SerializedProperty propRandom = serializedObject.FindProperty("random");

            EditorGUILayout.PropertyField(propOnCreation, true);
            EditorGUILayout.PropertyField(propInverted, true);
            EditorGUILayout.PropertyField(propRandom, true);
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
        serializedObject.ApplyModifiedProperties();
    }
}
