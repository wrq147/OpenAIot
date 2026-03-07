## DataSelect 下拉框选择器  先导入示例项目看看是否满足需求，然后再下载插件，有问题可以加微weiyila520
> **组件名：zxz-uni-data-select**
> 代码块： `zxz-uni-data-select`

当选项过多时，使用下拉菜单展示并选择内容
## API

### zxz-uni-data-select Props

|  属性名		|    类型				| 默认值	| 说明								|
| -				| -						| -			| -									|
| v-model		| String、Array、Number	|-			| 选中项绑定值						|
| multiple		| Boolean				| false		| 是否多选							|
| disabled		| Boolean				|false		| 是否禁用							|
| dataKey		| String				|"key"		| 作为 key 唯一标识的键名			|
| dataValue		| String				| "value"	| 作为 value 唯一标识的键名			|
| collapseTags	| Boolean				| false		| 多选时是否将选中值按文字的形式展示|
|collapseTagsNum|Number					| 1			| 多选时选中值按文字的形式展示的数量|
| localdata		| Array					|-			| 下拉列表本地数据					|
|label	| String	| -	| 左侧标题
|placeholder	| String	| "请选择"	| 输入框的提示文字
|emptyTips	| String	|"无选项"	| 无选项提示
|clear	| Boolean	| true| 是否清空
|format	| String	| -	| 格式化输出 用法 field="_id as value, version as text, uni_platform as label" format="{label} - {text}"
		
#### 如使用过程中有任何问题，或者您对组件有一些好的建议
## 欢迎加微weiyila520
