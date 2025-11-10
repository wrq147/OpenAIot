<template>
	<view class="page-container">
		<view style="border-bottom: solid 1px #2953FF;">
			<uni-easyinput class="iptTxt" trim="both" :focus="true" v-model="PostName" maxlength="20"
				:inputBorder="false" placeholder="请输入职位信息">
			</uni-easyinput>
		</view>
		<button type="primary" class="submit-btn" @click="submitClick">保存</button>
	</view>
</template>

<script>
	import {
		userInfo,
		editPost
	} from '@/api/org.js'
	import {
		reloadPrePage
	} from '@/common/util.js'
	export default {
		data() {
			return {
				PostName: "",
				Id: 0
			}
		},
		async onLoad(options) {
			this.Id = parseInt(options.id);
			let rsp = await userInfo(this.Id);
			this.PostName = rsp.data.post_name;
		},
		methods: {
			async submitClick() {
				uni.showLoading({
					title: '加载中...'
				});
				try {
					await editPost(this.Id, this.PostName);
					this.$store.commit('SET_USER_INFO', null);
					reloadPrePage();
					uni.navigateBack();
				} catch (err) {
					console.info('异常：', err);
				} finally {
					uni.hideLoading();
				}
		
			}
		}
	}
</script>

<style lang="scss">
	.page-container {
		padding: 60rpx 30rpx 0 30rpx;

		.iptTxt {
			color: #333;
			height: 90rpx;
			display: flex;
		}

		.uni-easyinput__content {
			background-color: #F6F6F6 !important;
		}

		.submit-btn {
			font-size: 30rpx;
			margin-top: 120rpx;
			background: linear-gradient(270deg, #2CC2FB 0%, #50A6FA 100%);
			color: #fff;
			border-radius: 20px;
			height: 90rpx;
			display: flex;
			align-items: center;
			justify-content: center;
		}
	}
</style>
