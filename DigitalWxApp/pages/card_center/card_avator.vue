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
		reloadPrePage,
		setPagesParam
	} from '@/common/util.js'
	import {
		editCard
	} from '@/api/userCard.js'
	import {
		uploadFile
	} from '@/api/file.js'
	export default {
		data() {
			return {
				cardId: 0,
				canback: true,
				isfirst:0
			};
		},
		onLoad(options) {
			this.cardId = options.id||0;
			this.isfirst=options.isFirst||0
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
					let res = await uploadFile(rsp.path);
					if(this.isfirst){
						setPagesParam('setAvatar',res,1,1000)
					}else{
						await editCard({
							Id: this.cardId,
							Avatar: res
						});
						reloadPrePage(1, "avator");
						this.$store.commit('SET_ISNEWCARD_DATA', true);//名片样式改变，设置名片重新刷新
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
