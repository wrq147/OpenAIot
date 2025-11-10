<template>
	<view id="PoolDetails">
		<top leftIcon="icon-fanhui" :isleftBack="true" leftText="Back" backgroundColor="#161A26" title="Details" rightText="Edit" class="addPool-top">
			<template v-slot:top_right>
				<view class="right_top" @click="handEdit()">
					<view style="font-size:32rpx;">Edit</view>
				</view>
			</template>
		</top>
		<view class="PoolDetails-content">
			<view class="Avatar-information">
				<view class="iconfont t-icon-kehumorentouxiang">
				
				</view>
				<view class="Avatar-information-title">
					<view>
						{{CustomerName}}
					</view>
					<view class="Avatar-time">{{arr.PlanTime}}</view>
				</view>
				<view class="Avatar-information-content">
					{{arr.Remark}}
				</view>
				<view  v-if="arr.CustomerType==0" class="detailsAgent">
					Agent
				</view>
				<view  v-if="arr.CustomerType==1" class="detailsDirectSales">
					Direct Sales
				</view>
			</view>
			<view class="Avatar-information-text">
				<view class="Avatar-information-text-item" >
					<view class="Avatar-information-text-item-title">Plan executor</view>
					<view class="Avatar-information-text-item-header" v-for="(item,index) in PlanExture" :key="index" style="margin-top:20rpx;">
						<img  :src="item.Avatar" alt="" class="Avatar-information-text-item-img"/>
						<view class="content">{{item.RealName}}</view>
					</view>
				</view>
				<view class="Avatar-information-text-item">
					<view class="Avatar-information-text-item-title">Scheduled time</view>
					<view class="Avatar-information-text-item-header">
						<view class="content">{{arr.PlanTime}}</view>
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
		yuanAarngemnt//员工管理
	} from "@/api/personalCenter";
	export default {
		data(){
			return{
				PlanExture:[],
				arr:[],
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
					url:'./addFollowPlan?id='+this.arr.Id
				})
			},
			list(id){
				PlanDetails({
					id:id
				}).then((res)=>{
					if(res.code==0){
						//console.log(res,'详情数据');
						this.arr=res.data;
						this.PlanExture =res.data.ExecutorUsers
					}
				})
			}
		}
	}
</script>

<style lang="less" scoped>
	page{
		background: #161A26;
	}
	.Avatar-time{
		color:rgba(153, 153, 153, 1);
		font-size:24rpx;
		text-align: center;
		font-weight: normal;
		margin-top:5rpx;
	}
	.PoolDetails-content{
		background: #1C2232;
	}
	.Avatar-information-textarea{
		text-align: left;
		padding:25rpx 20rpx;
		border-bottom: 1rpx solid rgba(255, 255, 255, .2);
		margin:0rpx 20rpx!important;
	}
	.Avatar-information-content{
		margin:20rpx 30rpx!important;
		margin-bottom: 0rpx!important;
		padding: 40rpx 0rpx;
		color:#fff!important;
		font-size:32rpx!important;
		text-align: left;
		border-top: 1rpx solid rgba(255, 255, 255, 0.2);
		
	}
	.Avatar-information {
		padding-bottom: 0rpx!important;
	}
	.Avatar-information-text{
		margin-top: 0rpx!important;
	}
	.detailsAgent{
	position: absolute;
	right:0rpx;
	top:0rpx;
	//width:100rpx;
	padding:0rpx 10rpx;
	text-align: center;
	font-size:20rpx;
	border-top-right-radius: 14rpx;
	border-bottom-left-radius: 14rpx;
	background: linear-gradient(180deg,#FF3535,#FF613D);
	}
	.detailsDirectSales{
		position: absolute;
		right:0rpx;
		top:0rpx;
		padding:0rpx 10rpx;
		text-align: center;
		font-size:23rpx;
		border-top-right-radius: 15rpx;
		border-bottom-left-radius: 15rpx;
		background: linear-gradient(180deg,#EFA902,#F4BE3F);
	}
</style>