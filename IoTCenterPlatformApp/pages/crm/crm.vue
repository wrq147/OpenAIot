<template>
	<view>
		<!--#ifdef H5 -->
		<jump-down></jump-down>
		<!--#endif -->
		<view id="CRM">
			<view class="CRM-bgHeader" :style="{'padding-top':statusBarHeight*2+50+'rpx'}"
				:class="{'hide_top_info':isShowFixedTitle}">
				<view class="top_info_title"><!-- :style="{'opacity': titleOpac}" -->
					<view class="title_left">
						<view class="user_name" style="color:#333;">客户管理</view>
					</view>
					<!--#ifndef MP-WEIXIN -->
					<view class="title_right">
						<view v-if="!hide" class="icon_con" style="margin-left: 36rpx;" @click="handAddPage">
							<custom-icons iconsName="icon-tianjia" iconsSize="36rpx" iconsColor="#333"></custom-icons>
						</view>
					</view>
					<!--#endif -->

				</view>

				<view class="CRM-bgHeader-text">
					您有{{CustomerLeadContent.KfCount}}个客户，{{CustomerLeadContent.ClueCount}}个线索，{{CustomerLeadContent.OpportCount}}个商机
					<!--#ifdef MP-WEIXIN -->
					<view class="title_right">
						<view v-if="!hide" class="icon_con" style="margin-left: 36rpx;" @click="handAddPage">
							<custom-icons iconsName="icon-tianjia" iconsSize="36rpx" iconsColor="#333"></custom-icons>
						</view>
					</view>
					<!--#endif -->
				</view>
				<!--天气-->
				<!--#ifdef MP-WEIXIN -->
				<view class="weather" style="margin-top: 50rpx;">
				<!--#endif -->
				<!--#ifndef MP-WEIXIN -->
				<view class="weather">
				<!--#endif -->
					<view class="weather-left" v-if="weatherInfo&&weatherInfo.weather">
						<view class="weather-left-logo">
							<image v-if="getSerVerUrl()" class="weather-left-logo-image" :src="getWeatherImg(weatherInfo.weather)" mode="">
							</image>
						</view>
						<view class="weather-left-text">
							<view class="title">{{weatherInfo.city}} {{weatherInfo.weather}}</view>
							<view class="content">
								温度{{weatherInfo.temperature}}℃｜湿度{{weatherInfo.humidity}}%
							</view>
						</view>
					</view>
					<view class="weather_info" v-else>
						<image v-if="getSerVerUrl()" class="weather_img" :src="getSerVerUrl()+'/appimg/weather_img/duoyun.png'" mode="">
						</image>
						<view class="info_con">
							<view class="wea_text">
								<text class="text1">本地天气</text>
								<text>--</text>
							</view>
							<view class="tem_hum">
								<view class="text">请开启定位服务</view>
							</view>
						</view>
					</view>
					<view class="weather-right">
						<image v-if="getSerVerUrl()" class="illustration" :src="getSerVerUrl()+'/appimg/newImg/illustration.png'" mode="">
						</image>
					</view>
				</view>
			</view>
			<view class="rules_title_fiex" id="minscrolldom" v-if="isShowFixedTitle"
				:style="{'height':Number(statusBarHeight*2)+88+'rpx','padding-top':Number(statusBarHeight*2)+'rpx'}">
				客户管理
			</view>
			<view id="rules_listcon" class="rules_list_con">
				<view class="crm-nav">
					<!-- <view class="crm-header-item" @click="handAuthorized" v-if="isCheckPermi(['/ProducerService/Agent/AuthList'])">
					<view class="crm-header-logo t-icon-wodeshouquan1">
						
					</view>
					<view class="crm-header-text">我的授权</view>
				</view> -->
					<view class="crm-header-item" @click="handHighseas"
						v-if="isCheckPermi(['/CRMService/Customer/PubList'])">
						<view class="crm-header-logo t-icon-gonghaichi1">

						</view>
						<view class="crm-header-text">公海池</view>
					</view>
					<view class="crm-header-item" @click="handCustomers"
						v-if="isCheckPermi(['/CRMService/Customer/List'])">
						<view class="crm-header-logo t-icon-kehu1">

						</view>
						<view class="crm-header-text">我的客户</view>
					</view>
					<view class="crm-header-item" @click="handCluePool"
						v-if="isCheckPermi(['/CRMService/Clue/PubList'])">
						<view class="crm-header-logo t-icon-xiansuochi1">

						</view>
						<view class="crm-header-text">线索池</view>
					</view>
					<view class="crm-header-item" @click="handXian" v-if="isCheckPermi(['/CRMService/Clue/PriList'])">
						<view class="crm-header-logo t-icon-xiansuo1">

						</view>
						<view class="crm-header-text">线索</view>
					</view>
					<view class="crm-header-item" @click="handContact"
						v-if="isCheckPermi(['/CRMService/Contact/List'])">
						<view class="crm-header-logo t-icon-lianxiren1">

						</view>
						<view class="crm-header-text">联系人</view>
					</view>
					<view class="crm-header-item" @click="handOppor"
						v-if="isCheckPermi(['/CRMService/Opportunity/List'])">
						<view class="crm-header-logo t-icon-shangji1">

						</view>
						<view class="crm-header-text">商机</view>
					</view>
					<view class="crm-header-item" @click="handFollowPlan"
						v-if="isCheckPermi(['/CRMService/Plan/List'])">
						<view class="crm-header-logo t-icon-genjinjihua1">

						</view>
						<view class="crm-header-text">跟进计划</view>
					</view>
					<view class="crm-header-item" @click="handFollowRecord"
						v-if="isCheckPermi(['/CRMService/Follow/List'])">
						<view class="crm-header-logo t-icon-genjinjilu1">

						</view>
						<view class="crm-header-text">跟进记录</view>
					</view>
				</view>
				<view class="CRM-Sales-data">
					<view class="CRM-Sales-data-header">
						<view class="CRM-Sales-data-logo iconfont icon-xiaoshoushuju">
							<text class="text">销售数据</text>
						</view>
						<view class="CRM-Sales-data-header-right" @click="handSalesData()">
							<text>{{saleTotDay}}</text>
							<text class="youjian iconfont icon-xialajiantou"></text>
						</view>
						<view class="pointAlert" v-if="salesDataHide">
							<view v-for="(item,index) in dayData" :key="index" class="pointAlert-item"
								@click="handSaleItem(item)">
								{{item.title}}
							</view>
						</view>
					</view>
					<view class="CRM-Sales-customers">
						<view class="CRM-Sales-customers-item">
							<view class="num">{{salesData.NewKfCount}}</view>
							<view class="parse-text">
								<view class="content">新增客户</view>
							</view>
						</view>

						<view class="CRM-Sales-customers-item">
							<view class="num">{{salesData.NewOpportCount}}</view>
							<view class="parse-text">
								<view class="content">新增商机</view>
							</view>
						</view>

						<view class="CRM-Sales-customers-item">
							<view class="num">{{salesData.NewFollowCount}}</view>
							<view class="parse-text">
								<view class="content">新增跟进</view>
							</view>
						</view>

						<view class="CRM-Sales-customers-item">
							<view class="num">{{salesData.FollowClueCount}}</view>
							<view class="parse-text">
								<view class="content">线索跟进</view>
							</view>
						</view>

						<view class="CRM-Sales-customers-item">
							<view class="num">{{salesData.FollowCustomerCount}}</view>
							<view class="parse-text">
								<view class="content">客户跟进</view>
							</view>
						</view>

						<view class="CRM-Sales-customers-item">
							<view class="num">{{salesData.FollowOpportCount}}</view>
							<view class="parse-text">
								<view class="content">商机跟进</view>
							</view>
						</view>
					</view>
				</view>
				<!--跟进提醒-->
				<view class="CRM-Sales-data">
					<view class="CRM-Sales-data-header">
						<view class="CRM-Sales-data-header-title">
							<view class="CRM-Sales-data-logo iconfont icon-genjintixing">
								<text class="text">跟进提醒</text>
							</view>
							<!-- <text class="newt-icon-genjintixing"></text>
						<text class="text" >跟进提醒</text> -->
						</view>
					</view>
					<view v-if="planList.length>0">
						<view class="FollowReminder" v-for="(item,index) in planList" :key="index">
							<view class="FollowReminder-top">
								<text class="title">跟进目标：</text>
								<text class="content">{{item.CustomerName}}</text>
							</view>
							<view class="FollowReminder-bottom">
								<text class="title">跟进时间：</text>
								<text class="content">
									<text>{{item.PlanTime}}</text>
									<text class="Expired" :class="[item.Status=='已超期'?'expiredActive':'expiredOn']">
										{{item.Status}}
									</text>
								</text>
							</view>
						</view>
					</view>
					<view class="emotyNull" v-else>
						<image v-if="getSerVerUrl()" class="emotyNull-img" :src="getSerVerUrl()+'/appimg/newImg/dataAvailable.png'" alt="">
						</image>
						<view>近期无跟进计划</view>
					</view>
					<view style="width: 100%;height:10rpx;">
					</view>

				</view>
			</view>
		</view>
		<!--弹框创建-->
		<view class="alert" v-if="hide">
			<view class="iconfont t-icon-crmtianjiaguanbi" @click="handAddPage"></view>
			<view class="alert-item" @click="handSeas">
				公海池
			</view>
			<view class="alert-item" @click="handCustomersRoute">
				我的客户
			</view>
			<view class="alert-item" @click="handClueRoute">
				线索池
			</view>
			<view class="alert-item" @click="handRouteItem">
				线索
			</view>
			<view class="alert-item" @click="handContactRoute">
				联系人
			</view>
			<view class="alert-item" @click="handOpportRoute">
				商机
			</view>
			<view class="alert-item" @click="handFollowRoute">
				跟进计划
			</view>
			<view class="alert-item" @click="handFollowed">
				跟进记录
			</view>
		</view>
		<view class="move" v-if="hide"></view>
		<my-tab-bar active="CRM" ref="mytab"></my-tab-bar>

	</view>
</template>

<script>
	var dayjs = require('@/common/day.js')
	import {
		getSevenPlan
	} from '@/api/home.js'
	import {
		crmData, //Sales data 数据信息
		crmCustomerData //客户、线索、商机
	} from "@/api/crmApi";
	import {
		FunnelPlot, //漏斗图
	} from "@/api/crmApi";
	import {
		dataList,
	} from "@/api/personalCenter";
	import {
		checkPermi
	} from '@/common/permission.js';
	import {
		getWeatherInfo,
		getGeocoder
	} from '@/api/weather.js'
	import {
		retunWeatherImg
	} from '@/common/weatherInfo.js'
	var dayjs = require('@/common/day.js')
	export default {
		data() {
			return {
				titleOpac: 1,
				statusBarHeight: uni.getSystemInfoSync().statusBarHeight,
				topopacity: 1,
				ischangeTopBg: true,
				saleTotDay: '今日',
				totDay: '今日',
				dayHide: false,
				salesDataHide: false,
				isShowFixedTitle: false,
				planList: [],
				dayData: [{
						title: '今日',
						value: 0
					},
					{
						title: '昨日',
						value: 1
					},
					{
						title: '近7天',
						value: 7
					},
					{
						title: '近30天',
						value: 30
					},
				],
				timeDateSales: '0',
				timeDate: '0',
				arr1: [],
				hide: false,
				salesData: [], //Sales data 数据信息
				arr: [{
						icon: 'icon-wodeshouquan',
						text: 'Authorized'
					}, {
						icon: 'icon-gonghaichi',
						text: 'High seas'
					}, {
						icon: 'icon-kehu',
						text: 'Customers'
					}, {
						icon: 'icon-xiansuochi',
						text: 'Clue pool'
					},
					{
						icon: 'icon-xiansuo',
						text: 'Clues'
					},
					{
						icon: 'icon-lianxiren',
						text: 'Contacts'
					},
					{
						icon: 'icon-lianxiren',
						text: 'Opportunity'
					},
					{
						icon: 'icon-genjinjihua',
						text: 'Follow up plan'
					},
					{
						icon: 'icon-genjinjilu',
						text: 'Followed up'
					},
				],
				scrollTop: '',
				PlanStartTime: '',
				PlanEndTime: '',
				weatherInfo: {},
				defaultTop: 0,
				CustomerLeadContent: {},
				timeComparison: ''
			}
		},
		onShow() {
			this.$nextTick(() => {
				if (this.$store.state.isReloadMessage) {
					this.$refs.mytab.getNoReadCount()
				}
			})
		},
		async onLoad() {
			this.$nextTick(() => {
				this.$refs.mytab.loadCheck()
				let query = uni.createSelectorQuery().in(this);
				query.select('#rules_listcon').boundingClientRect(data => {
					if (data) {
						this.defaultTop = data.top
					}
				}).exec()
			})
			if (this.$store.state.user.roles && this.$store.state.user.roles.length > 0) {} else {
				let rsp = await dataList({
					id: 0
				})
				await this.$store.dispatch('GetInfo')
				this.$store.commit('SET_ROLES', rsp.data.user.roleIds)
				this.$refs.mytab.loadCheck()
			}
			this.list(); //Sales data 数据信息
			//this.dataFullt();//漏斗图
			this.getLocationInfo()
			this.reminderList(); //跟进提醒
			this.CustomerLeadData();
		},
		onPageScroll(e) {
			this.getElementTop()
		},

		methods: {
			CustomerLeadData() {
				crmCustomerData().then((res) => {
					if (res.code == 0) {
						//console.log(res,'客户、线索')
						this.CustomerLeadContent = res.data;
					}
				})
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
					if (data) {
						if (data.top < minScrollhei) {
							this.isShowFixedTitle = true
						} else {
							this.isShowFixedTitle = false
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
					console.log("err天气", err);
				})
			},
			getLocationInfo() {
				//获取本地地址信息
				uni.getLocation({
					type: 'wgs84',
					success: (res) => {
						getGeocoder({
							location: res.latitude + ',' + res.longitude,
							maptype: 'lc'
						}).then(coderes => {
							this.getWeather(coderes.data.AddressCode)
						}).catch(err => {
							console.log('err', err);
						})
					}
				});
			},
			reminderList() {
				// 获取当前日期的年、月、日
				var today = new Date()
				var tYear = today.getFullYear();
				var tMonth = today.getMonth() + 1;
				var tDate = today.getDate();
				let statrt = tYear + '-' + tMonth + '-' + tDate + ' 00:00:00'
				let end = tYear + '-' + tMonth + '-' + (tDate + 7) + ' 23:59:59'
				// console.log(statrt,end,'11111') 
				getSevenPlan({
					PlanStartTime: statrt,
					PlanEndTime: end,
					pageSize: 10,
					pageNum: 1
				}).then(res => {
					// console.log("跟进计划", res);
					// var newDate=tYear+'-'+tMonth+'-'+tDate;
					var data = res.data.List;
					let strTime = ""
					let firstTime = []
					let secondTime = []
					if (data && data.length > 0) {
						data.forEach((row, index) => {
							//row.PlanTime='2024-9-01'
							strTime = Math.abs(new Date(dayjs(row.PlanTime).format('YYYY-MM-DD')) -
								new Date(dayjs(new Date()).format('YYYY-MM-DD')));
							secondTime = Math.ceil(strTime / (1000 * 60 * 60 * 24))
							row.Status = secondTime + '天后'
							this.planList = res.data.List
						})
					}
					// / console.log(tDate,'timeComparison')
				}).catch(err => {

				})
			},

			handSalesData() {
				this.salesDataHide = !this.salesDataHide
			},
			handSaleItem(ite) {
				this.timeDateSales = JSON.stringify(ite.value);
				this.saleTotDay = ite.title
				this.salesDataHide = false;
				this.list();
			},
			handClickItem(ite) {
				this.timeDate = JSON.stringify(ite.value);
				this.dayHide = false;
				this.totDay = ite.title
				//console.log(this.timeDate,'this.timeDate');
			},
			handClickDays() {
				this.dayHide = !this.dayHide
			},
			isCheckPermi(val) {
				return checkPermi(val)
			},
			dataFullt() {

			},
			handFollowed() {
				uni.navigateTo({
					url: '../../page_crm_moudule/followRrecords/AddFollowRecord'
				})
			},
			handFollowRoute() {
				uni.navigateTo({
					url: '../../page_crm_moudule/FollowPlan/addFollowPlan'
				})
			},
			handOpportRoute() {
				uni.navigateTo({
					url: '../../page_crm_moudule/Opportunity/additionOpportunity'
				})
			},
			handContactRoute() {
				uni.navigateTo({
					url: '../../page_crm_moudule/contacts/ContactAddition'
				})
			},
			handRouteItem() {
				uni.navigateTo({
					url: '../../page_crm_moudule/clue/addClue'
				})
			},
			handClueRoute() {
				uni.navigateTo({
					url: '../../page_crm_moudule/CluePool/addCluePool'
				})
			},
			handCustomersRoute() {
				uni.navigateTo({
					url: '../../page_crm_moudule/Customer/CreateCustomer'
				})
			},
			handSeas() {
				uni.navigateTo({
					url: '../../page_crm_moudule/OpenPool/addPool'
				})
			},
			handAddPage() {
				this.hide = !this.hide;
			},
			//我的授权
			handAuthorized() {
				uni.navigateTo({
					url: '/page_crm_moudule/MyAuthorization/index'
				})
			},
			//跟进记录
			handFollowRecord() {
				uni.navigateTo({
					url: '/page_crm_moudule/followRrecords/followRrecords'
				})
			},
			//跟进计划
			handFollowPlan() {
				uni.navigateTo({
					url: '/page_crm_moudule/FollowPlan/FollowPlan'
				})
			},
			//商机列表
			handOppor() {
				uni.navigateTo({
					url: '/page_crm_moudule/Opportunity/Opportunity'
				})
			},
			handContact() { //联系人
				uni.navigateTo({
					url: '/page_crm_moudule/contacts/contacts'
				})
			},
			handXian() { //线索
				uni.navigateTo({
					url: '/page_crm_moudule/clue/clue'
				})
			},
			handCluePool() { //线索池
				uni.navigateTo({
					url: '/page_crm_moudule/CluePool/CluePool'
				})
			},
			handCustomers() { //私海客户
				uni.navigateTo({
					url: '/page_crm_moudule/Customer/Customer'
				})

			},
			//公海池
			handHighseas() {
				uni.navigateTo({
					url: '/page_crm_moudule/OpenPool/OpenPool'
				})

			},
			list() {
				crmData({
					day: this.timeDateSales
				}).then((res) => {
					if (res.code == 0) {
						//console.log(res)
						this.salesData = res.data
					}
				}).catch((err) => {
					//console.log(err.message);
					this.setMsgTop(err)
				})
			}
		}
	}
</script>

<style lang="less">
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

	::v-deep.uni-navbar-btn-text span {
		font-size: 40rpx !important;
	}

	.device_list_con {
		background-color: #F5F8F9;
	}

	.alert {
		position: absolute;
		right: 15rpx;
		/* #ifdef H5*/
		top: 130rpx;
		/*#endif*/
		/* #ifndef H5*/
		top: 192rpx;
		/*#endif*/
		width: 240rpx;
		// height:634rpx;
		background: rgba(255, 255, 255, 1);
		color: rgba(51, 51, 51, 1);
		z-index: 229;
		border-radius: 9rpx;
		padding: 15rpx 30rpx;

		.t-icon-crmtianjiaguanbi {
			position: absolute;
			width: 36rpx;
			height: 36rpx;
			font-size: 60rpx;
			top: -44rpx;
			right: 10rpx;
		}

		.alert-item {
			height: 80rpx;
			line-height: 80rpx;
		}
	}

	.move {
		position: fixed;
		height: 100%;
		width: 100%;
		background: #000;
		opacity: .7;
		top: 0px;
		left: 0px;
		z-index: 222;
	}

	.fixedActive {
		position: fixed;
	}

	.fixedOn {
		position: relative;
	}

	#CRM {
		background: rgba(245, 248, 249, 1);
		height: 100%;
		// height:100vh;
		color: #fff;

		.CRM-bgHeader {
			position: relative;
			z-index: 1;
			width: 100%;
			// height: 686rpx;
			background: linear-gradient(180deg, rgba(222, 235, 250, 1), rgba(245, 248, 249, 1));

			.CRM-bgHeader-text {
				color: rgba(153, 153, 153, 1);
				font-size: 24rpx;
				margin-left: 30rpx;
				margin-right: 30rpx;
				display: flex;
				justify-content: space-between;
				align-items: center;
			}

			.weather {
				display: flex;
				align-items: center;
				justify-content: space-between;
				margin: 0rpx 30rpx;
				margin-top: 4rpx;

				.weather-right {
					width: 170rpx;
					height: 150rpx;

					.illustration {
						width: 100%;
						height: 100%;
					}
				}

				.weather_info {
					display: flex;
					justify-content: flex-start;

					.weather_img {
						width: 100rpx;
						height: 100rpx;
						margin-right: 20rpx;
					}

					.info_con {
						padding: 18rpx 0 20rpx 0;

						.wea_text {
							font-size: 24rpx;
							color: #999999;
							line-height: 24rpx;

							.text1 {
								margin-right: 10rpx;
							}
						}

						.tem_hum {
							display: flex;
							justify-content: flex-start;
							align-items: center;
							font-size: 22rpx;
							color: #999999;
							line-height: 22rpx;
							margin-top: 16rpx;

							.line {
								background-color: #999999;
								height: 22rpx;
								width: 2rpx;
								margin: 0 10rpx;
							}
						}
					}
				}

				.weather-left {
					display: flex;
					align-items: center;

					.weather-left-text {
						margin-left: 20rpx;
						color: rgba(153, 153, 153, 1);

						.title {
							font-size: 24rpx;
						}

						.content {
							font-size: 22rpx;
						}
					}

					.weather-left-logo {
						width: 100rpx;
						height: 100rpx;

						.weather-left-logo-image {
							width: 100%;
							height: 100%;
						}
					}

				}
			}
		}

		.CRM-Sales-data {
			width: 94%;
			margin: 20rpx 3%;
			background: #fff;
			border-radius: 10rpx;

			.FunnelPlot {
				margin-top: -100rpx;
				padding-bottom: 40rpx;

				::v-deepcanvas {
					height: 850rpx;
				}
			}

			.CRM-Sales-customers {
				display: flex;
				flex-wrap: wrap;
				align-items: center;
				padding-bottom: 18rpx;

				.CRM-Sales-customers-item {
					display: flex;
					flex-direction: column;
					align-items: center;
					width: 33%;
					text-align: center;
					margin-bottom: 25rpx;
					margin-top: 20rpx;

					.parse-text {
						line-height: 26rpx;
						margin-top: 10rpx;
					}

					.content {
						color: rgba(153, 153, 153, 1);
						font-size: 24rpx;
					}

					.num {
						font-size: 32rpx;
						color: rgba(51, 51, 51, 1);
						font-weight: bold;
					}

				}
			}

			.emotyNull {
				display: flex;
				align-items: center;
				color: rgba(153, 153, 153, 1);
				font-size: 26rpx;
				height: 136rpx;
				background: rgba(248, 248, 248, 1);
				margin: 20rpx 30rpx;
				border-radius: 10rpx;

				.emotyNull-img {
					width: 123rpx;
					height: 100rpx;
					margin: 0rpx 20rpx;
					margin-right: 30rpx;
				}
			}

			.FollowReminder {
				display: flex;
				flex-wrap: wrap;
				padding: 30rpx;
				// height:136rpx;
				margin: 0rpx 30rpx;
				margin-bottom: 20rpx;
				background: rgba(248, 248, 248, 1);
				border-radius: 10rpx;
				font-size: 26rpx;

				.FollowReminder-top {
					display: flex;
					width: 100%;
					align-items: center;
				}

				.FollowReminder-bottom {
					display: flex;
					align-items: center;
				}

				.Expired {
					font-size: 20rpx;
					border-radius: 4rpx;
					padding: 10rpx;
					margin-left: 16rpx;
				}

				.expiredActive {
					color: rgba(255, 53, 53, 1);
					background: rgba(255, 245, 245, 1);
				}

				.expiredOn {
					color: rgba(35, 113, 255, 1);
					background: rgba(233, 241, 255, 1);
				}

				.title {
					color: rgba(153, 153, 153, 1);
				}

				.content {
					color: rgba(51, 51, 51, 1);
				}
			}

			.CRM-Sales-data-header {
				position: relative;
				display: flex;
				padding: 25rpx;
				align-items: center;

				.CRM-Sales-data-header-title {
					display: flex;
					align-items: center;

					.newt-icon-genjintixing {
						display: inline-block;
						width: 36rpx;
						height: 36rpx;
					}

					.text {
						padding-left: 10px;
						color: #333333;
						font-weight: bold;
						font-size: 15px;
					}
				}

				.pointAlert {
					position: absolute;
					width: 200rpx;
					background: #f8f8f8;
					color: #333;
					right: 0rpx;
					top: 80rpx;
					border-radius: 12rpx;
					z-index: 100;
					box-shadow: 0rpx 0rpx 7rpx rgba(102, 102, 102, .2);

					.pointAlert-item {
						text-align: center;
						height: 80rpx;
						line-height: 80rpx;
					}
				}

				.CRM-Sales-data-header-right {
					margin-left: auto;
					color: rgba(153, 153, 153, 1);
					font-size: 24rpx;
					z-index: 100;

					.youjian {
						font-size: 12rpx;
						margin-left: 10rpx;

					}
				}

				.CRM-Sales-data-logo {
					color: #333333;

					.text {
						padding-left: 20rpx;
						color: rgba(51, 51, 51, 1);
						font-weight: bold;
						font-size: 30rpx;
					}
				}
			}

		}

		.crm-nav {
			display: flex;
			flex-wrap: wrap;
			text-align: center;
			font-size: 24rpx;
			position: relative;
			padding: 14rpx 0rpx;
			z-index: 2;
			margin: 0rpx 3%;
			// margin-top: -430rpx;
			background: #fff;
			border-radius: 10rpx;

			.crm-header-item {
				width: 33%;
				padding: 25rpx 0rpx;

				.crm-header-text {
					margin-top: 10rpx;
					color: rgba(102, 102, 102, 1);
					font-size: 24rpx;
				}

				.crm-header-logo {
					width: 44rpx;
					height: 44rpx;
					margin: 0 auto;
					font-size: 44rpx;
					color: #333333;
				}
			}

		}

		.CRM-header {
			font-size: 44rpx;
			font-weight: bold;


		}

		::v-deep.uni-navbar__content,
		.uni-navbar__header {
			background: none !important;
		}


		.bg_con {
			position: fixed;
			top: 0;
			width: 100%;
			// height: 100vh;
			z-index: 1;

			.bg_con_con {
				position: relative;
				width: 100%;
				height: 100%;

				.top_bg {
					position: absolute;
					top: 0;
					left: 0;
					width: 100%;
					height: 400rpx;
					background: linear-gradient(180deg, #DEEBFA 0%, #F5F8F9 100%);
				}

				.list_bg {
					position: absolute;
					top: 0;
					left: 0;
					width: 100%;
					background-color: #F5F8F9;
				}
			}
		}

		.top_info_title {
			display: flex;
			justify-content: space-between;
			align-items: center;
			line-height: 40rpx;
			transition: 2000;
			margin: 0rpx 30rpx;
			margin-bottom: 20rpx;

			.title_left {
				display: flex;
				justify-content: flex-start;
				align-items: center;

				.user_name {
					font-size: 40rpx;
					font-weight: bold;
				}
			}

			.title_right {
				display: flex;
				justify-content: flex-end;
				align-items: center;

				.icon_con {
					display: flex;
					justify-content: center;
					align-items: center;
				}
			}

			.icon_con {
				display: flex;
				align-items: center;
				justify-content: center;
				margin-left: 20rpx;
			}
		}
	}
</style>