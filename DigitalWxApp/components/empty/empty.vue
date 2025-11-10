<template>
	<view class="emptywrap">
		<image class="emptyImg" mode="aspectFill" :src="imgsrc" />
		<text class="emptyTxt">{{txt}}</text>
	</view>
</template>

<script>
	export default {
		name: "empty",
		props: {
			imgsrc: {
				type: String,
				default: "",
			},
			txt: {
				type: String,
				default: "",
			}
		}
	}
</script>

<style lang="scss">
	.emptywrap {
		display: flex;
		flex-direction: column;
		align-items: center;
		padding-top: 60rpx;
		padding-bottom: 60rpx;

		.emptyImg {
			width: 170rpx;
			height: 170rpx;
		}

		.emptyTxt {
			color: #666666;
			line-height: 40px;
		}
	}
</style>
