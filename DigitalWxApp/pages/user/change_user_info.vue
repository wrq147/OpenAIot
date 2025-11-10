<template>
	<view>
		<view class="top">
			<view class="nav" @click="onChooseAvatar">
				<!-- <image v-if="userOterInfo.Avatar" :src="userOterInfo.Avatar" class="photo"></image> -->
				<!-- <image v-else src="../../static/user/user_man.png" class="photo"></image> -->
				<button class="avatar-wrapper" open-type="chooseAvatar" @chooseavatar="onChooseAvatar"
					style="padding: 0;background-color: #fff;text-align: center;">
					<image class=" photo avatar"
						:src="userOterInfo.Avatar?userOterInfo.Avatar:'../../static/user/user_man.png'"
						mode="aspectFill" style="margin-left: 5rpx;margin-top: 22rpx;">
					</image>
					<text style="margin-top: 10rpx;font-size: 28rpx;">上传头像</text>
				</button>

			</view>
		</view>
		<view class="common_wrap">
			<view class="row">
				<text>姓名</text>
				<view class="right ext_right">
					<!-- <text>{{name}}</text>
					<uni-icons type="forward" size="20" color="#666666"></uni-icons> -->
					<uni-easyinput class="weui-input" type="nickname" trim="both" maxlength="20" v-model="name"
						@blur="saveName" :inputBorder="false" placeholder="请输入姓名">
					</uni-easyinput>
				</view>
			</view>
			<view class="row">
				<text>性别</text>
				<view class="right ext_right">
					<!-- <text>男</text> -->
					<picker style="flex: 1;text-align: right;" @change="bindSexChange" :value="index" :range="sexarray">
						<text>{{sexarray[index]}}</text>
					</picker>
					<uni-icons type="forward" size="20" color="#666666"></uni-icons>
				</view>
			</view>
			<view class="border-btn" style="border-top:none;">
				<!-- <view class="btn_con">
				<button class="btn1" @click="skipClick">
					跳过
				</button>
			</view> -->
				<view class="btn_con">
					<button class="btn" @click="onSaveClick">
						保存
					</button>
				</view>

			</view>
		</view>


	</view>
</template>

<script>
	import {
		uploadFile,
		uploadAvatar
	} from '@/api/file.js'
	import {
		getSelfInfo,
		editUser
	} from '@/api/user.js'
	import {
		getOrg
	} from '@/api/org.js'
	import {
		reloadPrePage
	} from '@/common/util.js'
	import {
		exitCompany
	} from '@/api/company.js'
	import request from '@/common/request.js'

	export default {
		data() {
			return {
				name: null,
				posi: '',
				sexarray: ['男', '女', '未知'],
				index: 0,
				headImg: "",
				orgId: 0,
				userOterInfo: {},
				orgName: ""
			}
		},
		onLoad() {
			this.reloadpage();
		},
		methods: {
			async onSaveClick() {
				if (this.name == '微信用户' || this.name == '') {
					if (this.name == '') {
						uni.showToast({
							title: '请输入用户名',
							icon: "none"
						});
					}
					if (this.name == '微信用户') {
						uni.showToast({
							title: '用户名错误',
							icon: "none"
						});
					}

				} else {
					await editUser({
						RealName: this.name
					});
					reloadPrePage(1,'changeName');
				}
				if (this.userOterInfo.Mobile) {
					uni.navigateBack();
				} else {
					uni.navigateTo({
						url: '/pages/user/change_phone'
					})
				}

			},
			async saveName() {
				if (this.name == '微信用户' || this.name == '') {
					await editUser({
						RealName: this.name
					});
				}
			},
			async onChooseAvatar(e) {
				if (this.$store.state.sysType == 'windows') {
					uni.navigateTo({
						url: '/pages/user/user-avatar'
					})
				} else {
					let respath = await uploadAvatar(e.detail.avatarUrl);
					let res = await editUser({
						Avatar: respath
					})
					this.userOterInfo.Avatar = request.config.baseURL + respath
					this.$store.commit('SET_USER_INFO', null);
					reloadPrePage(1, "useravator");
				}

			},
			async reloadpage(name) {
				if (name == 'avator') {
					this.$forceUpdate();
				}
				try {
					let rsp = await getSelfInfo();

					this.userOterInfo = rsp.data.user
					this.name = rsp.data.user.RealName;

					this.index = rsp.data.user.Sex;
					this.headImg = rsp.data.user.Avatar;

					let usrInfo = await this.$store.dispatch("userInfo");
				} catch (err) {
					console.info('异常：', err);
				}
			},
			async bindSexChange(e) {
				this.index = e.detail.value

				try {
					this.userOterInfo.Sex = this.index
					await editUser({
						Sex: this.index
					})
				} catch (e) {
					//TODO handle the exception
					console.info('异常：', e);
				}
			}

		}
	}
</script>

<style lang="scss">
	.top {
		width: 100%;
		height: 198rpx;
		background-color: #FFFFFF;
		padding: 0 30rpx;
		box-sizing: border-box;
		display: flex;
		align-items: center;
		justify-content: center;
		margin-top: 10rpx;
		margin-bottom: 20rpx;

		.nav {
			display: flex;
			flex-direction: column;
			padding: 10rpx 10rpx;

			.photo {
				display: block;
				width: 100rpx;
				height: 100rpx;
				border-radius: 50%;
			}
		}

	}


	.common_wrap .row .right text {
		color: #666666;
	}

	.common_wrap .row .right input {
		color: #666666;
	}

	.ext_wrap {
		.wrap-t {
			padding: 20rpx 30rpx;
			color: #666;
		}
	}

	.exitEn {
		display: flex;
		height: 90rpx;
		align-items: center;
		justify-content: center;
		color: #ff0000;
		background-color: #FFFFFF;
	}

	.ext_right {
		flex: 1;
		display: flex;
		justify-content: flex-end;
	}

	.border-btn {
		display: flex;
		justify-content: space-around;
		bottom: 30rpx;

		.btn_con {
			width: 330rpx;

			.btn1 {
				width: 100%;
				height: 100rpx;
				line-height: 100rpx;
				text-align: center;
				font-size: 30rpx;
				background-color: #FFFFFF;
				color: #333333;
				border: 2rpx solid rgba(232, 232, 232, 1);
				border-radius: 8rpx;
			}
		}
	}
</style>
