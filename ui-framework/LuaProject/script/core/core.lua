-- 初始化创建一个存放【类】的全局表。
supercline.classes = {}

-- 实现【类】的构造器
local function Construct(cls, obj, ...)
    -- 递归寻找.super父亲
    if cls.super then
        Construct(cls.super, obj, ...)
    end

    -- Ctor = constructor,从父到子逐次执行自己的建造者方法
    if cls.Ctor then
        cls.Ctor(obj, ...)
    end
end

-- 实现【类】
local function class(classname, super)
    -- 先检查类名，不nil、未重复
    assert(classname, "class name is null.")
    assert(not supercline.classes[classname], "duplicate class name: "..classname)

    local cls = {}
    cls.classname = classname
    cls.super = super
    -- Ctor是【类】自己的具体构造方法，由上方的Construct方法调用
    cls.Ctor = false
    
    -- 存放实例字段的地方
    local vtbl = {}
    cls.vtbl = vtbl

    -- 作为【实例】的obj特性，写在这里
    cls.obj_meta = 
    {
        -- 设置存放【字段】的vtbl为【实例】的索引
        __index = vtbl,
        -- 禁止掉在lua给【实例】新增字段的方法
        __newindex = function (tb, k, v)
            local val = v or "nil"
            print("instance don't allow to add field."..cls.classname.."["..k.."]="..tostring(val))
        end
    }

    -- 【类】的New方法在此定义，...是构造方法的入参
    cls.New = function (...)
        local obj = {}
        obj.classname = cls.classname
        Construct(cls,obj,...)
        -- 通过设置元表，禁止掉一切的在lua新增字段的方法
        setmetatable(obj, cls.obj_meta)
        return obj
    end

    -- 设置cls新增，只接function，且放到实例化的vtbl中
    setmetatable(cls, 
    {
        __newindex = function (tb, k, v)
            if type(v) ~= "function" then
                print("not a funton")
                return
            end
            tb.vtbl[k] = v
        end
    })

    -- 父类的vtbl继承给子类的vtbl中
    if super then
        setmetatable(vtbl, {
            __index = function (_, k)
                local val = super.vtbl[k]
                vtbl[k] = val
                return val
            end
        })
    end

    -- 注册新的class类到supercline.classes这个表中
    supercline.classes[classname] = cls
    return cls
end

return class