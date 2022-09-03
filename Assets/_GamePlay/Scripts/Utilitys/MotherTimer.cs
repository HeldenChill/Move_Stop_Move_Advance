using UnityEngine.Events;
using System;
using System.Collections.Generic;

namespace Utilitys.Timer
{
    public class MotherTimer
    {
        public const int TIMER_ADD_NUM = 10;
        public UnityEvent TimerUpdate;
        private static MotherTimer inst = null;
        private Queue<STimer> sTimers = new Queue<STimer>();

        public static MotherTimer Inst
        {
            get
            {
                if (inst == null)
                {
                    inst = new MotherTimer();
                }
                return inst;
            }
        }

        private MotherTimer()
        {
            //Debug.Log("Mother Timer Instance");
            TimerUpdate = new UnityEvent();
        }


        public void Update()
        {
            TimerUpdate?.Invoke();
        }

        ~MotherTimer()
        {
            TimerUpdate.RemoveAllListeners();
        }

        public void Push(STimer timer, bool checkContain = true)
        {
            if (checkContain)
            {
                if (sTimers.Contains(timer))
                    return;
            }


            sTimers.Enqueue(timer);
        }

        public STimer Pop()
        {
            if (sTimers.Count == 0)
            {
                for(int i = 0; i < TIMER_ADD_NUM; i++)
                {
                    sTimers.Enqueue(new STimer());
                }
            }

            STimer timer = sTimers.Dequeue();
            return timer;
        }
    }
}
