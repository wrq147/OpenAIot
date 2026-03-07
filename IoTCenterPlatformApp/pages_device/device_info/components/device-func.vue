<template>
	<view>
		<view class="func_list">
			<!--  -->
			<view class="func_li" v-if="isOnLine" v-for="(item,inx) in attrData"
				:class="item.disabled||matchesLoading?'disable_class':''">
				<view class="li_left">
					<view class="left_name">
						{{item.name}}
					</view>
					<view class="left_description">
						{{item.returnMsg?item.returnMsg:item.description}}
					</view>
				</view>
				<view class="li_right" @click="openFuncPop(item,inx)">
					<custom-icons iconsName="icon-zhihang" iconsSize="44rpx" iconsColor="#2371FF"></custom-icons>
					<view class="btn_text">
						<uni-icons type="spinner-cycle" size="18" color="#999" v-if="matchesLoading"></uni-icons>执行
					</view>
				</view>
			</view>
			<uni-load-more iconType="circle" :status="status" v-if="status" />
		</view>
		<uni-popup ref="funcPop" type="bottom" :mask-click="false" :zIndex="998">
			<scroll-view class="pop_con" :scroll-y="true" :scroll-top="popScollTop">
				<view class="pop_title">
					<view class="pop_title_con">
						<text>执行属性设置</text>
						<view class="close_icon" @click.stop="close">
							<custom-icons iconsName="icon-guanbidanchuang" iconsSize="36rpx"
								iconsColor="#D8D8D8"></custom-icons>
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
								<view class="label">输入类型</view>
								<view class="value">{{row.type}}</view>
							</view>
							<view class="num">
								{{index+1}}
							</view> -->
							<uni-forms-item required :id="row.code+'_form'" :showLabel="false" contentFont="32rpx"
								:name="row.code">
								<view class="li_info" :id="row.code">
									<!-- <view class="label">值</view> -->
									<view class="value input_val">
										<uni-easyinput type="number" :placeholder="row.text"
											v-model="paramsForm[row.code]" clearSize="18"
											placeholderStyle="color:#C1C1C1);font-size:32rpx" :styles="customstyles"
											primaryColor="#2371FF" inputHeight="88rpx" :isCustom="true"
											contentFontSize="32rpx" :inputBorder="false" v-if="row.type=='int'"
											:disabled="row.readOnly">
										</uni-easyinput>
										<uni-easyinput type="digit" :placeholder="row.text"
											v-model="paramsForm[row.code]" clearSize="18"
											placeholderStyle="color:#C1C1C1;font-size:32rpx" :styles="customstyles"
											primaryColor="#2371FF" inputHeight="88rpx" :isCustom="true"
											contentFontSize="32rpx" :inputBorder="false" v-else-if="row.type=='float'"
											:disabled="row.readOnly">
										</uni-easyinput>
										<uni-datetime-picker ref="dateChoice2" class="date" type="date"
											:clear-icon="false" v-model="paramsForm[row.code]" placeholder='结束日期'
											:isCustom="true" v-else-if="row.type=='date'" :disabled="row.readOnly">
											<view class="date_slot"
												:class="{'has_val':paramsForm[row.code],'dis_val':row.readOnly}">
												<custom-icons iconsName="icon-xuanzeshijian" iconsSize="36rpx"
													:iconsColor="row.readOnly?'#c1c1c1':'#999999'"></custom-icons>
												<view class="text">
													{{paramsForm[row.code]?paramsForm[row.code]:row.text}}
												</view>
											</view>
										</uni-datetime-picker>
										<view class="switch_con" v-else-if="row.type=='boolean'"
											:class="row.readOnly?'disable_switch':''">
											<view class="switch_text" :class="!paramsForm[row.code]?'active_text':''">假
											</view>
											<switch @change="switch2Change(row.code)" :disabled="row.readOnly"
												:checked="paramsForm[row.code]"
												style="transform:scale(0.9);width: 120rpx;" />
											<view class="switch_text" :class="paramsForm[row.code]?'active_text':''"> 真
											</view>
										</view>
										<uni-data-select v-else-if="row.type=='enum'" v-model="paramsForm[row.code]"
											:localdata="returnEnumList(row)" width="100%" :placeholder="row.text"
											borderColor="rgba(255, 255, 255, 0.20)" palColor="rgba(193, 193, 193, 1)"
											:isCustom="true" :isDark="false" :disabled="row.readOnly"></uni-data-select>
										<uni-easyinput type="text" :placeholder="row.text"
											v-model="paramsForm[row.code]" clearSize="18"
											placeholderStyle="color:#C1C1C1;font-size:32rpx" :styles="customstyles"
											primaryColor="rgba(255, 255, 255, 0.5)" inputHeight="88rpx" :isCustom="true"
											contentFontSize="32rpx" :inputBorder="false" v-else
											:disabled="row.readOnly">
										</uni-easyinput>
									</view>
								</view>
							</uni-forms-item>
							<view class="li_info">
								<view class="remark_con" v-if="row.remark">
									备注：{{row.remark}}
								</view>
							</view>
						</view>
						<uni-load-more iconType="circle" status="noMore"
							v-if="!implementParamsForm.inputsData||implementParamsForm.inputsData.length==0" />
					</view>
					<view class="btn_con_"></view>
					<view class="btn_con">
						<view class="cancel_btn btn_li" @click="close()" :class="{'disable_li':matchesLoading}">
							<uni-icons type="spinner-cycle" size="18" v-if="matchesLoading"></uni-icons>取消
						</view>
						<view class="execute_btn btn_li" @click="carryAction()"
							:class="{'disable_li':activeFunIsDisable||matchesLoading}">
							<uni-icons type="spinner-cycle" size="18" color="#fff" v-if="matchesLoading"></uni-icons>执行
						</view>
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
			id: { //设备id
				type: String,
				default: ""
			},
			deviceId: { //传通讯id
				type: String,
				default: ""
			},
			deviceBasic: {
				type: Object,
				default: ''
			}
		},
		data() {
			return {
				customstyles: {
					color: '#333333',
					backgroundColor: '#F8F8F8',
					disableColor: '#F7F6F6',
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
				popScollTop: 0,
				status: 'loading',
				isOnLine: false,
				activeFunIsDisable: false,
				activeFuncIndex: null,
			};
		},
		watch: {
			deviceBasic: {
				handler(newName, oldName) {
					this.$nextTick(() => {
						if (this.deviceBasic.Online == 0) {
							this.isOnLine = false
						} else if (this.deviceBasic.Online == 1) {
							this.isOnLine = true
						} else {
							this.isOnLine = false
						}
						this.getdeviceFuncList()
						if (this.deviceId) {
							this.initMQTT();
						}
					})
				},
				// 代表在wacth里声明了firstName这个方法之后立即先去执行handler方法
				immediate: true,
				deep: true,
			},
		},
		computed: {
			activeRules() {
				return this.implementParamsForm.paramsRules
			}
		},
		methods: {
			returnEnumList(row) {
				console.log("row", row);
				let arr = []
				for (let key in row.elements) {
					let obj = {
						text: key,
						value: row.elements[key]
					}
					arr.push(obj)
				}
				return arr
			},
			switch2Change(code) {
				this.paramsForm[code] = !this.paramsForm[code]
				// console.log(this.paramsForm,'uuuuuuu');
			},
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
									if (msgtxt.indexOf('update') > -1) {
										that.getdeviceFuncList()
									}
									if (msgtxt.indexOf('redirect') > -1) {
										let urlStr = msgtxt.substring(8, msgtxt.length);
										that.jumpFuncUrl(urlStr)
									}

									if (msgtxt.indexOf('show') > -1) {
										let msgStr = msgtxt.substring(4, msgtxt.length);
										that.attrData[that.activeFuncIndex].returnMsg = msgStr
										that.$forceUpdate()
									}
								},
							});
						}
					});
				});
			},
			jumpFuncUrl(url) {
				uni.navigateTo({
					url: '/pages/webview/webview?url=' + url
				})
			},
			getdeviceFuncList() {
				//获取设备功能列表
				deviceFuncList({
					id: this.id
				}).then((res) => {
					this.attrData = res.data;
					this.status = 'noMore'
				});
			},
			close() {
				this.$refs.funcPop.close()
			},
			openFuncPop(row, inx) {
				//打开功能执行弹窗
				this.implementParamsForm.functionId = row.code;
				this.activeFuncIndex = inx
				this.activeFunIsDisable = row.disabled

				let paramsForm = {}
				if (this.activeFunIsDisable) {
					return
				}
				if (row.inputs && row.inputs.length > 0) {
					for (let i = 0; i < row.inputs.length; i++) {
						// console.log(row, 'row.inputs[i]');
						row.inputs[i].text = "请输入 " + row.inputs[i].name;
						if (row.inputs[i].readOnly) {
							row.inputs[i].text = "此参数为只读"
						}
						if (row.inputs[i].type == "int" || row.inputs[i].type == "float") {
							// row.inputs[i].codeVal = null;
							paramsForm[row.inputs[i].code] = null
							if (row.inputs[i].defval || row.inputs[i].defval == 0) { //设置默认值
								paramsForm[row.inputs[i].code] = row.inputs[i].defval
							}
						} else {
							// row.inputs[i].codeVal = "";
							paramsForm[row.inputs[i].code] = ''
							if (row.inputs[i].defval || row.inputs[i].defval == 0 || row.inputs[i].defval != null && row
								.inputs[i].defval != undefined) {
								if (row.inputs[i].type == "boolean" && row.inputs[i].defval == '') {
									paramsForm[row.inputs[i].code] = false
								} else {
									paramsForm[row.inputs[i].code] = row.inputs[i].defval
								}

							} else {
								if (row.inputs[i].type == "boolean" && row.inputs[i].defval == null) {
									paramsForm[row.inputs[i].code] = false
								}
							}
						}
						this.paramsRules[row.inputs[i].code] = {}
						this.paramsRules[row.inputs[i].code].rules = [{
							required: true,
							errorMessage: '请输入 ' + row.inputs[i].name,
						}]
					}
					this.paramsForm = JSON.parse(JSON.stringify(paramsForm))
					// console.log("this.paramsRules", this.paramsRules);
					let lis = JSON.parse(JSON.stringify(row.inputs));
					this.implementParamsForm.inputsData = JSON.parse(JSON.stringify(lis));
					this.resetForm("paramsForm");
					this.$refs.funcPop.open()
				} else {
					this.noInputCarryAction()
				}

			},
			carryAction() {
				this.$forceUpdate()
				if (!this.matchesLoading) {
					if (this.$refs["paramsForm"]) {
						this.$refs["paramsForm"].validate().then(valid => {
							// console.log(valid, 'valid');
							this.matchesLoading = true;
							for (let keys in this.paramsForm) { //将整型数据转换
								let keysInfo = this.implementParamsForm.inputsData.find((x) => x.code == keys);
								if (keysInfo && keysInfo.type == 'int') {
									this.paramsForm[keys] = parseInt(this.paramsForm[keys])
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
										this.$refs.funcPop.close()
										this.matchesLoading = false;
										this.$refs.promptMsg.open('执行完成', 2000)
									}
								})
								.catch((err) => {
									this.matchesLoading = false;
									// console.log("报错", err);
									if (err.message) {
										this.$refs.promptMsg.noticeOpen(err.message)
									}
								});
						}).catch((err) => {
							// console.log("报错", err);
							if (err && err.length > 0) {
								let firstErr = "#" + err[0].key
								const query = uni.createSelectorQuery().in(this);
								query.select(firstErr).boundingClientRect(data => {
									this.popScollTop = data.top - 200

								}).exec();
							}
						});
					}
				}

			},
			noInputCarryAction() {
				if (this.matchesLoading) {
					return
				}
				this.matchesLoading = true
				deviceExeFunc({
						id: this.id,
						functionId: this.implementParamsForm.functionId,
						inputs: {},
					})
					.then((res) => {
						// console.log(res);
						if (res.code == 0) {
							this.$refs.funcPop.close()
							this.matchesLoading = false;
							this.$refs.promptMsg.open('执行完成', 2000)
						}
					})
					.catch((err) => {
						this.matchesLoading = false;
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
			padding: 22rpx 30rpx 24rpx;
			display: flex;
			justify-content: space-between;
			align-items: center;
			color: #333333;
			font-weight: 550;
			border-radius: 10rpx;
			margin-top: 20rpx;
			background-color: #FFFFFF;
			font-size: 30rpx;

			&.disable_class {
				opacity: 0.6;
			}

			.li_left {
				width: calc(100% - 182rpx);

				.left_description {
					font-size: 24rpx;
					line-height: 36rpx;
					color: rgba(153, 153, 153, 1);
					font-weight: normal;
				}

				.left_name {
					line-height: 46rpx;
				}
			}

			.li_right {
				padding-left: 52rpx;
				padding-right: 22rpx;
				width: 152rpx;
				height: 80rpx;
				box-sizing: border-box;
				display: flex;
				flex-direction: column;
				justify-content: center;
				align-items: center;
				color: #999999;
				border-left: 1rpx solid #EAEAEA;

				.btn_text {
					font-weight: normal;
					font-size: 24rpx;
					margin-top: 12rpx;
					height: 24rpx;
					display: flex;
					justify-content: center;
					align-items: center;
					flex-wrap: nowrap;
					white-space: nowrap;
				}


			}
		}
	}

	.pop_con {
		width: 100%;
		background-color: #F8F8F8;
		position: relative;
		padding: 23rpx 20rpx;
		box-sizing: border-box;
		border-radius: 20rpx 20rpx 0 0;
		max-height: 70vh;
		overflow-y: scroll;

		.pop_title {
			width: calc(100% + 40rpx);
			color: #333333;
			text-align: center;
			font-size: 34rpx;
			line-height: 50rpx;
			font-weight: bold;
			position: fixed;
			z-index: 1000;
			padding: 23rpx 0 33rpx;
			background-color: #F8F8F8;
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
					background: #2371FF;
					color: rgba(255, 255, 255, 1);
				}

				&.cancel_btn {
					background-color: #FFFFFF;
					color: #999999;
				}

				&.disable_li {
					opacity: 0.6;
				}
			}
		}

		.func_input_list {
			width: 100%;
			overflow-y: auto;
			padding-bottom: 100px;
			.input_li {
				position: relative;
				width: 100%;
				padding: 30rpx;
				box-sizing: border-box;
				background-color: #FFFFFF;
				border-radius: 10rpx;
				margin-top: 20rpx;

				.li_info {
					// padding-top: 30rpx;
					// border-top: 1rpx solid #EAEAEA;
					// margin-top: 30rpx;
					font-size: 32rpx;

					.label {
						color: #999999;
						font-size: 28rpx;
						line-height: 28rpx;
					}

					.value {
						color: #333333;
						margin-top: 20rpx;

						&.input_val {
							background-color: #F8F8F8;
							border-radius: 10rpx;
						}

						.switch_con {
							display: flex;
							justify-content: flex-start;
							align-items: center;

							.switch_text {
								font-size: 28rpx;
								margin-left: 10rpx;
								margin-right: 10rpx;

								&.active_text {
									color: #2371FF;
								}
							}

							&.disable_switch {
								opacity: 0.6;
							}

							::v-deep .uni-switch-input {
								width: 120rpx;

							}

							::v-deep uni-switch .uni-switch-input:before {
								width: 0;
							}

							::v-deep uni-switch .uni-switch-input.uni-switch-input-checked:after {
								-webkit-transform: translateX(60rpx);
								transform: translateX(60rpx);
							}
						}
					}

					.remark_con {
						font-size: 24rpx;
						line-height: 36rpx;
						color: rgba(153, 153, 153, 1);
					}
				}

				.num {
					display: flex;
					justify-content: center;
					align-items: center;
					background-color: #E9F1FF;
					color: #2371FF;
					border-radius: 0 10rpx 0 10rpx;
					// border: 1rpx solid rgba(255, 255, 255, 0.2);
					position: absolute;
					right: 0;
					top: 1rpx;
					width: 50rpx;
					height: 40rpx;
				}
			}
		}
	}

	.date_slot {
		display: flex;
		justify-content: flex-start;
		align-items: center;
		height: 88rpx;
		width: 100%;
		padding: 0 20rpx;
		box-sizing: border-box;
		color: #C1C1C1;

		&.has_val {
			color: #333333;
		}

		.text {
			margin-left: 20rpx;
		}

		&.dis_val {
			background-color: #f8f8f8;
			color: #C1C1C1;
			// border: 1rpx solid rgba(193, 193, 193,1);
		}
	}
</style>