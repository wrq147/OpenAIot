<template>
	<view style="background-color: #fff;">
		<template v-if="applylist.length>0">
			<view class="apply-item border-top" v-for="item in applylist">
				<view class="t">
					<fr-image class="ximg" :lazy-load="true" mode="aspectFill" :src="item.SendAvatar" />
					<view class="mid">
						<view class="np">
							<text class="rn">{{item.SendRealName}}</text><text class="pn">{{item.SendPostName}}</text>
						</view>
						<text class="cn">{{item.SendOrgName}}</text>
					</view>
					<view class="rig">
						<text v-if="item.Status==1" style="color: #50A6FA;margin-top: 14rpx;">已同意</text>
						<text v-else-if="item.Status==2" style="color: #999999;margin-top: 14rpx;">已忽略</text>
					</view>
				</view>
				<view class="b">
					<view class="triangle"></view>
					对方请求交换名片<text class="jiacu">【{{item.RecvOrgName}}｜{{item.RecvRealName}}】</text>
				</view>
				<view class="btnwrap" v-if="item.Status==0">
					<button class="refusebtn" @click="onRefuse(item)">忽略</button>
					<button class="agreebtn" @click="onAgree(item)">同意</button>
				</view>
			</view>
		</template>
		<empty v-else-if="status!='loading'" imgsrc="/static/empty/data_empty.png" txt="暂无数据"></empty>
		<view v-else style="padding-bottom: 10rpx;">
			<J-skeleton :loading="true" :showTitle="true" :row="2">
			</J-skeleton>
		</view>
		<uni-load-more v-if="applylist.length>0" iconType="circle" :showText="false" :status="status" />
	</view>
</template>

<script>
	import {
		getExchangeList,
		agreeExchange,
		refuseExchange
	} from '@/api/userCard.js'
	import {
		reloadPrePage
	} from '@/common/util.js'
	export default {
		data() {
			return {
				applylist: [],
				page: 1,
				status: 'loading',
				hashitems: {}
			};
		},
		async onLoad() {
			this.load_data();
		},
		onReachBottom: function() {
			if (this.status != 'noMore') {
				this.page++;
				this.status = "loading";
				this.load_data();
			}
		},
		methods: {
			async load_data() {
				let rsp = await getExchangeList({
					showAll: true,
					pageSize: 30,
					pageNum: this.page
				});

				rsp.data.List.forEach(it => {
					if (!this.hashitems[it.Id]) {
						this.hashitems[it.Id] = true;
						this.applylist.push(it);
					}
				});
				if (rsp.data.List.length < 30) {
					this.status = 'noMore';
				} else {
					this.status = 'more';
				}
			},
			async onAgree(item) {
				uni.showLoading({
					title: '加载中...'
				});
				try {
					let rsp = await agreeExchange(item.Id);
					item.Status = 1;
					reloadPrePage();
				} catch (err) {
					console.info('异常：', err);
				} finally {
					uni.hideLoading();
				}

			},
			async onRefuse(item) {
				uni.showLoading({
					title: '加载中...'
				});
				try {
					let rsp = await refuseExchange(item.Id);
					item.Status = 2;
					reloadPrePage();
				} catch (err) {
					console.info('异常：', err);
				} finally {
					uni.hideLoading();
				}


			}
		}
	}
</script>

<style lang="scss">
	.apply-item {
		background-color: #ffffff;
		padding: 20rpx 30rpx;

		.t {
			display: flex;

			.ximg {
				width: 98rpx;
				height: 98rpx;
				border-radius: 50%;
				overflow: hidden;
			}

			.mid {
				flex: 1;
				margin-left: 22rpx;

				.np {
					display: flex;
					align-items: center;
					margin-top: 10rpx;

					.rn {
						font-size: 30rpx;
						font-weight: bold;
						color: #333333;
					}

					.pn {
						margin-left: 20rpx;
						font-size: 24rpx;
						color: #999999;
					}
				}

				.cn {
					margin-top: 10rpx;
					font-size: 24rpx;
					color: #999999;
				}
			}

			.rig {
				width: 140rpx;
				margin-top: 14rpx;
				display: flex;
				justify-content: flex-end;
			}
		}

		.b {
			position: relative;
			background-color: #EFEFEF;
			border-radius: 100rpx;
			margin-top: 34rpx;
			padding: 23rpx 50rpx;
			font-size: 24rpx;
			color: #999999;
			line-height: 40rpx;

			.triangle {
				position: absolute;
				top: -30rpx;
				left: 50rpx;
				margin-left: -20rpx;
				width: 0;
				height: 0;
				border-style: solid;
				border-width: 20rpx;
				border-color: transparent transparent #EFEFEF transparent;
				font-size: 0;
				line-height: 0;
			}

			.jiacu {
				color: #333333;
			}
		}

		.btnwrap {
			display: flex;
			justify-content: space-between;
			margin-top: 20rpx;

			.refusebtn,
			.agreebtn {
				width: 240rpx;
				height: 66rpx;
				border-radius: 20rpx;
				display: flex;
				justify-content: center;
				align-items: center;
				font-size: 28rpx;
			}

			.agreebtn {
				background-color: #50A6FA;
				color: #fff;
			}

			.refusebtn {
				background-color: #EFEFEF;
				color: #868686;
			}
		}
	}
</style>
