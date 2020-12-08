//作成者 : 植村将太
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Damage
{
	/// <summary>
	/// ダメージをリクエストとして受け取るDamageReceiver
	/// </summary>
	[System.Serializable]
	public class DamageReceiver
	{
		/// <summary>リクエストを貯めるキュー</summary>
		public ReadOnlyCollection<RequestQueue> requestQueue { get; private set; } = null;
		public GameObject gameObject { get; private set; } = null;
		public Transform transform { get; private set; } = null;

		/// <summary>リクエストを貯めるキュー</summary>
		public List<RequestQueue> m_requestQueue { get { return m_drawingRequestQueue; } }
		
		/// <summary>ダメージリクエストを描画</summary>
		[SerializeField, Tooltip("ダメージリクエストを描画"), NonEditable]
		List<RequestQueue> m_drawingRequestQueue = new List<RequestQueue>();

		Dictionary<int, float> m_attackIDs = new Dictionary<int, float>();

		/// <summary>
		/// [Request]
		/// ダメージをリクエストする
		/// 引数1: 攻撃を行うオブジェクト
		/// 引数2: ふっとばし力
		/// 引数3: 攻撃力
		/// 引数4: 吹っ飛びタイプ
		/// </summary>
		public void Request(GameObject attackObject, AttackInfo info)
		{
			if (info.id >= 0 && !m_attackIDs.ContainsKey(info.id))
			{
				m_requestQueue.Add(new RequestQueue(attackObject, 
					info.attack * info.attackScale, info.details, info.id));
				m_attackIDs.Add(info.id, 60.0f);
			}
#if UNITY_EDITOR
			else if (info.id == -1)
			{
				Debug.LogError("DamageRequest.Request: Sender側でActionがEnable化されていませんReceiver[" + gameObject.name + "] Sender[" + attackObject.name + "]");
			}
#endif
		}

		public RequestQueue Pop()
		{
			if (m_requestQueue.Count > 0)
			{
				var result = m_requestQueue[0];
				m_attackIDs.Remove(result.attackID);
				m_requestQueue.RemoveAt(0);
				return result;
			}
			return default;
		}
		public void RemoveBegin()
		{
			if (m_requestQueue.Count > 0)
			{
				m_attackIDs.Remove(m_requestQueue[0].attackID);
				m_requestQueue.RemoveAt(0);
			}
		}
		public void Clear()
		{
			m_requestQueue.Clear();
			m_attackIDs.Clear();
		}

		public void Awake(GameObject gameObject)
		{
			this.gameObject = gameObject;
			this.transform = gameObject.transform;
			requestQueue = new ReadOnlyCollection<RequestQueue>(m_requestQueue);
		}

		public void Update()
		{
			// var keys = m_attackIDs.Keys;
            var keys =  new List<int>(m_attackIDs.Keys);
            foreach (var key in keys)
			{
				m_attackIDs[key] -= Time.deltaTime;
				if (m_attackIDs[key] <= 0.0f)
					m_attackIDs.Remove(key);
			}
		}
	}
}