<template>
	<view style="min-height: 100vh;background-color: #ffffff;">
		<top title=" " leftWidth="157rpx" leftIcon="icon-fanhui" :isleftBack="true"
			backgroundColor="#ffffff"></top>
		<view class="register_con">
			<!-- <view class="title_text">欢迎来到{{appName}}!</view> -->
			<view class="text_tips" style="margin-top: 154rpx;">请输入邀请链接</view>
			<view class="input_li">
				<uni-easyinput placeholderStyle="color:#999999;font-size:32rpx" inputHeight="88rpx"
					:styles="styles" type="textarea" v-model="linkUrl"
					placeholder="请输入邀请链接"
					contentFontSize="32rpx" primaryColor="#2371FF" :maxlength="-1" :autoHeight="true"/>
			</view>
			<button class="submit_button" @click="nextClick" :disabled="isLoading" :style="{'opacity':isLoading?0.6:1}"
				:loading="isLoading">
				下一步
			</button>
		</view>
		<msg-prompt ref="promptMsg"></msg-prompt>
	</view>
</template>

<script>
	export default {
		data() {
			return {
				isLoading: false,
				linkUrl: '', //邀请链接
				styles: {
					color: '#333333',
					backgroundColor: '#F8F8F8',
					disableColor: 'rgba(28, 34, 50, 0.5)',
					borderColor: 'rgba(255, 255, 255, 0.20)'
				},
				basicInfo: {},
				isAppReg: true, //是否是app直接注册
			};
		},
		computed:{
			appName() {
				return this.$store.state.appNameText
			},
		},
		onLoad(options) {
			if (options.isAppReg) {
				this.isAppReg = true
			}
		},
		methods: {
			nextClick() {
				//跳转下一步
				if(this.linkUrl){
					if(this.linkUrl.indexOf('yaoqingId=')>-1){
						let list=this.linkUrl.split('yaoqingId=')
						uni.reLaunch({
							url: '/page_register/daili_register3?yaoqingId='+list[1]
						})
					}else if(this.linkUrl.indexOf('code=')>-1){
						let list=this.linkUrl.split('code=')
						uni.navigateTo({
							url: '/page_register/register1?code=' + list[1]
						})
					}else{
						this.$refs.promptMsg.open('请输入正确的邀请链接', 1500)
					}
				}else{
					this.$refs.promptMsg.open('请输入邀请链接', 1500)
				}
			}
		}
	}
</script>

<style lang="less">

</style>