using MoreFun;
using UnityEngine;

namespace MoreFun
{
    /// <summary>
    /// <para>You MUST call Release() or Stop() when you don't need to use this timer</para>
    /// <para>RealTimer基于Time.realtimeSinceStartup，不受Time.timeScale的影响。</para>
    /// </summary>
    public class RealTimer
    {
        public delegate void TimerHandler();
        public delegate void TimerCompleteHandler();

        private event TimerHandler OnTimerHandler;
        private event TimerCompleteHandler OnTimerCompleteHandler;
        
        private int m_repeatCount;
        public int RepeatCount
        {
            get { return m_repeatCount; }
            set { m_repeatCount = value; }
        }

        private int m_currentCount;
        private float m_lastInterval;

        private bool m_isRunning = false;
        public bool IsRunning
        {
            get { return m_isRunning; }
        }

        private float m_delay;
        /// <summary>
        /// the delay time of timer in seconds.
        /// </summary>
        /// <value>The delay.</value>
        public float Delay
        {
            get { return m_delay; }
            set { m_delay = value; }
        }
        
        /// <summary>
        /// <para>You MUST call Release() or Stop() when you don't need to use this timer</para>
        /// <para>RealTimer基于Time.realtimeSinceStartup，不受Time.timeScale的影响。</para>
        /// </summary>
        /// <param name="delay">delay time in seconds</param>
        /// <param name="repeatCount">重复次数，小于等于0表示无限循环</param>
        public RealTimer(float delay, int repeatCount = 0)
        {
            m_delay = delay;
            m_repeatCount = repeatCount;
        }

        public void AddTimerHandler(TimerHandler handler)
        {
            OnTimerHandler -= handler;
            OnTimerHandler += handler;
        }

        public void RemoveTimerHandler(TimerHandler handler)
        {
            OnTimerHandler -= handler;
        }

        public void AddTimerCompleteHandler(TimerCompleteHandler handler)
        {
            OnTimerCompleteHandler -= handler;
            OnTimerCompleteHandler += handler;
        }

        public void RemoveTimerCompleteHandler(TimerCompleteHandler handler)
        {
            OnTimerCompleteHandler -= handler;
        }

        private void OnUpdate()
        {
            if (m_isRunning)
            {
                ///http://forum.unity3d.com/threads/realtimesincestartup-is-not-0-in-first-awake-call.205773/
                /// if current time is less than the past (which is impossible),
                /// this means it's unity's bug in editor:
                /// Time.realtimeSinceStartup will return the time elapsed since the last editor play mode.
                /// therefore, in order to fix this bug, here is the bug fix:
                if(m_lastInterval > Time.realtimeSinceStartup)
                {
                    m_lastInterval = Time.realtimeSinceStartup;
                }

                float deltaTime = Time.realtimeSinceStartup - m_lastInterval;

                if (deltaTime > m_delay)
                {
					int addCount = Mathf.FloorToInt(deltaTime / m_delay);
					m_currentCount += addCount;

					//2015-5-6, donaldwu says:
					// this following codes are more accurate, but more complex
					/*
					float compensate = deltaTime - addCount * m_delay;
					m_lastInterval = Time.realtimeSinceStartup - compensate;
					*/

					m_lastInterval = Time.realtimeSinceStartup;
                    
                    if (OnTimerHandler != null)
                    {
                        OnTimerHandler();
                    }
                }

                if (m_repeatCount > 0 && m_currentCount >= m_repeatCount)
                {
                    Stop();

                    if (OnTimerCompleteHandler != null)
                    {
                        OnTimerCompleteHandler();
                    }
                }
            }
        }

        public void Start()
        {
            if(false == m_isRunning)
            {
                m_isRunning = true;
                m_currentCount = 0;
                m_lastInterval = Time.realtimeSinceStartup;
                
                GlobalObject.Instance.AddUpdate(OnUpdate);
            }
        }
        
        /// <summary>
        /// <para>I am glad you remember calling me. ^_^.</para>
        /// <para>Stop the timer, remove the update</para>
        /// </summary>
        public void Stop()
        {
            m_isRunning = false;

            GlobalObject.Instance.RemoveUpdate(OnUpdate);
        }

        public void Reset()
        {
            m_currentCount = 0;
            Stop();
        }

        /// <summary>
        /// <para>I am glad you remember calling me. ^_^.</para>
        /// <para>Stop the timer, remove the update,
        /// and finally clear all event listeners</para>
        /// </summary>
        public void Release()
        {
            Stop();

            OnTimerHandler = null;
            OnTimerCompleteHandler = null;
        }
        
    }

}

