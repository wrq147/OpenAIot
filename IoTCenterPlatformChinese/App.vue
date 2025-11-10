<script>
	import checkUpdate from '@/common/check-update.js'
	// import phoneInfo from '@/common/js/phone-info.js';
	// import uniPushListener from '@/common/js/unipush.js';
	import {
		getToken,
		getRefreshToken
	} from '@/common/auth.js'
	import {
		logout
	} from '@/api/login.js'
	// #ifdef H5
	var jWeixin = require('jweixin-module')
	// #endif
	import {
		CorpWxConfigJson
	} from "@/api/H5Login.js";
	import {
		getConfigKey
	} from '@/api/config.js'
	var shouldJumpIndex = true
	import {
		jumpLogin,
		prohibitJumping
	} from "@/common/utillib.js"
	export default {
		data() {
			return {
				// shouldJumpIndex: true
			}
		},
		async onLaunch(options) {
			// this.shouldJumpIndex=true
			// var cururl2 = window.location.href.split('#')[0];
			// console.log(window.location.href,'window.location.href11111');
			// return
			// #ifdef H5
			//设置如果是pc端直接打开管理平台的页面
			let systemIn=uni.getSystemInfoSync().system
			let ua1 = window.navigator.userAgent.toLowerCase();
			if(systemIn.indexOf('Windows') > -1&&/wxwork/i.test(ua1)){
				window.location.href='/#/wxworkLogin'
				return
			}
			// #endif
			try {
				// if (this.$store.state && this.$store.state.loginOrg) {
				// 	await this.$store.dispatch('setThemeInfo')
				// }
				let time = 0
				setInterval(() => {
					time++
				}, 1000)
				console.warn('当前组件仅支持 uni_modules 目录结构 ，请升级 HBuilderX 到 3.1.0 版本以上！')
				// console.log('App Launch')
				// #ifdef APP-PLUS
				// plus.screen.lockOrientation("portrait-primary")
				// setTimeout(() => {
				// 	uniPushListener.getInfo();
				// 	plus.push.setAutoNotification(true); //设置通知栏显示通知 //必须设置
				// 	plus.screen.lockOrientation('portrait-primary'); //锁定屏幕方向
				// 	uni.setStorageSync('cancelUpdate', 'false'); // 进来APP 重置更新弹窗
				// 	// 获取App 当前版本号
				// 	if (Object.keys(uni.getStorageSync('widgetInfo')).length == 0) {
				// 		plus.runtime.getProperty(plus.runtime.appid, widgetInfo => {

				// 			// console.log('app信息', widgetInfo);
				// 			phoneInfo.manifestInfo = widgetInfo;
				// 			uni.setStorageSync('widgetInfo', widgetInfo);
				// 		});
				// 	}
				// 	uniPushListener.getClientInfoLoop(); // 循环获取cid
				// 	uniPushListener.pushListener(); // 监听通知栏信息

				// 	plus.runtime.setBadgeNumber(0); //清除app角标
				// 	plus.runtime.setBadgeNumber(-1);
				// }, 100)
				const systemInfo = uni.getSystemInfoSync();
				let globalData = {
					isIpx: (systemInfo.screenHeight / systemInfo.screenWidth) > 1.86,
					isAndroid: systemInfo.platform.toLowerCase().indexOf('android') > -1,
					isIOS: systemInfo.platform.toLowerCase().indexOf('ios') > -1, //
				}
				this.$store.commit('SET_GLOBAL_DATA', globalData) //保存设备信息到本地

				// //检查更新//换成根据主题获取最新版本的app
				checkUpdate();
				// #endif
				// #ifdef H5
				try{//企业微信登录配置
					let res = await getConfigKey("org.wxAppId");
					// console.log(res,'企业微信设置');
					if(res.data){
						let qywxAppId = res.data
						let cururl = location.href
						this.$store.commit('SET_qywxAppId', qywxAppId);
						await this.requestJsApiConfig(qywxAppId, cururl)
					}
				}catch(e){
					
				}
				// #endif
				let orgList = []
				if (getToken() && getRefreshToken()) {
					this.$store.commit('orgLis/SET_ORG_LIST', null)
					orgList = await this.$store.dispatch("orgLis/setOrgList");
					await this.$store.dispatch('GetInfo')
				}
				// #ifdef H5
				var ua = window.navigator.userAgent.toLowerCase();
				// console.log(this.$store.state.qywxAppId,'this.$store.state.qywxAppId');
				// ||this.$store.state.qywxAppId
				if (/wxwork/i.test(ua)) {//企业微信不再跳转
					return
				}
				// #endif
				
				this.routeJump(orgList)
			} catch (e) {
				console.log("e", e);
				//TODO handle the exception
				this.routeJump()
			}

		},
		onShow() {
			console.log('App Show')
			uni.hideTabBar({
				animation: false
			})
			this.$store.commit('SET_isSetNotimeoutTips', true)
			setTimeout(() => {
				this.$store.commit('SET_isSetNotimeoutTips', false)
			}, 2000)
			// #ifdef APP-PLUS
			setTimeout(function() {
				var args = plus.runtime.arguments.split("://page=")[1];
				// util.toast(urlMap[args])
				// console.log(args, 'argsargsargs');
				if (args) {
					plus.runtime.arguments = '';
					uni.navigateTo({
						url: args,
						// #ifdef APP-PLUS
						success: () => {
							shouldJumpIndex = false
							plus.navigator.closeSplashscreen();
						},
						// #endif
					})
				}
			}, 20);
			// #endif

		},
		onHide() {
			console.log('App Hide')
			this.$store.commit('SET_isSetNotimeoutTips', true)
			this.backLogout()
		},
		methods: {
			async requestJsApiConfig(appid, url) {
				try {
					let result = await CorpWxConfigJson(appid, url)
					var data = result.data;
					var wxdata = {
						debug: false, // 开启调试模式,调用的所有api的返回值会在客户端alert出来，若要查看传入的参数，可以在pc端打开，参数信息会通过log打出，仅在pc端时才会打印。
						appId: data.corpid, // 必填，公众号的唯一标识
						timestamp: data.timestamp, // 必填，生成签名的时间戳
						nonceStr: data.nonceStr, // 必填，生成签名的随机串
						signature: data.signature, // 必填，签名
						jsApiList: ['checkJsApi',
							'scanQRCode', // 微信扫一扫接口
							'chooseImage', // 微信拍照接口
							'uploadImage', //上传图片接口与
							'previewImage', //预览
							'getLocation', //获取位置
							'openLocation', //地图
							'chooseLocation', //选择地图
							'startRecord', 
							'stopRecord', 
							'onVoiceRecordEnd',
							'translateVoice',
							'onLocationChange',
							'stopAutoLBS',
							'startAutoLBS'
						]
					};

					jWeixin.config(wxdata);
				} catch (e) {
					//TODO handle the exception
					console.log(e, 'rrrrrr');
					alert("错误11111")
					alert(JSON.stringify(e))
				}
			},
			getUrlParam(name) {
				let reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
				let r = window.location.search.substr(1).match(reg);
				console.log(r, 'rrrrrrr', window.location.search);
				if (r != null) return unescape(r[2]);
				return null;
			},
			backLogout() {
				logout(true).then(res => {}).catch(err => {

				})
			},
			routeJump(list) {
				setTimeout(async () => {
					// console.log(getToken() && getRefreshToken(),'getToken() && getRefreshToken()');
					if (getToken() && getRefreshToken()) {
						try {
							if (list == undefined) {
								this.$store.commit('orgLis/SET_ORG_LIST', null)
								list = await this.$store.dispatch("orgLis/setOrgList");
							}
							if (list && list.length > 0) {
								setTimeout(() => {
									
									if (shouldJumpIndex&&prohibitJumping()) {
										uni.switchTab({
											url: this.$store.state.homejumpurl ? this.$store
												.state.homejumpurl : '/pages/index/index',
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
									if (shouldJumpIndex&&prohibitJumping()) {
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
								if (shouldJumpIndex&&prohibitJumping()) {
									jumpLogin()
								}
							}, 500)
						}
					} else {
						setTimeout(() => {
							if (shouldJumpIndex&&prohibitJumping()) {
								jumpLogin()
							}
						}, 500)
					}
				}, 10)
			}
		}
	}
</script>

<style lang="scss">
	* {
		// -webkit-user-select: text;
		-webkit-user-select: none;
	}

	/*每个页面公共css */
	@import '@/uni_modules/uni-scss/index.scss';
	@import '@/static/iconfont/iconfont.css';
	@import '@/static/iconfont/iconfont-weapp-icon.css';
	// @import '@/static/iconfont2/iconfont-weapp-icon.css';
	@import '@/style/common.scss';
	@import '@/style/crmStyle.scss';

	/* #ifndef APP-NVUE */
	// @import '@/static/customicons.css';
	// 设置整个项目的背景色
	/* #endif */
	page {
		// background-color: rgba(245, 248, 249, 1);
		background-color: #f8f8f8;
		// webkit-user-select: text;
		-webkit-user-select: none;
		// position: relative;
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