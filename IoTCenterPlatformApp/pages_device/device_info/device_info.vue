<template>
	<view class="pages_bgcon" @click="hideEditDiv">
		<top :isRightSlot="true" :title="topTitle" leftWidth="157rpx" leftIcon="icon-fanhui" rightWidth="157rpx"
			:isleftBack="true" backgroundColor="rgba(255, 255, 255, 1)" :key="componentKey">
			<template v-slot:top_right>
				<view class="right_top relative_right" @click.stop="changeMoreHandle">
					<custom-icons iconsName="icon-gengduo1" iconsSize="36rpx"></custom-icons>
					<view class="right_ablist" v-show="isShowMoreHandle">
						<view class="abli" @click="editDeviceInfo">
							<custom-icons iconsName="icon-bianji" iconsSize="28rpx"></custom-icons>
							<view class="text">编辑</view>
						</view>
						<view class="abli" v-if="isDeviceUser" @click="UnbindDevice">
							<custom-icons iconsName="icon-jiebang" iconsSize="28rpx"></custom-icons>
							<view class="text">解绑</view>
						</view>
						<view class="abli" @click="jumpNetWork" v-if="showNetWorkUrl">
							<custom-icons iconsName="icon-peiwang" iconsSize="28rpx"></custom-icons>
							<view class="text">配网</view>
						</view>
					</view>
				</view>
				<!-- <view class="right_top" @click="editDeviceInfo">
					<custom-icons iconsName="icon-bianji" iconsSize="36rpx"></custom-icons>
				</view> -->

			</template>
		</top>

		<uni-nav-bar :status-bar="false" :fixed="true" :border="false" :height="zhanweiHei"
			backgroundColor="rgba(255, 255, 255, 1)" :z-index="995">
			<template v-slot:allslot>
				<view class="details_top_con" id="detail_top" ref="detail_top">
					<view class="device_top">
						<view class="info_left" @click.stop="preViewImg(deviceBasicInfo.PhotoUrl)"
							v-if="deviceBasicInfo.PhotoUrl">
							<image class="image" :src="deviceBasicInfo.PhotoUrl+'?wh=500x500'" mode="aspectFill">
							</image>
						</view>
						<view class="info_left" @click.stop="preViewImg(getSerVerUrl()+'/appimg/device_default.png')"
							v-else>
							<image class="image" :src="getSerVerUrl()+'/appimg/device_default.png'" mode="aspectFill">
							</image>
						</view>
						<view class="info_right">
							<view class="info_title">
								{{deviceBasicInfo.Name}}
							</view>
							<view class="info_type">
								<text class="group_name" v-if="deviceBasicInfo.GroupName">
									{{deviceBasicInfo.GroupName}}
								</text>
								<view class="line" v-if="deviceBasicInfo.GroupName&&deviceBasicInfo.ProductName"></view>
								<text class="group_name" v-if="deviceBasicInfo.ProductName">
									{{deviceBasicInfo.ProductName}}
								</text>
							</view>
							<view class="info_status_con">
								<view class="info_status"
									:class="{'offline':deviceBasicInfo.Online==0,'unKnow':deviceBasicInfo.Online==2}">
									<view class="dot" v-if="deviceBasicInfo.Online!=2"></view>
									<view class="status_text">{{getOnlineInfo(deviceBasicInfo.Online)}}</view>
								</view>
								<view class="info_status default"
									style="display: flex; align-items: center;margin-left: 20rpx;"
									:class="{ maintenance: deviceBasicInfo.DState == '保养',normal: deviceBasicInfo.DState == '正常', repair: deviceBasicInfo.DState == '维修' }">
									<view class="text" v-if="deviceBasicInfo.DState=='正常'">{{deviceBasicInfo.DState}}
									</view>
									<view class="text" v-else-if="deviceBasicInfo.DState=='维修'">
										{{deviceBasicInfo.DState}}</view>
									<view class="text" v-else-if="deviceBasicInfo.DState=='保养'">
										{{deviceBasicInfo.DState}}</view>
									<view class="text" v-else>{{deviceBasicInfo.DState}}</view>
								</view>
							</view>
						</view>
					</view>
					<view class="alarm_cont" v-if="isAlarm" @click="jumpAlarm">
						<view class="alarm_left">
							<custom-icons iconsName="icon-baojing" iconsSize="28rpx"
								iconsColor="#FF3535"></custom-icons>
							<view class="text">
								<view class="text_top">
									{{newAlarmInfo.Description}}
								</view>
								<view class="text_bottom">
									{{newAlarmInfo.CreateOn}}
								</view>
							</view>
						</view>
						<view class="alarm_right">
							<custom-icons iconsName="icon-a-youjiantouhong" iconsSize="20rpx"
								iconsColor="#FF9A9A"></custom-icons>
						</view>
					</view>
					<view class="line_bor" v-if="!isAlarm"></view>
					<!-- <view class="margin_class"></view> -->
				</view>
			</template>
		</uni-nav-bar>
		<v-tabs ref="devicetabs" v-model="current" :scroll="false" :tabs="tabArr" color="rgba(153, 153, 153, 1)"
			activeColor="rgba(51, 51, 51, 1)" :fixed="true" @change="changeTab" :lineScale="0.16" fontSize="28rpx"
			activeFontSize="28rpx" paddingItem="0" lineHeight="4rpx" :hasBorder="false" height="72rpx" padding="0"
			bgColor="#ffffff" lineColor="rgba(35, 113, 255, 1)" :zIndex="995" scrollPadding="0 0 0 0"
			scrollBgColor="#ffffff" :key="'tab'+vtabsKey"></v-tabs>
		<!-- <view style="width: 100%;height: 20rpx;"> -->
		<!-- 这是用于控制v-tabs的顶部内边距占位 -->
		<!-- </view> -->
		<view class="info_con">
			<device-basic v-show="tabArr[current]=='详情'" :deviceBasicInfoObj="deviceBasicInfoObj"
				:deviceBasic="deviceBasicInfo"></device-basic>
			<view class="empty_con"
				v-if="tabArr[current]=='详情'&&deviceBasicInfoObj.basic&&deviceBasicInfoObj.basic.value&&deviceBasicInfoObj.Tag.value&&deviceBasicInfoObj.basic.value.length==0&&deviceBasicInfoObj.Tag.value.length==0">
				<image class="image" :src="getSerVerUrl()+'/appimg/no_data.png'" mode=""></image>
				<view class="text">暂无数据</view>
			</view>
			<device-iot v-if="isHasIotData" v-show="tabArr[current]=='状态'" @setDeviceOnline='setDeviceOnline'
				:deviceId="deviceBasicInfo.DeviceId" :deviceBasic="deviceBasicInfo"
				:properties="properties"></device-iot>
			<device-plane ref="devplane" v-if="tabArr[current]=='计划'" :deviceBasic="deviceBasicInfo"></device-plane>
			<device-func v-if="isHasIotData" v-show="tabArr[current]=='功能'" :id="deviceId"
				:deviceBasic="deviceBasicInfo" :deviceId="deviceBasicInfo.DeviceId"></device-func>
			<device-alarm :isHasIotData="isHasIotData" :ararmList="alarmList" v-show="tabArr[current]=='报警'"
				:isShowDevice="false" :status="alarmStatus" :properties="properties"
				:id="deviceBasicInfo.Id"></device-alarm>
			<devicePosition v-if="tabArr[current]=='位置'" :deviceBasic="deviceBasicInfo" :positionInfo="positionInfo">
			</devicePosition>

		</view>
		<msg-prompt ref="promptMsg" @confirm="confirmUnbind"></msg-prompt>
		<!-- 提示信息组件 -->
	</view>
</template>

<script>
	import deviceBasic from '@/pages_device/device_info/components/device-basic.vue'
	import devicePlane from '@/pages_device/device_info/components/device-plane.vue'
	import deviceIot from '@/pages_device/device_info/components/device-iot.vue'
	import deviceAlarm from '@/pages_device/device_info/components/device-alarm.vue'
	import deviceFunc from '@/pages_device/device_info/components/device-func.vue'
	import devicePosition from '@/pages_device/device_info/components/device-position.vue'
	import {
		crmDeviceInfo,
		crmDeviceTagInfo,
		crmDeviceProductInfo,
		warningList,
		UnUseDevice
	} from '@/api/device.js'
	import {
		setPagesParam
	} from '@/common/utillib.js'
	import {
		checkPermi
	} from '@/common/permission.js';

	// import BMap from 'baidumap-sdk';

	export default {
		components: {
			deviceBasic,
			devicePlane,
			deviceIot,
			deviceAlarm,
			deviceFunc,
			devicePosition
		},
		data() {
			return {
				vtabsKey: 1,
				topTitle: '设备详情',
				isShowMoreHandle: false, //是否显示更多的操作
				isAlarm: false,
				newAlarmInfo: {},
				tabArr: ['详情', '状态', '计划', '功能', '报警'],
				current: null,
				zhanweiHei: 0,
				alarmList: ['Ordinary'],
				// ararmList: ['Ordinary', 'Urgent', 'Warning', 'Ordinary', 'Urgent', 'Warning'],
				zhanweiHeiNumber: 0,
				deviceId: 0, //设备id
				deviceProduct: 0, //设备产品id
				deviceProductInfo: {}, //设备产品信息
				positionInfo: {}, //位置相关属性信息
				deviceBasicList: [],
				deviceBasicInfo: {},
				deviceBasicInfoObj: {
					basic: {
						name: '设备信息',
						value: []
					},
					Tag: {
						name: '标签信息',
						value: []
					}
				}, //设备基本信息整合
				alarmStatus: 'loading',
				alarmQuery: {
					pageNum: 1,
					pageSize: 30
				},
				isDeviceUser: false, //是否是设备使用者
				timer: null,
				componentKey: 1, //头部组件的key值
				properties: [], //产品属性列表
				stateInfo: {},
				isHasIotData: false, //是否有物联网数据
			};
		},
		async onLoad(options) {
			if (this.$store.state.user && this.$store.state.user.uid) {} else {
				await this.$store.dispatch('GetInfo')
			}
			// this.$store.dispatch('GetInfo').then(() => {})
			this.$nextTick(()=>{
				setTimeout(() => {
					this.getTopNavHei()
				}, 500)
			})
			if (options.id) {
				//设备id
				this.deviceId = options.id
				uni.showLoading({
					title: '加载中'
				})
				await this.getDeviceInfo()
				this.getDeviceTagInfo()
				this.getDeviceAlarm()
				uni.hideLoading()
			}
			if (options.Id) {
				//设备id
				this.deviceId = options.Id
				uni.showLoading({
					title: '加载中'
				})
				await this.getDeviceInfo()
				this.getDeviceTagInfo()
				this.getDeviceAlarm()
				uni.hideLoading()
			}
		},
		beforeDestroy() {
			if (this.timer) {
				clearInterval(this.timer)
			}
		},
		onUnload() {
			if (this.timer) {
				clearInterval(this.timer)
			}
		},
		onReachBottom() { //上拉触底
			// console.log("现在的状态是什么", this.status);
			if (this.tabArr[this.current] == '报警') {
				if (this.alarmStatus != 'noMore') {
					this.alarmQuery.pageNum++;
					this.alarmStatus = "loading";
					this.getDeviceAlarm();
					// console.log("现在是第几页", this.page);
				}
			}
			// if (this.current == 2) { //设备计划分页查询
			// 	let planeStatus = this.$refs.devplane.status
			// 	if (planeStatus != 'noMore') {
			// 		let pageNum = this.$refs.devplane.planeQuery.pageNum + 1;
			// 		this.$refs.devplane.getdevPlaneList(pageNum, 'loading');
			// 		// console.log("现在是第几页", this.page);
			// 	}
			// }
			if (this.tabArr[this.current] == '计划') {
				this.$refs.devplane.planeReachBottom()
			}
		},
		methods: {
			loadTaskList() {
				this.$refs.devplane.getdevPlaneList()
			},
			jumpAlarm() {
				let index = this.tabArr.findIndex(item => item === '报警');
				if (index && index > -1) {
					this.current = index
				}
			},
			showNetWorkUrl() {
				if (this.deviceProductInfo.PhysicsWay && this.deviceProductInfo.PhysicsWay.toLowerCase().indexof('wifi') >
					-1) {
					return true
				} else {
					return false
				}
			},
			hideEditDiv() {
				this.isShowMoreHandle = false
				this.componentKey++
			},
			changeMoreHandle() {
				this.isShowMoreHandle = !this.isShowMoreHandle
				this.componentKey++
				this.$forceUpdate()
			},
			jumpNetWork() {
				uni.navigateTo({
					url: '/pages_device/wifi-conf/wifi-conf?dtuId=' + this.deviceBasicInfo.DeviceId
				});
			},
			editDeviceInfo() {
				//跳转至编辑页面
				uni.navigateTo({
					url: '/pages_device/edit_device?id=' + this.deviceId
				})
			},
			preViewImg(url) {
				let list = []
				list.push(url)
				uni.previewImage({
					current: 0,
					urls: list
				});

			},
			UnbindDevice() {
				//解绑设备
				if (this.isDeviceUser) {
					this.$refs.promptMsg.noticeOpen(
						'解除绑定后，您将无法查看设备的信息。你确定要解除绑定吗?'
					)
				}
			},
			confirmUnbind() {
				//确认解绑
				UnUseDevice({
					id: this.deviceBasicInfo.Id
				}).then(res => {
					// console.log("res设备使用解绑",res);
					this.$refs.promptMsg.open('取消绑定成功', 2000)
					setTimeout(() => {
						setPagesParam('getDeviceList', 'unbind')
					}, 2050)
				}).catch(err => {
					this.setMsgTop(err)
				})
			},
			getDeviceAlarm() {
				//获取设备报警列表
				this.alarmQuery.DeviceId = this.deviceId;
				if (this.alarmQuery.pageNum == 1) {
					this.alarmList = [];
				}
				warningList(this.alarmQuery).then(
					async res => {
						// console.log("查询到的报警列表", res);
						this.alarmList = [...this.alarmList, ...res.data.List];
						if (res.data.List.length < this.alarmQuery.pageSize) {
							this.alarmStatus = 'noMore';
						} else {
							this.alarmStatus = 'more';
						}
						if (this.alarmList && this.alarmList.length > 0) {
							this.isAlarm = true
							this.newAlarmInfo = this.alarmList[0]
						} else {
							this.isAlarm = false
							this.newAlarmInfo = {}
						}
					}
				);
			},
			getWarningList(query) {
				if (query) {
					this.alarmQuery.pageNum = 1
					this.getDeviceAlarm()
				}
			},
			getOnlineInfo(val) {
				//获取在线信息
				if (val == 0) {
					return '离线'
				}
				if (val == 1) {
					return '在线'
				}
				if (val == 2) {
					return '未知'
				}
			},
			setDeviceOnline(val) {
				//设置设备在线信息
				this.deviceBasicInfo.Online = val
			},

			async getDeviceInfo() {
				//获取设备基本信息
				try {
					let res = await crmDeviceInfo({
						id: this.deviceId
					})
					// console.log("设备基本信息", res);
					this.deviceProduct = res.data.ProductId
					this.deviceBasicInfo = res.data
					// console.log(this.deviceBasicInfo, '基本信息');
					if (!this.$store.state.user.orgId || !this.$store.state.user.uid) {
						await this.$store.dispatch('GetInfo')
					}
					if (this.$store.state.user.orgId == res.data.UseOrgId || this.$store.state.user.uid == res.data
						.UseUserId) {
						this.isDeviceUser = true
					} else {
						this.isDeviceUser = false
					}
					this.getDeviceProductInfo()

					// if (this.deviceBasicInfo.Lat && this.deviceBasicInfo.Lng) {
					// 	this.tabArr = ['详情', '状态', '计划', '功能', '报警', '位置']
					// 	this.current = 0;

					// } else {
					// 	this.tabArr = ['详情', '状态', '计划', '功能', '报警'];
					// 	this.current = 0;
					// }

				} catch (e) {
					//TODO handle the exception
					this.setMsgTop(e)
				}

			},
			async getDeviceProductInfo() {
				//获取设备产品信息
				try {
					let rsp = await crmDeviceProductInfo({
						id: this.deviceBasicInfo.ProductId,
						notsl: false
					})
					this.deviceProductInfo = rsp.data
					if (this.deviceProductInfo.NetworkWay != '') {
						this.isHasIotData = true
						this.tabArr = ['详情', '状态', '计划', '功能', '报警']
						this.vtabsKey++
						this.current = 0;
					} else {
						this.isHasIotData = false
						this.tabArr = ['详情', '计划', '报警', ]
						this.current = 0;
						this.vtabsKey++
					}
					setTimeout(() => {
						this.$refs.devicetabs.update()
					}, 1000)
					setTimeout(() => {
						this.positionInfo = {}
						let mds = JSON.parse(this.deviceProductInfo.ModelTSL);
						this.properties = JSON.parse(JSON.stringify(mds.properties))
						let tags = mds.tags;
						tags.map((row) => {
							if (row.code == "position") {
								this.positionInfo = row;
								if (this.positionInfo && this.positionInfo.enable && this
									.deviceBasicInfo.Lat && this.deviceBasicInfo.Lng) {
									this.tabArr = [...this.tabArr, ...['位置']]
									this.current = 0;
									this.vtabsKey++
								} else {
									// this.tabArr = ['详情', '状态', '计划', '功能', '报警'];
									this.current = 0;
									this.vtabsKey++
								}
								this.$forceUpdate()
							} else {
								this.current = 0;
								this.vtabsKey++
							}
						});
					}, 50);
					// console.log("产品信息", rsp);
					let LastOnlinetime = ''
					if (this.deviceBasicInfo.Online == 0) {
						LastOnlinetime = this.deviceBasicInfo.LastOnline
					} else {
						if (this.deviceBasicInfo.Online == 1) {
							LastOnlinetime = '在线中'
						}
					}
					this.deviceBasicList = [{
						Name: '设备编号',
						DisplayValue: this.deviceBasicInfo.DeviceNumber
					}, {
						Name: '通讯编码',
						DisplayValue: this.deviceBasicInfo.DeviceId
					}, {
						Name: '设备分组',
						DisplayValue: this.deviceBasicInfo.GroupName
					}, {
						Name: '产品名称',
						DisplayValue: rsp.data.Name
					}, {
						Name: '产品分类',
						DisplayValue: rsp.data.ClassName
					}, {
						Name: '出厂编号',
						DisplayValue: this.deviceBasicInfo.PD
					}, {
						Name: '创建时间',
						DisplayValue: this.deviceBasicInfo.CreateOn
					}, {
						Name: '离线时间',
						DisplayValue: LastOnlinetime,
					}, {
						Name: '设备说明',
						DisplayValue: this.deviceBasicInfo.Remark,
					}]
					this.deviceBasicInfoObj.basic = {
						name: '设备信息',
						type: 1,
						value: this.deviceBasicList
					}
					// console.log(this.deviceBasicInfoObj, '111111111');
				} catch (e) {
					//TODO handle the exception
					console.log(e, 'eeeeeeeee');
					this.setMsgTop(e)
				}

			},
			getDeviceTagInfo() {
				//获取标签信息
				crmDeviceTagInfo({
					id: this.deviceId
				}).then(res => {
					// console.log('标签数据', res);
					res.data = res.data.map(row => {
						row.isEdit = false
						if(row.Option.type=='enum'){
							let elements=[]
							for(let key in row.Option.elements){
								let eleRow={
									text:key,
									value:row.Option.elements[key]
								}
								elements.push(eleRow)
							}
							row.Option.elements=JSON.parse(JSON.stringify(elements))
						}
						return row
					})
					this.deviceBasicInfoObj.Tag = {
						name: '标签信息',
						type: 2,
						canEdit: false,
						value: res.data
					}
					if (checkPermi(['/IoTService/IotProduct/ListPage'])) {
						this.deviceBasicInfoObj.Tag.canEdit=true
					}else{
						this.deviceBasicInfoObj.Tag.canEdit=false
					}
				}).catch(e => {
					this.setMsgTop(e)
				})
			},

			changeTab() {
				// tab切换
				// console.log("切换了吗？");
				if (this.current == 4) {

				}
			},
			getTopNavHei() {
				//获取页面头部导航的高度
				// let dataHeight=this.$refs.detail_top.$el.clientHeight
				// this.zhanweiHei = dataHeight
				// this.zhanweiHeiNumber = dataHeight * 2
				const query = uni.createSelectorQuery().in(this);
				query.select('#detail_top').boundingClientRect(data => {
					// console.log(data,'datadata');
					data.height = data.height
					this.zhanweiHei = data.height
					this.zhanweiHeiNumber = data.height * 2
					setTimeout(()=>{
						const query = uni.createSelectorQuery().in(this);
						query.select('#detail_top').boundingClientRect(data => {
							// console.log(data,'datadata');
							data.height = data.height
							this.zhanweiHei = data.height
							this.zhanweiHeiNumber = data.height * 2
							// console.log("this.zhanweiHei", this.zhanweiHei, this.zhanweiHeiNumber);
							this.$forceUpdate()
						}).exec();
					},500)
					// console.log("this.zhanweiHei", this.zhanweiHei, this.zhanweiHeiNumber);
					this.$forceUpdate()
				}).exec();
			
			},
		}
	}
</script>

<style lang="less" scoped>
	.relative_right {
		.right_ablist {
			position: absolute;
			z-index: 999;
			color: #333333;
			/* #ifndef MP-WEIXIN */
			right: 0;
			/* #endif */
			/* #ifdef MP-WEIXIN */
			right: auto;
			left: 0;
			/* #endif */
			top: 60rpx;
			box-shadow: 0rpx 4rpx 16rpx 0rpx rgba(0, 0, 0, 0.06);
			background: #FFFFFF;
			border-radius: 10rpx;

			.abli {
				width: 172rpx;
				height: 80rpx;
				display: flex;
				justify-content: center;
				align-items: center;

				.text {
					font-size: 28rpx;
					margin-left: 16rpx;
				}
			}
		}
	}

	.empty_con {
		display: flex;
		justify-content: center;
		align-items: center;
		flex-direction: column;
		margin-top: 200rpx;

		.image {
			width: 246rpx;
			height: 200rpx;
		}

		.text {
			font-size: 28rpx;
			color: rgba(153, 153, 153, 1);
			margin-top: 60rpx;
		}
	}


	.info_con {
		width: 100%;
		padding: 0 20rpx;
		box-sizing: border-box;
		padding-bottom: 50rpx;
		// margin-top: 60rpx;
	}
</style>