<template>
	<view id="CreateCustomer">
		<top leftIcon="icon-fanhui" :isleftBack="true" leftText="Back" backgroundColor="#161A26"
			title="Create department" class="addPool-top"></top>
		<uni-forms ref="form" :modelValue="formData" label-position="top" class="addPool-form" label-width="100%"
			:rules="rules">
			<uni-forms-item label="   Superior department" required name="parentId" class="addPool-form-item" v-if="hideShow!=0">
				<uni-data-select placeholder-class="PlaceStyle"  type="line" v-model="formData.parentId" :isDark="true" :localdata="listLoding"
					placeholder="Please select a superior department" class="addPool-selected"></uni-data-select>
			</uni-forms-item>
			<uni-forms-item label="  Department Name" name="deptName" required class="addPool-form-item"
				style="color:red;">
				<input placeholder-class="PlaceStyle" type="text" v-model="formData.deptName" class="addPool-easyinput"
					placeholder="Please enter the department name">
			</uni-forms-item>
			<uni-forms-item label="   Order" name="orderNum" required class="addPool-form-item" style="color:red;">
				<input placeholder-class="PlaceStyle" type="text" v-model="formData.orderNum" class="addPool-easyinput"
					placeholder="Please enter the orderNum">
			</uni-forms-item>
			<uni-forms-item label=" Manager" name="leader" class="addPool-form-item">
				<view class="Collaborator-item" @click="handManager">
					<view class="Collaborator-item-flex">
						<view v-if="ManagerList.length>0">
							<view v-for="(item,index) in ManagerList" :key="index" style="margin-right:10rpx;">
								{{item.name}}
							</view>
						</view>
						<text v-else style="color:rgba(255, 255, 255, .2);font-size:32rpx;">
							Please select a collaborator
						</text>
					</view>
				</view>
			</uni-forms-item>
			<uni-forms-item label="Telephone number" name="phone" class="addPool-form-item">
				<input placeholder-class="PlaceStyle" type="text" v-model="formData.phone" class="addPool-easyinput"
					placeholder="Please enter the telephone number">
			</uni-forms-item>
			<uni-forms-item label="E-mail" name="email" class="addPool-form-item">
				<input placeholder-class="PlaceStyle" type="text" v-model="formData.email" class="addPool-easyinput"
					placeholder="Please enter the email E-mail">
			</uni-forms-item>
			<uni-forms-item label="Sex" name="name" class="addPool-form-item" style="color:red;">
				<view class="addPool-form-item-Sex">
					<view :class="[formData.status==item.id?'active':'on']" class="addPool-form-item-Sex-item"
						v-for="(item,index) in sexSelect" @click="handClick(index)">
						{{item.title}}
					</view>
				</view>
			</uni-forms-item>
		</uni-forms>
		<button v-if="editId" :disabled="isSubmit" class="addPool-button" @click="handEdit">
			Save
		</button>
		<button v-else :disabled="isSubmit" class="addPool-button" @click="handSubmitHigh">
			Save
		</button>
		<msg-prompt ref="promptMsg"></msg-prompt>
	</view>
</template>

<script>
	import {
		DeptamentList, //部门列表
		DeptamentAdd, //新增部门
		DeptamentDetails ,//部门详情
		yuanAarngemnt,//员工
		DeptamentEdit//部门编辑
	} from "@/api/personalCenter";
	import {
		setPagesParam
	} from '@/common/utillib.js'
	export default {
		data() {
			return {
				isSubmit:false,
				rules: {
					parentId: {
						rules: [{
							required: true,
							errorMessage: 'Please select a superior department'
						}]
					},
					deptName: {
						rules: [{
							required: true,
							errorMessage: 'Please enter the department name'
						}, ]
					},
					orderNum: {
						rules: [{
							required: true,
							errorMessage: 'Please enter the orderNum'
						}, ]
					}
				},
				listLoding: [],
				formData: {
					deptName: '', //部门名称
					parentId: '', //商机部门
					phone: '', //手机号
					orderNum: '', //排序
					status: '', //是否禁用
					email: '', //邮箱
					leader: '', //负责人
				},
				ManagerList: [],
				selected: 0,
				sexSelect: [{
						title: 'normal',
						id: 0
					},
					{
						title: 'Deactivate',
						id: 1
					}

				],
				employeeMap:new Map(),
				hideShow:50,
				editId:''
			}
		},
		onLoad(option) {
			this.list(); //部门列表
			if (option.deptId) {
				this.detailsLoding(option.deptId);
				this.editId=option.deptId
			}
			this.hideShow=option.showHide
			//console.log(option.showHide)
		},
		methods: {
			handEdit(){
				if (!this.isSubmit) {
				       this.isSubmit = true;
				       setTimeout(() => {
				         this.isSubmit = false;
				       }, 2000); // 设置 2 秒后可再次点击
				}
				//编辑
				DeptamentEdit(this.formData).then((data) => {
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
				DeptamentDetails(id).then((res) => {
					if (res.code == 0) {
						var result = res.data;
						//console.log(res, '部门详情')
						yuanAarngemnt({
							pageNum:1,
							pageSize:100,
							deptIdWithChildren: true,
							showAll: true
						}).then((data1)=>{
							//console.log(data,'员工列表')
							data1.data.List.map(row => {
								this.employeeMap.set(row.Id, row);
							});
							if(this.employeeMap.get(parseInt(result.leader))){
								this.ManagerList.push({
									id:result.leader,
									name:this.employeeMap.get(parseInt(result.leader)).RealName
								})
							}
							
						})
						this.formData = {
							deptName: result.deptName, //部门名称
							parentId: result.parentId, //商机部门
							phone: result.phone, //手机号
							orderNum: result.orderNum, //排序
							status: parseInt(result.status), //是否禁用
							email: result.email, //邮箱
							leader: result.leader, //负责人
							OrgId:result.OrgId,
							deptId:result.deptId,
							createId:result.createId,
							createTime:result.createTime,
							updateTime:result.updateTime,
							ancestors:result.ancestors
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
					DeptamentAdd(this.formData).then((data) => {
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
			list() {
				DeptamentList({
					OrgId: this.$store.getters.orgId
				}).then((res) => {
					if (res.code == 0) {
						//console.log(res,'部门列表')
						var depteMent = res.data;
						depteMent.map((row) => {
							this.listLoding.push({
								text: row.deptName,
								value: row.deptId
							})
						})
					}
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
		.Collaborator-item-flex {
			color: #fff;
			display: flex;
			font-size:32rpx;
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
	.addPool-form{
		margin:30rpx 3%;
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