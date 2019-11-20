# EditorMarkdown/Version

## Release 191119

增加了UIElements支持 没做完 改动很多 不太想改

## Release 191022

1·修改了文档目录格式。
现在文档路径基于UnityAssetPath不在基于文件目录。
·文档查找改为相对目录
·增加Title字段
·重写了文档加载和文档目录逻辑。现在文档跳转将依照当前文档路径相对进行。
2·增加 /{/}折叠页
在编辑器中单独显示为foldout 折叠项
3·修复了几个出现几率极低的bug

## Release 190905

标准化了插件版本
修改了Table表现
>DocVersionDesign

## Release 190610

1·增加了MD代码块语法支持
用来TextArea 显示文字。
2·增加了文档搜索功能
当前功能还在试验中,只在UnityEditor中有效。通过脚本开启。
目前只是简单的筛选，当前版本只对代码和QA块生效。
3·数据结构和渲染结构更新
为了支持区块语法
4·增加了QA问答语法块
常见的文档中Q&A的形式。可以简单通过MD配置。支持问题查找。
只在UnityEditor中有效
支持富文本，不支持图文混排。
语法更新参考EditorMDAPI

>EditorMarkdownAPI
