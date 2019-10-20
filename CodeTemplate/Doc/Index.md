# CodeTemplate <color="red">(Internal)</color>
## 以模板的方式生成代码文件
### 在模板中定义特殊关键字，最后替换完成。 
!(img/1.jpg)
## CodeTemplate 
模板基类。每一个模板对应一类文件。继承并实现模板和模板关键字。
### 1.	TemplateName 
模板名称，没有实际作用只是用来标记模板对象
### 2.	FilePath 
文件路径，包含文件扩展名。
### 3.	Template 
模板本体
### 4.	KeyWords 
模板中关键字列表

## CodeGenerator 
代码生成器。
1.	继承并在Base.OnEnable()之前，写入模板子类。生成器会自行枚举所有关键字。
2.	填写所有关键字。
3.	点击Generate生成文件。

### 自行扩展 