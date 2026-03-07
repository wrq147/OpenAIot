<template>
	<view class="pages_bgcon">
		<top :title="topTitle" leftWidth="157rpx" :isleftBack="true" leftIcon="icon-fanhui"
			rightWidth="157rpx" backgroundColor="rgba(245, 248, 249, 1)">
		</top>
		<uni-nav-bar :status-bar="false" :fixed="true" :border="false" :height="zhanweiHei"
			backgroundColor="rgba(245, 248, 249, 1)">
			<template v-slot:allslot>
				<view class="device_list_con details_top_con" id="detail_top" style="background-color: inherit;">
					<view class="device_list">
						<view class="list_li" style="background-color: inherit;">
							<view class="li_left">
								<image class="image" :src="inventoryInfo.PhotoUrl+'?wh=500x500'" mode="aspectFit">
								</image>
							</view>
							<view class="li_right">
								<view class="dev_name">
									{{inventoryInfo.Name}}
								</view>
								<view class="group">
									<view class="text">
										{{inventoryInfo.DeviceNumber}}
									</view>
									<view class="text">
										×{{inventoryInfo.Quantity}}
									</view>
									<view class="locked" v-if="inventoryInfo.LockQuantity>0">
										Locked
									</view>
									<view class="text" v-if="inventoryInfo.LockQuantity>0">
										×{{inventoryInfo.LockQuantity}}
									</view>
								</view>
								<view class="group">
									Price: ￥{{inventoryInfo.Price}}
								</view>
							</view>
						</view>
					</view>
				</view>
			</template>
		</uni-nav-bar>
		<view class="info_con">
			<view class="basic_info_list">
				<view class="basic_info_li" style="border-top: none;">
					<view class="li_label">存储类型</view>
					<view class="li_val">{{inventoryInfo.TargetType == 0 ? "耗材" : "设备"}}</view>
				</view>
				<view class="basic_info_li">
					<view class="li_label">所在仓库</view>
					<view class="li_val">{{inventoryInfo.StoreName}}</view>
				</view>
			</view>
			<view class="date_con_con">
				<view class="li_label">变更记录</view>
				<view class="int_date">
					<view class="date_con" style="color: #fff;">
						<uni-datetime-picker ref="dateChoice1" class="date" type="date"
							placeholder-style="font-size:28rpx;color:#999999" :clear-icon="true" v-model="choiceTime[0]"
							placeholder='开始日期' :isCustom="true" @change='dateChange1' :isDark="isDark">
							<view class="date_slot" :class="{'has_val':choiceTime[0]}">
								<view class="text">
									{{choiceTime[0]?choiceTime[0]:'开始日期'}}
								</view>
								<view class="icons_con t-icon-yichu1" @click.stop="clearDate1" v-if="choiceTime[0]">
								</view>
								<custom-icons iconsName="icon-xuanzeshijian" iconsSize="36rpx"
									iconsColor="rgba(153, 153, 153, 1)"></custom-icons>
							</view>
						</uni-datetime-picker>
					</view>
					<view class="row_line"></view>
					<view class="date_con">
						<uni-datetime-picker ref="dateChoice2" class="date" type="date" :clear-icon="true"
							v-model="choiceTime[1]" placeholder='结束日期' :isCustom="true" @change='dateChange2'
							:isDark="isDark">
							<view class="date_slot" :class="{'has_val':choiceTime[1]}">
								<view class="text">
									{{choiceTime[1]?choiceTime[1]:'结束日期'}}
								</view>
								<view class="icons_con t-icon-yichu1" @click.stop="clearDate2" v-if="choiceTime[1]">
								</view>
								<custom-icons iconsName="icon-xuanzeshijian" iconsSize="36rpx"
									iconsColor="rgba(153, 153, 153, 1)"></custom-icons>
							</view>
						</uni-datetime-picker>
					</view>
				</view>
			</view>
			<view class="timer_line">
				<view class="line_li_con" v-for="item in tbList">
					<view class="line_li">
						<view class="li_left">
							<view class="dot"></view>
							<!-- <view class="line_line"></view> -->
						</view>
						<view class="li_right">
							<view class="time_text">{{item.CreatedOn}}</view>
							<view class="li_content_con">
								<view class="li_content">
									<view class="content_title">
										<view class="name" v-if="item.FormType == 1 || item.FormType == 3">入库</view>
										<view class="name" v-if="item.FormType == 0 || item.FormType == 2">出库</view>
										<view class="num add" v-if="item.FormType == 1 || item.FormType == 3">
											+{{item.Quantity}}</view>
										<view class="num reduce" v-if="item.FormType == 0 || item.FormType == 2">
											-{{item.Quantity}}</view>
										<view class="box_li" style="margin-top: 0;"
											v-if="item.FormType == 1 || item.FormType == 3">(变更后:{{item.Remnant+item.LockRemnant}}+{{item.Quantity}}={{item.Remnant+item.LockRemnant+item.Quantity}})
										</view>
										<view class="box_li" style="margin-top: 0;"
											v-if="item.FormType == 0 || item.FormType == 2">(变更后:{{item.Remnant+item.LockRemnant}}-{{item.Quantity}}={{item.Remnant+item.LockRemnant-item.Quantity}})
										</view>
									</view>
									<view class="content_box">
										<view class="box_li">原价: {{item.StockPrice?'￥'+item.StockPrice:0}}
										</view>
										<view class="box_li">出入价: {{item.Price?'￥'+item.Price:0}}</view>
										<view class="box_li">变更后均价:
											{{getAveragePrice(item)?'￥'+getAveragePrice(item):0}}
										</view>
									</view>
								</view>
							</view>
						</view>
					</view>
				</view>
			</view>
			<uni-load-more iconType="circle" :status="status" v-if="status" />
		</view>
		<msg-prompt ref="promptMsg"></msg-prompt>
	</view>
</template>

<script>
	import {
		recordList
	} from "@/api/stock";
	var dayjs = require('@/common/day.js')
	export default {
		data() {
			return {
				topTitle: '变更记录',
				inventoryInfo: {},
				zhanweiHei: 0,
				isDark: false,
				choiceTime: [null, null],
				choiceTime2: [null, null],
				// 查询参数
				queryParams: {
					pageNum: 1,
					pageSize: 10,
				},
				tbList: [],
				status: 'loading'
			};
		},
		async onLoad(options) {
			if (options.info) {
				this.inventoryInfo = JSON.parse(options.info)
				this.queryParams.HouseId = this.inventoryInfo.HouseId;
				this.queryParams.TargetType = this.inventoryInfo.TargetType;
				this.queryParams.TargetId = this.inventoryInfo.TargetId;
				this.status = 'loading'
				await this.getList()
			}
			setTimeout(() => {
				this.getTopNavHei()
			},300)
		},
		onReachBottom() { //上拉触底
			// console.log("现在的状态是什么", this.status);
			if (this.status != 'noMore') {
				this.queryParams.pageNum++;
				this.status = "loading";
				this.getList();
				// console.log("现在是第几页", this.page);
			}
		},
		methods: {
			getAveragePrice(item) {
				//获取变更后均价
				if (item.FormType == 1 || item.FormType == 3) {
					let totalPrice = item.StockPrice * (item.Remnant + item.LockRemnant) + item.Price * item.Quantity
					let totalNum = item.Remnant + item.LockRemnant + item.Quantity
					if (totalNum == 0) {
						return 0
					}
					return totalPrice / totalNum
				} else if (item.FormType == 0 || item.FormType == 2) {
					let totalPrice = item.StockPrice * (item.Remnant + item.LockRemnant) - item.Price * item.Quantity
					let totalNum = item.Remnant + item.LockRemnant - item.Quantity
					if (totalNum == 0) {
						return 0
					}
					return totalPrice / totalNum
				}

			},
			async getList() {
				try {
					if (this.queryParams.pageNum == 1) {
						this.status = 'loading'
						this.tbList = []
					}
					let rsp = await recordList(this.addDateRange(this.queryParams, this.choiceTime2));
					this.tbList = [...this.tbList, ...rsp.data.List];
					if (rsp.data.List.length < this.queryParams.pageSize) {
						this.status = 'noMore'
					} else {
						this.status = 'more'
					}
				} catch (e) {
					//TODO handle the exception
					this.setMsgTop(e)
				}
			},
			async clearDate1() {
				this.$refs.dateChoice1.clear()
				this.choiceTime[0] = null
				this.choiceTime2[0] = null
				this.choiceTime2[1] = null
				this.queryParams.pageNum = 1
				await this.getList()
			},
			async clearDate2() {
				this.$refs.dateChoice2.clear()
				this.choiceTime[1] = null
				this.choiceTime2[1] = null
				this.choiceTime2[0] = null
				this.queryParams.pageNum = 1
				await this.getList()
			},
			async dateChange1(val) {
				//开始时间变化
				// console.log('数据11',val);
				if (this.choiceTime[1] && !this.choiceTime2[1]) {
					this.choiceTime2[1] = this.choiceTime[1]
				}
				if (this.choiceTime[0] && dayjs(val).isAfter(dayjs(this.choiceTime[1]))) {
					this.$refs.promptMsg.open('开始时间不能大于结束时间', 2000)
					this.choiceTime[0] = this.choiceTime2[0]
					return
				}
				this.choiceTime[0] = val
				this.choiceTime2[0] = val
				if (this.choiceTime[0] && this.choiceTime[1]) {
					// this.getDeviceOldInfo()
					this.queryParams.pageNum = 1
					await this.getList()
				}
			},
			async dateChange2(val) {
				//结束时间变化
				// console.log('数据22',val);
				if (this.choiceTime[0] && !this.choiceTime2[0]) {
					this.choiceTime2[0] = this.choiceTime[0]
				}
				if (this.choiceTime[0] && dayjs(this.choiceTime[0]).isAfter(dayjs(val))) {
					this.$refs.promptMsg.open('结束时间不能小于开始时间e', 2000)
					this.choiceTime[1] = this.choiceTime2[1]
					return
				}
				this.choiceTime[1] = val
				this.choiceTime2[1] = val
				if (this.choiceTime[0] && this.choiceTime[1]) {
					// this.getDeviceOldInfo()
					this.queryParams.pageNum = 1
					await this.getList()
				}
			},
			getTopNavHei() {
				//获取页面头部导航的高度
				// console.log("获取高度");
				const query = uni.createSelectorQuery().in(this);
				query.select('#detail_top').boundingClientRect(data => {
					// console.log("得到布局位置信息", data, data.height - 10);
					data.height = data.height - 10
					this.zhanweiHei = data.height * 2 + 'rpx'
					this.zhanweiHeiNumber = data.height * 2
					// console.log("this.zhanweiHei", this.zhanweiHei, this.zhanweiHeiNumber);
					this.$forceUpdate()
				}).exec();

			},
		}
	}
</script>

<style lang="less" scoped>
	.details_top_con {
		// padding: 0 20rpx;
		width: 100%;
		box-sizing: border-box;
		padding-top: 10rpx;
		// background-color: #161A26;

		.device_list {
			.list_li {
				// background-color: #161A26;
				margin-top: 0;

				.li_left {
					width: 150rpx;
					height: 150rpx;

					.image {
						width: 150rpx;
						height: 150rpx;
					}
				}

				.li_right {
					.dev_name {
						font-size: 32rpx;
					}
				}
			}
		}
	}

	.info_con {
		width: 100%;
		padding: 0 20rpx;
		box-sizing: border-box;
		padding-bottom: 50rpx;

		.basic_info_list {
			// margin-top: 0;
		}
	}

	.date_con_con {
		width: 100%;
		display: flex;
		justify-content: center;
		align-items: center;
		flex-direction: column;
		padding-top: 40rpx;
		padding-bottom: 40rpx;
		background-color: rgba(245, 248, 249, 1);

		.li_label {
			color: rgba(153, 153, 153, 1);
			font-size: 32rpx;
			line-height: 32rpx;
			margin-bottom: 30rpx;
			width: 100%;
			text-align: left;
			padding-left: 20rpx;
			box-sizing: border-box;
		}
	}

	.timer_line {
		width: 100%;
		// padding: 0 20rpx;
		padding-left: 6rpx;
		box-sizing: border-box;

		.line_li_con {
			position: relative;
			padding-top: 6rpx;
			width: 100%;

			.line_li {
				display: flex;
				justify-content: flex-start;
				align-items: flex-start;
				color: rgba(153, 153, 153, 1);
				border-left: 1rpx solid rgba(234, 234, 234, 1);
				font-size: 28rpx;
				width: 100%;

				.li_left {
					position: absolute;
					left: -5.5rpx;
					top: 6rpx;
					height: 100%;

					.dot {
						width: 12rpx;
						height: 12rpx;
						background-color: rgba(35, 113, 255, 1);
						border-radius: 50%;
					}
				}

				.li_right {
					margin-left: 25rpx;
					margin-top: -10rpx;
					width: calc(100% - 25rpx);

					.time_text {
						width: 100%;
						line-height: 28rpx;
						height: 28rpx;
					}

					.li_content_con {
						width: 100%;
						padding: 20rpx 0 40rpx;
					}

					.li_content {
						width: 100%;
						box-sizing: border-box;
						padding: 24rpx; //18=24-6
						background-color: rgba(255, 255, 255, 1);
						border-radius: 10rpx;

						.content_title {
							display: flex;
							justify-content: flex-start;
							align-items: center;
							line-height: 32rpx;

							.name {
								font-size: 28rpx;
								color: rgba(51, 51, 51, 1);
								margin-right: 20rpx;
							}

							.num {
								margin-right: 10rpx;

								&.reduce {
									color: rgba(255, 53, 53, 1);
								}

								&.add {
									color: rgba(239, 169, 2, 1);
								}
							}
						}

						.content_box {
							display: flex;
							justify-content: flex-start;
							align-items: center;
							flex-wrap: wrap;
						}

						.box_li {
							width: 50%;
							height: 26rpx;
							line-height: 26rpx;
							font-size: 24rpx;
							margin-top: 20rpx;
							white-space: nowrap;
						}
					}
				}
			}
		}

		.line_li_con:last-child .line_li {
			border-left: none;

			.li_right {
				.li_content_con {
					padding-bottom: 0;
				}
			}

		}
	}
</style>