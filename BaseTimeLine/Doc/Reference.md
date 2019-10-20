# SceneReference 

### 场景引用，用来代替bug层出的ExposedReference。模拟了ExposedReference的操作和API 
核心是GUID匹配。

!(SceneRef.jpg)

## ReferenceObject 

·Component组件，直接添加到需要引用的对象上。
	-ReferenceObject GUID是主键。当引用建立关系建立时GUID以RefObj为准。
	-使用Resolve方法获取绑定对象上真正需要的对象实例。例如 获得Animator :Resolve<Animator>();
	-一个Gameobject只能有1个ReferenceObject绑定。
	-Data 序列化存储对象在场景中的数据。
### ReferenceObject.SceneObject 

序列化存储引用对象的场景数据。可以通过这个记录，实现动态还原场景。
·SceneObject
	-AssetPath 资源路径，按照需求序列化资源读取路径，动态生成场景对象。
	-ScenePath 场景绑定路径。声明绑定到场景中的路径，需要非常小心场景对象时生成顺序。
	-GUID 场景对象唯一ID
	-RefType 引用类型。用来指定ReferenceObject的实际实现类，可以通过生成ReferenceObject的子类重写引用逻辑。
	-其余 记录场景数据。

## SceneReference 

·场景引用声明。代替实体字段声明，实际对象通过Resolve方法获得。
	-需要通过ReferenceRoot找到实际引用对象。
	-TypeName 用来约束默认应用类型。如果是null或空字符串。则返回Gameobject
	-需求明确的场景下使用Resolve<T>()方法获取需要的组件。
	-EditorInspector中如果已经有绑定信息，则会在字段名后显示GUID。如果Resolve则会在对象引用框中显示引用的对象。
	-SceneReference 对象用青色GUI在Editor中显示。
### SceneRefAtt 
Editor表现形式
可选显示 字段名，引用类型名。可以参看文档中截图的不同显示样式。
 [InspectorPlus.SceneRefAtt(ShowFieldStr =false,ShowRefType =true)]
### 特殊用法 
	引用是运行时解析的，而序列化的只有GUID。所以1个场景中GUID不能冲突，但是可以通过不同场景中RefObj统一GUID的方式重用这个引用。

## ReferenceRoot 

·场景引用管理类。每个场景只需要1个实例，用来收集所有需要引用的对象。
	-CollectRefrence() 如果添加了新的ReferenceObject需要重新收集场景中的引用对象。
	-使用AddReference 自动完成引用绑定。
	-或者通过将GUID统一的方法，手动绑定。

## SceneTimeline Rebinder 
Timeline轨道对象动态绑定。
	-ReKey 扫描当前TimelineAsset生成绑定绑定集。一般是Editor下使用。
	-Rebind 通过绑定集绑定场景对象。

!(Rebinder.jpg)

## DynamicName 
动态名称。
在很多情况下，引用对象和GUID都是我们在运行时临时生成。
动态名称，是按照距离逻辑实现约定好的昵称。在运行时按照这个名称生成即可绑定。
!(DynamicName.jpg)

使用方法：
	点击引用对象右侧小窗口，打开详细信息面板。
	开启Dynamic
	设置DynamicName 字符串
运行时 使用ReferenceRoot.SetDynamicName 设置引用和Dynamic的关联。