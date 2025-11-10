<template>
	<view id="CreateCustomer">
		<top leftIcon="icon-fanhui" :isleftBack="true" leftText="Back" backgroundColor="#161A26"
			title="Create role" class="addPool-top"></top>
		<uni-forms ref="form" :modelValue="formData" label-position="top" class="addPool-form" label-width="100%"
			:rules="rules">
			<uni-forms-item label="   Role name" name="roleName" required class="addPool-form-item"
				style="color:red;">
				<input placeholder-class="PlaceStyle" type="text" v-model="formData.roleName" class="addPool-easyinput"
					placeholder="Please enter a role name">
			</uni-forms-item>
			<uni-forms-item label="Limits of authority" required name="parentId" class="addPool-form-item" v-if="hideShow!=0">
				<view class="authority">
					<view @click="handRolds"  class="authority-item">
						<!-- <uni-combox   placeholder="Please enter Limits of authority" v-model="selectNum"></uni-combox> -->
						<!-- <input
						disabled
						class="authority-item-input"
						placeholder="Please enter Limits of authority" 
						:isDark="true"
					  :clear="false"
					   v-model=""
					  >
					  </input> -->
					  <view v-if="selectNum" style="font-size:32rpx;">
						  {{selectNum}}
					  </view>
					  <view v-else style="color:rgba(255, 255, 255, .2);font-size:32rpx;">
						  Please enter Limits of authority
					  </view>
					  
					</view>
						<view class="authority-tree" v-if="hideRoles">
							<ly-tree :tree-data="data"
								:props="defaultProps" 
								node-key="id"
								show-checkbox 
								:default-checked-keys="checkedKeys"
								:expand-on-check-node="expandOnCheckNode"
								@check="handleCheck"
								@check-change="handleCheckChange" 
							/>
						</view>
			
				</view>
				
			</uni-forms-item>
			<uni-forms-item label="   Order" name="roleSort" required  class="addPool-form-item" style="color:red;">
				<input type="text" placeholder-class="PlaceStyle" v-model="formData.roleSort" class="addPool-easyinput"
					placeholder="Please enter the orderNum">
			</uni-forms-item>
			
			
			<uni-forms-item label="Status" name="name" class="addPool-form-item" style="color:red;">
				<view class="addPool-form-item-Sex">
					<view :class="[formData.status==item.id?'active':'on']" class="addPool-form-item-Sex-item"
						v-for="(item,index) in sexSelect" @click="handClick(index)">
						{{item.title}}
					</view>
				</view>
			</uni-forms-item>
			<uni-forms-item label="Notes" name="remark" class="addPool-form-item">
				<textarea placeholder-class="PlaceStyle" value="" v-model="formData.remark" placeholder="Please enter the notes"
					class="addPool-textarea" />
			</uni-forms-item>
		</uni-forms>
		
		<!-- <button v-if="editId" :disabled="isSubmit" class="addPool-button" @click="handEdit">
			Save
		</button> -->
		<view v-if="editId">
			<button  v-if="IsSystem==1" :disabled="isSubmit" class="addPool-button" @click="handEdit">
				Save
			</button>
			<view class="buttonGroup" v-else>
				<button  :disabled="isSubmit" class="addPool-button deleteActive" @click="handDelete">
					Delete
				</button>
				<button  :disabled="isSubmit" class="addPool-button deleteOn" @click="handEdit">
					Save
				</button>
			</view>
		</view>
		<button v-else :disabled="isSubmit" class="addPool-button" @click="handSubmitHigh">
			Save
		</button>
		<msg-prompt ref="promptMsg" @confirm="confireCloseDrawer"></msg-prompt>
	</view>
</template>

<script>
	import LyTree from '@/components/ly-tree/ly-tree.vue'
	import {
		RoleAdd,
		RoleDetails,
		RoleEdit,//编辑
		RolePermission,//角色权限
		RolePermissionEdit,//角色编辑
		RoleDelete
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
				isSubmit:false,
			checkedKeys:[],
			hideRoles:false,
			selectNum:'',	
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
							errorMessage: 'Please enter the orderNum'
						}]
					},
					roleName: {
						rules: [{
							required: true,
							errorMessage: 'Please enter the roleName'
						}, ]
					},
				},
				listLoding: [],
				formData: {
					remark:'',
					roleName:'',
					roleSort:'',
					status:0,
					menuIds:[]
				},
				ManagerList: [],
				selected: 0,
				sexSelect: [{
						title: 'Enable',
						id: 0
					},
					{
						title: 'Disable',
						id: 1
					}

				],
				employeeMap:new Map(),
				hideShow:50,
				editId:'',
				IsSystem:'',
				RolesName:''
			}
		},
		onLoad(option) {
			if (option.deptId) {
				this.detailsLoding(option.deptId);
				this.editId=option.deptId
				this.rolesListEdit(option.deptId)
			}else{
				this.rolesList()//权限
			}
			this.hideShow=option.showHide
			//console.log(option.showHide)
		},
		methods: {
			confireCloseDrawer(){
				RoleDelete(this.editId).then((res)=>{
					if(res.code==0){
						uni.showToast({
							title:'删除成功！',
							icon:'none'
						})
						setTimeout(() => {
							setPagesParam('list')
						}, 1000)
					}
					
				})
			},
			handDelete(){
					//console.log('删除')
					this.$refs.promptMsg.noticeOpen(
						"确定删除名称为"+this.RolesName+"的角色?"
					)
			},
			handRolds(){
				this.hideRoles=!this.hideRoles
			},
			rolesListEdit(deptId){
				RolePermissionEdit(deptId).then((res)=>{
					if(res.code==0){
						//console.log(res,'权限编辑')
						this.data=res.data.menus
						this.checkedKeys=res.data.checkedKeys
						if(this.checkedKeys.length>0){
							this.hideRoles=true;
							this.selectNum=this.checkedKeys.length+' selected'
						}
					}
				})
			},
			rolesList(){
				RolePermission().then((res)=>{
					if(res.code==0){
						//console.log(res,'权限')
						this.data=res.data
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
				this.selectNum=obj.checkedNodes.length+' selected';
				this.checkedKeys=obj.checkedKeys
				this.formData.menuIds=obj.checkedKeys
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
				
				console.log('handleCheckChange',obj);
			
			},
			handEdit(){
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
							title: 'Edit successful！',
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
						//console.log(res, '部门详情')
						this.IsSystem=result.IsSystem
						this.RolesName=result.roleName
						this.formData = {
						remark:result.remark,
						roleName:result.roleName,
						roleSort:result.roleSort,
						status:result.status,
						menuIds:result.menuIds,
						roleId:result.roleId,
						createTime:result.createTime,
						updateId:result.updateId,
						createId:result.createId,
						IsSystem:result.IsSystem,
						NoAlloca:result.NoAlloca,
						OrgId:result.OrgId,
						deptIds:result.deptIds
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
					url: '/pages_Inventory/employee_select?type=user'
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
								title: 'New successfully added！',
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

<style lang="less" scoped>
	page {
		/deep/.uni-select__input-text{
			font-size:32rpx;
		}
		/deep/.uni-forms-item__label{
			font-size:32rpx!important;
		}
		/deep/.uni-textarea-textarea{
			font-size:32rpx;
		}
		.buttonGroup{
			display: flex;
			.deleteActive{
				width: 345rpx;
				height:100rpx;
				background: rgba(28, 34, 50, 1)!important;
				color:rgba(255, 255, 255, .4)!important;
			}
			.deleteOn{
				
			}
		}
		.authority-item{
			position: relative;
			z-index:222;
			height: 88rpx;
			line-height: 88rpx;
			border-radius: 10rpx;
			border:1rpx solid rgba(255, 255, 255, .2);
			padding-left:20rpx;
			color:#fff;
			.authority-item-input{
				position: absolute;
				z-index:111;
				width:100px;
				height:88rpx;
				line-height:88rpx;
				margin-left:20rpx;
				color:#fff;
			}
		}
		/deep/.uni-combox__input-plac{
			color:rgba(255, 255, 255, 0.2);
			font-size:32rpx;
		}
	/deep/.uni-select__selector{
		 display: none!important;
		 height:0rpx;
	}
	/deep/.uni-input-input{
		color:#fff!important;
	}
	/deep/.uni-select--mask{
		display: none!important;
	}
	.uni-combox{
		height:66rpx;
		border:1rpx solid rgba(255, 255, 255, .2);
		color:#fff;
	}
	.authority-tree{
		width:100%;
		background: rgba(28, 34, 50, 1);
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
			color: FF3535;
		}

		.on {
			color: rgba(255, 255, 255, .6);
			border: 1rpx solid rgba(255, 255, 255, .2);
		}

	}
</style>