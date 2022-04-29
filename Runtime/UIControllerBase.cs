using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CCTU.UIFramework
{
	public abstract class UIControllerBase : IUIState
	{
		#region public-method
		public virtual Task OnEnter()
		{
			return Task.CompletedTask;
		}

		public virtual Task OnPause()
		{
			return Task.CompletedTask;
		}

		public virtual Task OnResume()
		{
			return Task.CompletedTask;
		}

		public virtual Task OnExit()
		{
			return Task.CompletedTask;
		}

		public virtual Task HandleUIEvent(IUIEvent uiEvent)
		{
			return Task.CompletedTask;
		}

		public virtual void RegisterEvent(Dictionary<Type, UIControllerBase> dict)
		{
		}
		#endregion public-method
	}
}