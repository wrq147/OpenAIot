<template>
	<view>
		<view class="register_con">
			<view class="logo_con">
				<image class="logo" src="/static/images/logo.png" mode=""></image>
			</view>
			<button class="submit_button" @click="hasAccount">
				Join the company
			</button>
			<button class="jump_button" @click="recreate">
				New Enterprise
			</button>
		</view>
		<msg-prompt ref="promptMsg"></msg-prompt>
	</view>
</template>

<script>
	export default {
		data() {
			return {
				statusBarHeight: uni.getSystemInfoSync().statusBarHeight,
				yaoqingId:null,
				code:null,
				isAppReg:false
			}
		},
		onLoad(options) {
			// if(options.yaoqingId){
			// 	this.yaoqingId=options.yaoqingId
			// }
			// if(options.code){
			// 	this.code=options.code
			// }
			if (options.isAppReg) {
				this.isAppReg = true
			}
		},
		computed:{
			appLogo() {
				return this.$store.state.appLogoUrl
			},
			appName() {
				return this.$store.state.appNameText
			},
		},
		methods: {
			recreate(){
				//创建企业
				uni.reLaunch({
					url: '/page_register/daili_register3?isAppReg=true'
				})
			},
			hasAccount(){
				//加入企业
				uni.reLaunch({
					url: '/page_register/link_join?isAppReg=true'
				})
			}
		}
	}
</script>

<style lang="less" scoped>
.first_jump{
	margin-top: 0;
}
</style>
