<template>
	<uni-nav-bar :status-bar="true" :fixed="true" :border="false" :showNavPla="false" height="72rpx" :zIndex="997"
		backgroundColor="rgba(255, 249, 234, 1)" v-if="showTips">
		<template v-slot:allslot>
			<view class="down_tips_con">
				<view class="tips_text">立即下载{{appName}}APP，使用更方便</view>
				<view class="down_con">
					<view class="down_btn" @click="jumpToDown">去下载</view>
					<view class="close_btn" @click="closeTips">
						<custom-icons iconsName="icon-guanbidanchuang" iconsSize="32rpx"
							iconsColor="rgba(240, 163, 0, 1)"></custom-icons>
					</view>
				</view>
			</view>
		</template>
	</uni-nav-bar>
</template>

<script>
	export default {
		name: "jump-down",
		data() {
			return {
				showTips:false,
			};
		},
		computed: {
			appName() {
				let text=this.$store.state.appNameText?this.$store.state.appNameText:''
				return text
			}
		},
		methods:{
			jumpToDown(){
				uni.navigateTo({
					url:'/page_register/down'
				})
			},
			closeTips(){
				//关闭下载提示
				this.showTips=false
			}
		}
	}
</script>

<style lang="scss" scoped>
	.down_tips_con {
		display: flex;
		justify-content: space-between;
		align-items: center;
		height: 72rpx;
		padding: 0 30rpx;
		box-sizing: border-box;

		.tips_text {
			font-size: 24rpx;
			color: rgba(240, 163, 0, 1);
			font-weight: bold;
		}

		.down_con {
			display: flex;
			justify-content: flex-end;
			align-items: center;

			.down_btn {
				width: 88rpx;
				height: 44rpx;
				color: rgba(240, 163, 0, 1);
				font-size: 20rpx;
				border: 1rpx solid rgba(240, 163, 0, 1);
				border-radius: 26rpx;
				text-align: center;
				margin-right: 36rpx;
				line-height: 44rpx;
			}
		}
	}
</style>