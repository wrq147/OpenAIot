<template>
	<view class="ManagementDetails">
		<top leftIcon="icon-fanhui"   :isleftBack="true" leftText="Back" backgroundColor="#161A26" title="Details" class="CRM-header"></top>
		<view class="ManagementDetails-details">
		 	<view class="ManagementDetails-details-header">
				<image :src="arr.Avatar" class="StaffManagement-item-img"></image>
			</view> 
			
			<view class="ManagementDetails-details-name">
				{{arr.RealName}}
			</view>
			<view class="ManagementDetails-details-content">
				{{arr.dept_name}}
			</view>
			<view class="ManagementDetails-details-item" style="border-top:none;">
				<view class="title">NO.</view>
				<view class="number">{{arr.Id}}</view>
			</view>
			<view class="telphpone" v-if="arr.Mobile">
				<view>
					<view class="title">Telephone number.</view>
					<view class="number">{{arr.Mobile}}</view>
				</view>
				<view @click="TelPhone()" class="t-icon-dianhua">
				
				</view>
			</view>
			<view class="ManagementDetails-details-item" >
				<view class="title">Creation date</view>
				<view class="number">{{arr.createTime}}</view>
			</view>
			<view class="ManagementDetails-details-item">
				<view class="title">role</view>
				<view class="number">{{rolesList}}</view>
			</view>
			<view class="AssignRoles" @click="handAssRole" v-if="isCheckPermi(['/AuthService/Member/Edit'])">
				+ Assign Roles
			</view>
		</view>
		<!-- <view class="Enable">
			<view class="left" :class="[select==0?'active':'on']" @click="select=0">
				Enable
			</view>
			<view class="left" :class="[select==1?'active':'on']" @click="select=1">
				Disable
			</view>
		</view> -->
		<view class="delete" @click="handDelete">
			Delete
		</view>
		<msg-prompt ref="promptMsg"  @confirm="confirmUnbind"></msg-prompt>
	</view>
</template>

<script>
	import {
		checkPermi
	} from '@/common/permission.js';
	import {
	deleteArngemnt,
	yuanAarngemntDetails
	} from "@/api/personalCenter";
	
	export default {
		data(){
			return{
				select:0,
				arr:[]
			}
		},
		onLoad(Option){
			// if(Option.item){
			// 	this.arr=JSON.parse(Option.item)
			// 	//console.log(this.arr)
			// }
			if(Option.id){
				this.editId=Option.id
				this.list(Option.id)
			}
		},
		methods:{
			list(id){
				yuanAarngemntDetails({id:id}).then((res)=>{
					if(res.code==0){
						console.log(res,'员工详情')
						this.arr=res.data.user
						var str=[]
						res.data.roles.map((row)=>{
							str.push(row.roleName)
						})
						this.rolesList=str.join(',')
					}
				})
			},
			handAssRole(){
				uni.navigateTo({
					url:'./AssignRoles?editId='+this.editId
				})
			},
			TelPhone(){
				      const res = uni.getSystemInfoSync(); //获取当前的手机机型
				                if (res.platform == 'ios') {
				                    uni.makePhoneCall({
				                        phoneNumber: this.arr.Mobile
				                    })
				                } else {
				                    uni.makePhoneCall({
				                        phoneNumber: this.arr.Mobile,
				                    })
				                }
			},
			confirmUnbind(){
				deleteArngemnt({id:this.arr.Id}).then((res)=>{
					uni.showToast({
						title:'Removal successful！',
						icon:'none'
					})
					uni.switchTab({
						url:'../../pages/profile/profile'
					})
				}).catch((err)=>{
					this.setMsgTop(err)
				})
			},
			handDelete(){
				this.$refs.promptMsg.noticeOpen(
					'Are you sure to remove user ID'+this.arr.Id+'Employees of？'
					)
			}
		}
	}
</script>

<style lang="less" scoped>
	.ManagementDetails{
		.AssignRoles{
			color:rgba(153, 153, 153, 1);
			height: 100rpx;
			line-height: 100rpx;
			font-size: 32rpx;
			border-top:1rpx solid rgba(255,255,255,.2);
		}
	.delete{
		height:100rpx;
		line-height: 100rpx;
		background:rgba(28, 34, 50, 1);
		color:rgba(255, 255, 255, .3);
		text-align: center;
		border-radius: 8rpx;
		margin:20rpx 3%;
		font-size:36rpx;
		margin-top:50rpx;
	}
	.Enable{
		display:flex;
		align-items: center;
		.left,.right{
			
			// display:flex;
			width:46%;
			height:88rpx;
			line-height:88rpx;
			border-radius:9rpx;
			text-align:center;
			margin:20px auto;
		}
		.right{
			
		}
		.active{
			border:1px solid #FF3535;
			color:#FF3535;
		}
		.on{
			border:1rpx solid rgba(255, 255, 255, .3);
			color:rgba(255,255,255,.6);
		}
	}
	.ManagementDetails-details{
		position: relative;
		margin:0rpx 20rpx;
		margin-top:140rpx;
		background: #1C2232;
		flex-direction: column;
		color:#fff;
		text-align:center;
		padding:0rpx 20rpx;
		border-radius:8rpx;
		padding-top:100rpx;
		.ManagementDetails-details-item{
			display:flex;
			flex-direction: column;
			text-align:left;
			padding:30rpx 0rpx;
			border-top:1rpx solid rgba(255,255,255,.2);
			.title{
				color:rgba(255,255,255,.6);
				font-size:28rpx;
			}
			.number{
				color:#fff;
				font-size:32rpx;
			}
			
		}
		.telphpone{
			display:flex;
			padding:30rpx 0rpx;
			text-align:left;
			align-items: center;
			border-top:1rpx solid rgba(255,255,255,.2);
			.t-icon-dianhua{
				margin-left:auto;
				width:50rpx;
				height:50rpx;
			}
			.title{
				color:rgba(255,255,255,.6);
				font-size:28rpx;
			}
		}
		.ManagementDetails-details-name{
			font-size:35rpx;
			margin-top:20rpx;
			margin-bottom: 10rpx;
		}  
		.ManagementDetails-details-content{
			font-size:28rpx;
			color:rgba(255,255,255,.6);
			border-bottom:1rpx solid rgba(255,255,255,.3);
			padding-bottom: 25rpx;
		}
		.StaffManagement-item-img{
		width:160rpx;
		height:160rpx;
			border-radius: 50%;
			margin:0 auto;
			// margin-top:-80rpx;
		}
		.ManagementDetails-details-header{
		position: absolute;
		display:flex;
		width:160rpx;
		height:160rpx;
		left:260rpx;
		border-radius:50%;
		margin:0 auto;
		background: url('../../static/headerLogo.png') no-repeat;
		background-size: 100% 100%;
		padding:20rpx;
		top:-90rpx;
			
		}
	}
	}
</style>

