<template>
	<view class="personData">
		<top :isleftBack="true" leftIcon="icon-fanhui" leftText="Back" backgroundColor="#161A26" title="Edit profile" class="CRM-header"></top>
		<view class="personData-data">
			<view class="personData-data-header_con" @click="uploadMul">
				<image :src="AvatarImg" mode="" class="personData-data-header"></image>
			</view>
			<!-- <input type="image" :src="userInfo.Avatar" alt="" class="personData-data-header"> -->
			
			<view class="personData-data-parent">
				<view class="nameTitle">
					User name
				</view>
				<view class="inputValue">
					<input v-model="userInfo.RealName" type="text" class="addPool-easyinput" placeholder="Please enter the user name" placeholder-style="color:rgba(255, 255, 255, .2);">
				</view>
				
			</view>
			
			<view class="personData-data-parent">
				<view class="nameTitle">
					Sex
				</view>
				<view class="gender" >
					<view :class="[userInfo.Sex==0?'active':'on']" class="gender-item"  @click="handClick(0)">
						Male
					</view>
					<view :class="[userInfo.Sex==1?'active':'on']" class="gender-item"  @click="handClick(1)">
						Female
					</view>
				</view>
			</view>
			
			<view class="personData-data-parent">
				<view class="nameTitle">
					Telephone number
				</view>
				<view class="content">
					<view >
						<text v-if="userInfo.Mobile" style="color:#fff;">
							{{userInfo.Mobile}}
						</text>
						<text v-else style="color:rgba(255, 255, 255, .2);">
							Please bind your email and phone number
						</text>
					</view>
					<view class="contentRight" @click="handNumber">
						<view>
							Binding
						</view>
						<view class="iconfont icon-a-youjiantoubai">
							
						</view>
					</view>
				</view>
			</view>
			
			
			<view class="personData-data-parent">
				<view class="nameTitle">
					E-mail
				</view>
				<view class="content">
					<view >
						<text v-if="userInfo.Email" style="color:#fff;">
							{{userInfo.Email}}
						</text>
						<text v-else style="color:rgba(255, 255, 255, .2);">
							Please bind your email
						</text>
					</view>
					<view class="contentRight" @click="handEmails()">
						<view>
							Binding
						</view>
						<view class="iconfont icon-a-youjiantoubai">
							
						</view>
					</view>
				</view>
			</view>
			
			
			<view class="personData-data-parent">
				<view class="nameTitle">
					Department
				</view>
				<view class="inputValue">
					<input v-model="userInfo.dept_name" type="text" disabled class="addPool-easyinput bgColor" placeholder="Please enter the user name" placeholder-style="color:rgba(255, 255, 255, .2);">
				</view>
				
			</view>
			
			
			<view class="personData-data-parent">
				<view class="nameTitle">
					Roles
				</view>
				<view class="inputValue">
					<input v-model="roleGroup" type="text" disabled class="addPool-easyinput bgColor" placeholder="Company administrator, Agent, Initial member" placeholder-style="color:rgba(255, 255, 255, .2);">
				</view>
			</view>
			
			
			<view class="personData-data-parent">
				<view class="nameTitle">
					Creation date
				</view>
				<view class="inputValue">
					<input  v-model="userInfo.createTime" type="text" disabled class="addPool-easyinput bgColor" placeholder="2023-09-02 20:00:00" placeholder-style="color:rgba(255, 255, 255, .2);">
				</view>
			</view>
			
			
			
			<view class="personData-data-parent">
				<view class="nameTitle">
					Others
				</view>
				<view class="content styleParent" @click="handChangePass">
					<view style="color:#fff;">
						Change password
					</view>
					<view class="contentRight">
						<view class="iconfont icon-a-youjiantoubai">
							
						</view>
					</view>
				</view>
			</view>
			<button :disabled="isSubmit" class="save-submit" @click="handSubmit">
				Save
			</button>
			<view style="height:20px;"></view>
		</view>
		<msg-prompt ref="promptMsg"></msg-prompt>

	</view>
</template>

<script>
	import {
		uploadPhoto,
		delPhoto,
	} from '@/api/user.js'
	import {
	dataList,
	updateUserProfile
	} from "@/api/personalCenter";
	export default {
		data(){
			return{
				AvatarImg:'',
				isSubmit:false,
				roleGroup:'',
				userInfo:{},
				Sex:'',
				selected:0,
				arr:[
					{
					title:'Male'
					},
					{
					title:'Female'
					},
				]
			}
		},
		onLoad() {
			this.dataInfo();
		},
		methods:{
			//选择地址
			uploadMul(type) {
				//手动上传图片
				uni.chooseImage({
					count: 1,
					sizeType: ['compressed'], //可以指定是原图还是压缩图，默认二者都有
					success: async (res) => {
						console.log("上传图片", res);
						this.$refs.promptMsg.loadingOpen('上传中...')
						if (res.tempFilePaths.length == 0) {
							return;
						}
						for (let i = 0; i < res.tempFiles.length; i++) {
							if (res.tempFiles[i].size > 10 * 1024 * 1024) {
								this.$refs.promptMsg.open('上传图片不能超过10M', 2000)
								return;
							}
							try {
								let paths = res.tempFilePaths[i];
								console.log(paths, 'pathspaths');
								let rsp = await uploadPhoto(paths)
								// if (this.userInfo.Avatar) {
								// 	let rsp2 = await delPhoto(this.userInfo.Avatar)
								// 	console.log("删除图片", rsp2);
								// }
								
								//this.formData.logo = rsp
								this.AvatarImg = rsp
								console.log("图片111111111", this.AvatarImg);
								this.$refs.promptMsg.loadingColse()
								this.$forceUpdate()
							} catch (e) {
								//TODO handle the exception
								this.$refs.promptMsg.loadingColse()
								console.log(e);
								this.setMsgTop(e)
							}
						}
					}
				})
			},
			handSubmit(){
				if (!this.isSubmit) {
				       this.isSubmit = true;
				       setTimeout(() => {
				         this.isSubmit = false;
				       }, 2000); // 设置 2 秒后可再次点击
				}
				var data={
					Sex:this.userInfo.Sex,
					RealName:this.userInfo.RealName,
				}
				if(this.AvatarImg!=''){
					data.Avatar=this.AvatarImg
				}
				 updateUserProfile(data).then(response => {
				      if(response.code==0){
						  uni.showToast({
						  	title:'Modified successfully',
							icon:'none'
						  })
						  this.dataInfo();
					  }
				   }).catch((err)=>{
					   this.setMsgTop(err)
				   })
			},
			handChangePass(){
				uni.navigateTo({
					url:'./changePassword'
				})
			},
			dataInfo() {
				//个人信息
				dataList().then((res) => {
					if (res.code == 0) {
						console.log(res,'个人信息');
						this.roleGroup=res.data.roleGroup
						this.userInfo=res.data.user
						this.AvatarImg=res.data.user.Avatar
						//console.log(res.data.user.OrgId,'res.data.user.OrgId')
					}
				})
			},
			handEmails(){
				uni.navigateTo({
					url:'./BindEmail?email='+this.userInfo.Email
				})
			},
			handNumber(){
				uni.navigateTo({
					url:'./BindPhoneNumber?phone='+this.userInfo.Mobile
				})
			},
			handClick(inx){
				this.selected=inx
				this.userInfo.Sex=inx
			},
		}
	}
</script>
<style lang="less" scoped>
	
	.personData{
		
	.personData-data{
		.save-submit{
			width:92%;
			margin:0rpx 4%;
			background: linear-gradient(180deg,rgba(255, 53, 53, 1),rgba(255, 97, 61, 1));
			height:100rpx;
			border-radius: 7rpx;
			color:#fff;
			line-height: 100rpx;
			text-align: center;
			font-size:36rpx;
		}
		.personData-data-parent{
			width:92%;
			margin:30rpx 4%;
			color:#fff;
			.content{
				display: flex;
				height: 88rpx;
				align-items: center;
				border:1rpx solid rgba(255, 255, 255, 0.2);
				margin-top:20rpx;
				border-radius: 6rpx;
				padding:0rpx 20rpx;
				font-size:32rpx;
				color:rgba(255, 255, 255, 0.3);
				.contentRight{
					display: flex;
					margin-left:auto;
					font-size:28rpx;
					.icon-a-youjiantoubai{
						color:rgba(255, 255, 255, .2);
						font-size:24rpx;
						margin-left:10rpx;
					}
				}
			}
			.gender{
				display: flex;
				justify-content: space-between;
				.gender-item{
					width: 48%;
					height:88rpx;
					line-height: 88rpx;
					text-align: center;
					border-radius: 6rpx;
					margin-top:15rpx;
				}
				.active{
					border:1rpx solid rgba(255, 53, 53, 1);
					color:rgba(255, 53, 53, 1);
				}
				.on{
					border:1rpx solid rgba(255, 255, 255, 0.2);
					color:rgba(255, 255, 255, .6);
				}
			}
			.addPool-easyinput{
				background: #161A26!important;
				border:1rpx solid rgba(255, 255, 255, .2);
				border-radius: 12rpx;
				font-size:32rpx;
				height: 88rpx;
				color:#fff;
				padding:0rpx 20rpx;
			}
			.bgColor{
				background: rgba(28, 34, 50, 1)!important;
			}
			.inputValue{
				margin-top:15rpx;
			}
			.nameTitle{
				color:rgba(255, 255, 255, .4);
				font-size:32rpx;
			}
		}
		.styleParent{
			background: #1C2232;
			border:none!important;
		}
		.personData-data-header_con{
			width: 100%;
			display: flex;
			justify-content: center;
		}
		.personData-data-header{
			width: 160rpx;
			height:160rpx;
			// background: pink;
			border-radius: 50%;
			margin:50rpx auto;
		}
	}
	
	}
	
	
</style>