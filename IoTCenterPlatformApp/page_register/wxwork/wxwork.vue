<template>
	<view>
		<!-- {{errMsgInfo}} -->
		<msg-prompt ref="promptMsg"></msg-prompt>
	</view>
</template>

<script>
	import request from '@/common/request.js'
	import {
		LoginByWxCorp,
		wxBaseUrl,
		CorpWxConfigJson
	} from "@/api/H5Login.js";
	import {
		getToken,
		getRefreshToken,
		setToken,
		setRefreshToken
	} from '@/common/auth.js'
	import {
		deviceIsOnline
	} from '@/api/wifi.js'
	import {
		getUserInfo,
	} from "@/api/login";
	import {
		getConfigKey
	} from '@/api/config.js'
	// #ifdef H5
	var jWeixin = require('jweixin-module')
	// #endif
	export default {
		data() {
			return {
				t: '',
				code: '',
				errMsgInfo: ''
			}
		},
		async onLoad(options) {
			if (options.t) {
				this.t = decodeURIComponent(options.t)
			}
			if (options) {
				this.options = JSON.parse(JSON.stringify(options))
			}
			//#ifdef H5
			let qywxAppId = ""
			try{
				
				let res = await getConfigKey("org.wxAppId");
				// console.log(res,'默认的企业微信appId');
				qywxAppId = res.data
				this.$store.commit('SET_qywxAppId', qywxAppId);
			}catch(e){
				
			}
			if (qywxAppId && options.code) {
				this.code = options.code
			}
			//首先orgId通过链接传过来,然后根据这个值获取appId
			if (qywxAppId) {
				try {
					this.NoTokenExe(qywxAppId, isNotAutoCreate)
					let isNotAutoCreate = this.$store.state.isNotAutoCreate
				} catch (e) {
					//TODO handle the exception
					console.log(e, '11111');
					this.setMsgTop(e)
				}
			} else {
				let urlStr = ""
				for (var key in this.options) {
					if (key != 't') {
						urlStr += '&' + key + '=' + this.options[key]
					}
				}
				urlStr = '?' + urlStr.substring(1)
				uni.reLaunch({
					url: '/page_register/login?t=' + encodeURIComponent(this.t) + '&noFirt=1' + urlStr,
				});
				return
			}
			//#endif
		},
		methods: {
			//#ifdef H5
			getUrlParam(name) {
				let reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
				let r = window.location.search.substr(1).match(reg);
				// console.log(r, 'rrrrr');
				if (r != null) return unescape(r[2]);
				return null;
			},
			NoTokenExe(appid, created) {
				try {
					var ua = window.navigator.userAgent.toLowerCase();
					if (/wxwork/i.test(ua)) {
						//企业微信
						uni.showLoading({
							title: "加载中"
						})
						var iptcode = this.getUrlParam('code') || "";
						if (iptcode == "") {
							this.redirectBaseUrl(appid);
							uni.hideLoading()
						} else {
							let tStr = this.getUrlParam('t') || "";
							if (tStr) {
								this.t = tStr
							}
							LoginByWxCorp({
								code: iptcode,
								appid: appid,
								created: created
							}).then(async res => {
								// console.log("企业微信登录", data);
								// this.setMsgTop(JSON.stringify(data))
								setToken(res.data.token)
								setRefreshToken(res.data.refresh_token)
								await this.$store.dispatch('GetInfo')
								try {
									this.$nextTick(async () => {
										this.$store.commit('orgLis/SET_ORG_LIST', null)
										let list = await this.$store.dispatch("orgLis/setOrgList");
										setTimeout(() => {
											if (this.t) {
												if (this.t.indexOf('crm/crm') > -1 || this
													.t.indexOf('devices/devices') > -1 ||
													this.t.indexOf('index/index') > -1 || this.t.indexOf('rules/rules') > -1 ||
													this.t.indexOf('profile/profile') > -1||this.t.indexOf('index/index2') > -1) {
														if(this.$store.state.homejumpurl&&this.t.indexOf('index/index') > -1){
															uni.switchTab({
																url: '/pages/index/index2'
															})
														}else{
															uni.switchTab({
																url: '/' + this.t
															})
														}
													
												} else {
													let urlStr = ""
													for (var key in this.options) {
														if (key != 't') {
															urlStr += '&' + key + '=' +
																this.options[key]
														}
													}
													urlStr = '?' + urlStr.substring(1)
													if (this.t.indexOf('index/login') > -
														1 || this.t.indexOf(
															'page_register/msg_tips') > -
														1) {
														uni.switchTab({
															url: this.$store.state.homejumpurl?this.$store.state.homejumpurl:'/pages/index/index'
														})
													} else {
														uni.redirectTo({
															url: '/' + this.t +
																urlStr
														})
													}

												}
											} else {
												// uni.switchTab({
												// 	url: '/pages/home/home'
												// })
												uni.switchTab({
													url: this.$store.state.homejumpurl?this.$store.state.homejumpurl:'/pages/index/index'
												})
											}
											return
										}, 500)
									})
									uni.hideLoading()
								} catch (error) {
									console.log(error, "eee");
									uni.hideLoading()
									this.setMsgTop(error)
								}
							}).catch(err => {
								uni.hideLoading()
								// this.setMsgTop(err)
								uni.showToast({
									title: '企业微信自动登录失败',
									icon: 'none'
								})
								let urlStr = ""
								for (var key in this.options) {
									if (key != 't') {
										urlStr += '&' + key + '=' + this.options[key]
									}
								}
								urlStr = '?' + urlStr.substring(1)
								uni.reLaunch({
									url: '/page_register/login?t=' + encodeURIComponent(this.t) +
										'&noFirt=1' + urlStr,
								});
							})
						}
					} else {}
				} catch (e) {
					//TODO handle the exception
					console.log(e);
					uni.hideLoading()
				}
			},
			redirectBaseUrl(appId) {
				// var cururl = location.href.split('#')[0];
				var cururl = location.href
				wxBaseUrl({
					"url": cururl,
					'appId': appId
				}).then(result => {
					location.href = result.data;
				}).catch(err => {
					uni.hideLoading()
					this.setMsgTop(err)
				})

			},
			//#endif
		}
	}
</script>

<style>

</style>