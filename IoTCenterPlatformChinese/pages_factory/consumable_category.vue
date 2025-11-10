<template>
	<view style="min-height: 100vh;background-color: #ffffff;">
		<top :title="topTitle" leftWidth="120rpx" rightWidth="60rpx" :isleftBack="true"
			leftIcon="icon-fanhui" backgroundColor="#FFFFFF" rightIcon="icon-tianjia" @clickRight="categoryAdd">
		</top>
		<view class="leo_tree_con">
			<leoTree :data="classListData" @editItem="editItem" @node-click="nodeClick"
				:defaultProps="{ id: 'Id', label: 'Name', children: 'Children',photoUrl:'PhotoUrl'}"></leoTree>
		</view>
		<uni-load-more iconType="circle" :status="status" v-if="status" />
		<msg-prompt ref="promptMsg"></msg-prompt>
	</view>
</template>

<script>
	import leoTree from '@/pages_factory/cmp/leo-tree/components/leo-tree/leo-tree.vue'
	import {
		classTree,
		classInfo,
		removeClass,
		addClass,
		classSort,
		editClass
	} from "@/api/partscls";
	export default {
		components:{leoTree},
		data() {
			return {
				topTitle: '耗材分类管理',
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
				uni.navigateTo({
					url:'/pages_factory/consumable_category_add?id='+item.Id
				})
			},
			loadData() {
				this.status = 'loading';
				classTree().then(response => {

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