<template>
	<scroll-view class="pages_con" :scroll-y="true" @scrolltolower="reachDevice">
		<top :title="topTitle" leftWidth="157rpx" backgroundColor="#fff" :isleftBack="false" leftIcon="icon-fanhui"
			rightWidth="157rpx" @clickLeft="clickLeft">
		</top>
		<search-compt @openSelect="openSelect" @searching="searching"
			pal="请输入设备名称或者设备编码" :zIndex="997" inputBg="#F8F8F8" :isOnlySearch="isCustomList"></search-compt>
		<select-compt ref="selectCompt" :selectListParam="selectListParam" :querydata="querydata"
			@selectFinsh="selectFinsh"></select-compt>
		<view class="device_list_con">
			<view class="device_list_flex">
				<view class="list_li" @click="selectDeviceFun(row,inx)" v-for="(row,inx) in deviceTableData">
					<view class="li_top">
						<image class="image" :src="row.PhotoUrl+'?wh=500x500'" mode="aspectFill" v-if="row.PhotoUrl">
						</image>
						<image class="image" :src="getSerVerUrl()+'/appimg/device_default.png'" mode="aspectFill" v-else></image>
						<view class="device_state">
							<view class="online_status" :class="{'offline':row.Online==0,'unKnow':row.Online==2}">
								<view class="dot"></view>
								<view class="text" v-if="row.Online==0">离线</view>
								<view class="text" v-if="row.Online==1">在线</view>
								<view class="text" v-if="row.Online==2">未知</view>
							</view>
							<view class="alarm_status" v-if="row.HavWarn">
								<custom-icons iconsName="icon-baojing" iconsSize="24rpx"
									iconsColor="rgba(255, 53, 53, 1)"></custom-icons>
							</view>
						</view>
					</view>
					<view class="li_name_group">
						<view class="dev_name">
							{{row.Name}}
						</view>
						<view class="group" v-if="row.GroupName||row.ProductName">
							<text class="group_name" v-if="row.GroupName">
								{{row.GroupName}}
							</text>
							<view class="line" v-if="row.GroupName&&row.ProductName"></view>
							<text class="group_name" v-if="row.ProductName">
								{{row.ProductName}}
							</text>
						</view>
					</view>
					<view class="select_icon t-icon-xuanzhongshebeihaocai1"
						v-if="selectDeviceId&&selectDeviceId.includes(row.Id)"></view>
				</view>
			</view>
			<uni-load-more iconType="circle" :status="status" v-if="status" />
			<view v-if="isMulSelect">
				<view class="btn_zhanwei" style="width: 100%;height: 98rpx;"></view>
				<button class="submit_button" @click="confirmSelectdev">
					确定({{selectDevice.length}})
				</button>
			</view>
		</view>
		<msg-prompt ref="promptMsg"></msg-prompt>

		<!-- 提示信息组件 -->
	</scroll-view>
</template>

<script>
	// import {
	// 	setPagesParam
	// } from '@/common/utillib.js'
	import {
		crmDeviceList,
		planeDevList,
	} from '@/api/device.js'
	import {
		enterDevList,
	} from "@/api/stock";
	export default {
		name: 'device-select',
		props: {
			ProductId: {//过滤的产品id
				type: String,
				default: "",
			},
			isMulSelect: {//是否多选
				type: Boolean,
				default: false,
			},
			isCustomList: {//是否选择已有设备
				type: Boolean,
				default: false,
			},
			limit:{
				type: Number,
				default: 1,
			},
			isEnterDev: {//是否是出库设备
				type: Boolean,
				default: false,
			},
			type: {
				default: "user",
				type: String,
			},
			selected: {
				type: Array,
				default: () => {
					return []
				}
			},
			ProductList: {//过滤的产品列表
				type: Array,
				default: () => {
					return []
				}
			},
			allDeviceList: {
				type: Array,
				default: () => {
					return []
				}
			},
			planeId: {
				default: "",
				type: String,
			},
		},
		data() {
			return {
				topTitle: '设备',
				selectListParam: [{
					name: '状态',
					params: 'Online',
					pal: '请选择状态',
					value: null,
					localdata: [{
							text: "离线",
							value: 0
						},
						{
							text: "在线",
							value: 1
						},
						{
							text: "未知",
							value: 2
						}
					]
				}],
				querydata: {
					pageNum: 1,
					pageSize: 30,
					Name: ""
				}, //过滤参数
				statusList: [{
						text: "离线",
						value: 0
					},
					{
						text: "在线",
						value: 1
					}
				],
				deviceTableData: [],
				key: '', //搜索关键词
				status: 'loading',
				selectDevice: [], //已经选择的设备列表
				selectDeviceId: [], //已经选择的设备id数组
				// isMulSelect: false,
				// isEnterDev: false
			}
		},
		mounted() {
			if(!this.isCustomList){
				if (this.ProductId) {
					this.querydata.ProductId = this.ProductId
				}
				if (this.ProductList&&this.ProductList.length>0) {
					this.querydata.ProductList = this.ProductList
				}
				this.getDeviceList()
			}else{
				this.getplaneDevList()
			}
			
		},
		watch:{
			allDeviceList:{
				handler(to){
					this.deviceTableData=this.allDeviceList
					this.status='noMore'
				},
				immediate:true,
				deep:true
			}
		},
		// onReachBottom() { //上拉触底
		// 	if (this.status != 'noMore') {
		// 		this.querydata.pageNum++;
		// 		this.status = "loading";
		// 		this.getDeviceList();
		// 	}
		// },
		methods: {
			clickLeft() {
				this.$emit('closeSelect')
			},
			reachDevice(){
				if(!this.isCustomList){
					if (this.status != 'noMore') {
						this.querydata.pageNum++;
						this.status = "loading";
						this.getDeviceList();
					}
				}else{
					
				}
				
			},
			setSelected(val){
				
				if (val) {
					this.selectDevice = JSON.parse(JSON.stringify(val))
					this.selectDeviceId = this.selectDevice.map(item => {
						if (item.id) {
							return item.id
						} else if (item.TargetId) {
							return item.TargetId
						}
					})
					
				}
			},
			confirmSelectdev() {
				//确定选择设备结果
				// setPagesParam('selectDevice', this.selectDevice, 1)
				this.$emit('selectDevice', this.selectDevice)
			},
			loadList() {
				//加载列表的方法
				this.querydata.pageNum = 1
				this.status = "loading";
				this.getDeviceList()
			},
			searching(val) {
				if(this.isCustomList){
					if(val){
						this.deviceTableData=this.allDeviceList.filter(row=>row.Name&&row.Name.indexOf(val)>-1||row.DeviceNumber&&row.DeviceNumber.indexOf(val)>-1||row.DeviceId&&row.DeviceId.indexOf(val)>-1)
					}else{
						this.deviceTableData=this.allDeviceList
					}
					
					return
				}
				this.key = val
				if (this.isEnterDev) {
					if (this.key) {
						this.querydata.Key = this.key
						this.querydata.pageNum = 1
						this.status = 'loading'
						this.deviceTableData = []
						this.getDeviceList()
					} else {
						delete this.querydata.Key
						this.querydata.pageNum = 1
						this.status = 'loading'
						this.deviceTableData = []
						this.getDeviceList()
					}
				} else {
					if (this.key) {
						this.querydata.Name = this.key
						this.querydata.pageNum = 1
						this.status = 'loading'
						this.deviceTableData = []
						this.getDeviceList()
					} else {
						delete this.querydata.Name
						this.querydata.pageNum = 1
						this.status = 'loading'
						this.deviceTableData = []
						this.getDeviceList()
					}
				}
			},
			getplaneDevList(){
				if(this.planeId){
					this.querydata.id=this.planeId
				}
			  	planeDevList(this.querydata).then(res=>{
					if (this.querydata.pageNum == 1) {
						this.deviceTableData = []
					}
					this.deviceTableData = [...this.deviceTableData, ...res.data.List];
					this.topTitle = '设备（' + res.data.Total + '台）';
					if (res.data.List.length < this.querydata.pageSize) {
						this.status = 'noMore';
					} else {
						this.status = 'more';
					}
				}).catch(err=>{
					this.status = 'noMore';
					this.setMsgTop(err)
				})
			},
			getDeviceList(query) {
				//获取设备列表
				if (query && query == 'unbind') {
					this.querydata.pageNum = 1
				}

				if (this.isEnterDev) {
					if (!this.querydata.Key) {
						delete this.querydata.Key
					}
					enterDevList(this.querydata).then(res => {
						if (this.querydata.pageNum == 1) {
							this.deviceTableData = []
						}
						this.deviceTableData = [...this.deviceTableData, ...res.data.List];
						this.topTitle = '设备（' + res.data.Total + '台）';
						if (res.data.List.length < this.querydata.pageSize) {
							this.status = 'noMore';
						} else {
							this.status = 'more';
						}
					}).catch(err => {
						this.status = 'noMore';
						this.setMsgTop(err)
					})
				} else {
					if (!this.querydata.Name) {
						delete this.querydata.Name
					}
					crmDeviceList(this.querydata).then(res => {
						if (this.querydata.pageNum == 1) {
							this.deviceTableData = []
						}
						this.deviceTableData = [...this.deviceTableData, ...res.data.List];
						this.topTitle = '设备（' + res.data.Total + '台）';
						if (res.data.List.length < this.querydata.pageSize) {
							this.status = 'noMore';
						} else {
							this.status = 'more';
						}
					}).catch(err => {
						this.status = 'noMore';
						this.setMsgTop(err)
					})
				}

			},
			selectDeviceFun(row) {
				if (this.selectDeviceId && this.selectDeviceId.includes(row.Id)) {
					let inxO = this.selectDeviceId.indexOf(row.Id)
					this.selectDeviceId.splice(inxO, 1)
					this.selectDevice.splice(inxO, 1)
				} else {
					if(this.isMulSelect&&this.limit<=this.selectDevice.length){
						
					}else{
						this.selectDevice.push(row)
						this.selectDeviceId.push(row.Id)
					}
					
				}
				if (this.isMulSelect) {} else {
					let arr = []
					arr.push(row)
					// setPagesParam('selectDevice', arr, 1)
					this.$emit('selectDevice', arr)
				}

			},
			openSelect() {
				if(this.isCustomList){
					return
				}
				this.$refs.selectCompt.openSelect()
			},
			selectFinsh(query) {
				this.querydata = JSON.parse(JSON.stringify(query))
				this.querydata.pageNum = 1
				this.status = 'loading'
				this.deviceTableData = []
				this.getDeviceList()
			}
		}
	}
</script>

<style lang="less">
	.pages_con {
		background-color: #F5F8F9;
		height: 100vh;
		line-height: 64rpx;
	}
	.select_con{
		top: 0;
	}
</style>