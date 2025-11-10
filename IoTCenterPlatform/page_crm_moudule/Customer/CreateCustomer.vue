<template>
	<view id="CreateCustomer">
		<top leftIcon="icon-fanhui" :isleftBack="true" leftText="Back" backgroundColor="#161A26" title="Create customer"
			class="addPool-top"></top>
		<uni-forms ref="form" :modelValue="formData" label-position="top" class="addPool-form" label-width="100%"
			:rules="rules">
			<uni-forms-item label=" Customer name" name="customerName" required class="addPool-form-item">
				<input placeholder-class="PlaceStyle" type="text" v-model="formData.customerName"
					class="addPool-easyinput" placeholder="Please enter customer name">
			</uni-forms-item>
			<uni-forms-item label=" Customer number" name="customerNumber" required class="addPool-form-item">
				<input placeholder-class="PlaceStyle" disabled v-model="formData.customerNumber" type="text"
					class="addPool-easyinput" style="background: #1C2232!important;color:rgba(255,255,255,.5);"
					placeholder="KF2309070000017">
			</uni-forms-item>
			<uni-forms-item label=" Customer type" name="customerType" required class="addPool-form-item">
				<uni-data-select :isDark="true" v-model="formData.customerType" :localdata="customerTypeData"
					type="line" placeholder="Please select a customer type" class="addPool-selected"></uni-data-select>
			</uni-forms-item>
			<uni-forms-item label="Manager" name="name" class="addPool-form-item">
				<view class="Collaborator-item" @click="handManager">
					<view class="Collaborator-item-flex">
						<view v-if="ManagerList.length>0">
							<view v-for="(item,index) in ManagerList" :key="index" style="margin-right:10rpx;">
								{{item.name}}
							</view>
						</view>
						<text v-else style="color:rgba(255, 255, 255, .2);font-size:32rpx;">
							Please select the person in charge
						</text>
						<view class="addPoolIcon iconfont icon-a-youjiantoubai">

						</view>
					</view>
				</view>



			</uni-forms-item>
			<uni-forms-item label="Collaborator" name="name" class="addPool-form-item">
				<view class="Collaborator-item" @click="handCollAvor">
					<view class="Collaborator-item-flex">
						<view v-if="CollXieData.length>0" style="display: flex;">
							<view v-for="(item,index) in CollXieData" :key="index" style="margin-right:10rpx;">
								{{item.name?item.name:item.RealName}}
							</view>
						</view>
						<text v-else style="color:rgba(255, 255, 255, .2);font-size:32rpx;">
							Please select a collaborator
						</text>
						<view class="addPoolIcon iconfont icon-a-youjiantoubai">

						</view>
					</view>
				</view>


			</uni-forms-item>
			<uni-forms-item label="Customer source" name="name" class="addPool-form-item">
				<uni-data-select :isDark="true" v-model="formData.fromType" :localdata="fromList" type="line"
					placeholder="Please select a customer source" class="addPool-selected"></uni-data-select>
			</uni-forms-item>
			<uni-forms-item label="Company website" name="companyUrl" class="addPool-form-item">
				<input placeholder-class="PlaceStyle" type="text" v-model="formData.companyUrl"
					class="addPool-easyinput" placeholder="Please enter company website">
			</uni-forms-item>
			<!-- 	<uni-forms-item label=" Company website" name="name"  class="addPool-form-item" style="color:red;">
				<input  type="text" class="addPool-easyinput" placeholder="Please enter company website">
			</uni-forms-item> -->
			<uni-forms-item label=" Telephone number" name="name" class="addPool-form-item" style="color:red;">
				<input placeholder-class="PlaceStyle" type="text" class="addPool-easyinput"
					placeholder="Please enter the telephone number" v-model="formData.companyTel">
			</uni-forms-item>
			<uni-forms-item label="Industry Type" name="name" class="addPool-form-item">
				<uni-data-picker class="addPool-selected" v-model="formData.industry"
					style="color:rgba(255, 255, 255, .2)" placeholder="Please select Industry type"
					popup-title="Please select" :isDark="true" :clear-icon="false" :localdata="ObtainInData"
					@change="onchange" @nodeclick="onnodeclick">
				</uni-data-picker>
			</uni-forms-item>
			<uni-forms-item label="Customer details" name="name" class="addPool-form-item">
				<textarea placeholder-class="PlaceStyle" value="" v-model="formData.remark"
					placeholder="Please enter the customer details" class="addPool-textarea" />
			</uni-forms-item>
		</uni-forms>
		<button v-if="EditId" :disabled="isSubmit" :style="{'opacity':isSubmit?0.6:1,'margin-top': '60rpx'}"
			class="addPool-button" @click="handEditForm" :loading="isSubmit">
			Save
		</button>
		<button v-else :disabled="isSubmit" :style="{'opacity':isSubmit?0.6:1,'margin-top': '60rpx'}"
			class="addPool-button" @click="handSubmitHigh" :loading="isSubmit">
			Save
		</button>
		<msg-prompt ref="promptMsg"></msg-prompt>
	</view>
</template>

<script>
	import {
		CustomerNumber, //生成客户编号
		IndustryType,
		pubAddKeHhu, //添加客户
		DataPoolsDetails, //客户详情
		pubAddEditXiu //修改客户
	} from "@/api/crmApi";
	import {
		setPagesParam
	} from '@/common/utillib.js'
	export default {
		data() {
			return {
				isSubmit: false,
				EditId: '',
				rules: {
					customerName: {
						rules: [{
							required: true,
							errorMessage: 'Customer name cannot be empty'
						}]
					},
					customerType: {
						rules: [{
							required: true,
							errorMessage: 'Customer type cannot be empty'
						}, {
							format: 'number',
							errorMessage: 'Please select customer type'
						}]
					}
				},
				ManagerList: [],
				customerTypeData: [{
						value: 0,
						text: 'agent'
					},
					{
						value: 1,
						text: 'Direct sales'
					},
				],
				CollXieData: [],
				ObtainInData: [],
				fromList: [{
						value: "weixin",
						text: "WeChat Clues"
					},
					{
						value: "form",
						text: "Process Form"
					},
					{
						value: "other",
						text: "other"
					}
				],
				formData: {
					leaderId: 0,
					BindOrgId: 0,
					helperName: '',
					helper: '',
					industry: '',
					customerNumber: '', //客户编号
					customerName: "", //客户名称
					customerType: null, //客户类型：0为代理，1为直销（新增必填）
					fromType: "", //线索来源
					companyUrl: '', //公司网址
					companyTel: '', //公司电话
					remark: '', //客户详情
				},
				value: '',
				range: [{
						value: 0,
						text: "篮球"
					},
					{
						value: 1,
						text: "足球"
					},
					{
						value: 2,
						text: "游泳"
					},
				],
				selectText: ''
			}
		},
		onLoad(option) {
			this.list();
			if (option.id) {
				this.detailsData(option.id);
				this.EditId = option.id
			} else {
				this.bainNumber();
			}
		},
		methods: {
			handEditForm() {
				if (!this.isSubmit) {
					this.isSubmit = true;
					setTimeout(() => {
						this.isSubmit = false;
					}, 2000); // 设置 2 秒后可再次点击
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
					pubAddEditXiu(this.formData).then((data) => {
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
				DataPoolsDetails({
					id: id
				}).then((res) => {
					if (res.code == 0) {
						var result = res.data;
						//this.CollXieData=result.HelperUsers
						if (result.HelperUsers) {
							result.HelperUsers.forEach((ite, inx) => {
								this.CollXieData.push({
									id: ite.Id,
									name: ite.RealName
								})
							})
						}
						this.ManagerList.push({
							id: result.LeaderId,
							name: result.LeaderName
						})
						//console.log(this.CollXieData,'this.ManagerList');
						this.formData = {
							leaderId: result.LeaderId,
							BindOrgId: result.BindOrgId,
							helperName: result.HelperName,
							helper: result.Helper,
							industry: result.Industry,
							customerNumber: result.CustomerNumber, //客户编号
							customerName: result.CustomerName, //客户名称
							customerType: result.CustomerType, //客户类型：0为代理，1为直销（新增必填）
							fromType: result.FromType, //线索来源
							companyUrl: result.CompanyUrl, //公司网址
							companyTel: result.CompanyTel, //公司电话
							remark: result.Remark, //客户详情
						}
						//console.log(this.formData,'this.formData')
					}
				})
			},
			handSubmitHigh() {
				// 部分表单进行校验，接受一个参数，类型为 String 或 Array ，只校验传入 name 表单域的值
				if (!this.isSubmit) {
					this.isSubmit = true;
					setTimeout(() => {
						this.isSubmit = false;
					}, 2000); // 设置 2 秒后可再次点击
				}
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
					}
					// console.log(this.formData,'this.formData');
					// return
					pubAddKeHhu(this.formData).then((data) => {
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
			onnodeclick() {

			},
			onchange(e) {
				//  console.log(e.detail,'1111')
				//const value = e.detail.value
				this.formData.industry = e.detail.value[1].value

			},
			list() {

				//行业类型
				IndustryType().then((res) => {
					if (res.code == 0) {
						//console.log(res,'获取行业列表')
						var arr = [] //第一级
						var children = [] //第二级
						for (var i = 0; i < res.data.length; i++) {
							if (res.data[i].ParentId == 0) {
								arr.push({
									value: res.data[i].Id,
									text: res.data[i].Name,
									ParentId: res.data[i].ParentId
								})
							} else {
								children.push({
									value: res.data[i].Id,
									text: res.data[i].Name,
									ParentId: res.data[i].ParentId
								})
							}
						}

						arr.forEach((item, index) => {
							var str = []
							children.forEach((v) => {
								if (item.value == v.ParentId) {
									//console.log(item.value,v.ParentId,'v.ParentId')
									str.push({
										value: v.value,
										text: v.text,
										ParentId: v.ParentId
									})
									//console.log(str,'str')
								}
							})
							item['children'] = str;
						})
						this.ObtainInData = arr;
						//console.log(arr,'add')
						//console.log(this.ObtainInData,'this.ObtainInData')
					}

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
			bainNumber() {
				//生成编号
				CustomerNumber().then((res) => {
					//console.log(res)
					this.formData.customerNumber = res.data;
				})
			},
		}
	}
</script>

<style lang="less" scoped>
	page {
		background: #161A26;

		/deep/.input-value-border {
			height: 90rpx;
			border: 1rpx solid rgba(255, 255, 255, .2);
		}

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
			color: #fff;
			display: flex;
			font-size: 32rpx;

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

	.addPoolIcon {
		font-size: .6rem;
		margin-left: auto;
		color: rgb(153, 153, 153);
	}
	/deep/.selected-list{
		font-size:32rpx!important;
	}
</style>