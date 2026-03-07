<template>
	<view class="ManagementDetails">
		<top leftIcon="icon-fanhui"   :isleftBack="true"  backgroundColor="#F5F8F9" title="员工详情" class="CRM-header"></top>
		<view class="ManagementDetails-details">
			
		 	 <view class="ManagementDetails-details-header" :style="{'background-image':`url(${getSerVerUrl()}/appimg/headerLogoNew.png)`}">
				<image :src="arr.Avatar" class="StaffManagement-item-img"></image>
			</view> 
			
			<view class="ManagementDetails-details-name">
				{{arr.RealName}}
			</view>
			<view class="ManagementDetails-details-content">
				{{arr.dept_name}}
			</view>
			<view class="ManagementDetails-details-item">
				<view class="title">员工编号</view>
				<view class="number">{{arr.Id}}</view>
			</view>
			<view class="telphpone" v-if="arr.Mobile">
				<view>
					<view class="title">手机号</view>
					<view class="number">{{arr.Mobile}}</view>
				</view>
				<view @click="TelPhone()" class="iconfont t-icon-dianhua1">
					
				</view>
			</view>
			<view class="ManagementDetails-details-item" v-if="arr.Email">
				<view>
					<view class="title">邮箱号码</view>
					<view class="number">{{arr.Email}}</view>
				</view>
			</view>
			<view class="ManagementDetails-details-item" v-if="arr.Email">
				<view>
					<view class="title">邮箱号码</view>
					<view class="number">{{arr.Email}}</view>
				</view>
			</view>
			<view class="ManagementDetails-details-item" v-if="arr.Signature">
				<view>
					<view class="title">工作签名</view>
					<view class="number">{{arr.Signature}}</view>
				</view>
			</view>
			<view class="ManagementDetails-details-item"  style="border-bottom:none;">
				<view class="title">创建时间</view>
				<view class="number">{{arr.createTime}}</view>
			</view>
			<view class="ManagementDetails-details-item">
				<view class="title">角色</view>
				<view class="number">{{rolesList}}</view>
			</view>
			<view class="AssignRoles" @click="handAssRole" v-if="isCheckPermi(['/AuthService/Member/Edit'])">
				+ 分配角色
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
			删除
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
	import {
		setPagesParam
	} from '@/common/utillib.js'
	export default {
		data(){
			return{
				select:0,
				arr:[],
				rolesList:''
			}
		},
		onLoad(Option){
			if(Option.item){
				//this.arr=JSON.parse(Option.item)
				//console.log(this.arr)
			}
			if(Option.id){
				this.editId=Option.id
				this.list()
			}
			
			
		},
		methods:{
			list(){
				yuanAarngemntDetails({id:this.editId}).then((res)=>{
					if(res.code==0){
						// console.log(res,'员工详情')
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
						title:'删除成功！',
						icon:'none'
					})
					// uni.switchTab({
					// 	url:'../../pages/profile/profile'
					// })
					setPagesParam('list','load',1)
				}).catch((err)=>{
					this.setMsgTop(err)
				})
			},
			handDelete(){
				this.$refs.promptMsg.noticeOpen(
					'您确定要删除用户为'+this.arr.Id+'的员工吗？'
					)
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
	.t-icon-dianhua1{
		width: 50rpx;
		height:50rpx;
		color:#E9F1FF;
		text-align: right;
		margin-left:auto;
		margin-right:20rpx;
		font-size:50rpx;
	}
	.ManagementDetails{
	.delete{
		height:100rpx;
		line-height: 100rpx;
		background:#fff;
		color:#999999;
		text-align: center;
		border-radius: 8rpx;
		margin:20rpx 3%;
		font-size:32rpx;
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
		background: #ffffff;
		flex-direction: column;
		color:#333;
		text-align:center;
		padding:0rpx 20rpx;
		border-radius:8rpx;
		padding-top:100rpx;
		.AssignRoles{
			color:rgba(153, 153, 153, 1);
			height: 100rpx;
			line-height: 100rpx;
			font-size: 28rpx;
		}
		.ManagementDetails-details-item{
			display:flex;
			flex-direction: column;
			text-align:left;
			padding:30rpx 0rpx;
			border-bottom:1rpx solid #f8f8f8;
			border-top:1rpx solid #f8f8f8;
			.title{
				color:#999999;
				margin-bottom:15rpx;
				font-size:28rpx;
			}
			.number{
				color:#333;
				font-size:32rpx;
			}
			
		}
		.telphpone{
			display:flex;
			padding:30rpx 0rpx;
			text-align:left;
			align-items: center;
			border-bottom:1rpx solid rgba(255,255,255,.2);
			.icon-dianhua11{
				margin-left:auto;
			}
			.title{
				color:#999999;
				margin-bottom:15rpx;
			}
		}
		.ManagementDetails-details-name{
			font-size:35rpx;
			margin-top:40rpx;
			margin-bottom: 10rpx;
		}  
		.ManagementDetails-details-content{
			font-size:28rpx;
			color:#999999;
			border-bottom:1rpx solid rgba(255,255,255,.3);
			padding-bottom: 25rpx;
		}
		.StaffManagement-item-img{
		width:160rpx;
		height:160rpx;
			border-radius: 50%;
			margin:0 auto;
			// margin-top:80rpx;
		}
		.ManagementDetails-details-header{
			position: absolute;
			display:flex;
			width:160rpx;
			height:160rpx;
			left:260rpx;
			border-radius:50%;
			margin:0 auto;
			// background: url('../../static/headerLogoNew.png') no-repeat;
			background-repeat: no-repeat;
			background-size: 100% 100%;
			padding:20rpx;
			top:-90rpx;
		
			
		}
	}
	}
</style>

