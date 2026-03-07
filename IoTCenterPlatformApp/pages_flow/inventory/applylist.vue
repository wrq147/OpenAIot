<template>
	<view class="pages_bgcon">
		<top :title="topTitle" leftWidth="120rpx" rightWidth="120rpx" :isleftBack="true"
			leftIcon="icon-fanhui" backgroundColor="#ffffff" :rightIcon="!isOutbounding?'icon-tianjia':''" @clickRight="wareRecordsAdd">
		</top>
		<search-compt @openSelect="openSelect" @searching="searching"
			pal="请输入申请单号或物品名称" backgroundColor="#fff" inputBg="#F8F8F8"></search-compt>
		<select-compt ref="selectCompt" :selectListParam="selectListParam" :querydata="querydata"
			@selectFinsh="selectFinsh" :haidate="true" :timeQuery="timeQuery" fixedHeight="216rpx"></select-compt>
		<v-tabs v-if="!isOutbounding" v-model="current" :scroll="true" :tabs="tabArr" color="#999999"
			activeColor="#333333" :fixed="true" @change="changeTab" :fixedLineWid="true" fontSize="28rpx"
			activeFontSize="28rpx" paddingItem="0 20rpx" lineHeight="4rpx" :hasBorder="false" height="72rpx"
			padding="0 0 0 0 " bgColor="#ffffff" lineColor="#2371FF" :zIndex="1"
			scrollBgColor="#ffffff" scrollConWid="100%"></v-tabs>
		<view class="records_list">
			<view class="records_li" v-for="(item,index) in tbList" @click="jumpPages(item)">
				<view class="li_title">
					<view class="tit_text">申请物品</view>
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
								{{item.List[0].TargetId}}
								<text style="margin-left: 10rpx;" v-if="item.List[0].Quantity!=null">×{{item.List[0].Quantity}}</text>
							</view>
							<view class="text2" v-if="item.List[0].Price!=null">
								价格: ￥{{item.List[0].Price}}
							</view>
						</view>
					</view>
				</view>
				<view class="li_bottom">
					<view class="records_num">{{item.ApplyNumber}}</view>
					<view class="type_status" :class="getSatusText(item).cls" v-if="getSatusText(item)">{{getSatusText(item).text}}</view>
				</view>
				<view class="li_btn" v-if="isOutbounding&&item.Status==2&&item.OutStatus==0" @click.stop="jumpPicking(item)">
					出库
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
	
	import { ApplyList } from "@/api/apply";
	export default {
		data() {
			return {
				statusBarHeight: uni.getSystemInfoSync().statusBarHeight*2 + 'rpx',
				topTitle: '出库申请',
				querydata: {
					pageNum: 1,
					pageSize: 10,
					ShowDetail:true
				},
				tbList: [],
				status: 'loading',
				tabArr: ['全部', '待提交', '待审核', '申请成功', '申请失败', '已取消'],
				current: 0,
				activeName: '',
				selectListParam: [{
					name: '仓库',
					params: 'HouseId',
					pal: '请选择仓库',
					value: null,
					localdata: []
				}],
				timeQuery: {
					name: '申请时间',
					params: ['beginTime', 'endTime'],
					value: [],
				},
				isOutbounding:false
			};
		},
		onLoad(options) {
			this.getHouseList()
			if(options.isout){
				this.isOutbounding=true
				this.querydata.Status=2
				this.querydata.OutStatus=0
				this.getList()
			}else{
				this.activeName = '-1'
				
				this.getList()
			}
			
		},
		onReachBottom() { //上拉触底
			if (this.status != 'noMore') {
				this.querydata.pageNum++;
				this.status = "loading";
				this.getList();
			}
		},
		methods: {
			jumpPicking(row){
				//跳转去领料出库
				uni.navigateTo({
					url:'/pages_flow/inventory/outbound_records_add?applyId='+row.Id
				})
				
			},
			jumpPages(row){
				if(row.Status == 0){
					uni.navigateTo({
						url:'/pages_flow/inventory/apply_add?id='+row.Id
					})
				}else{
					uni.navigateTo({
						url:'/pages_flow/inventory/apply_detail?id='+row.Id
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
							text: '申请成功',
							cls: 'success'
						}
						return obj2
					case 3:
						let obj3 = {
							text: '申请失败',
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
				if(!this.isOutbounding){
					this.querydata['Status'] = this.activeName;
					if(this.activeName==-1){
						delete this.querydata['Status']
					}
				}
				ApplyList(this.querydata).then(response => {
					console.log("申请列表",response);
					this.topTitle='出库申请('+response.data.Total+')'
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
				if(this.isOutbounding){
					return
				}
				uni.navigateTo({
					url:'/pages_flow/inventory/apply_add'
				})
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
		width: calc(100% - 138rpx);
		height: 88rpx;
		position: fixed;
		top: 0;
		left: 0;
		background-color: rgba(0,0,0, 0.5);
		z-index: 999;
	}
	.mask_con3{
		width: 19rpx;
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