<template>
	<view class="register_con">
		<view class="title_text">Employee invitation</view>
		<view class="touxiang" @click="getAvatarAfter">
			<image class="image" :src="basicInfo.avatar" mode="aspectFit" v-if="basicInfo.avatar"></image>
		</view>
		<view class="company">
			<custom-icons iconsName="icon-gongsi" iconsSize="26rpx"
				iconsColor="rgba(255, 255, 255, 0.5)"></custom-icons>
			<view class="name">{{basicInfo.company}}</view>
		</view>
		<view class="register_form_con">
			<uni-forms ref="basicInfo" :modelValue="basicInfo" :rules="rules" label-position="top">
				<view class="form_con">
					<uni-forms-item label="username" required name="realName" id="realName_form" labelFont="32rpx"
						contentFont="32rpx" :requireOpacity="0.5">
						<view id="realName" class="form_li">
							<uni-easyinput placeholderStyle="color:rgba(255, 255, 255, 0.2);font-size:32rpx"
								inputHeight="88rpx" :styles="styles" type="text" v-model="basicInfo.realName"
								placeholder="Please enter your username" contentFontSize="32rpx"
								primaryColor="rgba(255, 255, 255, 0.5)" disabled/>
						</view>
					</uni-forms-item>
					<uni-forms-item label="Department" required name="deptId" id="deptId_form" labelFont="32rpx"
						contentFont="32rpx" :requireOpacity="0.5">
						<view id="deptId" class="form_li">
							<uni-data-select v-model="basicInfo.deptId" :localdata="localdata" @change="change"
								width="100%" placeholder="Please select a department"
								borderColor="rgba(255, 255, 255, 0.20)" palColor="rgba(255, 255, 255, 0.2)"
								:isCustom="true" :isDark="true"></uni-data-select>
						</view>
					</uni-forms-item>
					<uni-forms-item label="Position" required name="postName" id="postName_form" labelFont="32rpx"
						contentFont="32rpx" :requireOpacity="0.5">
						<view id="postName" class="form_li">
							<uni-easyinput placeholderStyle="color:rgba(255, 255, 255, 0.2);font-size:32rpx"
								inputHeight="88rpx" :styles="styles" type="text" v-model="basicInfo.postName"
								placeholder="Please enter the Position" contentFontSize="32rpx"
								primaryColor="rgba(255, 255, 255, 0.5)" />
						</view>
					</uni-forms-item>
					<button class="submit_button" @click="nextClick" :disabled="isLoading"
						:style="{'opacity':isLoading?0.6:1}" :loading="isLoading">
						JOIN
					</button>
				</view>
			</uni-forms>
		</view>
		<msg-prompt ref="promptMsg"></msg-prompt>
	</view>
</template>

<script>
	import {
		pathToBase64,
		base64ToPath
	} from 'image-tools'
	import {
		explainCode,
		staffJoinOrg
	} from '@/api/login.js'
	export default {
		data() {
			return {
				localdata: [],
				basicInfo: {},
				isLoading: false,
				rules: {
					realName: {
						rules: [{
							required: true,
							errorMessage: 'Please enter your username',
						}]
					},
					deptId: {
						rules: [{
							required: true,
							errorMessage: 'Please select a department',
						}]
					},
					postName: {
						rules: [{
							required: true,
							errorMessage: 'Please enter the Position',
						}]
					},
				},
				styles: {
					color: '#fff',
					backgroundColor: '#161A26',
					disableColor: 'rgba(28, 34, 50, 1)',
					borderColor: 'rgba(255, 255, 255, 0.20)'
				},
				avatarGroup: [],
				isAppReg: false, //是否是app直接注册
				isFirstLoad: true
			};
		},
		async onLoad(options) {
			this.$nextTick(() => {
				this.isFirstLoad = true
			})
			// console.log('登录用户信息',this.$store.state.user.uid);
			if(this.$store.state.user.uid){
				this.basicInfo.avatar=this.$store.state.user.avatar
				this.basicInfo.realName=this.$store.state.user.name
			}else{
				
				try{
					let personInfo=await this.$store.dispatch("GetInfo")
					// console.log(personInfo,'返回的个人信息');
					this.basicInfo.avatar=personInfo.data.avatar
					this.basicInfo.realName=personInfo.data.name
				}catch(e){
					//TODO handle the exception
					
				}
			}
			
			if (options.code) {
				explainCode({
					code: options.code
				}).then(res => {
					// console.log("解析结果", res);
					this.basicInfo.code = options.code;
					this.basicInfo.company = res.data.OrgName;
					if (res.data.DeptList) {
						this.localdata=[]
						res.data.DeptList.map(row => {
							let obj = {
								text: row.label,
								value: row.id
							}
							this.localdata.push(obj)
						})
					}
				}).catch(err => {
					this.setMsgTop(err)
				})
			}
		},
		methods: {
			change() {
				//选择部门
			},
			nextClick() {
				//跳转下一步
				console.log("weee");
				this.$refs.basicInfo.validate().then(res => {
					this.isLoading=true
					staffJoinOrg({
						code:this.basicInfo.code,
						postName:this.basicInfo.postName,
						deptId:this.basicInfo.deptId
					}).then(async res=>{
						// console.log("加入邀请");
						this.$refs.promptMsg.succossOpen()
						await this.$store.dispatch("GetInfo")
						this.$store.commit('orgLis/SET_ORG_LIST', null)
						setTimeout(async () => {
							await this.$store.dispatch("GetInfo")
							uni.reLaunch({
								url: '/pages/devices/devices'
							})
						}, 1500)
					}).catch(err=>{
						this.isLoading=false
						this.setMsgTop(err)
						console.log('加入邀请失败',err);
					})
				})
			}
		}
	}
</script>

<style lang="less" scoped>
	.title_text {
		color: #fff;
		font-size: 48rpx;
		margin-top: 128rpx;
	}

	.touxiang {
		position: relative;
		margin-top: 80rpx;

		.image {
			width: 160rpx;
			height: 160rpx;
			border-radius: 50%;
		}

		.up_icons {
			position: absolute;
			right: 0;
			bottom: 0;
			width: 50rpx;
			height: 50rpx;
		}
	}

	.company {
		font-size: 32rpx;
		color: rgba(255, 255, 255, 0.5);
		display: flex;
		justify-content: flex-start;
		align-items: center;
		margin-top: 40rpx;

		.name {
			margin-left: 10rpx;
		}
	}

	.register_con {
		padding: 0;
	}

	.register_form_con {
		margin-top: 80rpx;
	}
</style>