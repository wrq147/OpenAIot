<template>
	<view>
		<image src="../static/wkt_mp.png"></image>
		<view class="btn">
			去开通数字名片
		</view>
	</view>
</template>

<script>
	export default {
		data() {
			return {

			}
		},
		methods: {

		}
	}
</script>

<style lang="scss">
	page {
		background-color: #FFFFFF;
		width: 100%;
		height: 100%;
		display: flex;
		flex-direction: column;
		justify-content: center;
		align-items: center;
		text-align: center;

		image {
			width: 480rpx;
			height: 494.27rpx;
		}

		.btn {
			width: 369rpx;
			height: 90rpx;
			line-height: 90rpx;
			border-radius: 8rpx;
			color: #fff;
			font-size: 30rpx;
			background-image: linear-gradient(270deg,
					rgba(44, 194, 251, 1) 0,
					rgba(80, 166, 250, 1) 100%);
			font-weight: 400;
			margin: 0 auto;
			margin-top: 100rpx;
		}

	}
</style>
