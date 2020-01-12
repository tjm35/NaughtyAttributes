﻿using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace NaughtyAttributes.Editor
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(UnityEngine.Object), true)]
	public class NaughtyInspector : UnityEditor.Editor
	{
		private IEnumerable<FieldInfo> _serializedFields;
		private IEnumerable<MethodInfo> _methods;

		private void OnEnable()
		{
			_serializedFields = ReflectionUtility.GetAllFields(
				target, f => serializedObject.FindProperty(f.Name) != null);

			_methods = ReflectionUtility.GetAllMethods(
				target, m => m.GetCustomAttributes(typeof(ButtonAttribute), true).Length > 0);
		}

		private void OnDisable()
		{
			SpecialCasePropertyDrawerDatabase.ClearCache();
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			foreach (var field in _serializedFields)
			{
				SerializedProperty property = serializedObject.FindProperty(field.Name);
				EditorGUIExtensions._PropertyField_Layout(property, true);
			}

			serializedObject.ApplyModifiedProperties();

			// Draw methods
			if (_methods.Any())
			{
				EditorGUIExtensions.HorizontalLine();
				EditorGUILayout.LabelField("Buttons", EditorStyles.boldLabel);

				foreach (var method in _methods)
				{
					EditorGUIExtensions.Button(serializedObject.targetObject, method);
				}
			}
		}
	}
}
