<template>
	<view>
		<view class="common_title">
			管理员
		</view>
		<view class="list">
			<navigator url="" v-for="item in managersList" :key="item.Id" @longtap="delManages(item.Id)">
				<view class="item">
					<view class="left">
						<image class="image" :src="item.Avatar" mode="aspectFill"></image>
						<text>{{item.RealName}}</text>
					</view>
					<view class="right">
						管理员
					</view>
				</view>
			</navigator>
		</view>
		<view class="add">
			<navigator url="" @click="addManages()">
				<uni-icons type="plusempty" color="#50A6FA" size="19"></uni-icons>
				<text>添加管理员</text>
			</navigator>
		</view>
		<view class="tips">
			管理员可对所有员工、部门及权限进行管理
		</view>
	</view>
</template>

<script>
	import {
		getManagersList,
		setManager,
		removeManager
	} from '@/api/managers.js'
	export default {
		data() {
			return {
				managersName: '', //管理员姓名
				identity: '', //身份
				managersList: [], //管理员列表
				orgId: '', //企业id
				chioceUsers: null,

			}
		},
		onLoad(options) {
			// console.log("jjjjjjjj", options)
			this.orgId = options.orgId;
			this.reloadpage();

		},
		methods: {
			delManages(id) {
				//删除管理员
				uni.showModal({ // 弹框询问是否进行下一步事件
					title: '提示',
					content: '是否移除该管理员',
					success: async (res) => {
						if (res.confirm) {
							// console.log('用户点击确定');
							let rs = await removeManager({
								id: id
							})
							// console.log("移除管理员结果返回", res);
							if (rs.code == 0) {
								uni.showToast({
									icon: 'success',
									title: '移除成功',
									duration: 1000
								});
								setTimeout(() => {
									this.reloadpage()
								}, 1000)
							}
							
						} else if (res.cancel) {
							console.log('用户点击取消');
							return
						}
					}
				});
			},
			addManages() {
				uni.navigateTo({
					url: 'managers_add?orgId=' + this.orgId +
						'&selectMethod=singleChoice' //singleChoice单选，mulChoice多选
				})
			},

			//选择的用户设置
			async onSelect(params) {
				this.chioceUsers = params;
				let rs = await setManager({
					id: params.Id
				})
				if (rs.code == 0) {
					uni.showToast({
						icon: 'success',
						title: '添加成功',
						duration: 1000
					});
					setTimeout(() => {
						this.reloadpage()
					}, 1000)
				}
				
				// console.log("添加管理员操作", rs);
				// console.log("接收到返回的数据了没", this.chioceUsers);
			},
			async reloadpage() {
				uni.showLoading({
					title: '加载中'
				})
				try {
					let res = await getManagersList(this.orgId);
					this.managersList = res.data;
					// console.log("管理员列表",this.managersList);
				} catch (err) {
					console.info('异常：', err);
				}
				uni.hideLoading();
			}

		}
	}
</script>

<style lang="scss">
	.list {
		width: 100%;
		background-color: #FFFFFF;


		.item {
			height: 120rpx;
			line-height: 120rpx;
			display: flex;
			justify-content: space-between;
			align-items: center;
			padding: 0 30rpx;
			font-size: 30rpx;

			.left {
				display: flex;
				align-items: center;

				.image {
					width: 68rpx;
					height: 68rpx;
					display: block;
					border-radius: 50%;
				}

				text {
					color: #333333;
					margin-left: 17rpx;
				}

			}

			.right {
				color: #999999;
			}

		}
	}

	.add {
		width: 100%;
		height: 120rpx;

		navigator {
			width: 100%;
			height: 120rpx;
			display: flex;
			align-items: center;
			padding: 0 30rpx;
			box-sizing: border-box;
			color: #50A6FA;
			font-size: 30rpx;
			background-color: #FFFFFF;

			text {
				margin-left: 10rpx;

			}
		}


	}


	.tips {
		color: #999999;
		font-size: 24rpx;
		width: 100%;
		box-sizing: border-box;
		padding: 0 30rpx;
		margin-top: 20rpx;
	}
</style>
