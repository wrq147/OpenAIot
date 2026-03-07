<template>
	<view style="min-height: 100vh;background-color: #fff;">
		<top title="创建企业" leftWidth="157rpx" leftIcon="icon-fanhui" :isleftBack="true" backgroundColor="#fff"></top>
		<view class="register_con">

			<view class="register_form_con">
				<uni-forms ref="orgForm" :modelValue="orgForm" :rules="orgRules" label-position="top" :labelWidth="200">

					<view class="form_con">
						<uni-forms-item required name="logo" id="logo_form" labelFont="32rpx" contentFont="32rpx"
							:requireOpacity="0.5" :showLabel="false" errorMsgLeft="calc(50% - 80rpx)">
							<view class="touxiang">
								<view class="image t-icon-qiyemorentupian1" v-if="!orgForm.logo"></view>
								<image class="image" :src="orgForm.logo2+'?wh=500x500'" mode="aspectFit"
									v-if="orgForm.logo2"></image>
							</view>
							<view class="upload_li" @click.stop="uploadMul">
								上传logo
							</view>
						</uni-forms-item>
						<uni-forms-item label="企业名称" required name="orgName" id="orgName_form" labelFont="32rpx"
							contentFont="32rpx" :requireOpacity="0.5">
							<view id="orgName" class="form_li">
								<uni-easyinput placeholderStyle="color:#c1c1c1;font-size:32rpx" inputHeight="88rpx"
									:styles="styles" type="text" v-model="orgForm.orgName" placeholder="请输入企业名称"
									contentFontSize="32rpx" primaryColor="#2371FF" />
							</view>
						</uni-forms-item>
						<uni-forms-item label="行业类型" required name="industry" id="industry_form" labelFont="32rpx"
							contentFont="32rpx" :requireOpacity="0.5">
							<view id="industry" class="form_li" @click="onHYClick">
								<!-- <uni-easyinput placeholderStyle="color:#999999;font-size:32rpx" inputHeight="88rpx"
									:styles="styles" type="text" v-model="hyName" placeholder="请选择行业类型"
									contentFontSize="32rpx" primaryColor="#2371FF" @clear="clearHyName"
									@focus="onHYClick" /> -->
								<view class="form_input" :class="{'placeholder_input':!hyName||hyName==''}">
									{{hyName?hyName:'请选择行业类型'}}
								</view>
								<view class="form_sel_icon">
									<custom-icons iconsName="icon-a-youjiantoubai" iconsSize="20rpx"
										iconsColor="#999999"></custom-icons>
								</view>
							</view>
						</uni-forms-item>
						<uni-forms-item label="预计使用规模" required name="size" id="size_form" labelFont="32rpx"
							contentFont="32rpx" :requireOpacity="0.5">
							<view id="size" class="form_li" @click="onSizeClick">
								<!-- <uni-easyinput placeholderStyle="color:#c1c1c1;font-size:32rpx" inputHeight="88rpx"
									:styles="styles" type="text" v-model="comSizeName" placeholder="请选择规模"
									contentFontSize="32rpx" primaryColor="#2371FF" @clear="clearSizeName"
									@focus="onSizeClick" /> -->
								<view class="form_input" :class="{'placeholder_input':!comSizeName||comSizeName==''}">
									{{comSizeName?comSizeName:'请选择规模'}}
								</view>
								<view class="form_sel_icon">
									<custom-icons iconsName="icon-a-youjiantoubai" iconsSize="20rpx"
										iconsColor="#999999"></custom-icons>
								</view>
							</view>
						</uni-forms-item>
						<button class="submit_button" @click="nextClick" :disabled="isLoading"
							:style="{'opacity':isLoading?0.6:1}" :loading="isLoading">
							下一步
						</button>
					</view>
				</uni-forms>
			</view>
		</view>
		<uni-data-picker ref="hypick" :map="{text:'Name',value:'Id',children:'children'}" popup-title="请选择行业类型"
			:value="hyValue" :localdata="hyItems" @change="onChangeHY" placeholder="placeholder" :isDark="isDark">
		</uni-data-picker>
		<jp-select ref="sizepick" name="label" idKey="value" :checkAll="false" :list="comSizeItems" :item="comSizeValue"
			select="radio" @checked="onChangeSize" tite="请选择规模"></jp-select>
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
		agentInviteInfo,
	} from "@/api/login";
	import {
		reverseGeocoder,
		AreaGeocoder
	} from '@/api/commonApi.js'
	export default {
		data() {
			return {
				isDark: false,
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
							errorMessage: "请输入企业名称"
						}]
					},
					industry: {
						rules: [{
							required: true,
							errorMessage: "请选择行业类型"
						}]
					},
					size: {
						rules: [{
							required: true,
							errorMessage: "请选择规模"
						}]
					},
					logo: {
						rules: [{
							required: true,
							errorMessage: "请上车企业logo"
						}]
					},
				},
				isLoading: false,
				styles: {
					color: '#333',
					backgroundColor: '#F8F8F8',
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
			let rt1=await this.$store.dispatch("data/industryTree")
			this.hyItems = rt1;
			let rt2=await this.$store.dispatch("data/dictList", "org_size")
			this.comSizeItems = rt2;
			if (options.yaoqingId && options.yaoqingId != 'null' && options.yaoqingId != null) {
				this.yaoqingId = options.yaoqingId
				this.loadYaoqingInfo(this.yaoqingId)
			}
			if (options.isAppReg) {
				this.isAppReg = true
			}
		},
		methods: {
			loadYaoqingInfo(id){
			  agentInviteInfo({id:id}).then(async res=>{
				console.log("邀请信息",res);
				let data=res.data
				if(data&&data.AddressCode){
				  this.orgForm.addressName=data.AddressName;
				  this.orgForm.addressCode=data.AddressCode;
				  this.orgForm.lng=data.Lng;
				  this.orgForm.lat=data.Lat;
				  this.orgForm.addressDetail=data.AddressDetail;
				}
				if(data&&data.Industry){
				  this.orgForm.industry=data.Industry
				  
				  this.hyValue = data.Industry;
				  let nameArr = await this.getIndustryArr(data.Industry);
				  if(nameArr&&nameArr.length==2){
				    this.hyName = nameArr[0] + ">" + nameArr[1];
				  }
				  
				}
				if(data&&data.Size){
				  this.orgForm.size=data.Size
				  let rowSize=this.comSizeItems.find(rw=>rw.value==data.Size)
				  if(rowSize){
					  this.comSizeName = rowSize.label;
					  this.comSizeValue = rowSize;
				  }
				  
				}
				if(data&&data.Logo){
				  this.orgForm.logo=data.Logo
				  this.orgForm.logo2=data.Logo
				}
				if(data&&data.OrgName){
				  this.orgForm.orgName=data.OrgName
				}
			  })
			},
			async getIndustryArr(Industry) {
			  //获取行业规模数组
			  let arr = await this.$store.dispatch("data/industryTree");
			  let isFinish = false;
			  let rsArray = [];
			  console.log("行业类型",arr);
			  for (let index = 0; index < arr.length; index++) {
				if (arr[index].children && arr[index].children.length > 0) {
				  for (let ix = 0; ix < arr[index].children.length; ix++) {
					if (arr[index].children[ix].Id == Industry) {
					  rsArray = [];
					  console.log(arr[index].children[ix],'arr[index].children[ix]');
					  rsArray.push(arr[index].Name);
					  rsArray.push(arr[index].children[ix].Name);
					  return rsArray;
					  // this.org.industryArr = JSON.parse(JSON.stringify(rsArray));
					}
				  }
				  if (isFinish) {
					return rsArray;
				  }
				}
			  }
			},
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
						this.$refs.promptMsg.loadingOpen('上传中...')
						if (res.tempFilePaths.length == 0) {
							return;
						}
						for (let i = 0; i < res.tempFiles.length; i++) {
							if (res.tempFiles[i].size > 10 * 1024 * 1024) {
								this.$refs.promptMsg.open('上传的图片不能超过10MB', 2000)
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
					creatCompany(submitQuery).then(async res => {
						// console.log(res, "创建企业成功");
						
						if (res.code == 0) {
							this.$refs.promptMsg.open('创建企业成功', 1500)
							// this.$refs.promptMsg.succossOpen()
							// this.$router.go(-1);
							if (this.yaoqingId) {
								setTimeout(() => {
									if (res.data) {
										this.$refs.promptMsg.loadingOpen('加入中...')
										joinInvite({
											id: this.yaoqingId,
											orgId: res.data
										}).then(async rsp => {

											if (rsp.code == 0) {
												this.$refs.promptMsg.loadingColse()
												this.$refs.promptMsg.open(
													'加入成功，正在登录',
													2000)
												try{
													this.$store.commit('orgLis/SET_ORG_LIST', null)
													await this.$store.dispatch("orgLis/setOrgList");
													await this.$store.dispatch('GetInfo')
													setTimeout(async () => {
														await this.$store.dispatch('GetInfo')
														this.isLoading = false;
														uni.reLaunch({
															url: this.$store.state.homejumpurl?this.$store.state.homejumpurl:'/pages/index/index'
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
								// console.log(this.isAppReg,'自行注册');
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
														url: this.$store.state.homejumpurl?this.$store.state.homejumpurl:'/pages/index/index'
													})
												})
											},1500)
										}
									}).catch((err) => {
										//console.log(err,'1111111')
										this.setMsgTop(err)
									})
								} else {
									await this.$store.dispatch('GetInfo')
									setTimeout(()=>{
										uni.reLaunch({
											url: this.$store.state.homejumpurl?this.$store.state.homejumpurl:'/pages/index/index'
										})
									},500)
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
				// uni.hideKeyboard()
				this.$refs.sizepick.toOpen();
			},
			onHYClick() {
				// uni.hideKeyboard()
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
		color: #2371FF;
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