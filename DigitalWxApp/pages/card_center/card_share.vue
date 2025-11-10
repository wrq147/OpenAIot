<template>
	<view>
		<view class="myHd">
			<image class="img" src="/static/user/user_man.png" mode="widthFix"></image>
			<view class="arrow-t"></view>
			<view class="arrow-b"></view>
			<view class="cardPreview">
				<view class="cft">
					<image class="limg" src="/static/logo.png" mode="widthFix"></image>
					<text class="txt">数字好名片</text>
				</view>
				<view class="t">{{shareTitle}}</view>
				<view class="c">
					<view class="canvas-hide">
						<!-- #ifdef MP-WEIXIN -->
						<canvas id="canvas" type="2d" style="width:420rpx;height: 336rpx;" />
						<!-- #endif -->
						<!-- #ifndef MP-WEIXIN -->
						<canvas canvas-id="canvas" id="canvas" style="width:420rpx;height: 336rpx;" />
						<!-- #endif -->
					</view>
				</view>
			</view>

		</view>
		<view class="sbte">
			<view class="tt">修改分享标题</view>
			<view class="shtit">
				<label class="xradio" @click="radioClick(1)">
					<radio value="1" color="#50A6FA" :checked="radioVal==1" style="transform:scale(0.7)" />这是我的数字名片，请收下
				</label>
				<label class="xradio" @click="radioClick(2)">
					<radio value="2" color="#50A6FA" :checked="radioVal==2" style="transform:scale(0.7)" />
					您好，这是我的名片，点击自动保存
				</label>
				<view class="xtxtww" @click="radioClick(3)">
					<radio value="3" color="#50A6FA" :checked="radioVal==3" style="transform:scale(0.7)" />
					<view class="xtxtar">
						<uni-easyinput type="textarea" maxlength="30" v-model="customTitle" :autoHeight="true"
							:inputBorder="false"></uni-easyinput>
						<view class="xnum">{{customTitle.length}}/30</view>
					</view>
				</view>

			</view>
			<view style="height: 60rpx;"></view>
			<button class="btn" @click="saveClick">
				保存
			</button>
		</view>
	</view>
</template>

<script>
	import {
		drawCard
	} from '@/common/card.js'
	import {
		getCardInfo,
		editCard
	} from '@/api/userCard.js'
	import {
		reloadPrePage
	} from '@/common/util.js'
	import {
		useDrawPoster
	} from '@/js_sdk/u-draw-poster'
	export default {
		data() {
			return {
				localImg: "",
				buttonShare: "",
				cardId: 0,
				shareTitle: "",
				radioVal: -1,
				customTitle: "",
				dataShareTitle: "" //数据库存储的分享标题字段
			};
		},
		onLoad(options) {
			this.cardId = parseInt(options.id || "0");
		},
		async onReady() {
			try {
				let rsp = await getCardInfo(this.cardId);
				this.shareTitle = rsp.data.ShareTitle == "" ? "这是我的数字名片，请收下" : rsp.data.ShareTitle;
				this.dataShareTitle = rsp.data.ShareTitle;
				if (this.dataShareTitle && this.dataShareTitle != "这是我的数字名片，请收下" && this.dataShareTitle !=
					"您好，这是我的名片，点击自动保存") {
					this.radioVal = 3;
					this.customTitle = this.dataShareTitle;
				} else {
					if (this.shareTitle == "这是我的数字名片，请收下") {
						this.radioVal = 1;
					} else if (this.shareTitle == "您好，这是我的名片，点击自动保存") {
						this.radioVal = 2;
					} else {
						this.radioVal = 3;
						this.customTitle = this.shareTitle;
					}
				}
				let dp = await drawCard('canvas', rsp.data, true);
				this.localImg = await dp.create();
				// console.log("this.localImg", this.localImg);
			} catch (err) {
				console.info('异常：', err);
			}
		},
		watch: {
			customTitle(val) {
				if (this.radioVal == 3) {
					this.shareTitle = val;
				}
			}
		},
		methods: {

			radioClick(val) {
				this.radioVal = val;
				if (val == 1) {
					this.shareTitle = "这是我的数字名片，请收下";
				} else if (val == 2) {
					this.shareTitle = "您好，这是我的名片，点击自动保存";
				} else {
					this.shareTitle = this.customTitle;
				}
			},
			async saveClick() {
				if (this.shareTitle == "") {
					uni.showToast({
						title: "分享标题不能为空",
						icon: "none",
						duration: 2000
					});
					return;
				}
				uni.showLoading({
					title: '加载中...'
				});
				try {
					await editCard({
						Id: this.cardId,
						ShareTitle: this.shareTitle
					});

					reloadPrePage(1, "share");
					uni.navigateBack();
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
	// .canvas-hide {
	// 	/* 1 */
	// 	position: fixed;
	// 	right: 100vw;
	// 	bottom: 100vh;
	// 	/* 2 */
	// 	z-index: -9999;
	// 	/* 3 */
	// 	opacity: 0;
	// }

	.button_share {
		width: 420rpx;
		box-sizing: border-box;
		height: 100rpx;
		line-height: 100rpx;
		display: flex;
		justify-content: center;
		align-items: center;

		.share_text {
			width: 300rpx;
			height: 60rpx;
			line-height: 60rpx;
			background-color: #48ADFB;
			border-radius: 30rpx;
			color: #ffffff;
			text-align: center;
		}
	}

	.myHd {
		display: flex;
		justify-content: flex-end;
		margin-top: 30rpx;
		margin-right: 30rpx;
		position: relative;

		.img {
			width: 85rpx;
			height: 85rpx;
			background-color: #FFFFFF;
			border-radius: 10px;
		}

		.arrow-t {
			position: absolute;
			top: 20rpx;
			right: 100rpx;
			width: 0;
			height: 0;
			border: 20rpx solid;

			border-color: transparent transparent #FFFFFF transparent;
		}

		.arrow-b {
			position: absolute;
			top: 52rpx;
			right: 100rpx;
			width: 0;
			height: 0;
			border: 30rpx solid;
			border-color: #FFFFFF transparent transparent transparent;
		}


		.cardPreview {
			position: absolute;
			right: 120rpx;
			top: 22rpx;
			width: 480rpx;
			background-color: #FFFFFF;
			border-radius: 16rpx;
			display: flex;
			flex-direction: column;
			box-sizing: border-box;
			padding-left: 30rpx;
			padding-right: 30rpx;
			padding-bottom: 20rpx;
			margin-bottom: 60vh;

			.cft {
				display: flex;
				height: 68rpx;
				align-items: center;
				padding-top: 10rpx;

				.limg {
					width: 48rpx;
					height: 48rpx;
					border-radius: 50%;
				}

				.txt {
					margin-left: 10rpx;
					font-size: 22rpx;
					color: #666666;
				}
			}

			.t {
				font-size: 30rpx;
				color: #333333;
				font-weight: bold;
				padding-top: 10rpx;
				padding-bottom: 10rpx;
				word-break: break-all;
				word-wrap: break-word;
			}
		}

	}

	.sbte {
		position: fixed;
		bottom: 0;
		left: 0;
		width: 100vw;
		background-color: #FFFFFF;

		.tt {
			padding-top: 30rpx;
			padding-left: 30rpx;
			padding-right: 30rpx;
			padding-bottom: 10rpx;
			color: #666666;
		}

		.shtit {
			display: flex;
			flex-direction: column;
			padding: 0 30rpx;

			.xradio {
				padding: 10rpx 0;
			}

			.xtxtww {
				display: flex;
				flex-direction: row;
				padding-top: 10rpx;
			}

			.xtxtar {
				flex: 1;
				display: flex;
				flex-direction: column;
				border-radius: 10rpx;
				border: solid 1px #dadada;
				padding: 0 20rpx;
				height: 200rpx;

				.uni-easyinput__content-textarea {
					height: 120rpx;
					min-height: 120rpx;
					line-height: 50rpx;
				}

				.xnum {
					display: flex;
					justify-content: flex-end;
					font-size: 24rpx;
					color: #999;
					padding: 10rpx 0;
				}
			}
		}

		.btn {
			margin: 20rpx 30rpx;
			background: linear-gradient(270deg, #2CC2FB 0%, #50A6FA 100%);
			border-radius: 8rpx;
			color: #ffffff;
		}
	}
</style>
