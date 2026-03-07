<template>
	<view id="CreateCustomer">
		<top leftIcon="icon-fanhui" :isleftBack="true" backgroundColor="#ffffff" title="添加线索" class="addPool-top"></top>
		<uni-forms ref="form" :modelValue="formData" label-position="top" class="addPool-form" label-width="100%" :rules="rules">
			<uni-forms-item label=" 客户名称" name="companyName" required class="addPool-form-item" >
				<input placeholder-class="PlaceStyle" type="text" v-model="formData.companyName" class="addPool-easyinput" placeholder="请输入客户名称">
			</uni-forms-item>
		
		<uni-forms-item label="  联系人" name="realName" required class="addPool-form-item" >
			<input placeholder-class="PlaceStyle" type="text" v-model="formData.realName" class="addPool-easyinput" placeholder="请输入联系人">
		</uni-forms-item>
		<uni-forms-item label=" 手机号" required name="mobile"  class="addPool-form-item">
		<input placeholder-class="PlaceStyle" maxlength="11" type="text" v-model="formData.mobile" class="addPool-easyinput" placeholder="请输入手机号">
		</uni-forms-item>
			<uni-forms-item label="部门" name="deptName"  class="addPool-form-item">
				<input placeholder-class="PlaceStyle" type="text" v-model="formData.deptName" class="addPool-easyinput" placeholder="请输入部门">
			</uni-forms-item>
			<uni-forms-item label="职务" name="postName"  class="addPool-form-item">
				<input placeholder-class="PlaceStyle" type="text" v-model="formData.postName" class="addPool-easyinput" placeholder="请输入职务">
			</uni-forms-item>
			<uni-forms-item label="线索来源" name="fromType"  class="addPool-form-item">
				<uni-data-select :isDark="true" v-model="formData.fromType" :localdata="fromList" type="line"
					placeholder="请选择线索来源" class="addPool-selected"></uni-data-select>
			</uni-forms-item>
		<!-- 	<uni-forms-item label=" Company website" name="name"  class="addPool-form-item" style="color:red;">
				<input  type="text" class="addPool-easyinput" placeholder="Please enter company website">
			</uni-forms-item> -->
			<uni-forms-item label="协作人" name="name"  class="addPool-form-item" style="color:red;">
				<view class="Collaborator-item" @click="handCollAvor">
					<view  class="Collaborator-item-flex">
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
		<uni-forms-item label="线索详情" name="remark"  class="addPool-form-item">
		 <textarea placeholder-class="PlaceStyle" value="" v-model="formData.remark" placeholder="请输入线索详情" class="addPool-textarea"/>
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
		AddCluePool,//新增
		ClueDataDetails,//详情
		AddPubEdit//编辑
	} from "@/api/crmApi";
	import {
		setPagesParam
	} from '@/common/utillib.js'
	export default {
		data(){
			return{
				isSubmit:false,
				rules: {
					companyName: {
						rules: [{
							required: true,
							errorMessage: '请输入客户名称'
						}]
					},
					realName: {
						rules: [{
							required: true,
							errorMessage: '请输入联系人名称'
						}]
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
							}]
					}
				},
				EditId:'',
				CollXieData:[],
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
						text: "微信线索"
					},
					{
						value: "form",
						text: "流程表单"
					},
					{
						value: "other",
						text: "其他"
					}
				],
			}
		},
		onLoad(option) {
			if(option.id){
				this.detailsData(option.id);
				this.EditId=option.id
			}
		},
		methods:{
			handEditForm(){
				if (!this.isSubmit) {
				       this.isSubmit = true;
				       setTimeout(() => {
				         this.isSubmit = false;
				       }, 2000); // 设置 2 秒后可再次点击
				}
				this.$refs.form.validate().then((res) => {
				var helperName=[]
				var helper=[]
				this.CollXieData.forEach((row)=>{
					helperName.push(row.name)
					helper.push(row.id)
				})
				this.formData.helperName=helperName.join(',');
				this.formData.id=this.EditId
				this.formData.helper=helper.join(',');
				// console.log(this.formData,'这里是编辑')
				// return
				AddPubEdit(this.formData).then((data)=>{
					if(data.code==0){
						uni.showToast({
							title:'修改成功！',
							icon:'none'
						})
						setTimeout(()=>{
							setPagesParam('list',this.EditId)
						},1000)
					}
				}).catch((err) => {
					this.setMsgTop(err)
				})
				})
			},
			detailsData(id){
				ClueDataDetails({id:id}).then((res)=>{
					if(res.code==0){
						var data=res.data;
						if(data.HelperUsers){
							data.HelperUsers.forEach((ite,inx)=>{
								this.CollXieData.push({
									id:ite.Id,
									name:ite.RealName
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
					var helperName=[]
					var helper=[]
					this.CollXieData.forEach((row)=>{
						helperName.push(row.name)
						helper.push(row.id)
					})
					this.formData.helperName=helperName.join(',');
					this.formData.helper=helper.join(',');
					AddCluePool(this.formData).then((data)=>{
						if(data.code==0){
							uni.showToast({
								title:'新建成功！',
								icon:'none'
							})
							setTimeout(()=>{
								setPagesParam('list')
							},1000)
						}
					}).catch((err) => {
					this.setMsgTop(err)
				})
				})
			
			},
			selectEmplee(data){
				//console.log(data,'data');
				this.CollXieData=data;
			},
			handCollAvor(){
				uni.navigateTo({
					url:'/pages_flow/inventory/employee_select?multiple=true'
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
	
		
		.Collaborator-item-flex{
			color:#333;
			display: flex;
		}
		.Collaborator-item{
			height: 90rpx;
			background: #f8f8f8;
			border-radius: 8rpx;
			line-height: 90rpx;
			padding: 0rpx 20rpx;
			font-size:32rpx;
		}
	
</style>