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
			_uiHeaps = new UIHeap[_heapTypeCount];
			for (var i = 0; i < _heapTypeCount; ++i) 
			{
				_uiHeaps[i] = new UIHeap();
			}
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
			var oldTops = GetTops().ToList();
			var wasInHeap = IsInHeap(uiController);
			var hadInTop = oldTops.Any(c => c == uiController);
			var bits = (uint)uiType;
			var index = 0;
			var tasks = new List<Task>();
			while (bits != 0)
			{
				if ((bits & 1) != 0)
				{
					_uiHeaps[index].Push(uiController);
				}
				++index;
				bits = bits >> 1;
			}
			var newTops = GetTops().ToList();
			var needPause = oldTops.Where(u => !newTops.Contains(u)).ToList();

			foreach (var pausedController in needPause) 
			{
				tasks.Add(pausedController.OnPause());
			}
			await Task.WhenAll(tasks);

			if (!wasInHeap)
			{
				await uiController.OnEnter();
				await uiController.OnResume();
			}
			else if (!hadInTop)
			{
				await uiController.OnResume();
			}
		}

		public async Task PopUI(int uiType, UIControllerBase uiController)
		{
			var oldTops = GetTops().ToList();
			var bits = (uint)uiType;
			var index = 0;
			while (bits != 0)
			{
				if ((bits & 1) != 0)
				{
					_uiHeaps[index].Pop(uiController);
				}
				++index;
				bits = bits >> 1;
			}
			var newTops = GetTops().ToList();
			var needResume = newTops.Where(u => !oldTops.Contains(u)).ToList();

			if (oldTops.Contains(uiController))
			{
				await uiController.OnPause();
				await uiController.OnExit();
			}

			var tasks = new List<Task>();
			foreach (var pausedController in needResume)
			{
				tasks.Add(pausedController.OnResume());
			}
			await Task.WhenAll(tasks);

		}
		#endregion public-method

		#region private-method

		private bool IsInHeap(UIControllerBase uiController)
		{
			return (from heap in _uiHeaps
					where heap.HasUnit(uiController)
					select heap).Any();
		}

		private IEnumerable<UIControllerBase> GetTops() 
		{
			return (from heap in _uiHeaps
					where heap.Top != null
					select heap.Top).Distinct();
		}

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