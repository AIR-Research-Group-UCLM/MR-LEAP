﻿using System;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Clase para el editor que genera automáticamente un identificador para un objeto.
/// Extraído de https://answers.unity.com/questions/487121/automatically-assigning-gameobjects-a-unique-and-c.html
/// </summary>
[CustomPropertyDrawer(typeof(UniqueIdentifierAttribute))]
public class UniqueIdentifierDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
    {
        // Generate a unique ID, defaults to an empty string if nothing has been serialized yet
        if (prop.stringValue == "")
        {
            Guid guid = Guid.NewGuid();
            prop.stringValue = guid.ToString();
        }

        // Place a label so it can't be edited by accident
        Rect textFieldPosition = position;
        textFieldPosition.height = 16;
        DrawLabelField(textFieldPosition, prop, label);
    }

    internal void DrawLabelField(Rect position, SerializedProperty prop, GUIContent label)
    {
        EditorGUI.LabelField(position, label, new GUIContent(prop.stringValue));
    }
}