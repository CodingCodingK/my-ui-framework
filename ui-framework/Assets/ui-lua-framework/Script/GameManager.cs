/*------------------------------------------------------------------------------
|
| COPYRIGHT (C) 2020 - 2026 All Right Reserved
|
| FILE NAME  : \Assets\Script\GameManager.cs
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

    public sealed class GameManager : MonoBehaviour
    {
        private static GameManager ms_instance = null;
        public static GameManager Instance
        {
            get { return ms_instance; }
        }

        /// <summary>
        /// 本脚本挂在GameManager游戏物体上，负责各模块Manager的初始化与销毁
        /// </summary>
        private void Awake()
        {
            // 单例获取
            ms_instance = this;
            DontDestroyOnLoad(gameObject);

            // 用工厂方法模式给mMgr赋值。UnityEditor情况赋本地路径，其他情况赋ab包路径。
            ResourceMgr.Instance.Init();
            // 把几个开始就存在于场景中的UI获取到，并加入 DontDestroyOnLoad 转场不销毁
            UIMgr.Instance.Init();
            // 所有的Panel存放字典、Layer存放字典都在这里。字典相关方法、PanelBase的一些基本方法比如Open、Close也在此。PanelBase自己的OnOpen也会在这被手动调用。
            PanelMgr.Instance.Init();
            // TODO 0723
            LuaMgr.Instance.Init();
            
        }

        private void OnEnable()
        {

        }

        private void Start()
        {
            
        }

        private void OnDisable()
        {

        }

        private void OnDestroy()
        {
            PanelMgr.Instance.Destroy();
            LuaMgr.Instance.Destroy();

            UIMgr.Instance.Destroy();
            ResourceMgr.Instance.Destroy();
        }

        private void FixedUpdate()
        {

        }

        private void Update()
        {

        }

        private void LateUpdate()
        {

        }

        private void OnApplicationPause(bool pause)
        {

        }

        private void OnApplicationFocus(bool focus)
        {

        }

        private void OnApplicationQuit()
        {
            ms_instance = null;
        }
    }
}