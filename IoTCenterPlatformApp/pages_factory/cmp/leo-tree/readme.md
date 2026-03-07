# leo-tree
#### props
#### data 数据源
#### defaultProps 配置选项，return { id: 'id', label: 'label', children: 'children' }
#### emit 
#### node-click 监听选中变化，返回选中的数据


### 使用
#### 本组件符合easycom规范，HBuilderX 2.5.5起，只需将本组件导入项目，在页面template中即可直接使用，无需在页面中import和注册components。
```javascript
	template
	<leo-tree :data="data" @node-click="nodeClick"></leo-tree>
	
	methods
	nodeClick(e) {
		console.log('点击的项目', e);
	}
	
```