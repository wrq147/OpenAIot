<template>
	<view class="pages_bgcon">
		<top :title="topTitle" leftWidth="157rpx" :isleftBack="true" leftIcon="icon-fanhui"
			backgroundColor="rgba(255, 255, 255, 1)" rightWidth="157rpx">
		</top>
		<search-compt @openSelect="openSelect" @searching="searching" pal="请输入产品名称或唯一编号" :fixed="true"
			backgroundColor="#fff" inputBg="#F8F8F8"></search-compt>
		<select-compt ref="selectCompt" :selectListParam="selectListParam" :querydata="querydata"
			@selectFinsh="selectFinsh"></select-compt>

		<v-tabs v-model="current" :scroll="false" :tabs="tabArr" color="rgba(153, 153, 153, 1)" activeColor="#333333"
			:fixed="true" @change="changeTab" :fixedLineWid="true" fontSize="28rpx" activeFontSize="28rpx"
			paddingItem="0" lineHeight="4rpx" :hasBorder="false" height="72rpx" padding="0 0 0 0 "
			bgColor="rgba(255, 255, 255, 1)" lineColor="rgba(35, 113, 255, 1)" :zIndex="1"
			scrollBgColor="rgba(255, 255, 255, 1)" scrollConWid="100%"></v-tabs>

		<view class="device_list_con">
			<!-- <view class="netWork_con" @click="onScanWifiConfig">
				<custom-icons iconsName="icon-saoma" iconsSize="30rpx"></custom-icons>
				<view class="text">Distribution Network</view>
			</view> -->
			<view class="device_list">
				<view class="list_li" @click="toStockDetail(row)" v-for="row in stockTableData">
					<view class="li_left">
						<image class="image" :src="row.PhotoUrl+'?wh=500x500'" mode="aspectFit"></image>
					</view>
					<view class="li_right">
						<view class="dev_name">
							{{row.Name}}
						</view>
						<view class="group">
							<view class="text">
								{{row.DeviceNumber}}
							</view>
							<view class="text">
								×{{row.Quantity}}
							</view>
							<view class="locked" v-if="row.LockQuantity>0">
								锁定
							</view>
							<view class="text" v-if="row.LockQuantity>0">
								×{{row.LockQuantity}}
							</view>
						</view>
						<view class="group">
							价格:￥{{row.Price}}
						</view>
					</view>
					<view class="select_icon t-icon-xuanzhongshebeihaocai1"
						v-if="selectId&&selectId.includes(row.TargetId)"></view>
				</view>
				<uni-load-more iconType="circle" :status="status" v-if="status" />
			</view>
			<view v-if="isMulSelect">
				<view class="btn_zhanwei" style="width: 100%;height: 98rpx;"></view>
				<button class="submit_button" @click="confirmSelectItems">
					确定({{selectId.length}})
				</button>
			</view>
		</view>
		<msg-prompt ref="promptMsg"></msg-prompt>
	</view>
</template>

<script>
	import {
		houseList,
	} from "@/api/house.js";
	import {
		stockList
	} from '@/api/stock.js'
	import {
		setPagesParam
	} from '@/common/utillib.js'
	export default {
		data() {
			return {
				current: 0,
				tabArr: ['全部', '成品', '半成品'],
				topTitle: '库存查询',
				status: 'loading',
				querydata: {
					pageNum: 1,
					pageSize: 10,
					// TargetType: undefined,
					// HouseId: undefined,
					// Key: undefined
				},
				selectListParam: [{
					name: '仓库',
					params: 'HouseId',
					pal: '请选择仓库',
					value: null,
					localdata: []
				}],
				stockTableData: [],
				key: '',
				selectItems: [],
				selectId: [],
				isSelect: false,
				isMulSelect: false,
				status: 'loading'
			};
		},
		onLoad(options) {
			//
			this.getHouseList()
			this.$nextTick(() => {
				this.getStockList()
			})
			if (options.HouseId) {
				this.querydata.HouseId = options.HouseId
			}
			if(options.type){
				if(options.type==-1){
					this.current=0
				}else if(options.type==1){
					this.current=1
				}else if(options.type==2){
					this.current=2
				}
			}
			if (options.isSelect) {
				this.isSelect = !!options.isSelect
				if (options.selectId) {
					this.selectItems = JSON.parse(options.selectId)
					this.selectId = this.selectItems.map(item => {
						if (item.id) {
							return item.id
						} else if (item.TargetId) {
							return item.TargetId
						}
					})
				}
				if (options.isMulSelect) {
					this.isMulSelect = !!options.isMulSelect
				}
			}
		},
		onReachBottom() { //上拉触底
			// console.log("现在的状态是什么", this.status);
			if (this.status != 'noMore') {
				this.querydata.pageNum++;
				this.status = "loading";
				this.getStockList();
				// console.log("现在是第几页", this.page);
			}
		},
		methods: {
			confirmSelectItems() {
				//确定选择物品结果
				setPagesParam('finishSelectItems', this.selectItems, 1)
			},
			toStockDetail(row) {
				//跳转至库存详情
				if (!this.isSelect) {
					uni.navigateTo({
						url: '/pages_flow/inventory/inventory_record?info=' + JSON.stringify(row)
					})
				} else {
					if (this.selectId && this.selectId.includes(row.TargetId)) {
						let inxO = this.selectId.indexOf(row.TargetId)
						this.selectId.splice(inxO, 1)
						this.selectItems.splice(inxO, 1)
					} else {
						this.selectItems.push(row)
						this.selectId.push(row.TargetId)
					}
					if (this.isMulSelect) {} else {
						let arr = []
						arr.push(row)
						setPagesParam('finishSelectItems', arr, 1)
					}
				}

			},
			getStockList() {
				//获取库存列表
				if (this.querydata.pageNum == 1) {
					this.$refs.promptMsg.loadingOpen()
				}
				stockList(this.querydata).then(res => {
					// console.log("库存列表", res);
					if (this.querydata.pageNum == 1) {
						this.stockTableData = []
					}
					this.stockTableData = [...this.stockTableData, ...res.data.List]
					if (res.data.List.length < this.querydata.pageSize) {
						this.status = 'noMore';
					} else {
						this.status = 'more';
					}
					this.$refs.promptMsg.loadingColse()
				}).catch(err => {
					this.$refs.promptMsg.loadingColse()
					this.setMsgTop(err)
				})
			},
			changeTab(index) {
				//tab切换
				if (index == 0) {
					delete this.querydata.TargetType
				} else if (index == 1) {
					this.querydata.TargetType = "1"
				} else if (index == 2) {
					this.querydata.TargetType = "0"
				}
				this.querydata.pageNum = 1
				this.getStockList()
			},
			getHouseList() {
				//获取仓库列表
				houseList({
					showAll: true
				}).then(res => {
					this.selectListParam[0].localdata = []
					res.data.List.map(row => {
						let obj = {
							text: row.StoreName,
							value: row.Id
						}
						this.selectListParam[0].localdata.push(obj)
					})
				}).catch(err => {
					this.setMsgTop(err)
				})
			},
			searching(val) {
				//搜索关键字函数
				this.key = val
				if (this.key) {
					this.querydata.Key = this.key
					this.querydata.pageNum = 1
					this.status = 'loading'
					this.stockTableData = []
					this.getStockList()
				} else {
					delete this.querydata.Key
					this.querydata.pageNum = 1
					this.status = 'loading'
					this.stockTableData = []
					this.getStockList()
				}
			},
			openSelect() {
				//打开筛选弹窗
				this.$refs.selectCompt.openSelect()
			},
			selectFinsh(query) {
				//筛选完毕结果，筛选参数返回结果函数
				this.querydata = JSON.parse(JSON.stringify(query))
				this.querydata.pageNum = 1
				this.status = 'loading'
				this.stockTableData = []
				this.getStockList()
			}
		}
	}
</script>

<style lang="less">

</style>