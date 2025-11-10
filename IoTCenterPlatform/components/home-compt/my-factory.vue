<template>
	<view class="inventory_statistic_con" style="padding-bottom: 0;">
		<view class="inventory_statistic">
			<view class="icon_title">
				<view class="icon_title_left">
					<custom-icons iconsName="icon-wodegongchang" iconsSize="28rpx"></custom-icons>
					<view class="left_text">Factory</view>
				</view>
			</view>
			<view class="inventory_count">
				<view class="count_li agent_total_con" v-if="totalSattus!='loading'">
					<view class="li_con">
						<view class="label">Agents</view>
						<view class="value total">{{totalCount}}</view>
					</view>
					<view class="total_right" @click.stop="toAgents">
						<view class="right_text">Detail</view>
						<custom-icons iconsName="icon-a-youjiantoubai" iconsSize="20rpx"
							iconsColor="rgba(255, 255, 255, 0.5)"></custom-icons>
					</view>
				</view>
				<view class="count_li skeleton_con" v-if="totalSattus=='loading'">
					
				</view>
			</view>
			<view class="inventory_classify">
				<view class="classify_li" v-for="row in inventoryClassifyList" @click="jumpPages(row)">
					<view class="li_left">
						<view class="icons">
							<custom-icons :iconsName="row.iconsName" iconsSize="42rpx"></custom-icons>
						</view>
						<view class="text">
							<text>{{row.text}}</text>
						</view>
					</view>
					<view class="li_right">
						<custom-icons iconsName="icon-a-youjiantoubai" iconsSize="20rpx"></custom-icons>
					</view>
				</view>
			</view>
		</view>
		<msg-prompt ref="promptMsg"></msg-prompt>
	</view>
</template>

<script>
	import {
		factorygetAgent
	} from "@/api/factory";
	export default {
		name: "myFactory", //搜索主键
		data() {
			return {
				inventoryClassifyList: [{ //出入库分类
					iconsName: 'icon-haocaiguanli',
					text: 'Consumable\nmanagement',
					name: 'consumable_management'
				}, {
					iconsName: 'icon-haocaifenlei',
					text: 'Consumable\ncategory',
					name: 'consumable_category'
				}],
				totalCount: 0,
				totalSattus:'loading'
			};
		},
		mounted() {
			this.getFactorygetAgent()

		},
		methods: {
			toAgents() {
				//跳转至库存查询
				uni.navigateTo({
					url: '/pages_factory/agents_list'
				})
			},
			jumpPages(item) {
				if (item.name === 'consumable_management') {
					uni.navigateTo({
						url: '/pages_factory/consumable_management'
					})
				}
				if (item.name === 'consumable_category') {
					uni.navigateTo({
						url: '/pages_factory/consumable_category'
					})
				}
			},
			getFactorygetAgent() {
				//获取代理商总数
				this.totalSattus='loading'
				factorygetAgent({
					pageNum: 1,
					pageSize: 10
				}).then((res) => {
					// console.log(res, "代理商");
					let data = res.data;
					this.totalCount=data.Total
					this.totalSattus='nomore'
				}).catch(err=>{
					this.setMsgTop(err)
				});
			},
		}
	}
</script>

<style lang="less" scoped>
	.inventory_statistic_con {
		width: 100%;
		padding: 20rpx;
		box-sizing: border-box;

		.inventory_statistic {
			width: 100%;
			padding: 30rpx 30rpx 0;
			box-sizing: border-box;
			background-color: rgba(28, 34, 50, 1);

			.inventory_count {
				display: flex;
				justify-content: flex-start;
				align-items: center;

				.count_li {
					display: flex;
					justify-content: flex-start;
					align-items: center;
					font-size: 24rpx;
					line-height: 24rpx;
					padding: 32rpx 0;
					height: 124rpx;
					box-sizing: border-box;
					&.skeleton_con{
						width: 100%;
						height: 64rpx;
						background-color: rgba(63, 67, 86, 1);
						border-radius: 5rpx;
						margin-top: 30rpx;
						margin-bottom: 29rpx;
					}

					.label {
						color: rgba(255, 255, 255, 0.5);
						height: 24rpx;
						margin-bottom: 12rpx;
					}

					.value {
						color: rgba(255, 255, 255, 1);
						height: 24rpx;

						&.total {
							font-size: 44rpx;
							height: 44rpx;
							line-height: 44rpx;
						}
					}

					.li_con {
						display: flex;
						justify-content: flex-start;
						align-items: flex-end;
					}

					.count_li_left {
						margin-right: 16rpx;
					}

					&.total_con {
						width: 222rpx;

						.label {
							padding-bottom: 4rpx;
							margin-right: 10rpx;
							margin-bottom: 0;
						}
					}

					&.agent_total_con {
						width: 100%;
						justify-content: space-between;

						.label {
							padding-bottom: 4rpx;
							margin-right: 10rpx;
							margin-bottom: 0;
						}

						.total_right {
							color: rgba(255, 255, 255, 0.5);
							display: flex;
							justify-content: flex-end;
							align-items: center;

							.right_text {
								margin-right: 10rpx;
							}
						}
					}

					&.device_con {
						width: 198rpx;
						padding-left: 20rpx;
					}

					&.parts_con {
						width: 230rpx;
						padding-left: 20rpx;
					}
				}
			}

			.inventory_classify {
				display: flex;
				justify-content: flex-start;
				align-items: flex-start;
				flex-wrap: wrap;
				width: 100%;

				.classify_li {
					width: 50%;
					display: flex;
					justify-content: space-between;
					align-items: center;
					color: #ffffff;
					box-sizing: border-box;
					border-top: 1rpx solid rgba(255, 255, 255, 0.2);
					height: 123rpx;

					.li_left {
						display: flex;
						justify-content: flex-start;
						align-items: center;

						.text {
							// white-space: pre-wrap;
							margin-left: 20rpx;
							font-size: 28rpx;
							line-height: 34rpx;
						}
					}
				}

				.classify_li:nth-child(2n-1) {
					border-right: 1rpx solid rgba(255, 255, 255, 0.2);
					padding-left: 3rpx;
					padding-right: 30rpx;
				}

				.classify_li:nth-child(2n) {
					padding-left: 33rpx;
				}
			}
		}
	}
</style>