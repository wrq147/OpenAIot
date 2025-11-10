<template>
	<view id="CreateCustomer">
		<top :isleftBack="true" leftIcon="icon-fanhui" leftText="Back" backgroundColor="#161A26" title="Follow-up"
			class="CRM-header">
		</top>
		<uni-forms ref="form" :rules="rules" :modelValue="formData" label-position="top" class="addPool-form"
			label-width="100%">
			<uni-forms-item required label=" Follow up content" name="remark" class="addPool-form-item">
				<textarea placeholder-style="font-size:32rpx;color:rgba(255,255,255,.2);" v-model="formData.remark" placeholder="Please enter content" class="addPool-textarea" />
			</uni-forms-item>
			<uni-forms-item v-model="formData.followTime" required label="  Follow up time" name="followTime"
				class="addPool-form-item">
				<uni-datetime-picker ref="dateChoice1" class="date" type="datetime" v-model="formData.followTime"
					placeholder-style="font-size:32rpx;color:rgba(255,255,255,.2);" :clear-icon="false"
					placeholder='Please select the Follow up time' :isCustom="true" :isDark="true">
				</uni-datetime-picker>
			</uni-forms-item>
			<uni-forms-item required label="  Follow up method" name="followWay" class="addPool-form-item">
				<uni-data-select :isDark="true" v-model="formData.followWay" :localdata="ExecutionMode" type="line"
					placeholder="Please select a  Follow up method" class="addPool-selected"></uni-data-select>
			</uni-forms-item>
			<uni-forms-item required label="Follow-up person" name="followUser" class="addPool-form-item">
				<view class="Collaborator-item" @click="handManager">
					<view class="Collaborator-item-flex">
						<view v-if="ManagerList.length>0">
							<view v-for="(item,index) in ManagerList" :key="index" style="margin-right:10rpx;">
								{{item.name}}
							</view>
						</view>
						<text v-else style="color:rgba(255, 255, 255, .2);font-size:32rpx;">
							Please select a Follow up person
						</text>
						<view class="addPoolIcon iconfont icon-a-youjiantoubai">
							
						</view>
					</view>
				</view>
			</uni-forms-item>
		</uni-forms>
		<view class="handConfirm" @click="handConfirmButton">
			Confirm
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
							errorMessage: 'Please enter content'
						}]
					},
					followTime: {
						rules: [{
							required: true,
							errorMessage: 'Please select the Follow up time'
						}]
					},
					followWay: {
						rules: [{
							required: true,
							errorMessage: 'Please select a  Follow up method'
						}]
					},
					followUser: {
						rules: [{
							required: true,
							errorMessage: 'Please select  Follow-up person'
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
								title:'Submitted successfully！',
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
					url: '/pages_Inventory/employee_select?type=user'
				})
			},
		}
	}
</script>

<style lang="less" scoped>
	page {
		background: #161A26;
	}
	/deep/.uni-select__input-text{
		font-size:32rpx;
	}
	/deep/.uni-forms-item__label{
		font-size:32rpx!important;
	}
	/deep/.uni-textarea-textarea{
		font-size:32rpx;
	}
	/deep/.uni-date__x-input{
		font-size:32rpx!important;
	}
	.handConfirm {
		width: 94%;
		margin: 0rpx 3%;
		height: 100rpx;
		line-height: 100rpx;
		text-align: center;
		color: #fff;
		background: linear-gradient(180deg, #FF3535 0%, #FF613D 100%);
		border-radius: 10rpx;
		font-size: 36rpx;
	}

	.Collaborator-item-flex {
		color: #fff;
		display: flex;
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
		font-size:32rpx;
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
	.addPoolIcon{
		font-size:.6rem;
		margin-left: auto;
		color:rgb(153, 153, 153);
	}
	// /deep/.uni-navbar__header-btns-left{
	// 	width: 125rpx!important;
	// }
</style>