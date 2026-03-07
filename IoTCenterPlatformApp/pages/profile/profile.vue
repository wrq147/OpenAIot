<template>
	<view class="profile" :class="[isShowDraw?'profileActive':'profileOn']">
		<!--#ifdef H5 -->
		<jump-down></jump-down>
		<!--#endif -->
		<view class="modify-header" :style="{'padding-top':statusBarHeight*2+50+'rpx'}"
			:class="{'hide_top_info':isShowFixedTitle}">
			<view class="modify-header-top" :style="{'opacity': titleOpac}">
				<view class="modify-header-top-left">
					<view class="NameCutting" v-if="!userName&&!userName.length<=0">
						{{userName.length>2?userName.slice(-2):userName}}
					</view>
					<view v-else-if="!userInfo.Avatar||userInfo.Avatar&&userInfo.Avatar.indexOf('profile.png')>-1" class="modify-header-top-left-tou t-icon-morentouxiang1"></view>
					<view class="modify-header-top-left-tou" v-else>
						<image style="width: 100%;height:100%;border-radius: 50%;" :src="userInfo.Avatar" alt="">
						</image>
					</view>
					<view class="modify-header-top-left-text" @click="showDrawer">
						<view class="modify-header-top-left-text-content">
							<text class="name">{{userInfo.RealName}}</text>
							<!-- <text class="newt-icon-dajiantou"></text> -->
							<text class="newt-icon-dajiantou iconfont icon-a-youjiantoubai">
							</text>
						</view>
						<view class="text_name">
							{{userInfo.dept_name}}
						</view>
					</view>
				</view>
				<!--#ifndef MP-WEIXIN -->
				<view class="modify-header-top-right">
					<view class="iconfont icon-xiaoxizhongxin" @click="jumpToMessage" style="font-size:40rpx;"></view>
					<view class="message_num" v-if="noReadCountNum>0"></view>
				</view>
				<!--#endif -->
				<!--#ifdef MP-WEIXIN -->
				<view class="modify-header-top-right" style="margin-top: 50rpx;">
					<view class="iconfont icon-xiaoxizhongxin" @click="jumpToMessage" style="font-size:40rpx;"></view>
					<view class="message_num" v-if="noReadCountNum>0"></view>
				</view>
				<!--#endif -->
			</view>
			<!--#ifndef MP-WEIXIN -->
			<view class="modify-header-bottom" :style="{'opacity': weatherOpac}">
			<!--#endif -->
			<!--#ifdef MP-WEIXIN -->
			<view class="modify-header-bottom" :style="{'opacity': weatherOpac,'margin-top':'50rpx'}">
			<!--#endif -->
				<view class="modify-header-bottom-parse">
					<view class="modify-header-bottom-left" @click="handPersonInfo()">
						<view class="iconfont icon-bianji"></view>
						<view>编辑信息</view>
					</view>
				</view>
				<view class="modify-header-bottom-right">
					<image v-if="getSerVerUrl()" style="width:100%;height:100%;" :src="getSerVerUrl()+'/appimg/newImg/centerIllustration.png'" alt="">
					</image>
				</view>
			</view>
		</view>
		<view class="rules_title_fiex" id="minscrolldom" v-if="isShowFixedTitle"
			:style="{'height':Number(statusBarHeight*2)+88+'rpx','padding-top':Number(statusBarHeight*2)+'rpx'}">
			个人中心
		</view>

		<view id="rules_listcon" class="rules_list_con">
			<view class="ProcessManagement" v-if="isCheckPermi(['/FlowService/Flow'])" style="margin-top:0rpx;">
				<view class="ProcessManagement-title">
					<view class="ProcessManagement-title-left">
						<view class="iconfont t-icon-liuchengguanli">

						</view>
						<view class="text_content">
							流程管理
						</view>
					</view>

					<view class="InitiateProcess" @click="toCreateProcess">
						<text class="InitiateProcess-add iconfont  icon-tianjia">

						</text>
						<text>
							发起流程
						</text>

					</view>
				</view>
				<view class="ProcessManagement-content">
					<view class="ProcessManagement-content-item" @click="toProcessList(0)">
						<view class="num">
							{{Interface.PendingCount}}
						</view>
						<view class="text">
							待办任务
						</view>
					</view>

					<view class="ProcessManagement-content-item" @click="toProcessList(1)">
						<view class="num">
							{{Interface.FinishCount}}
						</view>
						<view class="text">
							已办任务
						</view>
					</view>

					<view class="ProcessManagement-content-item" @click="toProcessList(2)">
						<view class="num">
							{{Interface.CopyCount}}
						</view>
						<view class="text">
							抄送我的
						</view>
					</view>

					<view class="ProcessManagement-content-item" @click="toProcessList(3)">
						<view class="num">
							{{Interface.MyFlowCount}}
						</view>
						<view class="text">
							我的流程
						</view>
					</view>
				</view>
				<!-- <view class="ProcessManagement-CreateProcess" @click="toCreateProcess">
				+ 新增流程
			</view> -->
			</view>
			<!--仓储管理-->
			<view class="ProcessManagement" style="margin-top:0rpx;" v-if="isCheckPermi(['/Stock/'])">
				<view class="ProcessManagement-title">
					<view class="ProcessManagement-title-left">
						<view class="iconfont icon-kucun" style="margin-right:20rpx;font-size: 32rpx;">

						</view>
						<view class="text_content">
							仓储管理
						</view>
					</view>
				</view>
				<view class="ProcessManagement-content" style="margin:0rpx 45rpx;">

					<view class="ProcessManagement-content-text" @click="handInventory(-1)">
						<view class="num">
							<text class="num_text">{{inventory.totalCount}}</text>
							<text class="newt-icon-youjiantou iconfont icon-a-youjiantoubai">
							</text>
						</view>
						<view class="text">
							总库存
						</view>
					</view>



					<view class="ProcessManagement-content-text" @click="handInventory(1)">
						<view class="num">
							<text class="num_text">{{inventory.DevCount}}</text>
							<text class="newt-icon-youjiantou iconfont icon-a-youjiantoubai">
							</text>
						</view>
						<view class="text">
							成品
						</view>
					</view>



					<view class="ProcessManagement-content-text" @click="handInventory(2)">
						<view class="num">
							<text class="num_text">{{inventory.PartsCount}}</text>
							<text class="newt-icon-youjiantou iconfont icon-a-youjiantoubai">
							</text>
						</view>
						<view class="text">
							半成品
						</view>
					</view>

				</view>
				<!--管理-->
				<view class="ManageList">
					<view class="ManageList-item" @click="handWarehouse('Warehouse_management')">
						<image v-if="getSerVerUrl()" style="width: 100%;height:100%;" :src="getSerVerUrl()+'/appimg/newImg/WarehouseManagement.png'"
							alt=""></image>
					</view>
					<view class="ManageList-item" @click="handWarehouse('Checking_tasks')">
						<image v-if="getSerVerUrl()" style="width: 100%;height:100%;" :src="getSerVerUrl()+'/appimg/newImg/InventoryTasks.png'" alt="">
						</image>
					</view>
					<view class="ManageList-item_con">
						<view class="ManageList-item" @click="handWarehouse('Warehousing_records')">
							<image v-if="getSerVerUrl()" style="width: 100%;height:100%;" :src="getSerVerUrl()+'/appimg/newImg/StorageRecords.png'" alt="">
							</image>
						</view>
						<view class="ManageList-item" @click="handWarehouse('Outbound_records')">
							<image v-if="getSerVerUrl()" style="width: 100%;height:100%;" :src="getSerVerUrl()+'/appimg/newImg/OutboundRecords.png'"
								alt="">
							</image>
						</view>
						<view class="ManageList-item" @click="handWarehouse('Outbound_apply')">
							<image v-if="getSerVerUrl()" style="width: 100%;height:100%;" :src="getSerVerUrl()+'/appimg/newImg/outApply.png'" alt="">
							</image>
						</view>
					</view>

				</view>
			</view>
			<!--我的工厂-->
			<view class="ProcessManagement" style="margin-top:0rpx;" v-if="isCheckPermi(['/AgentMan/'])">

				<view class="ProcessManagement-title">
					<view class="ProcessManagement-title-left">
						<view class="iconfont icon-wodegongchang" style="margin-right:20rpx;font-size: 32rpx;">

						</view>
						<view class="text_content">
							我的工厂
						</view>
					</view>
				</view>

				<view class="ProcessManagement-content">

					<view class="ProcessManagement_bg_content" @click="jumpToAgent(1)">
						<view class="text">
							代理商
						</view>
						<view class="num">
							<text class="num_text">{{factory.agentTotal}}</text>
							<text class="newt-icon-youjiantou iconfont icon-a-youjiantoubai">
							</text>
						</view>
						
					</view>



					<view class="ProcessManagement_bg_content" @click="jumpToAgent(2)">
						<view class="text">
							产品
						</view>
						<view class="num">
							<text class="num_text">{{factory.productTotal}}</text>
							<text class="newt-icon-youjiantou iconfont icon-a-youjiantoubai">
							</text>
						</view>

						
					</view>


				</view>

			</view>

			<view class="InformationData">
				<!-- <view class="InformationData-item" @click="handPersonInfo()"> 
				<view class="InformationData-item-text">
					<view class="InformationData-item-text-logo iconfont icon-gerenxinxi">

					</view>
					<view class="textHtml">
						个人信息
					</view>
				</view>
				<view class="iconfont icon-a-youjiantoubai">
				</view>
			</view> -->
				<view class="InformationData-item" @click="handTaskManage()"
					v-if="isCheckPermi(['/CRMService/DevPlaneTask/Records'])">
					<view class="InformationData-item-text">
						<view class="InformationData-item-text-logo iconfont icon-jihuaguanli" style="font-size: 32rpx;">

						</view>
						<view class="textHtml">
							计划记录
						</view>
					</view>
					<view class="iconfont icon-a-youjiantoubai">
					</view>
				</view>
				<view class="InformationData-item" @click="handCompanyInfo()">
					<view class="InformationData-item-text">

						<view class="InformationData-item-text-logo  iconfont icon-qiyexinxi" style="font-size: 32rpx;">

						</view>
						<view class="textHtml">
							企业信息
						</view>
					</view>
					<view class="iconfont icon-a-youjiantoubai">
					</view>
				</view>

				<view class="InformationData-item" @click="handDearcm()">
					<view class="InformationData-item-text">
						<view class="InformationData-item-text-logo iconfont icon-bumenguanli-bumentubiao" style="font-size: 32rpx;">

						</view>
						<view class="textHtml">
							部门管理
						</view>
					</view>
					<view class="iconfont icon-a-youjiantoubai">
					</view>
				</view>

				<view class="InformationData-item" @click="handStaff()">
					<view class="InformationData-item-text">

						<view class="InformationData-item-text-logo iconfont icon-yuangongguanli" style="font-size: 32rpx;">

						</view>
						<view class="textHtml">
							员工管理
						</view>
					</view>
					<view class="iconfont icon-a-youjiantoubai">
					</view>
				</view>

				<view class="InformationData-item" @click="handRoleMent()"
					v-if="isCheckPermi(['/AuthService/Role/List'])">
					<view class="InformationData-item-text">
						<view class="InformationData-item-text-logo iconfont icon-zhiweiguanli" style="font-size: 32rpx;">

						</view>
						<view class="textHtml">
							角色管理
						</view>
					</view>
					<view class="iconfont icon-a-youjiantoubai">
					</view>
				</view>
				<view class="InformationData-item" @click="handAuthorized()" v-if="isCheckPermi(['/ProducerService/Agent/AuthList'])">
					<view class="InformationData-item-text">
						<view class="InformationData-item-text-logo iconfont icon-wodeshouquan2" style="font-size: 32rpx;">

						</view>
						<view class="textHtml">
							我的授权
						</view>
					</view>
					<view class="iconfont icon-a-youjiantoubai">
					</view>
				</view>
				<view class="InformationData-item" @click="editDeviceDtuid()" v-if="isShowChange">
					<view class="InformationData-item-text">
						<view class="InformationData-item-text-logo iconfont icon-zhiweiguanli" style="font-size: 32rpx;">

						</view>
						<view class="textHtml">
							设备dtuid修改
						</view>
					</view>
					<view class="iconfont icon-a-youjiantoubai">
					</view>
				</view>
				<view class="InformationData-item" @click="switchServerUrl()" v-if="isShowChange">
					<view class="InformationData-item-text">
						<view class="InformationData-item-text-logo iconfont icon-liuchengguanli" style="font-size: 32rpx;">

						</view>
						<view class="textHtml">
							切换中英文
						</view>
					</view>
					<view class="iconfont icon-a-youjiantoubai">
					</view>
				</view>
				<!-- <view class="InformationData-item" @click="toLocation()"
					v-if="showLocation">
					<view class="InformationData-item-text">
						<view class="InformationData-item-text-logo iconfont icon-zhiweiguanli" style="font-size: 32rpx;">
				
						</view>
						<view class="textHtml">
							位置定位
						</view>
					</view>
					<view class="iconfont icon-a-youjiantoubai">
					</view>
				</view> -->
			</view>
			<!--  #ifdef APP-PLUS -->
			<view class="version_con">
				版本号：{{versionNum}}
			</view>
			<!--  #endif -->
		</view>


		<uni-drawer ref="showRight" mode="left" :mask-click="false">
			<scroll-view style="height: 100%;" scroll-y="true">
				<view class="uni-popup-alert">
					<!--#ifndef MP-WEIXIN -->
					<uni-nav-bar :isCenterSlot="true" :status-bar="true" :fixed="true" :border="false" height="108rpx" backgroundColor="#ffffff">
					<!--#endif -->
					<!--#ifdef MP-WEIXIN -->
					<uni-nav-bar :hasSeat="true" seatHeight="88rpx" :isCenterSlot="true" :status-bar="true" :fixed="true" :border="false" height="108rpx" backgroundColor="#ffffff">
					<!--#endif -->
						<view class="uni-popup-alert-item">
							<view class="uni-popup-alert-item-left">
								<view class="popupNameCutting" v-if="!userName&&!userName.length<=0">
									{{userName.length>2?userName.slice(-2):userName}}
								</view>
								<image v-else class="uni-popup-alert-item-atve" :src="userInfo.Avatar+'?wh=500x500'"
									alt=""></image>
								<view class="title">
									{{userInfo.RealName}}
								</view>
							</view>

							<view class="iconfont icon-guanbidanchuang" @click="handChose">

							</view>
						</view>
					</uni-nav-bar>

					<view class="uni-popup-alert-title">
						我的企业
					</view>
					<view style="height: 100%;" scroll-y="true">

						<view class="uni-popup-alert-enterprise" :class="[select==item.Id?'active':'on']"
							v-for="(item,index) in OrganData" :key="index" @click="handSelect(index,item.Id,item)">
							<view class="uni-popup-alert-enterprise-parse">
								<view class="uni-popup-alert-enterprise-tou">
									<image style="width: 100%;height:100%;" :src="item.Logo+'?wh=500x500'" alt="" v-if="item.Logo">
									</image>
									<view class="image t-icon-qiyemorentupian1" v-else></view>
								</view>
								<view class="uni-popup-alert-enterprise-text">
									{{item.OrgName}}
								</view>
							</view>
							<view class="iconfont icon-xuanzhongqiye" v-if="select==item.Id">

							</view>
						</view>
						<view class="uni-popup-alert-enterprise" @click="handCreatePage()">
							<view class="uni-popup-alert-enterprise-parse">
								<view class="createAdd">
									+
								</view>
								<view class="uni-popup-alert-enterprise-text">
									新建企业
								</view>
							</view>
						</view>

						<view style="height:160rpx;"></view>
						<view class="button-parse">
							<view class="DeleteAccount" @click="handAccount()">注销账号</view>
							<view class="LogOut" @click="closeDrawer">
								退出登录
							</view>

						</view>
					</view>
				</view>

			</scroll-view>
		</uni-drawer>
		<msg-prompt ref="promptMsg" @confirm="confirmUnbind"></msg-prompt>
		<msg-prompt ref="promptClose" @confirm="confireCloseDrawer"></msg-prompt>
		<my-tab-bar ref="mytab" active="Profile" @messageChange="messageChange"></my-tab-bar>
	</view>
</template>

<script>
	import {
		removeToken,
		removeRefreshToken,
		setStorageKey,
		getStorageKey,
		removeStorageKey
	} from "@/common/auth";
	import {
		dataList, //个人中心列表
		ReportInterface, //流程报表接口
		OrganizationList, //组织列表
		SwitchEnterprise, //切换企业
		GetDefaultData, //获取企业信息
		deleteAccountLogin //删除账号
	} from "@/api/personalCenter";
	import {
		factorygetAgent
	} from "@/api/factory";
	import {
		factoryProductListPost
	} from '@/api/product.js'
	import {
		StockStatisticsInfo
	} from '@/api/device.js'
	import {
		checkPermi
	} from '@/common/permission.js';
	import serverUrl from '@/common/constVar.js'
	import {
		noReadCount
	} from "@/api/message";

	import {
		getWeatherInfo,
		getGeocoder
	} from '@/api/weather.js'
	import {
		retunWeatherImg
	} from '@/common/weatherInfo.js'
	import {
		logout
	} from '@/api/login.js'
	export default {
		data() {
			return {
				showLocation:true,
				userName: '',
				isShowFixedTitle: false,
				isShowDraw: false,
				isShowChange: false,//是否显示中英文切换
				statusBarHeight: uni.getSystemInfoSync().statusBarHeight,
				MorenInfo: [],
				userInfo: {},
				Interface: [],
				OrganData: [

				],
				infoData: {},
				OrgId: '',
				select: 0,
				arr: [{
						title: 'Fujian Huade Group'
					},
					{
						title: 'Huade'
					}
				],
				weatherInfo: {},
				defaultTop: 0,
				factory: {
					agentTotal: 0,
					classTotal: 0,
					productTotal: 0,
				}, //我的工厂相关信息
				inventory: {
					totalCount: 0,
					PartsCount: 0,
					DevCount: 0
				},
				isReloadUserInfo: false,
				titleOpac: 1,//顶部透明度设置
				numOpac: 1,//顶部透明度设置
				weatherOpac: 1,//顶部透明度设置
			}
		},
		async onShow() {
			if (this.$store.state.orgLis.orgList == null) {
				this.OrganData = await this.$store.dispatch("orgLis/setOrgList");
			}
			if (this.isChangeOrg) {
				if (this.OrganData && this.OrganData.length > 0) {
					let item = this.OrganData[0]
					this.handSelect(0, item.Id, item)
					this.$store.commit('orgLis/SET_CHANGE_ORG', false)
				}

			}
			if (this.isReloadUserInfo) {
				try {
					this.dataInfo()
					await this.$store.dispatch('GetInfo')
					this.isReloadUserInfo = false
				} catch (e) {
					//TODO handle the exception
					console.log("报错", e);
				}
			}
			this.$nextTick(() => {
				if (this.$store.state.isReloadMessage) {
					this.$refs.mytab.getNoReadCount()
				}
			})
		},
		async onLoad(options) {
			//console.log(this.$store.state.orgLis.orgList,'66666666666666')
			//console.log(options,'options')
			// this.OrganData = await this.$store.dispatch("orgLis/setOrgList");
			// console.log("企业列表", this.OrganData);
			this.$store.commit('orgLis/SET_ORG_LIST', null)
			this.OrganData = await this.$store.dispatch("orgLis/setOrgList");
			uni.showLoading({
				title: 'loading'
			})
			// await this.$store.dispatch('GetInfo')
			if (this.$store.state.user && this.$store.state.user.uid) {
				// this.userInfo = this.$store.state.user
			} else {
				await this.$store.dispatch('GetInfo')
				// this.userInfo = this.$store.state.user
			}
			//console.log(this.userInfo,'userInfouserInfouserInfo')

			this.$nextTick(async () => {
				this.$refs.mytab.loadCheck()
				let query = uni.createSelectorQuery().in(this);
				query.select('#rules_listcon').boundingClientRect(data => {
					// console.log(data, 'data2data2data2');
					if (data) {
						this.defaultTop = data.top
					}
				}).exec()
				if (this.isCheckPermi(['/Stock/'])) {
					await this.getStockStatisticsInfo()
				}
				if (this.isCheckPermi(['/AgentMan/'])) {
					await this.getMyFactory()
				}
			})
			uni.hideLoading()
			this.list();
			this.dataInfo();
		},
		onPageScroll(e) {
			this.getElementTop()
		},
		onBackPress(options) {
			if (options.from === 'backbutton') {
				// 来自于导航条返回按钮或者系统返回按钮
				// 返回 `true` 表示拦截操作，不再继续执行返回操作
				// 返回 `false` 表示不拦截返回操作，继续执行默认的返回操作
				if (this.isShowDraw) {
					this.handChose()
					return true; // 根据需要返回true或false
				} else {
					return false; // 根据需要返回true或false
				}
			}
		},
		computed: {
			noReadCountNum() {
				return this.$store.state.noReadNum
			},
			versionNum() {
				return this.$store.state.appversion
			},
			isChangeOrg() { //是否进行切换企业操作
				return this.$store.state.orgLis.isChangeOrg
			}
		},
		methods: {
			toLocation(){
				uni.navigateTo({
					url:'/pages_factory/location/location'
				})
			},
			handAuthorized(){//我的授权
				uni.navigateTo({
					url:'/page_crm_moudule/MyAuthorization/index'
				})
			},
			handTaskManage() {
				//跳转致任务管理
				uni.navigateTo({
					url: '/pages_device/device_info/plane_task?isManage=true'
				})
			},
			messageChange() {
				//消息发生变化，刷新流程任务
				this.loadReportInfo()
			},
			async reloadPersonalInfo() {
				//重新加载个人信息
				this.isReloadUserInfo = true
			},
			editDeviceDtuid() {
				//修改dtuid
				uni.navigateTo({
					url: '/pages_device/device_tool/device_tool'
				})
			},
			handInventory(type) {
				uni.navigateTo({
					url: '/pages_flow/inventory/inventory_list?type=' + type
				})
			},
			async getStockStatisticsInfo() {
				//获取库存统计信息
				try {
					let res = await StockStatisticsInfo()
					// console.log(res, "库存统计信息");
					let data = res.data;
					this.inventory.totalCount = data.TotalCount
					this.inventory.PartsCount = data.PartsCount
					this.inventory.DevCount = data.DevCount
				} catch (err) {
					//TODO handle the exception
					this.setMsgTop(err)
				}
			},
			jumpToAgent(val) {
				//跳转至代理商，耗材管理，耗材分类
				let url = ''
				if (val && val == 1) {
					url = '/pages_factory/agents_list'
				} else if (val && val == 2) {
					url = '/pages_factory/product_list'
				}
				uni.navigateTo({
					url: url
				})
			},
			async getMyFactory() {
				//获取代理商，我的耗材，耗材分类总数
				try {
					let agentRes = await factorygetAgent({
						pageNum: 1,
						pageSize: 10
					})
					// let classRes = await classTree()
					let protRes = await factoryProductListPost({
						pageNum: 1,
						pageSize: 10
					})
					this.factory.agentTotal = agentRes.data.Total
					// this.factory.classTotal=agentRes.data.Total
					this.factory.productTotal = protRes.data.Total

				} catch (e) {
					//TODO handle the exception
					// console.log('e',e);
					if(e.statusCode!=404){
						this.setMsgTop(e)
					}
					
				}
			},
			getElementTop() {
				//获取距离顶部的距离
				let minScrollhei = 44
				let query = uni.createSelectorQuery().in(this);
				query.select('#minscrolldom').boundingClientRect(data2 => {
					// console.log(data2,'data2data2data2');
					if (data2) {
						minScrollhei = data2.height
					}
				}).exec()
				//需要给黄色区域设置一个id标识，在这里是demo
				query.select('#rules_listcon').boundingClientRect(data => {
					// console.log(data.top) //这个就是距离顶部的高度
					// if (data) {
					// 	if (data.top < minScrollhei) {
					// 		this.isShowFixedTitle = true
					// 	} else {
					// 		this.isShowFixedTitle = false
					// 	}
					// 	this.setDomOpac(data)
					// }
					if (data) {
					    if (data.top <= this.statusBarHeight + minScrollhei) {
					        this.isShowFixedTitle = true;
					    } else {
					        this.isShowFixedTitle = false;
					    }
					    this.setDomOpac(data)
					}
				}).exec();
			},
			setDomOpac(data) {
				//设置元素的透明度
				switch (true) {
					case data.top <= Number(this.defaultTop - this.statusBarHeight - 160):
						this.numOpac = 0
						this.titleOpac = 0
						this.weatherOpac = 0
						break;
					case data.top <= Number(this.defaultTop - this.statusBarHeight - 136):
						this.numOpac = 0
						this.titleOpac = 0
						this.weatherOpac = 0.1
						break;
					case data.top <= Number(this.defaultTop - this.statusBarHeight - 110):
						this.numOpac = 0
						this.titleOpac = 0
						this.weatherOpac = 0.2
						break;
					case data.top <= Number(this.defaultTop - this.statusBarHeight - 84):
						this.numOpac = 0
						this.titleOpac = 0
						this.weatherOpac = 0.3
						break;
					case data.top <= Number(this.defaultTop - this.statusBarHeight - 58):
						this.numOpac = 0
						this.titleOpac = 0
						this.weatherOpac = 0.4
						break;
					case data.top <= Number(this.defaultTop - this.statusBarHeight - 32):
						this.weatherOpac = 0.5
						this.numOpac = 0
						this.titleOpac = 0
						break;
					case data.top <= Number(this.defaultTop - this.statusBarHeight - 26):
						this.weatherOpac = 1
						this.numOpac = 0.25
						this.titleOpac = 0
						break;
					case data.top <= Number(this.defaultTop - this.statusBarHeight - 20):
						this.weatherOpac = 1
						this.numOpac = 0.5
						this.titleOpac = 0
						break;
					case data.top <= Number(this.defaultTop - this.statusBarHeight - 15):
						this.weatherOpac = 1
						this.numOpac = 1
						this.titleOpac = 0.14
						break;
					case data.top <= Number(this.defaultTop - this.statusBarHeight - 10):
						this.weatherOpac = 1
						this.numOpac = 1
						this.titleOpac = 0.26
						break;
					case data.top <= Number(this.defaultTop - this.statusBarHeight - 5):
						this.weatherOpac = 1
						this.numOpac = 1
						this.titleOpac = 0.35
						break;
					case data.top < Number(this.defaultTop - this.statusBarHeight):
						this.weatherOpac = 1
						this.numOpac = 1
						this.titleOpac = 0.5
						break;
					default:
						this.weatherOpac = 1
						this.numOpac = 1
						this.titleOpac = 1
				}
			},
			getWeatherImg(val) {
				//获取天气形象
				return retunWeatherImg(val)
			},
			getWeather(val) {
				getWeatherInfo(val).then(res => {
					if (res.data.city) {
						res.data.city = res.data.city.substring(0, res.data.city.length - 1)
					}
					this.weatherInfo = res.data
					if (this.canRefresh) {
						setTimeout(() => {
							this.getWeather();
						}, 50000);
					}
				}).catch(err => {
					this.setMsgTop(err)
				})
			},
			handWarehouse(name) {
				if (name === 'Warehouse_management') {
					uni.navigateTo({
						url: '/pages_flow/inventory/warehouse_management'
					})
				}
				if (name === 'Warehousing_records') {
					uni.navigateTo({
						url: '/pages_flow/inventory/warehouse_records'
					})
				}
				if (name === 'Outbound_records') {
					uni.navigateTo({
						url: '/pages_flow/inventory/outbound_records'
					})
				}
				if (name === 'Outbound_apply') {
					uni.navigateTo({
						url: '/pages_flow/inventory/applylist'
					})
				}
				if (name === 'Checking_tasks') {
					uni.navigateTo({
						url: '/pages_flow/inventory/checking_tasks'
					})
				}
			},
			
			readMessage() {
				//获取未读消息数量
				this.$refs.mytab.getNoReadCount()
			},
			switchServerUrl() {
				// 切换访问变量
				// #ifndef H5
				if (serverUrl.getServerUrl() == 'http://125.124.98.180:7080'||serverUrl.getServerUrl() == 'http://iot.wookongcloud.com') {
					uni.showModal({
						title: '系统提示',
						content: '是否确认切换AirLinkiot',
						success: (res) => {
							if (res.confirm) {
								// getApp().globalData.s/erverUrl= 'http://123.58.218.244'; //修改全局变量
								serverUrl.setServerUrl('http://123.58.218.244')
								removeToken();
								removeRefreshToken();
								setStorageKey("serverUrl", serverUrl.getServerUrl());
								this.$store.commit('SET_ROLES', [])
								this.$store.commit('SET_PERMISSIONS', [])
								setTimeout(() => {
									uni.reLaunch({
										url: '/page_register/login?noFirt=1'
									})
								}, 1000)
							}
						}
					});

				} else if (serverUrl.getServerUrl() == 'http://123.58.218.244') {
					uni.showModal({
						title: '系统提示',
						content: '是否确认切换悟空云',
						success: (res) => {
							if (res.confirm) {
								serverUrl.setServerUrl('http://iot.wookongcloud.com')
								removeToken();
								removeRefreshToken();
								setStorageKey("serverUrl", serverUrl.getServerUrl());
								this.$store.commit('SET_ROLES', [])
								this.$store.commit('SET_PERMISSIONS', [])
								setTimeout(() => {
									uni.reLaunch({
										url: '/page_register/login?noFirt=1'
									})
								}, 1000)
							}
						}
					});

				}

				// #endif
			},
			jumpToMessage() {
				//跳转到消息中心
				uni.navigateTo({
					url: '/pages/message/message'
				})
			},
			handStaff() {
				uni.navigateTo({
					url: '../../personal_center/StaffManagement/index'
				})
			},
			handRoleMent() {
				uni.navigateTo({
					url: '../../personal_center/Roles/index'
				})
			},
			handDearcm() {
				uni.navigateTo({
					url: '../../personal_center/Department/index'
				})
			},
			isCheckPermi(val) {
				return checkPermi(val)
			},
			loadData(query) {
				//更新
			},
			toProcessList(val) {
				//跳转至流程列表
				uni.navigateTo({
					url: '/pages_flow/process_list?item=' + val
				})
			},
			toCreateProcess() {
				//创建流程
				uni.navigateTo({
					url: '/pages_flow/add_process'
				})
			},
			//个人信息
			handPersonInfo() {
				uni.navigateTo({
					url: '../../personal_center/personData/personData?id=' + this.userInfo.Id
				})
			},
			//公司信息
			handCompanyInfo() {
				uni.navigateTo({
					url: '../../personal_center/CompanyInfo/CompanyInfo?id=' + this.OrgId
				})
			},
			confirmUnbind() {
				deleteAccountLogin({

				}).then(async (res) => {
					if (res.code == 0) {

						await logout()
						removeToken();
						removeRefreshToken();
						uni.reLaunch({
							url: '/page_register/login'
						})
					}
				}).catch((err) => {
					this.setMsgTop(err)
				})
			},
			handAccount() {
				this.$refs.promptMsg.noticeOpen(
					'您确认要注销此帐户吗？\n注销后，您将无法登录?'
				)

			},
			//获取企业信息
			information(id) {
				//console.log(id)
				GetDefaultData({
					id: id
				}).then((res) => {
					if (res.code == 0) {
						this.MorenInfo = res.data;
					}
				})
			},
			showDrawer() {
				this.$refs.showRight.open();
				this.isShowDraw = true
			},
			handChose() {
				this.$refs.showRight.close();
				this.isShowDraw = false
			},
			closeDrawer() {
				this.$refs.promptClose.noticeOpen(
					'你确定要退出登录吗?'
				)
			},
			async confireCloseDrawer() {
				this.$store.dispatch("mqttclient/getClient").then((client) => {
					let tkey = "user/" + this.$store.getters.uid + "/new";
					client.unsubscribe(tkey, (error) => {
						// console.info("取消订阅dddd22", error)
						this.$store.commit("mqttclient/Del_Handler", tkey);
					});
				});
				try {
					await logout()
					this.$store.commit('SET_ROLES', [])
					this.$store.commit('SET_PERMISSIONS', [])
					removeToken();
					removeRefreshToken();
					uni.reLaunch({
						url: '/page_register/login'
					})
				} catch (e) {
					//TODO handle the exception
					uni.reLaunch({
						url: '/page_register/login'
					})
				}
			},
			handCreatePage() {
				this.$refs.showRight.close();
				this.isShowDraw = false
				uni.navigateTo({
					url: '/personal_center/createCompany/createCompany'
				})
			},
			dataInfo() {
				//个人信息
				dataList().then((res) => {
					if (res.code == 0) {
						//console.log(res,'个人信息')
						this.userInfo = res.data.user
						this.userName = res.data.user.RealName
						this.select = res.data.user.OrgId
						//this.information(res.data.user.OrgId)
						this.OrgId = res.data.user.OrgId
						//console.log(res.data.user.OrgId,'res.data.user.OrgId')
					}
				})
			},
			list() {
				this.loadReportInfo()
				//组织列表
				OrganizationList().then((res) => {
					if (res.code == 0) {
						//	console.log(res,'组织列表')
						this.OrganData = res.data;
					}
				})
			},
			loadReportInfo() {
				//流程报表接口
				ReportInterface().then((res) => {
					if (res.code == 0) {
						//console.log(res,'流程报表接口')
						this.Interface = res.data
						this.$forceUpdate()
					}
				})
			},
			close() {

			},
			handSelect(inx, id, ite) {
				SwitchEnterprise({
					id: id
				}).then(async (res) => {
					if (res.code == 0) {
						uni.showToast({
							title: '切换成功！',
							icon: 'none'
						})
						this.$store.commit('SET_hasLoadVersion',false)
						this.$store.commit('SET_ROLES', [])
						this.$store.commit('SET_PERMISSIONS', [])
						this.select = inx;
						this.infoData = ite;
						// this.dataInfo();
						this.$store.commit('SET_MESSAGE_INFO', true)
						//消息取消订阅

						this.$refs.showRight.close();
						this.isShowDraw = false
						// this.$refs.promptMsg.loadingOpen('Loading...')
						try { //切换企业后重新设置权限信息
							await this.$store.dispatch("GetInfo");
							// this.$refs.promptMsg.loadingColse()
							this.$store.commit('SET_UID', '');
							uni.reLaunch({
								url: '/page_register/other'
							})
						} catch (e) {
							//TODO handle the exception
							this.setMsgTop(e)
							// this.$refs.promptMsg.loadingColse()
						}

					}
				}).catch((err) => {
					//console.log(err,'1111111')
					this.setMsgTop(err)
				})
			},
			handBread() {
				this.$refs.popup.open()
			}
		}
	}
</script>

<style lang="less" scoped>
	.version_con {
		padding: 20rpx 0;
		text-align: center;
		color: rgba(193, 193, 193, 1);
		font-size: 22rpx;
		line-height: 22rpx;
	}

	.profileActive {
		position: absolute;
	}

	.rules_list_con {
		position: relative;
		z-index: 1;
	}

	.rules_title_fiex {
		width: 100%;
		background-color: #fff;
		transition: 3000;
		opacity: 1;
		position: fixed;
		top: 0;
		left: 0;
		z-index: 3;
		line-height: 88rpx;
		text-align: center;
		font-size: 34rpx;
		font-weight: bold;
		box-sizing: border-box;
		color: #333;
	}

	.profile {
		position: relative;
		background: rgba(245, 248, 249, 1);

		.modify-header {
			position: relative;
			z-index: 1;
			width: 100%;
			// height: 686rpx;
			background: linear-gradient(180deg, rgba(222, 235, 250, 1), rgba(245, 248, 249, 1));

			.modify-header-bottom {
				display: flex;

				.modify-header-bottom-parse {
					display: flex;
					align-items: center;
					flex: 1;

					.modify-header-bottom-left {
						display: flex;
						width: 178rpx;
						height: 60rpx;
						border-radius: 36rpx;
						line-height: 60rpx;
						text-align: center;
						color: rgba(153, 153, 153, 1);
						font-size: 24rpx;
						background: rgba(255, 255, 255, .5);
						justify-content: center;
						margin-left: 30rpx;

						.icon-bianji {
							margin-right: 10rpx;
							font-size: 24rpx;
						}
					}
				}

				.modify-header-bottom-right {
					width: 180rpx;
					height: 160rpx;
					margin-right: 30rpx;
				}
			}

			.modify-header-top {
				display: flex;
				align-items: center;

				// padding-top:42rpx;
				.NameCutting {
					width: 100rpx;
					height: 100rpx;
					border-radius: 50%;
					line-height: 100rpx;
					text-align: center;
					color: #fff;
					margin-left: 30rpx;
					font-size: 28rpx;
					background: rgba(35, 113, 255, 1);
				}

				.modify-header-top-left-tou {
					width: 100rpx;
					height: 100rpx;
					border-radius: 50%;
					margin-left: 30rpx;

				}

				.modify-header-top-left {
					display: flex;
					align-items: center;
					flex: 1;

					.modify-header-top-left-text {
						margin-left: 30rpx;

						.modify-header-top-left-text-content {
							.name {
								font-size: 40rpx;
								color: rgba(51, 51, 51, 1);
								font-weight: bold;
							}

							.newt-icon-dajiantou {
								display: inline-block;
								width: 37rpx;
								height: 30rpx;
								margin-left: 20rpx;
								font-weight: bold;
							}
						}

						.text_name {
							color: rgba(153, 153, 153, 1);
							font-size: 24rpx;
							margin-top: 5rpx;
						}
					}
				}

				.modify-header-top-right {
					position: relative;
					margin-right: 30rpx;
					margin-top: -40rpx;

					.message_num {
						position: absolute;
						width: 12rpx;
						height: 12rpx;
						border-radius: 50%;
						right: -4rpx;
						top: -4rpx;
						background-color: #FF3535;
					}
				}
			}
		}

		.device_list_con {
			background-color: #F5F8F9;
		}

		.button-parse {
			display: flex;
			align-items: center;
			justify-content: space-around;
			position: fixed;
			width: 96%;
			bottom: 0rpx;
			margin: 0rpx 2%;
			padding-bottom: 20rpx;
			background: #fff;

			.LogOut {
				width: 47%;
				height: 100rpx;
				line-height: 100rpx;
				text-align: center;
				background: #F8F8F8;
				color: #999999;
				font-size: 32rpx;
				z-index: 10;
			}
		}

		.DeleteAccount {

			width: 47%;
			height: 100rpx;
			line-height: 100rpx;
			justify-content: space-evenly;
			align-items: center;
			background: #F8F8F8;
			color: #999999;
			font-size: 32rpx;
			border-radius: 8rpx;
			text-align: center;

		}

		::v-deep.uni-drawer {
			// position: absolute;
		}

		::v-deep.uni-drawer__content {
			width: 100vw !important;
			background: #ffffff !important;

		}

		::v-deep.uni-tabbar {
			z-index: 55;
		}

		::v-deep.uni-navbar__header-container {
			display: block;
			padding: 0rpx;
		}

		::v-deep.uni-navbar__header-btns-right,
		::v-deep.uni-navbar__header-btns-left {
			display: none;
		}

		.uni-popup-alert {
			position: relative;
			width: 100% !important;
			background: #FFFFFF !important;
			color: #333;
			z-index: 888;

			// min-height:100vh;// calc(100vh - 150rpx);
			// padding-bottom: 30rpx;
			::v-deep.uni-navbar__header-container {
				display: block;
			}

			::v-deep.uni-navbar__header {
				background: #fff !important;
				color: #333 !important;
				padding: 0rpx;
			}


			.uni-popup-alert-enterprise {
				height: 140rpx;
				// line-height: 140rpx;
				border-radius: 8rpx;
				margin: 0rpx 3%;
				align-items: center;
				display: flex;
				padding: 10rpx 30rpx;

				.icon-xuanzhongqiye {
					margin-left: auto;
					color: #2371FF;
				}

				.uni-popup-alert-enterprise-parse {
					display: flex;
					align-items: center;

					.createAdd {
						width: 80rpx;
						height: 80rpx;
						line-height: 80rpx;
						text-align: center;
						font-size: 40rpx;
						background: #F8F8F8;
						border-radius: 8rpx;
						color: #999999;
					}

					.uni-popup-alert-enterprise-text {
						color: #333;
						font-size: 32rpx;
						margin-left: 30rpx;
						font-weight: bold;
					}

					.uni-popup-alert-enterprise-tou {
						width: 80rpx;
						height: 80rpx;
						background: #fff;
						border-radius: 8rpx;
						.image{
							width: 80rpx;
							height: 80rpx;
							border-radius: 8rpx;
						}
					}

				}

			}

			.active {
				background: #f8f8f8;
			}

			.on {
				background: none;
			}

			.uni-popup-alert-title {
				margin: 20rpx 3%;
				font-size: 28rpx;
				color: #999999;
			}

			.uni-popup-alert-item {
				margin: 0rpx 3%;
				padding: 20rpx 0rpx;
				display: flex;
				align-items: center;
				padding-top: 25rpx;

				.uni-popup-alert-item-left {
					display: flex;
					align-items: center;

					.popupNameCutting {
						width: 60rpx;
						height: 60rpx;
						border-radius: 50%;
						line-height: 60rpx;
						text-align: center;
						color: #fff;
						font-size: 20rpx;
						background: rgba(35, 113, 255, 1);
					}

					.title {
						margin-left: 20rpx;
						font-size: 32rpx;
						font-weight: bold;
					}

					.uni-popup-alert-item-atve {
						width: 60rpx;
						height: 60rpx;
						//background: red;
						border-radius: 50%;
					}
				}

				.icon-guanbidanchuang {
					margin-left: auto;
					font-size: 38rpx;
					color: #999;
				}

			}
		}

		.InformationData {
			display: flex;
			flex-direction: column;
			margin: 0rpx 3%;

			.InformationData-item {
				display: flex;
				color: #333;
				padding: 0rpx 20rpx;
				height: 110rpx;
				line-height: 110rpx;
				background: #fff;
				justify-content: space-between;
				border-radius: 6rpx;
				margin-bottom: 20rpx;

				.InformationData-item-text {
					font-size: 30rpx;
					display: flex;
					align-items: center;

					.InformationData-item-text-logo {
						margin-right: 20rpx;
						color: rgba(51, 51, 51, 1);

					}

					.textHtml {
						font-weight: bold;
					}
				}

				.icon-a-youjiantoubai {
					font-size: 26rpx;
					color: #999999;
				}
			}
		}

		.ProcessManagement {
			margin: 20rpx;
			border: 1rpx solid rgba(255, 255, 255, .2);
			border-radius: 8rpx;
			padding: 25rpx 30rpx;
			background: #fff;

			.ProcessManagement-CreateProcess {
				display: flex;
				height: 88rpx;
				line-height: 88rpx;
				justify-content: space-evenly;
				align-items: center;
				background: rgba(35, 113, 255, 1);
				color: #fff;
				font-size: 28rpx;
				border-radius: 8rpx;
				margin-top: 28rpx;
				margin-bottom: 10rpx;
			}

			.MyFactory {}

			.ManageList {
				display: flex;
				flex-wrap: wrap;
				justify-content: space-between;
				margin-top: 15rpx;

				.ManageList-item {
					width: 322rpx;
					height: 88rpx;
					margin-bottom: 10rpx;
				}

				.ManageList-item_con {
					display: flex;
					align-items: center;
					margin-right: -10rpx;

					.ManageList-item {
						width: 210rpx;
						height: 88rpx;
						margin-right: 10rpx;
					}
				}
			}

			.ProcessManagement-content {
				display: flex;
				text-align: center;
				//padding: 30rpx 0rpx;
				//border-bottom: 1rpx solid rgba(255, 255, 255, .2);
				justify-content: space-between;
				padding: 10rpx 0rpx;
				padding-top: 40rpx;
				.ProcessManagement_bg_content{
					width: calc(50% - 5rpx);
					padding: 0 30rpx;
					box-sizing: border-box;
					height: 100rpx;
					background: rgba(248, 248, 248, 1);
					color: #333333;
					font-size: 28rpx;
					display: flex;
					justify-content: space-between;
					align-items: center;
					border-radius: 10rpx;
					.text{
						font-weight: bold;
					}
					.num{
						color: rgba(153, 153, 153, 1);
						display: flex;
						align-items: center;
						.newt-icon-youjiantou{
							font-size: 18rpx;
							margin-left: 16rpx;
						}
					}
				}
				.ProcessManagement-content-text {

					.num {
						position: relative;
						font-size: 36rpx;
						line-height: 36px;
						font-weight: bold;
						display: flex;
						justify-content: center;
						align-items: center;

						.num_text {
							// display: inline-block;
							// padding:10rpx;
							// width: 100%;
							margin-left: 20rpx;
						}

						.newt-icon-youjiantou {
							// position: absolute;
							// width: 15rpx;
							// height: 15rpx;
							margin-left: 10rpx;
							// margin-top: 15rpx;
							font-size: 14rpx;
							color: #999999;
							// right:0rpx;
							// top:15rpx;
						}

					}

					.text {
						font-size: 22rpx;
						line-height: 22rpx;
						margin-top: 10rpx;
						color: rgba(153, 153, 153, 1);
					}
				}

				.ProcessManagement-content-item {
					width: 155rpx;
					border-radius: 10rpx;
					padding: 25rpx 0rpx;
					background: rgba(245, 248, 249, 1);

					.num {
						color: #333;
						font-size: 32rpx;
					}

					.text {
						color: rgba(153, 153, 153, 1);
						margin-top: 10rpx;
						font-size: 24rpx;
					}
				}

			}

			.ProcessManagement-title {
				font-size: 30rpx;
				color: #333333;
				display: flex;
				align-items: center;

				.ProcessManagement-title-left {
					display: flex;
					flex: 1;
					align-items: center;

					.text_content {
						font-weight: bold;
					}
				}

				.InitiateProcess {
					color: rgba(35, 113, 255, 1);
					display: flex;
					align-items: center;

					.InitiateProcess-add {
						display: inline-block;
						font-size: 24rpx;
						margin-right: 7rpx;
						margin-top: 7rpx;
					}
				}

				.t-icon-liuchengguanli {
					width: 32rpx;
					height: 32rpx;
					margin-right: 20rpx;
				}
			}
		}
	}
</style>