<template>
	<view>
		<view class="topbox">
			<view class="box"></view>
			<view class="bps">
				<view class="bpswrap">
					<uni-icons custom-prefix="my-icon" type="my-icon-icon_choose_s" color="#ffffff" size="80">
					</uni-icons>
				</view>

			</view>
		</view>
		<view class="txtwrap">
			<text class="wft">恭喜您成功添加一张名片</text>
			<text class="mft">要继续完善名片，请点击下面的按钮</text>
		</view>

		<view class="btn-wrap">
			<button class="btn" @click="onContinueClick">
				继续完善名片
			</button>
			<navigator url="/pages/card_center/card_center" open-type="switchTab" class="txt">返回我的名片</navigator>
		</view>
	</view>
</template>

<script>
	export default {
		data() {
			return {
				cardId: 0
			};
		},
		onLoad(options) {
			this.cardId = parseInt(options.id || "0");
		},
		methods: {
			onContinueClick() {
				uni.redirectTo({
					url: 'card_edit?id=' + this.cardId
				});
			}
		}
	}
</script>

<style lang="scss">
	page {
		background-color: #ffffff;
	}

	.topbox {
		position: relative;
		width: 100vw;
		height: 300rpx;
		display: flex;
		justify-content: center;
		background-image: linear-gradient(270deg, #2CC2FB 0%, #50A6FA 100%);
		z-index: -9;

		.box {
			position: absolute;
			top: 120rpx;
			left: -15%;
			width: 130%;
			height: 300rpx;
			background-color: #ffffff;
			border-radius: 50%;
		}

		.bps {
			position: relative;

			.bpswrap {
				position: absolute;
				border-radius: 50%;
				background-color: #68BF7B;
				box-shadow: 0px -2px 10px 0px rgba(0, 0, 0, 0.1000), 0px 2px 10px 0px rgba(0, 0, 0, 0.1000);
				top: 60rpx;
				left: -40px;
			}
		}
	}

	.txtwrap {
		display: flex;
		flex-direction: column;

		.wft {
			font-size: 34rpx;
			font-weight: bolder;
			color: #333333;
			display: flex;
			justify-content: center;
			margin-bottom: 30rpx;
		}

		.mft {
			font-size: 28rpx;
			color: #666666;
			display: flex;
			justify-content: center;
		}
	}

	.btn-wrap {
		margin-top: 100rpx;

		.btn {
			margin-left: 60rpx;
			margin-right: 60rpx;
			color: #FFFFFF;
			background: linear-gradient(270deg, #2CC2FB 0%, #50A6FA 100%);
			border-radius: 8rpx;
		}

		.txt {
			margin-top: 40rpx;
			text-align: center;
			height: 90rpx;
			display: flex;
			justify-content: center;
			align-items: center;
		}
	}
</style>
