<!--切换公司——新建企业-->
<template>
	<view class="createCompany">
		<top leftIcon="icon-fanhui" :isleftBack="true" leftText="Back" backgroundColor="#161A26" title="Create company" class="CRM-header"></top>
		<view class="createCompany-header" @click="uploadMul">
			<!-- <view class="iconfont icon-qiyemorentupian">
				
			</view> -->
			<view v-if="!formData.logo2" class="image t-icon-qiyemorentupian"></view>
			<image v-else class="imagesPrice" :src="formData.logo2+'?wh=500x500'" mode=""></image>
			<view class="Upload-logo">
				Upload logo
			</view>
		</view>
		
		<uni-forms :modelValue="formData" ref="form" :rules="orgRules" label-position="top" class="addPool-form" label-width="100%">
			<uni-forms-item label=" Customer name" name="orgName" class="addPool-form-item">
				<input  v-model="formData.orgName" type="text" class="addPool-easyinput" placeholder="Please enter the company name" placeholder-style="color:rgba(255, 255, 255, .2);">
			</uni-forms-item>
			
			<uni-forms-item label="Industry type" name="industry"  class="addPool-form-item">
					<uni-data-picker placeholder="Please select Industry type" popup-title="Please select" :isDark="true" :clear-icon="false"  :localdata="ObtainInData" @change="onchange" @nodeclick="onnodeclick"></uni-data-picker>
			</uni-forms-item>
			
			<uni-forms-item label="Expected usage scale" name="size"  class="addPool-form-item">
				<uni-data-select
				  :clear="false"
				  :isDark="true"
				  :isCustom="true"
				      v-model="value"
				      :localdata="comSizeItems"
					    type="line"
						placeholder="Please select scale"
						class="addPool-selected"
						style="color:rgba(255, 255, 255, .2)"
						@change="handSelectValue"
				    ></uni-data-select>
			</uni-forms-item>
			
			<!--<uni-forms-item label="Company address" name="name"  class="addPool-form-item">
				 <uni-data-select
				      v-model="value"
				      :localdata="range"
					    type="line"
						placeholder="Please select address"
						class="addPool-selected"
				    ></uni-data-select> 
					<uni-data-picker  :localdata="ObtainInData" popup-title="请选择班级" @change="onchange" @nodeclick="onnodeclick"></uni-data-picker>
					<textarea placeholder-style="color:rgba(255, 255, 255, .2);" value="" placeholder="Please enter the customer details" class="addPool-textarea"/>
			</uni-forms-item>-->
		</uni-forms>
		<button :disabled="isSubmit" class="createCompany-Complete" @click="handCreate">
			Complete
		</button>
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
	creatCompany// 创建新企业
	} from "@/api/personalCenter";
	export default {
		data(){
			return{
				isSubmit:false,
				orgRules: {
					orgName: {
						rules: [{
							required: true,
							errorMessage: "Please enter the company"
						}]
					},
					industry: {
						rules: [{
							required: true,
							errorMessage: "Please select an industry"
						}]
					},
					size: {
						rules: [{
							required: true,
							errorMessage: "Please select the expected usage scale"
						}]
					},
					logo: {
						rules: [{
							required: true,
							errorMessage: "Please upload your profile picture"
						}]
					},
				},
				dataInfo:{
				},
				comSizeValue: {},
				comSizeItems: [],
				orgForm: {
					size: null,
				},
				formData:{
					Lng:0,
					Lat:0,
					Geo:'',
					AddressCode:'',
					AddressName:'',
					AddressDetail:'',
					logo2:'',
					orgName:'',
					industry:'',
					size:'',
					intro: ""
				},
				range: [
				  { value: 0, text: "篮球" },
				  { value: 1, text: "足球" },
				  { value: 2, text: "游泳" },
				],
				value: '',
				ObtainInData:[],
				listData:[]
			}
		},
		onLoad() {
			this.list();//获取行业列表
			this.cityData()//省市区列表
			//获取字典
			this.$store.dispatch("data/dictList", "org_size").then(rt => {
				this.comSizeItems = [];
				rt.forEach((it)=>{
					console.log(it,'it')
					this.comSizeItems.push({text:it.label,value:it.value})
				})
				//console.log(this.comSizeItems,'this.comSizeItems')
			});
		},
		methods:{
			//选择地址
			uploadMul(type) {
				//手动上传图片
				uni.chooseImage({
					count: 1,
					sizeType: ['compressed'], //可以指定是原图还是压缩图，默认二者都有
					success: async (res) => {
						console.log("上传图片", res);
						this.$refs.promptMsg.loadingOpen('Uploading...')
						if (res.tempFilePaths.length == 0) {
							return;
						}
						for (let i = 0; i < res.tempFiles.length; i++) {
							if (res.tempFiles[i].size > 10 * 1024 * 1024) {
								this.$refs.promptMsg.open('Uploading images cannot exceed10M', 2000)
								return;
							}
							try {
								let paths = res.tempFilePaths[i];
								console.log(paths, 'pathspaths');
								let rsp = await uploadPhoto(paths)
								if (this.formData.logo2) {
									let rsp2 = await delPhoto(this.formData.logo2)
									console.log("删除图片", rsp2);
								}
								
								this.formData.logo = rsp
								this.formData.logo2 = rsp
								console.log("图片111111111", this.formData.logo2);
								this.$refs.promptMsg.loadingColse()
								this.$forceUpdate()
							} catch (e) {
								//TODO handle the exception
								this.$refs.promptMsg.loadingColse()
								console.log(e);
								this.setMsgTop(e)
							}
						}
					}
				})
			},
			//企业创建
			handCreate(){
				if (!this.isSubmit) {
				       this.isSubmit = true;
				       setTimeout(() => {
				         this.isSubmit = false;
				       }, 2000); // 设置 2 秒后可再次点击
				}
				this.$refs.form.validate().then(data => {
					if(!this.formData.logo2){
						uni.showToast({
							icon:'none',
							title:'The company logo cannot be empty！'
						})
						return
					}
				creatCompany(
					this.formData
				).then((res)=>{
					console.log(res.code);
					if(res.code==0){
						this.$store.state.orgLis.orgList=null;
						uni.showToast({
							icon:'none',
							title:'Created successfully'
						})
							setTimeout(()=>{
						uni.switchTab({
							url:'/pages/profile/profile'
						})
						},500)
					}
				}).catch((err)=>{
					this.setMsgTop(err)
				})
					})
			},
			handSelectValue(e){
				console.log(e,'eeeeeee')
				this.formData.size=e;
			},
			onSizeClick() {
				this.$refs.sizepick.toOpen();
			},
			onChangeSize(e) {
				console.log("e,规模", e);
				this.comSizeName = e.label;
				this.comSizeValue = e;
				this.orgForm.size = e.value
				
			},
			onnodeclick(node){
				console.log(node)
				this.listData.forEach((item,index)=>{
						if(item.ParentId===node.value){
						//	this.ObtainInData.push({children:[{value:item.Id,text:item.Name}]})
						
						}
				})
			//	console.log(this.ObtainInData,'this.ObtainInData')
			},
			  onchange(e) {
					const value = e.detail.value
					console.log(value[1],'1111')
					this.formData.industry=value[1].value;
			      },
				  //省市区列表
				  cityData(){
					  provincesList().then((res)=>{
						    console.log(res,'省市区列表')
						  if(res.code==0){
							
							  
						  }
					  })
				  },
			//获取行业列表
			list(){
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
			}
		}
	}
</script>
<style lang="less" scoped>
	.createCompany{
		/deep/.selected-list{
			font-size:32rpx;
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
		/deep/.placeholder{
			font-size:32rpx!important;
			color:rgba(255, 255, 255, 0.2);
		}
		/deep/.text-color{
			color:#fff;
		}
		/deep/.uni-select__selector{
			//color:#333;
		}
		/deep/.input-value-border{
			border:1rpx solid rgba(255, 255, 255, 0.2);
			height:90rpx;
		}
		.createCompany-Complete{
			margin:55rpx 3%;
			height: 100rpx;
			line-height: 100rpx;
			color:#fff;
			text-align: center;
			border-radius: 6rpx;
			background: linear-gradient(180deg,rgba(255, 53, 53, 1),rgba(255, 97, 61, 1));
			font-size:36rpx;
		}
		.addPool-form{
			width:94%;
			margin:0rpx 3%;
			margin-top:50rpx;
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
					/deep/.uni-select__input-text{
						color:#fff!important;
					}
					/deep/.uni-select{
						height: 88rpx!important;
						border:1rpx solid rgba(255, 255, 255, .2);
					}
					/deep/.uni-select__input-placeholder{
						font-size:32rpx!important;
						color:rgba(255, 255, 255, .2)!important;
					}
				}
				
				/deep/.uni-forms-item__label{
					height:50rpx;
					color:rgba(255, 255, 255, .7);
				}
				/deep/.uni-forms-item{
					
				}
				
				.addPool-easyinput{
					background: #161A26!important;
					border:1rpx solid rgba(255, 255, 255, .2);
					border-radius: 12rpx;
					font-size:32rpx;
					height: 88rpx;
					color:#fff;
					padding:0rpx 20rpx;
				}
				
			}
			
			
		}
		.createCompany-header{
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
				font-size:32rpx;
				color:rgba(255, 255, 255, .5);
				text-align: center;
				text-decoration:underline;
				margin:0 auto;
			}
		}
	}
</style>

