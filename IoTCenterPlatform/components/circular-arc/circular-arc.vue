<template>
	<view>
		<div class="shape" :style="{'--line-color':lineColor}">
			<div class="bottom"></div>
			<div class="cover">
				<uni-icons custom-prefix="iconfont" color="#FFFFFF" type="icon-a-shebei-xianxingxiao"
					:size="10"></uni-icons>
			</div>
			<div class="ring-left"></div>
			<div class="ring-right"></div>
			<div class="ring-hide">
				<div></div>
			</div>
		</div>

	</view>
</template>

<script>
	export default {
		name: "circular-arc",
		props: {
			lineColor: {
				type: String,
				default: "#FF3535"
			},
			value: {
				type: Number,
				default: 40
			}
		},
		data() {
			return {

			};
		},
		watch: {
			value: {
				handler(newName, oldName) {
					this.setPercentValue(newName);
				},
				// 代表在wacth里声明了firstName这个方法之后立即先去执行handler方法
				immediate: false,
				deep: true,
			},
		},
		mounted() {
			this.setPercentValue(this.value);
		},
		methods: {
			setPercentValue(value) {
				let cover = document.querySelector('.shape>.cover'),
					left = document.querySelector('.shape>.ring-left'),
					hide = document.querySelector('.shape>.ring-hide');
				let deg; //左环转动角度
				if (Number.isNaN(value) || value < 0) value = 0;
				if (value > 100) value = 100;
				if (value % 1 !== 0) { //若value为小数，保留一位小数
					value = value.toFixed(1);
				}
				// cover.innerText = value + "%";
				if (value < 50) { //值小于50，显示遮罩
					hide.style.display = "inherit";
					deg = (50 - value) / 50 * 180;
				} else { //值大于或等于50，不显示遮罩
					hide.style.display = "none";
					deg = (50 - (value - 50)) / 50 * 180;
				}
				left.style.transform = "rotate(" + (-deg) + "deg)";
			}

		}
	}
</script>

<style lang="scss" scoped>
	// 占有百分比的显示颜色
	$line-color: var(--line-color);

	.shape {
		width: 45rpx;
		height: 45rpx;
		text-align: center;
		line-height: 45rpx;
		border-radius: 50%;
		font-weight: bold;
		color: #3F4356;
		position: relative;

	}

	.shape>div {
		position: absolute;
	}

	.shape>.bottom {
		width: 36rpx;
		height: 36rpx;
		border-radius: 50%;
		background-color: #3F4356;
		border: 4rpx solid #3F4356;
		top: 4rpx;
		left: 4rpx;
	}

	.shape>.cover {
		border-radius: 50%;
		width: 38rpx;
		height: 38rpx;
		background-color: #161A26;
		z-index: 10;
		top: 7rpx;
		left: 7rpx;
		color: #000;
		line-height: 39rpx;
		font-size: 18rpx;
		display: flex;
		align-items: center;
		justify-content: center;
	}

	.shape>.ring-left {
		width: 20rpx;
		height: 42rpx;
		border-radius: 21rpx 0 0 20rpx;
		background-color: $line-color;
		-webkit-transform-origin: right center;
		transform-origin: right center;
		-webkit-transform: rotate(0deg);
		transform: rotate(0deg);
		bottom: -4rpx;
		left: 4rpx;
	}

	.shape>.ring-right {
		width: 20rpx;
		height: 40rpx;
		border-radius: 0 20rpx 20rpx 0;
		background-color: $line-color;
		/* left: 50px; */
		-webkit-transform-origin: left center;
		transform-origin: left center;
		-webkit-transform: rotate(0deg);
		transform: rotate(0deg);
		right: -4rpx;
		top: 4rpx;
	}

	.shape>.ring-hide {
		    z-index: 9;
		    border-radius: 0 40rpx 40rpx 0;
		    width: 40rpx;
		    height: 80rpx;
		    background-color: #161A26;
		    top: 2rpx;
		    left: 22rpx;
		    -webkit-transform-origin: left center;
		    transform-origin: left center;
		    display: none;
		    -webkit-transform: rotate(0deg);
		    transform: rotate(0deg);
	}

	.shape>.ring-hide>div {
		z-index: 10;
		border-radius: 0 20rpx 20rpx 0;
		width: 22rpx;
		height: 38rpx;
		background-color: transparent;
		top: 1rpx;
		left: -1rpx;
		color: #000;
		line-height: 38rpx;
		position: absolute;
		border: 2rpx solid #3F4356;
		border-left: 0;
	}
</style>