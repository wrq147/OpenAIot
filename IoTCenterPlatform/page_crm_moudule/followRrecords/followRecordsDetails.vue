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
					{{arr.TargetName}}
				</view>
				<view class="Avatar-information-content">
					{{arr.FollowTime}}
				</view>
				<view class="Avatar-information-textarea" v-html="arr.Remark">
					
				</view>
				<view class="Agent" v-if="arr.TargetType==0">
					Customer<!--Agent代理0 直销为1-->
				</view>
				<view class="Agent" v-if="arr.TargetType==1">
					Clue
				</view>
			</view>
			<view class="Avatar-information-text">
				<view class="Avatar-information-text-item itemTextBorde">
					<view class="Avatar-information-text-item-title">Plan executor</view>
					<view class="Avatar-information-text-item-header" v-if="arr.FollowUserInfo">
						<img :src="arr.FollowUserInfo.Avatar" alt="" class="Avatar-information-text-item-img"/>
						<view class="content">{{arr.FollowUserInfo.RealName}}</view>
					</view>
				</view>
				<view class="Avatar-information-text-item" v-if="arr.ContactInfo">
					<view class="Avatar-information-text-item-title">Contacts</view>
					<view class="Avatar-information-text-item-header">
						<!-- <view class="Avatar-information-text-item-img"></view> -->
						<view class="content">{{arr.ContactInfo.RealName}}</view>
					</view>
				</view>
				<view class="Avatar-information-text-item" >
					<view class="Avatar-information-text-item-title">Execution mode</view>
					<view class="Avatar-information-text-item-header">
						<view class="content">{{ExecutionName}}</view>
					</view>
				</view>
				<view class="Avatar-information-text-item" v-if="arr.OpportName">
					<view class="Avatar-information-text-item-title">Opportunity</view>
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
				ExecutionName:''
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
						// /console.log(res,'详情数据');
						this.arr=res.data;
						this.ExecutionName=this.ExecutionMode.get(res.data.FollowWay)
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
	.PoolDetails-content{
		background: #1C2232;
	}
	.Avatar-information-textarea{
		text-align: left;
		padding:25rpx 20rpx;
		font-size:32rpx;
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
	.Agent{
		background: rgba(63, 67, 86, 1)!important;
		border-radius:4rpx!important;
		padding:0rpx 5rpx;
	}
</style>