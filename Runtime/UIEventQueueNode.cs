using Priority_Queue;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CCTU.UIFramework
{
	internal class UIEventQueueNode : FastPriorityQueueNode
	{
		#region private-field
		private static Queue<UIEventQueueNode> _queue = new Queue<UIEventQueueNode>();
		#endregion private-field

		#region public-property
		public IUIEvent Event
		{
			get;
			private set;
		}

		public TaskCompletionSource<bool> TaskCompletionSource
		{
			get;
			private set;
		}
		#endregion public-property

		#region public-method
		public static UIEventQueueNode CreateEvent(IUIEvent uiEvent)
		{
			if (_queue.Count > 0)
			{
				var t = _queue.Dequeue();
				t.Event = uiEvent;
				t.TaskCompletionSource = new TaskCompletionSource<bool>();
				return t;
			}
			var n = new UIEventQueueNode();
			n.Event = uiEvent;
			n.TaskCompletionSource = new TaskCompletionSource<bool>();
			return n;
		}

		public void Use()
		{
			_queue.Enqueue(this);
		}
		#endregion public-method

		#region private-field
		private UIEventQueueNode()
		{
		}
		#endregion private-field
	}

}