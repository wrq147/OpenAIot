<script>
	import checkUpdate from '@/common/check-update.js'
	import phoneInfo from '@/common/js/phone-info.js';
	import uniPushListener from '@/common/js/unipush.js';
	import {
		getToken,
		getRefreshToken,
	} from '@/common/auth.js'
	import {
		deviceIsOnline
	} from '@/api/wifi.js'
	import {testIsOk} from '@/api/commonApi.js'
	import {
		getUserInfo,
	} from "@/api/login";
	import store from './store'
	import {
		logout
	} from '@/api/login.js'
		var shouldJumpIndex = true
	export default {
		data() {
			return {
			}
		},
		onLaunch: function() {
			console.warn('当前组件仅支持 uni_modules 目录结构 ，请升级 HBuilderX 到 3.1.0 版本以上！')
			console.log('App Launch')
			testIsOk().then((res)=>{
				this.Checkforupdates()
				this.routeJump()
			}).catch(err=>{
				this.Checkforupdates()
				this.routeJump()
			})
		},
		onShow: function() {
			console.log('App Show')
			uni.hideTabBar({
				animation: false
			})
			this.$store.commit('SET_isSetNotimeoutTips', true)
			setTimeout(()=>{
				this.$store.commit('SET_isSetNotimeoutTips', false)
			},10000)
			// #ifdef APP-PLUS
			let urlMap = {
				index: "/pages/device/device"
			}
			setTimeout(function() {
				var args = plus.runtime.arguments.split("://page=")[1];
				// util.toast(urlMap[args])
				console.log(args, 'argsargsargs');
				if (args) {
					plus.runtime.arguments = '';
					uni.reLaunch({
						url: args,
						// #ifdef APP-PLUS
						success: () => {
							shouldJumpIndex = false
							plus.navigator.closeSplashscreen();
						},
						// #endif
					})
				}
			}, 10);
			// #endif

		},
		onHide: function() {
			console.log('App Hide')
			this.$store.commit('SET_isSetNotimeoutTips', true)
			this.backLogout()
		},
		methods: {
			Checkforupdates(){
				// #ifdef APP-PLUS
				
				plus.screen.lockOrientation("portrait-primary")
				setTimeout(() => {
					uniPushListener.getInfo();
					plus.push.setAutoNotification(true); //设置通知栏显示通知 //必须设置
					plus.screen.lockOrientation('portrait-primary'); //锁定屏幕方向
					uni.setStorageSync('cancelUpdate', 'false'); // 进来APP 重置更新弹窗
					// 获取App 当前版本号
					if (Object.keys(uni.getStorageSync('widgetInfo')).length == 0) {
						plus.runtime.getProperty(plus.runtime.appid, widgetInfo => {
							console.log('app信息', widgetInfo);
							phoneInfo.manifestInfo = widgetInfo;
							uni.setStorageSync('widgetInfo', widgetInfo);
						});
					}
					uniPushListener.getQuanxian()
					uniPushListener.getClientInfoLoop(); // 循环获取cid
					uniPushListener.pushListener(); // 监听通知栏信息
				
					plus.runtime.setBadgeNumber(0); //清除app角标
					plus.runtime.setBadgeNumber(-1);
				}, 100)
				const systemInfo = uni.getSystemInfoSync();
				let globalData = {
					isIpx: (systemInfo.screenHeight / systemInfo.screenWidth) > 1.86,
					isAndroid: systemInfo.platform.toLowerCase().indexOf('android') > -1,
					isIOS: systemInfo.platform.toLowerCase().indexOf('ios') > -1, //
				}
				this.$store.commit('SET_GLOBAL_DATA', globalData) //保存设备信息到本地
				
				//检查更新
				checkUpdate();
				// #endif
			},
			backLogout() {
				logout(true).then(res => {
					// console.log(res);
				}).catch(err => {

				})

			},
			routeJump(){
				setTimeout(async () => {
					if (getToken() && getRefreshToken()) {
						// console.log(this.$store.state.user.roles&&this.$store.state.user.roles.length>0);
						try {
							if(this.$store.state.user.roles&&this.$store.state.user.roles.length>0){
							}else{
								let rsp = await getUserInfo({
									id: 0
								})
								await this.$store.dispatch('GetInfo')
								this.$store.commit('SET_ROLES',rsp.data.user.roleIds)
							}
							this.$store.commit('orgLis/SET_ORG_LIST', null)
							let list = await this.$store.dispatch("orgLis/setOrgList");
							if (list && list.length > 0) {
								setTimeout(() => {
									if (shouldJumpIndex) {
										uni.switchTab({
											url: '/pages/home/home',
											// #ifdef APP-PLUS
											success: () => {
												plus.navigator.closeSplashscreen();
											},
											// #endif
										})
									}
									// uni.hideLoading()
									return
								}, 500)
							} else {
								setTimeout(() => {
									if (shouldJumpIndex) {
										uni.navigateTo({
											url: '/page_register/choose_addorg?isAppReg=true',
											// #ifdef APP-PLUS
											success: () => {
												plus.navigator.closeSplashscreen();
											},
											// #endif
										})
									}
								}, 500)
				
							}
						} catch (e) {
							//TODO handle the exception
							setTimeout(() => {
								if (shouldJumpIndex) {
									uni.reLaunch({
										url: '/pages/index/login',
										// #ifdef APP-PLUS
										success: () => {
											console.log("这里");
											plus.navigator.closeSplashscreen();
										},
										// #endif
									})
								}
							}, 500)
						}
					} else {
						setTimeout(() => {
							if (shouldJumpIndex) {
								uni.reLaunch({
									url: '/pages/index/login',
									// #ifdef APP-PLUS
									success: () => {
										console.log("这里");
										plus.navigator.closeSplashscreen();
									},
									// #endif
								})
							}
						}, 500)
					}
				},10)
			}
		}
	}
</script>

<style lang="scss">
	* {
		-webkit-user-select: text;
	}

	/*每个页面公共css */
	@import '@/uni_modules/uni-scss/index.scss';
	@import '@/static/iconfont/iconfont.css';
	@import '@/static/iconfont/iconfont-weapp-icon.css';
	@import '@/style/common.scss';
	@import '@/style/crmStyle.scss';

	/* #ifndef APP-NVUE */
	// @import '@/static/customicons.css';
	// 设置整个项目的背景色
	/* #endif */
	page {
		background-color: #161A26;
		webkit-user-select: text;
	}

	.example-info {
		font-size: 14px;
		color: #333;
		padding: 10px;
	}

	uni-modal {
		z-index: 199999 !important;
	}
</style>