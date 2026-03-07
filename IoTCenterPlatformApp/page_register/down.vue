<template>
	<view class="page_con">
		<top :title="topTitle" leftWidth="157rpx" :isleftBack="true" leftIcon="icon-fanhui"
			backgroundColor="rgba(255, 255, 255, 0)" rightWidth="157rpx">
		</top>
		<!-- <view class="bg_con"></view> -->
		<view class="cont_con" v-if="show">
			<view class="logo_con">
				<image class="logo" :src="downInfo.mobileLogo?downInfo.mobileLogo:''" mode="heightFix"></image>
			</view>
			<view class="name_text">{{downInfo.name}}</view>
			<view class="version_text" v-if="downInfo.UpVersion">版本号：V{{downInfo.UpVersion}}</view>
			<view class="version_text" v-if="errMsg">{{errMsg}}</view>
			<view class="down_btn" @click="downLoad" v-if="downInfo.UpVersion">立即下载</view>
		</view>
	</view>
</template>

<script>
	import {
	    orgStyle,
		upgradeInfo
	} from '@/api/code.js'
	import {
		getToken,
		getRefreshToken
	} from '@/common/auth.js'
	import {
		EnterpriseDetails
	} from "@/api/personalCenter";
	export default {
		data() {
			return {
				topTitle:'下载',
				downInfo:{},
				show:false,
				errMsg:'',
			};
		},
		onLoad() {
			if (getToken() && getRefreshToken()&&this.$store.state.user.uid) {
				console.log("");
				this.loadStyle()
			}else{
				this.show=false
			}
			
		},
		computed:{
			appLogo(){
				console.log("logo",this.downInfo);
				if(this.downInfo&&this.downInfo.mobileLogo){
					return this.downInfo.mobileLogo
				}else{
					// return '/static/images/logo.png'
					return ''
				}
			}
		},
		methods:{
			loadStyle(){
				if(this.$store.state.loginThemeId){
					this.downInfo.mobileLogo=this.$store.state.appLogoUrl
					this.downInfo.name=this.$store.state.appNameText+'客户端'
					this.downInfo.id=this.$store.state.loginThemeId
					this.topTitle='下载'+this.$store.state.appNameText+'客户端'
				}else{
					this.loadOrgInfo(result=>{
						this.downInfo.mobileLogo=result.Logo
						this.downInfo.name=result.OrgName+'客户端'
						this.downInfo.id=''
						this.topTitle='下载'+result.OrgName+'客户端'
						this.$forceUpdate()
					})
					
				}
				upgradeInfo({styleId:this.downInfo.id}).then(rsp=>{
					this.errMsg=''
					this.downInfo.UpVersion=rsp.data.UpVersion//app版本
					this.$forceUpdate()
				}).catch(err=>{
					this.errMsg=err.message
				})
				this.show=true
				
			},
			downLoad(){
				window.location.href = 'http://wxwx.huade-app.com:880/AuthService/Upgrade/Down?styleId=' + this.downInfo.id;
			},
			loadOrgInfo(cb){
				EnterpriseDetails({id:this.$store.state.user.orgId}).then((res)=>{
					if(res.code==0){
						let data=res.data;
						//this.OrgId=data.Creator.OrgId
						if(cb){
							cb(data)
						}
						//this.$set(this.formData,'size',data.Size)
					}
				})
			}
		}
	}
</script>

<style lang="scss" scoped>
	.page_con{
		height: 100vh;
		position: relative;
		background: rgba(255, 255, 255, 1);
		.bg_con{
			position: absolute;
			z-index: 0;
			top: 0;
			width: 100%;
			height: 662rpx;
			background: linear-gradient( 180deg, #DEEBFA 0%, #F5F8F9 100%);
		}
		.cont_con{
			position: relative;
			z-index: 1;
			display: flex;
			flex-direction: column;
			justify-content: center;
			align-items: center;
			.logo_con{
				padding-top: calc(env(safe-area-inset-bottom) + 160rpx);
				padding-bottom: 80rpx;
				.logo{
					width: 160rpx;
					height: 160rpx;
				}
			}
			.name_text{
				font-size: 36rpx;
				color: rgba(51, 51, 51, 1);
				font-weight: bold;
				text-align: center;
				line-height: 36rpx;
			}
			.version_text{
				font-size: 28rpx;
				color: rgba(153, 153, 153, 1);
				line-height: 28rpx;
				text-align: center;
				margin-top: 40rpx;
			}
			.down_btn{
				margin-top: 120rpx;
				width: 590rpx;
				height: 100rpx;
				background: rgba(35, 113, 255, 1);
				color: rgba(255, 255, 255, 1);
				font-size: 32rpx;
				font-weight: bold;
				border-radius: 10rpx;
				text-align: center;
				line-height: 100rpx;
			}
		}
	}
</style>