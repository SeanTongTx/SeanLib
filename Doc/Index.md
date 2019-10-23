# SeanLibManager 
### 插件模式集成编辑器功能模块，为所有模块的设置和使用提供一个统一风格的入口。 
/{详细说明
设置外部Package仓库目录，提供预览和导入功能。
-统一处理 UnityPackageManager,UnityPackage
-本地仓库下载仓库的和配置就必须手动了。
-不同的项目共享仓库。
-3种左右场景不同的插件模式。
1·*.unitypackage 古老的插件包模式。
	适合大容量资源包,整包导入项目。例如：几百mb的模型材质贴图。
	或者极小型插件,方便修改无需依赖。例如：一两个cs文件编辑器功能，不依赖其他资源。
2·LocalPlugin UnityPackageManager 导入LocalPackage。
	通过UnityPackageManager 导入本地插件，适合本机多个工程共同开发。
	例如 插件A 在ProjectA中开发。在ProjectB中测试。在ProjectC中集成。
	因为同时访问相同的本地文件，不需要额外的文件同步操作。
	同时在所有项目中将这个插件当成本地资源修改，适合单人本地开发。
3·RemotePlugin UnityPackageManager 导入PackageURL。
	与Local相对的，因为直接使用本地文件的原因。不适合发布/共享。
	当插件发布给他人时。必须用Remote或者.unitypackage的方式同步资源。
	目前 我将所有SeanLib插件放在GitHub开源仓库中。
/}
## GitHub 
(https://github.com/SeanTongTx/SeanLib.git)