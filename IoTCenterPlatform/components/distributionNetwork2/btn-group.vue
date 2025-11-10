<template>
	<view :class="{'no-padding':noPadding, 'flex':flex ,'fixed-bottom' : fixedBottom,'ipx' : ipx}" class="btn-group">
		<button v-for="(item,index) in buttons" :key="index" :disabled="item.disabled" class="btn need-hover"
			hover-start-time="20" hover-stay-time="70" hover-class="hover" @tap="onClickBtn" :data-index="index"
			:data-btn="item" :loading="loading" :class="item.type">
			<image v-if="item.icon" :src="item.icon" class="btn-icon" />
			<view v-else>{{item.btnText}}</view>
		</button>
	</view>
</template>

<script>
	export default {
		props: {
			buttons: {
				type: Array,
				value: [],
			},
			noPadding: {
				type: Boolean,
				value: false,
			},
			flex: {
				type: Boolean,
				value: false,
			},
			fixedBottom: {
				type: Boolean,
				value: false,
			},
			loading: {
				type: Boolean,
				value: false,
			},
		},
		name: "btnGroup",
		data() {
			return {
				ipx: false,
				globalData: {}
			};
		},
		mounted() {
			const systemInfo = uni.getSystemInfoSync();
			this.globalData = {
				isIpx: (systemInfo.screenHeight / systemInfo.screenWidth) > 1.86,
				isAndroid: systemInfo.platform.toLowerCase().indexOf('android') > -1,
				isIOS: systemInfo.platform.toLowerCase().indexOf('ios') > -1,
			};
		},
		methods: {
			onClickBtn(e) {
				let obj = {
					index: e.currentTarget.dataset.index,
					btn: e.currentTarget.dataset.btn,
				}
				// console.log("组件内按钮信息",obj);
				this.$emit('onBottomButtonClick', obj)
			},
		}
	}
</script>

<style lang="less" scoped>
	.btn-group {
		width: 100%;
		padding: 20rpx;
		box-sizing: border-box;
		background-color: #fff;

		.btn {
			margin-bottom: 20rpx;
			display: flex;
			justify-content: center;
			align-items: center;
			height: 92rpx;

			&.primary {
				background: linear-gradient(180deg, #FF3535 0%, #FF613D 100%);
				color: #fff;
			}
		}
	}

	.fixed-bottom {
		position: fixed;
		bottom: env(safe-area-inset-bottom);
		left: 0;
	}

	.noPadding {
		padding: 0;
	}
</style>