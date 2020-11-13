using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>MisoTempra editor</summary>
namespace Editor
{
	/// <summary>Behavior tree editor</summary>
	namespace BehaviorTree
	{
		/// <summary>Editor classes</summary>
		namespace NodeEditor
		{
			[CustomPropertyDrawer(typeof(AI.BehaviorTree.CashContainer.Detail.SubsequentTaskInfomations))]
			public class BTSubsequentTaskDrawer : PropertyDrawer
			{
				public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
				{
					position.height = EditorGUIUtility.singleLineHeight;
					EditorGUI.LabelField(position, property.FindPropertyRelative("m_className").stringValue);
					position.y += EditorGUIUtility.singleLineHeight;

					var useProperty = property.FindPropertyRelative("m_key");
					useProperty.stringValue = EditorGUI.TextField(position, " >call key", useProperty.stringValue);
				}
			}
		}
	}
}