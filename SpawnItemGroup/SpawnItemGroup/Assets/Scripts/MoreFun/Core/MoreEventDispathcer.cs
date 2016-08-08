
using System;
using System.Collections.Generic;
namespace MoreFun
{
    public class MoreEventArg
    {
    }

    public delegate void MoreEventHandler(MoreEventArg args);


    public interface IMoreEventDispatcher
    {
        void AddListener(string eventName, MoreEventHandler listener, int priority = 0);
        void RemoveListener(string eventName, MoreEventHandler eventHandler);
        int Dispatch(string eventName, MoreEventArg arg);
    }

    /// <summary>
    /// ---v0.1 by winuxli, donaldwu
    /// 带有优先级的EventHandler,类似As 里面的事件机制
    /// ---
    /// ---v0.2 change by atlas 
    /// 增加try catch功能, 避免多播调用中优先级高的hanlder奔溃导致后续hanlder执行丢失
    /// ---
    /// </summary>
    public class MoreEventDispathcer : IMoreEventDispatcher
    {

        private Dictionary<string, MoreEventHandlerList> m_eventHandler = new Dictionary<string, MoreEventHandlerList>();

        public void AddListener(string eventName, MoreEventHandler listener, int priority = 0)
        {
            if (m_eventHandler.ContainsKey(eventName))
            {
                MoreEventHandlerList eventHandlerList = m_eventHandler[eventName];
                eventHandlerList.HandlerRegister(listener, priority);
            }
            else
            {
                m_eventHandler[eventName] = new MoreEventHandlerList();
                m_eventHandler[eventName].HandlerRegister(listener, priority);
            }
        }

        public void RemoveListener(string eventName, MoreEventHandler listener)
        {
            if (m_eventHandler.ContainsKey(eventName))
            {
                MoreEventHandlerList eventHandlerList = m_eventHandler[eventName];
                eventHandlerList.HandlerUnregister(listener);
            }
        }

        public int Dispatch(string eventName, MoreEventArg arg)
        {
            int triggerCounts = 0;
            if (m_eventHandler.ContainsKey(eventName))
            {
                MoreEventHandlerList eventHandlerList = m_eventHandler[eventName];
                triggerCounts = eventHandlerList.Trigger(arg);
            }

            return triggerCounts;
        }


        private class MorePriorityEventHandler
        {
            public int m_priority;
            public MoreEventHandler m_eventHandler;

            public MorePriorityEventHandler(MoreEventHandler eventHandler, int priority)
            {
                m_priority = priority;
                m_eventHandler = eventHandler;
            }
        }
        private class MoreEventHandlerList
        {
            private List<MorePriorityEventHandler> m_eventHandlerList = new List<MorePriorityEventHandler>();

            public void HandlerRegister(MoreEventHandler eventHandler, int priority)
            {
                for (int i = 0; i < m_eventHandlerList.Count; i++)
                {
                    //If the new eventHandler is exist and it's priority higher than the new one, just return
                    if (eventHandler == m_eventHandlerList[i].m_eventHandler)
                    {
                        if (priority <= m_eventHandlerList[i].m_priority)
                        {
                            return;
                        }
                        //Or remove it first and then insert the new one with higher priority
                        else
                        {
                            m_eventHandlerList.RemoveAt(i);
                        }
                    }
                }
                int handlerInsertIndex = 0;
                int HandlerPriorityIndex = 0;
                int handlerOffet = 0;

                MorePriorityEventHandler priorityEventHandler = new MorePriorityEventHandler(eventHandler, priority);
                for (HandlerPriorityIndex = 0; HandlerPriorityIndex < m_eventHandlerList.Count; HandlerPriorityIndex++)
                {
                    int eventPriority = m_eventHandlerList[HandlerPriorityIndex].m_priority;
                    if (priority >= eventPriority)
                    {
                        // 2015.03.12
                        // Don't care about this
                        /*
                                        for(; handlerOffet + HandlerPriorityIndex < m_eventHandlerList.Count; handlerOffet ++)
                                        {
                                            if(priority < eventPriority)
                                            {
                                                break;
                                            }
                                        }
                        */
                        break;
                    }
                }

                handlerInsertIndex = HandlerPriorityIndex + handlerOffet;
                m_eventHandlerList.Insert(handlerInsertIndex, priorityEventHandler);
            }

            public void HandlerUnregister(MoreEventHandler eventHandler)
            {
                for (int i = 0; i < m_eventHandlerList.Count; i++)
                {
                    MorePriorityEventHandler priorityEventHandler = m_eventHandlerList[i];
                    if (eventHandler == priorityEventHandler.m_eventHandler)
                    {
                        m_eventHandlerList.RemoveAt(i);
                    }
                }
            }

            public int Trigger(MoreEventArg arg)
            {
                int triggerCounts = 0;
                for (int i = 0; i < m_eventHandlerList.Count; i++)
                {
                    MoreEventHandler eventHandler = m_eventHandlerList[i].m_eventHandler;

                    //sometimes m_eventHandlerList.cout maybe more than one
                    Delegate[] arrDelegate = eventHandler.GetInvocationList();

                    for (int index = 0; index < arrDelegate.Length; index++)
                    {
                        try
                        {
                            MoreEventHandler handler = (MoreEventHandler)arrDelegate[i];
                            handler.Invoke(arg);
                        }
                        catch(Exception ex)
                        {
                            //异常就留给机智的小伙伴们吧
                        }
                    }
                    eventHandler(arg);
                    triggerCounts++;
                }
                return triggerCounts;
            }
        }
    }


    //Example
    /*
    namespace Test20150312
    {
        class Program
        {
            class MyArg : MoreFunEventArg
            {
                public int m_value;
                public MyArg(int value)
                {
                    m_value = value;
                }
            }

            static void TestFuncA(MoreFunEventArg arg)
            {
                Console.WriteLine("I am TestFuncA");
            }

            static void TestFuncB(MoreFunEventArg arg)
            {
                Console.WriteLine("I am TestFuncB~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            }

            static void TestFuncC(MoreFunEventArg arg)
            {
                Console.WriteLine("I am TestFuncC~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            }

            static void TestFuncD(MoreFunEventArg arg)
            {
                MyArg myArg = arg as MyArg;
                Console.WriteLine("I am TestFuncD~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                Console.WriteLine(myArg.m_value);
            }

            static void Main(string[] args)
            {
                MoreFunEventArg arg = new MoreFunEventArg();

                MoreFunEventHandlerManager eventHandlerManager = new MoreFunEventHandlerManager();

                eventHandlerManager.HandlerRegister("TestHandler_2", TestFuncC);
                eventHandlerManager.HandlerRegister("TestHandler_2", TestFuncB);
                eventHandlerManager.Trigger("TestHandler_2", arg);

                eventHandlerManager.HandlerRegister("TestHandler_2", TestFuncC, 10);
                eventHandlerManager.HandlerRegister("TestHandler_2", TestFuncC, 9);
                eventHandlerManager.Trigger("TestHandler_2", arg);

                eventHandlerManager.HandlerRegister("TestHandler_2", TestFuncB, 20);
                eventHandlerManager.Trigger("TestHandler_2", arg);

                eventHandlerManager.HandlerUngister("TestHandler_2", TestFuncC);
                eventHandlerManager.Trigger("TestHandler_2", arg);

                eventHandlerManager.HandlerUngister("TestHandler_2", TestFuncB);
                eventHandlerManager.Trigger("TestHandler_2", arg);


                MyArg myArg = new MyArg(1);
                eventHandlerManager.HandlerRegister("TestHandler_3", TestFuncD);
                eventHandlerManager.Trigger("TestHandler_3", myArg);
            }
        }
    }
     */

}
