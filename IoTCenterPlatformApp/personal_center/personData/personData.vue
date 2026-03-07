<template>
	<view class="personData">
		<top :isleftBack="true" leftIcon="icon-fanhui" backgroundColor="#ffffff" title="个人信息"
			class="CRM-header"></top>
		<view class="personData-data">
			<view class="personData-data-header_con" @click="uploadMul">
				<view class="NameCutting" v-if="!userName&&!userName.length<=0">
					{{userName.length>2?userName.slice(-2):userName}}
				</view>
				<image v-else alt="头像加载失败" :src="AvatarImg" mode="" class="personData-data-header"></image>
				<view class="t-icon-shangchuantouxiang1">
				</view>
			</view>
			<view class="personData-data-parent">
				<view class="nameTitle">
					签名
				</view>
				<view class="inputValue">
					<input v-model="userInfo.WaitSignature" type="text" class="addPool-easyinput" placeholder="请输入用户名"
						placeholder-style="color:rgba(255, 255, 255, .2);">
				</view>
			
			</view>
			<view class="personData-data-parent">
				<view class="nameTitle">
					用户名
				</view>
				<view class="inputValue">
					<input v-model="userInfo.RealName" type="text" class="addPool-easyinput" placeholder="请输入用户名"
						placeholder-style="color:rgba(255, 255, 255, .2);">
				</view>

			</view>

			<view class="personData-data-parent">
				<view class="nameTitle">
					性别
				</view>
				<view style="margin-top:10rpx;">
					<uni-data-checkbox v-model="userInfo.Sex" :localdata="locatDsex"></uni-data-checkbox>
				</view>
			</view>

			<view class="personData-data-parent">
				<view class="nameTitle">
					手机号码
				</view>
				<view class="content">
					<view style="color:#333;">
						{{userInfo.Mobile?userInfo.Mobile:'请绑定您的电子邮件和电话号码'}}
					</view>
					<view class="contentRight" @click="handNumber">
						<view>
							手机号
						</view>
						<view class="iconfont icon-a-youjiantoubai">

						</view>
					</view>
				</view>
			</view>
			<view class="personData-data-parent">
				<view class="nameTitle">
					用户邮箱
				</view>
				<view class="content">
					<view style="color:#333;">
						{{userInfo.Email?userInfo.Email:'请绑定您的电子邮件'}}
					</view>
					<view class="contentRight" @click="handEmails()">
						<view>
							去绑定
						</view>
						<view class="iconfont icon-a-youjiantoubai">

						</view>
					</view>
				</view>
			</view>


			<view class="personData-data-parent">
				<view class="nameTitle">
					所属部门
				</view>
				<view class="inputValue">
					<input v-model="userInfo.dept_name" type="text" disabled class="addPool-easyinput actiBorder"
						placeholder="请输入用户名" placeholder-style="color:rgba(255, 255, 255, .2);">
				</view>

			</view>


			<view class="personData-data-parent">
				<view class="nameTitle">
					所属角色
				</view>
				<view class="inputValue">
					<input v-model="roleGroup" type="text" disabled class="addPool-easyinput actiBorder"
						placeholder="公司管理人、代理人、初始成员" placeholder-style="color:rgba(255, 255, 255, .2);">
				</view>
			</view>


			<view class="personData-data-parent">
				<view class="nameTitle">
					创建时间
				</view>
				<view class="inputValue">
					<input v-model="userInfo.createTime" type="text" disabled class="addPool-easyinput actiBorder"
						placeholder="创建时间" placeholder-style="color:rgba(255, 255, 255, .2);">
				</view>
			</view>



			<view class="personData-data-parent">
				<view class="nameTitle">
					其他
				</view>
				<view class="content styleParent" @click="handChangePass">
					<view style="color:#333;">
						修改密码
					</view>
					<view class="contentRight">
						<view class="iconfont icon-a-youjiantoubai">

						</view>
					</view>
				</view>
			</view>
			<button :disabled="isSubmit" class="save-submit" @click="handSubmit">
				保存
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
	import {
		setPagesParam
	} from '@/common/utillib.js'
	export default {
		data() {
			return {
				radio1: 0,
				locatDsex: [{
					text: '男',
					value: 0
				}, {
					text: '女',
					value: 1
				}, {
					text: '未知',
					value: 2
				}],
				isSubmit: false,
				roleGroup: '',
				userInfo: {},
				Sex: '',
				selected: 0,
				arr: [{
						title: 'Male'
					},
					{
						title: 'Female'
					},
				],
				AvatarImg: '',
				formData: {
					logo2: '',

				},
				userName: '',
			}
		},
		onLoad() {
			this.dataInfo();
		},
		methods: {
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
								// console.log(paths, 'pathspaths');
								let rsp = await uploadPhoto(paths)
								if (this.AvatarImg) {
									let rsp2 = await delPhoto(this.AvatarImg)
									console.log("删除图片", rsp2);
								}

								//this.formData.logo = rsp
								this.AvatarImg = rsp
								// console.log("图片111111111", this.AvatarImg);
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
			handSubmit() {
				if (!this.isSubmit) {
					this.isSubmit = true;
					setTimeout(() => {
						this.isSubmit = false;
					}, 2000); // 设置 2 秒后可再次点击
				}
				var data = {
					Sex: this.userInfo.Sex,
					RealName: this.userInfo.RealName,
					WaitSignature: this.userInfo.WaitSignature,
					Signature:this.userInfo.WaitSignature
				}
				if (this.AvatarImg != '') {
					data.Avatar = this.AvatarImg
				}
				// console.log(data,'datadatadatadata')
				// return
				updateUserProfile(data).then(response => {
					if (response.code == 0) {
						uni.showToast({
							title: '修改成功！',
							icon: 'none'
						})
						this.dataInfo();
						setPagesParam('reloadPersonalInfo', 'load', 1,false)
					}
				}).catch((err) => {
					this.setMsgTop(err)
				})
			},
			handChangePass() {
				uni.navigateTo({
					url: './changePassword'
				})
			},
			dataInfo() {
				//个人信息
				dataList().then((res) => {
					if (res.code == 0) {
						//console.log(res,'个人信息');
						this.roleGroup = res.data.roleGroup
						this.userInfo = res.data.user
						this.AvatarImg = res.data.user.Avatar
						this.userName = res.data.user.RealName
						this.userInfo.Sex = parseInt(res.data.user.Sex)
						//console.log(res.data.user.Avatar,'res.data.user.Avatar')
						//console.log(res.data.user.OrgId,'res.data.user.OrgId')
					}
				})
			},
			handEmails() {
				uni.navigateTo({
					url: './BindEmail?email=' + this.userInfo.Email
				})
			},
			handNumber() {
				uni.navigateTo({
					url: './BindPhoneNumber?phone=' + this.userInfo.Mobile
				})
			},
			handClick(inx) {
				this.selected = inx
				this.userInfo.Sex = inx
			},
		}
	}
</script>
<style>
	page {
		background: #ffffff;
	}
</style>
<style lang="less" scoped>
	.personData {
		.personData-data {
			.save-submit {
				width: 92%;
				margin: 0rpx 4%;
				background: #2371FF;
				height: 100rpx;
				border-radius: 7rpx;
				color: #fff;
				line-height: 100rpx;
				text-align: center;
				font-size: 32rpx;
			}

			.personData-data-parent {
				width: 92%;
				margin: 30rpx 4%;
				color: #fff;

				.content {
					display: flex;
					height: 88rpx;
					align-items: center;
					// border:1rpx solid rgba(255, 255, 255, 0.2);
					margin-top: 20rpx;
					border-radius: 6rpx;
					background: #F8F8F8;
					padding: 0rpx 20rpx;
					color: #999999;
					font-size: 30rpx;

					.contentRight {
						display: flex;
						margin-left: auto;
						font-size: 24rpx;
						align-items: center;

						.icon-a-youjiantoubai {
							color: #999999;
							font-size: 18rpx;
							margin-left: 10rpx;
						}
					}
				}

				.gender {
					display: flex;
					justify-content: space-between;

					.gender-item {
						width: 48%;
						height: 88rpx;
						line-height: 88rpx;
						text-align: center;
						border-radius: 6rpx;
						margin-top: 15rpx;
					}

					.active {
						border: 1rpx solid #2371FF;
						color: #2371FF;
					}

					.on {
						border: 1rpx solid #EAEAEA;
						color: #999999;
					}
				}

				.addPool-easyinput {
					background: #F8F8F8 !important;
					border: 1rpx solid rgba(255, 255, 255, .2);
					border-radius: 12rpx;
					height: 88rpx;
					color: #333333;
					padding: 0rpx 20rpx;
				}

				.bgColor {
					background: #F8F8F8 !important;
				}

				.inputValue {
					margin-top: 15rpx;
				}

				.actiBorder {
					border: 2rpx solid #EAEAEA;
				}

				.nameTitle {
					color: #999999;
				}
			}

			.styleParent {
				background: #EAEAEA;
				border: none !important;
			}

			.personData-data-header_con {
				position: relative;
				display: flex;
				width: 160rpx;
				height: 160rpx;
				margin: 0rpx auto;
				border-radius: 50%;
				justify-content: center;
				margin-bottom: 50px;

				.t-icon-shangchuantouxiang1 {
					position: absolute;
					width: 50rpx;
					height: 50rpx;
					bottom: -50rpx;
					right: 0rpx;
				}

				.NameCutting {
					width: 160rpx;
					height: 160rpx;
					border-radius: 50%;
					line-height: 160rpx;
					text-align: center;
					color: #fff;
					font-size: 40rpx;
					margin: 50rpx auto;
					background: rgba(35, 113, 255, 1);
				}
			}

			.personData-data-header {
				width: 160rpx;
				height: 160rpx;
				// background: pink;
				border-radius: 50%;
				margin: 50rpx auto;
			}
		}

	}
</style>