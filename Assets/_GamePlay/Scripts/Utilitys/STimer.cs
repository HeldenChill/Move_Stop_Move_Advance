using UnityEngine;
using UnityEngine.Events;
using System;

namespace Utilitys.Timer
{

    public class STimer
    {
        #region Event
        public event Action FrameUpdate;
        public event Action<int> TimeOut1;
        public event Action<Type, int> TimeOut2;
        public event Action<Vector2Int> TimeOutVector2Int;
        #endregion
        #region Property
        private bool isStart = false;
        private float timeRemaining = 0;
        private int timeFrame = 0;
        private int code = -1;
        private Type type = null;
        private Vector2Int vector2Int;

        public float TimeRemaining
        {
            get => timeRemaining;
        }
        public bool IsStart
        {
            get { return isStart; }
        }
        #endregion
        // Start is called before the first frame update


        public STimer()
        {
            MotherTimer.Inst.TimerUpdate.AddListener(Update);
        }
        public void Start(float time,Vector2Int code)
        {
            vector2Int = code;
            Start(time);
        }

        public void Start(float time,Type type = null,int code = -1)
        {
            this.type = type;
            Start(time, code);
        }
        public void Start(float time, int code = -1)
        {
            this.code = code;
            Start(time);
        }
        public void Start(int frame, int code = -1)
        {
            this.code = code;
            Start(frame);
        }
        public void Start(float time)
        {
            if(time > 0)
            {
                isStart = true;
                timeRemaining = time;
            }
        }
        public void Start(int frame)
        {
            if(frame > 0)
            {
                isStart = true;
                timeFrame = frame + 1;
            }        
        }
        public void Stop()
        {
            isStart = false;
            timeRemaining = 0;
            timeFrame = 0;
        }

        private void TriggerEvent()
        {
            TimeOut1?.Invoke(code);
            TimeOut2?.Invoke(type, code);
            TimeOutVector2Int?.Invoke(vector2Int);
        }
        public void Update()
        {
            if (isStart)
            {
                if(timeRemaining > 0)
                {
                    timeRemaining -= Time.deltaTime;
                    if (timeRemaining <= 0)
                    {
                        Stop();
                        TriggerEvent();
                    }
                }

                if(timeFrame > 0)
                {
                    timeFrame -= 1;
                    if(timeFrame <= 0)
                    {
                        Stop();
                        TriggerEvent();
                    }
                }
                FrameUpdate?.Invoke();
            }
        }

        ~STimer()
        {
            MotherTimer.Inst.TimerUpdate.RemoveListener(Update);
        }
    }
}
