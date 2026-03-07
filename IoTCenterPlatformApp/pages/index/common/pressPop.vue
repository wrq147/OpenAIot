<template>
	<view class="">
		<view class="window-top">
			<view class="window-topPd">
				<span class="number">已选中{{ this.selectData.length }}项</span>
				<view class="title">分配设备</view>
				<span class="close" @click="close">取消</span>
			</view>
		</view>
		<view class="window-bottom">
			<view class="window-bottomPd">
				<view class="infoBox start" @click="checkAll">
					<custom-icons v-if="!isCheckAll" class="icon" iconsName="icon-weixuanzhong" iconsSize="36rpx" iconsColor="#EAEAEA"></custom-icons>
					<view v-if="isCheckAll" class="icon icons_con t-icon-gouxuan1" style="width: 36rpx;height: 36rpx;"></view>
					<span>{{ isCheckAll ? '取消全选' : '全选' }}</span>
				</view>
				<view class="infoBox" v-if="current === 1" @click="devMove">
					<custom-icons class="icon" iconsName="icon-yidongshebei" iconsSize="28rpx" iconsColor="#999999"></custom-icons>
					<span class="gray">移动设备</span>
				</view>
				<view class="infoBox" v-if="current !== 1" @click="roomRemove">
					<custom-icons class="icon" iconsName="icon-tuihui" iconsSize="28rpx" iconsColor="#999999"></custom-icons>
					<span class="gray">移出车间</span>
				</view>
			</view>
		</view>
		<!-- 车间分类弹窗 -->
		<uni-popup ref="typePop" type="bottom" :mask-click="true" :zIndex="12"
			maskBackgroundColor="rgba(0, 0, 0, 0.5)">
			<view class="popup_list">
				<view class="popup_heard">
					选择分类
					<span @click="typePopClose">取消</span>
				</view>
				<view class="contianer" v-for="(item, index) in rangeList" :key="item.id">
				    <view class="contianer-name" @click="navClick(item.Id)">
					   {{ item.Name }}
					  <custom-icons iconsName="icon-a-youjiantouhong" iconsSize="20rpx"	iconsColor="#999999"></custom-icons>
					</view>
				    <view v-if="item.Children !=null && item.Children.length > 0">
				        <tree-item :data="item.Children" :isIndex="isIndex" :showIcon="false" @navIsIndex="navIsIndex"></tree-item>
				    </view>
				</view>
			</view>
		</uni-popup>
		<!-- 车间列表弹窗 -->
		<uni-popup ref="roomPop" type="bottom" :mask-click="true" :zIndex="13"
			maskBackgroundColor="rgba(0, 0, 0, 0.5)">
			<view class="popup_list">
				<view class="popup_heard">
					选择车间
					 <view class="return" @click="returnClick">
					 	<custom-icons iconsName="icon-fanhui" iconsSize="30rpx" iconsColor="#333"></custom-icons>
					 </view>
					<span @click="roomPopClose">取消</span>
				</view>
				<view :class="['popup_navList', { active: roomIndex === item.Id }]" v-for="item in navList" :key="item.Id" @click="navListClick(item.Id)">
					{{ item.Name }}
					<custom-icons v-if="roomIndex === item.Id" iconsName="icon-wancheng" iconsSize="30rpx" iconsColor="#2371FF"></custom-icons>
				</view>
				<button class="btn" @click="submit" type="submit">确定</button>
			</view>
		</uni-popup>
	</view>
	
</template>

<script>
	import treeItem from "./treeItem.vue";
	import {
		typeListTree,
		roomListRemove,
		roomList,
		roomListDevAdd
	} from '@/api/device.js'
	export default {
		components: {
			treeItem
		},
		props: {
		  deviceTableData: {
		    type: Array,
		    default: () => []
		  },
		  current: {
			type: Number,
			default: () => 0
		  },
		  roomId: {
			type: String,
		  }
		},
		data() {
			return {
				selectData: [],
				rangeList: [],
				navList: [],
				isCheckAll: false,
				isIndex: '',
				roomIndex: '',
			}
		},
		watch: {
		    deviceTableData: {
		        immediate: true,
		        deep: true,
		        handler() {
		            this.selectData = []
		            this.deviceTableData.forEach((item) => {
						if (item.isSelect) {
							this.selectData.push(item)
						}
					})
					if (this.deviceTableData.length === this.selectData.length && this.selectData!='') {
						this.isCheckAll = true;
					}
		        },
		    },
			current: {
				handler() {
					this.isCheckAll = false
				}
			}
		},
		
		methods: {
			// 获取分类列表
			getList() {
				typeListTree({ orgid: this.$store.state.user.orgId }).then(res => {
					console.log(res)
					this.rangeList = res.data;
				});
			},
			// 获取车间列表
			getRoomList(type) {
				roomList({ TargetOrgId: this.$store.state.user.orgId, CategoryId: type }).then(res => {
					this.navList = res.data
				})
			},
			navClick(id) {
				this.isIndex = id;
				this.getRoomList(id);
				this.typePopClose();
				this.$refs.roomPop.open();
			},
			navIsIndex(id) {
				this.isIndex = id;
				this.getRoomList(id);
				this.typePopClose();
				this.$refs.roomPop.open();
			},
			// 移动设备按钮操作
			devMove() {
				if (this.selectData.length > 0) {
					this.$refs.typePop.open();
				} else {
					uni.showToast({
					    title: '请选择要移动的设备',
					    icon:'error',
					    duration: 2000
					});
					return
				}
			},
			// 车间列表弹窗返回上一级
			returnClick() {
				this.roomPopClose();
				this.$refs.typePop.open();
			},
			// 点击车间列表获取id
			navListClick(id) {
				this.roomIndex = id;
			},
			// 关闭车间分类弹窗
			typePopClose() {
				this.$refs.typePop.close();
			},
			// 关闭车间列表弹窗
			roomPopClose() {
				this.$refs.roomPop.close();
			},
			// 关闭总弹窗
			close() {
				this.$emit('windowClose');
			},
			// 全选/ 取消全选
			checkAll() {
				this.isCheckAll = !this.isCheckAll;
				this.$emit('devChange', this.isCheckAll);
			},
			// 车间添加设备
			roomListAdd(data) {
				roomListDevAdd(data).then(res => {
				    uni.showToast({
				        title: '关联成功',
				        icon:'success',
				        duration: 2000
				    });
					this.roomPopClose();
				    this.$emit('getDeviceList')
				})
			},
			// 确定提交设备关联车间
			submit() {
				let data = [];
				let _that = this;
				if (this.roomIndex === '') {
					uni.showToast({
					    title: '请选择车间',
					    icon:'error',
					    duration: 2000
					});
				} else {
					uni.showModal({
					    title: '提示',
					    content: '你确定要移动这些设备吗？',
					    success: function (res) {
					        if (res.confirm) {
								_that.selectData.forEach((item) => {
									const obj = {
										roomId: _that.roomIndex,
										deviceId: item.Id
									}
									data.push(obj)
								})
								_that.roomListAdd(data)
					        } 
					    }
					})
				}
			},
			// 设备移除车间
			roomRemove() {
				if (this.selectData.length <= 0) {
					uni.showToast({
						title: '请选择要移出的设备',
						icon:'error',
						duration: 2000
					});
					return false;
				} 
				let data = [];
				let _that = this;
				uni.showModal({
				    title: '提示',
				    content: '你确定要移除设备吗？',
				    success: function (res) {
				        if (res.confirm) {
							_that.selectData.forEach((item) => {
								const obj = {
									roomId: _that.roomId,
									deviceId: item.Id
								}
								data.push(obj)
							})
				        	roomListRemove(data).then(res => {
				        		uni.showToast({
				        		    title: '移除成功',
				        		    icon:'success',
				        		    duration: 2000
				        		});
				        		_that.$emit('getDeviceList')
				        	})
				        } 
				    }
				})
			}
		}
	}
</script>

<style lang="less" scoped>
	.window-top{
		width: 100%;
		position: fixed;
		top: 88rpx;
		left: 0;
		background: #FFFFFF;
		z-index: 10;
	}
	.window-topPd{
		padding: 32rpx 0;
		display: flex;
		align-items: center;
		justify-content: center;
		position: relative;
	}
	.window-topPd .number{
		font-weight: 400;
		font-size: 24rpx;
		color: #999999;
		position: absolute;
		left: 20rpx;
		top: 32rpx;
	}
	.window-topPd .title{
		font-size: 34rpx;
		color: #333333;
		font-weight: bold;
	}
	.window-topPd .close{
		font-weight: 400;
		font-size: 28rpx;
		color: #999999;
		position: absolute;
		right: 20rpx;
		top: 32rpx;
	}
	.window-bottom{
		width: 100%;
		position: fixed;
		bottom: 0;
		left: 0;
		background: #FFFFFF;
		z-index: 10;
	}
	.window-bottomPd{
		padding: 34rpx 0;
		display: flex;
		align-items: center;
	}
	.window-bottomPd .infoBox{
		width: 50%;
		display: flex;
		align-items: center;
		justify-content: center;
		border-right: 1px solid #EAEAEA;
	}
	.window-bottomPd .infoBox>span{
		margin-left: 16rpx;
		font-weight: 400;
		font-size: 28rpx;
		font-weight: 500;
	}
	.window-bottomPd .infoBox>.gray{
		color: #999999;
		margin-left: 10rpx;
	}
	.window-bottomPd .infoBox>.icon{
		margin-left: 30rpx;
	}
	.window-bottomPd .start{
		justify-content: flex-start
	}
	.window-bottomPd .infoBox:last-child{
		border: 0;
	}
	.popup_list {
		background-color: #ffffff;
		border-radius: 20rpx 20rpx 0 0;
		padding-bottom: 40rpx;
	}
	.popup_heard{
		height: 114rpx;
		display: flex;
		align-items: center;
		justify-content: center;
		position: relative;
		font-weight: 500;
		font-size: 34rpx;
		color: #333333;
	}
	.popup_heard>span{
		font-weight: 400;
		font-size: 28rpx;
		color: #999999;
		position: absolute;
		right: 30rpx;
		top: 30rpx;
	}
	.popup_heard>.return{
		position: absolute;
		left: 30rpx;
		top: 42rpx;
	}
	.contianer{
		padding: 0 30rpx 0 30rpx;
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
	.popup_navList{
		padding: 35rpx 30rpx;
		background-color: #fff;
		font-weight: 500;
		font-size: 30rpx;
		color: #333333;
		display: flex;
		justify-content: space-between;
		align-items: center;
	}
	.popup_navList.active{
		background-color: #F8F8F8 ;
	}
	.btn{
		margin: 100rpx auto 0 auto;
		width: 690rpx;
		height: 88rpx;
		line-height: 88rpx;
		background: #2371FF;
		border-radius: 10rpx;
		font-weight: 400;
		font-size: 32rpx;
		color: #FFFFFF;
	}
</style>