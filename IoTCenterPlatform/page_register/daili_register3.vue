<template>
	<view>
		<top title="Create company" leftText="Back" leftWidth="157rpx" leftIcon="icon-fanhui" :isleftBack="true"
			backgroundColor="#161A26"></top>
		<view class="register_con">

			<view class="register_form_con">
				<uni-forms ref="orgForm" :modelValue="orgForm" :rules="orgRules" label-position="top" :labelWidth="200">

					<view class="form_con">
						<uni-forms-item required name="logo" id="logo_form" labelFont="32rpx" contentFont="32rpx"
							:requireOpacity="0.5" :showLabel="false" errorMsgLeft="calc(50% - 80rpx)">
							<view class="touxiang">
								<view class="image t-icon-qiyemorentupian" v-if="!orgForm.logo"></view>
								<image class="image" :src="orgForm.logo2+'?wh=500x500'" mode="aspectFit"
									v-if="orgForm.logo2"></image>
							</view>
							<view class="upload_li" @click.stop="uploadMul">
								Upload logo
							</view>
						</uni-forms-item>
						<uni-forms-item label="Company name" required name="orgName" id="orgName_form" labelFont="32rpx"
							contentFont="32rpx" :requireOpacity="0.5">
							<view id="orgName" class="form_li">
								<uni-easyinput placeholderStyle="color:rgba(255, 255, 255, 0.2);font-size:32rpx"
									inputHeight="88rpx" :styles="styles" type="text" v-model="orgForm.orgName"
									placeholder="Please enter your Company name" contentFontSize="32rpx"
									primaryColor="rgba(255, 255, 255, 0.5)" />
							</view>
						</uni-forms-item>
						<uni-forms-item label="Industry type" required name="industry" id="industry_form"
							labelFont="32rpx" contentFont="32rpx" :requireOpacity="0.5">
							<view id="industry" class="form_li" @click="onHYClick">
								<!-- <uni-easyinput placeholderStyle="color:rgba(255, 255, 255, 0.2);font-size:32rpx"
									inputHeight="88rpx" :styles="styles" type="text" v-model="hyName"
									placeholder="Please select industry type" contentFontSize="32rpx"
									primaryColor="rgba(255, 255, 255, 0.5)" @clear="clearHyName" @focus="onHYClick" /> -->
								<view class="form_input" :class="{'placeholder_input':!hyName||hyName==''}">
									{{hyName?hyName:'Please select industry type'}}
								</view>
								<view class="form_sel_icon">
									<custom-icons iconsName="icon-a-youjiantoubai" iconsSize="20rpx"
										iconsColor="rgba(255, 255, 255, 0.5)"></custom-icons>
								</view>
							</view>
						</uni-forms-item>
						<uni-forms-item label="Expected usage scale" required name="size" id="size_form"
							labelFont="32rpx" contentFont="32rpx" :requireOpacity="0.5">
							<view id="size" class="form_li" @click="onSizeClick">
								<!-- <uni-easyinput placeholderStyle="color:rgba(255, 255, 255, 0.2);font-size:32rpx"
									inputHeight="88rpx" :styles="styles" type="text" v-model="comSizeName"
									placeholder="Please select the expected usage scale" contentFontSize="32rpx"
									primaryColor="rgba(255, 255, 255, 0.5)" @clear="clearSizeName" @focus="onSizeClick"/> -->
								<view class="form_input" :class="{'placeholder_input':!comSizeName||comSizeName==''}">
									{{comSizeName?comSizeName:'Please select the expected usage scale'}}
								</view>
								<view class="form_sel_icon">
									<custom-icons iconsName="icon-a-youjiantoubai" iconsSize="20rpx"
										iconsColor="rgba(255, 255, 255, 0.5)"></custom-icons>
								</view>
							</view>
						</uni-forms-item>
						<button class="submit_button" @click="nextClick" :disabled="isLoading"
							:style="{'opacity':isLoading?0.6:1}" :loading="isLoading">
							Next
						</button>
					</view>
				</uni-forms>
			</view>
		</view>
		<uni-data-picker ref="hypick" :map="{text:'Name',value:'Id',children:'children'}"
			popup-title="Please select industry type" :value="hyValue" :localdata="hyItems" @change="onChangeHY"
			placeholder="placeholder" :isDark="isDark">
		</uni-data-picker>

		<jp-select ref="sizepick" name="label" idKey="value" :checkAll="false" :list="comSizeItems" :item="comSizeValue"
			select="radio" @checked="onChangeSize" tite="Please select the expected usage scale"></jp-select>
		<msg-prompt ref="promptMsg"></msg-prompt>
	</view>
</template>

<script>
	import {
		uploadPhoto,
		delPhoto,
		creatCompany,
		joinInvite,
		switchOrg
	} from '@/api/user.js'
	import {
		reverseGeocoder,
		AreaGeocoder
	} from '@/api/commonApi.js'
	export default {
		data() {
			return {
				isDark: true,
				hyName: '',
				hyValue: "0",
				hyItems: [],
				comSizeName: '',
				comSizeValue: {},
				comSizeItems: [],
				dzValue: null,
				dzItems: [],
				orgForm: {
					orgName: "",
					industry: null,
					size: null,
					logo: null,
					logo2: null,
					addressName: "",
					addressCode: '',
					lng: 0,
					lat: 0,
					geo: '',
					addressDetail: "",
					intro: ""
				},
				orgRules: {
					orgName: {
						rules: [{
							required: true,
							errorMessage: "Please enter the company name"
						}]
					},
					industry: {
						rules: [{
							required: true,
							errorMessage: "Please select industry type"
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
				isLoading: false,
				styles: {
					color: '#fff',
					backgroundColor: '#161A26',
					disableColor: 'rgba(28, 34, 50, 1)',
					borderColor: 'rgba(255, 255, 255, 0.20)'
				},
				localdata: [{
						text: "text",
						value: 0
					},
					{
						text: "develop",
						value: 1
					}
				],
				isAppReg: false, //是否是app内部注册
				yaoqingId: null, //是否是代理商邀请登录
			}
		},
		async onLoad(options) {
			this.$store.dispatch("data/industryTree").then(rt => {
				this.hyItems = rt;
			});
			this.$store.dispatch("data/dictList", "org_size").then(rt => {
				this.comSizeItems = rt;
			});
			if (options.yaoqingId && options.yaoqingId != 'null' && options.yaoqingId != null) {
				this.yaoqingId = options.yaoqingId
			}
			if (options.isAppReg) {
				this.isAppReg = true
			}
		},
		methods: {
			clearHyName() {
				this.hyValue = '0'
			},
			clearSizeName() {
				this.comSizeValue = {}
			},
			//选择地址
			uploadMul(type) {
				//手动上传图片
				uni.chooseImage({
					count: 1,
					sizeType: ['compressed'], //可以指定是原图还是压缩图，默认二者都有
					success: async (res) => {
						this.$refs.promptMsg.loadingOpen('Uploading...')
						if (res.tempFilePaths.length == 0) {
							return;
						}
						for (let i = 0; i < res.tempFiles.length; i++) {
							if (res.tempFiles[i].size > 10 * 1024 * 1024) {
								this.$refs.promptMsg.open('Uploading images cannot exceed 10MB', 2000)
								return;
							}
							try {
								let paths = res.tempFilePaths[i];
								let rsp = await uploadPhoto(paths)
								if (this.orgForm.logo2) {
									let rsp2 = await delPhoto(this.orgForm.logo2)
									// console.log("删除图片", rsp2);
								}
								this.orgForm.logo = rsp
								this.orgForm.logo2 = rsp
								this.$refs.promptMsg.loadingColse()
								this.$forceUpdate()
							} catch (e) {
								//TODO handle the exception
								this.$refs.promptMsg.loadingColse()
								// console.log(e);
								this.setMsgTop(e)
							}
						}
					}
				})
			},
			nextClick() {
				//完成创建企业
				this.$refs.orgForm.validate().then(aft => {
					// console.log("aft", aft);
					this.isLoading = true
					let submitQuery = JSON.parse(JSON.stringify(this.orgForm))
					delete submitQuery.logo2
					creatCompany(submitQuery).then(res => {
						// console.log(res, "创建企业成功");
						if (res.code == 0) {
							this.$refs.promptMsg.open('Successfully created enterprise', 2000)
							// this.$refs.promptMsg.succossOpen()
							// this.$router.go(-1);
							if (this.yaoqingId) {
								setTimeout(() => {
									if (res.data) {
										this.$refs.promptMsg.loadingOpen('Joining...')
										joinInvite({
											id: this.yaoqingId,
											orgId: res.data
										}).then(async rsp => {

											if (rsp.code == 0) {
												this.$refs.promptMsg.loadingColse()
												this.$refs.promptMsg.open(
													'Successfully joined, Logging...',
													2000)
												try{
													this.$store.commit('orgLis/SET_ORG_LIST', null)
													await this.$store.dispatch("orgLis/setOrgList");
													await this.$store.dispatch('GetInfo')
													setTimeout(async () => {
														await this.$store.dispatch('GetInfo')
														this.isLoading = false;
														uni.reLaunch({
															url: '/pages/home/home'
														})
													}, 2000)
												}catch(e){
													//TODO handle the exception
													this.isLoading = false;
												}
											}
											
										}).catch(e => {
											this.$refs.promptMsg.loadingColse()
											this.setMsgTop(e)
											this.isLoading = false;
										});
									}
								}, 2000)
							} else {

								if (this.isAppReg) {
									switchOrg({
										id: res.data
									}).then(async (res) => {
										if (res.code == 0) {
											this.$store.commit('orgLis/SET_ORG_LIST', null)
											await this.$store.dispatch("orgLis/setOrgList");
											await this.$store.dispatch('GetInfo')
											setTimeout(async ()=>{
												await this.$store.dispatch('GetInfo')
												this.$nextTick(()=>{
													uni.reLaunch({
														url: '/pages/devices/devices'
													})
												})
											},1500)
										}
									}).catch((err) => {
										//console.log(err,'1111111')
										this.setMsgTop(err)
									})
								} else {
									uni.reLaunch({
										url: '/pages/devices/devices'
									})
								}

							}

							// this.$router.push("/");
						}
					}).catch(err => {
						this.isLoading = false;
						this.setMsgTop(err)
					});
				})
			},
			onSizeClick() {
				uni.hideKeyboard()
				this.$refs.sizepick.toOpen();
			},
			onHYClick() {
				uni.hideKeyboard()
				this.$refs.hypick.show();
			},
			onChangeHY(e) {
				this.hyName = e.detail.value[e.detail.value.length - 2].text + ">" + e.detail.value[e.detail.value.length -
					1].text;
				this.hyValue = e.detail.value[e.detail.value.length - 1].value;
				this.orgForm.industry = this.hyValue
			},
			onChangeSize(e) {
				// console.log("e,规模", e);
				this.comSizeName = e.label;
				this.comSizeValue = e;
				this.orgForm.size = e.value
			},
		}
	}
</script>

<style lang="less" scoped>
	.register_con {
		padding: 0;
	}

	.touxiang {
		position: relative;
		// margin-top: 80rpx;
		display: flex;
		justify-content: center;

		.image {
			width: 160rpx;
			height: 160rpx;
		}

	}

	.upload_li {
		font-size: 32rpx;
		color: rgba(255, 255, 255, 0.5);
		text-decoration: underline;
		margin-top: 20rpx;
		margin-bottom: 20rpx;
		text-align: center;
	}

	.register_form_con {
		margin-top: 40rpx;

		.submit_button {
			margin-top: 160rpx;
		}
	}

	.marTop {
		margin-top: 20rpx;
		width: 100%;
	}

	.uni-data-tree {
		position: fixed;
		top: 200vh;
		left: 200vw;
	}
</style>