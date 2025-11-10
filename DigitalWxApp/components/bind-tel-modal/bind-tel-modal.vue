<template>
	<view>
		<uni-popup ref="telpup" background-color="rgba(153,153,153,0)">
			<view class="telWrap">
				<view class="title3">您的账号还未绑定手机</view>
				<view class="desc">
					<view>使用我们的相关服务</view>
					<view style="margin-top: 10rpx;">需要将您的手机号授权给我们</view>
				</view>
				<view>
					<button class="telVerification" open-type="getPhoneNumber" @getphonenumber="getPhoneNumber">本机号码一键绑定</button>
					<button class="telCancel"  @click="cancelOperation">取消</button>
				</view>
				
			</view>
		</uni-popup>
	</view>
</template>

<script>
	import {getTel} from '@/api/tel.js'
	
	export default {
		name: "bind-tel-modal",
		data() {
			return {

			};
		},
		onReady() {
			this.$refs.telpup.open("center");//打开弹出层
		},
		methods: {
			async getPhoneNumber(e) {
				this.$refs.telpup.close();
				uni.showLoading({
					title: '加载中...'
				});
				try {
					
					// await getTel();
					
					if (e.detail.errMsg == 'getPhoneNumber:ok') {
						
						//获取访问后台得到的数据
						await getTel();
						
					}
				} catch (err) {
					console.info('异常：', err);
				} finally {
					uni.hideLoading();
				}


			},
			
			//取消按钮
			async cancelOperation(e) {
				this.$refs.telpup.close();
			}
		}
	}
</script>

<style lang="scss">
	
	
	.telWrap{
		border-radius: 40rpx;
		text-align: center;
		padding: 40rpx;
		background-color: #fff;
		font-size: 36rpx;
		
		.title3{
			margin: 60rpx 0;
			color: #232323;
			font-weight: 700;
		}
		.desc{
			color: #9F9F9F;
			font-size: 24rpx;
			margin-bottom: 40rpx;
			display:flex;
			flex-direction: column;
		}
		.telVerification{
			background-color: #35BBFB;
			width: 580rpx;
			line-height: 100rpx;
			border-radius: 60rpx;
			color: #fff;
		}
		.telCancel{
			line-height: 100rpx;
			color: #070000;
			background-color: #fff;
			margin-bottom: 60rpx;
			
		}
		
		
	}

</style>
