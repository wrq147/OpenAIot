<template>
	<view id="CreateCustomer">
		<top leftIcon="icon-fanhui" :isleftBack="true" leftText="Back" backgroundColor="#161A26" title="Create contacts"
			class="addPool-top"></top>
		<uni-forms ref="form" :modelValue="formData" label-position="top" class="addPool-form" label-width="100%"
			:rules="rules">
			<uni-forms-item label=" Customer name" name="realName" required class="addPool-form-item"
				style="color:red;">
				<input placeholder-class="PlaceStyle" type="text" v-model="formData.realName" class="addPool-easyinput"
					placeholder="Please enter contact name">
			</uni-forms-item>

			<uni-forms-item label="   Telephone number" name="mobile" required class="addPool-form-item"
				style="color:red;">
				<input placeholder-class="PlaceStyle" maxlength="11" type="text" v-model="formData.mobile"
					class="addPool-easyinput" placeholder="Please enter the telephone number">
			</uni-forms-item>
			<uni-forms-item label="  Customer" required name="customerId" class="addPool-form-item">
					<view id="customerId" class="form_li" @click="selectCustoms">
						<view class="form_input"
							:class="{'placeholder_input':!formData.customerName||formData.customerName==''}">
							{{formData.customerName?formData.customerName:'Please select a customer'}}
						</view>
						<view class="form_sel_icon">
							<custom-icons iconsName="icon-a-youjiantoubai" iconsSize="20rpx"
								iconsColor="#999999"></custom-icons>
						</view>
					</view>
			</uni-forms-item>
			<uni-forms-item label="Department" name="deptName" class="addPool-form-item">
				<input placeholder-class="PlaceStyle" type="text" v-model="formData.deptName" class="addPool-easyinput"
					placeholder="Please enter the department">
			</uni-forms-item>
			<uni-forms-item label="Position" name="postName" class="addPool-form-item">
				<input placeholder-class="PlaceStyle" type="text" v-model="formData.postName" class="addPool-easyinput"
					placeholder="Please enter the position">
			</uni-forms-item>
			<uni-forms-item label="WeChat" name="wxNumber" class="addPool-form-item">
				<input placeholder-class="PlaceStyle" type="text" v-model="formData.wxNumber" class="addPool-easyinput"
					placeholder="Please enter your email address">
			</uni-forms-item>
			<!-- 	<uni-forms-item label=" Company website" name="name"  class="addPool-form-item" style="color:red;">
				<input  type="text" class="addPool-easyinput" placeholder="Please enter company website">
			</uni-forms-item> -->
			<uni-forms-item label="E-mail" name="email" class="addPool-form-item" style="color:red;">
				<input placeholder-class="PlaceStyle" type="text" v-model="formData.email" class="addPool-easyinput"
					placeholder="Please enter your email address">
			</uni-forms-item>
			<uni-forms-item label="Sex" name="name" class="addPool-form-item" style="color:red;">
				<view class="addPool-form-item-Sex">
					<view :class="[formData.sex==item.id?'active':'on']" class="addPool-form-item-Sex-item"
						v-for="(item,index) in sexSelect" @click="handClick(index)">
						{{item.title}}
					</view>
				</view>
			</uni-forms-item>
			<uni-forms-item label="Contact details" name="remark" class="addPool-form-item">
				<textarea placeholder-class="PlaceStyle" value="" v-model="formData.remark"
					placeholder="Please enter the customer details" class="addPool-textarea" />
			</uni-forms-item>
			<uni-forms-item label=" Manager" required name="leaderId" class="addPool-form-item">
				<view class="Collaborator-item" @click="handManager">
					<view class="Collaborator-item-flex">
						<view v-if="ManagerList.length>0">
							<view v-for="(item,index) in ManagerList" :key="index" style="margin-right:10rpx;">
								{{item.name}}
							</view>
						</view>
						<text v-else style="color:rgba(255, 255, 255, .2);font-size:32rpx;">
							Please select a Manager
						</text>
						<view class="addPoolIcon iconfont icon-a-youjiantoubai">

						</view>
					</view>
				</view>
			</uni-forms-item>
			<uni-forms-item label="Collaborator" name="name" class="addPool-form-item" style="color:red;">
				<view class="Collaborator-item" @click="handCollAvor">
					<view class="Collaborator-item-flex">
						<view v-if="CollXieData.length>0" style="display: flex;">
							<view v-for="(item,index) in CollXieData" :key="index" style="margin-right:10rpx;">
								{{item.name?item.name:item.RealName}}
							</view>
						</view>
						<text v-else style="color:rgba(255, 255, 255, .2);font-size:32rpx;">
							Please select a Collaborator
						</text>
						<view class="addPoolIcon iconfont icon-a-youjiantoubai">

						</view>
					</view>
				</view>
			</uni-forms-item>

		</uni-forms>
		<button v-if="EditId" :disabled="isSubmit" :style="{'opacity':isSubmit?0.6:1,'margin-top': '60rpx'}"
			class="addPool-button" :loading="isSubmit" @click="handEditForm">
			Save
		</button>
		<button v-else :disabled="isSubmit" :style="{'opacity':isSubmit?0.6:1,'margin-top': '60rpx'}"
			class="addPool-button" :loading="isSubmit" @click="handSubmitHigh">
			Save
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
							errorMessage: 'Please select a customer'
						}]
					},
					realName: {
						rules: [{
							required: true,
							errorMessage: 'Please enter contacts name'
						}, ]
					},
					mobile: {
						rules: [{
							required: true,
							errorMessage: 'Please enter the telephone number'
						}, {
							validateFunction: function(rule, value, data, callback) {
								let iphoneReg = (
									/^(13[0-9]|14[1579]|15[0-3,5-9]|16[6]|17[0123456789]|18[0-9]|19[89])\d{8}$/
								); //手机号码
								if (!iphoneReg.test(value)) {
									callback(
										'The format of the mobile phone number is incorrect. Please fill it out again'
										)
								}
							}
						}]
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
						title: 'Male',
						id: 0
					},
					{
						title: 'Female',
						id: 1
					}

				],
				value: '',
				EditId: '',
				employeeMap: new Map(),
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
				if (this.formData.email != '' && this.formData.email) {
					var Regex = /^(?:\w+\.?)*\w+@(?:\w+\.)*\w+$/;
					if (!Regex.test(this.formData.email)) {
						this.setMsgTop('Please enter the correct email address！')
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
								title: 'Modified successfully！',
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
						console.log(res,'详情')
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
				if (this.formData.email != '' && this.formData.email) {
					var Regex = /^(?:\w+\.?)*\w+@(?:\w+\.)*\w+$/;
					if (!Regex.test(this.formData.email)) {
						this.setMsgTop('Please enter the correct email address！')
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
					url: '/pages_Inventory/employee_select?multiple=true'
				})
			},

			handManager() {
				this.selectText = 'Manager'
				uni.navigateTo({
					url: '/pages_Inventory/employee_select?type=user'
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

<style lang="less" scoped>
	page {
		/deep/.uni-select__input-text {
			font-size: 32rpx;
		}

		/deep/.uni-forms-item__label {
			font-size: 32rpx !important;
		}

		/deep/.uni-textarea-textarea {
			font-size: 32rpx;
		}

		.Collaborator-item-flex {
			font-size: 32rpx;
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

	.addPoolIcon {
		font-size: .6rem;
		margin-left: auto;
		color: rgb(153, 153, 153);
	}
	.form_li{
		line-height: 88rpx;
		font-size: 32rpx;
		width: 100%;
		display: flex;
		justify-content: flex-start;
		align-items: center;
		position: relative;
		border: 1rpx solid rgba(255, 255, 255, 0.20);
		border-radius: 12rpx;
	}
	.form_input{
				width: 100%;
				height: 88rpx;
				display: flex;
				justify-content: space-between;
				align-items: center;
				border-radius: 10rpx;
				padding: 0 60rpx 0 30rpx;
				box-sizing: border-box;
				color: #fff;
				overflow-x: scroll;
				overflow-y: hidden;
				white-space: nowrap;
				
				&.placeholder_input{
					color: rgba(255, 255, 255, .3);
				}
			}
	.form_sel_icon{
		margin-right:20rpx;
	}
</style>