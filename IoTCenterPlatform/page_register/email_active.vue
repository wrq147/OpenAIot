<template>
	<view>
		<top title="Activate email" leftText="Back" leftWidth="157rpx" leftIcon="icon-fanhui" rightWidth="157rpx"
			:isleftBack="true" backgroundColor="#161A26"></top>
		<uni-popup ref="activePopup" type="center" :mask-click="false" :zIndex="999">
			<view class="notice_con">
				<view class="notice_cont">
					<view class="title">Verify</view>
					<view class="content">
						Please log in to the email
						<text class="red_text"> {{infoRsp?infoRsp.Email:''}} </text>to activate
					</view>
					<button class="submit_button yellow_btn" @click.stop="reSendEmail" v-if="!isDaoji">
						Resend activation email
					</button>
					<button class="jump_button botton_mar" v-if="isDaoji">
						Resend activation email {{daojiNum}}s
					</button>
					<button class="submit_button" @click.stop="confirm">
						Confirm
					</button>
				</view>
			</view>
		</uni-popup>
		<msg-prompt ref="promptMsg"></msg-prompt>
	</view>
</template>

<script>
	import {
		getUserInfo,
		sendEmailCode
	} from "@/api/login";
	export default {
		data() {
			return {
				t: '',
				isAppReg: false, //是否是app直接注册
				infoRsp: {}, //邮箱注册的信息
				isDaoji: false,
				daojiNum: 60,
				yaoqingId:null,
				code:null
			};
		},
		async onLoad(options) {
			if (options.t) {
				this.t = options.t
			}
			if (options.isAppReg) {
				this.isAppReg = true
			}
			if (options.yaoqingId && options.yaoqingId != 'null' && options.yaoqingId != null) {
				this.yaoqingId = options.yaoqingId
			}
			if(options.code && options.code != 'null' && options.code != null){
				this.code=options.code
			}
			let rsp = await getUserInfo({
				id: 0
			})
			this.infoRsp = rsp.data.user
			// console.log("用户信息",this.infoRsp);
			this.$nextTick(() => {
				this.$refs.activePopup.open()
				this.reSendEmail()
			})
		},
		methods: {
			reSendEmail() {
				//重新发送激活邮件
				this.$refs.promptMsg.loadingOpen('Sending...')
				sendEmailCode({
					email: this.infoRsp.Email
				}).then(res => {
					this.isDaoji = true
					this.$refs.promptMsg.loadingColse()
					this.$refs.promptMsg.open('Successfully sent！', 2000) //提示信息组件
					let timer = setInterval(() => {
						if (this.daojiNum > 0) {
							this.daojiNum--
						} else {
							clearInterval(timer)
							this.isDaoji = false
						}
					}, 1000)
				}).catch(err => {
					this.$refs.promptMsg.loadingColse()
					this.setMsgTop(err)
				})
			},
			async confirm() {
				let rsp = await getUserInfo({
					id: 0
				})
				this.infoRsp = rsp.data.user
				if (!this.infoRsp.EmailActive) {
					// this.openActivePop()
					this.$refs.promptMsg.open(
						'The email has not been activated yet. Please go to activate it before proceeding！', 2000)
				} else {
					// await this.$store.dispatch('GetInfo')
					// console.log("登入用户企业信息",list);
					if (this.yaoqing) {
						uni.reLaunch({
							url: '/page_register/daili_register3?yaoqingId=' + this.yaoqingId
						})
					}else if(this.code){//员工邀请
						uni.navigateTo({
							url:'/page_register/register1?code='+this.code
						})
					} else {
						this.$store.commit('orgLis/SET_ORG_LIST', null)
						let list = await this.$store.dispatch("orgLis/setOrgList");
						if (list && list.length > 0) {
							if (this.t) {
								if (this.t.indexOf('crm/crm') > -1 || this.t.indexOf('devices/devices') > -1 ||
									this.t
									.indexOf('home/home') > -1 || this.t.indexOf('message/message') > -1 ||
									this.t
									.indexOf('profile/profile') > -1) {
									uni.switchTab({
										url: '/' + this.t
									})
								} else {
									let pages = getCurrentPages();
									let route = pages[pages.length - 1].route;
									let options = pages[pages.length - 1].options;
									let urlStr = ""
									for (var key in options) {
										if (key != 't') {
											urlStr += '&' + key + '=' + options[key]
										}
									}
									urlStr = '?' + urlStr.substring(1)
									// console.log("urlStr", urlStr);
									uni.redirectTo({
										url: '/' + this.t + urlStr
									})
								}
							} else {
								// uni.switchTab({
								// 	url: '/pages/home/home'
								// })

								uni.reLaunch({
									url: '/pages/devices/devices'
								})
							}
						} else {
							uni.reLaunch({
								url: '/page_register/choose_addorg?isAppReg=true'
							})
						}
					}


				}
			},
		}
	}
</script>

<style lang="less">

</style>