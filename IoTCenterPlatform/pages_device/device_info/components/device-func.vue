<template>
	<view>
		<view class="func_list">
			<view class="func_li" v-for="(item,inx) in attrData" v-show="isOnLine">
				<view class="li_left">
					<view class="left_name">
						{{item.name}}
					</view>
					<view class="left_description">
						{{item.returnMsg?item.returnMsg:item.description}}
					</view>
				</view>
				<view class="li_right" @click="openFuncPop(item,inx)" :class="item.disabled?'disable_class':''" v-if="!item.loading">
					<custom-icons iconsName="icon-zhihang" iconsSize="44rpx" iconsColor="#EFA902"></custom-icons>
					<view class="btn_text">
						Execute
					</view>
				</view>
				<view class="li_right disable_class" v-if="item.loading">
					<uni-icons type="spinner-cycle" size="22" color="#EFA902"></uni-icons>
					<view class="btn_text">
						executing
					</view>
				</view>
			</view>
			<uni-load-more iconType="circle" :status="status" v-if="status"/>
		</view>
		<uni-popup ref="funcPop" type="bottom" :mask-click="false" :zIndex="998">
			<scroll-view class="pop_con" :scroll-y="true" :scroll-top="popScollTop">
				<view class="pop_title">
					<view class="pop_title_con">
						<text>Execute Property Settings</text>
						<view class="close_icon" @click.stop="close">
							<custom-icons iconsName="icon-guanbidanchuang" iconsSize="36rpx"
								iconsColor="rgba(255, 255, 255, 0.2)"></custom-icons>
						</view>
					</view>
				</view>
				<view class="pop_title_"></view>
				<uni-forms ref="paramsForm" :modelValue="paramsForm" :rules="paramsRules" labelWidth='80'
					label-position="top">
					<view class="func_input_list">
						<view class="input_li" v-for="(row,index) in implementParamsForm.inputsData">
							<view class="line_title">
								<view class="left_text">
									<text>{{row.name}}</text>
								</view>
								<view class="line"></view>
							</view>
							<!-- <view class="li_info">
								<view class="label">Input Type</view>
								<view class="value">{{row.type}}</view>
							</view>
							<view class="num">
								{{index+1}}
							</view> -->
							<uni-forms-item required :id="row.code+'_form'" :showLabel="false" contentFont="32rpx"
								:name="row.code">
								<view class="li_info" :id="row.code">
									<!-- <view class="label">value</view> -->
									<view class="value input_val">
										<uni-easyinput type="number" :placeholder="row.readOnly?'This parameter is read-only':row.text"
											v-model="paramsForm[row.code]" clearSize="18"
											placeholderStyle="color:rgba(255, 255, 255, 0.2);font-size:32rpx"
											:styles="customstyles" primaryColor="rgba(255, 255, 255, 0.5)"
											inputHeight="88rpx" :isCustom="true" contentFontSize="32rpx"
											:inputBorder="false" v-if="row.type=='int'" :disabled="row.readOnly">
										</uni-easyinput>
										<uni-easyinput type="digit" :placeholder="row.readOnly?'This parameter is read-only':row.text"
											v-model="paramsForm[row.code]" clearSize="18"
											placeholderStyle="color:rgba(255, 255, 255, 0.2);font-size:32rpx"
											:styles="customstyles" primaryColor="rgba(255, 255, 255, 0.5)"
											inputHeight="88rpx" :isCustom="true" contentFontSize="32rpx"
											:inputBorder="false" v-else-if="row.type=='float'" :disabled="row.readOnly">
										</uni-easyinput>
										<uni-datetime-picker ref="dateChoice2" class="date" type="date"
											:clear-icon="false" v-model="paramsForm[row.code]" placeholder='结束日期'
											:isCustom="true" v-else-if="row.type=='date'" :disabled="row.readOnly" :isDark="true">
											<view class="date_slot" :class="{'has_val':paramsForm[row.code],'dis_val':row.readOnly}">
												<view class="text">
													{{paramsForm[row.code]?paramsForm[row.code]:row.text}}
												</view>
												<custom-icons iconsName="icon-xuanzeshijian" iconsSize="36rpx"
													iconsColor="rgba(255, 255, 255, 0.5)"></custom-icons>
											</view>
										</uni-datetime-picker>
										<uni-easyinput type="text" :placeholder="row.readOnly?'This parameter is read-only':row.text"
											v-model="paramsForm[row.code]" clearSize="18"
											placeholderStyle="color:rgba(255, 255, 255, 0.2);font-size:32rpx"
											:styles="customstyles" primaryColor="rgba(255, 255, 255, 0.5)"
											inputHeight="88rpx" :isCustom="true" contentFontSize="32rpx"
											:inputBorder="false" v-else :disabled="row.readOnly">
										</uni-easyinput>
									</view>
								</view>
							</uni-forms-item>
							<view class="li_info">
								<view class="remark_con" v-if="row.remark">
									Notes: {{row.remark}}
								</view>
							</view>
							
						</view>
						<uni-load-more iconType="circle" status="noMore" v-if="!implementParamsForm.inputsData||implementParamsForm.inputsData.length==0"/>
					</view>
					<view class="btn_con_"></view>
					<!-- <view class="btn_con">
						<view class="cancel_btn btn_li" @click="close()">Cancel</view>
						<view class="execute_btn btn_li" @click="carryAction()">Execute</view>
					</view> -->
					<view class="btn_con">
						<view class="cancel_btn btn_li" @click="close()" :class="{'disable_li':matchesLoading}">
							<uni-icons type="spinner-cycle" size="18" v-if="matchesLoading"></uni-icons>Cancel</view>
						<view class="execute_btn btn_li" @click="carryAction()" :class="{'disable_li':activeFunIsDisable||matchesLoading}">
							<uni-icons type="spinner-cycle" size="18" color="#fff" v-if="matchesLoading"></uni-icons>Execute</view>
					</view>
				</uni-forms>
			</scroll-view>
		</uni-popup>
		<msg-prompt ref="promptMsg"></msg-prompt>
	</view>
</template>

<script>
	import {
		deviceFuncList,
		deviceExeFunc
	} from '@/api/device.js'
	export default {
		name: "device-func",
		props: {
			id: {//设备id
				type: String,
				default: ""
			},
			deviceBasic:{
				type:Object,
				default:''
			},
			deviceId: {//设备通讯id
				type: String,
				default: ""
			},
		},
		data() {
			return {
				customstyles: {
					color: '#ffffff',
					backgroundColor: 'rgba(22, 26, 38, 1)',
					disableColor: 'rgba(247,245,246, 0.5)',
					borderColor: 'rgba(255, 255, 255, 0.20)'
				},
				cusMask: true,
				funcvalue: '',
				inputList: [1, 1, 1, 1, 1, 1, 1],
				implementParamsForm: {
					functionId: "",
					inputsData: [],
					paramsRules: {},
				},
				paramsForm: {},
				paramsRules: {},
				matchesLoading: false,
				implementParams: false,
				attrData: [],
				popScollTop:0,
				status:'loading',
				isOnLine:false,
				activeFunIsDisable:false,
				activeFuncIndex:null,
			};
		},
		watch: {
			deviceBasic: {
				handler(newName, oldName) {
					this.$nextTick(()=>{
						if(this.deviceBasic.Online==0){
							this.isOnLine=false
						}else if(this.deviceBasic.Online==1){
							this.isOnLine=true
						}else{
							this.isOnLine=false
						}
						this.getdeviceFuncList()
						if(this.deviceId){
							this.initMQTT();
						}
					})
				},
				// 代表在wacth里声明了firstName这个方法之后立即先去执行handler方法
				immediate: false,
				deep: true,
			},
		},
		computed: {
			activeRules() {
				return this.implementParamsForm.paramsRules
			}
		},
		methods: {
			initMQTT() {
				let that = this;
				let relUrl = "";
				this.$store.dispatch("mqttclient/getClient").then((client) => {
					let tkey = "newfun/" + this.deviceId;
					client.subscribe(tkey, (error) => {
						if (!error) {
							that.$store.commit("mqttclient/Add_Handler", {
								key: tkey,
								func: function(message) {
									let msgtxt = message.toString();
									console.log("msgtxt设备数据mqtt返回", msgtxt);
									console.log(msgtxt.indexOf('show')>-1,'uuuuuu');
									if(msgtxt.indexOf('update')>-1){
									  that.getdeviceFuncList()
									}
									if(msgtxt.indexOf('redirect')>-1){
									  let urlStr = msgtxt.substring(8,msgtxt.length);
									  that.jumpFuncUrl(urlStr)
									}
									
									if(msgtxt.indexOf('show')>-1){
									  let msgStr = msgtxt.substring(4,msgtxt.length);
									  that.attrData[that.activeFuncIndex].returnMsg=msgStr
									  console.log(that.attrData[that.activeFuncIndex],'执行的');
									  that.$forceUpdate()
									}
								},
							});
						}
					});
				});
			},
			jumpFuncUrl(url){
				console.log(url,'跳转链接');
				uni.navigateTo({
					url: '/pages/webview/webview?url='+url
				})
			},
			getdeviceFuncList() {
				//获取设备功能列表
				deviceFuncList({
					id: this.id
				}).then((res) => {
					console.log("设备对应功能列表", res);
					this.attrData = res.data;
					this.$forceUpdate()
					this.status='noMore'
				});
			},
			close() {
				this.$refs.funcPop.close()
			},
			openFuncPop(row,inx) {
				//打开功能执行弹窗
				this.attrData[inx].loading=true
				this.implementParamsForm.functionId = row.code;
				this.activeFuncIndex=inx
				this.activeFunIsDisable=row.disabled
				let paramsForm = {}
				if(this.activeFunIsDisable){
					return
				}
				if(row.inputs&&row.inputs.length>0){
					for (let i = 0; i < row.inputs.length; i++) {
						console.log(row.inputs[i],'row.inputs[i]');
						row.inputs[i].text = "Please enter " + row.inputs[i].name;
						if(row.inputs[i].readOnly){
							row.inputs[i].text="This parameter is read-only"
						}
						if (row.inputs[i].type == "int" || row.inputs[i].type == "float") {
							// row.inputs[i].codeVal = null;
							paramsForm[row.inputs[i].code] = null
							if(row.inputs[i].defval||row.inputs[i].defval==0){//设置默认值
								paramsForm[row.inputs[i].code] = row.inputs[i].defval
							}
						} else {
							// row.inputs[i].codeVal = "";
							paramsForm[row.inputs[i].code] = ''
							if(row.inputs[i].defval||row.inputs[i].defval==0){
								paramsForm[row.inputs[i].code] = row.inputs[i].defval+''
							}
						}
						if(!row.inputs[i].readOnly){
							this.paramsRules[row.inputs[i].code] = {}
							this.paramsRules[row.inputs[i].code].rules = [{
								required: true,
								errorMessage: 'Please enter ' + row.inputs[i].name,
							}]
						}
						
					}
					this.paramsForm = JSON.parse(JSON.stringify(paramsForm))
					// console.log("this.paramsRules", this.paramsRules);
					let lis = JSON.parse(JSON.stringify(row.inputs));
					this.implementParamsForm.inputsData = JSON.parse(JSON.stringify(lis));
					this.resetForm("paramsForm");
					this.$refs.funcPop.open()
				}else{
					this.noInputCarryAction()
				}
			},
			carryAction() {
				this.$forceUpdate()
				if (this.$refs["paramsForm"]) {
					this.$refs["paramsForm"].validate().then(valid => {
						// console.log(valid, 'valid');
						this.matchesLoading = true;
						for(let keys in this.paramsForm){//将整型数据转换
							let keysInfo=this.implementParamsForm.inputsData.find((x) => x.code == keys);
							if(keysInfo&&keysInfo.type=='int'){
								this.paramsForm[keys]=parseInt(this.paramsForm[keys])
							}
						}
						deviceExeFunc({
								id: this.id,
								functionId: this.implementParamsForm.functionId,
								inputs: JSON.parse(JSON.stringify(this.paramsForm)),
							})
							.then((res) => {
								// console.log(res);
								if (res.code == 0) {
									this.attrData[this.activeFuncIndex].loading=false
									this.$refs.funcPop.close()
									this.matchesLoading = false;
									// this.$message({
									// 	message: "执行完成",
									// 	type: "success",
									// 	duration: 5 * 1000,
									// });
									this.$refs.promptMsg.open('Execution completed', 2000)
								}
							})
							.catch((err) => {
								this.attrData[this.activeFuncIndex].loading=false
								this.matchesLoading = false;
								// console.log("报错", err);
								if (err.message) {
									// uni.showModal({
									// 	title: '系统提示',
									// 	content: err.message,
									// 	showCancel: false,
									// 	success: (res) => {
									// 		if (res.confirm) {

									// 		}
									// 	}
									// });
									this.$refs.promptMsg.noticeOpen(err.message)
								}
							});
					}).catch((err) => {
						// console.log("报错", err);
						if (err && err.length > 0) {
							let firstErr = "#" + err[0].key
							const query = uni.createSelectorQuery().in(this);
							query.select(firstErr).boundingClientRect(data => {
								// uni.pageScrollTo({
								// 	scrollTop: data.top - 200,
								// 	// selector: firstErr,
								// 	duration: 500
								// });
								this.popScollTop=data.top - 200
						
							}).exec();
						}
					});
				}
			},
			noInputCarryAction() {
				deviceExeFunc({
						id: this.id,
						functionId: this.implementParamsForm.functionId,
						inputs: {},
					})
					.then((res) => {
						// console.log(res);
						if (res.code == 0) {
							this.attrData[this.activeFuncIndex].loading=false
							this.$refs.funcPop.close()
							this.matchesLoading = false;
							this.$refs.promptMsg.open('Execution completed', 2000)
							this.$forceUpdate()
						}
					})
					.catch((err) => {
						this.attrData[this.activeFuncIndex].loading=false
						this.matchesLoading = false;
						this.$forceUpdate()
						// console.log("报错", err);
						if (err.message) {
							this.$refs.promptMsg.noticeOpen(err.message)
						}
					});
			},
		}
	}
</script>

<style lang="scss" scoped>
	.func_list {
		width: 100%;

		.func_li {
			padding:22rpx 30rpx 24rpx;
			display: flex;
			justify-content: space-between;
			align-items: center;
			color: #fff;
			border-radius: 10rpx;
			margin-top: 20rpx;
			background-color: rgba(28, 34, 50, 1);
			.li_left{
				width: calc(100% - 182rpx);
				.left_description{
					font-size: 24rpx;
					line-height: 36rpx;
					color: rgba(255, 255, 255, 0.5);
					font-weight: normal;
				}
				.left_name{
					line-height: 46rpx;
					font-size: 32rpx;
				}
			}
			.li_right {
				padding-left: 30rpx;
				display: flex;
				flex-direction: column;
				justify-content: center;
				align-items: center;
				color: rgba(255, 255, 255, 0.5);
				border-left: 1rpx solid rgba(255, 255, 255, 0.2);
				&.disable_class{
					opacity: 0.6;
				}
			}
		}
	}

	.pop_con {
		width: 100%;
		background-color: rgba(22, 26, 38, 1);
		position: relative;
		padding: 23rpx 20rpx;
		box-sizing: border-box;
		border-radius: 20rpx 20rpx 0 0;
		max-height: 70vh;
		overflow-y: scroll;

		.pop_title {
			width: calc(100% + 40rpx);
			color: #fff;
			text-align: center;
			font-size: 36rpx;
			line-height: 50rpx;
			font-weight: bold;
			position: fixed;
			z-index: 1000;
			padding: 23rpx 0 33rpx;
			background-color: rgba(22, 26, 38, 1);
			top: 0;
			left: -20rpx;
			box-sizing: border-box;

			.pop_title_con {
				position: relative;

				.close_icon {
					position: absolute;
					right: 40rpx;
					top: -20rpx;
				}
			}

		}

		.pop_title_ {
			width: 100%;
			height: 50rpx;
			padding-bottom: 13rpx;
		}

		.btn_con_ {
			width: 100%;
			height: 98rpx;
		}

		.btn_con {
			width: 100%;
			height: 98rpx;
			display: flex;
			justify-content: space-between;
			align-items: center;
			position: fixed;
			bottom: 0;
			left: 0;
			z-index: 1000;

			.btn_li {
				width: 50%;
				height: 98rpx;
				display: flex;
				justify-content: center;
				align-items: center;
				font-size: 32rpx;

				&.execute_btn {
					background: linear-gradient(180deg, #FF3535 0%, #FF613D 100%);
					color: rgba(255, 255, 255, 1);
				}

				&.cancel_btn {
					background-color: rgba(28, 34, 50, 1);
					color: rgba(255, 255, 255, 0.5);
				}
				&.disable_li{
					opacity: 0.6;
				}
			}
		}

		.func_input_list {
			width: 100%;

			.input_li {
				position: relative;
				width: 100%;
				padding: 30rpx;
				box-sizing: border-box;
				background-color: rgba(28, 34, 50, 1);
				border-radius: 10rpx;
				margin-top: 20rpx;

				.li_info {
					// padding-top: 30rpx;
					// border-top: 1rpx solid rgba(255, 255, 255, 0.2);
					// margin-top: 30rpx;
					font-size: 32rpx;

					.label {
						color: rgba(255, 255, 255, 0.5);
						font-size: 28rpx;
						line-height: 28rpx;
					}

					.value {
						color: rgba(255, 255, 255, 1);
						margin-top: 20rpx;

						&.input_val {
							background-color: rgba(22, 26, 38, 1);
							border-radius: 10rpx;
						}
					}
					.remark_con{
						font-size: 28rpx;
						line-height: 38rpx;
						color: rgba(255, 255, 255, 0.5);
					}
				}
				.num{
					display: flex;
					justify-content: center;
					align-items: center;
					background-color: #1C2232;
					color: rgba(255, 255, 255, 0.5);
					border-radius: 0 10rpx 0 10rpx;
					border: 1rpx solid rgba(255, 255, 255, 0.2);
					position: absolute;
					right: 0;
					top: 1rpx;
					width: 50rpx;
					height: 40rpx;
				}
			}
		}
	}
	.date_slot{
		display: flex;
		justify-content: space-between;
		align-items: center;
		height: 88rpx;
		width: 100%;
		padding: 0 20rpx;
		box-sizing: border-box;
		color: rgba(255,255,255,0.5);
		border-radius: 10rpx;
		&.has_val{
			color: #fff;
		}
		&.dis_val{
			background-color: rgba(247,245,246, 0.5);
			color: #9F9FA4;
		}
	}
</style>