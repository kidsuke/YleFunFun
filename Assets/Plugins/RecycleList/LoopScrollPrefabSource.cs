using UnityEngine;
using System.Collections;

namespace UnityEngine.UI
{
	[System.Serializable]
	public class LoopScrollPrefabSource 
	{
		public GameObject prefab;
		public int poolSize = 5;

		public LoopScrollPrefabSource()
		{}

		public LoopScrollPrefabSource(GameObject prefab, int poolSize = 5)
		{
			this.prefab = prefab;
			this.poolSize = poolSize;
		}

		public virtual void InitPool()
		{
			ResourceManager.Instance.InitPool(prefab, poolSize);
		}
			
		public virtual GameObject GetObject()
		{
			return ResourceManager.Instance.GetObjectFromPool(prefab);
		}
	}
}
