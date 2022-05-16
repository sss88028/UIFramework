using CCTU.UIFramework;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class TestController : UIControllerBase
{
	#region public-field
	public string Name;
	#endregion public-field

	#region public-method
	public void TestPush(int type)
	{
		UIManager.Instance.PushUI(type, this);
	}

	public void TestPop(int type)
	{
		UIManager.Instance.PopUI(type, this);
	}
	#endregion public-method

	#region protected-method
	protected override Task OnEnterHandler()
	{
		Debug.Log($"[TestController.OnEnterHandler] {Name}");
		return base.OnEnterHandler();
	}

	protected override Task OnPauseHandler()
	{
		Debug.Log($"[TestController.OnPauseHandler] {Name}");
		return base.OnPauseHandler();
	}

	protected override Task OnResumeHandler()
	{
		Debug.Log($"[TestController.OnResumeHandler] {Name}");
		return base.OnResumeHandler();
	}

	protected override Task OnExitHandler()
	{
		Debug.Log($"[TestController.OnResumeHandler] {Name}");
		return base.OnExitHandler();
	}
	#endregion protected-method
}
