<template>
	<view class="changePassword">
		<top leftIcon="icon-fanhui" :isleftBack="true" leftText="Back" backgroundColor="#161A26" title="Change password" class="CRM-header"></top>
		<uni-forms ref="loginForm" :modelValue="loginForm" :rules="rules" labelWidth='80' label-position="top" style="width: 92%;margin:50rpx 4%;">
			<uni-forms-item required label="    Existing password" name="orgName" class="addPool-form-item">
				<input  v-model="loginForm.oldPassword" type="password" class="addPool-easyinput" placeholder="Please enter existing password" placeholder-style="color:rgba(255, 255, 255, .2);">
			</uni-forms-item>
			<uni-forms-item required label="     New password" name="orgName" class="addPool-form-item">
				<input  v-model="loginForm.newPassword" type="password" class="addPool-easyinput" placeholder="Please enter new password" placeholder-style="color:rgba(255, 255, 255, .2);">
			</uni-forms-item>
			
			<uni-forms-item required label="      Confirm new password" name="orgName" class="addPool-form-item">
				<input  v-model="loginForm.confirmPassword" type="password" class="addPool-easyinput" placeholder="Please enter new password again" placeholder-style="color:rgba(255, 255, 255, .2);">
			</uni-forms-item>
		</uni-forms>
		<button class="Confirm" :disabled="isSubmit" @click="handSubmit" :class="[loginForm.oldPassword&&loginForm.newPassword&&loginForm.confirmPassword?'ConfirmActive':'ConfirmOn']">Confirm</button>
		<msg-prompt ref="promptMsg"></msg-prompt>
	</view>
</template>

<script>
	import {
	updateUserPwd
	} from "@/api/personalCenter";
	export default {
		data(){
			return{
				isSubmit:false,
				hideCode:false,
				getCodeText: 'Get code',
						getCodeBtnColor: "#ffffff",
						getCodeisWaiting: false,
						disabled:false,
				loginForm:{
					oldPassword:'',
					newPassword:"",
					confirmPassword:""
				},
				rules: {
					username: {
						rules: [{
							required: true,
							errorMessage: 'Please enter an account',
						}]
					},
					password: {
						rules: [{
							required: true,
							errorMessage: 'Please enter the password',
						}]
					},
					code: {
						rules: [{
							required: true,
							errorMessage: 'Please enter the code',
						}]
					}
				},
				uuid:'',
				imgImg:''
			}
		},
		onLoad(Option) {
			if(Option.email){
				this.loginForm.email=Option.email
			}
		},
		methods:{
			handSubmit(){
				if (!this.isSubmit) {
				       this.isSubmit = true;
				       setTimeout(() => {
				         this.isSubmit = false;
				       }, 2000); // 设置 2 秒后可再次点击
				}
				if(!this.loginForm.oldPassword){
					uni.showToast({
						icon:'none',
						title:'Please enter the oldPassword'
					})
					return
				}
				if(!this.loginForm.newPassword){
					uni.showToast({
						icon:'none',
						title:'newPassword cannot be empty'
					})
					return
				}
				if(!this.loginForm.confirmPassword){
					uni.showToast({
						icon:'none',
						title:'confirmPassword cannot be empty'
					})
					return
				}
				if(this.loginForm.confirmPassword!=this.loginForm.newPassword){
					uni.showToast({
						icon:'none',
						title:'The two password inputs are inconsistent'
					})
					return
				}
				 updateUserPwd(this.loginForm.oldPassword, this.loginForm.newPassword).then(response => {
							  if(response.code==0){
								  uni.showToast({
								  	title:'Modified successfully！',
									icon:'none'
								  })
								  setTimeout(()=>{
									  uni.navigateTo({
									  	url:'./personData'
									  })
								  },500)
								
								  
							  }
				            
				          }).catch((err)=>{
							  this.setMsgTop(err)
						  })
			},
		
			
		
			handGetCode(){
				// console.log(this.uuid);
				// return
				if(this.loginForm.email==''){
					uni.showToast({
						title:'Please enter your phone number！',
						icon:'none'
					})
					return
				}
				//if(this.uuid){
					sendEmailCode({
						email: this.loginForm.email
					}).then((res)=>{
						if(res.code==0){
							this.disabled = true
								this.getCodeText = "Sending..." //发送验证码
								this.getCodeisWaiting = true;
								this.getCodeBtnColor = "rgba(255,255,255,0.5)" //追加样式，修改颜色
								//示例用定时器模拟请求效果
								//setTimeout(()用于在指定的毫秒数后调用函数或计算表达式
								setTimeout(() => {
									//this.$common.msg('验证码已发送')
									 uni.showToast({
									 	title: 'Verification code has been sent',
									 	icon: "none"
									 }); //弹出提示框
									this.setTimer(); //调用定时器方法
								}, 1000)
						}
						
					}).catch((err)=>{
						this.setMsgTop(err)
					})
				//}
		
			
				
			},
			setTimer() {
				let holdTime = 60; //定义变量并赋值
				this.getCodeText = "60s"
				//setInterval（）是一个实现定时调用的函数，可按照指定的周期（以毫秒计）来调用函数或计算表达式。
				//setInterval方法会不停地调用函数，直到 clearInterval被调用或窗口被关闭。
				this.Timer = setInterval(() => {
					if (holdTime <= 0) {
						this.disabled = false
						this.getCodeisWaiting = false;
						this.getCodeBtnColor = "#ffffff";
						this.getCodeText = "Get code"
						clearInterval(this.Timer); //清除该函数
						return; //返回前面
					}
					this.getCodeText = holdTime + "s"
					holdTime--;
				}, 1000)
			}
		}
	}
</script>

<style lang="less" scoped>
	.changePassword{
		/deep/.uni-select__input-text{
			font-size:32rpx;
		}
		/deep/.uni-forms-item__label{
			font-size:32rpx!important;
		}
		/deep/.uni-textarea-textarea{
			font-size:32rpx;
		}
		.BindPhoneNumber-text{
			display: flex;
			background: #161A26!important;
			border:1rpx solid rgba(255, 255, 255, .2);
			border-radius: 12rpx;
			font-size:30rpx;
			height: 88rpx;
			color:#fff;
			// padding:0rpx 20rpx;
			align-items: center;
			.GetCode{
				color:#FF3535;
				padding-left:20rpx;
				margin-left:auto;
				font-size:32rpx;
				line-height: 38rpx;
				outline: none;
				width:200rpx;
				background: none;
				text-align: center;
				
			}
			.BindPhoneNumber-text-input{
				width: 500rpx;
				padding-right:30rpx;
				border-right: 1rpx solid rgba(255, 255, 255, .2);
				padding-left:20rpx;
			}
		}
		.Confirm{
			width: 92%;
			margin:0rpx 4%;
			height:100rpx;
			line-height: 100rpx;
			text-align: center;
			border-radius: 8rpx;
			font-size:36rpx;
		}
		.ConfirmActive{
			background: linear-gradient(180deg,#FF3535,#FF613D);
			color:#fff;
		}
		.ConfirmOn{
			background: #1C2232;
			color:rgba(255, 255, 255, .2);
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
				/deep/.uni-select__input-text{
					color:#fff!important;
				}
				/deep/.uni-select{
					height: 88rpx!important;
					border:1rpx solid rgba(255, 255, 255, .2);
				}
				/deep/.uni-select__input-placeholder{
					font-size:30rpx!important;
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
		.imgImg{
			width: 224rpx;
			height:88rpx;
			margin-left: auto;
		}
	}
</style>