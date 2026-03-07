<template>
	<view id="CreateCustomer">
		<top leftIcon="icon-fanhui" :isleftBack="true"  backgroundColor="#ffffff" title="角色"
			class="addPool-top"></top>
		<uni-forms ref="form" :modelValue="formData" label-position="top" class="addPool-form" label-width="100%"
			:rules="rules">
			<uni-forms-item label="    角色名称" name="roleName" required class="addPool-form-item" style="color:red;">
				<input placeholder-class="PlaceStyle" type="text" v-model="formData.roleName" class="addPool-easyinput"
					placeholder="请输入角色名称">
			</uni-forms-item>
			<uni-forms-item label="菜单权限" required name="parentId" class="addPool-form-item" v-if="hideShow!=0">
				<view class="authority">
					<view @click="handRolds" class="authority-item">
						<view v-if="selectNum">
							{{selectNum}}
						</view>
						<view v-else style="color:rgba(193, 193, 193, 1);font-size:32rpx;">
							请分配权限
						</view>

					</view>
					<view class="authority-tree" v-if="hideRoles">
						<ly-tree :tree-data="data" :props="defaultProps" node-key="id" show-checkbox
							:default-checked-keys="checkedKeys" :expand-on-check-node="expandOnCheckNode"
							@check="handleCheck" @check-change="handleCheckChange" />
					</view>

				</view>

			</uni-forms-item>
			<uni-forms-item label="    角色顺序" name="roleSort" required class="addPool-form-item">
				<input type="text" placeholder-class="PlaceStyle" v-model="formData.roleSort" class="addPool-easyinput"
					placeholder="请选择角色顺序">
			</uni-forms-item>

			<uni-forms-item label="备注" name="remark" class="addPool-form-item">
				<textarea placeholder-class="PlaceStyle" value="" v-model="formData.remark" placeholder="请输入备注"
					class="addPool-textarea" />
			</uni-forms-item>

			<view style="display: flex;justify-content: space-between;">
				<view style="color:rgba(153, 153, 153, 1);">
					是否启用
				</view>
				<switch :checked="checkDisHide" color="rgba(71, 226, 241, 1)" style="transform:scale(0.7)"
					@change="switch1Change" />
			</view>
		</uni-forms>
		<view v-if="editId">
			<button v-if="IsSystem==1" :disabled="isSubmit" class="addPool-button" @click="handEdit">
				保存
			</button>
			<view class="buttonGroup" v-else>
				<button :disabled="isSubmit" class="addPool-button deleteActive" @click="handDelete">
					删除
				</button>
				<button :disabled="isSubmit" class="addPool-button deleteOn" @click="handEdit">
					保存
				</button>
			</view>
		</view>
		<button v-else :disabled="isSubmit" class="addPool-button" @click="handSubmitHigh">
			保存
		</button>
		<msg-prompt ref="promptMsg" @confirm="confireCloseDrawer"></msg-prompt>
	</view>
</template>

<script>
	import LyTree from '@/components/ly-tree/ly-tree.vue'
	import {
		RoleAdd,
		RoleDetails,
		RoleEdit, //编辑
		RolePermission, //角色权限
		RolePermissionEdit, //角色编辑
		RoleDelete //角色删除
	} from "@/api/personalCenter";
	import {
		setPagesParam
	} from '@/common/utillib.js'
	export default {
		components: {
			LyTree
		},
		data() {
			return {
				isSubmit: false,
				checkedKeys: [],
				hideRoles: false,
				selectNum: '',
				expandOnCheckNode: false, // 是否展开选中的节点
				data: [{
					id: 1,
					label: '一级 1',
					children: [{
						id: 4,
						label: '二级 1-1',
						children: [{
							id: 9,
							label: '三级 1-1-1'
						}, {
							id: 10,
							label: '三级 1-1-2'
						}]
					}]
				}, {
					id: 2,
					label: '一级 2',
					children: [{
						id: 5,
						label: '二级 2-1'
					}, {
						id: 6,
						label: '二级 2-2'
					}]
				}, {
					id: 3,
					label: '一级 3',
					children: [{
						id: 7,
						label: '二级 3-1'
					}, {
						id: 8,
						label: '二级 3-2'
					}]
				}],
				defaultProps: {
					children: 'children',
					label: 'label'
				},
				rules: {
					roleSort: {
						rules: [{
							required: true,
							errorMessage: '请输入订单号'
						}]
					},
					roleName: {
						rules: [{
							required: true,
							errorMessage: '请输入角色名称'
						}, ]
					},
				},
				listLoding: [],
				formData: {
					remark: '',
					roleName: '',
					roleSort: '',
					status: 0,
					menuIds: []
				},
				ManagerList: [],
				selected: 0,
				sexSelect: [{
						title: '启用',
						id: 0
					},
					{
						title: '禁用',
						id: 1
					}
				],
				employeeMap: new Map(),
				hideShow: 50,
				editId: '',
				IsSystem: '',
				RolesName: '',
				checkDisHide: false
			}
		},
		onLoad(option) {
			if (option.deptId) {
				this.detailsLoding(option.deptId);
				this.editId = option.deptId
				this.rolesListEdit(option.deptId)
			} else {
				this.rolesList() //权限
			}
			this.hideShow = option.showHide
			//console.log(option.showHide)
		},
		methods: {
			confireCloseDrawer() {
				RoleDelete(this.editId).then((res) => {
					if (res.code == 0) {
						uni.showToast({
							title: '删除成功！',
							icon: 'none'
						})
						setTimeout(() => {
							setPagesParam('list')
						}, 1000)
					}

				})
			},
			handDelete() {
				//console.log('删除')
				this.$refs.promptMsg.noticeOpen(
					"确定删除名称为" + this.RolesName + "的角色?"
				)
			},
			switch1Change: function(e) {
				console.log('switch1 发生 change 事件，携带值为', e.detail.value)
				if (e.detail.value) {
					this.formData.status = 0
				} else {
					this.formData.status = 1
				}
				//console.log(this.formData.status,'this.formData.status')
			},
			handRolds() {
				this.hideRoles = !this.hideRoles
			},
			rolesListEdit(deptId) {
				RolePermissionEdit(deptId).then((res) => {
					if (res.code == 0) {
						//console.log(res,'权限编辑')
						this.data = res.data.menus
						this.checkedKeys = res.data.checkedKeys
						if (this.checkedKeys.length > 0) {
							this.hideRoles = true;
							this.selectNum = this.checkedKeys.length + ' selected'
						}
					}
				})
			},
			rolesList() {
				RolePermission().then((res) => {
					if (res.code == 0) {
						//console.log(res,'权限')
						this.data = res.data
					}
				})
			},
			handleCheck(obj) {
				// obj: {
				// 	checkedKeys: [9, 5], // 当前选中节点的id数组
				// 	checkedNodes: [{...}, {...}], // 当前选中节点数组
				// 	data: {...}, // 当前节点的数据
				// 	halfCheckedKeys: [1, 4, 2], // 半选中节点的id数组
				// 	halfCheckedNodes: [{...}, {...}, {...}], // 半选中节点的数组
				// 	node: Node {...} // 当前节点实例
				// }
				console.log('handleCheck', obj);
				this.selectNum = obj.checkedNodes.length + ' selected';
				this.checkedKeys = obj.checkedKeys
				this.formData.menuIds = obj.checkedKeys
			},

			// 只要节点的选中或半选中状态改变就触发（包括设置默认选中，点击选中/取消选中），其相关的所有父子节点都会触发（也就是说选中一个节点时，触发多次，父子节点的选中状态只要被修改就会触发）
			handleCheckChange(obj) {
				// obj: {
				// 	checked: true, // 节点是否选中
				// 	checkedall: false, // 当前树的所有节点是否全选中
				// 	data: {...}, // 节点数据
				// 	indeterminate: false, // 是否半选中
				// 	node: Node {...} // 节点实例
				// }

				console.log('handleCheckChange', obj);

			},
			handEdit() {
				//编辑
				if (!this.isSubmit) {
					this.isSubmit = true;
					setTimeout(() => {
						this.isSubmit = false;
					}, 2000); // 设置 2 秒后可再次点击
				}
				RoleEdit(this.formData).then((data) => {
					if (data.code == 0) {
						uni.showToast({
							title: '编辑成功！',
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
			detailsLoding(id) {
				RoleDetails(id).then((res) => {
					if (res.code == 0) {
						var result = res.data;
						this.IsSystem = result.IsSystem
						this.RolesName = result.roleName
						//console.log(result,'this.IsSystem')
						//console.log(res, '部门详情')
						if (result.status == 0) {
							this.checkDisHide = true;
						} else {
							this.checkDisHide = false;
						}
						this.formData = {
							remark: result.remark,
							roleName: result.roleName,
							roleSort: result.roleSort,
							status: result.status,
							menuIds: result.menuIds,
							roleId: result.roleId,
							createTime: result.createTime,
							updateId: result.updateId,
							createId: result.createId,
							IsSystem: result.IsSystem,
							NoAlloca: result.NoAlloca,
							OrgId: result.OrgId,
							deptIds: result.deptIds
						}
					}
				})
			},
			selectEmplee(data) {
				//console.log(data,'data')
				this.ManagerList = data;
				this.formData.leader = data[0].id;
			},
			handManager() {
				uni.navigateTo({
					url: '/pages_flow/inventory/employee_select?type=user'
				})
			},
			handSubmitHigh() {
				if (!this.isSubmit) {
					this.isSubmit = true;
					setTimeout(() => {
						this.isSubmit = false;
					}, 2000); // 设置 2 秒后可再次点击
				}
				// 部分表单进行校验，接受一个参数，类型为 String 或 Array ，只校验传入 name 表单域的值
				this.$refs.form.validate().then((res) => {
					// console.log(this.formData,'数据')
					// return
					RoleAdd(this.formData).then((data) => {
						if (data.code == 0) {
							uni.showToast({
								title: '新建成功！',
								icon: 'none'
							})
							setTimeout(() => {
								setPagesParam('list')
							}, 1000)
						}
					}).catch((err) => {
						this.setMsgTop(err)
					})
				})

			},

			handClick(inx) {
				this.formData.status = inx;

			}
		}
	}
</script>
<style>
	page {
		background: #ffffff;
	}
</style>
<style lang="less" scoped>
	.buttonGroup {
		display: flex;

		.deleteActive {
			width: 345rpx;
			height: 100rpx;
			background: rgba(248, 248, 248, 1) !important;
			color: rgba(153, 153, 153, 1) !important;

		}

		.deleteOn {}
	}

	uni-button:after {
		border: none !important;
	}

	.authority-item {
		position: relative;
		z-index: 222;
		height: 88rpx;
		line-height: 88rpx;
		border-radius: 10rpx;
		border: 1rpx solid rgba(255, 255, 255, .2);
		padding-left: 20rpx;
		color: #333;
		background: #F8F8F8;

		.authority-item-input {
			position: absolute;
			z-index: 111;
			width: 100px;
			height: 88rpx;
			line-height: 88rpx;
			margin-left: 20rpx;
			color: #fff;
		}
	}
	.authority-tree {
		width: 100%;
		background: #fff;
	}

	.Collaborator-item-flex {
		color: #fff;
		display: flex;
	}

	.Collaborator-item {
		color: rgba(255, 255, 255, .4);
		height: 90rpx;
		border: 1rpx solid rgba(255, 255, 255, .2);
		border-radius: 8rpx;
		line-height: 90rpx;
		padding: 0rpx 20rpx;
	}


	.addPool-form-item-Sex {
		display: flex;
		justify-content: space-between;

		.addPool-form-item-Sex-item {
			width: 48%;
			border: 1rpx solid #FF3535;
			border-radius: 10rpx;
			text-align: center;
			height: 88rpx;
			line-height: 88rpx;
		}

		.active {
			color: #2371FF;
			border: 1rpx solid #2371FF;
		}

		.on {
			color: #999999;
			border: 1rpx solid #C1C1C1;
		}

	}
</style>