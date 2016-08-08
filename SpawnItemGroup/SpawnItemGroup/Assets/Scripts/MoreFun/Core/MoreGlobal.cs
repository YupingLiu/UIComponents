using MoreFun;
using UnityEngine;
using MoreFun.Utils;

public static class MoreGlobal
{
    public static bool IsValid(this object obj)
    {
        if(null != obj)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public static bool IsValid(this string obj)
    {
        return !(string.IsNullOrEmpty(obj));
    }

    public static bool IsValid(this Component obj)
    {
        if(null != obj && null != obj.gameObject)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    
    public static bool IsValid(this System.Collections.IList collection, int index)
    {
        return CollectionUtil.IsValidIndex(collection, index);
    }

    /// <summary>
    /// <para>Check a chain of objects for their validation.</para>
    /// <para>Note, you can call <c>IsValid(obj1, obj2)</c>.  
    /// <para>But do NOT call like this: </para>
    /// <para><c>IsValid(obj1, obj1.obj2)</c></para>, 
    /// because when you call <c>obj1.obj2</c>, it's NOT guaranteed that <c>obj1</c> is not null, 
    /// and may cause exception!</para>
    /// </summary>
    /// <returns><c>true</c> if is valid the specified list; otherwise, <c>false</c>.</returns>
    /// <param name="list">List.</param>
    public static bool IsValid(params object[] list)
    {
        if(null != list)
        {
            for(int i = 0; i < list.Length; ++i)
            {
                if(null == list[i])
                {
                    return false;
                }
            }
            
            return true;
        }
        else
        {
            return false;
        }
    }
}

