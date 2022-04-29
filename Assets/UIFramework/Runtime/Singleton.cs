using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CCTU.UIFramework
{
	public class Singleton<T> where T : Singleton<T>, new()
	{
		#region protected-field
		protected static T _instance;
		#endregion protected-field

		#region public-property
		public static T Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new T();
				}
				return _instance;
			}
		}
		#endregion public-property
	}
}