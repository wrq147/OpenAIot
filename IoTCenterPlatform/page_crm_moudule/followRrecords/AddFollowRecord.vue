<template>
	<view id="CreateCustomer">
		<top leftIcon="icon-fanhui" :isleftBack="true" leftText="Back" backgroundColor="#161A26"
			title="Create followed up" class="addPool-top"></top>
		<uni-forms ref="form" :rules="rules" :modelValue="formData" label-position="top" class="addPool-form"
			label-width="100%">
			<uni-forms-item required label=" Follow up content" name="remark" class="addPool-form-item">
				<textarea placeholder-class="PlaceStyle" v-model="formData.remark"
					placeholder="Please enter the follow up content" class="addPool-textarea" />
			</uni-forms-item>
			<uni-forms-item required label=" Objective" name="targetId" class="addPool-form-item">
				<view class="Collaborator-item">
					<uni-data-select placeholder-class="PlaceStyle" :clear="false" :isDark="true"
						v-model="formData.targetType" :localdata="range" type="line" placeholder="" class="left"
						@change="handClickSwitch"></uni-data-select>
					<uni-data-select :isDark="true" placeholder-class="PlaceStyle" v-model="formData.targetId"
						:localdata="periodArr" type="line" placeholder="Please select a customer"
						class="addPool-selected Collaborator-item-select" @change="handClickPeriod"></uni-data-select>
				</view>
			</uni-forms-item>
			<uni-forms-item required label=" Execution time" name="followTime" class="addPool-form-item">
				<uni-datetime-picker ref="dateChoice1" class="date" type="datetime" v-model="formData.followTime"
					placeholder-style="font-size:32rpx;color:#999999" :clear-icon="false"
					placeholder='Please select the outbound time' :isCustom="true" :isDark="true">
				</uni-datetime-picker>
			</uni-forms-item>
			<uni-forms-item required label=" Execution mode" name="followWay" class="addPool-form-item">
				<uni-data-select :isDark="true" placeholder-class="PlaceStyle" v-model="formData.followWay"
					:localdata="ExecutionMode" type="line" placeholder="Please select a execution mode"
					class="addPool-selected"></uni-data-select>
			</uni-forms-item>
			
			
			<uni-forms-item required label=" Plan executor" name="followUser" class="addPool-form-item">
				<view class="Collaborator-item" style="display: flex;" @click="handPerson">
						<view class="addPool-easyinput" style="display: flex;">
							<view v-if="formData.followUser">
								{{personText}}	
							</view>	
							<view v-else style="color:#C1C1C1;font-size:32rpx;">
								Please select a follow-up person
							</view>
							<view class="addPoolIcon iconfont icon-a-youjiantoubai">
							</view>
						</view>
				</view>
			</uni-forms-item>
			<uni-forms-item label="Opportunity" name="opportId" class="addPool-form-item">
				<view class="Collaborator-item" style="display: flex;" @click="handBusiness">
						<view class="addPool-easyinput" style="display: flex;">
							<view v-if="formData.opportId">
								{{opportNameText}}	
							</view>	
							<view v-else style="color:#C1C1C1;font-size:32rpx;">
								Please select the business opportunity
							</view>
							<view class="addPoolIcon iconfont icon-a-youjiantoubai">
							</view>
						</view>
				</view>
			</uni-forms-item>
			<uni-forms-item label="Contacts" name="contactId" class="addPool-form-item">
				<view class="Collaborator-item" style="display: flex;" @click="handContact">
						<view class="addPool-easyinput" style="display: flex;">
							<view v-if="formData.contactId">
								{{contentText}}	
							</view>	
							<view v-else style="color:#C1C1C1;font-size:32rpx;">
								Please select a contact person
							</view>
							<view class="addPoolIcon iconfont icon-a-youjiantoubai">
							</view>
						</view>
				</view>
			</uni-forms-item>
		</uni-forms>
		<button v-if="EditId" :disabled="isSubmit" class="addPool-button" @click="handEditForm">
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
		CustomerList, //客户接口
		BusineData, //商机列表
		contactData, //联系人
		xianSuo, //线索列表
		FollowRecord, //跟进记录新增
		flowDataRecordDetails, //跟进记录详情
		FollowEdit //跟进记录编辑
	} from "@/api/crmApi";
	import {
		yuanAarngemnt //员工
	} from "@/api/personalCenter";
	import {
		setPagesParam
	} from '@/common/utillib.js'
	var dayjs = require('@/common/day.js')
	export default {
		data() {
			return {
				isSubmit: false,
				rules: {
					remark: {
						rules: [{
							required: true,
							errorMessage: 'Please enter Follow up content'
						}]
					},
					targetId: {
						rules: [{
							required: true,
							errorMessage: 'Please select Objective'
						}]
					},
					followTime: {
						rules: [{
							required: true,
							errorMessage: 'Please select Execution time'
						}]
					},
					followWay: {
						rules: [{
							required: true,
							errorMessage: 'Please select Execution mode'
						}]
					},
					followUser: {
						rules: [{
							required: true,
							errorMessage: 'Please select  Plan executor'
						}]
					},

				},
				periodArr: [],
				formData: {
					contactId: '',
					followTime: '',
					followUser: '',
					followWay: '',
					opportId: '',
					remark: '',
					targetId: '',
					targetType: 0
				},
				value: '',
				range: [{
						value: 0,
						text: "Customer"
					},
					{
						value: 1,
						text: "clue"
					}
				],
				ExecutionMode: [], //跟进方式 Execution mode
				EmployeeList: [], //员工
				OpportunityList: [], //商机
				concantList: [], //联系人
				EditId: '',
				opportNameText:'',
				contentText:'',
				personText:''
			}
		},
		onLoad(option) {
			this.list(); //预加载数据
			this.data(); //线索客户切换预加载数据
			if (option.id) {
				this.detailsData(option.id);
				this.EditId = option.id
			} else {
				this.formData.followTime = dayjs(new Date()).format('YYYY-MM-DD HH:mm:ss')
			}
		},
		methods: {
			//跟进人
			selectEmplee(data){
				//console.log(data,'data')
				this.formData.followUser=data[0].id;
				this.personText=data[0].name;
			},
			//跟进人
			handPerson(){
				uni.navigateTo({
					url: '/pages_Inventory/employee_select?type=user'
				})
			},
			//联系人
			ContactData(data){
				//console.log(data,'联系人')
				this.formData.contactId=data.Id;
				this.contentText=data.RealName;
			},
			//联系人
			handContact(){
				uni.navigateTo({
					url:'/page_crm_moudule/contacts/contacts?ContactsAll=true'
				})
			},
			//商机
			customData(data){
				//console.log(data,'商机')
				this.formData.opportId=data.Id;
				this.opportNameText=data.OpportName;
			},
			//商机
			handBusiness(){
				uni.navigateTo({
					url:'/page_crm_moudule/Opportunity/Opportunity?opportHide=true'
				})
			},
			handEditForm() {
				// console.log(this.formData,'这里是编辑')
				// return
				if (!this.isSubmit) {
					this.isSubmit = true;
					setTimeout(() => {
						this.isSubmit = false;
					}, 2000); // 设置 2 秒后可再次点击
				}
				this.$refs.form.validate().then((res) => {
					FollowEdit(this.formData).then((data) => {
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
				flowDataRecordDetails({
					id: id
				}).then((res) => {
					if (res.code == 0) {
						var data = res.data;
						//console.log(res, '详情数据')
						if(data.FollowUserInfo){
							this.personText=data.FollowUserInfo.RealName
						}
						this.opportNameText=data.OpportName
						if(data.ContactInfo){
							this.contentText=data.ContactInfo.RealName
						}
						this.formData = {
							contactId: data.ContactId,
							followTime: data.FollowTime,
							followUser: data.FollowUser,
							followWay: data.FollowWay,
							opportId: data.OpportId,
							remark: data.Remark,
							targetId: data.TargetId,
							targetType: data.TargetType,
							id: data.Id
						}
						this.data();
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
					// console.log(this.formData,'this.formData');
					// return
					FollowRecord(this.formData).then((data) => {
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
			handClickSwitch(e) {
				//console.log(e)
				this.formData.targetType = e;
				this.data();
			},
			handClickPeriod(e) {
				this.formData.targetId = e
			},
			async list() {
				//字段选择方式  Execution mode
				let partUnit = await this.$store.dispatch("data/dictList", 'follow_way');
				//console.log(partUnit,'partUnit')
				partUnit.forEach((item) => {
					this.ExecutionMode.push({
						value: item.value,
						text: item.label
					})
				})
			},
			data() {
				if (this.formData.targetType == 0) {
					//客户
					CustomerList({
						pageNum: 1,
						pageSize: 100
					}).then((res) => {
						//console.log(res,'客户')
						this.periodArr = []
						res.data.List.forEach((row) => {
							this.periodArr.push({
								value: row.Id,
								text: row.CustomerName,
							})
						})
					})
				} else {
					//线索
					xianSuo({
						pageNum: 1,
						pageSize: 100
					}).then((res) => {
						//console.log(res,'线索')
						this.periodArr = []
						res.data.List.forEach((row) => {
							this.periodArr.push({
								value: row.Id,
								text: row.CompanyName,
							})
						})
					})
				}
			},
		}
	}
</script>

<style lang="less" scoped>
	page {
		background: #161A26;
	}
	.addPool-easyinput{
		width: 100%;
		padding:0rpx 12rpx!important;
		border:none!important;
	}
	.addPoolIcon{
		font-size:.6rem;
		margin-left: auto;
		color:rgb(153, 153, 153);
	}
	/deep/.dark_picker .uni-date-x .uni-date__x-input {
		color: #fff !important;
		font-size: 32rpx !important;
	}

	/deep/.dark_picker .uni-date-x--border {
		height: 90rpx !important;
	}

	.addPool-top {
		margin-bottom: 35rpx;
	}

	.Collaborator-item {
		color: rgba(255, 255, 255, .4);
		height: 90rpx;
		border: 1rpx solid rgba(255, 255, 255, .2);
		border-radius: 8rpx;
		line-height: 90rpx;
		padding: 0rpx 20rpx;
		display: flex;
		font-size: 32rpx;

		/deep/.left {}

		.left.uni-stat__select {
			flex: .3;
			width: 200rpx !important;
			text-align: center;

			/deep/.uni-select__input-text {
				margin-right: 22rpx !important;
			}
		}

		/deep/.uni-select {
			border: none !important;
		}
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

	// /deep/.uni-navbar__header-btns-left{
	// 	width: 125rpx!important;
	// }
</style>