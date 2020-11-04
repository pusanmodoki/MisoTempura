using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.Graphs;
using FileAccess;

/// <summary>MisoTempra editor</summary>
namespace Editor
{
	/// <summary>Behavior tree editor</summary>
	namespace BehaviorTree
	{
		public class BehaviorTreeWindow : EditorWindow
		{
			static readonly Vector2 m_cWindowSize = new Vector2(100.0f, 100.0f);
			static readonly Vector2 m_cSelectFileFieldPosition = new Vector2(10, 10);

			/// <summary>static instance</summary>
			public static List<BehaviorTreeWindow> instances { get; private set; } = new List<BehaviorTreeWindow>();
			/// <summary>EditorApplication.playModeStateChangedにSaveCallbackを追加したか</summary>
			static bool m_isAddSaveCallback = false;

			public BehaviorTreeNodeView nodeView { get; private set; } = null;
			public string fileName { get { return m_fileName; } set { m_fileName = value; titleContent = new GUIContent("BTEditor: " + m_fileName); } }
			[SerializeField, HideInInspector]
			string m_fileName = "";

			public bool isDeleteFile { get; private set; } = false;
			public void SetTrueIsDeleteFile() { isDeleteFile = true; }

			public void RegisterEditorGUI()
			{
				if (rootVisualElement.childCount == 2)
					rootVisualElement.RemoveAt(1);

				IMGUIContainer container = new IMGUIContainer(nodeView.scriptableEditor.OnInspectorGUI);
				rootVisualElement.Add(container);
			}
			public void UnregisterEditorGUI()
			{
				if (rootVisualElement.childCount == 2)
					rootVisualElement.RemoveAt(1);
			}

			/// <summary>Open</summary>
			[MenuItem("Window/Misotempra/Behavior tree")]
			static void Open()
			{
				//インスタンス作成
				var window = CreateInstance<BehaviorTreeWindow>();
				window.titleContent = new GUIContent("BTEditor");

				//表示
				window.Show();
			}

			void OnEnable()
			{
				instances.Add(this);

				//コールバック追加
				if (!m_isAddSaveCallback)
				{
					EditorApplication.playModeStateChanged += SaveCallaback;
					m_isAddSaveCallback = true;
				}

				nodeView = new BehaviorTreeNodeView(this);
				rootVisualElement.Add(nodeView);
			}

			void OnDisable()
			{
				//インスタンスがあれば削除
				if (instances.Contains(this))
					instances.Remove(this);

				if (isDeleteFile) return;

				try { if (nodeView != null) nodeView.Save(); }
				catch (System.Exception) { return; }
				Debug.Log("Behavior tree (" + fileName + ") Save completed.");
			}

			/// <summary>EditorApplication用コールバック</summary>
			static void SaveCallaback(PlayModeStateChange change)
			{
				//プレイモードになった場合セーブを行う
				if (change == PlayModeStateChange.EnteredPlayMode)
				{
					foreach (var e in instances)
						if (e.nodeView != null) e.nodeView.Save();
				}
			}
		}
	}
}