<template>
	<view>
		<view style="padding-top: 30vh;">
			<uni-load-more iconType="circle" status="loading" />
		</view>
		<yq-avatar ref="imgref" selWidth="160rpx" selHeight="160rpx" @upload="imgUpload"
			avatarStyle="width: 112rpx; height: 112rpx;display:none;" @end="closePage">
		</yq-avatar>
	</view>
</template>

<script>
	import {
		setPagesParam
	} from '@/common/util.js'
	import {
		uploadFile
	} from '@/api/file.js'
	import request from '@/common/request.js'
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
			//上传企业图片
			async imgUpload(rsp) {
				this.canback = false;
				// uni.showLoading({
				// 	title: '加载中...'
				// });
				try {
					// console.log(await uploadFile(rsp.path))
					let imgPath=await uploadFile(rsp.path);
					imgPath=request.config.baseURL+imgPath
					setPagesParam('getImgUrl',imgPath,1,1000);
				} catch (e) {
					console.info(e);
				} finally {
					// uni.hideLoading();
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
