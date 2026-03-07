<template>
	<view id="CreateCustomer">
		<top :isleftBack="true" leftIcon="icon-fanhui" 
			title="添加客户" backgroundColor="#fff" class="addPool-top"></top>
		<uni-forms ref="form" :modelValue="formData" label-position="top" class="addPool-form" label-width="100%"
			:rules="rules">
			<uni-forms-item label=" 客户名称" name="customerName" required class="addPool-form-item"
				>
				<input type="text" placeholder-class="PlaceStyle" v-model="formData.customerName" class="addPool-easyinput"
					placeholder="请输入客户名称">
			</uni-forms-item>
			<uni-forms-item label=" 客户编号" name="customerNumber" required class="addPool-form-item">
				<input style="color:#999999;border:2rpx solid #EAEAEA;" placeholder-class="PlaceStyle" disabled type="text" v-model="formData.customerNumber" class="addPool-easyinput"
					 placeholder="客户编号">
			</uni-forms-item>
			<uni-forms-item label=" 客户类型" name="customerType" required class="addPool-form-item"
				>
				<uni-data-select placeholder-class="PlaceStyle" :isDark="true" v-model="formData.customerType" :localdata="customerTypeData" type="line"
					placeholder="请选择客户类型" class="addPool-selected"></uni-data-select>
			</uni-forms-item>
			<uni-forms-item label="协作人" name="name" class="addPool-form-item" >
					<view class="Collaborator-item" @click="handCollAvor">
						<view  class="Collaborator-item-flex">
							<view v-if="CollXieData.length>0" style="display:flex;">
								<view v-for="(item,index) in CollXieData" :key="index" style="margin-right:10rpx;">
										{{index==0?item.name:','+item.name}}
								</view>
							</view>
							<view v-else style="color:#C1C1C1;font-size:32rpx;">
								请选择协作人
							</view>
							<view class="addPoolIcon iconfont icon-a-youjiantoubai">
								
							</view>
						</view>
					</view>
			</uni-forms-item>
			<uni-forms-item label="客户来源" name="fromType" class="addPool-form-item">
				<uni-data-select placeholder-class="PlaceStyle" :isDark="true" v-model="formData.fromType" :localdata="fromList" type="line"
					placeholder="请选择客户来源" class="addPool-selected"></uni-data-select>
			</uni-forms-item>
			<uni-forms-item label="公司网址" name="companyUrl" class="addPool-form-item">
				<input placeholder-class="PlaceStyle" type="text" v-model="formData.companyUrl" class="addPool-easyinput"
					placeholder="请输入公司网址">
			</uni-forms-item>
		
			<uni-forms-item label=" 公司电话" name="companyTel" class="addPool-form-item" >
				<input placeholder-class="PlaceStyle" type="text" class="addPool-easyinput" v-model="formData.companyTel"
					placeholder="请输入公司电话">
			</uni-forms-item>
			<uni-forms-item label="行业类型" name="industry" class="addPool-form-item">
				<uni-data-picker  class="addPool-selected"  v-model="formData.industry"   
				placeholder="请选择行业类型" popup-title="请选择" 
				:clear-icon="false" placeholder-class="IndustryType"  :localdata="ObtainInData" @change="onchange" @nodeclick="onnodeclick">
				</uni-data-picker>
			</uni-forms-item>
			<uni-forms-item label="客户详情" name="remark" class="addPool-form-item">
				<textarea placeholder-class="PlaceStyle" value="" v-model="formData.remark" placeholder="请输入客户详情"
					class="addPool-textarea" />
			</uni-forms-item>
		</uni-forms>
		<button v-if="EditId" :disabled="isSubmit" :style="{'opacity':isSubmit?0.6:1,'margin-top': '60rpx'}" class="addPool-button" @click="handEditForm" :loading="isSubmit">
			保存
		</button>
		<button :disabled="isSubmit" v-else :style="{'opacity':isSubmit?0.6:1,'margin-top': '60rpx'}" class="addPool-button" @click="handSubmitHigh" :loading="isSubmit">
			保存
		</button>
		<msg-prompt ref="promptMsg"></msg-prompt>
	</view>
</template>
<script>
	import {
		CustomerNumber,//生成客户编号
		IndustryType,//行业类型
		pubAddCustomer,//添加公海
		pubEditCustomer,//编辑公海
		DataPoolsDetails//客户详情
	} from "@/api/crmApi";
	import {
		setPagesParam
	} from '@/common/utillib.js'
	export default {
		data() {
			return {
				isSubmit:false,
				EditId:'',
				CollXieData:[],
				rules: {
					customerName: {
						rules: [{
							required: true,
							errorMessage: '客户名称不能为空'
						}]
					},
					customerType: {
						rules: [{
							required: true,
							errorMessage: '客户类型不能为空'
						}]
					}
				},
				ObtainInData:[],
				formData: {
					leaderId:0,
					BindOrgId:0,
					helperName:'',
					helper:'',
					industry:'',
					customerNumber: '', //客户编号
					customerName: "", //客户名称
					customerType: null, //客户类型：0为代理，1为直销（新增必填）
					fromType: "", //线索来源
					companyUrl: '', //公司网址
					companyTel: '', //公司电话
					remark: '', //客户详情
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
				customerTypeData: [{
						value: 0,
						text: '代理'
					},
					{
						value: 1,
						text: '直销'
					},
				]
			}
		},
		onLoad(option) {
			this.list() 
			if(option.id){
				this.detailsData(option.id);
				this.EditId=option.id
			}else{
				this.bainNumber();
			}
		},
		methods: {
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
				pubEditCustomer(this.formData).then((data)=>{
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
				DataPoolsDetails({id:id}).then((res)=>{
					if(res.code==0){
						var result=res.data;
						//console.log(result.HelperUsers,'result.HelperUsers')
						if(result.HelperUsers){
							result.HelperUsers.forEach((ite,inx)=>{
								this.CollXieData.push({
									id:ite.Id,
									name:ite.RealName
								})
							})
						}
						
						//this.CollXieData=result.HelperUsers
						
						//console.log(this.CollXieData,'11111')
						this.formData={
							leaderId:result.LeaderId,
							BindOrgId:result.BindOrgId,
							helperName:result.HelperName,
							helper:result.Helper,
							industry:result.Industry,
							customerNumber: result.CustomerNumber, //客户编号
							customerName: result.CustomerName, //客户名称
							customerType: result.CustomerType, //客户类型：0为代理，1为直销（新增必填）
							fromType:result.FromType, //线索来源
							companyUrl: result.CompanyUrl, //公司网址
							companyTel: result.CompanyTel, //公司电话
							remark: result.Remark, //客户详情
						}
						//console.log(this.formData,'this.formData')
					}
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
			onnodeclick(){
				
			},
			onchange(e) {
				this.formData.industry=e.detail.value[1].value
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
					
					//console.log(this.formData,'this.formData')
					pubAddCustomer(this.formData).then((data)=>{
						if(data.code==0){
							uni.showToast({
								title:'新建成功！',
								icon:'none'
							})
							setTimeout(()=>{
								setPagesParam('list')
							},1000)
						}
					})
				}).catch((err) => {
					//this.setMsgTop(err)
					//this.isSubmit=false;
				})
			},
			bainNumber(){
				//生成编号
				CustomerNumber().then((res) => {
					//console.log(res)
					this.formData.customerNumber = res.data;
				})
			},
			list() {
				//行业类型
			IndustryType().then((res)=>{
				if(res.code==0){
					//console.log(res,'获取行业列表')
					var arr=[]//第一级
					var children=[]//第二级
					for(var i=0;i<res.data.length;i++){
						if(res.data[i].ParentId==0){
							arr.push({value:res.data[i].Id,text:res.data[i].Name,ParentId:res.data[i].ParentId})
						}else{
							children.push({value:res.data[i].Id,text:res.data[i].Name,ParentId:res.data[i].ParentId})
						}
					}
					
					arr.forEach((item,index)=>{
						var str=[]
						children.forEach((v)=>{
							if(item.value==v.ParentId){
								//console.log(item.value,v.ParentId,'v.ParentId')
								str.push({
									value:v.value,
									text:v.text,
									ParentId:v.ParentId
								})
								//console.log(str,'str')
							}
						})
						item['children']=str;
					})
					this.ObtainInData=arr;
				}
				
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
	
		::v-deep.selected-list{
			font-size:32rpx!important;
		}
		::v-deep.placeholder{
			font-size:32rpx;
			color:#c1c1c1;
		}
		::v-deep.input-value-border{
			height:90rpx;
			border:1rpx solid rgba(255, 255, 255, .2);
		}
		.Collaborator-item-flex{
			color:#333;
			display: flex;
		}
		.Collaborator-item{
			color:#333;
			background: #F8F8F8;
			height:90rpx;
			border:1rpx solid rgba(255, 255, 255, .2);
			border-radius: 8rpx;
			line-height: 90rpx;
			padding:0rpx 14rpx;
			font-size:32rpx;
		}
		#CreateCustomer{
			background:#fff;
		}

</style>


