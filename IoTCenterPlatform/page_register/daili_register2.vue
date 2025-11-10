<template>
	<view>
		<top title=" " leftText="Back" leftWidth="157rpx" leftIcon="icon-fanhui" :isleftBack="true"
			backgroundColor="#161A26"></top>
		<view class="register_con">
			<view class="title_text">Welcome!</view>
			<view class="text_tips">Choose a company</view>
			<view class="input_li noborder">
				<uni-data-select v-model="chooseCompany" :localdata="localdata" width="100%"
					placeholder="Please select a company" borderColor="rgba(255, 255, 255, 0.20)"
					palColor="rgba(255, 255, 255, 0.2)" :isCustom="true" :isDark="true"></uni-data-select>
			</view>
			<button class="submit_button" @click="nextClick" :disabled="isLoading" :style="{'opacity':isLoading?0.6:1}"
				:loading="isLoading">
				Accept the invitation
			</button>
			<button class="jump_button" @click="jump">
				Create new company
			</button>
		</view>
		<msg-prompt ref="promptMsg"></msg-prompt>
	</view>
</template>

<script>
	import {
		joinInvite
	} from '@/api/user.js'
	import {
		getToken,
		getRefreshToken,
	} from '@/common/auth.js'
	export default {
		data() {
			return {
				localdata: [],
				isLoading: false,
				chooseCompany: '', //选择的企业
				readAgreement: false,
				styles: {
					color: '#fff',
					backgroundColor: '#161A26',
					disableColor: 'rgba(28, 34, 50, 1)',
					borderColor: 'rgba(255, 255, 255, 0.20)'
				},
				yaoqingId: null,
				code: null
			};
		},
		computed: {
			appName() {
				return this.$store.state.appNameText
			},
		},
		onLoad(options) {
			let pages = getCurrentPages();
			if (options.yaoqingId) {
				this.yaoqingId = options.yaoqingId
			}
			if (options.code) {
				this.code = options.code
			}
			if (getToken() && getRefreshToken()) {
				this.reqOrgLis()
			} else {
				this.$nextTick(() => {
					let route = pages[pages.length - 1].route;
					let urlStr = ""
					for (var key in options) {
						if (key != 't') {
							urlStr += '&' + key + '=' + options[key]
						}
					}
					uni.navigateTo({
						url: '/pages/index/login?t=' + encodeURIComponent(route) + urlStr
					})
				})
			}

		},
		methods: {
			async reqOrgLis() {
				try {
					uni.showLoading({
						title: 'Loading'
					})
					this.$store.commit('orgLis/SET_ORG_LIST', null)
					let list = await this.$store.dispatch("orgLis/setOrgList");
					this.localdata = []
					list.map(row => {
						let obj = {
							text: row.OrgName,
							value: row.Id
						}
						this.localdata.push(obj)
					})
					uni.hideLoading()
				} catch (e) {
					//TODO handle the exception
					// console.log("e", e);
					uni.hideLoading()
					this.setMsgTop(e)
				}

			},
			nextClick() {
				//选中企业接受邀请
				if (this.chooseCompany && this.chooseCompany != "") {
					joinInvite({
						id: Number(this.yaoqingId),
						orgId: this.chooseCompany
					}).then(async rsp => {
						if (rsp.code == 0) {
							this.$refs.promptMsg.succossOpen()
							// his.$refs.promptMsg.open('Successfully joined, please log in to the app', 2000)
							await this.$store.dispatch('GetInfo')
							setTimeout(() => {
								uni.reLaunch({
									url: '/pages/home/home'
								})
							}, 2000)
						}
					}).catch(err => {
						this.setMsgTop(err)
					});
				} else {
					this.$refs.promptMsg.open('Please select the company you want to accept the invitation from', 2000)
				}
			},
			jump() {
				//跳转去新建企业
				uni.navigateTo({
					url: '/page_register/daili_register3?yaoqingId=' + this.yaoqingId
				})
			}
		}
	}
</script>

<style lang="less" scoped>
	.submit_button {
		margin-top: 92rpx;
	}
</style>