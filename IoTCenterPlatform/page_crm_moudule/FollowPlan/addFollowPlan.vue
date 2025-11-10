<template>
	<view id="CreateCustomer">
		<top leftIcon="icon-fanhui" :isleftBack="true" leftText="Back" backgroundColor="#161A26" title="Create plan"
			class="addPool-top"></top>
		<uni-forms :modelValue="formData" ref="form" :rules="rules" label-position="top" class="addPool-form"
			label-width="100%">

			<uni-forms-item required label=" Objective" name="customerId" class="addPool-form-item">
				<uni-data-select :isDark="true" v-model="formData.customerId" :localdata="periodArr" type="line"
					placeholder="Please select a customer" class="addPool-selected"
					@change="handClickPeriod"></uni-data-select>
			</uni-forms-item>
			<uni-forms-item required label=" Plan executor" name="executor" class="addPool-form-item">
				<view class="Collaborator-item" @click="handCollAvor">
					<view class="Collaborator-item-flex">
						<view v-if="CollXieData.length>0" style="display: flex;">
							<view v-for="(item,index) in CollXieData" :key="index" style="margin-right:10rpx;">
								{{item.name?item.name:item.RealName}}
							</view>
						</view>
						<text v-else style="color:rgba(255, 255, 255, .2);font-size:32rpx;">
							Please select a Customer name
						</text>
						<view class="addPoolIcon iconfont icon-a-youjiantoubai">
							
						</view>
					</view>
				</view>
			</uni-forms-item>
			<uni-forms-item required label=" Scheduled time" name="planTime" class="addPool-form-item">
				<uni-datetime-picker ref="dateChoice1" class="date" type="datetime" v-model="formData.planTime"
					placeholder-style="font-size:32rpx;color:#999999" :clear-icon="false"
					placeholder='Please select the outbound time' :isCustom="true" :isDark="true">
				</uni-datetime-picker>
			</uni-forms-item>

			<uni-forms-item required label=" Plan content" name="remark" class="addPool-form-item">
				<textarea placeholder-class="PlaceStyle" value="" v-model="formData.remark" placeholder="Please enter the plan content"
					class="addPool-textarea" />
			</uni-forms-item>
		</uni-forms>
		<button v-if="EditId" :disabled="isSubmit" :style="{'opacity':isSubmit?0.6:1,'margin-top': '60rpx'}" :loading="isSubmit" class="addPool-button" @click="handEditForm">
			Save
		</button>
		<button v-else :disabled="isSubmit" :style="{'opacity':isSubmit?0.6:1,'margin-top': '60rpx'}" :loading="isSubmit" class="addPool-button" @click="handSubmitHigh">
			Save
		</button>
		<msg-prompt ref="promptMsg"></msg-prompt>
	</view>
</template>

<script>
	import {
		CustomerList, //客户接口
		PlanAdd,
		PlanDetails ,//跟进详情
		PlanEdit//跟进编辑
	} from "@/api/crmApi";
	import {
		yuanAarngemnt,
	} from "@/api/personalCenter";
	import {
		setPagesParam
	} from '@/common/utillib.js'
	var dayjs = require('@/common/day.js')
	export default {
		data() {
			return {
				isSubmit:false,
				EditId:'',
				periodArr: [],
				formData: {
					remark: '',
					planTime: '',
					executor: '',
					customerId: ''
				},
				rules: {
					planTime: {
						rules: [{
							required: true,
							errorMessage: 'The planned time cannot be empty'
						}]
					},
					executor: {
						rules: [{
							required: true,
							errorMessage: 'Follow up with customers cannot be empty'
						}]
					},
					customerId: {
						rules: [{
							required: true,
							errorMessage: 'Please select the plan executor'
						}]
					},
					remark: {
						rules: [{
							required: true,
							errorMessage: 'The plan content cannot be empty'
						}]
					},

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
				CollXieData: [],
			}
		},
		onLoad(option) {
			this.list(); //预加载数据
			if (option.id) {
				this.detailsData(option.id);
				this.EditId=option.id
			}else{
				this.formData.planTime=dayjs(new Date()).format('YYYY-MM-DD HH:mm:ss')
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
				this.formData.id = this.EditId
				this.formData.executor = helper.join(',');
				// console.log(this.formData,'这里是编辑')
				// return
				PlanEdit(this.formData).then((data) => {
					if (data.code == 0) {
						uni.showToast({
							title: 'Modified successfully！',
							icon: 'none'
						})
						setTimeout(() => {
							setPagesParam('list',this.EditId)
						}, 1000)
					}
				}).catch((err) => {
					this.setMsgTop(err)
				})
				})
			},
			detailsData(id) {
				PlanDetails({
					id: id
				}).then((res) => {
					if (res.code == 0) {
						var data = res.data;
						this.CollXieData=res.data.ExecutorUsers.map(row=>{
							let obj={
								id:row.Id,
								name:row.RealName,
								avatar:row.Avatar
							}
							return obj
						})
						//console.log(res,'详情数据')
						this.formData = {
							remark: data.Remark,
							planTime: data.PlanTime,
							executor: data.Executor,
							customerId: data.CustomerId,
							customerName: data.CustomerName,
							id:data.Id
						}
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
				var helperName = []
				var helper = []
				this.CollXieData.forEach((row) => {
					helperName.push(row.name)
					helper.push(row.id)
				})
				//this.formData.helperName = helperName.join(',');
				this.formData.executor = helper.join(',');
				this.$refs.form.validate().then((res) => {
					PlanAdd(this.formData).then((data) => {
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
			list() {
				CustomerList({
					pageNum: 1,
					pageSize: 100
				}).then((res) => {
					//console.log(res,'客户')
					res.data.List.forEach((row) => {
						this.periodArr.push({
							value: row.Id,
							text: row.CustomerName,
						})
					})
				})
			},
			handClickPeriod(e) {
				this.formData.customerId = e
			},
		}
	}
</script>

<style lang="less" scoped>
	page {
		background: #161A26;
		/deep/.uni-select__input-text{
			font-size:32rpx;
		}
		/deep/.uni-forms-item__label{
			font-size:32rpx!important;
		}
		/deep/.uni-textarea-textarea{
			font-size:32rpx;
		}
		/deep/.dark_picker .uni-date-x .uni-date__x-input{
			color:#fff!important;
			font-size:32rpx!important;
		}
		.Collaborator-item-flex {
			color: #fff;
			display: flex;
			font-size:32rpx;
		}

		/deep/.dark_picker .uni-date-x--border {
			height: 90rpx !important;
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

	.addPool-top {
		margin-bottom: 35rpx;
	}
	.addPoolIcon{
		font-size:.6rem;
		margin-left: auto;
		color:rgb(153, 153, 153);
	}
</style>