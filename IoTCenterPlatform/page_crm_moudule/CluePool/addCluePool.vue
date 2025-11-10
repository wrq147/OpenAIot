<template>
	<view id="CreateCustomer">
		<top leftIcon="icon-fanhui" :isleftBack="true" leftText="Back" backgroundColor="#161A26"
			title="Create clue pool" class="addPool-top"></top>
		<uni-forms ref="form" :modelValue="formData" label-position="top" class="addPool-form" label-width="100%"
			:rules="rules">
			<uni-forms-item label=" Customer name" name="companyName" required class="addPool-form-item">
				<input placeholder-class="PlaceStyle" type="text" v-model="formData.companyName"
					class="addPool-easyinput" placeholder="Please enter customer name">
			</uni-forms-item>

			<uni-forms-item label="  Contacts" name="realName" required class="addPool-form-item">
				<input placeholder-class="PlaceStyle" type="text" v-model="formData.realName" class="addPool-easyinput"
					placeholder="Please enter contacts name">
			</uni-forms-item>
			<uni-forms-item label=" Telephone number" required name="mobile" class="addPool-form-item">
				<input placeholder-class="PlaceStyle" type="text" v-model="formData.mobile" class="addPool-easyinput"
					placeholder="Please enter the telephone number">
			</uni-forms-item>
			<uni-forms-item label="Department" name="deptName" class="addPool-form-item">
				<input placeholder-class="PlaceStyle" type="text" v-model="formData.deptName" class="addPool-easyinput"
					placeholder="Please enter the department">
			</uni-forms-item>
			<uni-forms-item label="Position" name="postName" class="addPool-form-item">
				<input placeholder-class="PlaceStyle" type="text" v-model="formData.postName" class="addPool-easyinput"
					placeholder="Please enter the position">
			</uni-forms-item>
			<uni-forms-item label="Clue source" name="fromType" class="addPool-form-item">
				<uni-data-select :isDark="true" v-model="formData.fromType" :localdata="fromList" type="line"
					placeholder="Please select a Clue source" class="addPool-selected"></uni-data-select>
			</uni-forms-item>
			<!-- 	<uni-forms-item label=" Company website" name="name"  class="addPool-form-item" style="color:red;">
				<input  type="text" class="addPool-easyinput" placeholder="Please enter company website">
			</uni-forms-item> -->
			<uni-forms-item label="Collaborator" name="name" class="addPool-form-item" style="color:red;">
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
			<uni-forms-item label="Customer details" name="remark" class="addPool-form-item">
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
		AddCluePool, //新增
		ClueDataDetails, //详情
		AddPubEdit //编辑
	} from "@/api/crmApi";
	import {
		setPagesParam
	} from '@/common/utillib.js'
	export default {
		data() {
			return {
				isSubmit: false,
				rules: {
					companyName: {
						rules: [{
							required: true,
							errorMessage: 'Please enter customer name'
						}]
					},
					realName: {
						rules: [{
							required: true,
							errorMessage: 'Please enter contacts name'
						}]
					},
					mobile: {
						rules: [{
							required: true,
							errorMessage: 'Please enter the telephone number'
						}]
					}
				},
				EditId: '',
				CollXieData: [],
				formData: {
					realName: "",
					fromId: "", //表单id
					changeId: "", //转移的客户id
					mobile: "",
					companyName: "",
					fromType: "",
					postName: "",
					deptName: "",
					helperName: [],
					helper: "",
					leaderId: 0 //线索跟进人（为0则为公海线索）
				},
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
			}
		},
		onLoad(option) {
			if (option.id) {
				this.detailsData(option.id);
				this.EditId = option.id
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
					// console.log(this.formData,'这里是编辑')
					// return
					AddPubEdit(this.formData).then((data) => {
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
				ClueDataDetails({
					id: id
				}).then((res) => {
					if (res.code == 0) {
						var data = res.data;
						if (data.HelperUsers) {
							data.HelperUsers.forEach((ite, inx) => {
								this.CollXieData.push({
									id: ite.Id,
									name: ite.RealName
								})
							})
						}

						//this.CollXieData=result.HelperUsers
						//console.log(this.CollXieData,'11111')
						this.formData = {
							realName: data.RealName,
							fromId: data.FromId, //表单id
							changeId: data.ChangeId, //转移的客户id
							mobile: data.Mobile,
							companyName: data.CompanyName,
							fromType: data.FromType,
							postName: data.PostName,
							deptName: data.DeptName,
							// userInfo: useList,
							helperName: data.HelperName,
							helper: data.Helper,
							id: data.Id,
							remark: data.Remark,
							leaderId: data.LeaderId
						};
						//console.log(data,this.formData,'this.formData')
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
					AddCluePool(this.formData).then((data) => {
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
				this.CollXieData = data;
			},
			handCollAvor() {
				uni.navigateTo({
					url: '/pages_Inventory/employee_select?multiple=true'
				})
			},
		}
	}
</script>

<style lang="less" scoped>
	page {
		background: #161A26;

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
</style>