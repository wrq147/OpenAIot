<template>
	<view class="page-container">
		<view style="border-bottom: solid 1px #50A6FA;">
			<uni-easyinput class="iptTxt" trim="both" :focus="true" v-model="deptName" maxlength="50"
				:inputBorder="false" placeholder="请输入部门名称">
			</uni-easyinput>
		</view>
		<button type="primary" class="submit-btn" @click="submitClick" :disabled="isChange">保存</button>
	</view>
</template>

<script>
	import {
		editDept,
		addDept,
		getDeptInfo
	} from '@/api/dept.js'
	import {
		reloadPrePage
	} from '@/common/util.js'
	export default {
		data() {
			return {
				deptId: 0,
				parentId: 0,
				topDept: null,
				deptName: "",
				isChange: false
			};
		},
		onLoad(options) {
			this.deptId = parseInt(options.id || "0");
			this.topDept = this.$store.state.submitMod.formArrary.pop();
			if (this.deptId) {
				this.readLoadDept()
				uni.setNavigationBarTitle({
					title: '编辑部门'
				});
			} else {
				uni.setNavigationBarTitle({
					title: '添加部门'
				});
			}
		},
		methods: {
			async readLoadDept() {
				//获取部门信息
				let rsp = await getDeptInfo(this.deptId)
				console.log("部门信息", rsp);
				this.deptName = rsp.data.deptName
				this.parentId = rsp.data.parentId
			},
			async submitClick() {
				uni.showLoading({
					title: '加载中...'
				});
				try {
					let rsp = null
					if (this.deptId == 0) {
						rsp = await addDept({
							parentId: this.topDept.id,
							orderNum: 0,
							deptName: this.deptName,
							phone: "",
							email: ""
						})
					} else {
						rsp = await editDept({
							deptId: this.deptId,
							parentId: this.parentId,
							deptName: this.deptName
						});
					}
					if (rsp.code == 0) {
						this.isChange = true
						uni.hideLoading();
						uni.showToast({
							icon: 'success',
							title: '保存成功',
							duration: 1500
						});
						setTimeout(() => {
							reloadPrePage();
							uni.navigateBack();
						}, 1500);
					}

				} catch (err) {
					console.info('异常：', err);
					uni.hideLoading();

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
		padding: 60rpx 30rpx 0 30rpx;

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
		}
	}
</style>
