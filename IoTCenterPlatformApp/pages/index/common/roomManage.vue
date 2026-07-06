<template>
	<view class="roomManage">
		<top title="车间管理" leftWidth="120rpx" rightWidth="120rpx" :isleftBack="true" leftIcon="icon-fanhui"
			backgroundColor="#ffffff" rightIcon="icon-tianjia" @clickRight="wareRecordsAdd"></top>
		<view class="roomManage-container">
			<view class="container-info" v-for="item in rangeList" :key="item.Id">
				<view class="infoBox">
					<view class="top">
						<view class="title">{{ item.Name }}</view>
						<view class="icon">
							<image v-if="getSerVerUrl()" :src="getSerVerUrl()+'/appimg/treeDel.png'" mode=""
								@click="treeDelete(item)"></image>
							<image v-if="getSerVerUrl()" :src="getSerVerUrl()+'/appimg/treeEdit.png'" mode=""
								style="margin-left: 40rpx" @click="treeEdit(item)"></image>
						</view>
					</view>
					<view class="bottom">
						<view class="people-f">负责人：{{ item.LeaderInfo.RealName }}</view>
						<!-- <view class="people-x">创建人：{{ item.createName }}</view> -->
					</view>
				</view>
			</view>
		</view>
		<uni-popup ref="popup" type="dialog" :zIndex="60">
			<view class="popup-content">
				<view class="popup-container">
					<view class="popup-title">{{ title }}车间</view>
					<uni-forms ref="valiForm" :rules="rules" :modelValue="valiFormData" label-width="90"
						label-position="top">
						<uni-forms-item label="车间名称" required name="name">
							<uni-easyinput v-model="valiFormData.name" placeholder="请输入车间名称" />
						</uni-forms-item>
						<uni-forms-item label="所属分类" required name="categoryId">
							<uni-data-select v-model="valiFormData.categoryId" :clear="false" :localdata="range"
								placeholder="请选择所属分类"></uni-data-select>
						</uni-forms-item>
						<uni-forms-item label="负责人" required name="leaderId">
							<view style="display: flex; align-items: center">
								<uni-tag :inverted="true" :text="valiFormData.leaderName" type="primary" />
								<view style="width: 156rpx; margin-left: 20rpx">
									<button class="peopleSelect" @click="peopleSelect(1)">
										选择负责人
									</button>
								</view>
							</view>
						</uni-forms-item>
						<uni-forms-item label="协作人">
							<view style="display: flex; align-items: center">
								<uni-tag v-for="(item, index) in peopleX" :key="index" :inverted="true"
									:text="item.name" type="primary" style="margin-right: 20rpx" />
								<view style="width: 156rpx">
									<button class="peopleSelect" @click="peopleSelect(2)">
										选择协作人
									</button>
								</view>
							</view>
						</uni-forms-item>
						<uni-forms-item label="关联客户">
							<view style="display: flex">
								<uni-data-checkbox v-model="valiFormData.autoAdd"
									:localdata="relevance"></uni-data-checkbox>
								<span style="font-size: 12px; color: #999">
									（是否自动关联设备到车间）
								</span>
							</view>
						</uni-forms-item>
						<uni-forms-item label="排序">
							<uni-easyinput type="number" v-model="valiFormData.sort" />
						</uni-forms-item>
						<button class="button" @click="submit">保存</button>
					</uni-forms>
				</view>
			</view>
		</uni-popup>
	</view>
</template>

<script>
	import {
		typeListTree,
		roomList,
		roomListAdd,
		roomListEdit,
		roomListDelete,
		roomListInfo,
	} from '@/api/device.js';
	export default {
		data() {
			return {
				rangeList: [],
				title: '添加',
				showPopup: true,
				valiFormData: {
					customerId: '',
					name: '',
					id: '',
					targetOrgId: '',
					leaderId: '',
					leaderName: '',
					categoryId: '',
					helper: '',
					autoAdd: 'false',
					sort: 1,
				},
				// 校验规则
				rules: {
					name: {
						rules: [{
							required: true,
							errorMessage: '车间名称不能为空'
						}],
					},
					categoryId: {
						rules: [{
							required: true,
							errorMessage: '所属分类不能为空'
						}],
					},
					leaderId: {
						rules: [{
							required: true,
							errorMessage: '请选择负责人'
						}],
					},
				},
				relevance: [{
						text: '是',
						value: 'true'
					},
					{
						text: '否',
						value: 'false'
					},
				],
				range: [],
				peopleTage: 1,
				peopleX: [],
			};
		},
		onLoad(options) {
			this.getList();
			this.getTypeList();
		},
		methods: {
			getList() {
				roomList({
					TargetOrgId: this.$store.state.user.orgId
				}).then((res) => {
					this.rangeList = res.data;
				});
			},
			getTypeList() {
				this.range = [];
				typeListTree({
					orgid: this.$store.state.user.orgId
				}).then((res) => {
					res.data.forEach((item) => {
						this.range.push({
							value: item.Id,
							text: item.Name
						});
						item.Children.forEach((v) => {
							this.range.push({
								value: v.Id,
								text: v.Name
							});
						});
					});
				});
			},
			wareRecordsAdd() {
				this.title = '添加';
				this.valiFormData.id = '';
				this.valiFormData.targetOrgId = this.$store.state.user.orgId;
				this.valiFormData.name = '';
				this.valiFormData.leaderId = '';
				this.valiFormData.leaderName = '';
				this.valiFormData.categoryId = '';
				this.valiFormData.helper = '';
				this.valiFormData.sort = 1;
				this.valiFormData.customerId = '';
				this.valiFormData.autoAdd = 'false';
				this.peopleX = [];
				this.$refs.popup.open();
			},
			treeEdit(item) {
				this.peopleX = [];
				roomListInfo({
					id: item.Id
				}).then((res) => {
					this.valiFormData.leaderName = res.data.LeaderInfo.RealName;
					res.data.HelperUsers.forEach((item) => {
						const obj = {
							avatar: item.Avatar,
							id: item.Id,
							name: item.RealName,
							type: 'user',
							selected: true,
						};
						this.peopleX.push(obj);
					});
					this.valiFormData.helper = res.data.HelperUsers.map((row) => row.Id).toString();
				});
				this.title = '编辑';
				this.valiFormData.id = item.Id;
				this.valiFormData.name = item.Name;
				this.valiFormData.leaderId = item.LeaderId;
				this.valiFormData.sort = item.Sort;
				this.valiFormData.targetOrgId = item.TargetOrgId;
				this.valiFormData.categoryId = item.CategoryId;
				this.valiFormData.customerId = item.CustomerId;
				this.$refs.popup.open();
			},
			treeDelete(item) {
				let that = this;
				uni.showModal({
					title: '提示',
					content: '你确定要删除吗',
					success: function(res) {
						if (res.confirm) {
							roomListDelete({
								id: item.Id
							}).then((res) => {
								uni.showToast({
									title: '删除成功',
									icon: 'success',
									duration: 2000,
								});
								that.getList();
							});
						}
					},
				});
			},
			getAdd() {
				roomListAdd(this.valiFormData).then((res) => {
					uni.showToast({
						title: '添加成功',
						icon: 'success',
						duration: 2000,
					});
					this.$refs.popup.close();
					this.getList();
				});
			},
			getUpdata() {
				roomListEdit(this.valiFormData).then((res) => {
					uni.showToast({
						title: '编辑成功',
						icon: 'success',
						duration: 2000,
					});
					this.$refs.popup.close();
					this.getList();
				});
			},
			submit() {
				this.$refs.valiForm.validate((err, valiFormData) => {
					if (!err) {
						if (this.title === '编辑') {
							this.getUpdata();
						} else {
							this.getAdd();
						}
					}
				});
			},
			peopleSelect(type) {
				this.peopleTage = type;
				if (type === 1) {
					uni.navigateTo({
						url: '/pages_flow/inventory/employee_select?type=user',
					});
				} else {
					uni.navigateTo({
						url: '/pages_flow/inventory/employee_select?type=user&multiple=true&selected=' +
							JSON.stringify(this.peopleX),
					});
				}
			},
			selectEmplee(item) {
				if (this.peopleTage === 1) {
					this.valiFormData.leaderName = item[0].name;
					this.valiFormData.leaderId = item[0].id;
				} else {
					this.peopleX = item;
					this.valiFormData.helper = item.map((row) => row.id).toString();
				}
			},
		},
	};
</script>

<style lang="less" scoped>
	.tag-view {
		height: 35rpx;
	}

	.roomManage {
		width: 100%;
		
	}

	.roomManage-container {
		padding: 20rpx;
	}

	.container-info {
		width: 100%;
		background-color: #fff;
		margin-bottom: 20rpx;
		border-radius: 10rpx;
	}

	.infoBox {
		padding: 30rpx;
	}

	.infoBox .top {
		display: flex;
		align-items: center;
		justify-content: space-between;
	}

	.infoBox .top .title {
		font-weight: bold;
		font-size: 30rpx;
		color: #333333;
	}

	.infoBox .top .icon {
		display: flex;
		align-items: center;
	}

	.infoBox .top .icon>image {
		width: 28rpx;
		height: 28rpx;
	}

	.infoBox .bottom {
		margin-top: 10px;
		display: flex;
		font-weight: 400;
		font-size: 24rpx;
		color: #999999;
	}

	.infoBox .bottom .people-x {
		margin-left: 102rpx;
	}

	.popup-content {
		width: 650rpx;
		height: 1260rpx;
		border-radius: 10rpx;
		background-color: #fff;
	}

	.popup-container {
		padding: 20rpx;
	}

	.popup-container .popup-title {
		font-size: 34rpx;
		color: #333333;
		font-weight: bold;
		text-align: center;
		margin-bottom: 40rpx;
	}

	.button {
		width: 100%;
		height: 88rpx;
		background: #2371ff;
		border-radius: 10rpx;
		font-size: 28rpx;
		color: #ffffff;
		line-height: 88rpx;
	}

	.peopleSelect {
		width: 100%;
		height: 60rpx;
		background: #2371ff;
		border-radius: 10rpx;
		font-size: 20rpx;
		color: #ffffff;
		line-height: 60rpx;
	}
</style>