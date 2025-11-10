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