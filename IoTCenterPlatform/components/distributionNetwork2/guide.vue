<template>
	<view>

		<view class="page-subtitle">Configure devices</view>
		<view class="page-main">
			<image src="/static/config-device-img.jpg" class="config-device-img" />
			<view class="config-device-guide">
				<view class="config-device-guide-line">1、Please confirm that the device is powered on.</view>
				<view class="config-device-guide-line">2、Please confirm if the position positioning function is enabled.
				</view>
				<view class="config-device-guide-line">3、Please ensure that your phone is connected to the network.
				</view>
			</view>
			<view class="tip-txt" hover-class="checkActive" @click="jmptoErr" v-if="!isAgain&&!isNewDevice">Network distribution
				failure? Click here to view the solution!</view>
		</view>

		<btn-group :buttons="[{ btnText: 'Next', type: 'primary', id: 'complete'}]"
			@onBottomButtonClick="onBottomButtonClick" :fixed-bottom="true" />
	</view>
</template>

<script>
	import btnGroup from '@/components/distributionNetwork/btn-group.vue'
	export default {
		name: "guide",
		components: {
			btnGroup,
		},
		props: {
			isAgain: {
				type: Boolean,
				default: false,
			},
			isNewDevice:{
				type:Boolean,
				default:false,
			}
		},
		data() {
			return {


			};
		},
		methods: {
			jmptoErr() {
				this.$emit('onGuideComplete', 'fail');
			},
			onBottomButtonClick(val) {
				// console.log("组件返回的值",val);
				this.$emit('onGuideComplete', val)
			}
		}
	}
</script>

<style lang="less" scoped>
	:host {
		width: 100%;
	}

	.tip-txt {
		margin-top: 180rpx;
		font-size: 32rpx;
		cursor: pointer;
		text-decoration: underline;
		color: rgba(255, 53, 53, 1);
	}

	.config-device-img {
		width: 670rpx;
		height: 378rpx;
		border-radius: 8rpx;
	}

	.config-device-guide {
		margin-top: 50rpx;
		color: #333333;
		font-size: 28rpx;
		line-height: 1.5;
		word-break: break-all;
	}

	.config-device-guide-line {
		margin-bottom: 7px;
		// font-size:38rpx;

	}

	.config-device-guide-author {
		margin-top: 40px;
		color: #333333;
		font-size: 28rpx;
		line-height: 1.5;
		word-break: break-all;
	}
</style>