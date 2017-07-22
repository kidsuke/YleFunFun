using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UnityEngine.UI
{
    public abstract class LoopScrollDataSource
    {
        public abstract void ProvideData(Transform transform, int idx);
    }

	public class LoopScrollSendIndexSource : LoopScrollDataSource
    {
		public static readonly LoopScrollSendIndexSource Instance = new LoopScrollSendIndexSource();

		LoopScrollSendIndexSource(){}

        public override void ProvideData(Transform transform, int idx)
        {
            transform.SendMessage("ScrollCellIndex", idx);
        }
    }

	public class LoopScrollArraySource<T> : LoopScrollDataSource
    {
        T[] objectsToFill;

		public LoopScrollArraySource(T[] objectsToFill)
        {
            this.objectsToFill = objectsToFill;
        }

        public override void ProvideData(Transform transform, int idx)
        {
            transform.SendMessage("ScrollCellContent", objectsToFill[idx]);
        }
    }

	public class LoopScrollListSource<T> : LoopScrollDataSource
	{
		List<T> data;

		public LoopScrollListSource(List<T> data)
		{
			this.data = data;
		}

		public override void ProvideData(Transform transform, int idx)
		{
			transform.SendMessage("ScrollCellContent", data[idx]);
		}
	}
}