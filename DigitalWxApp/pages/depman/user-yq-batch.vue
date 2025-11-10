<template>
	<view>
		<view class="topbox">
			<view class="box"></view>
			<view class="bps">
				<view class="bpswrap">
					<view class="yq-c">已成功邀请<text class="jiacu">{{yqcount}}</text>人</view>
				</view>
			</view>
		</view>

		<view style="margin-top: 260rpx;">
			<view class="yqm-tip">企业邀请码</view>
			<view class="yqm-txt">{{YQCode}}</view>
			<button class="btn" @click="copyYq">
				复制邀请码
			</button>
		</view>
	</view>
</template>

<script>
	import {
		getDeptInfo
	} from '@/api/dept'
	import {
		inviteOrg,
		parseYq
	} from '@/api/org'
	export default {
		data() {
			return {
				YQCode: "",
				yqdeptId: 0,
				yqcount: 0,
				orgId: 0
			};
		},
		async onLoad(options) {
			this.yqdeptId = parseInt(options.dp);

			try {
				let depRsp = await getDeptInfo(this.yqdeptId);
				this.orgId = depRsp.data.OrgId;

				let rsp = await inviteOrg({
					OrgId: this.orgId,
					DeptId: this.yqdeptId,
					Limit: false,
					YQCodeType: 0
				});
				this.YQCode = rsp.data;

			} catch (err) {
				console.info('异常：', err);
			}
			this.refreshCount();
		},
		methods: {
			//刷新邀请人数
			refreshCount() {
				parseYq(this.YQCode).then(rsp => {
					this.yqcount = rsp.data.YQCount;
				});
				setTimeout(this.refreshCount, 2000);
			},
			copyYq() {
				uni.setClipboardData({
					data: this.YQCode,
					success: function() {
						uni.showToast({
							icon: 'success',
							title: '复制成功'
						});
					}
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
		z-index: -9;

		.box {
			position: absolute;
			top: -90rpx;
			left: -15%;
			width: 130%;
			height: 500rpx;
			background-image: linear-gradient(270deg, #2CC2FB 0%, #50A6FA 100%);
			border-radius: 50%;
		}

		.bps {
			position: relative;

			.bpswrap {
				position: absolute;
				top: 180rpx;
				left: -90px;
				width: 380rpx;
				text-align: center;

				.yq-c {
					color: #ffffff;

					.jiacu {
						font-size: 42rpx;
						font-weight: bold;
					}
				}
			}
		}
	}

	.yqm-tip {
		display: flex;
		justify-content: center;
	}

	.yqm-txt {
		border-bottom: solid 1px #2CC2FB;
		font-size: 56rpx;
		font-weight: bolder;
		margin: 0 30rpx;
		padding-bottom: 16rpx;
		padding-top: 26rpx;
		text-align: center;
	}


	.btn {
		margin-top: 60rpx;
		margin-left: 60rpx;
		margin-right: 60rpx;
		color: #FFFFFF;
		background: linear-gradient(270deg, #2CC2FB 0%, #50A6FA 100%);
		border-radius: 8rpx;
	}
</style>
