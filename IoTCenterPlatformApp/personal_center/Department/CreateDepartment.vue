<template>
	<view id="CreateCustomer">
		<top leftIcon="icon-fanhui" :isleftBack="true"  backgroundColor="#ffffff"
			title="部门" class="addPool-top"></top>
		<uni-forms ref="form" :modelValue="formData" label-position="top" class="addPool-form" label-width="100%"
			:rules="rules">
			<uni-forms-item label="    上级部门" required name="parentId" class="addPool-form-item" v-if="hideShow!=0">
				<uni-data-select placeholder-class="PlaceStyle"  type="line" v-model="formData.parentId" :isDark="true" :localdata="listLoding"
					placeholder="请选择上级部门" class="addPool-selected"></uni-data-select>
			</uni-forms-item>
			<uni-forms-item label="   部门名称" name="deptName" required class="addPool-form-item"
				style="color:red;">
				<input placeholder-class="PlaceStyle" type="text" v-model="formData.deptName" class="addPool-easyinput"
					placeholder="请选择部门名称">
			</uni-forms-item>
			<uni-forms-item label="   显示排序" name="orderNum" required class="addPool-form-item" style="color:red;">
				<input placeholder-class="PlaceStyle" type="text" v-model="formData.orderNum" class="addPool-easyinput"
					placeholder="排序">
			</uni-forms-item>
			<!-- <uni-forms-item label=" 负责人" name="leader" class="addPool-form-item">
				<view class="Collaborator-item" @click="handManager">
					<view class="Collaborator-item-flex">
						<view v-if="ManagerList.length>0">
							<view v-for="(item,index) in ManagerList" :key="index" style="margin-right:10rpx;">
								{{item.name}}
							</view>
						</view>
						<view v-else style="color:#C1C1C1;font-size:32rpx;">
							请选择负责人
						</view>
					</view>
				</view>
			</uni-forms-item> -->
			<uni-forms-item label="联系电话" name="phone" class="addPool-form-item">
				<input placeholder-class="PlaceStyle" type="text" v-model="formData.phone" class="addPool-easyinput"
					placeholder="请选择联系电话">
			</uni-forms-item>
			<uni-forms-item label="邮箱" name="email" class="addPool-form-item">
				<input placeholder-class="PlaceStyle" type="text" v-model="formData.email" class="addPool-easyinput"
					placeholder="请选择邮箱">
			</uni-forms-item>
			<!-- <uni-forms-item label="是否启用" name="name" class="addPool-form-item" style="color:red;">
				<view class="addPool-form-item-Sex">
					<view :class="[formData.status==item.id?'active':'on']" class="addPool-form-item-Sex-item"
						v-for="(item,index) in sexSelect" @click="handClick(index)">
						{{item.title}}
					</view>
				</view>
			</uni-forms-item> -->
			<view style="display: flex;justify-content: space-between;">
				<view style="color:rgba(153, 153, 153, 1);">
					是否启用
				</view>
				<switch :checked="checkDisHide" color="rgba(71, 226, 241, 1)" style="transform:scale(0.7)" @change="switch1Change"/>
			</view>
		</uni-forms>
		<button v-if="editId" :disabled="isSubmit" class="addPool-button" @click="handEdit">
			保存
		</button>
		<button v-else :disabled="isSubmit" class="addPool-button" @click="handSubmitHigh">
			保存
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
				checkDisHide:false,
				isSubmit:false,
				rules: {
					parentId: {
						rules: [{
							required: true,
							errorMessage: '请选择上级部门'
						}]
					},
					deptName: {
						rules: [{
							required: true,
							errorMessage: '请输入部门名称'
						}, ]
					},
					orderNum: {
						rules: [{
							required: true,
							errorMessage: '请输入订单号'
						}, ]
					}
				},
				listLoding: [],
				formData: {
					deptName: '', //部门名称
					parentId: '', //商机部门
					phone: '', //手机号
					orderNum: '', //排序
					status: 0, //是否禁用
					email: '', //邮箱
					// leader: '', //负责人
				},
				ManagerList: [],
				selected: 0,
				sexSelect: [{
						title: '启用',
						id: 0
					},
					{
						title: '停用',
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
		},
		methods: {
			switch1Change: function (e) {
									if(e.detail.value){
										this.formData.status=0
									}else{
										this.formData.status=1
									}
									//console.log(this.formData.status,'this.formData.status')
			       },
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
						if(result.status==0){
							this.checkDisHide=true;
						}else{
							this.checkDisHide=false;
						}
						//console.log(result.status,'111111')
						this.formData = {
							deptName: result.deptName, //部门名称
							parentId: result.parentId, //商机部门
							phone: result.phone, //手机号
							orderNum: result.orderNum, //排序
							status: result.status, //是否禁用
							email: result.email, //邮箱
							// leader: result.leader, //负责人
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
			// handManager() {
			// 	uni.navigateTo({
			// 		url: '/pages_flow/inventory/employee_select?type=user'
			// 	})
			// },
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
<style>
	page{
		background: #ffffff;
	}
</style>
<style lang="less" scoped>
	
		 
		.Collaborator-item-flex {
			color: #333;
			display: flex;
		}

		.Collaborator-item {
			height: 90rpx;
			border: 1rpx solid rgba(255, 255, 255, .2);
			border-radius: 8rpx;
			line-height: 90rpx;
			padding: 0rpx 20rpx;
			background: #F8F8F8;
			font-size:32rpx;
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
			border:1rpx solid #2371FF;
			color:#2371FF;
		}

		.on {
			color: #999999;
			border: 1rpx solid #C1C1C1;
		}

	}
</style>