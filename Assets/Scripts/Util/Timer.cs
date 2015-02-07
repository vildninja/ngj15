using System;

namespace GameCore
{
    public class Timer
    {
        private float remaining;
        private bool triggered;
        private bool isRunning = false;
        private Action action;

        public void Update(float dtime)
        {
            remaining -= dtime;
            if (remaining < 0)
            {
                if (!triggered)
                {
                    if (action != null)
                    {
                        action();
                    }
                    isRunning = false;
                    triggered = true;
                }
            }

        }

        private void Start(float time, Action action)
        {
            this.action = action;
            remaining = time;
            isRunning = true;
        }

        public Timer(float time, Action action)
        {
            Start(time, action);
        }

        public bool IsRunning
        {
            get { return isRunning; }
        }
    }
}