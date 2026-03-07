<template>
	<view :style="{ height: statusBarHeight,backgroundColor:themeBgColor }" class="uni-status-bar">
		<slot />
	</view>
</template>

<script>
	export default {
		name: 'UniStatusBar',
		props: {
			bgc: {
				type: String,
				default: ""
			}
		},
		data() {
			return {
				statusBarHeight: uni.getSystemInfoSync().statusBarHeight*2 + 'rpx'
			}
		},
		computed: {
			themeBgColor() {
				return this.bgc
			},
		}
	}
</script>

<style lang="scss">
	.uni-status-bar {
		width: 100%;
		height: 40rpx;
		// height: var(--status-bar-height);
	}
</style>