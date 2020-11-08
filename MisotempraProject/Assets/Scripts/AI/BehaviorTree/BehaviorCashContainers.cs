using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using AI.BehaviorTree;
using AI.BehaviorTree.Node.Composite;

namespace AI
{
	/// <summary>Behavior tree editor</summary>
	namespace BehaviorTree
	{
		namespace CashContainer
		{
			namespace Detail
			{
				[System.Serializable]
				public struct ServiceInfomations
				{
					public string className { get { return m_className; } }
					public float callInterval { get { return m_callInterval; } }

					public ServiceInfomations(string className, float callInterval)
					{
						m_className = className;
						m_callInterval = callInterval;
					}

					[SerializeField]
					string m_className;
					[SerializeField]
					float m_callInterval;
				}

				[System.Serializable]
				public class BaseCashContainer
				{
					public string nodeName { get { return m_nodeName; } set { m_nodeName = value; } }
					public string className { get { return m_className; } }
					public string editNodeClassName { get { return m_editNodeClassName; } }
					public string guid { get { return m_guid; } }
					public string memo { get { return m_memo; } set { m_memo = value; } }
					public Vector2 position { get { return m_position; } set { m_position = value; } }

					public virtual bool isSaveReady { get { return false; } }

					public void Initialize(string nodeName, string className, string editNodeClassName, Vector2 position)
					{
						m_nodeName = nodeName;
						m_className = className;
						m_editNodeClassName = editNodeClassName;
						m_guid = System.Guid.NewGuid().ToString();
						m_position = position;
					}

					[SerializeField]
					string m_nodeName = "";
					[SerializeField]
					string m_className = "";
					[SerializeField]
					string m_editNodeClassName = "";
					[SerializeField]
					string m_guid = "";
					[SerializeField]
					string m_memo = "";
					[SerializeField]
					Vector2 m_position = Vector2.zero;
				}
			}
			[System.Serializable]
			public class RootCashContainer : Detail.BaseCashContainer
			{
				public BlackboardCashContainer blackbord { get { return m_blackboard; } }
				public List<string> childrenNodesGuid { get { return m_childrenNodesGuid; } }
				
				public override bool isSaveReady { get { return true; } }

				public bool isBlackboardSaveReady
				{
					get
					{
						HashSet<string> names = new HashSet<string>();
						foreach (var e in m_blackboard.keys)
						{
							if (e.Length == 0) return false;
							else if (names.Contains(e)) return false;
							names.Add(e);
						}
						return true;
					}
				}

				[SerializeField]
				BlackboardCashContainer m_blackboard = new BlackboardCashContainer();
				[SerializeField]
				List<string> m_childrenNodesGuid = new List<string>();
			}

			[System.Serializable]
			public abstract class NotRootCashContainer : Detail.BaseCashContainer
			{
				public List<Detail.ServiceInfomations> serviceClasses { get { return m_serviceClasses; } }
				public List<string> decoratorClasses { get { return m_decoratorClasses; } }
				public string parentGuid { get { return m_parentGuid; } set { m_parentGuid = value; } }

				public override bool isSaveReady { get { return m_parentGuid != null && m_parentGuid.Length > 0; } }

				[SerializeField]
				List<Detail.ServiceInfomations> m_serviceClasses = new List<Detail.ServiceInfomations>();
				[SerializeField]
				List<string> m_decoratorClasses = new List<string>();
				[SerializeField]
				string m_parentGuid = "";
			}

			[System.Serializable]
			public class TaskCashContainer : NotRootCashContainer
			{
				public string taskClassName { get { return m_taskClassName; } set { m_taskClassName = value; } }
				public string taskToJson { get { return m_taskToJson; } set { m_taskToJson = value; } }
				
				[SerializeField]
				string m_taskClassName = "";
				[SerializeField]
				string m_taskToJson = "";
			}

			[System.Serializable]
			public class CompositeCashContainer : NotRootCashContainer
			{
				public List<string> childrenNodesGuid { get { return m_childrenNodesGuid; } }
				
				[SerializeField]
				List<string> m_childrenNodesGuid = new List<string>();
			}

			[System.Serializable]
			public class ParallelCashContainer : CompositeCashContainer
			{
				public ParallelFinishMode finishMode { get { return m_finishMode; } set { m_finishMode = value; } }

				public override bool isSaveReady { get { return parentGuid != null && parentGuid.Length > 0 
							&& finishMode != ParallelFinishMode.Null; } }

				[SerializeField]
				ParallelFinishMode m_finishMode = ParallelFinishMode.Null;
			}

			[System.Serializable]
			public class RandomCashContainer : CompositeCashContainer
			{
				public List<float> probabilitys { get { return m_probabilitys; } }
				
				[SerializeField]
				List<float> m_probabilitys = new List<float>();
			}

			[System.Serializable]
			public class BlackboardCashContainer
			{
				public List<int> classeNameIndexes { get { return m_classeNameIndexes; } }
				public List<string> keys { get { return m_keys; } }
				public List<string> memos { get { return m_memos; } }
				public List<bool> isStatics { get { return m_isStatics; } }

				[SerializeField]
				List<int> m_classeNameIndexes = new List<int>();
				[SerializeField]
				List<string> m_keys = new List<string>();
				[SerializeField]
				List<string> m_memos = new List<string>();
				[SerializeField]
				List<bool> m_isStatics = new List<bool>();
			}
		}
	}
}