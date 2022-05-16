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

	public override Task OnEnter()
	{
		Debug.Log($"[TestController.OnEnter] {Name}");
		return base.OnEnter();
	}

	public override Task OnPause()
	{
		Debug.Log($"[TestController.OnPause] {Name}");
		return Task.CompletedTask;
	}

	public override Task OnResume()
	{
		Debug.Log($"[TestController.OnResume] {Name}");
		return Task.CompletedTask;
	}

	public override Task OnExit()
	{
		Debug.Log($"[TestController.OnExit] {Name}");
		return Task.CompletedTask;
	}
	#endregion public-method
}
