using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CCTU.UIFramework
{
    public interface IUIEvent
    {
        public int Priority
        {
            get;
        }
    }
}