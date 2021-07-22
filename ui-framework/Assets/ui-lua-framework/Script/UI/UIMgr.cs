/*------------------------------------------------------------------------------
|
| COPYRIGHT (C) 2020 - 2026 All Right Reserved
|
| FILE NAME  : \Assets\Script\UI\UIMgr.cs
| AUTHOR     : http://supercline.com/
| PURPOSE    :
|
| SPEC       :
|
| MODIFICATION HISTORY
|
| Ver      Date            By              Details
| -----    -----------    -------------   ----------------------
| 1.0      2020-4-4      SuperCLine           Created
|
+-----------------------------------------------------------------------------*/

namespace CAE.Core
{
    using System;
    using UnityEngine;
    using UnityEngine.EventSystems;

    /// <summary>
    /// 存放一些UI相关的通用物体
    /// </summary>
    public sealed class UIMgr
    {
        public static UIMgr Instance { get; } = Activator.CreateInstance<UIMgr>();

        public Canvas UICanvas
        { get; set; }
        public RectTransform UICanvasTransform
        { get; set; }
        public RectTransform UI2DRoot
        { get; set; }
        public EventSystem UI2DEventSystem
        { get; set; }

        /// <summary>
        /// 把几个开始就存在于场景中的UI获取到，并加入 DontDestroyOnLoad 转场不销毁
        /// </summary>
        public void Init()
        {
            GameObject go = GameObject.FindWithTag("UI2DCanvas");
            GameObject.DontDestroyOnLoad(go);

            UICanvas = go.GetComponent<Canvas>();
            UICanvasTransform = go.GetComponent<RectTransform>();
            UI2DRoot = Find(go.transform, "UI2DRoot") as RectTransform;

            UI2DEventSystem = GameObject.FindWithTag("UI2DEventSystem").GetComponent<EventSystem>();
            GameObject.DontDestroyOnLoad(UI2DEventSystem);
        }

        public void Destroy()
        {
            UICanvas = null;
            UICanvasTransform = null;
            UI2DRoot = null;
            UI2DEventSystem = null;
        }

        /// <summary>
        /// 寻找name为指定值的Transform。用GetComponentsInChildren。TODO 确认GetComponentsInChildren是否真的可以找到孙子、曾孙等
        /// </summary>
        /// <param name="tf">父物体(查找的根)</param>
        /// <param name="name">寻找这个名字的</param>
        /// <param name="includeInactive"></param>
        /// <returns></returns>
        private Transform Find(Transform tf, string name, bool includeInactive = false)
        {
            Transform[] childTF = tf.GetComponentsInChildren<Transform>(includeInactive);

            for (int i = 0; i < childTF.Length; ++i)
            {
                if (childTF[i].name == name)
                    return childTF[i];
            }

            return null;
        }

    }
}