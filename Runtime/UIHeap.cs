using Priority_Queue;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace CCTU.UIFramework
{
    internal class UIHeap
	{
		public class UIControllerNode : FastPriorityQueueNode
		{
			public readonly UIControllerBase Controller;

			public UIControllerNode(UIControllerBase controller)
			{
				Controller = controller;
			}
		}

		#region private-field
		private int _priotity = 0;
		private const int _defaultHeapSize = 100;
		private FastPriorityQueue<UIControllerNode> _uiQueue = new FastPriorityQueue<UIControllerNode>(_defaultHeapSize);
		private object _lockQueue = new object();

		private Dictionary<UIControllerBase, UIControllerNode> _nodeDict = new Dictionary<UIControllerBase, UIControllerNode>();
		private object _lockDict = new object();

		#endregion private-field

		#region public-property
		public UIControllerBase Top
		{
			get
			{
				lock (_lockQueue)
				{
					if (_uiQueue.Count == 0)
					{
						return null;
					}
					return _uiQueue.First.Controller;
				}
			}
		}
		#endregion public-property

		#region public-method
		public void Push(UIControllerBase controller)
		{
			var hasKey = false;
			UIControllerNode node;
			lock (_lockDict)
			{
				hasKey = _nodeDict.TryGetValue(controller, out node);
			}
			var oldTop = Top;
			if (hasKey)
			{
				lock (_lockQueue)
				{
					_uiQueue.UpdatePriority(node, _priotity--);
				}
			}
			else
			{
				node = new UIControllerNode(controller);
				_nodeDict[controller] = node;
				lock (_lockQueue)
				{
					_uiQueue.Enqueue(node, _priotity--);
				}
			}
		}

		public void Pop(UIControllerBase controller)
		{
			var hasKey = false;
			UIControllerNode node;
			lock (_lockDict)
			{
				hasKey = _nodeDict.TryGetValue(controller, out node);
			}
			if (hasKey)
			{
				lock (_lockQueue)
				{
					_uiQueue.Remove(node);
				}
				lock (_lockDict)
				{
					_nodeDict.Remove(controller);
				}
			}
		}

		public bool HasUnit(UIControllerBase uIController) 
		{
			return _nodeDict.ContainsKey(uIController);
		}
		#endregion public-method
	}
}