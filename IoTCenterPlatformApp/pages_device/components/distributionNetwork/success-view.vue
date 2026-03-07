<template>
	<view>
		<view class="result-view">
			<!-- <icon type="success" size="108rpx" class="result-icon" color="#ddd"/> -->
			<view class="result-icon">
				<custom-icons iconsName="icon-chenggong" iconsSize="128rpx" iconsColor="#49D475"></custom-icons>
			</view>

			<view class="result-title">恭喜你，配网完成！</view>
			<view v-if="isShowAddTips">
				<view class="result_qus">
					是否需要添加该设备？
				</view>
				<view class="btn_con">
					<button class="submit_button border_btn" @click="onBottomButtonClick">
						不需要
					</button>
					<button class="submit_button" @click="successAddDevice">
						需要
					</button>
				</view>
			</view>
		</view>
		<btn-group :buttons="[{ btnText: '完成', type: 'primary', id: 'complete'}]"
			@onBottomButtonClick="onBottomButtonClick" :fixed-bottom="true" v-if="!isShowAddTips&&isShowAddTips!=null"/>
	</view>
</template>

<script>
	import btnGroup from '@/pages_device/components/distributionNetwork/btn-group.vue'
	import {
		InfoOfDtuId
	} from '@/api/device.js'
	export default {
		name: "success-view",
		components: {
			btnGroup,
		},
		props: {
			title: {
				type: String,
			},
			dtu: {
				type: String
			},
			ismode: {
				type: Boolean
			}
		},
		data() {
			return {
				isShowAddTips: null
			};
		},
		mounted() {
			this.getInfoOfDtuId()
		},
		methods: {
			getInfoOfDtuId() {
				//根据通讯id获取设备信息
				InfoOfDtuId({
					dtuId: this.dtu
				}).then(res => {
					// console.log("dtu", res, this.$store.state.user.orgId);

					if (this.$store.state.user.orgId == res.data.OrgId || this.$store.state.user.orgId == res.data
						.OwnerOrgId||this.$store.state.user.orgId == res.data.UseOrgId) {
						this.isShowAddTips = false
					} else {
						this.isShowAddTips = true
					}
				}).catch(err => {
					// console.log("错误",err);
					if (err.code > 9) {
						uni.showToast({
							title: err.cusMsg,
							icon:'none'
						})
					} else {
						if (err.message) {
							uni.showToast({
								title: err.message,
								icon:'none'
							})
						}
					}
				})
			},
			successAddDevice() {
				//配网成功，添加设备进列表
				this.$emit('successAddDevice')
			},
			onBottomButtonClick() {
				//配网成功，不添加设备，直接回首页

				this.$emit('connectSuccess')
			}
		}
	}
</script>

<style lang="less" scoped>
	:host {
		width: 100%;
	}

	.result-view {
		text-align: center;
	}

	.result-view .result-icon {
		margin-top: 348rpx;
		height: 120rpx;
	}

	.result-view .result-title {
		font-size: 32rpx;
		color: rgba(153, 153, 153, 1);
		margin-top: 40rpx;
		line-height: 1.4;
	}

	.result-view .result_qus {
		font-size: 36rpx;
		color: rgba(0, 0, 0, 1);
		line-height: 1.4;
		margin-top: 150rpx;
	}

	.result-view .btn_con {
		width: 100%;
		display: flex;
		justify-content: space-around;
		align-items: center;
		margin-top: 120rpx;

		.submit_button {
			width: 325rpx;

			&.border_btn {
				background: rgba(255, 255, 255, 1);
				border: 1rpx solid rgba(255, 53, 53, 1);
				color: rgba(255, 53, 53, 1);
			}
		}
	}

	.result-view .result-subtitle {
		font-size: 28rpx;
		color: #000;
		margin-top: 8rpx;
		line-height: 1.4;
	}

	.result-view .result-info-container {
		margin: 20px auto 0;
		font-size: 28rpx;
		color: #888;
		line-height: 1.7;
		max-width: 590rpx;
		display: flex;
		flex-direction: column;
		align-items: center;
	}

	.result-view .result-info-list {
		text-align: left;
	}
</style>