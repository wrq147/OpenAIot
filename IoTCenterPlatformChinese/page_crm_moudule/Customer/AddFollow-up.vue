<template>
	<view id="CreateCustomer">
		<top :isleftBack="true" leftIcon="icon-fanhui" backgroundColor="#ffffff" title="添加跟进"
			class="CRM-header">
		</top>
		<uni-forms ref="form" :rules="rules" :modelValue="formData" label-position="top" class="addPool-form"
			label-width="100%">
			<uni-forms-item required label="  跟进内容" name="remark" class="addPool-form-item">
				<textarea v-model="formData.remark" placeholder="请输入内容" class="addPool-textarea" />
			</uni-forms-item>
			<uni-forms-item required label="   跟进动态" name="followTime"
				class="addPool-form-item">
				<uni-datetime-picker ref="dateChoice1" class="date" type="datetime" v-model="formData.followTime"
					placeholder-style="font-size:32rpx;color:#999999" :clear-icon="false"
					placeholder='请输入跟进动态' :isCustom="true" >
				</uni-datetime-picker>
			</uni-forms-item>
			<uni-forms-item required label="   跟进方式" name="followWay" class="addPool-form-item">
				<uni-data-select :isDark="true" v-model="formData.followWay" :localdata="ExecutionMode" type="line"
					placeholder="请选择跟进方式" class="addPool-selected"></uni-data-select>
			</uni-forms-item>
			<uni-forms-item required label=" 跟进人" name="followUser" class="addPool-form-item">
				<view class="Collaborator-item" @click="handManager">
					<view class="Collaborator-item-flex">
						<view v-if="ManagerList.length>0">
							<view v-for="(item,index) in ManagerList" :key="index" style="margin-right:10rpx;font-size:32rpx;">
								{{item.name}}
							</view>
						</view>
						<text v-else style="color:#C1C1C1;font-size:32rpx;">
							请选择跟进人
						</text>
					</view>
				</view>
			</uni-forms-item>
		</uni-forms>
		<view class="handConfirm" @click="handConfirmButton">
			确认
		</view>
	</view>
</template>

<script>
	import {
		FastTracking,//快速跟进提交
	} from "@/api/crmApi";
	import {
		setPagesParam
	} from '@/common/utillib.js'
		var dayjs = require('@/common/day.js')
	export default {
		data() {
			return {
				formData: {
					followTime: '',
					followUser: '',
					followWay: '',
					remark: '',
					targetId: '',
					targetType: 0
				},
				ManagerList: [],
				rules: {
					remark: {
						rules: [{
							required: true,
							errorMessage: '请输入内容'
						}]
					},
					followTime: {
						rules: [{
							required: true,
							errorMessage: '请选择随访时间'
						}]
					},
					followWay: {
						rules: [{
							required: true,
							errorMessage: '请选择后续方法'
						}]
					},
					followUser: {
						rules: [{
							required: true,
							errorMessage: '请选择跟进人员'
						}]
					},
				},
				ExecutionMode: [],
			}
		},
		onLoad(option) {
			if(option.id){
				this.list();
				this.formData.targetId=option.id
				this.formData.followTime=dayjs(new Date()).format('YYYY-MM-DD HH:mm:ss')
			}
			if(option.TargetType){
				this.formData.targetType=option.TargetType
			}
		},
		methods: {
			handConfirmButton() {
				this.$refs.form.validate().then((res) => {
					// console.log(this.formData,'this.formData')
					// return
					FastTracking(this.formData).then((res) => {
						if (res.code == 0) {
							uni.showToast({
								title:'提交成功！',
								icon:'none'
							})
							setTimeout(()=>{
								setPagesParam('listData',this.formData.targetId)
							},500)
						}
					})
				})
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
			selectEmplee(data) {
				this.ManagerList = data;
				this.formData.followUser=data[0].id
			},
			handManager() {
				uni.navigateTo({
					url: '/pages_flow/inventory/employee_select?type=user'
				})
			},
		}
	}
</script>
<style>
	page {
		background: #ffffff;
	}
</style>
<style lang="less" scoped>
	::v-deep.uni-date__x-input{
		font-size:32rpx;
		color:#333333;
	}
	::v-deep.uni-textarea-placeholder{
		color:#C1C1C1;
		font-size:32rpx;
	}
	::v-deep.uni-date-x{
		color:#333;
	}
	.handConfirm {
		width: 94%;
		margin: 0rpx 3%;
		height: 100rpx;
		line-height: 100rpx;
		text-align: center;
		color: #fff;
		background: #2371FF;
		border-radius: 10rpx;
		font-size: 32rpx;
	}

	.Collaborator-item-flex {
		color: #fff;
	}

	::v-deep.uni-date-x--border {
		height: 90rpx !important;
		border:none!important;
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
		background: #f8f8f8;
		font-size:32rpx;
		::v-deep.left {}
		.Collaborator-item-flex{
			color:#333;
		}
		.left.uni-stat__select {
			flex: .3;
			width: 200rpx !important;
			text-align: center;

			::v-deep.uni-select__input-text {
				margin-right: 22rpx !important;
			}
		}

		::v-deep.uni-select {
			border: none !important;
		}
	}

	// ::v-deep.uni-navbar__header-btns-left{
	// 	width: 125rpx!important;
	// }
</style>