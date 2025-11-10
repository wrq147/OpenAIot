<template>
	<view class="register_con" style="min-height: 100vh;background-color: #fff;">
		<top title=" " leftWidth="157rpx" leftIcon="none" :isleftBack="false" backgroundColor="#fff"></top>
		<!-- <view class="logo_con onlylogo">
			<image class="logo" :src="appLogo" mode="heightFix"></image>
			<view class="logo_text">
				{{appName}}
			</view>
		</view> -->
		<!-- <view class="logo_bg" :style="{'height':Number(statusBarHeight*2)+512+'rpx'}"></view> -->
		<view class="register_title_text" style="margin-top: 40rpx;">账号注册</view>
		<button class="submit_button" @click="hasAccount" style="margin-top: 120rpx;">
			<view class="text" style="margin-right: 10rpx;">
				加入企业
			</view>
			<custom-icons iconsName="icon-jinru" iconsSize="18rpx" iconsColor="#FFFFFF"></custom-icons>
		</button>
		<button class="jump_button" @click="recreate">
			<view class="text">
				新建企业
			</view>
			<custom-icons iconsName="icon-jinru" iconsSize="18rpx" iconsColor="#2371FF"></custom-icons>
		</button>
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
