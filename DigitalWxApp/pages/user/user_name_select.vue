<template>
	<view class="page-container">
		<view style="border-bottom: solid 1px #2953FF;">
			<!-- <uni-easyinput class="iptTxt" trim="both" :focus="true" v-model="RealName" maxlength="20" :inputBorder="false"
				placeholder="请输入真实姓名">
			</uni-easyinput> -->
			<uni-easyinput class="iptTxt weui-input" type="nickname" :focus="true" trim="both" maxlength="20"
				v-model="RealName" :inputBorder="false" placeholder="请输入真实姓名">
			</uni-easyinput>
		</view>
		<button type="primary" class="submit-btn" @click="submitClick">保存</button>
	</view>
</template>

<script>
	import {
		editUser,
		getSelfInfo
	} from '@/api/user.js'
	import {
		reloadPrePage
	} from '@/common/util.js'
	export default {
		data() {
			return {
				RealName: ""
			}
		},
		async onLoad(){
			let rsp = await getSelfInfo();
			this.RealName = rsp.data.user.RealName;
		},
		methods: {
			async submitClick() {
				uni.showLoading({
					title: '加载中...'
				});
				try {
					if(this.RealName==''){
						uni.showToast({
							title:"姓名不能为空",
							icon:'error'
						})
						return
					}
					if(this.RealName=='微信用户'){
						uni.showToast({
							title:"姓名错误!",
							icon:'error'
						})
						return
					}
					await editUser({
						RealName: this.RealName,
					});
					reloadPrePage(2,'changeName');
					reloadPrePage();//执行上一页的reloadpage函数
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
		.iptTxt{
			color: #333;
			height: 90rpx;
			display: flex;	
		}
		.uni-easyinput__content{
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
