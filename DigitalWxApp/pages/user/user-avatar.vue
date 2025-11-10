<template>
	<view>
		<view style="padding-top: 30vh;">
			<uni-load-more iconType="circle" status="loading" />
		</view>
		<yq-avatar ref="imgref" selWidth="160rpx" selHeight="160rpx" @upload="avatarUpload"
			avatarStyle="width: 112rpx; height: 112rpx;display:none;" @end="closePage">
		</yq-avatar>
	</view>
</template>

<script>
	import {
		reloadPrePage
	} from '@/common/util.js'
	import {
		uploadAvatar
	} from '@/api/file.js'
	import {
		editUser
	} from '@/api/user.js'
	
	export default {
		data() {
			return {
				canback: true
			};
		},
		onLoad(options) {
			this.$refs.imgref.fChooseImg();
		},
		methods: {
			//上传头像
			async avatarUpload(rsp) {
				this.canback = false;
				uni.showLoading({
					title: '加载中...'
				});
				try {
					let respath=await uploadAvatar(rsp.path);
					// console.log("头像",respath);
					let res=await editUser({Avatar:respath})
					this.$store.commit('SET_USER_INFO', null);
					// console.log("提交结果",res);
					if(res.data==1){
						let pages = getCurrentPages();
						reloadPrePage(1, "avator");
						reloadPrePage(2, "useravator");
						uni.navigateBack();
					}
					
				} catch (e) {
					console.info(e);
				} finally {
					uni.hideLoading();
				}
			},
			closePage() {
				if (this.canback) {
					uni.navigateBack();
				}
		
			}
		}
	}
</script>

<style lang="scss">

</style>
