<template>
	<view class="register_con">
		<view class="logo_con">
			<image class="logo" src="/static/images/logo.png" mode=""></image>
		</view>
		<button class="jump_button first_jump" @click="hasAccount">
			<view class="text">
				Already have an account
			</view>
			<custom-icons iconsName="icon-jinru" iconsSize="18rpx"></custom-icons>
		</button>
		<button class="jump_button" @click="recreate">
			<view class="text">
				New Account
			</view>
			<custom-icons iconsName="icon-jinru" iconsSize="18rpx"></custom-icons>
		</button>
	</view>
</template>

<script>
	export default {
		data() {
			return {
				statusBarHeight: uni.getSystemInfoSync().statusBarHeight,
				yaoqingId: null,
				code: null
			}
		},
		onLoad(options) {
			if (options.yaoqingId) {
				this.yaoqingId = options.yaoqingId
			}
			if (options.code) {
				this.code = options.code
			}
		},
		computed: {
			appLogo() {
				return this.$store.state.appLogoUrl
			},
			appName() {
				return this.$store.state.appNameText
			},
		},
		methods: {
			recreate() {
				//创建新用户
				if (this.code && !this.yaoqingId) {
					uni.navigateTo({
						url: '/page_register/daili_register4?code=' + this.code
					})
				} else if (!this.code && this.yaoqingId) {
					uni.navigateTo({
						url: '/page_register/daili_register4?yaoqingId=' + this.yaoqingId
					})
				}

			},
			hasAccount() {
				//有账号
				if (this.code && !this.yaoqingId) {
					uni.navigateTo({
						url: '/page_register/register1?code=' + this.code
					})
				} else if (!this.code && this.yaoqingId) {
					uni.navigateTo({
						url: '/page_register/daili_register2?yaoqingId=' + this.yaoqingId
					})
				}
			}
		}
	}
</script>

<style lang="less" scoped>
	.first_jump {
		margin-top: 0;
	}
</style>