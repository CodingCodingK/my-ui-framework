/*------------------------------------------------------------------------------
|
| COPYRIGHT (C) 2020 - 2026 All Right Reserved
|
| FILE NAME  : \Assets\Script\UI\PanelMgr.cs
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
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    public sealed class PanelMgr
    {
        public static PanelMgr Instance { get; } = Activator.CreateInstance<PanelMgr>();

        /// <summary>
        /// 固定值string return "Layer"
        /// </summary>
        private readonly string mPrefix = "Layer";
        
        /// <summary>
        /// 所有的Layer字典。它的值Layer是用来给新Panel设置Parent的。
        /// </summary>
        private Dictionary<int, RectTransform> mLayerHash = new Dictionary<int, RectTransform>();
        
        /// <summary>
        /// 所有的Panel存放字典
        /// </summary>
        private Dictionary<string, PanelBase> mPanelHash = new Dictionary<string, PanelBase>();

        /// <summary>
        /// 所有的Panel存放字典、Layer存放字典都在这里。字典相关方法、PanelBase的一些基本方法比如Open、Close也在此。PanelBase自己的OnOpen也会在这被手动调用。
        /// </summary>
        public void Init()
        {

        }

        /// <summary>
        /// 销毁全部：清空层级字典mLayerHash 和 面板字典mPanelHash
        /// </summary>
        public void Destroy()
        {
            using (var itr = mPanelHash.GetEnumerator())
            {
                while (itr.MoveNext())
                {
                    DestroyPanel(itr.Current.Value);
                }
            }
            mPanelHash.Clear();

            using (var itr = mPanelHash.GetEnumerator())
            {
                while (itr.MoveNext())
                {
                    GameObject.Destroy(itr.Current.Value.gameObject);
                }
            }
            mLayerHash.Clear();
        }

        /// <summary>
        /// 打开Panel：寻找字典中的指定panel运行其OnOpen()方法，如果没有，就创建一个加入字典。
        /// </summary>
        /// <param name="prefabPath"></param>
        /// <param name="show">初始状态：panel是否显示</param>
        public void Open(string prefabPath, bool show = true)
        {
            PanelBase panel;
            if (!mPanelHash.TryGetValue(prefabPath, out panel))
            {
                panel = CreatePanel(prefabPath);
            }

            // 找到panel后做的事：
            panel.OnOpen();

            if (show)
            {
                Show(prefabPath);
            }
            else
            {
                //panel.gameObject.SetActive(false);
                SetActive(panel, false);
            }
        }

        /// <summary>
        /// 关闭Panel：从资源字典移除panel
        /// </summary>
        /// <param name="prefabPath"></param>
        public void Close(string prefabPath)
        {
            PanelBase panel;
            if (mPanelHash.TryGetValue(prefabPath, out panel))
            {
                DestroyPanel(panel);
                mPanelHash.Remove(prefabPath);
            }
        }

        /// <summary>
        /// 显示Panel
        /// </summary>
        /// <param name="prefabPath"></param>
        public void Show(string prefabPath)
        {
            PanelBase panel;
            if (mPanelHash.TryGetValue(prefabPath, out panel))
            {
                //panel.gameObject.SetActive(true);
                SetActive(panel, true);

                panel.OnShow();
            }
        }

        /// <summary>
        /// 隐藏Panel
        /// </summary>
        /// <param name="prefabPath"></param>
        public void Hide(string prefabPath)
        {
            PanelBase panel;
            if (mPanelHash.TryGetValue(prefabPath, out panel))
            {
                //panel.gameObject.SetActive(false);
                SetActive(panel, false);

                panel.OnHide();
            }
        }

        /// <summary>
        /// 显示/隐藏Panel：控制 CanvasGroup 的 透明度alpha 与 射线检测开关blocksRaycasts
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="active"></param>
        private void SetActive(PanelBase panel, bool active)
        {
            if (active)
            {
                panel.CanvasGroup.alpha = 1;
                panel.CanvasGroup.blocksRaycasts = true;
            }
            else
            {
                panel.CanvasGroup.alpha = 0;
                panel.CanvasGroup.blocksRaycasts = false;
            }
        }

        /// <summary>
        /// 创建Panel：预制体需继承PanelBase。
        /// 创建完后设置 Prefab(其实是它的Path) 和 CanvasGroup
        /// ** 根据PanelBase.PanelLayer作为key，从Layer字典里取出或创建对应的layer，设置为parent
        /// </summary>
        /// <param name="prefabPath"></param>
        /// <returns></returns>
        private PanelBase CreatePanel(string prefabPath)
        {
            GameObject go = GameObject.Instantiate(ResourceMgr.Instance.LoadGameObject(prefabPath));
            PanelBase panel = go.GetComponent<PanelBase>();
            panel.Prefab = prefabPath;
            panel.CanvasGroup = go.AddComponent<CanvasGroup>();
            Transform parent = GetLayer(panel.PanelLayer);
            RectTransform rect = go.transform as RectTransform;
            AddUIChild(rect, parent as RectTransform);
            
            // 注解： Puts the panel to the front as it is now the last UI element to be drawn. 放到最后被渲染，离摄像机最近。
            rect.SetAsLastSibling();
            
            panel.BuildControl();
            panel.OnCreate();

            mPanelHash.Add(prefabPath, panel);

            return panel;
        }

        /// <summary>
        /// 销毁Panel：执行panel的OnHide()、OnClose()，销毁 GameObject 并从 mPanelHash字典 中去除
        /// </summary>
        /// <param name="panel"></param>
        private void DestroyPanel(PanelBase panel)
        {
            if (panel != null)
            {
                panel.OnHide();
                panel.OnClose();

                GameObject.Destroy(panel.gameObject);

                // TO CLine: release asset
            }
        }

        /// <summary>
        /// 获取层级：TODO 疑问点：mLayerHash作用是什么？
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private RectTransform GetLayer(int index)
        {
            RectTransform layer;
            // 如果层级字典中找不到，就创建
            if (!mLayerHash.TryGetValue(index, out layer))
            {
                GameObject go = new GameObject(mPrefix + index);
                // UI2DRoot.gameObject的Layer层是固定的，Layer层 = UI
                go.layer = UIMgr.Instance.UI2DRoot.gameObject.layer;

                layer = go.AddComponent<RectTransform>();
                // 把UI2DRoot设置为该layer的父亲
                AddUIChild(layer, UIMgr.Instance.UI2DRoot);
                layer.anchorMin = Vector2.zero;
                layer.anchorMax = Vector2.one;
                layer.sizeDelta = Vector2.zero;

                // 发番号，取Key号，起始2，步长1。
                layer.SetSiblingIndex(GetSiblingIndex(index));
                mLayerHash[index] = layer;
            }

            return layer;
        }

        /// <summary>
        /// 获得兄弟索引，即同层下的排位
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private int GetSiblingIndex(int index)
        {
            int siblingIndex = 2;

            using (var itr = mLayerHash.Keys.GetEnumerator())
            {
                while (itr.MoveNext())
                {
                    if (itr.Current < index)
                    {
                        ++siblingIndex;
                    }
                }
            }

            return siblingIndex;
        }

        /// <summary>
        /// Transform.SetParent的封装，保持其local位置不变 TODO 需验证目的性
        /// </summary>
        /// <param name="child"></param>
        /// <param name="parent"></param>
        public static void AddUIChild(RectTransform child, RectTransform parent)
        {
            Vector3 localScale = child.localScale;
            Vector3 localPosition = child.anchoredPosition3D;
            Quaternion localRotation = child.localRotation;

            Vector2 offsetMax = child.offsetMax;
            Vector2 offsetMin = child.offsetMin;
            Vector2 sizeDelta = child.sizeDelta;

            child.SetParent(parent);

            child.localScale = localScale;
            child.anchoredPosition3D = localPosition;
            child.localRotation = localRotation;

            child.offsetMax = offsetMax;
            child.offsetMin = offsetMin;
            child.sizeDelta = sizeDelta;
        }

    }

}