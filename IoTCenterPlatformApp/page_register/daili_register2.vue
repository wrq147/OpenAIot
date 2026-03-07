<template>
	<view style="min-height: 100vh;background-color: #fff;">
		<top title=" " leftWidth="157rpx" leftIcon="icon-fanhui" :isleftBack="true"
			backgroundColor="#ffffff"></top>
		<view class="register_con">
			<!-- <view class="title_text">欢迎来到{{appName}}!</view> -->
			<view class="text_tips" style="margin-top: 154rpx;">请选择公司</view>
			<view class="input_li noborder">
				<uni-data-select v-model="chooseCompany" :localdata="localdata" width="100%"
					placeholder="请选择公司" borderColor="rgba(255, 255, 255, 0.20)"
					palColor="#999999" :isCustom="true" :isDark="false"></uni-data-select>
			</view>
			<button class="submit_button" @click="nextClick" :disabled="isLoading" :style="{'opacity':isLoading?0.6:1}"
				:loading="isLoading">
				接受邀请
			</button>
			<button class="jump_button" @click="jump">
				新建企业
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
				localdata: [{
						text: "测试",
						value: 0
					},
					{
						text: "开发",
						value: 1
					}
				],
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
				code:null
			};
		},
		computed:{
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
			if(getToken()&&getRefreshToken()){
				this.reqOrgLis()
			}else{
				this.$nextTick(()=>{
					let route = pages[pages.length - 1].route;
					let urlStr=""
					for(var key in options){
						if(key !='t'){
							urlStr+='&'+key+'='+options[key]
						}
					}
					uni.redirectTo({
						url:'/page_register/login?t='+encodeURIComponent(route)+urlStr
					})
				})
			}
			
		},
		methods: {
			async reqOrgLis() {
				try {
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
				} catch (e) {
					//TODO handle the exception
					// console.log("e", e);
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
							setTimeout(()=>{
								uni.reLaunch({
									url:this.$store.state.homejumpurl?this.$store.state.homejumpurl:'/pages/index/index'
								})
							},2000)
						}
					}).catch(err=>{
						this.setMsgTop(err)
					});
				} else {
					this.$refs.promptMsg.open('请选择您要接受邀请的公司', 2000)
				}
			},
			jump() {
				//跳转去新建企业
				uni.navigateTo({
					url:'/page_register/daili_register3?yaoqingId='+this.yaoqingId
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