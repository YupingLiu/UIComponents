using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MoreFun
{
    /// <summary>
    /// LuaBehaviour = Lua + MonoBehaviour.
    /// 
    /// 由于项目中途决定暂不用Lua，现LuaBehaviour还处于构思阶段，尚未实现完成。
    /// 
    /// LuaBehaviour的设计初衷是，用户可以沿用MonoBehaviour的思想，
    /// 仿佛仅仅是将c#语言切换至lua语言一样进行模块设计、模块划分、模块依赖。
    /// 
    /// 用户将LuaBehaviour这个组件挂接到GameObject上，并且给它绑定具体的Lua代码文本资源，
    /// 然后这个Lua代码在运行时也能：
    ///     - 得到用户通过Inspector绑定的参数对象
    ///     - 得到需要的消息函数（Awake、Start等）
    ///     - 获取到其他Lua脚本进行调用
    /// 
    /// 难点在于，需要设计一个合理的机制，让Lua脚本间能够正确的互相依赖，
    /// 而且Lua脚本能够正确地从属于另一个Lua脚本。
    /// 比如，
    /// 商城视图MallView这个Prefab由Lua脚本MallViewControl.lua进行逻辑控制，
    /// 背包视图BagView这个Prefab由Lua脚本BagViewControl.lua进行逻辑控制，
    /// 并且MallView和BagView内部都使用了基础组件ItemPrefab，其由ItemPrefab.lua进行逻辑控制,
    /// 怎么实现这个MallViewControl.lua依赖一些ItemPrefab.lua实例、BagViewControl.lua依赖另外一些背包的ItemPrefab.lua实例，
    /// 是需要解决的问题
    /// </summary>
    public class LuaBehaviour : MonoBehaviour
    {
        public TextAsset luaText;

        public const string FUNC_INITIALIZE = "Initialize";
        public const string FUNC_AWAKE = "Awake";
        public const string FUNC_ON_ENABLE = "OnEnable";
        public const string FUNC_START = "Start";
        public const string FUNC_ON_DISABLE = "OnDisable";
        public const string FUNC_ON_DESTROY = "OnDestroy";

        public const string FUNC_UPDATE = "Update";
        public const string FUNC_FIXEDUPDATE = "FixedUpdate";
        public const string FUNC_LATEUPDATE = "LateUpdate";

        public UnityEngine.Object[] unityParams;

        void Awake()
        {
            LuaScript script = new LuaScript();
            script.ReadFromCode(luaText.text, luaText.name);
            //LuaManager.me.DoScript(script);

        }

        void Start()
        {
        } 
    }
}
