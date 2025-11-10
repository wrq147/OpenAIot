<template>
	<view class="register_con" style="min-height: 100vh;background-color: #FFFFFF;">
		<view class="title_text">员工邀请</view>
		<view class="touxiang">
			<image class="image" :src="basicInfo.avatar" mode="aspectFit" v-if="basicInfo.avatar"></image>
		</view>
		<view class="company">
			<!-- <custom-icons iconsName="icon-qiye" iconsSize="26rpx"
				iconsColor="#999999"></custom-icons> -->
			<view class="image t-icon-qiye" style="width: 36rpx;height: 36rpx;"></view>
			<view class="name">{{basicInfo.company}}</view>
		</view>
		<view class="register_form_con">
			<uni-forms ref="basicInfo" :modelValue="basicInfo" :rules="rules" label-position="top">
				<view class="form_con">
					<uni-forms-item label="用户名" required name="realName" id="realName_form" labelFont="32rpx"
						contentFont="32rpx" :requireOpacity="0.5">
						<view id="realName" class="form_li">
							<uni-easyinput placeholderStyle="color:#999999;font-size:32rpx"
								inputHeight="88rpx" :styles="styles" type="text" v-model="basicInfo.realName"
								placeholder="请输入用户名" contentFontSize="32rpx"
								primaryColor="#2371FF" disabled/>
						</view>
					</uni-forms-item>
					<uni-forms-item label="部门" required name="deptId" id="deptId_form" labelFont="32rpx"
						contentFont="32rpx" :requireOpacity="0.5">
						<view id="deptId" class="form_li">
							<uni-data-select v-model="basicInfo.deptId" :localdata="localdata" @change="change"
								width="100%" placeholder="请选择部门" borderColor="rgba(255, 255, 255, 0.20)" palColor="#999999"
								:isCustom="true"></uni-data-select>
						</view>
					</uni-forms-item>
					<uni-forms-item label="职位" required name="postName" id="postName_form" labelFont="32rpx"
						contentFont="32rpx" :requireOpacity="0.5">
						<view id="postName" class="form_li">
							<uni-easyinput placeholderStyle="color:#999999;font-size:32rpx"
								inputHeight="88rpx" :styles="styles" type="text" v-model="basicInfo.postName"
								placeholder="请输入职位" contentFontSize="32rpx"
								primaryColor="#2371FF" />
						</view>
					</uni-forms-item>
					<button class="submit_button" @click="nextClick" :disabled="isLoading"
						:style="{'opacity':isLoading?0.6:1}" :loading="isLoading">
						加入邀请
					</button>
				</view>
			</uni-forms>
		</view>
		<msg-prompt ref="promptMsg"></msg-prompt>
	</view>
</template>

<script>
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
							errorMessage: '请输入用户名',
						}]
					},
					deptId: {
						rules: [{
							required: true,
							errorMessage: '请选择部门',
						}]
					},
					postName: {
						rules: [{
							required: true,
							errorMessage: '请输入职位',
						}]
					},
				},
				styles: {
					color: '#333333',
					backgroundColor: '#F8F8F8',
					disableColor: 'rgba(28, 34, 50, 0.2)',
					borderColor: 'rgba(255, 255, 255, 0.20)'
				},
				avatarGroup: [],
				isAppReg: false, //是否是app直接注册
				isFirstLoad: true
			};
		},
		computed:{
		},
		async onLoad(options) {
			this.$nextTick(() => {
				this.isFirstLoad = true
			})
			// console.log('登录用户信息',this.$store.state.user.uid);
			try{
				let personInfo=await this.$store.dispatch("GetInfo")
				// console.log(personInfo,'返回的个人信息');
				this.basicInfo.avatar=personInfo.data.avatar
				this.basicInfo.realName=personInfo.data.name
			}catch(e){
				//TODO handle the exception
				
			}
			
			if (options.code) {
				console.log(options.code,'options.codeoptions.code');
				explainCode({
					code: decodeURIComponent(options.code)
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
						setTimeout(async() => {
							await this.$store.dispatch("GetInfo")
							uni.reLaunch({
								url: this.$store.state.homejumpurl?this.$store.state.homejumpurl:'/pages/index/index'
							})
						}, 1500)
					}).catch(err=>{
						this.isLoading=false
						console.log('加入邀请失败',err);
					})
				})
			}
		}
	}
</script>

<style lang="less" scoped>
	.title_text {
		color: #333333;
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
		color: #999999;
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