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
					<text style="margin-top: 10rpx;font-size: 28rpx;">修改头像</text>
				</button>

			</view>
		</view>
		<view class="common_wrap">
			<navigator url="/pages/user/user_name_select" class="row">
				<text>姓名</text>
				<view class="right ext_right">
					<text>{{name}}</text>
					<uni-icons type="forward" size="20" color="#666666"></uni-icons>
				</view>
			</navigator>
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

		</view>

		<view class="ext_wrap common_wrap" v-if="orgId>0">
			<view class="wrap-t">以下信息只在“{{orgName}}”展示</view>
			<navigator :url="'/pages/user/user_post_select?id='+orgId" class="row">
				<text>职位</text>
				<view class="right ext_right">
					<text>{{posi}}</text>
					<uni-icons type="forward" size="20" color="#666666"></uni-icons>
				</view>
			</navigator>
		</view>

		<view class="border-top" style="margin-top: 20rpx;" v-if="orgId>0" @click="exitOrg">
			<view class="border-bottom exitEn">
				退出企业
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
			async exitOrg() {
				//退出企业
				uni.showModal({
					title: '提示',
					content: '确定要退出企业吗？',
					success: async res => {
						if (res.confirm) {
							// this.$refs.popup.close();
							uni.showLoading({
								title: '加载中...'
							});
							try {
								await exitCompany(this.orgId); //访问退出企业后端
								setTimeout(() => {
									uni.showToast({
										icon: 'success',
										title: '退出成功'
									});
								}, 200)
								reloadPrePage(1, "companyEdit");
								uni.navigateBack(); //返回我的

							} catch (err) {
								console.info('异常：', err);
							} finally {
								uni.hideLoading();
							}
						}
					}
				});
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
					if (usrInfo.OrgId > 0) {
						this.orgId = usrInfo.OrgId;
						this.posi = rsp.data.user.post_name;
						let rs = await getOrg(this.orgId);
						this.orgName = rs.data.OrgName;
					} else {
						this.orgId = 0
					}
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
</style>
