using Priority_Queue;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace CCTU.UIFramework
{
    public class UIManager : Singleton<UIManager>
	{
		#region private-method
		private FastPriorityQueue<UIEventQueueNode> _eventQueue = new FastPriorityQueue<UIEventQueueNode>(100);
		private Dictionary<Type, UIControllerBase> _eventDict = new Dictionary<Type, UIControllerBase>();

		private const int _heapTypeCount = 32;
		private UIHeap[] _uiHeaps = new UIHeap[_heapTypeCount];

		private CancellationTokenSource _cts;
		#endregion private-method

		#region public-method
		public UIManager()
		{
		}

		public void Init()
		{
			InitUIControllers();
			HandleEvents();
		}

		public async Task TriggerUIEvent(IUIEvent uiEvent)
		{
			if (uiEvent.Priority == 0)
			{
				await SendEvent(uiEvent);
			}
			else
			{
				var queueNode = UIEventQueueNode.CreateEvent(uiEvent);
				_eventQueue.Enqueue(queueNode, uiEvent.Priority);
				await queueNode.TaskCompletionSource.Task;
			}
		}

		public async Task PushUI(int uiType, UIControllerBase uiController)
		{
			var bits = (uint)uiType;
			var index = 0;
			var tasks = new List<Task>();
			while (bits != 0)
			{
				if ((bits & 1) != 0)
				{
					if (_uiHeaps[index] == null)
					{
						_uiHeaps[index] = new UIHeap();
					}
					tasks.Add(_uiHeaps[index].Push(uiController));
				}
				++index;
				bits = bits >> 1;
			}
			await Task.WhenAll(tasks);
		}

		public async Task PopUI(int uiType, UIControllerBase uiController)
		{
			var bits = (uint)uiType;
			var index = 0;
			var tasks = new List<Task>();
			while (bits != 0)
			{
				if ((bits & 1) != 0)
				{
					if (_uiHeaps[index] != null)
					{
						tasks.Add(_uiHeaps[index].Pop(uiController));
					}
				}
				++index;
				bits = bits >> 1;
			}
			await Task.WhenAll(tasks);
		}
		#endregion public-method

		#region private-method
		private void InitUIControllers()
		{
			var type = typeof(UIControllerBase);

			var types = AppDomain.CurrentDomain.GetAssemblies().
				SelectMany(ass => ass.GetTypes()).
				Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(type));
			foreach (var t in types)
			{
				((UIControllerBase)Activator.CreateInstance(t)).RegisterEvent(_eventDict);
			}
		}

		private async void HandleEvents()
		{
			try
			{
				_cts?.Cancel();
				_cts?.Dispose();
				_cts = new CancellationTokenSource();
				var token = _cts.Token;
				while (!token.IsCancellationRequested)
				{
					if (_eventQueue.Count == 0)
					{
						await Task.Yield();
						continue;
					}

					var eventNode = _eventQueue.Dequeue();
					var evt = eventNode.Event;
					await SendEvent(evt);
					eventNode.TaskCompletionSource.SetResult(true);
					eventNode.Use();
				}
			}
			catch (Exception ex)
			{
				Debug.LogError(ex);
				HandleEvents();
			}
		}

		private async Task SendEvent(IUIEvent uiEvent)
		{
			if (_eventDict.TryGetValue(uiEvent.GetType(), out var uiControllerBase))
			{
				await uiControllerBase.HandleUIEvent(uiEvent);
			}
		}
		#endregion private-method
	}
}