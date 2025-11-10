<template>
	<view>

		
	</view>
</template>

<script>
	import {
		getToken,
		getRefreshToken,
	} from '@/common/auth.js'
	import {
		deviceIsOnline
	} from '@/api/wifi.js'
	import {
		getUserInfo,
	} from "@/api/login";
	export default {
		data() {
			return {
			}
		},
		onLoad(options) {
			console.log("index页面");
			// if(options.code){
			// 	uni.reLaunch({
			// 		url:'/page_register/register1?code='+options.code
			// 	})
			// }else if(options.yaoqingId){
			// 	uni.reLaunch({
			// 		url:'/page_register/daili_register1?yaoqingId='+options.yaoqingId
			// 	})
			// }else{
				// this.$nextTick(async ()=>{
				// 	if(getToken()&&getRefreshToken()){
				// 		console.log(this.$store.state.user.roles&&this.$store.state.user.roles.length>0);
				// 		if(this.$store.state.user.roles&&this.$store.state.user.roles.length>0){
				// 		}else{
				// 			let rsp = await getUserInfo({
				// 				id: 0
				// 			})
				// 			await this.$store.dispatch('GetInfo')
				// 			this.$store.commit('SET_ROLES',rsp.data.user.roleIds)
				// 		}
				// 		let list = await this.$store.dispatch("orgLis/setOrgList");
				// 		if (list && list.length > 0) {
				// 			setTimeout(() => {
				// 				uni.switchTab({
				// 					url: '/pages/devices/devices'
				// 				})
				// 				uni.hideLoading()
				// 				return
				// 			}, 500)
				// 		} else {
				// 			uni.navigateTo({
				// 				url: '/page_register/daili_register3?isAppReg=true'
				// 			})
				// 		}
				// 	}else{
				// 		uni.reLaunch({
				// 			url:'/pages/index/login'
				// 		})
				// 	}
				// })
				
			// }
			
		},
		methods: {
		}
	}
</script>

<style lang="scss">
	.uni-padding-wrap {
		padding: 30rpx 30rpx;
		margin-top: 200rpx;

		.button-wrap {
			margin-bottom: 30rpx;
		}
	}
</style>