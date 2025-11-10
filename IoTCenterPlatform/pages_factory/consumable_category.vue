<template>
	<view>
		<top :title="topTitle" leftText="Back" leftWidth="120rpx" rightWidth="60rpx" :isleftBack="true"
			leftIcon="icon-fanhui" backgroundColor="#161A26" rightIcon="icon-tianjia" @clickRight="categoryAdd">
		</top>
		<view class="leo_tree_con">
			<leo-tree :data="classListData" @editItem="editItem" @node-click="nodeClick"
				:defaultProps="{ id: 'Id', label: 'Name', children: 'Children',photoUrl:'PhotoUrl' }"></leo-tree>
		</view>
		<uni-load-more iconType="circle" :status="status" v-if="status" />
		<msg-prompt ref="promptMsg"></msg-prompt>
	</view>
</template>

<script>
	import {
		classTree,
		classInfo,
		removeClass,
		addClass,
		classSort,
		editClass
	} from "@/api/partscls";
	import {
		log
	} from "mqtt/dist/mqtt";
	export default {
		data() {
			return {
				topTitle: 'Management category',
				classListData: [],
				status: 'loading'
			};
		},
		onLoad() {
			this.loadData()
		},
		methods: {
			categoryAdd() {
				uni.navigateTo({
					url:'/pages_factory/consumable_category_add'
				})
			},
			editItem(item) {
				console.log("item编辑", item);
				uni.navigateTo({
					url:'/pages_factory/consumable_category_add?id='+item.Id
				})
			},
			loadData() {
				this.status = 'loading';
				classTree().then(response => {
					console.log("查询到的产品分类", response);

					this.classListData = response.data;
					this.status = 'noMore';
				}).catch(err => {
					this.setMsgTop(err)
				});
			},
			nodeClick(e) {
				// console.log('点击的项目', e);
			}
		}
	}
</script>

<style lang="less">
	.leo_tree_con {
		width: 100%;
		padding: 0 20rpx;
		box-sizing: border-box;
	}
</style>