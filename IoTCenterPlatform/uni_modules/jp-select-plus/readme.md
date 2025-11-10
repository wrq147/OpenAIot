## 阿里矢量图-图标组件
> **组件名：al-icon**

### 安装方式

本组件符合[easycom](https://uniapp.dcloud.io/collocation/pages?id=easycom)规范，`HBuilderX 2.5.5`起，只需将本组件导入项目，在页面`template`中即可直接使用，无需在页面中`import`和注册`components`。

###本组件是jp-select 升级组件，该组件能满足大多数开发需求
####拥有 单选、多选、搜索、弹框选择、多选拼接展示...采用v-model双向绑定操作，用户使用更加方便

### 用法

```html
<template>
	<view class="content">
		<view class="hader">基础用法</view>
		<jp-select-plus label="单选不带搜索" placeholder="请选择"  v-model="va1" :list="listc"></jp-select-plus>
		<jp-select-plus label="单选带搜索设置主题色" color="#f00"placeholder="请选择" isSearch v-model="va1" :list="listc"></jp-select-plus>
		<jp-select-plus label="多选带搜索-排列展示结果" placeholder="请选择" checkbox isSearch v-model="va2" :list="listc"></jp-select-plus>
		<jp-select-plus label="多选带搜索-拼接展示结果" placeholder="请选择" isJoin checkbox isSearch v-model="va2" :list="listc"></jp-select-plus>
	    <jp-select-plus label="一行选择框" :isLineFeed="false" placeholder="请选择" isSearch v-model="va1" :list="listc"></jp-select-plus>
		<jp-select-plus label="改变选择主题颜色"  placeholder="请选择" isSearch v-model="va1" :list="listc"></jp-select-plus>
		<view class="hader">进阶用法</view>
		<jp-select-plus  @onOpen="getList()" @onSearch="getList()" label="点击后才去后端拿数据,展示加载中" isSearchFun isLoading placeholder="请选择" isSearch v-model="va1" :list="list2"></jp-select-plus>
	    <view class="hader">只使用弹框</view>
		<view class="but" @click="toOpen">
			点击开启弹框选择数据
		</view>
		选中的数据：{{va3}}
		<jp-select-plus  ref="selectPlus" :isShow="false"  v-model="va3" :list="listc"></jp-select-plus>
	</view>
</template>
<script>
	export default {
		data() {
			return {
				va1: '',
				va2: [],
				va3: '',			
				listc: [{
					code: 'CHN',
					name: '中国'
				}, {
					name: '美国5',
					code: 'USA'
				}, {
					name: '巴西',
					code: 'BRA'
				}, {
					name: '日本',
					code: 'JPN'
				}, {
					name: '英国',
					code: 'ENG'
				}, {
					name: '法国',
					code: 'FRA'
				}, {
					name: '小人国',
					code: 'xr1'
				}, {
					name: '大人国',
					code: 'xr2'
				}, {
					name: '中人国中人国中人国中人国中人国中人国中人国中人国中人国中人国中人国',
					code: 'xr3'
				}],
				list2:[]
			}
		},
		methods: {
			toOpen(){
				this.$refs.selectPlus.open()
			},
			getList(){
				let that = this
				setTimeout(()=>{
					that.list2 = that.listc
				},2000)
			}
		}
	}
</script>
<style scoped lang="scss">
	.content {
		padding: 0 20px;
		.but{
			margin: 10rpx 25rpx;
			background-color: #00aaff;
			color: #fff;
			text-align: center;
			line-height: 80rpx;
		}
		.hader {
			line-height: 80rpx;
			font-weight: 800;
		}
	}
</style>
```

## API

### 参数
|  属性名	|    类型	| 默认值	| 说明			|
|			|			|			|				|
| list		| Array| 	| 需要选择的数据（如果是对象需要配置 keys 和 showName 具体说明如下 ）	|
| keys| String	|  code			| 需要识别的code标识	|
| showName	| String	|  name		| 需要展示标识	|
| v-model		| |	| 双向绑定		|
| value		|  [String, Array]	| |  初始值，多选时必须为数组	|
| label		| String	| |  label名称	|
| labelColor		| String	| #646566 |  label颜色	|
| labelWidth|String| '90px'|  label宽度（当选项为一行时生效）	|
| placeholder|String| '请选择'|  placeholder	|
| placeholderColor|String| '#999'|  placeholder字体颜色	|
| isLineFeed|Boolean| true|  选项框是否为一行，默认独立开	|
| required|Boolean| false|  是否显示必填	|
| direction|String| left|  文字方向 默认左对齐	|
| color|String|'#00aaff' |  主题色	|
| alls|Boolean|false |  是否数据全部返回	|
| isoverflow|Boolean|false  |  超出隐藏	|
| isJoin| Boolean|false |  多选时是否拼接展示	|
| isLine|Boolean| true |  底部是否有线条	|
| disabled|Boolean| fasle |  是否可以选择	|

### 弹框参数
|  属性名	|    类型	| 默认值	| 说明			|
|			|			|			|				|
| title		| String| 请选择	|弹框名称	|
| cancelText| String	|  取消	| 取消文字|
| confirmText	| String	|  确定		| 确定文字	|
| isSearch	| Boolean| fasle | 是否开启头部搜索	|
| isSearchFun	| Boolean| fasle | 点击头部搜索是是否调用外部方法	|
| isLoading	| Boolean| fasle | 是否显示加载中	|

### 事件

|  事件名	|    类型	|  回调参数	|    说明		|
|			|			|			|				|
| input	| function	|当前选择的数据 key 数组| 	|
| toConfirm	| function	| 当前选择的数据 全数据	| 点击确认	|
| onOpen	| function	|无			| 点击选择框时回调	|
| cancel	| function	|无			| 点击取消	|
| input	| function	|无			| e 当前选择的数据 key 数组	|

### 方法

|  方法名| 	说明	|  
|			|			|	
| open()	| 打开弹框	|
| close()	| 关闭弹框	|

	
		





