<template>
	<view class="pages_con pages_bgcon">
		<top :title="topTitle" leftWidth="157rpx" backgroundColor="#fff" :isleftBack="true" leftIcon="icon-fanhui"
			rightWidth="157rpx">
		</top>
		<search-compt @openSelect="openSelect" @searching="searching" pal="请输入设备名称或者设备编码"
			inputBg="#F8F8F8"></search-compt>
		<select-compt ref="selectCompt" :selectListParam="selectListParam" :querydata="querydata"
			@selectFinsh="selectFinsh"></select-compt>
		<view class="device_list_con">
			<view class="device_list_flex">
				<view class="list_li" @click="selectDeviceFun(row,inx)" v-for="(row,inx) in deviceTableData"
					:style="{'opacity':isDisable&&!selectDeviceId||isDisable&&!selectDeviceId.includes(row.Id)?0.5:1}">
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
			<view v-if="isMulSelect">
				<view class="btn_zhanwei" style="width: 100%;height: 98rpx;"></view>
				<button class="submit_button" @click="confirmSelectdev">
					确定({{selectDevice.length}})
				</button>
			</view>
			<uni-load-more iconType="circle" :status="status" v-if="status" />
		</view>
		<msg-prompt ref="promptMsg"></msg-prompt>

		<!-- 提示信息组件 -->
	</view>
</template>

<script>
	import {
		setPagesParam
	} from '@/common/utillib.js'
	import {
		crmDeviceList,
		addUseDevice
	} from '@/api/device.js'
	import {
		enterDevList,
	} from "@/api/stock";
	export default {
		data() {
			return {
				topTitle: '选择设备',
				selectListParam: [{
					name: '设备状态',
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
				selectDevice: [], //已经选择的
				selectDeviceId: [], //已经选择的
				isMulSelect: false,
				isEnterDev: false,
				isNotReturn: false,//控制选择后是需要返回页面还是关闭弹窗
				isDisable: false, //是否禁用
				isOnlyRead:false,//是否只读
				dataType: '',
				rulesAdForm: {},
				infoIndex: null,
				allInx: 0,
				rulesType:null,//规则添加类型
			}
		},
		onLoad(options) {
			if (options.ProductId) {
				this.querydata.ProductId = options.ProductId
			}
			if (options.ProductList) {
				this.querydata.ProductList = JSON.parse(options.ProductList)
			}
			if (options.isMulSelect) {
				this.isMulSelect = !!options.isMulSelect
			}
			if (options.isEnterDev) {
				this.isEnterDev = options.isEnterDev
			}
			if (options.isNotReturn) {
				this.isNotReturn = true
			}
			if (options.isDisable||options.isOnlyRead) {
				this.isDisable = true
				if(options.isOnlyRead){
					this.isOnlyRead=true
				}
			}
			if (options.dataType) {
				this.dataType = options.dataType
			}
			if (options.rulesType) {
				this.rulesType = options.rulesType
			}
			if (options.selectDevice) {
				this.selectDevice = JSON.parse(options.selectDevice)
				this.selectDeviceId = this.selectDevice.map(item => {
					if (item.id) {
						return item.id
					} else if (item.TargetId) {
						return item.TargetId
					}
				})
			}
			if (options.allInx) {
				this.allInx = options.allInx
			}
			if (options.inx) {
				this.infoIndex = options.inx
				let rulesAdForm = this.$store.state.rulesAddForm
				this.rulesAdForm = JSON.parse(JSON.stringify(rulesAdForm))
				// console.log("规则", this.rulesAdForm);
				if (this.rulesType=='condition'&&this.dataType == 'event') {
					this.selectDevice = rulesAdForm.eventDevice
					this.selectDeviceId = this.selectDevice.map(item => {
						if (item.id) {
							return item.id
						} else if (item.TargetId) {
							return item.TargetId
						} else if (item.Id) {
							return item.Id
						}
					})
					this.$forceUpdate()
				} else if (this.dataType == 'attribute') {
					let arr = []
					arr.push(rulesAdForm.attrDevice[Number(options.inx)])
					this.selectDevice = arr
					this.selectDeviceId = this.selectDevice.map(item => {
						if (item&&item.id) {
							return item.id
						} else if (item&&item.TargetId) {
							return item.TargetId
						} else if (item&&item.Id) {
							return item.Id
						}
					})
				} else if (this.rulesType=='function'&&this.dataType == 'event'||this.dataType == 'function') {
					let arr = []
					arr.push(rulesAdForm.funcDevice[Number(options.inx)])
					this.selectDevice = arr
					this.selectDeviceId = this.selectDevice.map(item => {
						if (item.id) {
							return item.id
						} else if (item.TargetId) {
							return item.TargetId
						} else if (item.Id) {
							return item.Id
						}
					})
				}
			}
			this.getDeviceList()
		},
		onReachBottom() { //上拉触底
			if (this.status != 'noMore') {
				this.querydata.pageNum++;
				this.status = "loading";
				this.getDeviceList();
			}
		},
		methods: {
			confirmSelectdev() {
				//确定选择设备结果
				setPagesParam('selectDevice', this.selectDevice, 1)
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
						this.topTitle = '选择设备（' + res.data.Total + '台）';
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
						this.topTitle = '选择设备（' + res.data.Total + '台）';
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
			selectDeviceFun(row, inx) {//规则中选择设备

				if (this.isDisable) {
					if (this.selectDeviceId && this.selectDeviceId.includes(row.Id)) {
						let obj = {
							deviceInfo: row
						}
						this.$store.commit('SET_OTHERRULES_INFO', obj)
						if(this.isOnlyRead){
							uni.navigateTo({
								url: '/pages_rules/fun_select?dataType=' + this
									.dataType + '&isDisable=true' + '&haival=true&inx=' + this.infoIndex + '&allInx=' +
									this.allInx+'&isOnlyRead='+this.isOnlyRead+'&rulesType='+this.rulesType
							})
						}else{
							uni.navigateTo({
								url: '/pages_rules/fun_select?dataType=' + this
									.dataType + '&isDisable=true' + '&haival=true&inx=' + this.infoIndex + '&allInx=' +
									this.allInx+'&rulesType='+this.rulesType
							})
						}
						

					}
				} else if (this.rulesType == 'condition' &&this.dataType == 'event' && this.rulesAdForm.eventDevice && this.rulesAdForm.eventDevice[0] &&
					this.rulesAdForm.eventDevice[0].Id == row.Id) {

					if (this.rulesAdForm.eventDevice && this.rulesAdForm.eventDevice.length > 0) {
						let this_device = "/" + row.ProductId + "/" + row.DeviceId
						let arr = []
						arr.push(row)
						let triObj = {
							topicDevice: this_device,
							topicMsg: 'Event'
						}
						let triggerLs = []
						triggerLs.push(triObj)
						let process = {
							id: "root",
							parentId: null,
							type: "ROOT",
							name: "发起人",
							desc: "任何人",
							children: {}
						};
						let obj = this.rulesAdForm
						obj.eventDevice = JSON.parse(JSON.stringify(arr))
						obj.triggerList = JSON.parse(JSON.stringify(triggerLs))
						obj.name = row.Name
						this.$store.commit('SET_OTHERRULES_INFO', obj)
					}
					uni.navigateTo({
						url: '/pages_rules/fun_select?dataType=' + this
							.dataType + '&haival=true&inx=' + this.infoIndex + '&allInx=' + this.allInx+'&rulesType='+this.rulesType
					})
				} else {
					if (this.selectDeviceId && this.selectDeviceId.includes(row.Id)) {
						let inxO = this.selectDeviceId.indexOf(row.Id)
						this.selectDeviceId.splice(inxO, 1)
						this.selectDevice.splice(inxO, 1)
					} else {
						if (this.isNotReturn) {
							this.selectDevice = []
							this.selectDeviceId = []
							this.selectDevice.push(row)
							this.selectDeviceId.push(row.Id)
						} else {
							this.selectDevice.push(row)
							this.selectDeviceId.push(row.Id)
						}

					}
					if (this.isMulSelect) {} else {
						let arr = []
						arr.push(row)
						if (this.isNotReturn) {
							// setPagesParam('selectDevice', arr, 1, false)
							if (this.rulesType == 'condition'&&this.dataType == 'event') {
								let rulesAdForm = this.$store.state.rulesAddForm
								if (rulesAdForm.eventDevice && rulesAdForm.eventDevice.length > 0) {
									let this_device = "/" + row.ProductId + "/" + row.DeviceId
									let triObj = {
										topicDevice: this_device,
										topicMsg: 'Event'
									}
									let triggerLs = []
									triggerLs.push(triObj)
									let process = {
										id: "root",
										parentId: null,
										type: "ROOT",
										name: "发起人",
										desc: "任何人",
										children: {}
									};
									let obj = rulesAdForm
									obj.eventDevice = JSON.parse(JSON.stringify(arr))
									obj.triggerList = JSON.parse(JSON.stringify(triggerLs))
									obj.name = row.Name
									this.$store.commit('SET_OTHERRULES_INFO', obj)
								} else {
									let this_device = "/" + row.ProductId + "/" + row.DeviceId
									let triObj = {
										topicDevice: this_device,
										topicMsg: 'Event'
									}
									let triggerLs = []
									triggerLs.push(triObj)
									let process = {
										id: "root",
										parentId: null,
										type: "ROOT",
										name: "发起人",
										desc: "任何人",
										children: {}
									};
									let obj = {
										eventDevice: arr,
										name: row.Name,
										sort: 0,
										triggerWay: 0,
										triggerList: JSON.parse(JSON.stringify(triggerLs)),
										timerCron: '',
										ruleJson: JSON.stringify(process),
										httpParams: '',
										remark: '',
										debug: 1
									}
									// this.$store.commit('SET_RULESADD_INFO', obj)
									this.$store.commit('SET_OTHERRULES_INFO', obj)
								}
								
							} else {
								let obj = {
									deviceInfo: row
								}
								this.$store.commit('SET_OTHERRULES_INFO', obj)
							}
							if (this.allInx && this.infoIndex) {
								uni.navigateTo({
									url: '/pages_rules/fun_select?dataType=' + this
										.dataType + '&haival=true&allInx=' + this.allInx + '&inx=' + this.infoIndex+'&rulesType='+this.rulesType
								})
							} else {
								uni.navigateTo({
									url: '/pages_rules/fun_select?dataType=' + this.dataType+'&rulesType='+this.rulesType
								})
							}

						} else {
							setPagesParam('selectDevice', arr, 1)
						}

					}
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

<style lang="less" scoped>
	.pages_con {
		// background-color: rgba(22, 26, 38, 1);
	}
</style>