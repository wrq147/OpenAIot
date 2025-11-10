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
				<view class="Avatar-information-title">
					{{arr.TargetName}}
				</view>
				<view class="Avatar-information-content">
					{{arr.FollowTime}}
				</view>
				<view class="Avatar-information-textarea" v-html="arr.Remark">
					
				</view>
				<view class="Agent" v-if="arr.TargetType==0">
					客户<!--Agent代理0 直销为1-->
				</view>
				<view class="Agent xiansuo" v-if="arr.TargetType==1">
					线索
				</view>
			</view>
			<view class="Avatar-information-text">
				<view class="Avatar-information-text-item">
					<view class="Avatar-information-text-item-title">跟进人</view>
					<view class="Avatar-information-text-item-header" v-if="arr.FollowUserInfo" style="display: flex;">
						<img  :src="arr.FollowUserInfo.Avatar" alt="" class="Avatar-information-text-item-img"/>
						<view class="content">{{arr.FollowUserInfo.RealName}}</view>
					</view>
				</view>
				<view class="Avatar-information-text-item" v-if="arr.ContactInfo">
					<view class="Avatar-information-text-item-title">联系人</view>
					<view class="Avatar-information-text-item-header">
						<view class="content">{{arr.ContactInfo.RealName}}</view>
					</view>
				</view>
				
				
				
				<view class="Avatar-information-text-item" >
					<view class="Avatar-information-text-item-title">跟进方式</view>
					<view class="Avatar-information-text-item-header">
						<view class="content">{{ExecutionName}}</view>
					</view>
				</view>
				<view class="Avatar-information-text-item itemTextBorde" v-if="arr.OpportName">
					<view class="Avatar-information-text-item-title">商机</view>
					<view class="Avatar-information-text-item-header">
						<view class="content">{{arr.OpportName}}</view>
					</view>
				</view>
			</view>
		</view>
	</view>
</template>

<script>
	import {
		flowDataRecordDetails,//跟进详情
		contactData,//联系人
		ExecutionMode,//字典
		BusineData//商机列表
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
				ExecutionMode:new Map(),
				ExecutionName:'',
			}
		},
	 async	onLoad(option) {
			if(option.id){
				this.list(option.id)
				let partUnit = await this.$store.dispatch("data/dictList", 'follow_way');
				partUnit.forEach((item) => {
					 this.ExecutionMode.set(item.value, item.label);
				})
			}
		},
		methods:{
			handEdit(){
				uni.navigateTo({
					url:'./AddFollowRecord?id='+this.arr.Id
				})
			},
			list(id){
				flowDataRecordDetails({
					id:id
				}).then((res)=>{
					if(res.code==0){
						//console.log(res,'详情数据');
						this.arr=res.data;
						this.ExecutionName=this.ExecutionMode.get(res.data.FollowWay)
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
	
	.PoolDetails-content{
		background: #ffffff;
		margin:0rpx 3%;
		border-radius: 12rpx;
		margin-top:90rpx;
	}
	.t-icon-kehumorentouxiang1{
		margin-top:-90rpx!important;
	}
	.Avatar-information-textarea{
		text-align: left;
		padding:25rpx 20rpx;
		border-bottom: 1rpx solid #f8f8f8;
		margin:0rpx 20rpx!important;
		font-size: 32rpx;
		border-top: 1rpx solid #f8f8f8;
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
	.Agent{
		background: none!important;
		border-radius:6rpx!important;
		color:#2371FF!important;
		border:1rpx solid #2371FF!important;
		padding:0rpx 1rpx;
		right:20rpx!important;
		top:20rpx!important;
		font-size:20rpx!important;
		width: 40rpx;
		height: 30rpx;
		line-height:30rpx;
	}
	.xiansuo{
		border:1rpx solid #47E2F1!important;
		color:#47E2F1!important;
	}
</style>