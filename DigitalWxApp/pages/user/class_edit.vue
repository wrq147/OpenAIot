<template>
	<view class="page-container">
		<view style="border-bottom: solid 1px #50A6FA;">
			<uni-easyinput class="iptTxt" trim="both" :focus="true" v-model="CategoryName" maxlength="50"
				:inputBorder="false" placeholder="请输入产品分类">
			</uni-easyinput>
		</view>
		<button type="primary" :class="{'submit-btn':true,'dis':curLen}" @click="submitClick"
			:disabled="curLen">保存</button>
	</view>
</template>

<script>
	import {
		addProClass,
		editProClass
	} from '@/api/product.js'
	import {
		reloadPrePage
	} from '@/common/util.js'
	export default {
		data() {
			return {
				deptId: 0,
				topDept: null,
				CategoryName: "", //分类名称
				curLen: false
			};
		},
		async onLoad(options) {
			// console.log("打印options",options);
			this.deptId = parseInt(options.id || "0");
			this.classParId = parseInt(options.classParId || "");
			this.topDept = this.$store.state.submitMod.formArrary.pop();
			// console.log("添加分类获取store中的父极id", this.topDept);
			if (options.id && options.className) {
				this.CategoryName = JSON.parse(decodeURIComponent(options.className))
			}

		},
		methods: {
			async submitClick() {
				uni.showLoading({
					title: '加载中...'
				});

				try {
					let usrInfo = await this.$store.dispatch("userInfo");

					if (this.deptId == 0) {

						let rsp = await addProClass({
							parentId: this.classParId,
							orgId: usrInfo.orgId,
							CategoryName: this.CategoryName,
							// sort: 0,
						})
						if (rsp.code == 0) {
							this.curLen=true
							uni.showToast({
								icon: 'success',
								title: '保存成功',
								duration: 1500
							});
							setTimeout(() => {
								reloadPrePage(1, 'add');
								uni.navigateBack();
							}, 1500);
						}

					} else {
						let rsp = await editProClass({
							id: this.deptId,
							parentId: this.topDept.parentId==0?'':this.topDept.parentId,
							orgId: usrInfo.orgId,
							CategoryName: this.CategoryName,
							sort: 0,
						});
						if (rsp.code == 0) {
							this.curLen=true
							uni.showToast({
								icon: 'success',
								title: '修改成功',
								duration: 1500
							});
							setTimeout(() => {
								reloadPrePage(1, 'edit');
								reloadPrePage(2, 'edit');
								uni.navigateBack();
							}, 1500);
						}

					}


				} catch (err) {
					uni.hideLoading();
					console.info('异常：', err);
				}
			}
		}
	}
</script>

<style lang="scss">
	page {
		background-color: #fff;
	}

	.page-container {
		padding: 60rpx 30rpx 30px 30rpx;

		.iptTxt {
			color: #333;
			height: 90rpx;
			display: flex;
			align-items: center;
		}

		.submit-btn {
			font-size: 30rpx;
			margin-top: 120rpx;
			background: linear-gradient(270deg, #2CC2FB 0%, #50A6FA 100%);
			border-radius: 20px;
			height: 90rpx;
			display: flex;
			align-items: center;
			justify-content: center;

			&.dis {
				background: #C1E1FF !important;
			}
		}
	}
</style>
