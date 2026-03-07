<template>
	<view class="pages_bgcon">
		<!--#ifdef H5 -->
		<jump-down></jump-down>
		<!--#endif -->
		<view class="bg_con" :style="{'height':Number(statusBarHeight*2)+662+'rpx'}">
			<view class="bg_con_con">
				<view class="top_bg" :style="{'height':Number(statusBarHeight*2)+662+'rpx'}">
				</view>
				<!-- <view v-if="isShowListBg" class="list_bg" :style="{'height':Number(statusBarHeight)+204+'rpx','top':Number(statusBarHeight)+174+'rpx'}"></view> -->
			</view>
		</view>

		<!-- <top :title="topTitle" leftWidth="0rpx" :titleIsLeft="true" :isNoLeftPadding="true" :rightWidth="120"
			:backgroundColor="ischangeTopBg?'#fff':''" :fixed="true" rightIcon="icon-tianjia"
			@clickRight="toAddrules">
		</top> -->
		<search-compt @closeSearch="closeSearchDiv" @searchInput="searchInput" @searching="searching" pal="请输入规则名称"
			:fixed="true" :isNoBg="false" backgroundColor="#fff" inputBg="#F8F8F8" :isRightText="true"
			v-if="showDeviceSearch" :hasKey="key" :statusbar="true"></search-compt>
		<select-compt ref="selectCompt" :selectListParam="selectListParam" :querydata="querydata"
			@selectFinsh="selectFinsh" @close="selectClose" :notOnlySearch='false'></select-compt>
		<view class="top_info_con" :style="{'padding-top':statusBarHeight*2+50+'rpx'}"
			:class="{'hide_top_info':isShowFixedTitle}">
			<view class="top_info_title" :style="{'opacity': titleOpac}">
				<view class="title_left">
					<view class="user_name">规则引擎</view>
				</view>
				<!--#ifndef MP-WEIXIN -->
				<view class="title_right">
					<view class="icon_con" @click.stop="openDeviceSearch">
						<custom-icons iconsName="icon-sousuo" iconsSize="36rpx" iconsColor="#333"></custom-icons>
					</view>
					<view class="icon_con" style="margin-left: 36rpx;" @click.stop="toAddrules">
						<custom-icons iconsName="icon-tianjia" iconsSize="36rpx" iconsColor="#333"></custom-icons>
					</view>
				</view>
				<!--#endif -->
			</view>
			<view class="device_num_tips" :style="{'opacity': numOpac}">
				您已创建{{total}}条规则
				<!--#ifdef MP-WEIXIN -->
				<view class="title_right">
					<view class="icon_con" @click.stop="openDeviceSearch">
						<custom-icons iconsName="icon-sousuo" iconsSize="36rpx" iconsColor="#333"></custom-icons>
					</view>
					<view class="icon_con" style="margin-left: 36rpx;" @click.stop="toAddrules">
						<custom-icons iconsName="icon-tianjia" iconsSize="36rpx" iconsColor="#333"></custom-icons>
					</view>
				</view>
				<!--#endif -->
			</view>
			<view class="bg_info" :style="{'opacity': weatherOpac}">
				<view class="weather_info" v-if="weatherInfo&&weatherInfo.weather">
					<image v-if="getSerVerUrl()" class="weather_img" :src="getWeatherImg(weatherInfo.weather)" mode=""></image>
					<view class="info_con">
						<view class="wea_text">
							<text class="text1">{{weatherInfo.city}}</text>
							<text>{{weatherInfo.weather}}</text>
						</view>
						<view class="tem_hum">
							<view class="text">温度{{weatherInfo.temperature}}℃</view>
							<view class="line"></view>
							<view class="text">湿度{{weatherInfo.humidity}}%</view>
						</view>
					</view>
				</view>
				<view class="weather_info" v-else>
					<image v-if="getSerVerUrl()" class="weather_img" :src="getSerVerUrl()+'/appimg/weather_img/duoyun.png'" mode=""></image>
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
				<view class="right_bg">
					<image v-if="getSerVerUrl()" class="bg_image" :src="getSerVerUrl()+'/appimg/chahua_rules.png'" mode=""></image>
				</view>
			</view>
			<view class="search_result"
				v-if="key||querydata.way||querydata.way==0||querydata.status||querydata.status==0"
				style="margin-bottom: 20rpx;">
				已为您搜索<text v-if="key">“{{key}}”</text><text
					v-if="querydata.way||querydata.way==0">“{{getRulesType(querydata.way)}}”</text><text
					v-if="querydata.status||querydata.status==0">“{{getRulesStatus(querydata.status)}}”</text>
			</view>
		</view>
		<view class="rules_title_fiex" id="minscrolldom" v-if="isShowFixedTitle"
			:style="{'height':Number(statusBarHeight*2)+88+'rpx','padding-top':Number(statusBarHeight*2)+'rpx'}">
			规则引擎
		</view>
		<!-- <view v-if="isShowFixedTitle" class="fixed_top_zhanwei" :style="{'height':Number(statusBarHeight*2)+88+'rpx'}">
		</view> -->
		<view class="rules_list_con" id="rules_listcon" v-if="rulesList&&rulesList.length>0||status=='loading'">
			<view class="rules_li" v-for="(item,index) in rulesList" @click="editRules(item)"
				:style="{'margin-top':index==0?'0':'20rpx'}">
				<view class="title">
					<view class="sign_con_con">
						<view class="sign_con" v-if="!item.CreatedFrom||item.CreatedFrom&&item.CreatedFrom=='pc'">PC</view>
					</view>
					<text class="text">{{item.Name}}</text>
					
				</view>
				<view class="cont_con">
					<view class="cot_left">
						<view class="dev_img">
							<view class="image t-icon-dingshi" v-if="item.TriggerWay==2"></view>
							<view class="image t-icon-shijian" v-if="item.TriggerWay==0"></view>
							<view class="image t-icon-shuxing" v-if="hasTiaojian(item)"></view>
						</view>
						<view class="lianjiefu"v-if="hasShijian(item)||hasGongneng(item)">
							<custom-icons iconsName="icon-a-youjiantoubai" iconsSize="20rpx"
								iconsColor="#999999"></custom-icons>
						</view>
						<view class="dev_img">
							<view class="image t-icon-shijian1" v-if="hasShijian(item)"></view>
							<view class="image t-icon-gongneng11" v-if="hasGongneng(item)"></view>
						</view>
					</view>
					<view class="cot_right">
						<view class="input_enter_con" @click.stop="jumpToEditParams(item)" v-if="!item.CreatedFrom&&item.EnableParam||item.CreatedFrom&&item.CreatedFrom=='pc'&&item.EnableParam">
							<text class="text">修改参数</text>
							<custom-icons iconsName="icon-a-youjiantoubai" iconsSize="16rpx"
								iconsColor="#999999"></custom-icons>
						</view>
						<view class="conten_icon_con">
							<view class="conten_icon t-icon-kaibeifen" v-if="item.Status==0" @click.stop="setStatus(item,item.Status)"></view>
							<view class="conten_icon t-icon-guan1" v-else-if="item.Status==1" @click.stop="setStatus(item,item.Status)"></view>
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
			<view class="empty_text">点击右上角“+”创建规则</view>
		</view>
		<msg-prompt ref="promptMsg" @confirm="confirmHandle"></msg-prompt>
		<my-tab-bar ref="mytab" active="Rules"></my-tab-bar>
	</view>
</template>

<script>
	import {
		rulesListFun,
		editRuselServe
	} from "@/api/ruselSevic";
	import {
		getWeatherInfo,
		getGeocoder
	} from '@/api/weather.js'
	import {
		retunWeatherImg
	} from '@/common/weatherInfo.js'
	import serverUrl from '@/common/constVar.js'
	export default {
		data() {
			return {
				isDownRefresh: false,
				status: 'loading',
				rulesList: [],
				ischangeTopBg: false,
				statusBarHeight: uni.getSystemInfoSync().statusBarHeight,
				topTitle: '规则引擎',
				querydata: {
					pageNum: 1,
					pageSize: 12,
				},
				selectListParam: [{
					name: '触发方式',
					params: 'way',
					pal: '请选择触发方式',
					value: null,
					isTransform: true,
					localdata: [{
							text: "设备触发",
							value: "0"
						},
						{
							text: "Http触发",
							value: "1"
						},
						{
							text: "定时触发",
							value: "2"
						}
					]
				}, {
					name: '状态',
					params: 'status',
					pal: '请选择过滤状态',
					value: null,
					isTransform: true,
					localdata: [{
							text: "正常",
							value: "0"
						},
						{
							text: "暂停",
							value: "1"
						}
					]
				}],
				isShowFixedTitle: false,
				showDeviceSearch: false,
				total: 0, //规则总条数
				key: '', //搜索关键字
				weatherInfo: {},
				titleOpac: 1,
				numOpac: 1,
				weatherOpac: 1,
				defaultTop: 0,
				timer: null,
				setShowScroll:false,//是否要回滚到顶部
			};
		},
		async onLoad() {
			if (this.$store.state.user && this.$store.state.user.uid) {} else {
				await this.$store.dispatch('GetInfo')
			}
			this.$nextTick(() => {
				this.$refs.mytab.loadCheck()
				let query = uni.createSelectorQuery().in(this);
				query.select('#rules_listcon').boundingClientRect(data => {
					if (data) {
						this.defaultTop = data.top
					}
				}).exec()
			})
			this.getList()
			if (serverUrl.getServerUrl() == 'http://125.124.98.180:7080'||serverUrl.getServerUrl() == 'http://iot.wookongcloud.com') {
				this.getLocationInfo()
			}

		},
		onShow() {
			if(this.setShowScroll){
				this.setShowScroll=false
				setTimeout(()=>{
					uni.pageScrollTo({
						scrollTop: 1,
						selector: '#rules_listcon',
						duration: 300
					});
				},500)
			}
			this.$nextTick(()=>{
				if(this.$store.state.isReloadMessage){
					this.$refs.mytab.getNoReadCount()
				}
			})
		},
		beforeDestroy() {
			clearInterval(this.timer)
		},
		onUnload() {
			clearInterval(this.timer)
		},
		onPullDownRefresh() {
			this.querydata.pageNum = 1
			this.status = "loading";
			this.isDownRefresh = true
			this.getList()

		},
		onReachBottom() { //上拉触底
			if (this.status != 'noMore') {
				this.querydata.pageNum++;
				this.status = "loading";
				this.getList();
			}
		},
		onPageScroll() {
			this.getElementTop()
		},
		methods: {
			jumpToEditParams(item){
				uni.navigateTo({
					url:'/pages_rules/edit_params/edit_params?id='+item.Id+'&name='+item.Name
				})
			},
			setStatus(item, sta) {
				let id=item.Id
				//设置规则状态
				if (sta == 0) {
					uni.showModal({
						title: '警告',
						content: '是否确认暂停规则"' + item.Name + '"?',
						showCancel: true,
						success: (res) => {
							if (res.confirm) {
								let par = {
									id: id,
									status: 1
								};
								editRuselServe(par).then((rsp) => {

										if (rsp.code == 0) {
											this.querydata.pageNum=1
											this.getList();
											this.$refs.promptMsg.open('该规则已暂停', 2000) //提示信息组件
										}
									})
									.catch((err) => {
										this.setMsgTop(err)
									});
							}
						}
					});
				} else if (sta == 1) {
					uni.showModal({
						title: '警告',
						content: '是否确认启用规则"' + item.Name + '"?',
						showCancel: true,
						success: (res) => {
							if (res.confirm) {
								let par = {
									id: id,
									status: 0
								};
								editRuselServe(par)
									.then((rsp) => {
										if (rsp.code == 0) {
											this.querydata.pageNum=1
											this.getList();
											this.$refs.promptMsg.open('该规则已启用', 2000) //提示信息组件
										}
									})
									.catch((err) => {
										this.setMsgTop(err)
									});
							}
						}
					});
				}
			},
			getLocationInfo() {
				//获取本地地址信息
				uni.getLocation({
					type: 'wgs84',
					success: (res) => {
						getGeocoder({
							location: res.latitude + ',' + res.longitude,
							maptype: 'lc'
						}).then(async coderes => {
							await this.getWeather(coderes.data.AddressCode)
							this.timer = setInterval(async () => {
								await this.getWeather(coderes.data.AddressCode)
							}, 50000)

						}).catch(err => {
						})
					},
					fail: (res) => {
						if (res.errMsg == "getLocation:fail auth deny") {
							uni.showModal({
								content: '检测到您没打开获取信息功能权限，是否去设置打开？',
								confirmText: "确认",
								cancelText: '取消',
								success: (res) => {
									if (res.confirm) {
										uni.openSetting({
											success: (res) => {}
										})
									} else {
										return false;
									}
								}
							})
						}
					}
				});
			},
			getWeatherImg(val) {
				//获取天气形象
				return retunWeatherImg(val)
			},
			async getWeather(val) {
				try {
					let res = await getWeatherInfo(val)
					if (res.data.city) {
						res.data.city = res.data.city.substring(0, res.data.city.length - 1)
					}
					this.weatherInfo = res.data
				} catch (err) {
					//TODO handle the exception
					if (this.timer) {
						clearInterval(this.timer)
					}
				}
			},
			getRulesType(val) {
				if (val == 0) {
					return '设备触发'
				} else if (val == 1) {
					return 'Http触发'
				} else if (val == 2) {
					return '定时触发'
				}
			},
			getRulesStatus(val) {
				if (val == 0) {
					return '正常'
				} else if (val == 1) {
					return '暂停'
				}
			},
			openDeviceSearch() {
				//打开设备搜索
				this.showDeviceSearch = true
				this.openSelect()
			},
			closeSearchDiv() {
				//隐藏设备搜索
				this.showDeviceSearch = false
				this.openSelect()
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
			hasTiaojian(item) {
				let process = JSON.parse(item.RuleJson)
				return this.determineTiaojian(process)
			},
			determineTiaojian(process) {
				if (process.type == "CONDITIONS") {
					return true
				} else {
					if (process.children && process.children != {}) {
						return this.determineTiaojian(process.children)
					} else {
						return false
					}
				}
			},
			hasGongneng(item) {
				let process = JSON.parse(item.RuleJson)
				return this.determineGongneng(process)
			},
			hasShijian(item){
				let process = JSON.parse(item.RuleJson)
				return this.determineShijian(process)
			},
			determineShijian(process) {
				if (process.type == "WARN") {
					return true
				} else {
					if (process.children && process.children != {}) {
						return this.determineShijian(process.children)
					} else {
						return false
					}
				}
			},
			determineGongneng(process) {
				if (process.type == "FUNC") {
					return true
				} else {
					if (process.children && process.children != {}) {
						return this.determineGongneng(process.children)
					} else {
						return false
					}
				}
			},
			editRules(item) {
				//修改规则
				if(item.CreatedFrom&&item.CreatedFrom=='mobile'){
					uni.navigateTo({
						url: '/pages_rules/rules_add?id=' + item.Id
					})
				}
				
			},
			toAddrules() {
				//添加规则
				uni.navigateTo({
					url: '/pages_rules/rules_add'
				})
			},
			loadData(query) {
				if (query && query == 'load') {
					this.setShowScroll=true
					this.querydata.pageNum = 1
					this.status='loading'
					this.getList()
				}
			},
			getList() {
				if (this.querydata.pageNum == 1) {
					this.rulesList = []
				}
				rulesListFun(this.querydata).then(response => {
					// console.log("规则列表",response);
					if (response.code == 0) {
						this.rulesList = [...this.rulesList, ...response.data.List];
						// for(let i=0;i<10;i++){
						// 	this.rulesList = [...this.rulesList,...response.data.List];
						// }
						this.total = response.data.Total;
						if (response.data.List.length < this.querydata.pageSize) {
							this.status = 'noMore';
						} else {
							this.status = 'more';
						}
					}
					if (this.isDownRefresh) {
						uni.stopPullDownRefresh()
						this.isDownRefresh = false
					}
					// this.myProcessList = response.data.List;
					// this.total = response.data.Total;
					// this.loading = false;
				}).catch(err => {
					this.status = 'noMore';
					if (this.isDownRefresh) {
						uni.stopPullDownRefresh()
						this.isDownRefresh = false
					}
					this.setMsgTop(err)
				});
			},
			confirmHandle() {
				//弹出提示框的确认事件
			},
			searchInput(val) {
				this.key = val
			},
			searching(val) {
				this.key = val
				this.closeSearchDiv()
				if (this.key) {
					this.querydata.key = this.key
					this.querydata.pageNum = 1
					this.status = 'loading'
					this.rulesList = []
					this.getList()
				} else {
					delete this.querydata.key
					this.querydata.pageNum = 1
					this.status = 'loading'
					this.rulesList = []
					this.getList()
				}
			},
			openSelect() {
				this.$refs.selectCompt.openSelect()
				this.ischangeTopBg = !this.ischangeTopBg
			},
			selectClose() {
				this.ischangeTopBg = false
				this.showDeviceSearch = false
			},
			selectFinsh(query, isresat) {
				this.querydata = JSON.parse(JSON.stringify(query))
				if (isresat) { //是否是重置过滤
					this.key = ''
				}
				if (this.key) {
					this.querydata.key = this.key
				} else {
					delete this.querydata.key
				}
				this.$forceUpdate()
				this.querydata.pageNum = 1
				this.status = 'loading'
				this.rulesList = []
				this.getList()
			}
		}
	}
</script>

<style lang="less" scoped>
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
	}

	.fixed_top_zhanwei {
		width: 100%;
	}

	.rules_list_con {
		padding: 0 20rpx;
		width: 100%;
		box-sizing: border-box;
		position: relative;
		z-index: 1;

		.rules_li {
			padding: 23rpx 30rpx 30rpx;
			box-sizing: border-box;
			border-radius: 10rpx;
			width: 100%;
			background-color: #ffffff;
			margin-top: 20rpx;

			.title {
				font-size: 30rpx;
				color: #333333;
				font-weight: 550;
				line-height: 45rpx;
				display: flex;
				align-items: flex-start;
				.sign_con_con{
					display: flex;
					justify-content: center;
					align-items: flex-start;
					padding-top: 7.5rpx;
					.sign_con{
						margin-right: 10rpx;
						width: 44rpx;
						height: 30rpx;
						line-height: 20rpx;
						box-sizing: border-box;
						padding: 0 8rpx;
						border-radius: 6rpx;
						background: rgba(233, 241, 255, 1);
						font-size: 20rpx;
						color: rgba(35, 113, 255, 1);
						font-weight: normal;
						display: flex;
						justify-content: center;
						align-items: center;
					}
				}
				
			}

			.cont_con {
				display: flex;
				justify-content: space-between;
				margin-top: 23rpx;

				.conten_icon {
					width: 76rpx;
					height: 40rpx;
				}

				.cot_left {
					display: flex;
					justify-content: flex-start;
					align-items: center;

					.lianjiefu {
						margin-right: 20rpx;
					}

					.dev_img {
						display: flex;
						justify-content: flex-start;
						align-items: center;

						.image {
							margin-right: 20rpx;
							width: 48rpx;
							height: 48rpx;
						}
					}
				}
				.cot_right{
					display: flex;
					justify-content: flex-end;
					align-items: center;
					.input_enter_con{
						display: flex;
						justify-content: flex-end;
						align-items: center;
						margin-right: 40rpx;
						.text{
							margin-right: 8rpx;
							font-size: 24rpx;
							color: rgba(153, 153, 153, 1);
						}
					}
				}
				
			}
		}
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
</style>