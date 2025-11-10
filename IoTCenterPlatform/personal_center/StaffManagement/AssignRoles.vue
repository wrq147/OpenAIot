<template>
	<view class="AssignRoles">
		<top leftIcon="icon-fanhui" :isleftBack="true" leftText="Back" backgroundColor="#161A26" title="Assigning roles"
			class="CRM-header"></top>
		<view class="AssignRoles-top">
			<!-- <view v-if="!userInfo.Avatar" class="AssignRoles-top-img">
				{{userInfo.RealName.length>2?userInfo.RealName.slice(1,3):userInfo.RealName}}
			</view> -->
			<view  class="AssignRoles-top-parse">
				<img :src="userInfo.Avatar" alt="图片" style="width: 100%;height:100%;border-radius:50%;">
			</view>

			<view class="AssignRoles-top-content">
				<view class="title">
					{{userInfo.RealName}}
				</view>
				<view class="content">
					{{userInfo.dept_name}}
				</view>
			</view>
		</view>
		<view class="Department-tree">
			<checkbox-group @change="checkboxChange">
				<view class="Department-tree-item borderMargin" style="padding-left:20rpx;"
					v-for="(item,index1) in listLoding" :key="index1">

					<view style="display: flex;">
						<!-- <image v-if="OrderArr.indexOf(xuanId)>1" @click.stop="handStop(item)" class="select" src="../../static/images/select.png"></image>
						<image  v-else @click.stop="handStop(item)" class="select" src="../../static/images/weixuan.png"></image> -->
						<view v-if="!item.disabled">
							<checkbox style="transform:scale(0.75)" :value="item.roleId" :checked="item.selected"
								:disabled="item.disabled" />
						</view>
						<view v-else class="iconfont icon-jinyong"
							style="color:rgba(255,255,255,.4);display: flex;align-items: center;width:40rpx;height:40rpx;margin:10rpx;margin-right:20rpx;">


						</view>
					</view>

					<view class="Department-tree-item-left">
						<view :class="[item.status==1?'addDelete':'']" class="Department-tree-item-left-xitong">
							{{item.roleName}}
						</view>
						<view class="SystemTags" v-if="item.IsSystem==1">
							Syetem
						</view>
						<view v-else class="SystemTags activeQiye">
							Enterprise
						</view>
					</view>

				</view>
			</checkbox-group>

		</view>
		<view class="preserve" @click="handSave">
			Save
		</view>
		<msg-prompt ref="promptMsg"></msg-prompt>
	</view>
</template>

<script>
	import {
		AssignRolesList, //角色列表
		AssignRolesSave, //提交
		RoleList
	} from "@/api/personalCenter";
	import {
		setPagesParam
	} from '@/common/utillib.js'
	export default {
		data() {
			return {
				checkbox1: [0],
				hobby: [],
				listLoding: [],
				rolesList: [],
				xuanId: '',
				IsSystem: '',
				RolesName: '',
				userInfo: [],
				OrderArr: [],
				value: []
			}
		},
		onLoad(Option) {
			// if (Option.arrList) {
			// 	this.rolesList = JSON.parse(Option.arrList)
			// 	this.xuanId = this.rolesList.roleId
			// 	this.list(this.rolesList.Id)
			// 	//console.log(this.rolesList.Id)
			// }
			if (Option.editId) {
				this.xuanId = Option.editId
				//console.log(this.roleIds,'this.roleIds')
				this.list(Option.editId)


			}
		},
		methods: {
			handSave() {
				if (this.value.length <= 0) {
					uni.showToast({
						title: "请选择角色！",
						icon: 'none'
					})
					return
				}
				AssignRolesSave({
					userId: this.xuanId,
					roleIds: this.value.join(',')
				}).then((row) => {
					if (row.code == 0) {
						uni.showToast({
							title: '分配成功！',
							icon: 'none'
						})
						setTimeout(() => {
							setPagesParam('list')
						}, 1000)
					}
				}).catch((err) => {
					this.setMsgTop(err)
				})
			},
			checkboxChange(e) {
				this.value = e.detail.value
			},

			handStop(ite) {
				// this.xuanId=ite.roleId;
				// this.IsSystem=ite.IsSystem
				// this.RolesName=ite.roleName
				this.OrderArr.push(ite.roleId)


			},
			list(id) {
				AssignRolesList(id).then((res) => {
					if (res.code == 0) {
						this.value = res.data.roleIds
						var array3 = []
						RoleList({
							showAll: true
						}).then((row1) => {
							this.canList = row1.data.List

							res.data.roles.map((ite, inx) => {
							ite.roleId=JSON.stringify(ite.roleId)
								res.data.roleIds.map((row, index) => {
									if (ite.roleId == row) {
										//console.log(row,'row')
										ite.selected = true;
									} else {
										//console.log(ite.roleId,'ite.roleId')
									}
								})
								if (!this.canList.some(x => x.roleId == ite.roleId)) {
									//return false;
										ite.disabled = true
								} else {
										ite.disabled = false
									// return true;
								}
							})
							this.listLoding = res.data.roles;
							//console.log(res, '分配权限')
							this.userInfo = res.data.user;
						})
					}
				})
			},
			handSet() {
				if (this.listLoding.length <= 0) {
					uni.showToast({
						title: '当前没有可用的角色！',
						icon: 'none'
					})
					return
				} else {
					this.xuanId = ''
				}

			},
		}
	}
</script>

<style scoped lang="scss">
	.AssignRoles {
		/deep/.uni-checkbox-input {
			border-radius: 50% !important;
			color: #fff !important;
			border: 1rpx solid rgba(255, 255, 255, .4) !important;
			background: none !important;
		}

		/deep/.uni-checkbox-input-checked {
			background: #FF3535 !important;
			border: 1rpx solid #FF3535 !important;
		}

		.select {
			width: 40rpx;
			height: 40rpx;
			border-radius: 50%;
			margin: 0rpx 20rpx;
		}

		.preserve {
			position: absolute;
			width: 94%;
			margin: 30rpx 3%;
			bottom: 20rpx;
			background: linear-gradient(180deg, #FF3535, #FF613D);
			height: 100rpx;
			line-height: 100rpx;
			text-align: center;
			color: #fff;
			border-radius: 10rpx;
			font-size: 32rpx;
			font-weight: bold;
		}

		.Department-tree {

			.Department-tree-item {
				display: flex;
				align-items: center;
				margin: 30rpx 25rpx;

				.select {
					width: 40rpx;
					height: 40rpx;
					border-radius: 50%;
					margin: 0rpx 20rpx;
				}

				.Department-tree-item-right {
					margin-left: auto;
					display: flex;
					align-items: center;

					.borderXian {
						width: 1rpx;
						height: 40rpx;
						background: rgba(255, 255, 255, .2);
						margin: 0rpx 25rpx;
					}

					.t-icon-a-youjiantoubai {
						width: 25rpx;
						height: 25rpx;
						margin-left: auto;
					}
				}

				.backIndex {
					display: flex;
					margin-left: auto;

					.borderXian {
						width: 1rpx;
						height: 40rpx;
						background: rgba(255, 255, 255, .2);
						margin: 0rpx 25rpx;
					}
				}

				.Department-tree-item-left {
					display: flex;
					align-items: center;
					color: #fff;


					.SystemTags {
						color: #FF3535;
						border: 1rpx solid #FF3535;
						font-size: 20rpx;
						padding: 0rpx 8rpx;
						border-radius: 4rpx;
						line-height: 32rpx;
						margin-left: 20rpx;
					}

					.activeQiye {
						border: 1rpx solid rgba(255, 255, 255, 0.40);
						color: rgba(255, 255, 255, 0.40);
					}

					.Department-tree-item-left-xitong {
						display: flex;
						align-items: center;

					}

					.addDelete {
						text-decoration: line-through;
						color: #999999;
					}

					.t-icon-bumenguanli-gongsitubiao,
					.t-icon-bumenguanli-bumentubiao {
						width: 35rpx;
						height: 35rpx;
						margin-right: 14rpx;
					}
				}

				.icon-bianji {
					width: 25rpx;
					height: 25rpx;
					margin-left: auto;
					color: #999;
				}

				.t-icon-bumenguanli-fanhuishangji {
					width: 25rpx;
					height: 25rpx;
					margin-left: auto;
				}
			}

			.borderMargin {
				margin: 20rpx 25rpx;
				background: #1C2232;
				padding: 34rpx 30rpx;
				border-radius: 10rpx;
			}
		}

		.AssignRoles-top {
			background: #1C2232;
			height: 148rpx;
			margin: 3%;
			border-radius: 10rpx;
			display: flex;
			align-items: center;

			.AssignRoles-top-content {

				.title {
					color: #fff;
					font-size: 30rpx;
					font-weight: bold;
				}

				.content {
					color: rgba(255, 255, 255, .4);
					font-size: 24rpx;
				}
			}

			.AssignRoles-top-parse {
				width: 88rpx;
				height: 88rpx;
				border-radius: 50%;
				text-align: center;
				color: #fff;
				margin: 30rpx;
				border: 1rpx solid #EAEAEA;
			}

			.AssignRoles-top-img {
				width: 88rpx;
				height: 88rpx;
				border-radius: 50%;
				line-height: 88rpx;
				text-align: center;
				color: #fff;
				background: #2371FF;
				margin: 30rpx;
			}
		}
	}
</style>