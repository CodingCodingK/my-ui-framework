local cPanelMgr = CS.CAE.Core.PanelMgr.Instance
local PanelMgr = supercline.class("PanelMgr")

function PanelMgr:Ctor()

    self.panels = {}
end

-- 新开窗口并注册进panels的方法
function PanelMgr:Open(prefabPath, show)
    if self.panels[prefabPath] then
        print(string.format("PanelMgr:Open//%s is exist.", prefabPath))
        return
    end

    self.panels[prefabPath] = {}

    -- 调用c#端的 PanelMgr 的Open方法接口。也就在这里面，会触发c#的OnOpen。
    cPanelMgr:Open(prefabPath, show)

    return self:GetPanel(prefabPath)
end

function PanelMgr:Close(prefabPath)
    if not self.panels[prefabPath] then
        print(string.format("PanelMgr:Close//%s not exist.", prefabPath))
        return
    end

    cPanelMgr:Close(prefabPath)

    self.panels[prefabPath] = nil
end

function PanelMgr:Show(prefabPath)
    if not self.panels[prefabPath] then
        print(string.format("PanelMgr:Show//%s not exist.", prefabPath))
        return
    end

    cPanelMgr:Show(prefabPath)
end

function PanelMgr:Hide(prefabPath)
    if not self.panels[prefabPath] then
        print(string.format("PanelMgr:Hide//%s not exist.", prefabPath))
        return
    end

    cPanelMgr:Hide(prefabPath)
end

function PanelMgr:GetPanel(prefabPath)
    if not self.panels[prefabPath] then
        print(string.format("PanelMgr:GetPanel//%s not exist.", prefabPath))
        return
    end

    return self.panels[prefabPath].Instance
end


-- 在c#侧得ULuaPanel中OnCreate方法，通过LuaPanelMgr调用到该接口
function PanelMgr:NewPanel(panelName, prefabPath, transform, gameObject)

    assert(supercline.classes[panelName], "unknown panel class: "..panelName)
    
    local cls = supercline.classes[panelName]
    -- Q：这里调用了类的实例化，那么该类【panelName】的Ctor构造方法，是在哪里定义的呢？类【panelName】又是什么时候加入字典的呢？
    -- A：Ctor构造方法在具体XXPanel.lua文件中改写，类【panelName】是在此时加入字典的。
    -- 综上，Instance就是在lua用cls.New出来的具体Panel实例
    local obj = cls.New(transform, gameObject)
    
    self.panels[prefabPath].Instance = obj
end

function PanelMgr:NewPanelItem(itemName, transform, gameObject)

    assert(supercline.classes[itemName], "unknown panel item class: "..itemName)

    local cls = supercline.classes[itemName]
    local obj = cls.New(transform, gameObject)

    return obj
end

function PanelMgr:OnOpen(prefabPath, controls)
    print('test,onpen!',prefabPath,controls)
    self.panels[prefabPath].Instance:OnOpen(controls)
end

function PanelMgr:OnShow(prefabPath)
    self.panels[prefabPath].Instance:OnShow()
end

function PanelMgr:OnHide(prefabPath)
    self.panels[prefabPath].Instance:OnHide()
end

function PanelMgr:OnClose(prefabPath)
    self.panels[prefabPath].Instance:OnClose()
end


function PanelMgr:OnClick(prefabPath, btn)
    self.panels[prefabPath].Instance:OnClick(btn)
end

function PanelMgr:OnInputValueChanged(prefabPath, input, val)
    self.panels[prefabPath].Instance:OnInputValueChanged(input, val)
end

function PanelMgr:OnInputEndEdit(prefabPath, input, val)
    self.panels[prefabPath].Instance:OnInputEndEdit(input, val)
end

function PanelMgr:OnToggleValueChanged(prefabPath, tog, val)
    self.panels[prefabPath].Instance:OnToggleValueChanged(tog, val)
end

function PanelMgr:OnSliderValueChanged(prefabPath, slider, val)
    self.panels[prefabPath].Instance:OnSliderValueChanged(slider, val)
end

function PanelMgr:OnLoopGridValueChanged(prefabPath, grid, item, index)
    self.panels[prefabPath].Instance:OnLoopGridValueChanged(grid, item, index)
end

function PanelMgr:OnDown(prefabPath, go)
    self.panels[prefabPath].Instance:OnDown(go)
end

function PanelMgr:OnUp(prefabPath, go)
    self.panels[prefabPath].Instance:OnUp(go)
end

function PanelMgr:OnEnter(prefabPath, go)
    self.panels[prefabPath].Instance:OnEnter(go)
end

function PanelMgr:OnExit(prefabPath, go)
    self.panels[prefabPath].Instance:OnExit(go)
end

function PanelMgr:OnLongPress(prefabPath, go)
    self.panels[prefabPath].Instance:OnLongPress(go)
end

function PanelMgr:OnLongPressEnd(prefabPath, go)
    self.panels[prefabPath].Instance:OnLongPressEnd(go)
end

function PanelMgr:OnDragStart(prefabPath, go, eventData)
    self.panels[prefabPath].Instance:OnDragStart(go, eventData)
end

function PanelMgr:OnDrag(prefabPath, go, eventData)
    self.panels[prefabPath].Instance:OnDrag(go, eventData)
end

function PanelMgr:OnDragEnd(prefabPath, go, eventData)
    self.panels[prefabPath].Instance:OnDragEnd(go, eventData)
end

function PanelMgr:OnClickItem(item, btn)
    item:PlayClickAudio(btn)
end

-- 在这里，执行力本案例的操作！！！
-- supercline.Singleton.PanelMgr = supercline.PanelMgr.New()
function PanelMgr:Main()
    supercline.Singleton.PanelMgr:Open(supercline.Prefab.PanelLogin, true)
end

return PanelMgr