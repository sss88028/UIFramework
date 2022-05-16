using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CCTU.UIFramework
{
	public abstract class UIControllerBase : IUIState
	{
		#region private-field
		private SemaphoreSlim _semaphore = new SemaphoreSlim(1);
		#endregion private-field

		#region public-method
		public async Task OnEnter()
		{
			await _semaphore.WaitAsync();
			await OnEnterHandler();
			_semaphore.Release();
		}

		public async Task OnPause()
		{
			await _semaphore.WaitAsync();
			await OnPauseHandler();
			_semaphore.Release();
		}

		public async Task OnResume()
		{
			await _semaphore.WaitAsync();
			await OnResumeHandler();
			_semaphore.Release();
		}

		public async Task OnExit()
		{
			await _semaphore.WaitAsync();
			await OnExitHandler();
			_semaphore.Release();
		}

		public virtual Task HandleUIEvent(IUIEvent uiEvent)
		{
			return Task.CompletedTask;
		}

		public virtual void RegisterEvent(Dictionary<Type, UIControllerBase> dict)
		{
		}
		#endregion public-method

		#region protected-method
		protected virtual Task OnEnterHandler()
		{
			return Task.CompletedTask;
		}

		protected virtual Task OnPauseHandler()
		{
			return Task.CompletedTask;
		}

		protected virtual Task OnResumeHandler()
		{
			return Task.CompletedTask;
		}

		protected virtual Task OnExitHandler()
		{
			return Task.CompletedTask;
		}
		#endregion protected-method
	}
}