#BaseTimeLine <color="olive">(Beta)</color>

Unity官方文档内容不赘述。
(https://docs.unity3d.com/Manual/TimelineSection.html)
## 快速开始 
***
 1. 新建4个类分别继承Timeline.Behaviour,Timeline.Clip,Timeline.Mixer,Timeline.Track。
 2. 为Track子类添加[TrackClipType(typeof(clip子类))]。重写CreatTrackMixer制定Mixer为Mixer子类，并显式调用SetContext注册上下文引用。（参考Track基类的实现）。
 3. 重写Clip子类CreatPlayable函数指定Behavior子类，并显式调用SetContext注册上下文。(参考Clip基类实现）
 4. 重写Behavior子类ProcessFrame等函数实现Timeline动画。**注意**重写时要调用Base函数。
 5. 重写Mixer子类ProcessFrame等函数实现Timeline动画。**注意**重写时要调用Base函数。
 6. UnityEditor中创建TimelineAsset，创建Timeline轨道并添加对应的Clip
## 说明 
***
 7. Track
 Timeline轨道运行实体，Clip容器。添加类标签指定轨道表现。
    [TrackClipType]绑定的Clip类型。指定以后所有的Clip子类都会出现在右键菜单中直接添加。
    [TrackBindingType]绑定对象类型。指定轨道绑定场景对象在运行时绑定操作对象在behavior.processFrame时可以直接获得操作绑定对象。
    [TrackColor]轨道颜色标签。轨道最左边的颜色显示。
 8. Clip
Timeline片段注意添加[Serializable]保证序列化正确
    重写clipCaps设置片段的融合选项。
    所有需要序列化的设置参数，应该写在Clip中。
    Clip.Await实现了Timeline的暂停功能。如果Await有效则片段运行完毕时所在timeline会暂停。
 9. Behaviour
 片段执行逻辑实现。每一个片段都会对应一个Behavior实例。通过重写基类函数实现多种不同阶段的事件回调。**需要注意的是**保持base函数的调用，除非特殊情况。
    Behaviour.context运行时上下文。通过这个上下文可以获得Clip,Track等对象，实现逻辑时会用到。
    Behaviour.IsEarly判断当前Clip是否还未到*开始*时间。
    Behaviour.IsLate判断当前Clip是否超过*结束*时间。
    Behaviour.IsTimeOut判断当前Clip是否在执行时间内。
 10. Mixer
轨道融合器，每一个Track对应一个Mixer实例。处理Clip融合问题。Mixer本身是Behaviour但是Context中上下文缺少一些引用。
    Mixer.clipA/clipB融合片段。无关乎AB时间永远从A融合到B。不推荐直接重写ProcessFrame，建议使用以下回调。
    Mixer.ProcessZero没有Clip运行时回调。
    Mixer.ProcessOne一个Clip运行时回调。这个回调基本等于对应Behavior.ProcessFrame但是运行时机在它之前。
    Mixer.ProcessTwo2个Clip融合运行时回调。主要融合代码实现在这里，可以同时访问2个ClipA/B通过对应权重处理融合问题。
 10. Event
运行时时间回调，例如Await暂停Resume继续功能。
 11. Reference
 场景引用，因为Unity自带的ExposedReference有大量Bug,同时为了动态绑定Timeline轨道对象。因此实现了自己场景引用。
    -ReferenceObject引用组件挂在对应需要引用的Gameobject上。
    -ReferenceRoot每一个场景有且只有1个，用来记录引用信息，序列化在场景或者Prefab。
    -SceneReference Clip中用来标记场景引用的对象。基本功能和ExposedReference相同。
>Reference
##Generator
     代码生成器。
>Generator
## 细节 
***
 1. TimelineEditorWindow中的回调和运行时的回调不是完全一致的。editor运行为有不同的回调情况。
 2. Editor行为
    开preview(选择director对象时自动打开)
[m]create-create-[m]start-[m]play-[m]prepare-start-play-prepare
    关preview(取消director对象时自动关闭)
[m]pause-pause-[m]stop-[m]destroy-stop-destroy
    设置Timeline 时间点
[m]prepare-prepare-[m]pf-pf
    播放
    如果之前停止过-即stop
[m]start-[m]play-start-play-"[m]prepare-prepare-[m]process-process"否则"[m]prepare-prepare-[m]process-process"
    停止
[m]pause-[m]stop-pause-stop
    编辑器没有暂停按钮 只有start 和Stop每次显示timeline实例就start 然后pause到游标位置。之后拖动都是持续pause状态只设置时间点通过播放(space) 从pause状态 到stop之后无论是拖动还是播放都会再次start

### Clip融合
有2种特殊的clip融合细节 需要仔细考虑。
1·2个clip头尾相接，只有1帧融合。
2·3个clip相接，融合帧当中有1帧是3个clip一起。
