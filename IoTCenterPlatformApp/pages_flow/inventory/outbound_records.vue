<template>
	<view class="pages_bgcon" :class="{'fixclass':isShowOutMethod}">
		<top :title="topTitle" leftWidth="120rpx" rightWidth="122rpx" :isleftBack="true"
			leftIcon="icon-fanhui" backgroundColor="#fff" :rightIcon="isShowOutMethod?'icon-crmtianjiaguanbi':'icon-tianjia'" :rightIconColor="isShowOutMethod?'#ffffff':''" @clickRight="wareRecordsAdd" :isShowAbs="isShowOutMethod" :absList="absList" @comfirmClick="comfirmClick">
		</top>
		<search-compt @openSelect="openSelect" @searching="searching"
			pal="请输入出库单号或目标企业" backgroundColor="#fff" inputBg="#F8F8F8"></search-compt>
		<select-compt ref="selectCompt" :selectListParam="selectListParam" :querydata="querydata"
			@selectFinsh="selectFinsh" :haidate="true" :timeQuery="timeQuery" :hasSearch='true' :searchQuery="searchQuery" fixedHeight="216rpx"></select-compt>
		<v-tabs v-model="current" :scroll="true" :tabs="tabArr" color="#999999"
			activeColor="#333333" :fixed="true" @change="changeTab" :fixedLineWid="true" fontSize="28rpx"
			activeFontSize="28rpx" paddingItem="0 20rpx" lineHeight="4rpx" :hasBorder="false" height="72rpx"
			padding="0 0 0 0 " bgColor="#ffffff" lineColor="#2371FF" :zIndex="1"
			scrollBgColor="#ffffff" scrollConWid="100%"></v-tabs>
		
		<view class="records_list">
			<view class="errlist_con" @click="jumpApplist">
				<view class="errli_left">
					<custom-icons iconsName="icon-chukujilu" iconsSize="28rpx"
						iconsColor="#333"></custom-icons>
					<view class="text">
						待出库申请单
					</view>
				</view>
				<view class="err_right">
					<custom-icons iconsName="icon-a-youjiantouhong" iconsSize="20rpx"
						iconsColor="#999999"></custom-icons>
				</view>
			</view>
			<view class="records_li" v-for="(item,index) in tbList" @click="jumpPages(item)">
				<view class="li_title">
					<view class="tit_text">出库物品</view>
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
								价格: ￥{{item.List[0].Price}}
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
					name:'新增出库单',
					value:0
				},{
					name:'新增退货单',
					value:1
				},{
					name:'新增调拨单',
					value:2
				}],
				isShowOutMethod:false,//是否显示添加出库单的弹窗
				searchQuery:{
					pal:'请输入企业名称',
					name:'目标企业',
					key:'',
					params:'ToCompany'
				},
				topTitle: '出库记录',
				querydata: {
					pageNum: 1,
					pageSize: 10,
					ShowItems:true
				},
				tbList: [],
				status: 'loading',
				tabArr: ['全部', '待提交', '待审核', '出库成功', '出库失败', '已取消'],
				current: 0,
				activeName: '',
				selectListParam: [{
					name: '出库方式',
					params: 'LeaveMethod',
					pal: '请选择出库方式',
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
						text: '领料',
						value: '3'
					}]
				},{
					name: '当前仓库',
					params: 'FromHouseId',
					pal: '请选择仓库',
					value: null,
					localdata: []
				}],
				timeQuery: {
					name: '出库时间',
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
			jumpApplist(){
				uni.navigateTo({
					url: '/pages_flow/inventory/applylist?isout=true'
				})
			},
			jumpPages(row){
				if(row.Status == 0){
					uni.navigateTo({
						url:'/pages_flow/inventory/outbound_records_add?id='+row.Id
					})
				}else{
					uni.navigateTo({
						url:'/pages_flow/inventory/outbound_records_detail?id='+row.Id
					})
				}
				
			},
			getSatusText(row) {
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
							text: '出库成功',
							cls: 'success'
						}
						return obj2
					case 3:
						let obj3 = {
							text: '出库失败',
							cls: 'be_return'
						}
						return obj3
					case 4:
						let obj4 = {
							text: '已取消',
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
					console.log("出库记录",response);
					this.topTitle='出库记录('+response.data.Total+')'
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
				this.isShowOutMethod=false
				if(row.value==1){
					uni.navigateTo({
						url:'/pages_flow/inventory/partial_returns?LeaveMethod='+row.value
					})
				}else{
					uni.navigateTo({
						url:'/pages_flow/inventory/outbound_records_add?LeaveMethod='+row.value
					})
				}
				
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
		/* #ifndef MP-WEIXIN */
		width: calc(100% - 142rpx);
		left: 0;
		/* #endif */
		/* #ifdef MP-WEIXIN */
		width: calc(100% - 262rpx);
		left: auto;
		right: 0;
		/* #endif */
		height: 88rpx;
		position: fixed;
		top: 0;
		background-color: rgba(0,0,0, 0.5);
		z-index: 999;
	}
	.mask_con3{
		width: 20rpx;
		height: 88rpx;
		position: fixed;
		top: 0;
		/* #ifndef MP-WEIXIN */
		right: 0;
		/* #endif */
		/* #ifdef MP-WEIXIN */
		right: auto;
		left: 0;
		/* #endif */
		background-color: rgba(0,0,0, 0.5);
		z-index: 999;
	}
	.mask_con4{
		width: 100%;
		position: fixed;
		top: 0;
		
		/* #ifndef MP-WEIXIN */
		right: 0;
		/* #endif */
		/* #ifdef MP-WEIXIN */
		right: auto;
		left: 0;
		/* #endif */
		background-color: rgba(0,0,0, 0.5);
		z-index: 999;
	}
	.fixclass{
		width: 100%;
		height: 100vh;
		overflow: hidden;
	}
</style>