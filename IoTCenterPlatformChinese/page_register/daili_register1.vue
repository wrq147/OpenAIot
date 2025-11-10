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
		<view class="register_title_text" style="margin-top: 40rpx;">加入邀请</view>
		<button class="submit_button" @click="hasAccount">
			<view class="text" style="margin-right: 10rpx;">
				已有账号
			</view>
			<custom-icons iconsName="icon-jinru" iconsSize="18rpx" iconsColor="#FFFFFF"></custom-icons>
		</button>
		<button class="jump_button" @click="recreate">
			<view class="text">
				新建账号
			</view>
			<custom-icons iconsName="icon-jinru" iconsSize="18rpx" iconsColor="#2371FF"></custom-icons>
		</button>
	</view>
</template>

<script>
	import {agentInviteLogin,agentInviteInfo} from "@/api/login";
	import {
		setToken,
		setRefreshToken,
	} from '@/common/auth.js'
	export default {
		data() {
			return {
				statusBarHeight: uni.getSystemInfoSync().statusBarHeight,
				yaoqingId:null,
				code:null,
				hasInviteInfo:false,
			}
		},
		onLoad(options) {
			if(options.yaoqingId){
				this.yaoqingId=options.yaoqingId
				this.loadYaoqingInfo(this.yaoqingId)
			}
			if(options.yqcode&&options.node){
				this.yaoqingId=options.yqcode
				this.node=options.node
				this.handleLoginCheck()
				this.loadYaoqingInfo(this.yaoqingId)
			}
			if(options.code){
				this.code=options.code
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
			loadYaoqingInfo(id) {
				agentInviteInfo({
					id: id
				}).then(res => {
					console.log("邀请信息", res);
					let data = res.data
					if (data) {
						if(data.ContactName&&data.Tel){
							this.hasInviteInfo=true
						}
					}
			
				})
			},
			handleLoginCheck(){
			  agentInviteLogin({mobileCode:this.node,yqCode:this.yaoqingId}).then(async res=>{
				// console.log("res邀请登录",res);
				setToken(res.data.token)
				setRefreshToken(res.data.refresh_token)
				this.$store.commit('orgLis/SET_ORG_LIST', null)
				let list = await this.$store.dispatch("orgLis/setOrgList");
				uni.navigateTo({
					url:'/page_register/daili_register2?yaoqingId='+this.yaoqingId
				})
			  })
			},
			recreate(){
				//创建新用户
				if(this.code&&!this.yaoqingId){
					uni.navigateTo({
						url:'/page_register/daili_register4?code='+this.code
					})
				}else if(!this.code&&this.yaoqingId){
					if(this.hasInviteInfo){
						uni.navigateTo({
							url:'/page_register/daili_duanxin?yaoqingId='+this.yaoqingId
						})
					}else{
						uni.navigateTo({
							url:'/page_register/daili_register4?yaoqingId='+this.yaoqingId
						})
					}
					
				}
				
			},
			hasAccount(){
				//有账号
				if(this.code&&!this.yaoqingId){
					uni.navigateTo({
						url:'/page_register/register1?code='+this.code
					})
				}else if(!this.code&&this.yaoqingId){
					uni.navigateTo({
						url:'/page_register/daili_register2?yaoqingId='+this.yaoqingId
					})
				}
			}
		}
	}
</script>

<style lang="less" scoped>
.first_jump{
	margin-top: 0;
}
</style>
