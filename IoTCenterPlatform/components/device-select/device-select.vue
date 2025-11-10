<template>
	<scroll-view class="pages_con" :scroll-y="true" @scrolltolower="reachDevice">
		<top :title="topTitle" leftWidth="157rpx" backgroundColor="#161A26" :isleftBack="false" leftIcon="icon-fanhui"
			rightWidth="157rpx" @clickLeft="clickLeft">
		</top>
		<search-compt @openSelect="openSelect" @searching="searching"
			pal="Please enter the name or number" :zIndex="997"></search-compt>
		<select-compt ref="selectCompt" :selectListParam="selectListParam" :querydata="querydata"
			@selectFinsh="selectFinsh"></select-compt>
		<view class="device_list_con">
			<view class="device_list">
				<view class="list_li" @click="selectDeviceFun(row)" v-for="row in deviceTableData">
					<view class="li_left">
						<image class="image" :src="row.PhotoUrl+'?wh=500x500'" mode="aspectFit"></image>
					</view>
					<view class="li_right">
						<view class="dev_name">
							{{row.Name}}
						</view>
						<view class="group">
							{{row.DeviceNumber}}
						</view>
						<view class="group" v-if="row.GroupName">
							Category:{{row.GroupName}}
						</view>
					</view>
					<view class="select_icon t-icon-xuanzhongshebeihaocai"
						v-if="selectDeviceId&&selectDeviceId.includes(row.Id)"></view>
				</view>
				<uni-load-more iconType="circle" :status="status" v-if="status" />
			</view>
			<view v-if="isMulSelect">
				<view class="btn_zhanwei" style="width: 100%;height: 98rpx;"></view>
				<button class="submit_button" @click="confirmSelectdev">
					Confirm({{selectDevice.length}})
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
		addUseDevice
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
				// default: () => {
				// 	return []
				// }
			},
		},
		data() {
			return {
				topTitle: 'Machines',
				selectListParam: [{
					name: 'Status',
					params: 'Online',
					pal: 'Please select a status',
					value: null,
					localdata: [{
							text: "offline",
							value: 0
						},
						{
							text: "Online",
							value: 1
						},
						{
							text: "unKnow",
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
		mounted(options) {
			if (this.ProductId) {
				this.querydata.ProductId = this.ProductId
			}
			console.log(this.ProductList,'过滤的产品');
			if (this.ProductList&&this.ProductList.length>0) {
				this.querydata.ProductList = this.ProductList
			}
			this.getDeviceList()
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
				if (this.status != 'noMore') {
					this.querydata.pageNum++;
					this.status = "loading";
					this.getDeviceList();
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
						this.topTitle = 'Machines（' + res.data.Total + '台）';
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
						this.topTitle = 'Machines（' + res.data.Total + '台）';
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
		background-color: rgba(22, 26, 38, 1);
		height: 100vh;
		line-height: 64rpx;
	}
	.select_con{
		top: 0;
	}
</style>