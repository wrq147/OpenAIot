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
				statusBarHeight: uni.getSystemInfoSync().statusBarHeight + 'px'
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
		// width: 750rpx;
		height: 20px;
		// height: var(--status-bar-height);
	}
</style>