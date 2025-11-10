<template>
	<view>
		<top title=" " leftWidth="157rpx" leftIcon="icon-fanhui" :isleftBack="true"
			backgroundColor="#161A26"></top>
		<view class="register_con">
			<view class="title_text">Welcome!</view>
			<view class="text_tips">Please enter the invitation link</view>
			<view class="input_li">
				<uni-easyinput placeholderStyle="color:rgba(255, 255, 255, 0.2);font-size:32rpx" inputHeight="88rpx"
					:styles="styles" type="textarea" v-model="linkUrl"
					placeholder="Please enter the invitation link"
					contentFontSize="32rpx" primaryColor="rgba(255, 255, 255, 0.5)" :maxlength="-1" :autoHeight="true" :inputBorder="true"/>
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
					color: '#fff',
					backgroundColor: '#161A26',
					disableColor: 'rgba(28, 34, 50, 1)',
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
						this.$refs.promptMsg.open('Please enter the correct invitation link', 1500)
					}
				}else{
					this.$refs.promptMsg.open('Please enter the invitation link', 1500)
				}
			}
		}
	}
</script>

<style lang="less">

</style>