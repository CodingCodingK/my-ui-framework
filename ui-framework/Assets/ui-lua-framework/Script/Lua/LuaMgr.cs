﻿/*------------------------------------------------------------------------------
|
| COPYRIGHT (C) 2020 - 2026 All Right Reserved
|
| FILE NAME  : \Assets\Script\Lua\LuaMgr.cs
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
    using XLua;
    using System;
    using System.IO;
    using System.Collections.Generic;

    public sealed class LuaMgr
    {
        private LuaEnv mLuaState = null;

        public static LuaMgr Instance { get; } = Activator.CreateInstance<LuaMgr>();

        public ILuaPanelMgr LuaPanelMgr
        { get; private set; }

        public void Init()
        {
            mLuaState = new LuaEnv();
            // TODO 20210722 看到看不懂路径为什么在LuaProject\script文件夹里
            mLuaState.AddLoader(HandleLoad);
            mLuaState.DoString("require('script.init')");

            NewLuaPanelMgr newFunc = mLuaState.Global.GetInPath<NewLuaPanelMgr>("supercline.lua.PanelMgr");
            LuaPanelMgr = newFunc();

            // TO CLine: test main, u can add another interface to do.
            LuaPanelMgr.Main();
        }

        public void Destroy()
        {
            LuaPanelMgr = null;

            mLuaState.Dispose();
        }

        /// <summary>
        /// 自定义Loader，替换.为/，读取.lua文件。资源读取路径：ui-lua-framework（Unity工程名）/LuaProject
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        private byte[] HandleLoad(ref string filepath)
        {
#if UNITY_EDITOR
            filepath = filepath.Replace('.', '/');

            string fullPath = Path.Combine(ResHelper.RootCurrent+ "LuaProject", filepath);
            fullPath += ".lua";

            return ReadBytes(fullPath);
#endif
        }

        private byte[] ReadBytes(string path)
        {
            if (File.Exists(path))
                return File.ReadAllBytes(path);

            return null;
        }

    }

}