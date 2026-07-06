<template>
	<view class="pages_bgcon" style="background-color: #FFFFFF;">
		<top :title="topTitle" leftWidth="157rpx" :isleftBack="true" leftIcon="icon-fanhui" rightWidth="157rpx"
			:rightIcon="isOnlyRead?'':'icon-wancheng'" rightIconColor="#2371FF" backgroundColor="#ffffff" @clickRight="finishFunChoice">
		</top>
		<view class="attribute_con">
			<view class="image_con">
				<image class="image" :src="otherRulesForm.PhotoUrl+'?wh=500x500'" mode=""
					v-if="otherRulesForm.PhotoUrl"></image>
				<image class="image" :src="getSerVerUrl()+'/appimg/device_default.png'" mode="" v-else></image>
			</view>
			<view class="device_name">
				{{otherRulesForm.deviceInfo&&otherRulesForm.deviceInfo.Name?otherRulesForm.deviceInfo.Name:''}}
			</view>
			<view class="attr_text">
				<view class="lable">
					条件：
				</view>
				<view class="text">
					{{attrForm.name}}
				</view>
			</view>
			<uni-forms ref="attrForm" :modelValue="attrForm" :rules="rules" labelWidth='80' label-position="top">
				<view class="form_con"
					:style="{'top':Number(statusBarHeight*2)+396+'rpx','background':'#ffffff','padding':0}"
					v-if="valType===ValueType2.date">
					<uni-forms-item label="在" required name="password" id="password_form" labelFont="32rpx"
						contentFont="32rpx" :requireOpacity="0.5">
						<view id="password" class="form_li">
							<view class="int_date">
								<view class="date_con long_date" style="color: #fff;">
									<uni-datetime-picker ref="dateChoice1" class="date" type="datetime"
										placeholder-style="font-size:32rpx;color:rgba(193, 193, 193, 1)"
										:clear-icon="false" v-model="attrForm.value" placeholder='请选择时间日期'
										:isCustom="true" :isDark="false" @change="changeDateTime"
										:disabled="isOnlyRead">
										<view class="date_slot" :class="{'has_val':attrForm.value}">
											<custom-icons iconsName="icon-xuanzeshijian" iconsSize="28rpx"
												iconsColor="rgba(193, 193, 193, 1)"></custom-icons>
											<view class="text">
												{{attrForm.value?attrForm.value:'请选择时间日期'}}
											</view>
										</view>
									</uni-datetime-picker>
								</view>
							</view>
						</view>
					</uni-forms-item>
					<uni-forms-item label="选择值" required name="username" id="username_form" labelFont="32rpx"
						contentFont="32rpx" :requireOpacity="0.5">
						<view id="username" class="form_li">
							<uni-data-select v-model="attrForm.compare" :localdata="symbolList" @change="change"
								width="100%" placeholder="请选择值" borderColor="rgba(255, 255, 255, 0.20)"
								palColor="rgba(193, 193, 193, 1)" :isCustom="true" :isDark="false"
								:disabled="isOnlyRead"></uni-data-select>
						</view>
					</uni-forms-item>
				</view>
				<view class="form_con"
					:style="{'top':Number(statusBarHeight*2)+396+'rpx','background':'#ffffff','padding':0}"
					v-else-if="valType === ValueType2.enum || valType === ValueType2.boolean">
					<uni-forms-item label="判断符" required name="username" id="username_form" labelFont="32rpx"
						contentFont="32rpx" :requireOpacity="0.5">
						<view id="username" class="form_li">
							<uni-data-select v-model="attrForm.compare" :localdata="symbolList" @change="change"
								width="100%" placeholder="请选择判断符" borderColor="rgba(255, 255, 255, 0.20)"
								palColor="rgba(193, 193, 193, 1)" :isCustom="true" :isDark="false"
								:disabled="isOnlyRead"></uni-data-select>
						</view>
					</uni-forms-item>
					<uni-forms-item label="选择值" required name="username" id="username_form" labelFont="32rpx"
						contentFont="32rpx" :requireOpacity="0.5">
						<view id="username" class="form_li">
							<uni-data-select v-model="attrForm.value" :localdata="compareItems" @change="change"
								width="100%" placeholder="请选择值" borderColor="rgba(255, 255, 255, 0.20)"
								palColor="rgba(193, 193, 193, 1)" :isCustom="true" :isDark="false"
								v-if="valType === ValueType2.boolean" :disabled="isOnlyRead"></uni-data-select>

							<!-- <zxz-uni-data-select v-model="attrForm.value" :filterable="false" :multiple="false" dataKey="text"
								dataValue="value" :localdata="compareItems" @change="changeChoice" :iscustom="true"
								v-if="valType === ValueType2.enum"></zxz-uni-data-select> -->
							<uni-data-select v-model="attrForm.value" :localdata="compareItems" @change="changeChoice"
								width="100%" placeholder="请选择值" borderColor="rgba(255, 255, 255, 0.20)"
								palColor="rgba(193, 193, 193, 1)" :isCustom="true" :isDark="false"
								v-if="valType === ValueType2.enum" :disabled="isOnlyRead"></uni-data-select>
						</view>
					</uni-forms-item>
				</view>
				<view class="form_con"
					:style="{'top':Number(statusBarHeight*2)+396+'rpx','background':'#ffffff','padding':0}"
					v-else-if="valType === ValueType2.int || valType === ValueType2.float">
					<uni-forms-item label="判断符" required name="username" id="username_form" labelFont="32rpx"
						contentFont="32rpx" :requireOpacity="0.5">
						<view id="username" class="form_li">
							<uni-data-select v-model="attrForm.compare" :localdata="symbolList" @change="change"
								width="100%" placeholder="请选择判断符" borderColor="rgba(255, 255, 255, 0.20)"
								palColor="rgba(193, 193, 193, 1)" :isCustom="true" :isDark="false"
								:disabled="isOnlyRead"></uni-data-select>
						</view>
					</uni-forms-item>
					<uni-forms-item label="比较值" required name="password" id="password_form" labelFont="32rpx"
						contentFont="32rpx" :requireOpacity="0.5" v-if="conditionValType(attrForm.compare) === 0">
						<view id="password" class="form_li">
							<uni-easyinput placeholderStyle="color:rgba(193, 193, 193, 1);font-size:32rpx"
								inputHeight="88rpx" :styles="styles" type="digit" v-model="attrForm.value[0]"
								placeholder="请输入比较值" contentFontSize="32rpx" primaryColor="rgba(35, 113, 255, 1)"
								:disabled="isOnlyRead" />
							<view class="num_cli" :style="{'background-image':`url(${getSerVerUrl()}/appimg/images/input_cli.png)`}">
								<view class="cli_up" @click.stop="numUp(0)">
									<custom-icons iconsName="icon-shuzijiajiansanjiao" iconsSize="14rpx"
										iconsColor="rgba(153, 153, 153, 1)"></custom-icons>
								</view>
								<view class="cli_up rotate-180" @click.stop="numDown(0)">
									<custom-icons iconsName="icon-shuzijiajiansanjiao" iconsSize="14rpx"
										iconsColor="rgba(153, 153, 153, 1)"></custom-icons>
								</view>
							</view>
						</view>
					</uni-forms-item>
					<uni-forms-item label="比较值" required name="password" id="password_form" labelFont="32rpx"
						contentFont="32rpx" :requireOpacity="0.5" v-else>
						<view id="password" class="form_li">
							<view class="" style="position: relative;">
								<uni-easyinput placeholderStyle="color:rgba(193, 193, 193, 1);font-size:32rpx"
									inputHeight="88rpx" :styles="styles" type="digit" v-model="attrForm.value[0]"
									placeholder="请输入比较值" contentFontSize="32rpx" primaryColor="rgba(35, 113, 255, 1)"
									:disabled="isOnlyRead" />
								<view class="num_cli" :style="{'background-image':`url(${getSerVerUrl()}/appimg/images/input_cli.png)`}">
									<view class="cli_up" @click.stop="numUp(0)">
										<custom-icons iconsName="icon-shuzijiajiansanjiao" iconsSize="14rpx"
											iconsColor="rgba(153, 153, 153, 1)"></custom-icons>
									</view>
									<view class="cli_up rotate-180" @click.stop="numDown(0)">
										<custom-icons iconsName="icon-shuzijiajiansanjiao" iconsSize="14rpx"
											iconsColor="rgba(153, 153, 153, 1)"></custom-icons>
									</view>
								</view>
							</view>
							<view class="fengefu">
								~
							</view>
							<view class="" style="position: relative;">
								<uni-easyinput placeholderStyle="color:rgba(193, 193, 193, 1);font-size:32rpx"
									inputHeight="88rpx" :styles="styles" type="digit" v-model="attrForm.value[1]"
									placeholder="请输入比较值" contentFontSize="32rpx" primaryColor="rgba(35, 113, 255, 1)"
									:disabled="isOnlyRead" />
								<view class="num_cli" :style="{'background-image':`url(${getSerVerUrl()}/appimg/images/input_cli.png)`}">
									<view class="cli_up" @click.stop="numUp(1)">
										<custom-icons iconsName="icon-shuzijiajiansanjiao" iconsSize="14rpx"
											iconsColor="rgba(153, 153, 153, 1)"></custom-icons>
									</view>
									<view class="cli_up rotate-180" @click.stop="numDown(1)">
										<custom-icons iconsName="icon-shuzijiajiansanjiao" iconsSize="14rpx"
											iconsColor="rgba(153, 153, 153, 1)"></custom-icons>
									</view>
								</view>
							</view>
						</view>
					</uni-forms-item>
				</view>
				<view class="form_con"
					:style="{'top':Number(statusBarHeight*2)+396+'rpx','background':'#ffffff','padding':0}"
					v-else-if="valType === ValueType2.string">
					<uni-forms-item label="判断符" required name="username" id="username_form" labelFont="32rpx"
						contentFont="32rpx" :requireOpacity="0.5">
						<view id="username" class="form_li">
							<uni-data-select v-model="attrForm.compare" :localdata="symbolList" @change="change"
								width="100%" placeholder="请选择判断符" borderColor="rgba(255, 255, 255, 0.20)"
								palColor="rgba(193, 193, 193, 1)" :isCustom="true" :isDark="false"
								:disabled="isOnlyRead"></uni-data-select>
						</view>
					</uni-forms-item>
					<uni-forms-item label="比较值" required name="password" id="password_form" labelFont="32rpx"
						contentFont="32rpx" :requireOpacity="0.5"
						v-if="attrForm.compare === '=' || attrForm.compare === '!='">
						<view id="password" class="form_li">
							<uni-easyinput placeholderStyle="color:rgba(193, 193, 193, 1);font-size:32rpx"
								inputHeight="88rpx" :styles="styles" v-model="attrForm.value[0]" placeholder="请输入比较值"
								contentFontSize="32rpx" primaryColor="rgba(35, 113, 255, 1)" :disabled="isOnlyRead" />
						</view>
					</uni-forms-item>
					<uni-forms-item label="选择值" required name="username" id="username_form" labelFont="32rpx"
						contentFont="32rpx" :requireOpacity="0.5" v-else>
						<view id="username" class="form_li">
							<zxz-uni-data-select v-model="attrForm.value" filterable multiple dataKey="text"
								dataValue="value" :localdata="candidates" @change="changeChoiceString"
								:isCanCustom="true" :iscustom="true" :disabled="isOnlyRead"></zxz-uni-data-select>
						</view>
					</uni-forms-item>
				</view>
			</uni-forms>
		</view>
	</view>
</template>

<script>
	import {
		productInfo
	} from "@/api/ruselSevic.js";
	import {
		setPagesParam
	} from '@/common/utillib.js'
	var dayjs = require('@/common/day.js')
	export default {
		data() {
			return {
				choiceValue: '', //选择值
				candidates: [], //自定义选择项使用的变量
				symbolval: '',
				symbolList: [{
						text: "等于",
						value: "="
					},
					{
						text: "不等于",
						value: "!="
					},
					{
						text: "大于",
						value: ">"
					},
					{
						text: "大于等于",
						value: ">="
					},
					{
						text: "小于",
						value: "<"
					},
					{
						text: "小于等于",
						value: "<="
					}
				],
				choiceList: [], //选择值
				statusBarHeight: uni.getSystemInfoSync().statusBarHeight,
				topTitle: '属性',
				attrForm: {},
				styles: {
					color: '#333',
					backgroundColor: 'rgba(248, 248, 248, 1)',
					disableColor: 'rgba(248, 248, 248, 0.5)',
					borderColor: 'rgba(255, 255, 255, 0.20)'
				},
				rules: {
					username: {
						rules: [{
							required: true,
							errorMessage: '请输入账号',
						}]
					},
					password: {
						rules: [{
							required: true,
							errorMessage: '请输入密码',
						}]
					},
					code: {
						rules: [{
							required: true,
							errorMessage: '请输入验证码',
						}]
					},
				},
				ValueType2: {
					int: 'int',
					float: 'float',
					string: 'string',
					enum: 'enum',
					boolean: 'boolean',
					object: 'object',
					array: 'array',
					number: 'number',
					date: 'date',
					user: 'user',
					dept: 'dept',
					dateRange: 'dateRange'
				},
				valType: '',
				rulesAdForm: {},
				otherRulesForm: {},
				attrFormOptions: {},
				compareItems: [],
				allInx: 0,
				infoIndex: 0,
				isOnlyRead: false
			}
		},
		onLoad(options) {
			if (this.$store.state.otherRulesForm) {
				this.otherRulesForm = JSON.parse((JSON.stringify(this.$store.state.otherRulesForm)))
				// console.log("规则表单设备属性", this.otherRulesForm);
			}
			if (options.allInx) {
				this.allInx = options.allInx
			}
			if (options.inx) {
				this.infoIndex = options.inx
			}
			if (options.isOnlyRead) {
				this.isOnlyRead = true
			}
			if (this.otherRulesForm.attrVal) {
				let newitem = this.otherRulesForm.attrVal;
				this.attrForm = this.otherRulesForm.attrVal
				this.attrForm.title = newitem.name;
				this.attrForm.gname = this.otherRulesForm.deviceInfo.Name;
				if(newitem.isEventParams){
					this.attrForm.code = "$input." + newitem.code;
				}else{
					this.attrForm.code = "$devprop." + this.otherRulesForm.deviceInfo.Id + "." + newitem.code;
				}
				
				this.attrFormOptions = this.attrForm.option ? this.attrForm.option : {}
				this.valType = this.attrForm.option ? this.attrForm.option.type : ''
				this.attrForm.valueType = this.valType
				this.attrForm.value = []
				this.attrForm.compare = ''
				if (this.valType == "boolean") {
					this.attrForm.value = ''
					let boolArr = [];
					boolArr.push({
						"text": this.attrFormOptions.trueText,
						"value": "true"
					});
					boolArr.push({
						"text": this.attrFormOptions.falseText,
						"value": "false"
					});
					this.compareItems = boolArr;
				}
				if (this.valType == "enum") {
					this.attrForm.value = ''
					let enumArr = [];
					for (let key in this.attrFormOptions.elements) {
						if (newitem.option !== undefined) {
							enumArr.push({
								"text": this.attrFormOptions.elements[key],
								"value": this.attrFormOptions.elements[key]
							});
						} else {
							enumArr.push({
								"text": this.attrFormOptions.elements[key],
								"value": key
							});
						}

					}
					this.compareItems = enumArr;
				}
				this.setAttrValType()
				if (this.$store.state.rulesAddForm) {
					this.rulesAdForm = this.$store.state.rulesAddForm
					if (options.haival) {
						let finishVal = this.rulesAdForm.attrInfo[options.inx]
						this.attrForm.compare = finishVal.compare ? finishVal.compare : ''
						this.$forceUpdate()
						this.$nextTick(() => {
							if (finishVal.value) {
								if (this.valType == "string" && this.attrForm.compare == 'IN') {
									this.candidates = []
									finishVal.value.map(row => {
										let obj = {
											text: row,
											value: row
										}
										this.candidates.push(obj)
									})
									this.attrForm.value = finishVal.value
								} else if (this.valType === 'int' || this.valType === 'float') {
									let a = Number(finishVal.value[0])
									this.attrForm.value.push(a)
									this.$forceUpdate()
								} else {
									if (this.valType == "date") {
										this.attrForm.value = dayjs(finishVal.value).format('YYYY-MM-DD HH:mm:ss')
									}else{
										this.attrForm.value = finishVal.value
									}
									
									this.$forceUpdate()
								}

							} else {
								if (this.valType == "enum" || this.valType == "boolean") {
									this.attrForm.value = ''
								} else if (this.valType == "date") {
									this.attrForm.value = ''
								} else {
									this.attrForm.value = []
								}
							}
						})

					}

				}

			}
		},
		methods: {
			changeDateTime(val) {
				this.attrForm.value = val
				this.$forceUpdate()
			},
			finishFunChoice() {
				if(this.isOnlyRead){
					return
				}
				let rulesAdForm = this.$store.state.otherRulesForm
				let attrFinishVal = JSON.parse(JSON.stringify(this.attrForm))
				if (attrFinishVal.valueType == "date") {
					attrFinishVal.value = dayjs(this.attrForm.value).valueOf()
				}
				rulesAdForm.attrVal = attrFinishVal
				if (this.infoIndex && this.allInx) {
					rulesAdForm.infoIndex = this.infoIndex
					rulesAdForm.allInx = this.allInx
				}
				setPagesParam('finshSelectAttr', rulesAdForm, 3)
			},
			numUp(inx) {
				if (this.attrForm.value[inx]) {
					if (String(this.attrForm.value[inx]).indexOf('.') > -1) {
						let arr = JSON.parse(JSON.stringify(String(this.attrForm.value[inx]).split('.')))
						this.attrForm.value[inx] = Number(String(Number(arr[0]) + 1) + '.' + arr[1])
					} else {
						this.attrForm.value[inx] = Number(this.attrForm.value[inx]) + 1
					}
					// this.attrForm.value[inx]=Number(this.attrForm.value[inx])+1
				} else {
					this.attrForm.value[inx] = 0
					if (String(this.attrForm.value[inx]).indexOf('.') > -1) {
						let arr = JSON.parse(JSON.stringify(String(this.attrForm.value[inx]).split('.')))
						this.attrForm.value[inx] = Number(String(Number(arr[0]) + 1) + '.' + arr[1])
					} else {
						this.attrForm.value[inx] = Number(this.attrForm.value[inx]) + 1
					}
				}
				this.$forceUpdate()
			},
			numDown(inx) {
				if (this.attrForm.value[inx]) {
					if (String(this.attrForm.value[inx]).indexOf('.') > -1) {
						let arr = JSON.parse(JSON.stringify(String(this.attrForm.value[inx]).split('.')))
						this.attrForm.value[inx] = Number(String(Number(arr[0]) - 1) + '.' + arr[1])
					} else {
						this.attrForm.value[inx] = Number(this.attrForm.value[inx]) - 1
					}
					// this.attrForm.value[inx]=Number(this.attrForm.value[inx])-1
				} else {
					this.attrForm.value[inx] = 0
					if (String(this.attrForm.value[inx]).indexOf('.') > -1) {
						let arr = JSON.parse(JSON.stringify(String(this.attrForm.value[inx]).split('.')))
						this.attrForm.value[inx] = Number(String(Number(arr[0]) - 1) + '.' + arr[1])
					} else {
						this.attrForm.value[inx] = Number(this.attrForm.value[inx]) - 1
					}
				}
				this.$forceUpdate()
			},
			changeChoice() {
				//包含枚举选择值函数
				// console.log("包含组件选值", this.choiceValue);

			},
			changeChoiceString(arr) {
				//选择字符串
				if (arr) {
					if (Array.isArray(arr)) {
						arr.map(rw => {
							let findObj = this.candidates.find(ro => ro.value == rw.value)
							if (findObj) {} else {
								this.candidates.push(rw.value)
								let tableList = JSON.parse(JSON.stringify(this.candidates))
								this.candidates = JSON.parse(JSON.stringify(tableList))
							}
						})
					} else {
						let findObj = this.candidates.find(ro => ro.value == arr.value)
						if (findObj) {} else {
							this.candidates.push(arr.value)
							let tableList = JSON.parse(JSON.stringify(this.candidates))
							this.candidates = JSON.parse(JSON.stringify(tableList))
						}
					}
				
				}
			},
			changeChoiceNum() {
				//选择字符串
			},
			containValSet(val) {
				//设置包含符合的值
				// console.log(val, '输入值打印');
				this.candidates = []
				this.candidates.push(val)
			},
			conditionValType(type) {
				switch (type) {
					case "=":
						return 0;
					case "!=":
						return 0;
					case ">":
						return 0;
					case ">=":
						return 0;
					case "<":
						return 0;
					case "<=":
						return 0;
					case "IN":
						return 1;
					default:
						return 0;
				}
			},
			setAttrValType() {
				if (this.valType == this.ValueType2.string) {
					this.symbolList = [{
							text: "等于",
							value: "="
						},
						{
							text: "不等于",
							value: "!="
						},
						{
							text: "包含",
							value: "IN"
						}
					]

				} else if (this.valType === this.ValueType2.int || this.valType === this.ValueType2.float) {
					this.symbolList = [{
							text: "等于",
							value: "="
						},
						{
							text: "不等于",
							value: "!="
						},
						{
							text: "大于",
							value: ">"
						},
						{
							text: "大于等于",
							value: ">="
						},
						{
							text: "小于",
							value: "<"
						},
						{
							text: "小于等于",
							value: "<="
						}
					]
				} else if (this.valType === this.ValueType2.enum || this.valType === this.ValueType2.boolean) {
					this.symbolList = [{
							text: "等于",
							value: "="
						},
						{
							text: "不等于",
							value: "!="
						}
					]

				} else if (this.valType === this.ValueType2.date) {
					this.symbolList = [{
							text: "之前",
							value: "before"
						},
						{
							text: "之后",
							value: "after"
						}
					]

				}
			},
			async initProEvts() {
				if (this.selProList == null || this.selProList.length == 0) {
					this.proEvt = [];
					return;
				}
				let pinfo = (await productInfo({
					id: this.selProList[0].Id
				})).data;
				let tsl = JSON.parse(pinfo.ModelTSL);
				this.proEvt = tsl.events;
			},
			change() {
				this.$forceUpdate()
			},
		}
	}
</script>

<style lang="less" scoped>
	page {
		background-color: #ffffff;
	}

	.attribute_con {
		display: flex;
		justify-content: flex-start;
		align-items: center;
		flex-direction: column;
		width: 100%;
		padding: 40rpx;
		box-sizing: border-box;

		.image_con {
			width: 140rpx;
			height: 140rpx;

			.image {
				width: 140rpx;
				height: 140rpx;
			}
		}

		.device_name {
			width: 100%;
			color: #333333;
			font-size: 32rpx;
			margin-top: 34rpx;
			font-weight: 550;
			text-align: center;
		}

		.attr_text {
			margin-top: 34rpx;
			margin-bottom: 60rpx;
			width: 100%;
			display: flex;
			justify-content: flex-start;
			align-items: center;
			background-color: #F8F8F8;
			padding: 0 30rpx;
			box-sizing: border-box;
			height: 100rpx;

			.lable {
				font-size: 28rpx;
				color: #999999;
			}

			.text {
				color: #333333;
			}
		}

		.form_con {
			width: 100%;
		}

		.uni-forms {
			width: 100%;
		}
	}
</style>