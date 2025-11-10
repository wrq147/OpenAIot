<template>
	<view :class="{'fixclass':isShowOutMethod}">
		<top :title="topTitle" leftText="Back" leftWidth="120rpx" rightWidth="120rpx" :isleftBack="true"
			leftIcon="icon-fanhui" backgroundColor="#161A26" :rightIcon="isShowOutMethod?'icon-crmtianjiaguanbi':'icon-tianjia'" @clickRight="wareRecordsAdd" :isShowAbs="isShowOutMethod" :absList="absList" @comfirmClick="comfirmClick">
		</top>
		<search-compt @openSelect="openSelect" @searching="searching"
			pal="Enter the Outbound tracking number or item name"></search-compt>
		<select-compt ref="selectCompt" :selectListParam="selectListParam" :querydata="querydata"
			@selectFinsh="selectFinsh" :haidate="true" :timeQuery="timeQuery" :hasSearch='true' :searchQuery="searchQuery" fixedHeight="216rpx"></select-compt>
		<v-tabs v-model="current" :scroll="true" :tabs="tabArr" color="rgba(255, 255, 255, 0.5)"
			activeColor="rgba(255, 255, 255, 1)" :fixed="true" @change="changeTab" :fixedLineWid="true" fontSize="28rpx"
			activeFontSize="28rpx" paddingItem="0 20rpx" lineHeight="4rpx" :hasBorder="false" height="72rpx"
			padding="0 0 0 0 " bgColor="rgba(22, 26, 38, 1)" lineColor="rgba(255, 255, 255, 1)" :zIndex="1"
			scrollBgColor="rgba(22, 26, 38, 1)" scrollConWid="100%"></v-tabs>
		<view class="records_list">
			<view class="records_li" v-for="(item,index) in tbList" @click="jumpPages(item)">
				<view class="li_title">
					<view class="tit_text">Warehousing items</view>
					<view class="pro_tips" v-if="item.List&&item.List.length>0">
						<view class="text">
							All {{item.List.length}} items
						</view>
						<custom-icons iconsName="icon-jinru" iconsSize="14rpx"
							iconsColor="rgba(255, 255, 255, 0.5)"></custom-icons>
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
								Price: ￥{{item.List[0].Price}}
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
		<view class="mask_con4" :style="{'height': statusBarHeight,'width':'100%' }" v-if="isShowOutMethod"></view>
		<view class="mask_con" v-if="isShowOutMethod"></view>
		<view class="mask_con2" v-if="isShowOutMethod" :style="{'top':statusBarHeight}"></view>
		<view class="mask_con3" v-if="isShowOutMethod" :style="{'top':statusBarHeight}"></view>
		<msg-prompt ref="promptMsg"></msg-prompt>
	</view>
</template>

<script>
	import {
		houseList,
	} from "@/api/house.js";
	import { leaveList, cancelLeave } from "@/api/stock";
	export default {
		data() {
			return {
				statusBarHeight: uni.getSystemInfoSync().statusBarHeight*2 + 'rpx',
				absList:[{
					name:'Outbound',
					value:0
				},{
					name:'Return',
					value:1
				},{
					name:'Allocated',
					value:2
				}],
				isShowOutMethod:false,//是否显示添加出库单的弹窗
				searchQuery:{
					pal:'Please enter the target company',
					name:'Target Enterprise',
					key:'',
					params:'ToCompany'
				},
				topTitle: 'Outbound records',
				querydata: {
					pageNum: 1,
					pageSize: 10,
					ShowItems:true
				},
				tbList: [],
				status: 'loading',
				tabArr: ['All', 'Pending submission', 'Pending approval', 'Successful', 'Failed', 'Canceled'],
				current: 0,
				activeName: '',
				selectListParam: [{
					name: 'Outbound method',
					params: 'LeaveMethod',
					pal: 'Please select a Outbound method',
					value: null,
					localdata: [{
						text: 'Outbound',
						value: '0'
					}, {
						text: 'return of goods',
						value: '1'
					}, {
						text: 'allocate and transfer',
						value: '2'
					}]
				},{
					name: 'Current warehouse',
					params: 'FromHouseId',
					pal: 'Please select a warehouse',
					value: null,
					localdata: []
				}],
				timeQuery: {
					name: 'Outbound date',
					params: ['beginTime', 'endTime'],
					value: [],
				},
			};
		},
		onLoad() {
			this.activeName = '-1'
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
				console.log("单行数据详情",row);
				if(row.Status == 0){
					if(row.LeaveMethod==1){
						uni.navigateTo({
							url:'/pages_Inventory/partial_returns?id='+row.Id
						})
					}else{
						uni.navigateTo({
							url:'/pages_Inventory/outbound_records_add?id='+row.Id
						})
					}
					
				}else{
					uni.navigateTo({
						url:'/pages_Inventory/outbound_records_detail?id='+row.Id
					})
				}
				
			},
			getSatusText(row) {
				switch (row.Status) {
					case 0:
						let obj = {
							text: 'Pending submission',
							cls: 'pend_sbmit'
						}
						return obj
					case 1:
						let obj1 = {
							text: 'Pending approval',
							cls: 'pend_approval'
						}
						return obj1
					case 2:
						let obj2 = {
							text: 'Successful',
							cls: 'success'
						}
						return obj2
					case 3:
						let obj3 = {
							text: 'Failed',
							cls: 'be_return'
						}
						return obj3
					case 4:
						let obj4 = {
							text: 'Canceled',
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
			getList() {
				this.querydata['Status'] = this.activeName;
				leaveList(this.querydata).then(response => {
					console.log("出库记录", response);
					this.topTitle='Outbound records('+response.data.Total+')'
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
				this.isShowOutMethod=!this.isShowOutMethod
			},
			comfirmClick(row){
				console.log("点击的添加类型",row);
				this.isShowOutMethod=false
				if(row.value==1){
					uni.navigateTo({
						url:'/pages_Inventory/partial_returns?LeaveMethod='+row.value
					})
				}else{
					uni.navigateTo({
						url:'/pages_Inventory/outbound_records_add?LeaveMethod='+row.value
					})
				}
			},
			getHouseList() {
				//获取仓库列表
				houseList({
					showAll: true
				}).then(res => {
					console.log("res", res);
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
				console.log('current', e);
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
	.mask_con{
		height: 100vh;
		width: 100%;
		position: fixed;
		top: 0;
		left: 0;
		background-color: rgba(0,0,0, 0.5);
		z-index: 995;
	}
	.mask_con2{
		width: calc(100% - 60rpx);
		height: 88rpx;
		position: fixed;
		top: 0;
		left: 0;
		background-color: rgba(0,0,0, 0.5);
		z-index: 999;
	}
	.mask_con3{
		width: 20rpx;
		height: 88rpx;
		position: fixed;
		top: 0;
		right: 0;
		background-color: rgba(0,0,0, 0.5);
		z-index: 999;
	}
	.mask_con4{
		width: 100%;
		position: fixed;
		top: 0;
		right: 0;
		background-color: rgba(0,0,0, 0.5);
		z-index: 999;
	}
	.fixclass{
		width: 100%;
		height: 100vh;
		overflow: hidden;
	}
</style>