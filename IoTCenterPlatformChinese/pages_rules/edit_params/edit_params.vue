<template>
	<view>
		<top :title="topTitle" leftWidth="157rpx" leftIcon="icon-fanhui" rightWidth="157rpx" :isleftBack="true"
			backgroundColor="#ffffff" rightText="保存" rightTextColor="#2371FF" @clickRight="saveRulesParams"></top>
		<uni-forms ref="consumbleForm" :modelValue="paramsForm" labelWidth='80' label-position="top">
			<view class="form_con page_form_con">
				<template v-for="(item,index) in inputList">
					<uni-forms-item :label="item.name" :name="item.code" labelFont="32rpx" contentFont="32rpx"
						:requireOpacity="0.5" labelPosition="top" v-if="item.type=='enum'">
						<view class="form_li">
							<uni-data-select v-model="item.defval" :localdata="returnLocaldata(item.elements)"
								width="100%" :placeholder="'请选择'+item.name" :borderColor="item.readOnly?'#EFEFEF':'rgba(255, 255, 255, 0.20)'"
								palColor="rgba(193, 193, 193, 1)" :isCustom="true" :isDark="false"
								:disabled="item.readOnly"></uni-data-select>
						</view>
					</uni-forms-item>
					<uni-forms-item :label="item.name" :name="item.code" labelFont="32rpx" contentFont="32rpx"
						:requireOpacity="0.5" v-if="item.type=='string'">
						<view id="Name" class="form_li">
							<uni-easyinput placeholderStyle="color:rgba(193, 193, 193, 1);font-size:32rpx"
								inputHeight="88rpx" :styles="styles" type="text" v-model="item.defval"
								:placeholder="'请输入'+item.name" contentFontSize="32rpx" :inputBorder="item.readOnly"
								:disabled="item.readOnly" />
						</view>
					</uni-forms-item>
					<uni-forms-item :label="item.name" :name="item.code" labelFont="32rpx" contentFont="32rpx"
						:requireOpacity="0.5" labelPosition="top" v-if="item.type=='int'">
						<view id="Unit" class="form_li">
							<view style="width: 100%;">
								<uni-easyinput placeholderStyle="color:rgba(255, 255, 255, 0.2);font-size:32rpx"
									inputHeight="88rpx" :styles="styles" type="number" v-model="item.defval"
									:placeholder="'请输入'+item.name" contentFontSize="32rpx"
									@input="filterValue2($event,index,item,true)" :inputBorder="item.readOnly"
									:disabled="item.readOnly" />
								<view class="num_cli" :style="{'background-image':`url(${getSerVerUrl()}/appimg/images/input_cli.png)`}">
									<view class="cli_up" @click.stop="numUp(index,item,true)">
										<custom-icons iconsName="icon-shuzijiajiansanjiao" iconsSize="14rpx"
											:iconsColor="!item.readOnly?'rgba(153, 153, 153, 1)':'rgba(153, 153, 153, 0.5)'"></custom-icons>
									</view>
									<view class="cli_up rotate-180" @click.stop="numDown(index,item,true)">
										<custom-icons iconsName="icon-shuzijiajiansanjiao" iconsSize="14rpx"
											:iconsColor="!item.readOnly?'rgba(153, 153, 153, 1)':'rgba(153, 153, 153, 0.5)'"></custom-icons>
									</view>
								</view>
							</view>
						</view>
					</uni-forms-item>
					<uni-forms-item :label="item.name" :name="item.code" labelFont="32rpx" contentFont="32rpx"
						:requireOpacity="0.5" labelPosition="top" v-if="item.type=='float'">
						<view id="Unit" class="form_li">
							<view style="width: 100%;">
								<uni-easyinput placeholderStyle="color:rgba(255, 255, 255, 0.2);font-size:32rpx"
									inputHeight="88rpx" :styles="styles" type="digit" v-model="item.defval"
									:placeholder="'请输入'+item.name" contentFontSize="32rpx"
									@input="filterValue2($event,index,item,false)" :disabled="item.readOnly"
									:inputBorder="item.readOnly" />
								<view class="num_cli" :style="{'background-image':`url(${getSerVerUrl()}/appimg/images/input_cli.png)`}">
									<view class="cli_up" @click.stop="numUp(index,item,false)">
										<custom-icons iconsName="icon-shuzijiajiansanjiao" iconsSize="14rpx"
											:iconsColor="!item.readOnly?'rgba(153, 153, 153, 1)':'rgba(153, 153, 153, 0.5)'"></custom-icons>
									</view>
									<view class="cli_up rotate-180" @click.stop="numDown(index,item,false)">
										<custom-icons iconsName="icon-shuzijiajiansanjiao" iconsSize="14rpx"
											:iconsColor="!item.readOnly?'rgba(153, 153, 153, 1)':'rgba(153, 153, 153, 0.5)'"></custom-icons>
									</view>
								</view>
							</view>
						</view>
					</uni-forms-item>
					<uni-forms-item :label="item.name" :name="item.code" labelFont="32rpx" contentFont="32rpx"
						:requireOpacity="0.5" v-if="item.type=='date'">
						<view class="form_li">
							<view class="int_date">
								<view class="date_con long_date" style="color: #fff;">
									<uni-datetime-picker ref="dateChoice1" class="date" type="datetime"
										placeholder-style="font-size:32rpx;color:#C1C1C1" :clear-icon="false"
										v-model="item.defval" :placeholder='"请选择"+item.name' :isCustom="true"
										:isDark="false" :disabled="item.readOnly">
										<view class="date_slot" :class="{'has_val':item.defval,'disabled_text':item.readOnly}">
											<custom-icons iconsName="icon-xuanzeshijian" iconsSize="28rpx"
												iconsColor="#C1C1C1"></custom-icons>
											<view class="text">
												{{item.defval?returnTime(item.defval):'请选择'+item.name}}
											</view>

										</view>
									</uni-datetime-picker>
								</view>
							</view>
						</view>
					</uni-forms-item>
					<uni-forms-item :label="item.name" :name="item.code" labelFont="32rpx" contentFont="32rpx"
						:requireOpacity="0.5" v-if="item.type=='boolean'">
						<view class="form_li">
							<switch :checked="item.defval" v-model="item.defval" @change="switchChange($event,item,index)"
								:disabled="item.readOnly" />
						</view>
					</uni-forms-item>
				</template>
			</view>
		</uni-forms>
		<msg-prompt ref="promptMsg"></msg-prompt>
	</view>
</template>

<script>
	import {
		getRuselDetail,
		editRuselServe
	} from "@/api/ruselSevic.js";
	import {
		setPagesParam
	} from '@/common/utillib.js'
	var dayjs = require('@/common/day.js')
	export default {
		data() {
			return {
				isLoad: false,
				inputList: [], //输入参数列表
				topTitle: '修改参数',
				paramsForm: {
					Unit: '',
					Number: 0,
					checked: false
				},
				value: 0,
				min: 0,
				max: 100,
				disabled: false,
				styles: {
					color: '#333',
					backgroundColor: 'rgba(248, 248, 248, 1)',
					disableColor: '#F8F8F8',
					borderColor: 'rgba(234, 234, 234, 1)'
				},
				localdata: [],
				rulesId:0
			};
		},
		async onLoad(options) {
			if(options.name){
				this.topTitle=options.name+'参数'
			}
			this.rulesId=options.id
			await this.loadFormInfo(options.id)
		},
		methods: {
			returnTime(time){
				return dayjs(time).format('YYYY-MM-DD HH:mm:ss')
			},
			saveRulesParams(){
				let submitForm={
					Id:this.rulesId,
					httpParams:JSON.stringify(this.inputList)
				}
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
			},
			returnLocaldata(obj) {
				let arr = []
				for (let row in obj) {
					let rowObj = {
						value: row,
						text: row
					}
					arr.push(rowObj)
				}
				return arr
			},
			switchChange(value,item,inx) {
				this.inputList[inx].defval=!item.defval
			},
			filterValue2(input, inx, item, isInt) {
				this.$nextTick(() => {
					let value = input
					if (isInt) {
						var t = value.charAt(0);
						value = value.replace(/\D+/g, '');
						value = value ? Number(value).toString() : value //去掉开头多个0
						// this.$nextTick(() => {
						if (t == '-') {
							value = '-' + value;
						}
						this.inputList[inx].defval = Number(value)
						// })
					} else {
						value = value.replace(/[^\d-.]/g, '');
						value = value.replace(/^\./g, '');
						value = value.replace('.', '$#$').replace(/\./g, '').replace('$#$', '.');
						if (value && !value.includes('.')) {
							value = Number(value).toString() //去掉开头多个0
						}
						// this.$nextTick(() => {
						this.inputList[inx].defval = value
						// })
					}

				})

			},
			numUp(inx, item, isInt) {
				let value = this.inputList[inx].defval
				if (!item.readOnly) {
					// this.$nextTick(() => {
					if (!isInt && String(value).indexOf('.') > -1) {
						let arr = String(value).split('.')
						this.inputList[inx].defval = Number(String(Number(arr[0]) + 1) + '.' + arr[1])
					} else {
						this.inputList[inx].defval = Number(value) + 1
					}

					// })
				}
			},
			numDown(inx, item, isInt) {
				let value = this.inputList[inx].defval
				if (!item.readOnly) {
					// this.$nextTick(() => {
					if (!isInt && String(value).indexOf('.') > -1) {
						let arr = String(value).split('.')
						this.inputList[inx].defval = Number(String(Number(arr[0]) - 1) + '.' + arr[1])
					} else {
						this.inputList[inx].defval = Number(value) - 1
					}

					// })
				}
			},
			async loadFormInfo(rulseId) {
				uni.showLoading({
					title: '加载中'
				})
				this.isLoad = true
				try {
					let rsp = await getRuselDetail({
						id: rulseId
					});
					let data = rsp.data
					this.inputList = JSON.parse(data.HttpParams)
					uni.hideLoading()
				} catch (e) {
					//TODO handle the exception
					if (this.isLoad) {
						this.isLoad = false
						uni.hideLoading()
					}

					// console.log(e, 'eeeeeeeeeeeeeeeeee');
				}
			}
		}
	}
</script>

<style lang="less">

</style>