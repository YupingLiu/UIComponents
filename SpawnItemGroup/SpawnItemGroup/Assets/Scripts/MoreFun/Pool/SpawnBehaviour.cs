using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoreFun
{
    /// <summary>
    /// 当GameObject接受SpawnPool的管理时，SpawnPool会在GameObject出池/入池分别发送OnSpawn/OnDespawn消息。
    /// SpawnBehaviour的作用：
    /// 1、不用用户硬敲那两个消息函数的名字，而是使用重载，防止出错；
    /// 2、由于是抽象函数，所以能提醒用户必须实现这两个消息函数。
    /// </summary>
    public abstract class SpawnBehaviour : MoreBehaviour
    {
        protected abstract void OnSpawn();

        protected abstract void OnDespawn();
    }
}
