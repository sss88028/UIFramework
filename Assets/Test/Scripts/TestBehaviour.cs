using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBehaviour : MonoBehaviour
{
	#region private-field
	[SerializeField]
	private int _type;
	[SerializeField]
	private string _name;

	private int _typeCahce;

	private TestController _controller = new TestController();
	#endregion private-field

	#region public-method
	[NaughtyAttributes.Button("TestPush", NaughtyAttributes.EButtonEnableMode.Playmode)]
	public void TestPush() 
	{
		_controller.TestPush(_typeCahce);
	}

	[NaughtyAttributes.Button("TestPop", NaughtyAttributes.EButtonEnableMode.Playmode)]
	public void TestPop()
	{
		_controller.TestPop(_typeCahce);
	}
	#endregion public-method

	#region MonoBehaviour-method
	private void Start()
	{
		_controller.Name = _name;
		_typeCahce = _type;
	}
	#endregion MonoBehaviour-method
}
