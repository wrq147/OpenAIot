<template>
	<view id="CreateCustomer">
		<top leftIcon="icon-fanhui" :isleftBack="true" backgroundColor="#ffffff" title="添加联系人"
			class="addPool-top"></top>
		<uni-forms ref="form" :modelValue="formData" label-position="top" class="addPool-form" label-width="100%"
			:rules="rules">
			<uni-forms-item label=" 姓名" name="realName" required class="addPool-form-item">
				<input placeholder-class="PlaceStyle" type="text" v-model="formData.realName" class="addPool-easyinput"
					placeholder="请输入姓名">
			</uni-forms-item>

			<uni-forms-item label="   手机号" name="mobile" required class="addPool-form-item">
				<input placeholder-class="PlaceStyle" maxlength="11" type="text" v-model="formData.mobile" class="addPool-easyinput"
					placeholder="请输入手机号">
			</uni-forms-item>
			<uni-forms-item label="客户" required name="customerId" id="customerId_form" labelFont="32rpx"
				contentFont="32rpx" :requireOpacity="0.5" class="addPool-form-item">
				<view id="customerId" class="form_li" @click="selectCustoms">
					<view class="form_input"
						:class="{'placeholder_input':!formData.customerName||formData.customerName==''}">
						{{formData.customerName?formData.customerName:'请选择客户'}}
					</view>
					<view class="form_sel_icon">
						<custom-icons iconsName="icon-a-youjiantoubai" iconsSize="20rpx"
							iconsColor="#999999"></custom-icons>
					</view>
				</view>
			</uni-forms-item>
			<uni-forms-item label="部门" name="deptName" class="addPool-form-item">
				<input placeholder-class="PlaceStyle" type="text" v-model="formData.deptName" class="addPool-easyinput"
					placeholder="请输入部门">
			</uni-forms-item>
			<uni-forms-item label="职务" name="postName" class="addPool-form-item">
				<input placeholder-class="PlaceStyle" type="text" v-model="formData.postName" class="addPool-easyinput"
					placeholder="请输入职务">
			</uni-forms-item>
			<uni-forms-item label="微信号" name="wxNumber" class="addPool-form-item">
				<input placeholder-class="PlaceStyle" type="text" v-model="formData.wxNumber" class="addPool-easyinput"
					placeholder="请输入微信号">
			</uni-forms-item>
			<!-- 	<uni-forms-item label=" Company website" name="name"  class="addPool-form-item" style="color:red;">
				<input  type="text" class="addPool-easyinput" placeholder="Please enter company website">
			</uni-forms-item> -->
			<uni-forms-item label="邮箱" name="email" class="addPool-form-item">
				<input placeholder-class="PlaceStyle" type="text" v-model="formData.email" class="addPool-easyinput"
					placeholder="请输入邮箱">
			</uni-forms-item>
			<uni-forms-item label="性别" name="name" class="addPool-form-item">
				<view class="addPool-form-item-Sex">
					<view :class="[formData.sex==item.id?'active':'on']" class="addPool-form-item-Sex-item"
						v-for="(item,index) in sexSelect" @click="handClick(index)">
						{{item.title}}
					</view>
				</view>
			</uni-forms-item>
			<uni-forms-item label="联系人详情" name="remark" class="addPool-form-item">
				<textarea placeholder-class="PlaceStyle" value="" v-model="formData.remark" placeholder="请输入联系人详情"
					class="addPool-textarea" />
			</uni-forms-item>
			<uni-forms-item label="  负责人" required name="leaderId" class="addPool-form-item">
				<view class="Collaborator-item" @click="handManager">
					<view class="Collaborator-item-flex">
						<view v-if="ManagerList.length>0">
							<view v-for="(item,index) in ManagerList" :key="index" style="margin-right:10rpx;">
								{{item.name}}
							</view>
						</view>
						<view v-else style="color:#C1C1C1;">
							请输入负责人
						</view>
						<view class="addPoolIcon iconfont icon-a-youjiantoubai">
							
						</view>	
					</view>
				</view>
			</uni-forms-item>
			<uni-forms-item label="协作人" name="name" class="addPool-form-item" style="color:red;">
				<view class="Collaborator-item" @click="handCollAvor">
					<view class="Collaborator-item-flex">
						<view v-if="CollXieData.length>0" style="display: flex;">
							<view v-for="(item,index) in CollXieData" :key="index" style="margin-right:10rpx;">
								{{index==0?item.name:','+item.name}}
							</view>
						</view>
						<view v-else style="color:#C1C1C1;">
							请输入协作人
						</view>
						<view class="addPoolIcon iconfont icon-a-youjiantoubai">
							
						</view>	

					</view>
				</view>
			</uni-forms-item>

		</uni-forms>
		<button v-if="EditId" :style="{'opacity':isSubmit?0.6:1,'margin-top': '60rpx'}" :disabled="isSubmit" :loading="isSubmit" class="addPool-button" @click="handEditForm">
			保存
		</button>
		<button v-else :style="{'opacity':isSubmit?0.6:1,'margin-top': '60rpx'}" :disabled="isSubmit" :loading="isSubmit" class="addPool-button" @click="handSubmitHigh">
			保存
		</button>
		<msg-prompt ref="promptMsg"></msg-prompt>
	</view>
</template>

<script>
	import {
		yuanAarngemnt //员工管理
	} from "@/api/personalCenter";
	import {
		CustomerList, //客户名称
		addContact, //新增联系人
		CustomerInfo, //联系人详情
		EditContact //联系人编辑
	} from "@/api/crmApi";
	import {
		setPagesParam
	} from '@/common/utillib.js'
	export default {
		data() {
			return {
				isSubmit: false,
				rules: {
					// leaderId: {
					// 	rules: [{
					// 		required: true,
					// 		errorMessage: 'Please select a Manager'
					// 	}]
					// },
					customerId: {
						rules: [{
							required: true,
							errorMessage: '请选择客户'
						}]
					},
					realName: {
						rules: [{
							required: true,
							errorMessage: '请输入联系人姓名'
						}, ]
					},
					mobile: {
						rules: [{
							required: true,
							errorMessage: '请输入电话号码'
						},{
							validateFunction: function(rule, value, data, callback) {
								let iphoneReg = (
									/^(13[0-9]|14[1579]|15[0-3,5-9]|16[6]|17[0123456789]|18[0-9]|19[89])\d{8}$/
								); //手机号码
								if (!iphoneReg.test(value)) {
									callback('请输入正确的手机号格式！')
								}
							}
							} ]
					}
				},
				selectText: '',
				CollXieData: [],
				ManagerList: [],
				CustomerData: [],
				formData: {
					realName: "", //联系人
					mobile: "", //手机号
					customerId: "", //客户id
					customerName: "", //客户名称
					postName: "", //职位
					deptName: "", //部门
					wxNumber: "", //微信号
					email: "", //邮箱
					sex: 2, //性别
					helperName: '', //协作人员名称
					helper: "",
					remark: "", //联系人详情
					leaderId: "", //负责人
				},
				selected: 0,
				sexSelect: [{
						title: '男',
						id: 0
					},
					{
						title: '女',
						id: 1
					}

				],
				value: '',
				EditId: '',
				//employeeMap: new Map(),
			}
		},
		onLoad(option) {
			this.CustomeLoading(); //客户名称
			if (option.id) {
				this.detailsData(option.id);
				this.EditId = option.id
			}
		},
		methods: {
			finishSelectCustom(arr) {
				if (!arr || arr.length == 0) return
				this.formData.customerId = arr[0].Id;
				this.formData.customerName = arr[0].CustomerName;
			},
			selectCustoms() {
				uni.navigateTo({
					url: '/pages_factory/custom_select?isSelect=true'
				})
			},
			handEditForm() {
				if (!this.isSubmit) {
					this.isSubmit = true;
					setTimeout(() => {
						this.isSubmit = false;
					}, 2000); // 设置 2 秒后可再次点击
				}
				if(this.formData.email!=''&&this.formData.email){
					var Regex = /^(?:\w+\.?)*\w+@(?:\w+\.)*\w+$/;
					if(!Regex.test(this.formData.email)){
						this.setMsgTop('请输入正确的邮箱！')	
						return
					}
				}
				this.$refs.form.validate().then((res) => {
				var helperName = []
				var helper = []
				this.CollXieData.forEach((row) => {
					helperName.push(row.name)
					helper.push(row.id)
				})
				this.formData.helperName = helperName.join(',');
				this.formData.id = this.EditId
				this.formData.helper = helper.join(',');
				if (this.ManagerList.length == 1) {
					this.formData.leaderId = this.ManagerList[0].id
				}
				// console.log(this.formData,'这里是编辑')
				// return
				EditContact(this.formData).then((data) => {
					if (data.code == 0) {
						uni.showToast({
							title: '修改成功！',
							icon: 'none'
						})
						setTimeout(() => {
							setPagesParam('list', this.EditId)
						}, 1000)
					}
				}).catch((err) => {
					this.setMsgTop(err)
				})
				})
			},
			detailsData(id) {
				CustomerInfo({
					id: id
				}).then((res) => {
					if (res.code == 0) {
						var data = res.data;
						//this.CollXieData=result.HelperUsers
						//console.log(res,'详情')
						if (data.Helper) {
							this.CollXieData = data.HelperUsers.map(row => {
								let obj = {
									id: row.Id,
									name: row.RealName,
									avatar: row.Avatar,
								}
								return obj
							})
							data.HelperName = data.HelperUsers.map(row => row.RealName)
						}
						if (data.LeaderId) {
							//获取责任人相关信息
							this.ManagerList = [{
								id: data.LeaderUser.Id,
								name: data.LeaderUser.RealName,
								avatar: data.LeaderUser.Avatar,
								type: "user",
							}]
						}
						//console.log(this.CollXieData,'1111')
						this.formData = {
							realName: data.RealName, //联系人
							mobile: data.Mobile, //手机号
							customerId: data.CustomerId, //客户id
							customerName: data.CustomerName, //客户名称
							postName: data.PostName, //职位
							deptName: data.DeptName, //部门
							wxNumber: data.WxNumber, //微信号
							email: data.Email, //邮箱
							sex: data.Sex, //性别
							helperName: data.HelperName,
							helper: data.Helper,
							remark: data.Remark, //联系人详情
							leaderId: data.LeaderId, //负责人
							id: data.Id,
						};
						//console.log(this.formData,'this.formData')
					}
				})
			},
			handSubmitHigh() {
				if (!this.isSubmit) {
					this.isSubmit = true;
					setTimeout(() => {
						this.isSubmit = false;
					}, 2000); // 设置 2 秒后可再次点击
				}
				if(this.formData.email!=''&&this.formData.email){
					var Regex = /^(?:\w+\.?)*\w+@(?:\w+\.)*\w+$/;
					if(!Regex.test(this.formData.email)){
						this.setMsgTop('请输入正确的邮箱！')	
						return
					}
				}
				// 部分表单进行校验，接受一个参数，类型为 String 或 Array ，只校验传入 name 表单域的值
				this.$refs.form.validate().then((res) => {
					var helperName = []
					var helper = []
					this.CollXieData.forEach((row) => {
						helperName.push(row.name)
						helper.push(row.id)
					})
					this.formData.helperName = helperName.join(',');
					this.formData.helper = helper.join(',');
					if (this.ManagerList.length == 1) {
						this.formData.leaderId = this.ManagerList[0].id
						this.formData.LeaderName = this.ManagerList[0].name
					}
					//	console.log(this.formData,'this.formData')
					//return
					addContact(this.formData).then((data) => {
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
			selectEmplee(data) {
				//console.log(data,'data');
				if (this.selectText == 'Collaborator') {
					this.CollXieData = data;
				} else {
					//console.log('负责人数据')
					this.ManagerList = data;
				}
			},
			handCollAvor() {
				this.selectText = 'Collaborator'
				uni.navigateTo({
					url: '/pages_flow/inventory/employee_select?multiple=true'
				})
			},

			handManager() {
				this.selectText = 'Manager'
				uni.navigateTo({
					url: '/pages_flow/inventory/employee_select?type=user'
				})
			},
			CustomeLoading() {
				CustomerList({
					showAll: true
				}).then((res) => {
					if (res.code == 0) {
						//console.log(res,'客户名称数据')
						res.data.List.forEach((ite, inx) => {
							this.CustomerData.push({
								value: ite.Id,
								text: ite.CustomerName
							})
						})
					}
				})

			},
			handClick(inx) {
				this.formData.sex = inx;
				console.log(this.formData.sex)
			}
		}
	}
</script>
<style>
	page {
		background: #fff;
	}
</style>
<style lang="less" scoped>
	.Collaborator-item-flex {
		color: #333;
		display: flex;
	}

	.Collaborator-item {
		height: 90rpx;
		background: #f8f8f8;
		border-radius: 8rpx;
		line-height: 90rpx;
		padding: 0rpx 20rpx;
		font-size: 32rpx;
	}


	.addPool-form-item-Sex {
		display: flex;
		justify-content: space-between;

		.addPool-form-item-Sex-item {
			width: 48%;
			border: 1rpx solid #2371FF;
			border-radius: 10rpx;
			text-align: center;
			height: 88rpx;
			line-height: 88rpx;
			color: #2371FF;

		}

		.active {
			color: FF3535;
		}

		.on {
			color: #333333;
			border: 1rpx solid #eeeeee;
		}

	}
</style>