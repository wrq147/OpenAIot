<template>
	<view>
		<view class="yq_title">请输入邀请码</view>
		<view style="padding: 30rpx;">
			<password-input :numLng='code' :psdIptNum="4"></password-input>
		</view>
		<number-keyboard tabBar ref='KeyboarHid' @input='KeyInput' psdLength='4'></number-keyboard>
	</view>
</template>

<script>
	import {
		joinOrg
	} from "@/api/org.js"
	import {
		reloadPrePage
	} from '@/common/util.js'
	export default {
		data() {
			return {
				code: ""
			};
		},
		onLoad() {
			this.$refs.KeyboarHid.open();
		},
		methods: {
			KeyInput(val) {
				this.code = val;
				if (this.code.length == 4) {
					this.finishedOne();
				}
			},
			async finishedOne() {
				uni.showLoading({
					title: '加载中...'
				});

				try {
					let rsp = await joinOrg({
						"code": this.code
					});
					this.$store.commit('SET_USER_INFO', null);
					uni.showToast({
						icon: 'success',
						title: '加入成功',
						duration: 1500
					});
					reloadPrePage(1, "orgJoin");
					setTimeout(() => {
						uni.navigateBack();
					}, 1500);
				} catch {
					console.info('异常：', err);
				} finally {
					uni.hideLoading();
				}

			}
		}
	}
</script>

<style lang="scss">
page{
	background-color: #fff;
}
.yq_title{
	display: flex;align-items: center;justify-content: center;height: 80rpx;font-size: 32rpx;
}
</style>
