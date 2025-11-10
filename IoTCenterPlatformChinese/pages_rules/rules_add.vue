<template>
	<view class="pages_bgcon">
		<top :title="topTitle" leftWidth="157rpx" leftIcon="icon-fanhui" backgroundColor="#F5F8F9" rightWidth="157rpx"
			:isleftBack="true" :rightText="getTopRightTxt()" @clickRight="setCouldEdit"></top>

		<view class="rules_con">
			<view class="rules_name_input" v-if="rulesId&&isCouldEditInfo">
				<uni-easyinput placeholderStyle="color:#C1C1C1;font-size:32rpx" inputHeight="88rpx" :styles="styles"
					type="text" v-model="rulesName" placeholder="请输入规则名称" contentFontSize="32rpx" />
			</view>
			<view class="rules_name_input" v-else-if="rulesId">
				<view class="input_text">
					{{rulesName}}
				</view>
			</view>
			<view class="rules_type_list">
				<view class="type_title">
					当以下情况发生
				</view>
				<view :key="returnConditionKey(ruleTypeKey,inx)" class="type_li" v-for="(ite,inx) in allRulesList" @click="viewRulesInfo(ite,ite.attrInx,inx,'condition')"
					:style="{'opacity':rulesAdForm.id&&ite.isEvent?0.6:1}"
					v-if="ite.isAttr||!ite.isFunc&&ite.isEvent||ite.isFixedTime">
					<view class="li_left">
						<view class="image t-icon-dingshinaozhong" v-if="ite.isFixedTime"></view>
						<view class="image t-icon-shijian" v-else-if="!ite.isFunc&&ite.isEvent"></view>
						<view class="image t-icon-shuxing" v-else="ite.isAttr"></view>
						<view class="li_cot">
							<view class="name">
								{{ite.deviceName}}
							</view>
							<view class="text">
								{{ite.isAttr?ite.attrtName:ite.eventName}}
							</view>
						</view>
					</view>
					<view class="li_right">
						<custom-icons iconsName="icon-a-youjiantoubai" iconsSize="20rpx"
							iconsColor="#999999"></custom-icons>
					</view>
					<view class="del_con" v-if="isCouldEditInfo&&!ite.isEvent&&!ite.isFixedTime"
						@click.stop="deleteAttr(ite,ite.attrInx,inx)">
						<view class="image t-icon-yichu1"></view>
					</view>
				</view>
				<view class="add_con" @click="openPopup(0)" v-if="isCouldEditInfo">
					<view class="icon_con">
						<custom-icons iconsName="icon-tianjia" iconsSize="18rpx" iconsColor="#2371FF"></custom-icons>
					</view>
					<view class="add_text">
						添加触发条件
					</view>
				</view>
			</view>
			<view class="rules_type_list">
				<view class="type_title">
					将设备调整到
				</view>
				<view :key="returnConditionKey(ruleTypeKey,inx)" class="type_li" v-for="(ite,inx) in allRulesList" @click="viewRulesInfo(ite,ite.funcInx,inx,'function')"
					v-if="ite.isFunc">
					<view class="li_left">
						<view class="image t-icon-gongneng11" v-if="ite.isFunc&&!ite.isEvent"></view>
						<view class="image t-icon-shijian1" v-else-if="ite.isFunc&&ite.isEvent"></view>
						<view class="li_cot">
							<view class="name">
								{{ite.deviceName}}
							</view>
							<view class="text">
								{{ite.funcName}}
							</view>
						</view>
					</view>
					<view class="li_right">
						<custom-icons iconsName="icon-a-youjiantoubai" iconsSize="20rpx"
							iconsColor="#999999"></custom-icons>
					</view>
					<view class="del_con" @click.stop="deleteFunc(ite,ite.funcInx,inx)" v-if="isCouldEditInfo">
						<view class="image t-icon-yichu1"></view>
					</view>
				</view>
				<view class="add_con" @click="openPopup(1)" v-if="isCouldEditInfo">
					<view class="icon_con">
						<custom-icons iconsName="icon-tianjia" iconsSize="18rpx" iconsColor="#2371FF"></custom-icons>
					</view>
					<view class="add_text">
						添加执行动作
					</view>
				</view>
			</view>
			<view class="qiyong_con" v-if="rulesId">
				<view class="label">是否启用</view>
				<view class="cot_right">
					<view class="conten_icon t-icon-kai" v-if="rulesStatic==0" @click.stop="setStatus('1')"></view>
					<view class="conten_icon t-icon-guan1" v-else-if="rulesStatic==1" @click.stop="setStatus('0')">
					</view>
				</view>
			</view>
			<view class="zhanwei"></view>
			<view class="botton_con">
				<button class="submit_button" @click="submit" :disabled="isLoading||isdisable||isLoad"
					:style="{'opacity':isLoading||isdisable||isLoad?0.5:1}" :loading="isLoading" v-if="isCouldEditInfo">
					{{rulesId?'保存':'创建'}}
				</button>
				<button class="submit_button huise_btn" @click="delrule" :disabled="isLoading||isdisable||isLoad"
					:style="{'opacity':isLoading||isdisable||isLoad?0.5:1}" :loading="isLoading"
					v-if="!isCouldEditInfo">
					删除规则
				</button>
			</view>
		</view>
		<uni-popup ref="typePopup" type="bottom" :mask-click="true" :zIndex="998">
			<view class="popup_con">
				<view class="popup_title">
					{{popupTitle}}
				</view>
				<view class="choice_type" @click="choiceDevice('event','condition')" v-if="addType=='condition'">
					<view class="choice_left">
						<view class="image t-icon-xuanzeshebei"></view>
						<view class="name">
							设备
						</view>
					</view>
					<view class="choice_right">
						<custom-icons iconsName="icon-a-youjiantoubai" iconsSize="20rpx"
							iconsColor="#999999"></custom-icons>
					</view>
				</view>
				<view class="choice_type" @click="choiceDevice('function','function')" v-if="addType=='function'">
					<view class="choice_left">
						<view class="image t-icon-gongneng11"></view>
						<view class="name">
							功能
						</view>
					</view>
					<view class="choice_right">
						<custom-icons iconsName="icon-a-youjiantoubai" iconsSize="20rpx"
							iconsColor="#999999"></custom-icons>
					</view>
				</view>
				<view class="choice_type" @click="choiceDevice('event','function')" v-if="addType=='function'">
					<view class="choice_left">
						<view class="image t-icon-shijian1"></view>
						<view class="name">
							事件
						</view>
					</view>
					<view class="choice_right">
						<custom-icons iconsName="icon-a-youjiantoubai" iconsSize="20rpx"
							iconsColor="#999999"></custom-icons>
					</view>
				</view>
				<view class="choice_type" @click="toTimeTigger"
					v-if="addType=='condition'&&conditionRulesList.length==0">
					<view class="choice_left">
						<view class="image t-icon-dingshinaozhong"></view>
						<view class="name">
							定时
						</view>
					</view>
					<view class="choice_right">
						<custom-icons iconsName="icon-a-youjiantoubai" iconsSize="20rpx"
							iconsColor="#999999"></custom-icons>
					</view>
				</view>
				<button class="submit_button yellow_btn" @click="closePup">
					取消
				</button>
			</view>
		</uni-popup>
		<msg-prompt ref="promptMsg"></msg-prompt>
	</view>
</template>

<script>
	import {
		productInfo,
		getRuselDetail,
		addRuselServe,
		editRuselServe,
		delRusel,
		toCronDes
	} from "@/api/ruselSevic.js";
	import {
		crmDeviceList
	} from "@/api/device.js"
	import {
		setPagesParam
	} from '@/common/utillib.js'
	import {
		crmDeviceInfo
	} from '@/api/device.js'
	export default {
		data() {
			return {
				ruleTypeKey:1,
				isCouldEditInfo: true,
				styles: {
					color: '#333',
					backgroundColor: '#ffffff',
					disableColor: '#F8F8F8',
					borderColor: '#ffffff'
				},
				isLoad: false,
				topTitle: '添加规则',
				isLoading: false,
				// isdisable: true,
				popupTitle: '',
				conditionRulesList: [],
				functionRulesList: [],
				allRulesList: [], //所有的规则
				addType: '',
				rulesAdForm: {}, //保存规则时的数据
				deviceDtuId: [],
				topicMsgList: [{
						value: "Offline",
						label: "设备离线",
						icon: "el-icon-close"
					},
					{
						value: "Online",
						label: "设备在线",
						icon: "el-icon-check"
					},
					// { value: "Upgrade", label: "更新固件" },
					{
						value: "ReadPropertyReply",
						label: "属性上报",
						icon: "el-icon-upload2"
					},
					{
						value: "Event",
						label: "设备事件",
						icon: "el-icon-data-line"
					}
				],
				attrInx: 0,
				funcInx: 0,
				productmap: new Map(),
				devicemap: new Map(),
				deviceDtuIdmap: new Map(),
				rulesId: 0,
				conditionsNodeId: 0,
				conditionNodeId: 0,
				emptyNodeId: 0,
				isShouldEdit: false, //是否是编辑状态
				rulesName: '', //规则名称
				rulesStatic: '',
			};
		},
		computed: {
			isdisable() {
				return this.functionRulesList.length == 0 || this.conditionRulesList.length == 0
			}
		},
		onLoad(options) {
			var pages = getCurrentPages();
			this.$store.commit('SET_RULESADD_INFO', {})
			if (options.id) {
				this.isCouldEditInfo = false
				this.rulesId = options.id
				this.loadFormInfo(options.id)
			}
		},
		methods: {
			returnConditionKey(key,inx){
				return key+inx
			},
			setStatus(val) {
				//设置规则启用状态
				if (this.isCouldEditInfo) {
					this.rulesStatic = val
				}
			},
			getTopRightTxt() {
				if (this.rulesId) {
					if (this.isCouldEditInfo) {
						return '取消'
					} else {
						return '编辑'
					}
				}
			},
			setCouldEdit() {
				//设置是否可以编辑
				if (this.rulesId) {
					this.isCouldEditInfo = !this.isCouldEditInfo
				}
			},
			delrule() {
				//删除规则
				this.isLoading = true
				uni.showModal({
					title: '警告',
					content: '是否确认删除名称为"' + this.rulesName + '"的规则?',
					showCancel: true,
					success: (res) => {
						if (res.confirm) {
							delRusel(this.rulesId).then((rsp) => {
									if (rsp.code == 0) {
										this.$refs.promptMsg.open('删除成功', 2000) //提示信息组件
										setTimeout(() => {
											setPagesParam('loadData', 'load', 1)
										}, 2000);
									}
								})
								.catch((err) => {
									this.setMsgTop(err)
									this.isLoading = false
								});
						} else {
							this.isLoading = false
						}
					},
					fail: () => {
						this.isLoading = false
					}
				});
			},
			async finishFixedTime(obj) {
				//完成定时
				if (obj.str) {
					let httpParams = [{
						"name": "累计时间（秒）",
						"code": "TimeDelta",
						"type": "int",
						"defval": 0
					}, {
						"name": "触发间隔（秒）",
						"code": "TriggerDelta",
						"type": "int",
						"defval": 0
					}];
					let firstObj = {}
					let rulesAdForm = this.$store.state.rulesAddForm
					let rulesName = ''
					if (obj.str.indexOf('/') > -1) {
						let arr = obj.str.split(' ')
						let eventNa = ''
						if (arr[0].indexOf('/') > -1) {
							let arr2 = arr[0].split("/")
							eventNa = "间隔" + arr2[1] + '秒'
						}
						if (arr[1].indexOf('/') > -1) {
							let arr2 = arr[1].split("/")
							eventNa = "间隔" + arr2[1] + '分钟'
						}
						if (arr[2].indexOf('/') > -1) {
							let arr2 = arr[2].split("/")
							eventNa = "间隔" + arr2[1] + '时'
						}
						firstObj = {
							deviceName: '定时',
							eventName: eventNa,
							cannotEdit: false,
							isFixedTime: true,
							attrInx: 0,
							info: {
								model: obj.str
							}
						}
						rulesName = '定时触发 ' + eventNa
					} else {
						// let arr = obj.str.split(' ')
						// let h = arr[2].length == 2 ? arr[2] : '0' + arr[2]
						// let m = arr[1].length == 2 ? arr[1] : '0' + arr[1]
						try {
							let x = await toCronDes(obj.str)
							firstObj = {
								deviceName: '定时',
								eventName: x.data,
								cannotEdit: false,
								isFixedTime: true,
								attrInx: 0,
								info: {
									model: obj.str
								}
							}
							rulesName = '定时触发 ' + x.data
						} catch (e) {
							//TODO handle the exception
						}

					}
					rulesAdForm.timerCron = obj.str
					rulesAdForm.ruleJson = ''
					rulesAdForm.httpParams = httpParams
					rulesAdForm.remark = ''
					rulesAdForm.debug = 1
					rulesAdForm.triggerWay = 2
					rulesAdForm.triggerList = []
					rulesAdForm.eventDevice = []
					rulesAdForm.name = rulesName
					rulesAdForm.eventinfo = {}
					rulesAdForm.info = {
						model: obj.str
					}
					if (obj.infoIndex && obj.allInx) {
						this.conditionRulesList[Number(obj.infoIndex)] = firstObj
						this.allRulesList[Number(obj.allInx)] = firstObj
					} else {
						this.conditionRulesList.push(firstObj)
						this.allRulesList.push(firstObj)
						// this.attrInx = this.attrInx + 1
					}

					this.$store.commit('SET_RULESADD_INFO', rulesAdForm)
					this.$store.commit('SET_OTHERRULES_INFO', {})
					this.$forceUpdate()
				}
				this.ruleTypeKey++
			},
			toTimeTigger() {
				//去定时触发
				this.closePup()
				// uni.navigateTo({
				// 	url: '/pages_rules/time_select'
				// })
				uni.navigateTo({
					url: '/pages_rules/time_type/time_type'
				})
			},
			deleteFunc(item, funcInx, allinx) {
				this.allRulesList.splice(Number(allinx), 1)
				this.functionRulesList.splice(Number(funcInx), 1)
				this.funcInx = this.funcInx - 1
				let rulesAdForm = this.$store.state.rulesAddForm
				rulesAdForm.funcDevice.splice(Number(funcInx), 1)
				rulesAdForm.funcInfo.splice(Number(funcInx), 1)
				this.$store.commit('SET_RULESADD_INFO', rulesAdForm)

			},
			deleteAttr(item, attrInx, allinx) {
				this.allRulesList.splice(Number(allinx), 1)
				this.conditionRulesList.splice(Number(attrInx - 1), 1)
				this.attrInx = this.attrInx - 1
				let rulesAdForm = this.$store.state.rulesAddForm
				rulesAdForm.attrDevice.splice(Number(attrInx - 1), 1)
				rulesAdForm.attrInfo.splice(Number(attrInx - 1), 1)
				this.$store.commit('SET_RULESADD_INFO', rulesAdForm)

			},
			viewRulesInfo(item, inx, inx2,addType) { //inx表示在触发条件中的序号，inx2表示在执行动作中的序号
				try {
					let proAttrType = ''
					if (item.isEvent) {
						proAttrType = 'event'
						let eventInx=0
						if(addType=='function'){//如果是功能执行的设备事件，需要赋值序号
							eventInx=inx
						}
						if (item.cannotEdit) {
							if (this.isCouldEditInfo) {
								uni.navigateTo({
									url: '/pages_device/select_list?dataType=' + proAttrType +
										'&isDisable=true&inx='+eventInx + '&allInx=' + inx2+'&rulesType='+addType
								})
							} else {
								uni.navigateTo({
									url: '/pages_device/select_list?dataType=' + proAttrType +
										'&isDisable=true&inx='+eventInx + '&allInx=' + inx2 + '&isOnlyRead=true'+'&rulesType='+addType
								})
							}

						} else {
							if (this.isCouldEditInfo) {
								uni.navigateTo({
									url: '/pages_device/select_list?isNotReturn=' + true + '&dataType=' +
										proAttrType + '&inx='+eventInx + '&allInx=' + inx2+'&rulesType='+addType
								})
							} else {
								uni.navigateTo({
									url: '/pages_device/select_list?isNotReturn=' + true + '&dataType=' +
										proAttrType + '&inx='+eventInx + '&allInx=' + inx2 + '&isOnlyRead=true'+'&rulesType='+addType
								})
							}
						}

					} else if (item.isAttr) {
						proAttrType = 'attribute'

						if (item.cannotEdit) {
							if (this.isCouldEditInfo) {
								uni.navigateTo({
									url: '/pages_device/select_list?dataType=' + proAttrType + '&inx=' + Number(
										inx) + '&allInx=' + inx2+ '&isNotReturn=true' 
								})
							} else {
								uni.navigateTo({
									url: '/pages_device/select_list?dataType=' + proAttrType + '&inx=' + Number(
										inx) + '&isDisable=true' + '&allInx=' + inx2 + '&isOnlyRead=true'
								})
							}

						} else {
							if (this.isCouldEditInfo) {
								uni.navigateTo({
									url: '/pages_device/select_list?dataType=' + proAttrType + '&inx=' + Number(
										inx) + '&allInx=' + inx2 + '&isNotReturn=true' 
								})
							} else {
								uni.navigateTo({
									url: '/pages_device/select_list?dataType=' + proAttrType + '&inx=' + Number(
										inx) + '&allInx=' + inx2 + '&isDisable=true' + '&isOnlyRead=true'
								})
							}

						}

					} else if (item.isFunc) {
						proAttrType = 'function'
						if (this.isCouldEditInfo) {
							uni.navigateTo({
								url: '/pages_device/select_list?dataType=' + proAttrType + '&inx=' + Number(inx) +
									'&allInx=' + inx2 + '&isNotReturn=true' 
							})
						} else {
							uni.navigateTo({
								url: '/pages_device/select_list?dataType=' + proAttrType + '&inx=' + Number(inx) +
									'&allInx=' + inx2 + '&isNotReturn=true' + '&isDisable=true' +
									'&isOnlyRead=true'
							})
						}


					} else if (item.isFixedTime) {
						if (this.isCouldEditInfo) {
							uni.navigateTo({
								url: '/pages_rules/time_type/time_type?cronStr=' + item.info.model + '&inx=' +
									Number(inx) + '&allInx=' + inx2
							})
						} else {
							uni.navigateTo({
								url: '/pages_rules/time_type/time_type?cronStr=' + item.info.model + '&inx=' +
									Number(inx) + '&allInx=' + inx2 + '&isOnlyRead=true'
							})
						}

					}
				} catch (e) {
					//TODO handle the exception
					console.log(e, 'eeeeeeeee');
				}
			},
			async loadFormInfo(rulseId) {
				//如果是编辑规则流程，则显示已有规则
				uni.showLoading({
					title: '加载中'
				})
				this.isLoad = true
				try {

					let rsp = await getRuselDetail({
						id: rulseId
					});
					let rulse = {};
					rulse.id = rsp.data.Id;
					rulse.ruleJson = JSON.parse(rsp.data.RuleJson);
					rulse.name = rsp.data.Name;
					this.rulesName = rsp.data.Name
					this.rulesStatic = rsp.data.Status
					rulse.sort = rsp.data.Sort;
					rulse.remark = rsp.data.Remark;
					rulse.triggerWay = rsp.data.TriggerWay;
					rulse.triggerList = rsp.data.TriggerList;
					rulse.createdFrom = rsp.data.CreatedFrom
					if (rsp.data.HttpParams) {
						rulse.httpParams = JSON.parse(rsp.data.HttpParams)
					}

					rulse.timerCron = rsp.data.TimerCron
					let tmptsl = null;
					let producInfo = null
					let productId = ''

					if (rulse.triggerList.length > 0) {
						this.product = rulse.triggerList[0].TopicDevice.split("/")[1];
						productId = this.product;
						for (let j = 0; j < rulse.triggerList.length; j++) {
							let spLs = rulse.triggerList[j].TopicDevice.split("/");
							this.deviceDtuId.push(spLs[2]);
						}
						await this.initDeviceMap()
						tmptsl = await this.getProductAttr(productId);
					}
					if (rulse.triggerWay == 0) {
						if (rulse.triggerList[0].TopicMsg.indexOf("Event") == 0 && tmptsl != null) {
							let tmpccc = rulse.triggerList[0].TopicMsg.substr(6);
							let tmpeeee = tmptsl.events.filter(x => x.code == tmpccc);
							// console.log("设备",tmpeeee);
							let firstObj = {
								deviceName: this.deviceDtuIdmap.get(this.deviceDtuId[0]) ? this.deviceDtuIdmap
									.get(this.deviceDtuId[0]).Name : '没有获取到设备信息',
								eventName: tmpeeee[0].name,
								cannotEdit: true,
								isEvent: true,
								attrInx: this.attrInx,
								info: {
									deviceInfo: this.deviceDtuIdmap.get(this.deviceDtuId[0]),
									model: tmpeeee[0]
								}
							}
							let arr = []
							arr.push(this.deviceDtuIdmap.get(this.deviceDtuId[0]))
							rulse.eventDevice = arr
							rulse.eventinfo = tmpeeee[0]
							this.allRulesList.push(firstObj)
							this.conditionRulesList.push(firstObj)
							// this.attrInx = this.attrInx + 1
							this.$store.commit('SET_RULESADD_INFO', rulse)
						}
					} else if (rulse.triggerWay == 2) {
						let arr = rulse.timerCron.split(' ')
						let firstObj = {}
						if (rulse.timerCron.indexOf('/') > -1) {
							let eventNa = ''
							if (arr[0].indexOf('/') > -1) {
								let arr2 = arr[0].split("/")
								eventNa = "间隔" + arr2[1] + '秒'
							}
							if (arr[1].indexOf('/') > -1) {
								let arr2 = arr[1].split("/")
								eventNa = "间隔" + arr2[1] + '分钟'
							}
							if (arr[2].indexOf('/') > -1) {
								let arr2 = arr[2].split("/")
								eventNa = "间隔" + arr2[1] + '时'
							}
							firstObj = {
								deviceName: '定时',
								eventName: eventNa,
								cannotEdit: false,
								isFixedTime: true,
								attrInx: 0,
								info: {
									model: rulse.timerCron
								}
							}
						} else {
							try {
								let x = await toCronDes(rulse.timerCron)
								firstObj = {
									deviceName: '定时',
									eventName: x.data,
									cannotEdit: false,
									isFixedTime: true,
									attrInx: 0,
									info: {
										model: rulse.timerCron
									}
								}
							} catch (e) {
								//TODO handle the exception
							}

						}

						this.allRulesList.push(firstObj)
						this.conditionRulesList.push(firstObj)
						// this.attrInx = this.attrInx + 1
						this.$store.commit('SET_RULESADD_INFO', rulse)
					}

					this.setOtherConditionRles(rulse.ruleJson)

					this.rulesAdForm = JSON.parse(JSON.stringify(rulse))
					this.$forceUpdate();
					// uni.hideLoading()
					this.ruleTypeKey++
				} catch (e) {
					//TODO handle the exception
					if (this.isLoad) {
						this.isLoad = false
						uni.hideLoading()
					}

					// console.log(e, 'eeeeeeeeeeeeeeeeee');
				}
			},
			async setOtherConditionRles(Process) {
				try {
					if (Process.type == "CONDITIONS") {
						this.conditionsNodeId = Process.id //条件总节点id
						Process.branchs.map(row => {
							this.conditionNodeId = row.id //条件分支节点id
							row.props.groups.map(it => {
								it.conditions.map(async item => {
									// console.log("条件", item);
									let arr = item.code.split('.')
									let deviceObj = {
										Id: arr[1],
										Name: item.gname
									}
									let deviceInfo = this.devicemap.get(deviceObj.Id)
									if (deviceInfo) {

									} else {
										deviceInfo = await this.getDeviceInfo(deviceObj.Id)
									}
									let obj = {
										deviceName: item.gname,
										attrtName: item.title,
										cannotEdit: false,
										isAttr: true,
										attrInx: this.attrInx,
										info: {
											deviceInfo: deviceInfo,
											model: item
										}
									}
									this.allRulesList.push(obj)
									this.conditionRulesList.push(obj)
									this.attrInx = this.attrInx + 1
									let rulesAdForm = this.$store.state.rulesAddForm
									if (rulesAdForm.attrDevice) {
										rulesAdForm.attrDevice.push(deviceObj)
									} else {
										rulesAdForm.attrDevice = []
										rulesAdForm.attrDevice.push(deviceObj)
									}
									if (rulesAdForm.attrInfo) {
										rulesAdForm.attrInfo.push(item)
									} else {
										rulesAdForm.attrInfo = []
										rulesAdForm.attrInfo.push(item)
									}
									this.$store.commit('SET_RULESADD_INFO', rulesAdForm)
								})
							})
						})
					} else if (Process.type == "FUNC") { //功能节点
						let jsonLis = {}
						let deviceInfo = {};
						if (Process.props && Process.props.TargetId) {
							deviceInfo = this.devicemap.get(Process.props.TargetId)
							if (deviceInfo) {

							} else {
								deviceInfo = await this.getDeviceInfo(Process.props.TargetId)
							}
							let proInfo = {}
							if (deviceInfo.ProductId) {
								proInfo = this.productmap.get(deviceInfo.ProductId)
								if (proInfo) {
									jsonLis = JSON.parse(proInfo.ModelTSL);
								} else {
									jsonLis = await this.getProductAttr(deviceInfo.ProductId)
								}
							}
						}
						// console.log('规则设备信息',deviceInfo);
						if (jsonLis.functions) {
							for (let i = 0; i < jsonLis.functions.length; i++) {
								let its = jsonLis.functions[i]
								if (its.code == Process.props.FunctionId) {
									let obj = {
										nodeId: Process.id,
										nodePartId: Process.parentId,
										deviceName: deviceInfo.Name,
										funcName: its.name,
										cannotEdit: false,
										isFunc: true,
										funcInx: this.funcInx,
										info: {
											deviceInfo: deviceInfo,
											model: its
										}
									}
									this.allRulesList.push(obj)
									this.functionRulesList.push(obj)
									this.funcInx = this.funcInx + 1
									let rulesAdForm = this.$store.state.rulesAddForm
									if (rulesAdForm.funcDevice) {
										rulesAdForm.funcDevice.push(deviceInfo)
									} else {
										rulesAdForm.funcDevice = []
										rulesAdForm.funcDevice.push(deviceInfo)
									}
									if (rulesAdForm.funcInfo) {
										rulesAdForm.funcInfo.push(its)
									} else {
										rulesAdForm.funcInfo = []
										rulesAdForm.funcInfo.push(its)
									}
									this.$store.commit('SET_RULESADD_INFO', rulesAdForm)
								}
							}
						}
					}else if (Process.type == "WARN") { //设备事件节点
						let jsonLis = {}
						let deviceInfo = {};
						if (Process.props && Process.props.TargetId) {
							deviceInfo = this.devicemap.get(Process.props.TargetId)
							if (deviceInfo) {

							} else {
								deviceInfo = await this.getDeviceInfo(Process.props.TargetId)
							}
							let proInfo = {}
							if (deviceInfo.ProductId) {
								proInfo = this.productmap.get(deviceInfo.ProductId)
								if (proInfo) {
									jsonLis = JSON.parse(proInfo.ModelTSL);
								} else {
									jsonLis = await this.getProductAttr(deviceInfo.ProductId)
								}
							}
						}
						// console.log('规则设备信息',deviceInfo);
						if (jsonLis.events) {
							for (let i = 0; i < jsonLis.events.length; i++) {
								let its = jsonLis.events[i]
								if (its.code == Process.props.EventId) {
									let obj = {
										nodeId: Process.id,
										nodePartId: Process.parentId,
										deviceName: deviceInfo.Name,
										funcName: its.name,
										cannotEdit: false,
										isFunc: true,
										isEvent: true,
										funcInx: this.funcInx,
										info: {
											deviceInfo: deviceInfo,
											model: its
										}
									}
									this.allRulesList.push(obj)
									this.functionRulesList.push(obj)
									this.funcInx = this.funcInx + 1
									let rulesAdForm = this.$store.state.rulesAddForm
									if (rulesAdForm.funcDevice) {
										rulesAdForm.funcDevice.push(deviceInfo)
									} else {
										rulesAdForm.funcDevice = []
										rulesAdForm.funcDevice.push(deviceInfo)
									}
									if (rulesAdForm.funcInfo) {
										rulesAdForm.funcInfo.push(its)
									} else {
										rulesAdForm.funcInfo = []
										rulesAdForm.funcInfo.push(its)
									}
									this.$store.commit('SET_RULESADD_INFO', rulesAdForm)
								}
							}
						}
					} else if (Process.type == "EMPTY") {
						this.emptyNodeId = Process.id
					}
					if (Process.children && Process.children != {}) {
						this.setOtherConditionRles(Process.children)
					} else {
						if (this.isLoad) {
							this.isLoad = false
							uni.hideLoading()
						}
					}
					this.ruleTypeKey++
					// console.log('整理后的规则', this.allRulesList);
				} catch (e) {
					//TODO handle the exception
					if (this.isLoad) {
						this.isLoad = false
						uni.hideLoading()
					}
					// console.log(e, 'eeeeeeeeeeeeeeeeee');
				}
			},
			async getDeviceInfo(deviceId) {
				//获取设备详情
				try {
					let res = await crmDeviceInfo({
						id: deviceId
					})
					let curnode = res.data
					this.devicemap.set(curnode.Id, curnode);
					this.deviceDtuIdmap.set(curnode.DeviceId, curnode)
					return curnode
				} catch (e) {
					//TODO handle the exception
					console.log(e, 'eerrrrrrr');
					this.setMsgTop(e)
				}
			},
			async initDeviceMap() {
				try {
					let rsp = await crmDeviceList({
						"DeviceId": this.deviceDtuId[0],
						"showAll": true
					})
					for (let idx = 0; idx < rsp.data.List.length; idx++) {
						let curnode = rsp.data.List[idx];
						this.devicemap.set(curnode.Id, curnode);
						this.deviceDtuIdmap.set(curnode.DeviceId, curnode)
					}
				} catch (e) {
					//TODO handle the exception
					console.log(e, 'tttttttttttt');
					this.setMsgTop(e)
				}
			},
			async getProductAttr(productId) {
				try {
					let rsp = await productInfo({
						id: productId,
						notsl: false
					});
					if (rsp.code == 0) {
						this.productmap.set(rsp.data.Id, rsp.data);
						let jsonLis = JSON.parse(rsp.data.ModelTSL);
						this.$forceUpdate();
						return jsonLis;
					}
				} catch (e) {
					//TODO handle the exception
					// console.log(e, 'wwwwwwwwww');
					this.setMsgTop(e)
				}
			},
			finshSelectFunc(form) {
				//功能属性选择后
				// console.log("选择功能定义返回",form);
				if (form.infoIndex && form.allInx) {
					let rulesAdForm = this.$store.state.rulesAddForm
					let obj = JSON.parse(JSON.stringify(this.allRulesList[Number(form.allInx)]))
					obj.deviceName = form.rulesAdForm.deviceInfo.Name
					obj.funcName = form.rulesAdForm.funcValue.name
					obj.info.deviceInfo = form.rulesAdForm.deviceInfo
					obj.info.model = form.rulesAdForm.funcValue
					this.allRulesList[Number(form.allInx)] = JSON.parse(JSON.stringify(obj))
					this.functionRulesList[Number(form.infoIndex)] = JSON.parse(JSON.stringify(obj))
					rulesAdForm.funcDevice[Number(form.infoIndex)] = JSON.parse(JSON.stringify(form.rulesAdForm
						.deviceInfo))
					rulesAdForm.funcInfo[Number(form.infoIndex)] = JSON.parse(JSON.stringify(form.rulesAdForm.funcValue))

					this.$store.commit('SET_RULESADD_INFO', rulesAdForm)
					this.$forceUpdate()
				} else {
					// console.log(form.rulesAdForm.funcValue,'功能');
					let obj = {
						deviceName: form.rulesAdForm.deviceInfo.Name,
						funcName: form.rulesAdForm.funcValue.name,
						cannotEdit: false,
						isFunc: true,
						isEvent: false,
						funcInx: this.funcInx,
						info: {
							deviceInfo: form.rulesAdForm.deviceInfo,
							model: form.rulesAdForm.funcValue
						}
					}
					this.allRulesList.push(obj)
					this.functionRulesList.push(obj)
					this.funcInx = this.funcInx + 1
					let rulesAdForm = this.$store.state.rulesAddForm
					if (rulesAdForm.funcDevice) {
						rulesAdForm.funcDevice.push(form.rulesAdForm.deviceInfo)
					} else {
						rulesAdForm.funcDevice = []
						rulesAdForm.funcDevice.push(form.rulesAdForm.deviceInfo)
					}
					if (rulesAdForm.funcInfo) {
						rulesAdForm.funcInfo.push(form.rulesAdForm.funcValue)
					} else {
						rulesAdForm.funcInfo = []
						rulesAdForm.funcInfo.push(form.rulesAdForm.funcValue)
					}
					this.$store.commit('SET_RULESADD_INFO', rulesAdForm)
				}
				this.ruleTypeKey++
			},
			finshSelectAttr(form) {
				//选择完比较属性后
				console.log("属性选择返回",form);
				if (form.infoIndex && form.allInx) {
					let rulesAdForm = this.$store.state.rulesAddForm
					let deviceObj = {
						Id: form.deviceInfo.Id,
						Name: form.deviceInfo.Name
					}
					let obj = JSON.parse(JSON.stringify(this.allRulesList[Number(form.allInx)]))
					delete form.attrVal.select
					obj.deviceName = form.deviceInfo.Name
					obj.attrtName = form.attrVal.title
					obj.info.deviceInfo = form.deviceInfo
					obj.info.model = form.attrVal
					this.allRulesList[Number(form.allInx)] = JSON.parse(JSON.stringify(obj))
					this.conditionRulesList[Number(form.infoIndex)] = JSON.parse(JSON.stringify(obj))
					rulesAdForm.attrDevice[Number(form.infoIndex)] = JSON.parse(JSON.stringify(deviceObj))
					rulesAdForm.attrInfo[Number(form.infoIndex)] = JSON.parse(JSON.stringify(form.attrVal))
					this.$store.commit('SET_RULESADD_INFO', rulesAdForm)
				} else {
					let deviceObj = {
						Id: form.deviceInfo.Id,
						Name: form.deviceInfo.Name
					}
					delete form.attrVal.select
					let obj = {
						deviceName: form.deviceInfo.Name,
						attrtName: form.attrVal.title,
						cannotEdit: false,
						isAttr: true,
						attrInx: this.attrInx,
						info: {
							deviceInfo: form.deviceInfo,
							model: form.attrVal
						}
					}
					this.allRulesList.push(obj)
					this.conditionRulesList.push(obj)
					let rulesAdForm = this.$store.state.rulesAddForm
					if (rulesAdForm.attrDevice) {
						rulesAdForm.attrDevice.push(deviceObj)
					} else {
						rulesAdForm.attrDevice = []
						rulesAdForm.attrDevice.push(deviceObj)
					}
					if (rulesAdForm.attrInfo) {
						rulesAdForm.attrInfo.push(form.attrVal)
					} else {
						rulesAdForm.attrInfo = []
						rulesAdForm.attrInfo.push(form.attrVal)
					}
					// console.log("rulesAdForm",rulesAdForm);
					this.$store.commit('SET_RULESADD_INFO', rulesAdForm)
					this.attrInx = this.attrInx + 1
				}
				this.ruleTypeKey++
			},
			finshSelectEvent(form) {
				var pages = getCurrentPages();
				let rulesAdForm = this.$store.state.rulesAddForm
				if (rulesAdForm && rulesAdForm.eventDevice && rulesAdForm.eventDevice.length > 0) {
					rulesAdForm.eventDevice = form.rulesAdForm.eventDevice
					rulesAdForm.name = form.rulesAdForm.name
					rulesAdForm.eventinfo = JSON.parse(JSON.stringify(form.rulesAdForm.eventinfo))
					rulesAdForm.triggerList = form.rulesAdForm.triggerList
					rulesAdForm.info = {
						deviceInfo: form.rulesAdForm.eventDevice[0],
						model: form.rulesAdForm.eventinfo
					}
					let firstObj = {
						deviceName: form.rulesAdForm.eventDevice[0].Name,
						eventName: form.rulesAdForm.eventinfo.name,
						cannotEdit: false,
						isEvent: true,
						attrInx: 0,
						info: {
							deviceInfo: form.rulesAdForm.eventDevice[0],
							model: form.rulesAdForm.eventinfo
						}
					}
					// console.log("选择完后",rulesAdForm);
					this.conditionRulesList[Number(form.infoIndex)] = JSON.parse(JSON.stringify(firstObj))
					this.allRulesList[Number(form.allInx)] = JSON.parse(JSON.stringify(firstObj))
					this.$store.commit('SET_RULESADD_INFO', rulesAdForm)
					this.$store.commit('SET_OTHERRULES_INFO', {})
					this.$forceUpdate()
				} else {
					delete form.select
					let firstObj = {
						deviceName: form.rulesAdForm.eventDevice[0].Name,
						eventName: form.rulesAdForm.eventinfo.name,
						cannotEdit: false,
						isEvent: true,
						attrInx: 0,
						info: {
							deviceInfo: form.rulesAdForm.eventDevice[0],
							model: form.rulesAdForm.eventinfo
						}
					}
					rulesAdForm.timerCron = form.rulesAdForm.timerCron
					rulesAdForm.ruleJson = form.rulesAdForm.ruleJson
					rulesAdForm.httpParams = form.rulesAdForm.httpParams
					rulesAdForm.remark = form.rulesAdForm.remark
					rulesAdForm.debug = form.rulesAdForm.debug
					rulesAdForm.triggerWay = form.rulesAdForm.triggerWay
					rulesAdForm.triggerList = form.rulesAdForm.triggerList
					rulesAdForm.eventDevice = form.rulesAdForm.eventDevice
					rulesAdForm.name = form.rulesAdForm.name
					rulesAdForm.eventinfo = JSON.parse(JSON.stringify(form.rulesAdForm.eventinfo))
					rulesAdForm.info = {
						deviceInfo: form.rulesAdForm.eventDevice[0],
						model: form.rulesAdForm.eventinfo
					}
					this.conditionRulesList.push(firstObj)
					this.allRulesList.push(firstObj)
					this.$store.commit('SET_RULESADD_INFO', rulesAdForm)
					this.$store.commit('SET_OTHERRULES_INFO', {})
					this.$forceUpdate()
				}
				this.ruleTypeKey++

			},
			finshSelectFuncEvent(form) {
				//功能执行设备事件选择后
				// console.log("选择功能设备事件返回",form);
				if (form.infoIndex && form.allInx) {
					let rulesAdForm = this.$store.state.rulesAddForm
					let obj = JSON.parse(JSON.stringify(this.allRulesList[Number(form.allInx)]))
					obj.deviceName = form.rulesAdForm.deviceInfo.Name
					obj.funcName = form.rulesAdForm.funcValue.name
					obj.info.deviceInfo = form.rulesAdForm.deviceInfo
					obj.info.model = form.rulesAdForm.funcValue
					this.allRulesList[Number(form.allInx)] = JSON.parse(JSON.stringify(obj))
					this.functionRulesList[Number(form.infoIndex)] = JSON.parse(JSON.stringify(obj))
					rulesAdForm.funcDevice[Number(form.infoIndex)] = JSON.parse(JSON.stringify(form.rulesAdForm.deviceInfo))
					rulesAdForm.funcInfo[Number(form.infoIndex)] = JSON.parse(JSON.stringify(form.rulesAdForm.funcValue))
			
					this.$store.commit('SET_RULESADD_INFO', rulesAdForm)
					this.$forceUpdate()
				} else {
					// console.log(form.rulesAdForm.funcValue,'功能设备事件');
					let obj = {
						deviceName: form.rulesAdForm.deviceInfo.Name,
						funcName: form.rulesAdForm.funcValue.name,
						cannotEdit: false,
						isFunc: true,
						isEvent: true,
						funcInx: this.funcInx,
						info: {
							deviceInfo: form.rulesAdForm.deviceInfo,
							model: form.rulesAdForm.funcValue
						}
					}
					this.allRulesList.push(obj)
					this.functionRulesList.push(obj)
					this.funcInx = this.funcInx + 1
					let rulesAdForm = this.$store.state.rulesAddForm
					if (rulesAdForm.funcDevice) {
						rulesAdForm.funcDevice.push(form.rulesAdForm.deviceInfo)
					} else {
						rulesAdForm.funcDevice = []
						rulesAdForm.funcDevice.push(form.rulesAdForm.deviceInfo)
					}
					if (rulesAdForm.funcInfo) {
						rulesAdForm.funcInfo.push(form.rulesAdForm.funcValue)
					} else {
						rulesAdForm.funcInfo = []
						rulesAdForm.funcInfo.push(form.rulesAdForm.funcValue)
					}
					this.$store.commit('SET_RULESADD_INFO', rulesAdForm)
				}
				this.ruleTypeKey++
			},
			choiceDevice(type,addtype) {
				//选择设备
				this.closePup()
				let proAttrType = ''
				if (this.addType == 'condition'&& this.conditionRulesList.length == 0) {
					proAttrType = 'event'
				} else if (this.addType == 'condition') {
					proAttrType = 'attribute'
				} else if (this.addType == 'function') {
					if(type&&type=='function'){
						proAttrType = 'function'
					}else if(type&&type=='event'){
						proAttrType = 'event'
					}
					
				}
				uni.navigateTo({
					url: '/pages_device/select_list?isNotReturn=' + true + '&dataType=' + proAttrType+'&rulesType='+addtype
				})
			},
			closePup() {
				this.$refs.typePopup.close()
			},
			openPopup(val) {
				if (val == 0) {
					this.popupTitle = "添加触发条件"
					this.addType = "condition"
				} else {
					this.popupTitle = '添加执行动作'
					this.addType = 'function'
				}
				this.$refs.typePopup.open()
			},
			submit() {
				if (this.rulesId) {
					this.saveSubmit()
				} else {
					this.addSubmit()
				}

			},
			saveSubmit() {
				try {
					this.isLoading = true
					let rulesAdForm = this.$store.state.rulesAddForm
					let submitForm = {}
					submitForm.Id = this.rulesId

					if (rulesAdForm.triggerWay == 0) {
						submitForm.triggerWay = 0
						submitForm.httpParams = "[]"
						submitForm.CreatedFrom = rulesAdForm.createdFrom
						let triggerLs = []
						let obj = {
							TopicDevice: "/" + rulesAdForm.eventDevice[0].ProductId + "/" + rulesAdForm.eventDevice[0].DeviceId,
							TopicMsg: "Event#" + rulesAdForm.eventinfo.code
						}
						triggerLs.push(obj)
						submitForm.triggerList = JSON.parse(JSON.stringify(triggerLs))
						submitForm.timerCron = ""
						submitForm.name = rulesAdForm.eventDevice[0].Name + ' ' + rulesAdForm.eventinfo.name
					} else if (rulesAdForm.triggerWay == 2) {
						submitForm.triggerWay = 2
						submitForm.httpParams = JSON.stringify(rulesAdForm.httpParams)
						submitForm.CreatedFrom = rulesAdForm.createdFrom
						submitForm.timerCron = rulesAdForm.timerCron
						submitForm.name = rulesAdForm.name
					}
					let jsonStr = {
						id: "root",
						parentId: null,
						type: "ROOT",
						name: "发起人",
						desc: "任何人",
						children: {}
					}
					if (rulesAdForm.attrInfo && rulesAdForm.attrInfo.length > 0) {
						for (let i = 0; i < rulesAdForm.attrInfo.length; i++) {
							if (i == 0) {
								let conditionNodeId = this.conditionsNodeId
								jsonStr.children = {
									branchs: [],
									children: {},
									id: conditionNodeId,
									name: "条件分支",
									parentId: "root",
									props: {},
									type: "CONDITIONS",
								}
								jsonStr.children.branchs[0] = {
									children: {},
									id: this.conditionNodeId,
									name: "条件1",
									parentId: conditionNodeId,
									props: {
										expression: "",
										groups: [{
											cids: [],
											conditions: [],
											groupType: "AND"
										}],
										groupsType: "OR"
									},
									type: "CONDITION"
								}
							}
							let cidStr = rulesAdForm.attrInfo[i].code
							let conditionsLi = rulesAdForm.attrInfo[i]
							jsonStr.children.branchs[0].props.groups[0].cids.push(cidStr)
							jsonStr.children.branchs[0].props.groups[0].conditions.push(conditionsLi)
						}
					}
					// console.log("条件节点转化后", jsonStr, rulesAdForm.funcInfo && rulesAdForm.funcInfo.length > 0);
					if (rulesAdForm.funcInfo && rulesAdForm.funcInfo.length > 0) {
						let hasCondition = false
						if (rulesAdForm.attrInfo && rulesAdForm.attrInfo.length > 0) {
							hasCondition = true
						}
						let arr = []
						for (let e = 0; e < rulesAdForm.funcInfo.length; e++) {
							// console.log(e,'长度序号');
							let funObj = {}
							if (e == 0 && hasCondition) {
								jsonStr.children.children = {
									children: {},
									id: this.emptyNodeId ? this.emptyNodeId : this.getRandomId(),
									parentId: jsonStr.children.id,
									type: "EMPTY"
								}
							}
							let parId = null

							if (this.functionRulesList[e].nodeId && this.functionRulesList[e].nodePartId) {
								if (arr.length > 0) {
									parId = arr[arr.length - 1].id
								} else {
									parId = hasCondition ? jsonStr.children.children.id : jsonStr.id
								}
								if(this.functionRulesList[e].isFunc&&!this.functionRulesList[e].isEvent){
									funObj = {
										children: {},
										id: this.functionRulesList[e].nodeId,
										name: "功能节点",
										parentId: parId,
										props: {},
										type: "FUNC"
									}
								}else if(this.functionRulesList[e].isFunc&&this.functionRulesList[e].isEvent){
									funObj = {
										children: {},
										id: this.functionRulesList[e].nodeId,
										name: "触发事件",
										parentId: parId,
										props: {},
										type: "WARN"
									}
								}
								
							} else {
								if (arr.length > 0) {
									parId = arr[arr.length - 1].id
								} else {
									parId = hasCondition ? jsonStr.children.children.id : jsonStr.id
								}
								if(this.functionRulesList[e].isFunc&&!this.functionRulesList[e].isEvent){
									funObj = {
										children: {},
										id: this.getRandomId(),
										name: "功能节点",
										parentId: parId,
										props: {},
										type: "FUNC"
									}
								}else if(this.functionRulesList[e].isFunc&&this.functionRulesList[e].isEvent){
									funObj = {
										children: {},
										id: this.getRandomId(),
										name: "触发事件",
										parentId: parId,
										props: {},
										type: "WARN"
									}
								}
							}
							if(this.functionRulesList[e].isFunc&&!this.functionRulesList[e].isEvent){
								funObj.props = {
									EventInput: true,
									FunctionId: rulesAdForm.funcInfo[e].code,
									ReturnOutput: true,
									TargetId: rulesAdForm.funcDevice[e].Id,
									TargetType: 1
								}
							}else if(this.functionRulesList[e].isFunc&&this.functionRulesList[e].isEvent){
								funObj.props = {
									EventId: rulesAdForm.funcInfo[e].code,
									TargetId: rulesAdForm.funcDevice[e].Id,
									TargetType: 1
								}
							}
							arr.push(funObj)
						}
						let result = []
						if (arr.length == 1) {
							result = arr
						} else {
							if (hasCondition) {
								result = this.transList2TreeData(arr, jsonStr.children.children.id)
							} else {
								result = this.transList2TreeData(arr, jsonStr.id)
							}
						}
						if (hasCondition) {
							jsonStr.children.children.children = JSON.parse(JSON.stringify(result[0]))
						} else {
							jsonStr.children = JSON.parse(JSON.stringify(result[0]))
						}

					}
					submitForm.ruleJson = JSON.stringify(jsonStr)
					// submitForm.ruleJson = jsonStr
					submitForm.name = this.rulesName
					submitForm.status = this.rulesStatic
					editRuselServe(submitForm).then(res => {
						this.$refs.promptMsg.open('保存成功', 2000) //提示信息组件
						setTimeout(() => {
							setPagesParam('loadData', 'load', 1)
							// uni.navigateBack()
						}, 2000);
					}).catch(e => {
						this.isLoading = false
						this.setMsgTop(e)
					})

				} catch (e) {
					console.log("报错啦", e);
					//TODO handle the exception
					this.isLoading = false
				}
			},
			addSubmit() {
				try {
					this.isLoading = true
					let rulesAdForm = this.$store.state.rulesAddForm
					let submitForm = {}
					if (rulesAdForm.eventinfo && rulesAdForm.eventinfo.code) {
						submitForm.triggerWay = 0
						submitForm.httpParams = "[]"
						submitForm.CreatedFrom = "mobile"
						submitForm.sort = 0
						let triggerLs = []
						let obj = {
							TopicDevice: "/" + rulesAdForm.eventDevice[0].ProductId + "/" + rulesAdForm.eventDevice[0]
								.DeviceId,
							TopicMsg: "Event#" + rulesAdForm.eventinfo.code
						}
						triggerLs.push(obj)
						submitForm.triggerList = JSON.parse(JSON.stringify(triggerLs))
						submitForm.timerCron = ""
						submitForm.remark = ''
						submitForm.debug = 1
						submitForm.name = rulesAdForm.eventDevice[0].Name + ' ' + rulesAdForm.eventinfo.name
					} else if (rulesAdForm.triggerWay == 2) {
						submitForm.triggerWay = 2
						submitForm.httpParams = JSON.stringify(rulesAdForm.httpParams)
						submitForm.CreatedFrom = "mobile"
						submitForm.sort = 0
						submitForm.triggerList = []
						submitForm.timerCron = rulesAdForm.timerCron
						submitForm.remark = ''
						submitForm.debug = 1
						submitForm.name = rulesAdForm.name
					}
					let jsonStr = {
						id: "root",
						parentId: null,
						type: "ROOT",
						name: "发起人",
						desc: "任何人",
						children: {}
					}
					if (rulesAdForm.attrInfo && rulesAdForm.attrInfo.length > 0) {
						for (let i = 0; i < rulesAdForm.attrInfo.length; i++) {
							if (i == 0) {
								let conditionNodeId = this.getRandomId()
								jsonStr.children = {
									branchs: [],
									children: {},
									id: conditionNodeId,
									name: "条件分支",
									parentId: "root",
									props: {},
									type: "CONDITIONS",
								}
								jsonStr.children.branchs[0] = {
									children: {},
									id: this.getRandomId(),
									name: "条件1",
									parentId: conditionNodeId,
									props: {
										expression: "",
										groups: [{
											cids: [],
											conditions: [],
											groupType: "AND"
										}],
										groupsType: "OR"
									},
									type: "CONDITION"
								}
							}
							let cidStr = rulesAdForm.attrInfo[i].code
							let conditionsLi = rulesAdForm.attrInfo[i]
							jsonStr.children.branchs[0].props.groups[0].cids.push(cidStr)
							jsonStr.children.branchs[0].props.groups[0].conditions.push(conditionsLi)
						}
					}
					if (rulesAdForm.funcInfo && rulesAdForm.funcInfo.length > 0) {
						let hasCondition = false
						if (rulesAdForm.attrInfo && rulesAdForm.attrInfo.length > 0) {
							hasCondition = true
						}
						let arr = []
						for (let e = 0; e < rulesAdForm.funcInfo.length; e++) {
							let funObj = {}
							if (e == 0 && hasCondition) {
								jsonStr.children.children = {
									children: {},
									id: this.getRandomId(),
									parentId: jsonStr.children.id,
									type: "EMPTY"
								}
							}
							let parId = null
							if (arr.length > 0) {
								parId = arr[arr.length - 1].id
							} else {
								parId = hasCondition ? jsonStr.children.children.id : jsonStr.id
							}
							if(this.functionRulesList[e].isFunc&&!this.functionRulesList[e].isEvent){
								funObj = {
									children: {},
									id: this.getRandomId(),
									name: "功能节点",
									parentId: parId,
									props: {},
									type: "FUNC"
								}
								funObj.props = {
									EventInput: true,
									FunctionId: rulesAdForm.funcInfo[e].code,
									ReturnOutput: true,
									TargetId: rulesAdForm.funcDevice[e].Id,
									TargetType: 1
								}
							}else if(this.functionRulesList[e].isFunc&&this.functionRulesList[e].isEvent){
								funObj = {
									children: {},
									id: this.getRandomId(),
									name: "触发事件",
									parentId: parId,
									props: {},
									type: "WARN"
								}
								funObj.props = {
									EventId: rulesAdForm.funcInfo[e].code,
									TargetId: rulesAdForm.funcDevice[e].Id,
									TargetType: 1
								}
							}
							
							
							arr.push(funObj)
						}
						if (hasCondition) {
							let result = this.transList2TreeData(arr, jsonStr.children.children.id)
							jsonStr.children.children.children = JSON.parse(JSON.stringify(result[0]))
						} else {
							let result = this.transList2TreeData(arr, jsonStr.id)
							jsonStr.children = JSON.parse(JSON.stringify(result[0]))
						}

					}
					// console.log('添加时的规则',jsonStr);
					submitForm.ruleJson = JSON.stringify(jsonStr)
					addRuselServe(submitForm).then(res => {
						this.$refs.promptMsg.open('添加成功', 2000) //提示信息组件
						setTimeout(() => {
							setPagesParam('loadData', 'load', 1)
							// uni.navigateBack()
						}, 2000);
					}).catch(e => {
						this.isLoading = false
						this.setMsgTop(e)
					})
				} catch (e) {
					//TODO handle the exception
					this.isLoading = false
				}
			},
			transList2TreeData(list, rootValue) {
				const deepList = JSON.parse(JSON.stringify(list)) // 深拷贝
				const arr = []
				deepList.map(item => {
					if (item.parentId === rootValue) {
						const children = this.transList2TreeData(deepList, item.id)

						item.children = children[0] ? children[0] : {} // 如果希望每个item都有children属性, 可以直接赋值
						arr.push(item)
					}
				})
				return arr
			},
			getRandomId() {
				return `node_${new Date().getTime().toString().substring(5)}${Math.round(Math.random()*9000+1000)}`
			},
		}
	}
</script>

<style lang="less" scoped>
	.yellow_btn {
		margin-bottom: 0;
		margin-top: 100rpx;
	}

	.popup_con {
		background-color: #fff;
		width: 100%;
		border-radius: 20rpx 20rpx 0 0;
		padding: 30rpx 50rpx;
		box-sizing: border-box;
		color: #333333;

		.popup_title {
			width: 100%;
			padding: 30rpx 0;
			font-size: 34rpx;
			text-align: center;
		}

		.choice_type {
			margin-top: 20rpx;
			width: 100%;
			padding: 0 30rpx;
			box-sizing: border-box;
			height: 150rpx;
			display: flex;
			justify-content: space-between;
			align-items: center;
			background-color: #F8F8F8;
			border-radius: 10rpx;

			.choice_left {
				display: flex;
				justify-content: flex-start;
				align-items: center;

				.image {
					width: 66rpx;
					height: 66rpx;
				}

				.name {
					font-size: 32rpx;
					margin-left: 30rpx;
				}
			}
		}
	}

	.botton_con {
		width: 100%;
		padding: 30rpx 20rpx;
		box-sizing: border-box;
		background-color: #F5F8F9;
		position: fixed;
		bottom: env(safe-area-inset-bottom);
		left: 0;
	}

	.zhanwei {
		width: 100%;
		height: 180rpx;
	}

	.rules_con {
		padding: 0 20rpx;
		width: 100%;
		box-sizing: border-box;

		.rules_name_input {
			width: 100%;
			height: 126rpx;
			border-radius: 10rpx;
			background-color: #ffffff;
			display: flex;
			align-items: center;
			justify-content: flex-start;

			.input_text {
				width: 100%;
				padding-left: 30rpx;
				box-sizing: border-box;
				color: #333333;
				font-weight: 600;
			}
		}

		.rules_type_list {
			padding: 30rpx 0;

			.type_title {
				padding: 10rpx;
				font-size: 32rpx;
				color: #333333;
			}

			.add_con {
				width: 100%;
				height: 126rpx;
				display: flex;
				justify-content: center;
				align-items: center;
				background-color: #ffffff;
				border-radius: 10rpx;
				margin-top: 20rpx;

				.icon_con {
					width: 40rpx;
					height: 40rpx;
					border-radius: 50%;
					background-color: #E9F1FF;
					display: flex;
					justify-content: center;
					align-items: center;
					margin-right: 16rpx;
				}

				.add_text {
					color: #2371FF;
					font-size: 30rpx;
				}
			}

			.type_li {
				width: 100%;
				padding: 23rpx 30rpx;
				box-sizing: border-box;
				background-color: #ffffff;
				border-radius: 10rpx;
				display: flex;
				justify-content: space-between;
				align-items: center;
				min-height: 44rpx;
				margin-top: 20rpx;
				position: relative;

				.del_con {
					position: absolute;
					right: -10rpx;
					top: -10rpx;
					width: 30rpx;
					height: 30rpx;
					border-radius: 50%;

					.image {
						width: 30rpx;
						height: 30rpx;
						border-radius: 50%;
					}
				}

				.li_left {
					display: flex;
					justify-content: flex-start;
					align-items: center;

					.image {
						width: 66rpx;
						height: 66rpx;
						margin-right: 30rpx;
					}

					.li_cot {
						.name {
							font-size: 28rpx;
							color: #333333;
							line-height: 42rpx;
							font-weight: 550;
						}

						.text {
							font-size: 24rpx;
							color: #999999;
							line-height: 36rpx;
						}
					}
				}
			}
		}

		.qiyong_con {
			display: flex;
			justify-content: space-between;
			align-items: center;
			width: 100%;
			height: 44rpx;
			line-height: 44rpx;
			margin-top: 14rpx;

			.label {
				font-size: 32rpx;
				color: #333333;
			}

			.cot_right {
				.conten_icon {
					width: 84rpx;
					height: 44rpx;
				}
			}
		}
	}
</style>