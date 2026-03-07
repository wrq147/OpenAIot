<template>
	<view id="PoolDetails">
		<top :isRightSlot="true" leftIcon="icon-fanhui" :isleftBack="true" backgroundColor="#F5F8F9" title="详情" rightText="编辑" class="addPool-top">
			<template v-slot:top_right>
				<view class="right_top" @click="handEdit()">
					<view class="iconfont icon-bianji" style="font-size:32rpx;"></view>
				</view>
			</template>
		</top>
		<view class="PoolDetails-content">
			<view class="Avatar-information">
				<view class="iconfont t-icon-kehumorentouxiang1">
				
				</view>
				<view class="Avatar-information-title" style="border: none;">
					{{CustomerName}}
				</view>
				<view class="Avatar-information-content" v-if="planDetail.FollowTime">
					{{planDetail.FollowTime}}
				</view>
				<view class="Agent" v-if="planDetail.TargetType==0">
					客户<!--Agent代理0 直销为1-->
				</view>
				<view class="Agent xiansuo" v-if="planDetail.TargetType==1">
					线索
				</view>
			</view>
			<view class="Avatar-information-text">
				<view class="Avatar-information-text-item" style="border:none;border-bottom:2rpx solid #f8f8f8;border-top:2rpx solid #f8f8f8;">
					<view class="Avatar-information-text-item-title">计划执行人</view>
					<view class="Avatar-information-text-item-header" v-for="(item,index) in PlanExture" :key="index" style="margin-top:20rpx;">
						<img :src="item.Avatar" alt="" class="Avatar-information-text-item-img"/>
						<view class="content">{{item.RealName}}</view>
					</view>
				</view>
				<view class="Avatar-information-text-item" style="display: flex;align-items: center;">
					<view>
						<view class="Avatar-information-text-item-title">计划时间</view>
						<view class="Avatar-information-text-item-header">
							<view class="content">{{planDetail.PlanTime}}</view>
							
						</view>
					</view>
					<view class="information-status statusColr" v-if="planDetail.Status=='A'">
						待完成
					</view>
					<view class="information-status" v-if="planDetail.Status=='F'">
						已完成
					</view>
				</view>
				
				<view class="Avatar-information-text-item" style="display: flex;align-items: center;">
					<view>
						<view class="Avatar-information-text-item-title">计划内容</view>
						<view class="Avatar-information-text-item-header">
							<view class="content">{{planDetail.Remark}}</view>
						</view>
					</view>
				</view>
			</view>
		</view>
	</view>
</template>

<script>
	import {
		PlanDetails,//跟进计划详情
		ExecutionMode,//字典
		flowDataList
	} from "@/api/crmApi";
	import {
		yuanAarngemnt,//员工管理
		yuanAarngemntDetails
	} from "@/api/personalCenter";
	export default {
		data(){
			return{
				PlanExture:[],
				planDetail:[],
				lianData:[],
				shangjiArr:[],
				CustomerName:""
			}
		},
		onLoad(option) {
			if(option.id){
				this.list(option.id);
				this.CustomerName=option.CustomerName
			}
		},
		methods:{
			handEdit(){
				uni.navigateTo({
					url:'./addFollowPlan?id='+this.planDetail.Id
				})
			},
			list(id){
				PlanDetails({
					id:id
				}).then((res)=>{
					if(res.code==0){
						//console.log(res,'详情数据');
						this.planDetail=res.data;
						this.PlanExture =res.data.ExecutorUsers
						// ExecutionMode({OpportId
						// }).then((res)=>{
						// 	console.log(res,'字典')
						// })
					}
				})
			}
		}
	}
</script>
<style>
	page{
		background: #F5F8F9;
	}
</style>
<style lang="less" scoped>
	
	.Avatar-information-title{
		border-bottom: 1rpx solid #f8f8f8;
		padding-bottom: 30rpx;
	}
	.t-icon-kehumorentouxiang1{
		margin-top:-90rpx!important;
	}
	.PoolDetails-content{
		background: #ffffff;
		margin:0rpx 3%;
		border-radius: 12rpx;
		margin-top:90rpx;
	}
	.Avatar-information-textarea{
		text-align: left;
		padding:25rpx 20rpx;
		border-bottom: 1rpx solid rgba(255, 255, 255, .2);
		margin:0rpx 20rpx!important;
	}
	.Avatar-information-content{
		border-bottom: 1rpx solid rgba(255, 255, 255, .2);
		margin:12rpx 20rpx!important;
		margin-bottom: 0rpx!important;
		padding-bottom: 20rpx;
		
	}
	.Avatar-information {
		padding-bottom: 0rpx!important;
	}
	.Avatar-information-text{
		margin-top: 0rpx!important;
	}
	.Avatar-information-text-item-header{
		display: flex;
	}
	.information-status{
		margin-left:auto;
		background: rgba(238, 250, 242, 1);
		color:rgba(80, 201, 122, 1);
		border-radius:4rpx;
		font-size:24rpx;
		padding:0rpx 10rpx;
		height: 40rpx;
		line-height: 40rpx;
	}
	.statusColr{
		color:rgba(35, 113, 255, 1);
		background: rgba(233, 241, 255, 1);
	}
</style>