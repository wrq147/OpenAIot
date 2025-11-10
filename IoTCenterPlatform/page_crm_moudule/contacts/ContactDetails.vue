<template>
	<view class="ContactDetails">
		<top leftIcon="icon-fanhui" :isleftBack="true" leftText="Back" backgroundColor="#161A26"  title="Details" rightText="Edit"
			class="CRM-header">
			<template v-slot:top_right>
				<view class="right_top" @click="handEdit()">
					<!-- <custom-icons iconsName="icon-tianjia" iconsSize="36rpx"></custom-icons> -->
					<view style="font-size:31rpx;">Edit</view>
				</view>
			</template>
			</top>
			<view class="ContactDetails-section">
				<view class="ContactDetails-section-autve iconfont t-icon-morentouxiang">
				
				</view>
				<view class="ContactDetails-section-title">
					{{arr.RealName}}
				</view>
				<view class="ContactDetails-section-txt">
					{{arr.PostName}}
				</view>
				<view class="ContactDetails-section-border"></view>
				<view class="ContactDetails-section-item">
					<view class="ContactDetails-section-item-left">
						<view class="ContactDetails-section-txt">Telephone number</view>
						<view class="phone">{{arr.Mobile}}</view>
					</view>
					<view class="iconfont t-icon-dianhua" @click="dialogConfirm()"></view>
				</view>
				
				<view class="ContactDetails-section-item" v-if="arr.Email">
					<view class="ContactDetails-section-item-left">
						<view class="ContactDetails-section-txt">E-mail</view>
						<view class="phone">{{arr.Email}}</view>
					</view>
					
				</view>
				<view class="ContactDetails-section-item" v-if="arr.WxNumber">
					<view class="ContactDetails-section-item-left">
						<view class="ContactDetails-section-txt">WeChat</view>
						<view class="phone">{{arr.WxNumber}}</view>
					</view>
					
				</view>
				<view class="ContactDetails-section-item" v-if="arr.DeptName">
					<view class="ContactDetails-section-item-left">
						<view class="ContactDetails-section-txt">Department</view>
						<view class="phone">{{arr.DeptName}}</view>
					</view>
					
				</view>
				
				<view class="ContactDetails-section-item" v-if="arr.PostName">
					<view class="ContactDetails-section-item-left">
						<view class="ContactDetails-section-txt">Position</view>
						<view class="phone">{{arr.PostName}}</view>
					</view>
					
				</view>
				
				
				<view class="ContactDetails-section-item" v-if="arr.HelperName">
					<view class="ContactDetails-section-item-left" >
						<view class="ContactDetails-section-txt">Collaborator</view>
						<view class="phone">{{arr.HelperName}}</view>
					</view>
					
				</view>
				<view class="ContactDetails-section-item">
					<view class="ContactDetails-section-item-left">
						<view class="ContactDetails-section-txt">Creation date</view>
						<view class="phone">{{arr.createTime}}</view>
					</view>
					
				</view>
			</view>
			<view class="Delete-button" @click="handDelete()">
				Delete
			</view>
			<view style="height:1rpx;widht:100%;"></view>
			<msg-prompt ref="promptMsg" @confirm="confirmUnbind"></msg-prompt>
	</view>
</template>

<script>
import {
		CustomerInfo,//联系人详情
		ContactRemove//联系人删除
	} from "@/api/crmApi";
	import {
		setPagesParam
	} from '@/common/utillib.js'
	export default {
		data(){
			return{
				arr:[]
			}
		},
		onLoad(option) {
			if(option.id){
				this.list(option.id);
				
			}
		},
		methods:{
			dialogConfirm() {
				uni.makePhoneCall({
					phoneNumber: this.arr.Mobile,// 这里就是自己要拨打的电话号码
					success: (res) => {
						console.log('调用成功!')
					},
					fail: (res) => {
						console.log('调用失败!')
					}
				})
			},
			confirmUnbind(){
				//删除
				ContactRemove({id:this.arr.Id}).then((res)=>{
					if(res.code==0){
						uni.showToast({
							title:'Delete successful！',
							icon:'none'
						})
						setTimeout(()=>{
							setPagesParam('list')
						},700)
					}
				}).catch((err) => {
					this.setMsgTop(err)
				})
			},
			handDelete(){
				//删除
				this.$refs.promptMsg.noticeOpen(
					"Are you sure to delete the customers "+this.arr.RealName+"?"
				)
			},
			handEdit(){
				uni.navigateTo({
					url:'./ContactAddition?id='+this.arr.Id
				})
			},
			list(id){
				CustomerInfo({
				id:id	
				}).then((res)=>{
					if(res.code==0){
						//console.log(res,'联系人详情')
						this.arr=res.data
					}
				})
			},
		}
	}
</script>

<style lang="less" scoped>
	.ContactDetails{
		.Delete-button{
			width: 94%;
			margin:50rpx 3%;
			height: 100rpx;
			background: #1C2232;
			color:rgba(255, 255, 255, .5);
			text-align: center;
			line-height: 100rpx;
			border-radius: 8rpx;
			font-size:36rpx;
		}
		.ContactDetails-section{
			background: #1C2232;
			display: flex;
			flex-direction: column;
			margin:0rpx 3%;
			margin-top:100rpx;
			color:#fff;
			text-align: center;
			.ContactDetails-section-item{
				display: flex;
				margin:0rpx 4%;
				margin-top:17rpx;
				align-items: center;
				// padding-bottom: 25rpx;
				padding:20rpx 0rpx;
				border-top: 1rpx solid rgba(255,255,255,.1);
				.customIcons-dianhua{
					margin-left:auto;
					margin-top:10rpx;
				}
				.ContactDetails-section-item-left{
					text-align: left;
					.phone{
						font-size:32rpx;
						margin-top:10rpx;
					}
				}
				.t-icon-dianhua{
					width: 50rpx;
					height:50rpx;
					margin-left:auto;
					margin-top:10rpx;
				}
			}
			.borderNone{
				border:none;
			}
			.ContactDetails-section-title{
				font-size:36rpx;
				color:#FFFFFF;
				margin-top:20rpx;
			}
			
			.ContactDetails-section-txt{
				font-size:28rpx;
				margin-top:10rpx;
				color:rgba(255, 255, 255, .6);
			}
			.ContactDetails-section-autve{
				width: 160rpx;
				height:160rpx;
				border-radius: 50%;
				margin:0 auto;
				margin-top:-80rpx;
			}
		}
	}
</style>