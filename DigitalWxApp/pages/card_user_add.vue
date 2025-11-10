<template>
	<view>
		<view class="wrap">
			<view class="mp">
				<!-- #ifdef MP-WEIXIN -->
				<canvas id="canvas" type="2d" style="width:690rpx; height:400rpx;" />
				<!-- #endif -->
				<!-- #ifndef MP-WEIXIN -->
				<canvas canvas-id="canvas" id="canvas" style="width:690rpx; height:400rpx;" />
				<!-- #endif -->
			</view>
		</view>
		<view class="choose">
			<view class="title">
				选择名片样式
			</view>
			<scroll-view :scroll-x="true" class="content">
				<block>
					<view class="contwrap">
						<view v-for="(bg,idx) in bglist" class="item" @click="bgSelected(idx)">
							<view class="ac" v-if="(idx+1)==cardData.TemplateBk"></view>
							<fr-image class="ximg" :lazy-load="true" mode="widthFix" :src="bg"
								loading-ing-img="oblique-light" />
						</view>
					</view>
				</block>
			</scroll-view>
			<view class="border-bottom" style="padding-top: 40rpx;"></view>
		</view>
		<view class="xrow border-bottom">
			<view class="xleft">姓名</view>
			<view class="xright ipt">
				<uni-easyinput class="weui-input" type="nickname" trim="both" maxlength="20" @input="changeDraw"
					v-model="cardData.RealName" :inputBorder="false" placeholder="请输入姓名">
				</uni-easyinput>
				<!-- <input type="nickname" class="weui-input" @input="changeDraw" v-model="cardData.RealName" placeholder="请输入姓名"/> -->
			</view>
		</view>

		<view class="xrow border-bottom">
			<view class="xleft">手机</view>
			<view class="xright ipt">
				<uni-easyinput trim="both" type="number" maxlength="11" @input="changeDraw" v-model="cardData.Mobile"
					:inputBorder="false" placeholder="请输入手机号码">
				</uni-easyinput>
			</view>
		</view>
		<view class="xrow border-bottom">
			<view class="xleft">头像</view>
			<view class="xright ipt betwen">
				<image class="avatar" :src="cardData.Avatar?cardData.Avatar:'../static/user/user_man.png'"
					mode="aspectFill" @click.stop="seeImg(cardData.Avatar)" style="width: 100rpx;height: 80rpx;">
				</image>
				<button @click="onChooseAvatar" class="avatar-wrapper" open-type="chooseAvatar"
					@chooseavatar="onChooseAvatar">

				</button>
				<view class="iconwrap">
					<uni-icons type="forward" size="20" color="#666666"></uni-icons>
				</view>
			</view>
		</view>
		<view class="xrow border-bottom" @click="changeCompany">
			<view class="xleft">企业</view>
			<view class="xright betwen">
				<view class="tip" v-if="cardData.OrgId>0">{{cardData.OrgName}}</view>
				<view class="tip huise" v-else>请选择名片所属企业</view>
				<view class="iconwrap">
					<uni-icons type="forward" size="20" color="#666666"></uni-icons>
				</view>
			</view>
		</view>
		<view class="xrow border-bottom" v-if="cardData.OrgId>0" style="margin-bottom: 20px;">
			<view class="xleft">职位</view>
			<view class="xright ipt">
				<uni-easyinput trim="both" maxlength="50" v-model="cardData.PostName" @input="changeDraw"
					:inputBorder="false" placeholder="请输入所属职位">
				</uni-easyinput>
			</view>
		</view>
		<view style="height: 100rpx;">

		</view>
		<view class="border-btn" style="border-top:none;">
			<view class="btn_con">
				<button class="btn1" @click="skipClick">
					跳过
				</button>
			</view>
			<view class="btn_con">
				<button class="btn" @click="onSaveClick">
					保存
				</button>
			</view>

		</view>
	</view>
</template>

<script>
	import {
		getDrawObj,
		drawCard
	} from '@/common/card.js'

	import {
		addCard
	} from '@/api/userCard.js'
	import {
		getOrg
	} from '@/api/org.js'
	import {
		reloadPrePage
	} from '@/common/util.js'
	import {
		editUser
	} from '@/api/user.js'
	import {
		uploadAvatar
	} from '@/api/file.js'
	import request from '@/common/request.js'
	export default {
		data() {
			return {
				cardId: 0,
				cardData: {
					RealName: "",
					Mobile: "",
					OrgName: "",
					PostName: "",
					Avatar: "",
					TemplateId: 1,
					TemplateBk: 1,
					OrgId: 0,
					usrInfo: null
				},
				bglist: [],
				orgData: {}, //企业信息
				avatarUrl: '',
			};
		},
		onLoad(options) {
			this.cardId = parseInt(options.cardId || "0");
			this.bglist = getDrawObj().bglist;
		},
		async onReady() {
			try {
				this.usrInfo = await this.$store.dispatch("userInfo");
				// console.log("gerenxinxi",this.usrInfo);
				this.cardData.Avatar = this.usrInfo.avatar;
				await drawCard('canvas', this.cardData);
			} catch (err) {
				console.info('异常：', err);
			}
		},
		onShow() {
			drawCard('canvas', this.cardData);
		},

		methods: {
			skipClick() {
				//跳过
				uni.switchTab({
					url: "/pages/card_center/card_center"
				})
			},
			seeImg(img) { //预览头像
				if (img) {
					let list = []
					list.push(img)
					uni.previewImage({
						urls: list,
						longPressActions: {
							success: function(data) {

							},
							fail: function(err) {
								console.log(err.errMsg);
							}
						}
					});
				}

			},
			async onChooseAvatar(e) {
				if (this.$store.state.sysType == 'windows') {
					uni.navigateTo({
						url: '/pages/card_center/card_avator?isFirst=1'
					})
				} else {
					// console.log("头像的只", e);
					let respath = await uploadAvatar(e.detail.avatarUrl);
					this.cardData.disAvatarUrl = respath
					this.cardData.Avatar = request.config.baseURL + respath
				}

			},

			changeCompany() {
				this.$store.state.submitMod.formArrary.push(this.cardData);
				uni.navigateTo({
					url: '/pages/company/company_sel'
				});
			},
			bgSelected(idx) {
				this.cardData.TemplateBk = idx + 1;
				drawCard('canvas', this.cardData);
			},
			async changeDraw(e) {
				await drawCard('canvas', this.cardData);
			},
			async onSaveClick() {
				if (this.cardData.OrgId <= 0) {
					uni.showToast({
						title: "请选择企业",
						icon: "none",
						duration: 2000
					});
					return;
				}
				if (this.cardData.PostName == '' || this.cardData.PostName == null) {
					uni.showToast({
						title: "请填写职位",
						icon: "none",
						duration: 2000
					});
					return;
				}
				if (this.cardData.RealName == '' || this.cardData.RealName == null) {
					uni.showToast({
						title: "请填写姓名",
						icon: "none",
						duration: 2000
					});
					return;
				}
				if (this.cardData.Mobile == '' || this.cardData.Mobile == null) {
					uni.showToast({
						title: "请填写联系方式",
						icon: "none",
						duration: 2000
					});
					return;
				} else if (!(/^1[3456789]\d{9}$/.test(this.cardData.Mobile))) {
					uni.showToast({
						title: '手机号码格式错误',
						icon: "none"
					});
					return;
				}

				uni.showLoading({
					title: '加载中...'
				});
				try {
					if (this.cardData.OrgId > 0 && this.cardData.PostName && this.cardData.Mobile && this.cardData
						.RealName) {
						let result = await getOrg(this.cardData.OrgId);
						// console.log("企业信息",result);
						this.orgData = result.data;

						let rsp = await addCard({
							RealName: this.cardData.RealName,
							Mobile: this.cardData.Mobile,
							WxNumber: '',
							Website: '',
							Email: '',
							Avatar: this.cardData.Avatar,
							TemplateId: 1,
							TemplateBk: this.cardData.TemplateBk,
							OrgId: this.cardData.OrgId,
							DeptName: this.cardData.DeptName,
							PostName: this.cardData.PostName,
							ShareTitle: '',
							Intro: '',
							AddressCode: this.orgData.AddressCode,
							AddressDetail: this.orgData.AddressDetail,
							AddressName: this.orgData.AddressName,
							Lat: this.orgData.Lat,
							Lng: this.orgData.Lng
						});
						if ((this.usrInfo.name == '微信用户' || this.usrInfo.name == '') && this.usrInfo.avatar == '') {
							await editUser({
								RealName: this.cardData.RealName,
								Avatar: this.cardData.disAvatarUrl
							});
							this.$store.commit('SET_USER_INFO', null);
							this.$store.commit('CARRY_USER_SHOW', true);
						} else if (this.usrInfo.name == '微信用户' || this.usrInfo.name == '') {
							await editUser({
								RealName: this.cardData.RealName
							});
							this.$store.commit('SET_USER_INFO', null);
							this.$store.commit('CARRY_USER_SHOW', true);
						} else if (this.usrInfo.avatar == '') {
							await editUser({
								Avatar: this.cardData.disAvatarUrl
							});
							this.$store.commit('SET_USER_INFO', null);
							this.$store.commit('CARRY_USER_SHOW', true);
						}
						if(this.cardId){
							uni.redirectTo({
								url:'/pages/card_center/card?id=' + this.cardId
							})
						}else{
							uni.redirectTo({
								url: '/pages/card_center/card_success?id=' + rsp.data
							});
						}
						
					}


				} catch (err) {
					console.info('异常：', err);
				} finally {
					uni.hideLoading();
				}
			}
		}
	}
</script>

<style lang="scss">
	page {
		background-color: #FFFFFF;
	}

	.wrap {
		width: 100%;
		box-sizing: border-box;
		padding-left: 30rpx;
		padding-right: 30rpx;
		background-color: #FFFFFF;
		margin-bottom: 20rpx;

		.mp {
			width: 690rpx;
			height: 400rpx;
			overflow: hidden;
			border-radius: 10rpx;
			box-sizing: border-box;
			color: #FFFFFF;
			position: relative;
			background-color: #14395F;
			box-shadow: 0px 6px 15px 0px rgba(0, 0, 0, 0.2000);


		}

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


	.choose {
		margin-top: 61rpx;
		padding-left: 30rpx;
		padding-right: 30rpx;

		.title {
			box-sizing: border-box;
			width: 100%;
			font-size: 30rpx;
			color: #999999;
			margin-bottom: 30rpx;
		}

		.content {

			height: 87rpx;
			white-space: nowrap;
			width: 100%;

			.contwrap {


				.item {
					display: inline-block;
					margin-right: 22rpx;
					position: relative;

					.ac {
						position: absolute;
						top: 0;
						left: 0;
						width: 147rpx;
						height: 87rpx;
						background: linear-gradient(270deg, #2CC2FB 0%, #50A6FA 100%);
						background-image: url('/static/icon_choose_n.png');
						z-index: 9;
						background-size: 50rpx 50rpx;
						background-repeat: no-repeat;
						background-position: center;
					}

					.ximg {
						display: flex;
						width: 147rpx;
						height: 87rpx;
						overflow: hidden;
					}
				}
			}


		}

	}

	.xrow {
		display: flex;
		justify-content: space-between;
		align-items: center;
		padding: 0 30rpx;
		height: 90rpx;
		font-size: 30rpx;

		.xleft {
			width: 200rpx;
		}

		.xright {
			flex: 1;

			.uni-easyinput__content-input {
				font-size: 30rpx !important;
			}

			.huise {
				color: #999999 !important;
				font-size: 26rpx;
			}

			&.ipt {
				margin-left: -10px;
			}

			&.betwen {
				display: flex;
				justify-content: space-between;
				align-items: center;
			}

			.avatar-wrapper {
				width: 100%;
				height: 80rpx;
				display: flex;
				justify-content: flex-start;
				align-items: center;
				background-color: #FFFFFF;

				.avatar {
					width: 100rpx;
					height: 80rpx;
					border-radius: 5rpx;
				}
			}

			.iconwrap {
				width: 34rpx;
				height: 90rpx;
				display: flex;
				justify-content: center;
				align-items: center;
				padding-left: 20rpx;
			}
		}
	}
</style>
