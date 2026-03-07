<template>
	<view id="CreateCustomer">
		<top leftIcon="icon-fanhui" :isleftBack="true" backgroundColor="#ffffff" title="添加计划"
			class="addPool-top"></top>
		<uni-forms :modelValue="formData" ref="form" :rules="rules" label-position="top" class="addPool-form"
			label-width="100%">
			<uni-forms-item label="跟进客户" required name="customerId" id="customerId_form" labelFont="32rpx"
				contentFont="32rpx" :requireOpacity="0.5" class="addPool-form-item">
				<view id="customerId" class="form_li" @click="selectCustoms">
					<view class="form_input"
						:class="{'placeholder_input':!formData.customerName||formData.customerName==''}">
						{{formData.customerName?formData.customerName:'请选择跟进客户'}}
					</view>
					<view class="form_sel_icon">
						<custom-icons iconsName="icon-a-youjiantoubai" iconsSize="20rpx"
							iconsColor="#999999"></custom-icons>
					</view>
				</view>
			</uni-forms-item>
			<uni-forms-item required label="  计划执行人" name="executor" class="addPool-form-item">
				<view class="Collaborator-item" @click="handCollAvor" >
					<view class="Collaborator-item-flex">
						<view v-if="CollXieData.length>0" style="display: flex;">
							<view v-for="(item,index) in CollXieData" :key="index" style="margin-right:10rpx;">
								{{index==0?item.name:','+item.name}}
							</view>
						</view>
						<view v-else style="font-size:32rpx;color:#C1C1C1;">
							请选择计划执行人
						</view>
						<view class="addPoolIcon iconfont icon-a-youjiantoubai">
							
						</view>
					</view>
				</view>
			</uni-forms-item>
			<uni-forms-item required label="  计划时间" name="planTime" class="addPool-form-item">
				<uni-datetime-picker ref="dateChoice1" class="date" type="datetime" v-model="formData.planTime"
					placeholder-style="font-size:32rpx;color:#999999" :clear-icon="false"
					placeholder='请选择日期时间' :isCustom="true" >
				</uni-datetime-picker>
			</uni-forms-item>

			<uni-forms-item required label="  计划内容" name="remark" class="addPool-form-item">
				<textarea placeholder-class="PlaceStyle" value="" v-model="formData.remark" placeholder="请输入计划内容"
					class="addPool-textarea" />
			</uni-forms-item>
		</uni-forms>
		<button v-if="EditId" :disabled="isSubmit" :style="{'opacity':isSubmit?0.6:1,'margin-top': '60rpx'}" class="addPool-button" :loading="isSubmit" @click="handEditForm">
			保存
		</button>
		<button v-else :disabled="isSubmit" :style="{'opacity':isSubmit?0.6:1,'margin-top': '60rpx'}" class="addPool-button" :loading="isSubmit" @click="handSubmitHigh">
			保存
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
					customerId: '',
					customerName:''
				},
				rules: {
					planTime: {
						rules: [{
							required: true,
							errorMessage: '计划时间不能为空'
						}]
					},
					executor: {
						rules: [{
							required: true,
							errorMessage: '客户的跟进不能为空'
						}]
					},
					customerId: {
						rules: [{
							required: true,
							errorMessage: '请选择计划执行人'
						}]
					},
					remark: {
						rules: [{
							required: true,
							errorMessage: '计划内容不能为空'
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
							title: '修改成功！',
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
						//console.log(res,'详情数据')
						this.CollXieData=res.data.ExecutorUsers.map(row=>{
							let obj={
								id:row.Id,
								name:row.RealName,
								avatar:row.Avatar
							}
							return obj
						})
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
								title: '添加成功！',
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
				// console.log(data,'data');
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
		}
	}
</script>
<style>
	page{
		background: #ffffff;
	}
</style>
<style lang="less" scoped>
	::v-deep.cus_picker .uni-date-x{
		color:#333!important;
	}
		
		::v-deep .dark_picker .uni-date-x .uni-date__x-input{
			color:#333333 !important;
		}
		::v-deep.uni-date__x-input{font-size:32rpx!important;}
		.Collaborator-item-flex {
			color: #333;
			display: flex;
		}

		::v-deep.uni-date-x--border{
			height: 90rpx!important;
			border: none!important;
		}

		.Collaborator-item {
			height: 90rpx;
			background: #f8f8f8;
			border-radius: 8rpx;
			line-height: 90rpx;
			padding: 0rpx 20rpx;
			font-size:32rpx;
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