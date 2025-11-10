<template>
	<view class="pages_bgcon">
		<top :title="topTitle" leftWidth="120rpx" rightWidth="120rpx" :isleftBack="true"
			leftIcon="icon-fanhui" backgroundColor="#ffffff" rightIcon="icon-tianjia" @clickRight="wareRecordsAdd">
		</top>
		<search-compt @openSelect="openSelect" @searching="searching"
			pal="请输入入库单号或者来源企业名称" backgroundColor="#fff" inputBg="#F8F8F8"></search-compt>
		<select-compt ref="selectCompt" :selectListParam="selectListParam" :querydata="querydata"
			@selectFinsh="selectFinsh" :haidate="true" :timeQuery="timeQuery" fixedHeight="216rpx"></select-compt>
		<v-tabs v-model="current" :scroll="false" :tabs="tabArr" color="#999999"
			activeColor="#333333" :fixed="true" @change="changeTab" :fixedLineWid="true" fontSize="28rpx"
			activeFontSize="28rpx" paddingItem="0 20rpx" lineHeight="4rpx" :hasBorder="false" height="72rpx"
			padding="0 0 0 0 " bgColor="#ffffff" lineColor="#2371FF" :zIndex="1"
			scrollBgColor="#ffffff" scrollConWid="100%"></v-tabs>
		<view class="records_list">
			<view class="records_li" v-for="(item,index) in tbList" @click="jumpPages(item)">
				<view class="li_title">
					<view class="tit_text">入库物品</view>
					<view class="pro_tips" v-if="item.List&&item.List.length>0">
						<view class="text">
							共 {{item.List.length}} 件
						</view>
						<custom-icons iconsName="icon-jinru" iconsSize="14rpx"
							iconsColor="#999999"></custom-icons>
					</view>
				</view>
				<view class="li_pro" v-if="item.List&&item.List.length>0">
					<view class="pro_left">
						<image class="image" :src="item.List[0].PhotoUrl+'?wh=500x500'" mode=""></image>
					</view>
					<view class="pro_right" v-if="item.List&&item.List[0]">
						<view class="pro_title">{{item.List[0].TargetName}}</view>
						<view class="pro_num_price">
							<view class="text1">
								{{item.List[0].TargetNumber}}
								<text style="margin-left: 10rpx;" v-if="item.List[0].Quantity!=null">×{{item.List[0].Quantity}}</text>
							</view>
							<view class="text2" v-if="item.List[0].Price!=null">
								价格: ${{item.List[0].Price}}
							</view>
						</view>
					</view>
				</view>
				<view class="li_bottom">
					<view class="records_num">{{item.StockNumber}}</view>
					<view class="type_status" :class="getSatusText(item).cls" v-if="getSatusText(item)">{{getSatusText(item).text}}</view>
				</view>
			</view>
			<uni-load-more iconType="circle" :status="status" v-if="status" />
		</view>
		<msg-prompt ref="promptMsg"></msg-prompt>
	</view>
</template>

<script>
	import {
		houseList,
	} from "@/api/house.js";
	import {
		enterList
	} from '@/api/stock.js'
	import {
		setPagesParam
	} from '@/common/utillib.js'
	export default {
		data() {
			return {
				isSelect:false,//是否是选择
				topTitle: '入库记录',
				querydata: {
					pageNum: 1,
					pageSize: 10,
					ShowItems:true
				},
				tbList: [],
				status: 'loading',
				tabArr: ['全部', '待提交', '待审核', '已完成', '待退货', '已退货'],
				current: 0,
				activeName: '',
				selectListParam: [{
					name: '入库方式',
					params: 'EnterMethod',
					pal: '请选择入库方式',
					value: null,
					localdata: [{
						text: '出库',
						value: '0'
					}, {
						text: '退货',
						value: '1'
					}, {
						text: '调拨',
						value: '2'
					}, {
						text: '手动',
						value: '3'
					}]
				}, {
					name: '仓库',
					params: 'ToHouseId',
					pal: '请选择仓库',
					value: null,
					localdata: []
				}],
				timeQuery: {
					name: '入库时间',
					params: ['beginTime', 'endTime'],
					value: [],
				},
			};
		},
		onLoad(options) {
			this.activeName = '-1'
			if(options.isSelect){
				this.isSelect=true
			}
			this.getHouseList()
			this.getList()
		},
		onReachBottom() { //上拉触底
			if (this.status != 'noMore') {
				this.querydata.pageNum++;
				this.status = "loading";
				this.getList();
			}
		},
		methods: {
			jumpPages(row){
				// console.log("单行数据详情",row);
				if(this.isSelect){
					setPagesParam('finishChoicewWareRecords', row, 1)
				}else{
					if(row.Status == 0){
						uni.navigateTo({
							url:'/pages_flow/inventory/warehouse_records_add?id='+row.Id
						})
					}else{
						uni.navigateTo({
							url:'/pages_flow/inventory/warehouse_records_detail?id='+row.Id
						})
					}
				}
				
				
			},
			getSatusText(row) {//设置对应出库单样式
				switch (row.Status) {
					case 0:
						let obj = {
							text: '待提交',
							cls: 'pend_sbmit'
						}
						return obj
					case 1:
						let obj1 = {
							text: '待审核',
							cls: 'pend_approval'
						}
						return obj1
					case 2:
						let obj2 = {
							text: '已完成',
							cls: 'success'
						}
						return obj2
					case 3:
						let obj3 = {
							text: '待退货',
							cls: 'be_return'
						}
						return obj3
					case 4:
						let obj4 = {
							text: '已退货',
							cls: 'return'
						}
						return obj4
				}
			},
			loadData(query){
				this.querydata.pageNum=1
				this.status='loading'
				this.getList()
			},
			getList() {//获取出库列表
				this.querydata['Status'] = this.activeName;
				enterList(this.querydata).then(response => {
					// console.log("入库记录", response);
					if (this.querydata.pageNum == 1) {
						this.tbList = []
					}
					this.tbList = [...this.tbList, ...response.data.List];
					if (response.data.List.length < this.querydata.pageSize) {
						this.status = 'noMore';
					} else {
						this.status = 'more';
					}
					// this.total = response.data.Total;
				}).catch(err => {
					this.status = 'noMore';
					this.setMsgTop(err)
				})
			},
			wareRecordsAdd() {
				//入库记录添加
				uni.navigateTo({
					url:'/pages_flow/inventory/warehouse_records_add'
				})
			},
			getHouseList() {
				//获取仓库列表
				houseList({
					showAll: true
				}).then(res => {
					this.selectListParam[1].localdata = []
					res.data.List.map(row => {
						let obj = {
							text: row.StoreName,
							value: row.Id
						}
						this.selectListParam[1].localdata.push(obj)
					})
				}).catch(err => {
					this.setMsgTop(err)
				})
			},
			changeTab(e) {
				// console.log('current', e);
				//切换出库单列表
				this.current = e
				switch (e) {
					case 0:
						this.activeName = "-1";
						break
					case 1:
						this.activeName = "0";
						break
					case 2:
						this.activeName = "1";
						break
					case 3:
						this.activeName = "2";
						break
					case 4:
						this.activeName = "3";
						break
					case 5:
						this.activeName = "4";
						break
				}
				this.querydata.pageNum = 1
				this.status='loading'
				this.getList()
			},
			searching(val) {
				this.key = val
				if (this.key) {
					this.querydata.Key = this.key
					this.querydata.pageNum = 1
					this.status = 'loading'
					this.tbList = []
					this.getList()
				} else {
					delete this.querydata.Key
					this.querydata.pageNum = 1
					this.status = 'loading'
					this.tbList = []
					this.getList()
				}
			},
			openSelect() {
				this.$refs.selectCompt.openSelect()
			},
			selectFinsh(query) {
				this.querydata = JSON.parse(JSON.stringify(query))
				this.querydata.pageNum = 1
				this.status = 'loading'
				this.tbList = []
				this.getList()
			}
		}
	}
</script>

<style lang="less" scoped>
	
</style>