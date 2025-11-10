<template>
	<view class="allPop" @click="close">
		<div class="management">
			<view :class="['contianer-name', {active: isIndex === ''}] " style="padding: 20rpx 30rpx;" @click="navClick('', '全部')">
			   全部
			   <custom-icons v-if="isIndex === ''" iconsName="icon-wancheng" iconsSize="30rpx" iconsColor="#2371FF"></custom-icons>
			</view>
			<view class="contianer" v-for="(item, index) in rangeList" :key="item.id">
			    <view :class="['contianer-name', {active: isIndex === item.Id}]" @click="navClick(item.Id, item.Name)">
				   {{ item.Name }}
				  <custom-icons v-if="isIndex === item.Id" iconsName="icon-wancheng" iconsSize="30rpx" iconsColor="#2371FF"></custom-icons>
				</view>
			    <view v-if="item.Children !=null && item.Children.length > 0">
			        <tree-item :data="item.Children" :isIndex="isIndex" :showIcon="true" @navIsIndex="navIsIndex"></tree-item>
			    </view>
			</view>
			<ul class="second">
				<li @click.stop="handlePage(1)">
					分类管理
				    <custom-icons iconsName="icon-guanli" iconsSize="30rpx" iconsColor="#333"></custom-icons>
				</li>
				<li @click.stop="handlePage(2)">
					车间管理
					 <custom-icons iconsName="icon-guanli" iconsSize="30rpx" iconsColor="#333"></custom-icons>
				</li>
			</ul>
		</div>
	</view>
</template>

<script>
	import treeItem from "./treeItem.vue";
	import {
		typeListTree,
	} from '@/api/device.js'
	export default {
		components: {
			treeItem
		},
		data() {
			return {
				rangeList: [],
				isIndex: ''
			}
		},
		methods: {
			getList() {
				typeListTree({ orgid: this.$store.state.user.orgId }).then(res => {
					this.rangeList = res.data;
				});
			},
			handlePage(type) {
				if(type === 1){
					uni.navigateTo({
						url: './common/classifyManage'
					})
					this.$emit('windowClose');
				} else {
					uni.navigateTo({
						url: './common/roomManage'
					})
					this.$emit('windowClose');
				}
			},
			navClick(id, text) {
				this.isIndex = id;
				this.$emit('roomChange', this.isIndex, text);
			},
			navIsIndex(id, text) {
				this.isIndex = id;
				this.$emit('roomChange', this.isIndex, text);
			},
			close() {
				this.$emit('windowClose');
			}
		}
	}
</script>

<style lang="less" scoped>
	.allPop{
		width: 100%;
		height: 100%;
		position: fixed;
		top:0;
		left: 0;
		background-color: rgba(0, 0, 0, 0.7);
		z-index: 10;
	}
	.management{
		width: 400rpx;
		background: #FFFFFF;
		box-shadow: 0rpx 4rpx 16rpx 0rpx rgba(0,0,0,0.06);
		border-radius: 10rpx;
		margin-left: 30rpx;
		margin-top: 370rpx;
	}
	.contianer{
		padding: 0 30rpx;
	}
	.contianer-name{
		font-size: 30rpx;
		color: #333333;
		font-weight: bold;
		display: flex;
		align-items: center;
		justify-content: space-between;
		padding: 10rpx 0;
	}
	.contianer-name.active{
		color: #2371FF;
	}
	.management ul{
		padding: 0;
		margin: 0;
		padding: 20rpx 30rpx;
		position: relative;
	}
	.management ul.second::before{
		content: "";
		position: absolute;
		top: 0;
		left: 50%;
		width: 340rpx;
		height: 1px;
		margin-left: -170rpx;
		background: #EAEAEA;
		border-radius: 10rpx;
	}
	.management ul>li{
		width: 100%;
		list-style: none; 
		padding: 0;
		margin: 0;
		height: 90rpx;
		display: flex;
		align-items: center;
		justify-content: space-between;
		font-weight: bold;
		font-size: 30rpx;
		color: #333333;
	}
	.second>li>image{
		width: 30rpx;
		height: 30rpx;
	}
</style>