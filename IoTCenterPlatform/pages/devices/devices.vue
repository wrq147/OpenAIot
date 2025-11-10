<template>
	<view class="pages_con">
		<top :title="topTitle" leftWidth="0rpx" :titleIsLeft="true" :isNoLeftPadding="true" backgroundColor="#161A26"
			:rightWidth="120">
			<template v-slot:top_right>
				<view class="right_top right1" @click="toAlarmList">
					<custom-icons iconsName="icon-baojingjilu" iconsSize="36rpx"></custom-icons>
					<view class="dot" v-if="aralmTotal>0"></view>
				</view>
				<view class="right_top" @click="scanAddDevice">
					<custom-icons iconsName="icon-tianjia" iconsSize="36rpx"></custom-icons>
				</view>
			</template>
		</top>
		<search-compt @openSelect="openSelect" @searching="searching"
			pal="Please enter the name or number"></search-compt>
		<select-compt ref="selectCompt" :selectListParam="selectListParam" :querydata="querydata"
			@selectFinsh="selectFinsh"></select-compt>
		<view class="device_list_con">
			<!-- <view class="netWork_con" @click="onScanWifiConfig">
				<custom-icons iconsName="icon-saoma" iconsSize="30rpx"></custom-icons>
				<view class="text">Distribution Network</view>
			</view> -->
			<view class="device_list">
				<view class="list_li" @click="toDeviceDetail(row)" v-for="row in deviceTableData">
					<view class="li_left">
						<image class="image" :src="row.PhotoUrl+'?wh=500x500'" mode="aspectFill" v-if="row.PhotoUrl">
						</image>
						<image class="image" src="../../static/device_default.png" mode="aspectFill" v-else></image>
					</view>
					<view class="li_right">
						<view class="dev_name">
							{{row.Name}}
						</view>
						<view class="group">
							{{row.GroupName}}
						</view>
						<view class="device_state">
							<view class="online_status" :class="{'offline':row.Online==0,'unKnow':row.Online==2}">
								<view class="dot"></view>
								<view class="text" v-if="row.Online==0">offline</view>
								<view class="text" v-if="row.Online==1">Online</view>
								<view class="text" v-if="row.Online==2">unKnow</view>
							</view>
							<view class="alarm_status" v-if="row.HavWarn">
								<custom-icons iconsName="icon-baojing" iconsSize="30rpx"
									iconsColor="rgba(255, 53, 53, 1)"></custom-icons>
							</view>
						</view>
					</view>
				</view>
				<uni-load-more iconType="circle" :status="status" v-if="status" />
			</view>
		</view>
		<msg-prompt ref="promptMsg" @confirm='confirmToNet'></msg-prompt>
		<my-tab-bar active="Machines" ref="mytab"></my-tab-bar>

		<!-- 提示信息组件 -->
	</view>
</template>

<script>
	import {
		crmDeviceList,
		addUseDevice,
		warningList,
		getUpdate
	} from '@/api/device.js'
	import {
		getUserInfo,
	} from "@/api/login";
	import {
		InfoOfDtuId
	} from '@/api/device.js'
	import mixin from '@/mixins/mixin.js'
	import {
		SwitchEnterprise
	} from "@/api/personalCenter";
	export default {
		mixins: [mixin],
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
					Online: null,
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
				scanId: '',
				isDownRefresh:false,
				aralmTotal:0
			}
		},
		async onLoad(options) {
			// console.log("设备页面");
			this.$nextTick(() => {
				this.$refs.mytab.loadCheck()
			})
			if(this.$store.state.user.roles&&this.$store.state.user.roles.length>0){
			}else{
				let rsp = await getUserInfo({
					id: 0
				})
				await this.$store.dispatch('GetInfo')
				this.$store.commit('SET_ROLES',rsp.data.user.roleIds)
				this.$refs.mytab.loadCheck()
			}
			this.getDeviceList()
			setTimeout(()=>{
				this.getUpdateDevice()
				this.$nextTick(()=>{
					if(options.network){
						this.$refs.promptMsg.noticeOpen2(
							'Do you want to go to the distribution network?',
							'system prompt', true)
					}
				})
			},500)
		},
		onShow() {
			if (this.$store.state.isLoadDevice) {
				this.querydata.pageNum = 1
				this.status = "loading";
				this.getDeviceList()
			}
			this.getAlarmList()
		},
		onPullDownRefresh() {
			this.querydata.pageNum = 1
			this.status = "loading";
			this.isDownRefresh=true
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
			async getUpdateDevice(){
				//获取升级设备
				try{
					let res=await getUpdate()
					// console.log("通过升级的设备",res);
					let arr=res.data.split(',')
					// console.log("通过升级的设备列表",arr);
					this.$store.commit('SET_UPDATE_DEVICE', arr)
				}catch(e){
					//TODO handle the exception
					this.setMsgTop(e)
				}
			},
			getAlarmList(){
				warningList({
					Status:0,
					pageNum:1,
					pageSize:5
				}).then(res=>{
					console.log("报警，未处理",res);
					this.aralmTotal=res.data.Total
				}).catch(err=>{
					this.setMsgTop(err)
				})
			},
			loadList() {
				//加载列表的方法
				this.querydata.pageNum = 1
				this.status = "loading";
				this.getDeviceList()
			},
			searching(val) {
				this.key = val
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
			},
			getDeviceList(query) {
				//获取设备列表
				if (query && query == 'unbind') {
					this.querydata.pageNum = 1
				}
				if (!this.querydata.Name) {
					delete this.querydata.Name
				}
				crmDeviceList(this.querydata).then(res => {
					if (this.querydata.pageNum == 1) {
						this.deviceTableData = []
					}
					this.deviceTableData = [...this.deviceTableData, ...res.data.List];
					this.topTitle = 'Machines（' + res.data.Total + '）';
					if (res.data.List.length < this.querydata.pageSize) {
						this.status = 'noMore';
					} else {
						this.status = 'more';
					}
					if(this.isDownRefresh){
						uni.stopPullDownRefresh()
						this.isDownRefresh=false
					}
					this.$store.commit('SET_DEVICE_LIST', false)
				}).catch(err => {
					this.status = 'noMore';
					if(this.isDownRefresh){
						uni.stopPullDownRefresh()
						this.isDownRefresh=false
					}
					this.setMsgTop(err)
				})
			},
			toDeviceDetail(row) {
				//跳转至设备详情
				uni.navigateTo({
					url: '/pages_device/device_info/device_info?id=' + row.Id
				})
			},
			scanAddDevice() {
				//扫码添加设备
				let that = this
				uni.scanCode({
					success: (res) => {
						// console.log("扫码结果",res);
						let tmpidx = res.result.lastIndexOf("iot");
						if (tmpidx > -1) {
							that.$refs.promptMsg.loadingOpen('Adding...')
							let id = res.result.substring(tmpidx);
							this.scanId = id
							InfoOfDtuId({
								dtuId: id
							}).then(res1 => {
								console.log("dtu", res1, this.$store.state.user.orgId);
								addUseDevice(id).then(res2 => {
									that.$refs.promptMsg.loadingColse()
									if (res2.code == 0) {
										that.$refs.promptMsg.open('Added successfully',
											2000)
										that.querydata.pageNum = 1
										that.status = 'loading';
										that.getDeviceList()
										this.$refs.promptMsg.noticeOpen2(
											'Do you want to go to the distribution network?',
											'system prompt', true)
									}
								}).catch(err => {
									that.$refs.promptMsg.loadingColse()
									if(err.code==1){
										if(err.data){
											this.handSelect(err.data)
										}
									}
									that.setMsgTop(err)
								})
							}).catch(err => {
								console.log("错误", err);
								that.$refs.promptMsg.loadingColse()
								this.setMsgTop(err)
							})

						} else {
							this.$refs.promptMsg.open('Invalid QR code', 2000)
						}

					}
				});
			},
			confirmToNet() {
				//去配网
				uni.navigateTo({
					url: '/pages/wifi-conf/wifi-conf?dtuId=' + this.scanId
				});
			},
			toAlarmList() {
				uni.navigateTo({
					url: '/pages_device/alarm/list'
				})
			},
			handSelect(id) {
				SwitchEnterprise({
					id: id
				}).then(async (res) => {
					if (res.code == 0) {
						this.$store.commit('SET_ROLES', [])
						this.$store.commit('SET_PERMISSIONS', [])
						// this.dataInfo();
						this.$store.commit('SET_MESSAGE_INFO', true)
						//消息取消订阅
						this.$store.dispatch("mqttclient/getClient").then((client) => {
							let tkey = "user/" + this.$store.getters.uid + "/new";
							client.unsubscribe(tkey, (error) => {
								console.info("取消订阅dddd22", error)
								this.$store.commit("mqttclient/Del_Handler", tkey);
							});
						});
			
						this.$refs.showRight.close();
						this.isShowDraw = false
						// this.$refs.promptMsg.loadingOpen('Loading...')
						try { //切换企业后重新设置权限信息
							await this.$store.dispatch("GetInfo");
							// this.$refs.promptMsg.loadingColse()
							// this.$refs.mytab.loadCheck()
							this.$store.commit('SET_UID', '');
							uni.reLaunch({
								url: '/pages/index/other2?url=/pages/devices/devices?network=true'
							})
						} catch (e) {
							//TODO handle the exception
							console.log("eeeeeee", e);
							// this.$refs.promptMsg.loadingColse()
						}
			
					}
				}).catch((err) => {
					//console.log(err,'1111111')
					this.setMsgTop(err)
				})
			},
			openSelect() {
				this.$refs.selectCompt.openSelect()
			},
			onScanWifiConfig() {
				//配网扫码总入口
				uni.scanCode({
					success: function(res) {
						let tmpidx = res.result.indexOf("iot");
						if (tmpidx > -1) {
							let id = res.result.substring(tmpidx);
							uni.navigateTo({
								url: '/pages/wifi-conf/wifi-conf?dtuId=' + id
							});
						}

					}
				});
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
		background-color: rgba(22, 26, 38, 1);
	}
</style>