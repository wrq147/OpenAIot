<template>
	<view class="CompanyInfo">
		<top  leftIcon="icon-fanhui" :isleftBack="true" backgroundColor="#ffffff" title="企业信息" class="CRM-header"></top>
		<view class="createCompany-header" @click="uploadMul">
			<view v-if="!formData.logo2" class="image t-icon-qiyemorentupian1"></view>
			<img v-else class="imagesPrice" :src="formData.logo2+'?wh=500x500'" mode=""></img>
			<view class="Upload-logo">
				上传logo
			</view>
			
		</view>
		<!--企业信息-->
		<view class="CompanyInfo-Information">
			<uni-forms :modelValue="formData" ref="form" :rules="orgRules" label-position="top" class="addPool-form" label-width="100%">
				<uni-forms-item label=" 企业名称" name="orgName" class="addPool-form-item">
					<input  v-model="formData.orgName" type="text" class="addPool-easyinput" placeholder="请输入企业名称" placeholder-style="color:#C1C1C1;">
				</uni-forms-item>
				
				<uni-forms-item label="行业类型" name="industry"  class="addPool-form-item">
			<uni-data-picker  v-model="formData.industry"   placeholder="请选择行业类型" popup-title="请选择"  :clear-icon="false"  :localdata="ObtainInData" @change="onchange" @nodeclick="onnodeclick"></uni-data-picker>
				</uni-forms-item>
				
				<uni-forms-item label="预计使用规模" name="size"  class="addPool-form-item">
					<uni-data-select
					  :clear="false"
					      v-model="formData.size"
					      :localdata="comSizeItems"
						    type="line"
							placeholder="请选择规模"
							class="addPool-selected"
							@change="handSelectValue"
					    ></uni-data-select>
				</uni-forms-item>
				
				<uni-forms-item label=" 创建时间" name="createTime"  class="addPool-form-item">
					<input disabled type="text" v-model="formData.createTime" class="addPool-easyinput"  placeholder="创建时间">
				</uni-forms-item>
				<uni-forms-item label="更新时间" name="updateTime"  class="addPool-form-item">
					<input disabled type="text" v-model="formData.updateTime" class="addPool-easyinput"  placeholder="更新时间">
				</uni-forms-item>
				
				
				<uni-forms-item label="其它" name="size"  class="addPool-form-item">
					<!-- <view class="Others"> 
						<view>
							PSI set
						</view>
						<view class="iconfont icon-a-youjiantoubai">
							
						</view>
					</view> -->
					
					<view class="Others" @click="handExtended()">
						<view>
							扩展操作
						</view>
						<view class="iconfont icon-a-youjiantoubai">
							
						</view>
					</view>
						
				</uni-forms-item>
			</uni-forms>
			<button :disabled="isSubmit" class="CompanyInfoSave" @click="handSave()">
				保存
			</button>
			<view style="height:10rpx;"></view>
		</view>
		<msg-prompt ref="promptMsg"></msg-prompt>
	</view>
</template>

<script>
	import {
		uploadPhoto,
		delPhoto,
	} from '@/api/user.js'
	import {
	IndustryList,//获取行业列表
	provincesList,//省市区列表
	creatCompany,// 创建新企业

	EnterpriseDetails,//企业信息
	BusinessEditors//编辑企业信息
	} from "@/api/personalCenter";
	import {
		serverUrl
	} from '@/common/constVar.js'
	export default {
		data(){
			return{
				isSubmit:false,
				value:'',
				ObtainInData:[],
				value:'PSI set',
				value1:'Extended operation',
				range: [
				  { value: 0, text: "篮球" },
				  { value: 1, text: "足球" },
				  { value: 2, text: "游泳" },
				],
				formData:{
					Lng:0,
					Lat:0,
					Geo:'',
					AddressCode:'',
					AddressName:'',
					AddressDetail:'',
					orgName:'',
					logo2:'',
					createTime:'',
					updateTime:'',
					industry:'',
					size:null
				},
				orgRules: {
					orgName: {
						rules: [{
							required: true,
							errorMessage: "请输入公司"
						}]
					},
					industry: {
						rules: [{
							required: true,
							errorMessage: "请选择一个行业"
						}]
					},
					size: {
						rules: [{
							required: true,
							errorMessage: "请选择预期的使用规模"
						}]
					},
					logo: {
						rules: [{
							required: true,
							errorMessage: "请上传您的个人资料图片"
						}]
					},
				},
				comSizeItems: [],
				editId:'',
				
			}
		},
	
		onLoad(option) {
			
			this.dataInfo();//获取行业列表
			if(option.id){
				this.list(option.id);
				this.editId=option.id
			}
			
			//获取字典
			this.$store.dispatch("data/dictList", "org_size").then(rt => {
				this.comSizeItems = [];
				rt.forEach((it)=>{
					// console.log(it,'it')
					this.comSizeItems.push({text:it.label,value:it.value})
				})
				//console.log(this.comSizeItems,'this.comSizeItems')
			});
		},
		methods:{
			handExtended(){
				uni.navigateTo({
					url:'./comAtionSonPage?id='+this.editId
				})
			},
			handSave(){
				if (!this.isSubmit) {
				       this.isSubmit = true;
				       setTimeout(() => {
				         this.isSubmit = false;
				       }, 2000); // 设置 2 秒后可再次点击
				}
				//console.log(this.formData,'企业编辑')
				this.$refs.form.validate().then(data => {
				BusinessEditors(this.formData).then((res)=>{
					
					
					if(res.code==0){
						
						uni.showToast({
							icon:'none',
							title:'修改成功！'
						})
						setTimeout(()=>{
							uni.switchTab({
								url:'/pages/profile/profile'
							})
						},500)
						
					}
				})
				
				}).catch((err)=>{
					this.setMsgTop(e)
				})
			},
			handSelectValue(e){
				// console.log(e,'eeeeeee')
				this.formData.size=e;
			},
			onChangeSize(e) {
				this.comSizeName = e.label;
				this.comSizeValue = e;
				this.orgForm.size = e.value
				
			},
			onnodeclick(node){
			//	console.log(node)
				// this.listData.forEach((item,index)=>{
				// 		if(item.ParentId===node.value){
				// 		//	this.ObtainInData.push({children:[{value:item.Id,text:item.Name}]})
						
				// 		}
				// })
			//	console.log(this.ObtainInData,'this.ObtainInData')
			},
			  onchange(e) {
				//  console.log(e,'1111')
					//const value = e.detail.value
					//this.formData.industry=e
					this.formData.industry = e.detail.value[1].value
			      },
			//获取行业列表
			dataInfo(){
				IndustryList().then((res)=>{
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
						//console.log(arr,'add')
						//console.log(this.ObtainInData,'this.ObtainInData')
					}
					
				})
			},
			//选择地址
			uploadMul(type) {
				//手动上传图片
				uni.chooseImage({
					count: 1,
					sizeType: ['compressed'], //可以指定是原图还是压缩图，默认二者都有
					success: async (res) => {
						this.$refs.promptMsg.loadingOpen('上传中...')
						if (res.tempFilePaths.length == 0) {
							return;
						}
						for (let i = 0; i < res.tempFiles.length; i++) {
							if (res.tempFiles[i].size > 10 * 1024 * 1024) {
								this.$refs.promptMsg.open('上传图片不能超过10M', 2000)
								return;
							}
							try {
								let paths = res.tempFilePaths[i];
								let rsp = await uploadPhoto(paths)
								if (this.formData.logo2) {
									let rsp2 = await delPhoto(this.formData.logo2)
								}
								
								this.formData.logo = rsp
								this.formData.logo2 = rsp
								this.$refs.promptMsg.loadingColse()
								this.$forceUpdate()
							} catch (e) {
								//TODO handle the exception
								this.$refs.promptMsg.loadingColse()
								this.setMsgTop(e)
							}
						}
					}
				})
			},
			list(id){
				//企业信息
				EnterpriseDetails({id:id}).then((res)=>{
					if(res.code==0){
						var data=res.data;
						//this.OrgId=data.Creator.OrgId
						this.formData={
							Lng:0,
							Lat:0,
							Geo:'',
							AddressCode:'',
							AddressName:'',
							AddressDetail:'',
							orgName:data.OrgName,
							createTime:data.createTime,
							updateTime:data.updateTime,
							logo2:data.Logo,
							industry:data.Industry,
							size:JSON.stringify(data.Size),
							Id:this.editId
						}
						//this.$set(this.formData,'size',data.Size)
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
	
	.CompanyInfo{
		.Others{
			display:flex;
			height:88rpx;
			line-height: 88rpx;
			color:#333;
			padding:0rpx 20rpx;
			border-radius: 12rpx;
			background: #f8f8f8;
			margin-top:25rpx;
			font-size:32rpx;
			.icon-a-youjiantoubai{
				margin-left:auto;
				font-size:24rpx;
				
			}
		}
		.CompanyInfo-Information{
			width:94%;
			margin:0rpx 3%;
			margin-top:50rpx;
			.CompanyInfoSave{
				background: #2371FF;
				height: 100rpx;
				line-height: 100rpx;
				text-align: center;
				border-radius: 12rpx;
				color:#fff;
				font-size:32rpx;
				margin:50rpx 0rpx;
			}
			::v-deep.input-value-border{
				background: #F8F8F8!important;
				border:1rpx solid rgba(255, 255, 255, .2);
				border-radius: 12rpx;
				font-size:30rpx;
				height: 88rpx;
				color:#fff;
				padding:0rpx 20rpx;
				overflow-y: none;
			}
			.addPool-form-item{
				margin-top:20rpx;
				margin-bottom: 35rpx;
				.addPool-textarea{
					font-size:28rpx;
					border:1rpx solid rgba(255, 255, 255, .2);
					border-radius: 12rpx;
					padding:20rpx;
					height:120rpx;
					width:94%;
					color:#fff;
					margin-top:20rpx;
				}
				.addPool-selected{
					::v-deep.uni-select__input-text{
						color:#333!important;
						font-size:32rpx;
					}
					::v-deep.uni-select{
						height: 88rpx!important;
						background: #F8F8F8;
						border:none;
					}
					::v-deep.uni-select__input-placeholder{
						font-size:32rpx!important;
						color:rgba(255, 255, 255, .2)!important;
					}
				}
				::v-deep.xuanSelect{
					color:rgba(255, 255, 255, 1)!important;
				}
				::v-deep.uni-forms-item__label{
					height:50rpx;
					color:rgba(255, 255, 255, .7);
				}
				::v-deep.uni-forms-item{
					
				}
				
				.addPool-easyinput{
					background: #F8F8F8!important;
					border:1rpx solid rgba(255, 255, 255, .2);
					border-radius: 12rpx;
					font-size:32rpx;
					height: 88rpx;
					color:#333;
					padding:0rpx 20rpx;
				}
				
			}
		}
		.createCompany-header{
			margin-top:30rpx;
			.image,.imagesPrice{
				width: 160rpx;
				height: 160rpx;
				line-height: 160rpx;
				font-size:60rpx;
				margin:20rpx auto;
				margin-top:50rpx;
				text-align: center;
				border-radius: 4rpx;
			}
			.imagesPrice{
			display: flex;
			width: 180rpx;
			height: 180rpx;
			align-items: center;
			text-align: center;
			margin:0 auto;
			margin-top:50rpx;
			}
			.Upload-logo{
				width: 200rpx;
				font-size:28rpx;
				color:#2371FF;
				text-align: center;
				text-decoration:underline;
				margin:20rpx auto;
			}                                                                    
		}
	}
	
</style>

