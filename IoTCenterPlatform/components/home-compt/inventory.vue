<template>
	<view class="inventory_statistic_con">
		<view class="inventory_statistic">
			<view class="icon_title">
				<view class="icon_title_left">
					<custom-icons iconsName="icon-kucun" iconsSize="28rpx"></custom-icons>
					<view class="left_text">Inventory</view>
				</view>
				<view class="icon_title_right" @click.stop="toInventory()">
					<view class="right_text">Detail</view>
					<custom-icons iconsName="icon-a-youjiantoubai" iconsSize="20rpx"
						iconsColor="rgba(255, 255, 255, 0.5)"></custom-icons>
				</view>
			</view>
			<view class="inventory_count">
				<view class="count_li total_con" v-if="totalStatus!='loading'">
					<view class="li_con">
						<view class="label">Total</view>
						<view class="value total">{{totalCount}}</view>
					</view>
				</view>
				<view class="count_li total_con skeleton_con" v-if="totalStatus=='loading'">
					<view class="skeleton_con_li"></view>
				</view>
				<view class="count_li device_con" v-if="totalStatus!='loading'">
					<view class="count_li_left">
						<l-circle v-model:current="DevCount" trailColor="#3F4356" :percent="DevCount"
							:strokeWidth="strokeWidth" :trailWidth="strokeWidth" size="45rpx" strokeColor="#FF3535" :max="totalCount?totalCount:100">
							<custom-icons iconsName="icon-a-shebei-xianxingxiao" iconsSize="20rpx"></custom-icons>
						</l-circle>
					</view>
					<view class="count_li_right">
						<view class="label">Machines</view>
						<view class="value">{{DevCount}}</view>
					</view>
				</view>
				<view class="count_li device_con skeleton_con" v-if="totalStatus=='loading'">
					<view class="skeleton_con_li"></view>
				</view>
				<view class="count_li parts_con" v-if="totalStatus!='loading'">
					<view class="count_li_left">
						<l-circle v-model:current="PartsCount" trailColor="#3F4356" :percent="PartsCount"
							:strokeWidth="strokeWidth" :trailWidth="strokeWidth" size="45rpx" strokeColor="#EFA902" :max="totalCount?totalCount:100">
							<custom-icons iconsName="icon-a-haocai-xianxingxiao" iconsSize="20rpx"></custom-icons>
						</l-circle>
					</view>
					<view class="count_li_right">
						<view class="label">Consumables</view>
						<view class="value">{{PartsCount}}</view>
					</view>
				</view>
				<view class="count_li parts_con skeleton_con" v-if="totalStatus=='loading'">
					<view class="skeleton_con_li"></view>
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
	import {StockStatisticsInfo} from '@/api/device.js'
	export default {
		name: "inventory", //搜索主键
		data() {
			return {
				trailColor: '#3F4356', //轨道环线颜色
				strokeColor: '#FF3535', //进度条颜色
				strokeWidth: '3rpx', //进度条宽度
				target: 30, //进度环目标值
				modelVale: 20, //进度环当前值//
				inventoryClassifyList: [{ //出入库分类
					iconsName: 'icon-cangkuguanli',
					text: 'Warehouse\nmanagement',
					name:'Warehouse_management'
				}, {
					iconsName: 'icon-pandianrenwu',
					text: 'Checking\ntasks',
					name:'Checking_tasks'
				}, {
					iconsName: 'icon-rukujilu',
					text: 'Warehousing\nrecords',
					name:'Warehousing_records'
				}, {
					iconsName: 'icon-chukujilu',
					text: 'Outbound \n records',
					name:'Outbound_records'
				}],
				totalCount:0,
				PartsCount:0,
				DevCount:0,
				totalStatus:'loading'
			};
		},
		mounted() {
			this.getStockStatisticsInfo()
		},
		methods: {
			toInventory(){
				//跳转至库存查询
				uni.navigateTo({
					url:'/pages_Inventory/inventory_list'
				})
			},
			jumpPages(item){
				if(item.name==='Warehouse_management'){
					uni.navigateTo({
						url:'/pages_Inventory/warehouse_management'
					})
				}
				if(item.name==='Warehousing_records'){
					uni.navigateTo({
						url:'/pages_Inventory/warehouse_records'
					})
				}
				if(item.name==='Outbound_records'){
					uni.navigateTo({
						url:'/pages_Inventory/outbound_records'
					})
				}
				if(item.name==='Checking_tasks'){
					uni.navigateTo({
						url:'/pages_Inventory/checking_tasks'
					})
				}
			},
			getStockStatisticsInfo() {
				//获取库存统计信息
				this.totalStatus='loading'
				StockStatisticsInfo().then((res) => {
					// console.log(res, "库存统计信息");
					let data = res.data;
					this.totalCount=data.TotalCount
					this.PartsCount=data.PartsCount
					this.DevCount=data.DevCount
					this.totalStatus='nomore'
				}).catch(err=>{
					this.setMsgTop(err)
				});
			},
			change(e) { //轮播页面发生变化
				this.current = e.detail.current;
			},
			clickItem(e) { //点击指示点
				this.swiperDotIndex = e
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

					&.device_con {
						width: 198rpx;
						padding-left: 20rpx;
					}

					&.parts_con {
						width: 230rpx;
						padding-left: 20rpx;
					}
					&.skeleton_con{
						.skeleton_con_li{
							width: 100%;
							height: 100%;
							background-color: rgba(63, 67, 86, 1);
							border-radius: 5rpx;
						}
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
							margin-left: 10rpx;
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