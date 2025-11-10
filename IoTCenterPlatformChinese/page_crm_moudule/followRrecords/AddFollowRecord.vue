<template>
	<view id="CreateCustomer">
		<top leftIcon="icon-fanhui" :isleftBack="true" backgroundColor="#ffffff"
			title="添加跟进记录" class="addPool-top"></top>
		<uni-forms ref="form" :rules="rules" :modelValue="formData" label-position="top" class="addPool-form"
			label-width="100%">
			<uni-forms-item required label="  跟进内容" name="remark" class="addPool-form-item">
				<textarea placeholder-class="PlaceStyle" v-model="formData.remark" placeholder="请输入跟进内容"
					class="addPool-textarea" />
			</uni-forms-item>
			<uni-forms-item required label="  跟进目标" name="targetId" class="addPool-form-item">
				<view class="Collaborator-item">
					<uni-data-select placeholder-class="PlaceStyle" :clear="false"  v-model="formData.targetType" :localdata="range"
						type="line" placeholder="" class="left" @change="handClickSwitch"></uni-data-select>
					<uni-data-select :isDark="true" placeholder-class="PlaceStyle" v-model="formData.targetId" :localdata="periodArr" type="line"
						placeholder="请选择跟进目标" class="addPool-selected Collaborator-item-select"
						@change="handClickPeriod"></uni-data-select>
				</view>
			</uni-forms-item>
			<uni-forms-item required label="  跟进时间" name="followTime" class="addPool-form-item">
				<uni-datetime-picker ref="dateChoice1" class="date" type="datetime" v-model="formData.followTime"
					placeholder-style="font-size:32rpx;color:#999999" :clear-icon="false"
					placeholder='请选择日期时间' :isCustom="true" >
				</uni-datetime-picker>
			</uni-forms-item>
			<uni-forms-item required label="  跟进方式" name="followWay" class="addPool-form-item">
				<uni-data-select :isDark="true" placeholder-class="PlaceStyle" v-model="formData.followWay" :localdata="ExecutionMode" type="line"
					placeholder="请选择跟进方式" class="addPool-selected"></uni-data-select>
			</uni-forms-item>
	
			<uni-forms-item label="跟进人" name="followUser" class="addPool-form-item" >
					<view class="Collaborator-item" style="display: flex;" @click="handPerson">
							<view class="addPool-easyinput" style="display: flex;">
								<view v-if="formData.followUser">
									{{personText}}	
								</view>	
								<view v-else style="color:#C1C1C1;font-size:32rpx;">
									请选择跟进人
								</view>
								<view class="addPoolIcon iconfont icon-a-youjiantoubai">
								</view>
							</view>
					</view>
			</uni-forms-item>
			<uni-forms-item label="商机" name="opportId" class="addPool-form-item" >
					<view class="Collaborator-item" style="display: flex;" @click="handBusiness">
							<view class="addPool-easyinput" style="display: flex;">
								<view v-if="formData.opportId">
									{{opportNameText}}	
								</view>	
								<view v-else style="color:#C1C1C1;font-size:32rpx;">
									请选择商机
								</view>
								<view class="addPoolIcon iconfont icon-a-youjiantoubai">
								</view>
							</view>
					</view>
			</uni-forms-item>
		
			
			<uni-forms-item label="联系人" name="contactId" class="addPool-form-item" >
					<view class="Collaborator-item" style="display: flex;" @click="handContact">
							<view class="addPool-easyinput" style="display: flex;">
								<view v-if="formData.contactId">
									{{contentText}}	
								</view>	
								<view v-else style="color:#C1C1C1;font-size:32rpx;">
									请选择联系人
								</view>
								<view class="addPoolIcon iconfont icon-a-youjiantoubai">
								</view>
							</view>
					</view>
			</uni-forms-item>
		</uni-forms>
		<button v-if="EditId"  :disabled="isSubmit" :style="{'opacity':isSubmit?0.6:1,'margin-top': '60rpx'}" class="addPool-button" :loading="isSubmit" @click="handEditForm">
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
				isSubmit:false,
				rules: {
					remark: {
						rules: [{
							required: true,
							errorMessage: '请输入后续内容'
						}]
					},
					targetId: {
						rules: [{
							required: true,
							errorMessage: '请选择目标'
						}]
					},
					followTime: {
						rules: [{
							required: true,
							errorMessage: '请选择执行时间'
						}]
					},
					followWay: {
						rules: [{
							required: true,
							errorMessage: '请选择执行模式'
						}]
					},
					followUser: {
						rules: [{
							required: true,
							errorMessage: '请选择计划执行人'
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
						text: "客户"
					},
					{
						value: 1,
						text: "线索"
					}
				],
				ExecutionMode: [], //跟进方式 Execution mode
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
			}else{
				this.formData.followTime=dayjs(new Date()).format('YYYY-MM-DD HH:mm:ss')
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
					url: '/pages_flow/inventory/employee_select?type=user'
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
				flowDataRecordDetails({
					id: id
				}).then((res) => {
					if (res.code == 0) {
						var data = res.data;
						//console.log(data, '详情数据')
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
<style>
	page {
		background: #ffffff;
	}
</style>
<style lang="less" scoped>
	.addPool-easyinput{
		width: 100%;
		padding:0rpx 12rpx!important;
	}
	::v-deep.cus_picker .uni-date-x{
		color:#333!important;
	}
	::v-deep.uni-select__selector{
		color:#333!important;
	}
::v-deep.dark_picker .uni-date-x .uni-date__x-input{
			//color:rgba(255, 255, 255, .2)!important;
		}
		::v-deep.uni-date__x-input{
			font-size:32rpx!important;
		}
	::v-deep.uni-date-x--border {
		height: 90rpx !important;
		border:none!important
	}

	.addPool-top {
		margin-bottom: 35rpx;
	}

	.Collaborator-item {
		color: rgba(255, 255, 255, .4);
		height: 90rpx;
		border: 1rpx solid rgba(255, 255, 255, .2);
		border-radius: 12rpx;
		line-height: 90rpx;
		display: flex;

		::v-deep.left {}

		.left.uni-stat__select {
			flex: .3;
			width: 200rpx !important;
			text-align: center;
			background: #F8F8F8;
			
			::v-deep.uni-select__input-text {
				margin-right: 22rpx !important;
				color:#999999;
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