using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilitys.Timer
{
    public class TimerManager : Singleton<TimerManager>
    {
        // Start is called before the first frame update
        protected override void Awake()
        {
            base.Awake();
            Debug.Log("TimerManager Start");
        }                   
       
        private void Update()
        {
            MotherTimer.Inst.Update();                
        }   
    }
}
