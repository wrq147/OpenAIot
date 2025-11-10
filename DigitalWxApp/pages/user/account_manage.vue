<template>
	<view class="common_wrap">
		<view class="row" @click="jump('phone')">
			<text>绑定电话</text>
			<view class="right">
				<text>{{userInfo.Mobile==null?'':userInfo.Mobile}}</text>
				<uni-icons type="right" color="#333333" size="20"></uni-icons>
			</view>
		</view>
		<view class="row" @click="jump('email')">
			<text>绑定邮箱</text>
			<view class="right">
				<text>{{userInfo.Email}}</text>
				<uni-icons type="right" color="#333333" size="20"></uni-icons>
			</view>
		</view>
		
<!-- 		<view class="row row_gap">
			<text>注销号码</text>
			<view class="right" @click="open">
				<uni-icons type="right" color="#333333" size="20"></uni-icons>
			</view>
		</view> -->

		<uni-popup ref="popup" type="dialog">
			<uni-popup-dialog mode="base" message="成功消息" :duration="2000" :before-close="true" @close="close"
				@confirm="confirm" title="提示" content="是否确定要注销账号"></uni-popup-dialog>
		</uni-popup>
	</view>
</template>

<script>
	import {getSelfInfo} from '@/api/user.js'
	export default {
		data() {
			return {
				userInfo:{},

			}
		},
		onLoad(){
			this.reloadpage();
			
		},
		methods: {
			jump(params){
				let data=this.userInfo
				if(params=='phone'){
					uni.navigateTo({
						url:'/pages/user/change_phone?phone='+data.Mobile
					})
				}
				if(params=='email'){
					uni.navigateTo({
						url:'/pages/user/change_mail?email='+data.Email
					})
				}
			},
			//获取用户信息
			async reloadpage(name){
				if(name=='email'){
					this.$forceUpdate();
				}
				if(name=='phone'){
					this.$forceUpdate();
				}
				let data=await getSelfInfo();//获取用户信息
				// console.log(data)
				this.userInfo=data.data.user
				// console.log(this.userInfo);
			},
			open() {
				this.$refs.popup.open()
			},
			close() {
				this.$refs.popup.close()
			},
			confirm() {
				this.$refs.popup.close()
			}
		}
	}
</script>

<style lang="scss">
.common_wrap{
	padding-top: 20rpx;
}
</style>
