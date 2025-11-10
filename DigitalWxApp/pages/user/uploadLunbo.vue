<template>
	<view>
		<view style="padding-top: 30vh;">
			<uni-load-more iconType="circle" status="loading" />
		</view>
		<yq-avatar ref="imgref" selWidth="160rpx" selHeight="160rpx" @upload="imgUpload"
			avatarStyle="width: 690rpx; height: 556rpx;display:none;" @end="closePage">
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
				canback: true,
				sort:null,//判断需要上传的是第几张轮播图
				type:null,//判断跳转过来的是那个页面
			};
		},
		onLoad(options) {
			this.$refs.imgref.fChooseImg();
			if(options){
				if(options.type){
					this.type=options.type
				}
				if(options.sort){
					this.sort=options.sort;
				}
			}
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
					let url={}
					if(this.type=='pro'){
						if(this.sort==1){
							url={
								firstProImg:imgPath
							}
						}
						if(this.sort==2){
							url={
								secondProImg:imgPath
							}
						}
						if(this.sort==3){
							url={
								thirdProImg:imgPath
							}
						}
					}
					setPagesParam('getImgUrl',url,1,1000);
					// uni.navigateBack({
					// 	delta: 1
					// })
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
