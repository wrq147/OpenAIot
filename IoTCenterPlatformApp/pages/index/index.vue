<template>
	<view class="pages_con pages_bgcon" @click="handleDocumentClick">
		<!--#ifdef H5 -->
		<jump-down></jump-down>
		<!--#endif -->
		<search-compt :statusbar="true" @closeSearch="closeSearchDiv" @searching="searching" pal="请输入设备名称或编码"
			:fixed="true" :isNoBg="false" backgroundColor="#fff" inputBg="#F8F8F8" :isRightText="true"
			v-if="showDeviceSearch" :hasKey="key"></search-compt>
		<view class="fiexbgzhanwei" v-if="showDeviceSearch" @click.stop="closeSearchDiv"></view>

		<top-weather :isfixed="!showDeviceSearch" :hasHandleIcon="!showDeviceSearch" ref="topWeather" :userName="userInfo.name" :statusBarHeight="statusBarHeight"
			:isShowFixedTitle="isShowFixedTitle" :defaultTop="defaultTop" @openLeftPopup="openLeftPopup">
			<template v-slot:default>
				<view>
					<text @click.stop="filterSet('')">您有 {{ totalDeviceNum.TotalCount }} 台设备</text>，<text @click.stop="filterSet(1)">在线{{ totalDeviceNum.OnlineCount }} 台</text>，<text @click.stop="filterSet(0)">离线 {{ totalDeviceNum.OfflineCount }}</text> 台
				</view>
			</template>
			<template v-slot:title_right>
				<view class="title_right">
					<view class="icon_con" @click.stop="openDeviceSearch">
						<custom-icons iconsName="icon-sousuo" iconsSize="36rpx" iconsColor="#333"></custom-icons>
					</view>
					<view class="icon_con" style="margin-left: 36rpx" @click.stop="scanAddDevice">
						<custom-icons iconsName="icon-tianjia" iconsSize="36rpx" iconsColor="#333"></custom-icons>
					</view>
				</view>
			</template>
			<template v-slot:footercontent>
				<view class="device_nav" id="minscrolldom" :style="{ opacity: isShowFixedTitle ? 0 : 1 }">
					<view style="width: 100%; height: 24rpx"></view>
					<view class="name_alarm">
						<view class="name" @click="allIsShowClick">
							<view class="" style="display: flex; align-items: center">
								{{ allText }}
								<custom-icons style="margin-left: 16rpx" iconsName="icon-xialasanjiao" iconsSize="14rpx"
									iconsColor="#333"></custom-icons>
							</view>
							<view class="name_bg"></view>
						</view>
						<view class="alarm" @click.stop="toAlarmList">
							<custom-icons iconsName="icon-baojingjilu" iconsSize="36rpx"></custom-icons>
							<view class="dot" v-if="aralmTotal > 0">{{ aralmTotal }}</view>
						</view>
					</view>
					<view class="nav">
						<v-tabs v-model="current" :scroll="true" :tabs="tabarList" color="#999999" activeColor="#333333"
							@change="changeTab" fontSize="28rpx" bold bgColor="rgba(0, 0, 0, 0)" lineHeight="0"
							:hasBorder="false" :zIndex="1" scrollConWid="100%"></v-tabs>
					</view>
					<view style="width: 100%">
						<devStatus :defaultNetTitle="defaultNetTitle" ref="devStatus" :current="current" @onLongPress="onLongPress" @getOnline="getOnline"
							@getDState="getDState"></devStatus>
					</view>
					<view class="search_result" v-if="key">已为您搜索“{{ key }}”</view>
				</view>
			</template>
		</top-weather>
		<view v-if="isShowFixedTitle" class="fixed_top" :style="{
                'padding-top': Number(statusBarHeight * 2) + 'rpx',
                'padding-bottom': '10rpx',
            }">
			<view class="device_nav" id="minscrolldom">
				<view style="width: 100%; height: 24rpx"></view>
				<view class="name_alarm">
					<view class="name">
						<text>我的设备</text>
					</view>
					<view class="search_alarm">
						<view class="icon_con" @click.stop="openDeviceSearch">
							<custom-icons iconsName="icon-sousuo" iconsSize="36rpx" iconsColor="#333"></custom-icons>
						</view>
						<view class="alarm" @click.stop="toAlarmList">
							<custom-icons iconsName="icon-baojingjilu" iconsSize="36rpx"></custom-icons>
							<view class="dot" v-if="aralmTotal > 0">{{ aralmTotal }}</view>
						</view>
					</view>
				</view>
				<view class="nav">
					<v-tabs v-model="current" :scroll="true" :tabs="tabarList" color="#999999" activeColor="#333333"
						@change="changeTab" fontSize="28rpx" bold bgColor="rgba(0, 0, 0, 0)" lineHeight="1rpx"
						lineColor="rgba(0, 0, 0, 0)" :hasBorder="false" :zIndex="1" scrollConWid="100%"></v-tabs>
					<view style="width: 100%">
						<devStatus ref="devStatus" :current="current" @onLongPress="onLongPress" @getOnline="getOnline"
							@getDState="getDState"></devStatus>
					</view>
				</view>
				<view class="search_result" v-if="key">已为您搜索“{{ key }}”</view>
			</view>
		</view>
		<view class="device_list_con" id="device_liscon"
			v-if="(deviceTableData && deviceTableData.length > 0) || status == 'loading'">
			<view class="device_list_flex">
				<view class="list_li" @click="toDeviceDetail(row)" @longpress="onLongPress"
					v-for="row in deviceTableData">
					<view class="li_top">
						<image class="image" :src="row.PhotoUrl + '?wh=500x500'" mode="aspectFill" v-if="row.PhotoUrl">
						</image>
						<image class="image" :src="getSerVerUrl()+'/appimg/device_default.png'" mode="aspectFill" v-else-if="getSerVerUrl()"></image>
						<view class="device_state">
							<view class="online_status">
								<view v-if="isPerss">
									<view v-if="row.isSelect" class="icons_con t-icon-gouxuan1"
										style="width: 36rpx; height: 36rpx"></view>
									<custom-icons v-else iconsName="icon-weixuanzhong" iconsSize="36rpx"
										iconsColor="#EAEAEA"></custom-icons>
								</view>
								<view v-if="!isPerss" style="display: flex; align-items: center">
									<view style="display: flex; align-items: center" :class="{ offline: row.Online == 0, unKnow: row.Online == 2 }">
										<view class="dot"></view>
										<view class="text" v-if="row.Online == 0">离线</view>
										<view class="text" v-if="row.Online == 1">在线</view>
										<view class="text" v-if="row.Online == 2">未知</view>
									</view>
									<view class="default" style="display: flex; align-items: center;margin-left: 20rpx;" :class="{ maintenance: row.DState == '保养',normal: row.DState == '正常', repair: row.DState == '维修' }">
										<view class="text" v-if="row.DState=='正常'">{{row.DState}}</view>
										<view class="text" v-else-if="row.DState=='维修'">{{row.DState}}</view>
										<view class="text" v-else-if="row.DState=='保养'">{{row.DState}}</view>
										<view class="text" v-else>{{row.DState}}</view>
									</view>
								</view>
							</view>
							<view class="alarm_status" v-if="row.HavWarn">
								<custom-icons iconsName="icon-baojing" iconsSize="24rpx"
									iconsColor="rgba(255, 53, 53, 1)"></custom-icons>
							</view>
						</view>
					</view>
					<view class="li_name_group">
						<view class="dev_name">
							{{ row.Name }}
						</view>
						<view class="group" v-if="row.DeviceNumber">
							<text class="group_name" v-if="row.DeviceNumber">
								编号：{{ row.DeviceNumber }}
							</text>
						</view>
						<view class="group" v-if="row.GroupName || row.ProductName">
							<text class="group_name" v-if="row.GroupName">
								{{ row.GroupName }}
							</text>
							<view class="line" v-if="row.GroupName && row.ProductName"></view>
							<text class="group_name" v-if="row.ProductName">
								{{ row.ProductName }}
							</text>
						</view>
					</view>
				</view>
			</view>

			<uni-load-more iconType="circle" :status="status" v-if="status" />
		</view>
		<view class="empty_con" v-else>
			<view class="empty_image">
				<image v-if="getSerVerUrl()" class="image" :src="getSerVerUrl()+'/appimg/no_data.png'" mode=""></image>
			</view>
			<view class="empty_text">点击右上角“+”添加设备</view>
		</view>
		<msg-prompt ref="promptMsg" @confirm="confirmToNet"></msg-prompt>
		<my-tab-bar active="Machines" ref="mytab"></my-tab-bar>
		<!-- 切换企业的弹窗 -->
		<org-change ref="notificationcmt" :orgList="orgList" :orgId="userInfo.orgId" :userInfo="userInfo"></org-change>

		<!-- 点击全部弹窗 -->
		<allPop ref="allPop" v-show="allIsShow" @windowClose="windowClose" @roomChange="roomChange" />
		<!-- 长按设备显示 -->
		<pressPop ref="pressPop" v-show="isPerss" :deviceTableData="deviceTableData" :current="current"
			:roomId="querydata.RoomId" @windowClose="windowClose" @devChange="devChange"
			@getDeviceList="getDeviceList" />
	</view>
</template>

<script>
	import allPop from './common/allPop';
	import pressPop from './common/pressPop';
	import devStatus from './common/devStatus';
	import {
		getUserInfo
	} from '@/api/login';
	import {
		crmDeviceList,
		addUseDevice,
		warningList,
		getUpdate,
		deviceStatistics,
		roomList,
		InfoOfDtuId,
	} from '@/api/device.js';
	import mixin from '@/mixins/mixin.js';
	export default {
		mixins: [mixin],
		components: {
			allPop,
			pressPop,
			devStatus,
		},
		data() {
			return {
				aralmTotal: 0, //报警未处理数量
				// topTitle: '我的设备',
				statusBarHeight: uni.getSystemInfoSync().statusBarHeight,
				selectListParam: [{
					name: '状态',
					params: 'Online',
					pal: '请选择设备状态',
					value: null,
					localdata: [{
							text: '离线',
							value: 0,
						},
						{
							text: '在线',
							value: 1,
						},
						{
							text: '未知',
							value: 2,
						},
					],
				}, ],
				querydata: {
					pageNum: 1,
					pageSize: 30,
					RoomId: '',
					RoomCategory: '',
					Online: '',
					ShowWarn: true, //是否显示报警
					FilterRoom: false,
					DState: '',
					// Name: ""
				}, //过滤参数
				statusList: [{
						text: '离线',
						value: 0,
					},
					{
						text: '在线',
						value: 1,
					},
				],
				deviceTableData: [],
				key: '', //搜索关键词
				status: 'loading',
				scanId: '',
				isDownRefresh: false,
				activeDevStatus: 0, //过滤的设备状态
				isShowFixedTitle: false,
				showDeviceSearch: false,
				userInfo: {},
				totalDeviceNum: {}, //设备分类数量
				defaultTop: 0,
				timer: null,
				orgList: [],
				isShowDraw: false,
				allIsShow: false, // 点击全部弹窗
				navList: [], // 头部导航
				tabarList: ['全屋', '未分配'],
				current: 0,
				isPerss: false, // 长按设备弹窗
				allText: '全部',
				roomChangeId: '', // 车间分类改变时存储id
				isRoomChange: false,
				hasNetWork: false,
				defaultNetTitle:'',//设备状态过滤传值
			};
		},
		async onLoad() {
			try{
				if (this.$store.state.user && this.$store.state.user.uid) {
					this.userInfo = this.$store.state.user;
				} else {
					await this.$store.dispatch('GetInfo');
					this.userInfo = this.$store.state.user;
				}
				this.$store.commit('orgLis/SET_ORG_LIST', null);
				this.orgList = await this.$store.dispatch('orgLis/setOrgList');
				this.$nextTick(() => {
					this.$refs.mytab.loadCheck();
					let query = uni.createSelectorQuery().in(this);
					query
						.select('#device_liscon')
						.boundingClientRect((data) => {
							if (data) {
								this.defaultTop = data.top;
							}
						})
						.exec();
				});
				
				this.getDeviceStatistics();
				this.getRoomList('');
				// this.getWeather()//获取天气相关
				
				setTimeout(async () => {
					await this.getUpdateDevice();
				}, 500);
			}catch(err){
				if(err.code==400||err.code==50009){
					this.$refs.promptMsg.open(err.cusMsg, 3000)
					// #ifndef MP-WEIXIN
					var iptcode = this.getUrlParam('code') || "";
					if(iptcode){
						uni.reLaunch({
							url: '/page_register/login',
							// #ifdef APP-PLUS
							success: () => {
								plus.navigator.closeSplashscreen();
							},
							// #endif
						})
					}else{
						jumpLogin()
					}
					// #endif
					// #ifdef MP-WEIXIN
					jumpLogin()
					// #endif
				}
			}
		},
		onShow() {
			if (this.$store.state.isLoadDevice) {
				this.querydata.pageNum = 1;
				this.status = 'loading';
				if (this.isRoomChange) {
					this.getRoomList(this.roomChangeId);
				}
			}


			this.getAlarmList();
			this.$nextTick(() => {
				if (this.$store.state.isReloadMessage) {
					this.$refs.mytab.getNoReadCount();
				}
			});
		},
		beforeDestroy() {
			if (this.timer) {
				clearInterval(this.timer);
			}
		},
		onUnload() {
			if (this.timer) {
				clearInterval(this.timer);
			}
		},
		onPullDownRefresh() {
			this.querydata.pageNum = 1;
			this.status = 'loading';
			this.isDownRefresh = true;
			this.getDeviceList();
		},
		onReachBottom() {
			//上拉触底
			if (this.status != 'noMore') {
				this.querydata.pageNum++;
				this.status = 'loading';
				this.getDeviceList();
			}
		},
		onPageScroll() {
			this.getElementTop('');
		},
		onBackPress(options) {
			if (options.from === 'backbutton') {
				// 来自于导航条返回按钮或者系统返回按钮
				// 返回 `true` 表示拦截操作，不再继续执行返回操作
				// 返回 `false` 表示不拦截返回操作，继续执行默认的返回操作
				if (this.$refs.notificationcmt.isShowDraw) {
					this.$refs.notificationcmt.closeLeftPopup();
					return true; // 根据需要返回true或false
				} else {
					return false; // 根据需要返回true或false
				}
			}
		},
		methods: {
			filterSet(val){
				//手动设置默认的设备状态过滤
				this.defaultNetTitle=val
			},
			handleDocumentClick() {
				if (this.$refs.devStatus) {
					this.$refs.devStatus.handleDocumentClick();
				}
			},
			// 获取车间列表
			getRoomList(type) {
				this.tabarList = ['全屋', '未分配'];
				const data = {
					TargetOrgId: this.$store.state.user.orgId,
					CategoryId: type,
				};
				roomList(data)
					.then((res) => {
						this.navList = res.data;
						res.data.forEach((item) => {
							this.tabarList.push(item.Name);
						});
						this.getDeviceList();
						this.isRoomChange = true;
					})
					.catch((err) => {
						this.setMsgTop(err);
					});
			},
			openLeftPopup() {
				//打开左侧切换企业弹窗
				this.$refs.notificationcmt.openLeftPopup();
			},

			openDeviceSearch() {
				//打开设备搜索
				this.showDeviceSearch = true;
			},
			closeSearchDiv() {
				//隐藏设备搜索
				this.showDeviceSearch = false;
			},
			getElementTop() {
				//获取距离顶部的距离
				let minScrollhei = 70;
				let query = uni.createSelectorQuery().in(this);
				//需要给黄色区域设置一个id标识，在这里是demo
				query
					.select('#minscrolldom')
					.boundingClientRect((data2) => {
						// console.log(data2,'data2data2data2');
						if (data2) {
							minScrollhei = data2.height;
						}
					})
					.exec();
				query
					.select('#device_liscon')
					.boundingClientRect((data) => {
						// console.log('wwwwww', data.top, this.statusBarHeight, Number(this.defaultTop - this
						// 	.statusBarHeight)) //这个就是距离顶部的高度
						if (data) {
							if (data.top <= this.statusBarHeight + minScrollhei) {
								this.isShowFixedTitle = true;
							} else {
								this.isShowFixedTitle = false;
							}
							this.$refs.topWeather.setDomOpac(data);
						}
					})
					.exec();
			},

			getDeviceStatistics() {
				//获取设备统计信息
				deviceStatistics()
					.then((res) => {
						let data = res.data;
						this.totalDeviceNum = data;
					})
					.catch((err) => {
						this.setMsgTop(err);
					});
			},
			setDeviceStatus(val) {
				this.activeDevStatus = val;
				this.querydata.pageNum = 1;
				this.status = 'loading';
				// this.deviceTableData = []
				if (this.activeDevStatus) {
					if (this.activeDevStatus == 1) {
						this.querydata.Online = 1;
					}
					if (this.activeDevStatus == 2) {
						this.querydata.Online = 0;
					}
					if (this.activeDevStatus == 3) {
						this.querydata.Online = 2;
					}
				} else {
					delete this.querydata.Online;
				}
				this.getDeviceList();
			},

			async getUpdateDevice() {
				//获取升级设备
				try {
					let res = await getUpdate();
					let arr = res.data.split(',');
					this.$store.commit('SET_UPDATE_DEVICE', arr);
				} catch (e) {
					//TODO handle the exception
					this.setMsgTop(e);
				}
			},
			getAlarmList() {
				warningList({
						Status: 0,
						pageNum: 1,
						pageSize: 5,
					})
					.then((res) => {
						this.aralmTotal = res.data.Total;
					})
					.catch((err) => {
						this.setMsgTop(err);
					});
			},
			loadList() {
				//加载列表的方法
				this.querydata.pageNum = 1;
				this.status = 'loading';
				this.getDeviceList();
			},
			searching(val) {
				this.key = val;
				this.closeSearchDiv();
				if (this.key) {
					this.querydata.Key = this.key;
					this.querydata.pageNum = 1;
					this.status = 'loading';
					this.deviceTableData = [];
					this.getDeviceList();
				} else {
					delete this.querydata.Key;
					this.querydata.pageNum = 1;
					this.status = 'loading';
					this.deviceTableData = [];
					this.getDeviceList();
				}
			},
			getDeviceList(query) {
				//获取设备列表
				if (query && query == 'unbind') {
					this.querydata.pageNum = 1;
				}
				if (!this.querydata.Name) {
					delete this.querydata.Name;
				}
				crmDeviceList(this.querydata)
					.then((res) => {
						if (this.querydata.pageNum == 1) {
							this.deviceTableData = [];
						}
						res.data.List.forEach((item) => {
							item.isSelect = false;
						});
						this.deviceTableData = [...this.deviceTableData, ...res.data.List];
						// this.topTitle = '我的设备（' + res.data.Total + '）';
						if (res.data.List.length < this.querydata.pageSize) {
							this.status = 'noMore';
						} else {
							this.status = 'more';
						}
						if (this.isDownRefresh) {
							uni.stopPullDownRefresh();
							this.isDownRefresh = false;
						}
						this.$store.commit('SET_DEVICE_LIST', false);
					})
					.catch((err) => {
						this.status = 'noMore';
						if (this.isDownRefresh) {
							uni.stopPullDownRefresh();
							this.isDownRefresh = false;
						}
						this.setMsgTop(err);
					});
			},
			toDeviceDetail(row) {
				if (this.isPerss) {
					row.isSelect = !row.isSelect;
				} else {
					//跳转至设备详情
					uni.navigateTo({
						url: '/pages_device/device_info/device_info?id=' + row.Id,
					});
				}
			},
			scanAddDevice() {
				//扫码添加设备
				let that = this;
				uni.scanCode({
					success: (res) => {
						let tmpidx = res.result.lastIndexOf('iot');
						if (tmpidx > -1) {
							that.$refs.promptMsg.loadingOpen('添加中...');
							let id = res.result.substring(tmpidx);
							this.scanId = id;
							InfoOfDtuId({
									dtuId: id,
								})
								.then((res1) => {
									if (
										(res1.data &&
											this.$store.state.user.orgId == res1.data.OrgId) ||
										(res1.data &&
											this.$store.state.user.orgId == res1.data.OwnerOrgId) ||
										(res1.data &&
											this.$store.state.user.orgId == res1.data.UseOrgId)
									) {
										that.$refs.promptMsg.loadingColse();
										if (this.hasNetWork) {
											this.$refs.promptMsg.noticeOpen2(
												'是否去进行设备配网?',
												'系统消息',
												true
											);
										} else {
											that.$refs.promptMsg.open('设备已添加', 1000);
											setTimeout(() => {
												uni.navigateTo({
													url: '/pages_device/device_info/device_info?id=' +
														res1.data.Id,
												});
											}, 1000)
										}

									} else {
										addUseDevice(id)
											.then((res2) => {
												that.$refs.promptMsg.loadingColse();
												if (res2.code == 0) {
													that.$refs.promptMsg.open('添加成功', 2000);
													that.querydata.pageNum = 1;
													that.status = 'loading';
													that.getDeviceList();
													if (this.hasNetWork) {
														this.$refs.promptMsg.noticeOpen2(
															'是否去进行设备配网?',
															'系统消息',
															true
														);
													}
												}
											})
											.catch((err) => {
												that.$refs.promptMsg.loadingColse();
												that.setMsgTop(err);
											});
									}
								})
								.catch((err) => {
									that.$refs.promptMsg.loadingColse();
									this.setMsgTop(err);
								});
						} else {
							this.$refs.promptMsg.open('二维码无效', 2000);
						}
					},
				});
			},
			confirmToNet() {
				//去配网
				uni.navigateTo({
					url: '/pages_device/wifi-conf/wifi-conf?dtuId=' + this.scanId,
				});
			},
			toAlarmList() {
				uni.navigateTo({
					url: '/pages_device/alarm/list',
				});
			},
			change() {},
			openSelect() {
				this.$refs.selectCompt.openSelect();
			},
			onScanWifiConfig() {
				//配网扫码总入口
				uni.scanCode({
					success: function(res) {
						let tmpidx = res.result.indexOf('iot');
						if (tmpidx > -1) {
							let id = res.result.substring(tmpidx);
							uni.navigateTo({
								url: '/pages_device/wifi-conf/wifi-conf?dtuId=' + id,
							});
						}
					},
				});
			},
			selectFinsh(query) {
				this.querydata = JSON.parse(JSON.stringify(query));
				this.querydata.pageNum = 1;
				this.status = 'loading';
				this.deviceTableData = [];
				this.getDeviceList();
			},
			// 打开全部弹窗
			allIsShowClick() {
				this.allIsShow = true;
				this.isPerss = false;
				this.$refs.allPop.getList();
			},
			// 关闭蒙版弹窗
			windowClose() {
				this.allIsShow = false;
				this.isPerss = false;
			},
			// 车间分类改变事件
			roomChange(id, text) {
				this.roomChangeId = id;
				this.allText = text;
				this.querydata.pageNum = 1;
				this.querydata.RoomCategory = id;
				this.querydata.RoomId = '';
				this.querydata.FilterRoom = false;
				this.querydata.DState = '';
				this.querydata.Online = '';
				this.current = 0;
				this.getRoomList(id);
				this.getDeviceList();
			},
			// 点击车间导航事件
			changeTab(index) {
				this.querydata.pageNum = 1;
				if (index === 0) {
					this.querydata.RoomId = '';
					this.querydata.FilterRoom = false;
					this.isPerss = false;
				} else if (index === 1) {
					this.querydata.RoomId = '';
					this.querydata.FilterRoom = true;
				} else {
					this.querydata.FilterRoom = false;
					this.querydata.RoomId = this.navList[index - 2].Id;
				}
				this.querydata.DState = '';
				this.querydata.Online = '';
				this.getDeviceList();
			},
			// 长按设备事件
			onLongPress() {
				if (this.current !== 0) {
					this.isPerss = !this.isPerss;
					this.$refs.pressPop.getList();
				} else {
					this.isPerss = false;
				}
			},
			// 网络状态切换
			getOnline(id) {
				this.querydata.Online = id;
				this.querydata.pageNum = 1;
				this.getDeviceList();
			},
			// 运行状态切换
			getDState(value) {
				this.querydata.DState = value;
				this.querydata.pageNum = 1;
				this.getDeviceList();
			},
			// 全选设备/ 取消全选
			devChange(isCheckAll) {
				this.deviceTableData.forEach((item) => {
					item.isSelect = isCheckAll;
				});
			},
		},
	};
</script>

<style lang="less" scoped>
	.fiexbgzhanwei {
		width: 100%;
		height: 100vh;
		position: fixed;
		top: 0;
		left: 0;
		z-index: 9;
		background: rgba(0, 0, 0, 0.7);
	}

	.device_nav {
		.name_alarm {
			display: flex;
			justify-content: space-between;
			align-items: flex-start;
			line-height: 36rpx;
			.search_alarm{
				display: flex;
				justify-content: flex-end;
				align-items: center;
				.icon_con{
					margin-right: 30rpx;
				}
			}
			.name {
				font-size: 36rpx;
				color: #333333;
				position: relative;
				font-weight: bold;

				.name_bg {
					position: absolute;
					left: 0;
					bottom: -6rpx;
					width: 112rpx;
					height: 20rpx;
					background: linear-gradient(90deg,
							rgba(35, 113, 255, 0.5) 0%,
							rgba(35, 113, 255, 0) 100%);
					border-radius: 10px;
				}
			}

			.alarm {
				position: relative;
				width: 36rpx;
				height: 36rpx;

				.dot {
					position: absolute;
					right: -14rpx;
					top: -14rpx;
					width: 30rpx;
					height: 30rpx;
					border-radius: 50%;
					color: #ffffff;
					font-size: 20rpx;
					background-color: #ff3535;
					display: flex;
					justify-content: center;
					align-items: center;
				}
			}
		}

		.nav {
			margin-top: 20rpx;
			.nav_li {
				line-height: 60rpx;
				font-size: 28rpx;
				margin-right: 80rpx;
				color: #999999;

				&.active {
					color: #333;
					font-weight: bold;
				}
			}
		}

		.search_result {
			line-height: 39rpx;
			padding: 23.5rpx 0 3.5rpx;
			font-size: 26rpx;
			color: #999999;
		}
	}

	.fixed_top_zhanwei {
		width: 100%;
	}

	.fixed_top {
		position: fixed;
		top: 0;
		left: 0;
		background-color: #ffffff;
		width: 100%;
		z-index: 3;
		padding: 24rpx 30rpx 0;
		box-sizing: border-box;
		opacity: 1;
		transition: 2000;

		.device_nav {
			.search_result {
				line-height: 39rpx;
				padding: 23.5rpx 0 3.5rpx;
				font-size: 26rpx;
				color: #999999;
			}
		}
	}

	.device_list_con {
		position: relative;

		::v-deep.uni-load-more {
			.uni-load-more__text {
				z-index: 1;
			}
		}
	}
</style>