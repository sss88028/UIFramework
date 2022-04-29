using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace CCTU.UIFramework
{
	public abstract class UIViewerBase<T> : MonoBehaviour, IUIState where T : UIViewerBase<T>
	{
		#region public-property
		public static T Instance
		{
			get;
			protected set;
		}
		#endregion public-property

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
		#endregion public-method

		#region protected-method

		protected virtual void OnAwakeEventHandler()
		{
		}

		protected virtual void OnDestroyEventHandler()
		{
		}
		#endregion protected-method

		#region private-method
		private void Awake()
		{
			if (Instance != null && Instance != this)
			{
				Destroy(this);
				return;
			}
			Instance = (T)this;
			OnAwakeEventHandler();
		}

		private void OnDestroy()
		{
			if (Instance == null || Instance != this)
			{
				return;
			}
			OnDestroyEventHandler();
			Instance = null;
		}
		#endregion private-method
	}
}