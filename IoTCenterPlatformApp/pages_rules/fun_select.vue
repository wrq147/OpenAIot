<template>
	<view class="pages_bgcon">
		<top :title="topTitle" leftWidth="157rpx" :isleftBack="true" backgroundColor="#F5F8F9" leftIcon="icon-fanhui"
			rightWidth="157rpx">
			<!-- :rightIcon="!isDisable?'icon-wancheng':''" rightIconColor="#2371FF" @clickRight="finishFunChoice" -->
		</top>
		<view class="fun_con">
			<view class="fun_li" v-for="(item,inx) in proEvt" @click="finishFunChoice(item,inx)"
				:style="{'opacity':isDisable&&!item.select?0.5:1}">
				<view class="name">
					{{item.name}}
				</view>
				<view class="right">
					<custom-icons iconsName="icon-weixuanzhong" iconsSize="32rpx" iconsColor="#EAEAEA"
						v-if="!item.select"></custom-icons>
					<custom-icons iconsName="icon-danxuan" iconsSize="32rpx" iconsColor="#2371FF" v-else></custom-icons>
				</view>
			</view>
			<uni-load-more iconType="circle" :status="status" v-if="status" />
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
	export default {
		data() {
			return {
				topTitle: '选择事件',
				status: 'noMore',
				deviceInfo: null,
				proEvt: [],
				isDisable: false,
				isOnlyRead: false,
				haival: false, //是否有值
				infoIndex: null,
				alReadChoice: '',
				allInx: 0,
				rulesType:null
			};
		},
		async onLoad(options) {
			uni.showLoading({
				title: "加载中"
			})
			try {
				if(options.rulesType){
					this.rulesType=options.rulesType
				}
				if (options.dataType) {
					// console.log("dataType", options.dataType);
					this.dataType = options.dataType
					if(this.dataType=='event'){
						this.topTitle='选择事件'
					}else if(this.dataType=='attribute'){
						this.topTitle='选择属性或参数'
					}else if(this.dataType=='function'){
						this.topTitle='选择功能'
					}
				}
				let rulesAdForm2 = JSON.parse(JSON.stringify(this.$store.state.rulesAddForm))
				let rulesOtherForm2 = JSON.parse(JSON.stringify(this.$store.state.otherRulesForm))


				if (options.isDisable||options.isOnlyRead) {
					this.isDisable = options.isDisable
					if(options.isOnlyRead){
						this.isOnlyRead=true
					}
					if (this.rulesType=='condition'&&this.dataType == 'event') {
						// console.log("事件", rulesAdForm2);
						if (rulesAdForm2 && rulesAdForm2.eventDevice) {
							this.deviceInfo = JSON.parse(JSON.stringify(rulesAdForm2.eventDevice[0]))
							await this.initProEvts()
						}else if(rulesOtherForm2 && rulesOtherForm2.eventDevice){
							this.deviceInfo = JSON.parse(JSON.stringify(rulesOtherForm2.eventDevice[0]))
							await this.initProEvts()
						}
					} else {
						this.deviceInfo = JSON.parse(JSON.stringify(rulesOtherForm2.deviceInfo))
						// console.log(this.deviceInfo, 'this.deviceInfothis.deviceInfo');
						await this.initProEvts()
					}
				} else {
					if (this.rulesType=='condition'&&this.dataType == 'event') {
						// console.log("事件", rulesAdForm2);
						if (rulesAdForm2 && rulesAdForm2.eventDevice) {
							this.deviceInfo = JSON.parse(JSON.stringify(rulesAdForm2.eventDevice[0]))
							await this.initProEvts()
						}else if (rulesOtherForm2 && rulesOtherForm2.eventDevice) {
							this.deviceInfo = JSON.parse(JSON.stringify(rulesOtherForm2.eventDevice[0]))
							await this.initProEvts()
						}
					} else {
						this.deviceInfo = JSON.parse(JSON.stringify(rulesOtherForm2.deviceInfo))
						// console.log(this.deviceInfo, 'this.deviceInfothis.deviceInfo');
						await this.initProEvts()
					}
				}
				if (options.allInx) {
					this.allInx = options.allInx
				}
				if (options.inx) {
					this.infoIndex = options.inx
				}
				if (options.haival) {
					this.$nextTick(() => {
						if (this.rulesType=='condition'&&this.dataType == 'event') {

							this.haival = options.haival
							let rulesAdForm = this.$store.state.rulesAddForm
							// console.log(rulesAdForm, '规则');
							this.proEvt.map(row => {
								if (rulesAdForm.eventinfo && row.code == rulesAdForm.eventinfo.code) {
									row.select = true
								} else {
									row.select = false
								}
							})
						} else if (this.dataType == 'attribute') {
							if (options.inx) {
								this.infoIndex = options.inx
								let rulesAdForm = this.$store.state.rulesAddForm
								let selCode = ''
								if (rulesAdForm.attrInfo && rulesAdForm.attrInfo[Number(options.inx)]) {
									let arr1 = rulesAdForm.attrInfo[Number(options.inx)].code.split('.')
									if(arr1[0]==='$devprop'){
										selCode = arr1[2]
									}else if(arr1[0]==='$input'){
										selCode = arr1[1]
									}
									
									this.alReadChoice = selCode
								}
								this.proEvt.map(row => {
									if (selCode && row.code == selCode) {
										row.select = true
									} else {
										row.select = false
									}
								})
							}
						} else if (this.rulesType=='function'&&this.dataType == 'event'||this.dataType == 'function') {
							this.haival = options.haival
							let rulesAdForm = this.$store.state.rulesAddForm
							this.proEvt.map(row => {
								if (rulesAdForm.funcInfo && rulesAdForm.funcInfo[Number(options
									.inx)] && row.code == rulesAdForm.funcInfo[Number(options.inx)]
									.code) {
									row.select = true
								} else {
									row.select = false
								}
							})
						}
						uni.hideLoading()
						this.$forceUpdate()
					})
				} else {
					uni.hideLoading()
				}
			} catch (e) {
				//TODO handle the exception
				console.log("e",e);
				uni.hideLoading()
			}
		},
		methods: {
			async initProEvts() {
				try{
					if (this.deviceInfo == null || this.deviceInfo.length == {}) {
						this.proEvt = [];
						return;
					}
					let pinfo = (await productInfo({
						id: this.deviceInfo.ProductId,
						notsl: false
					})).data;
					if (pinfo.ModelTSL) {
						let tsl = JSON.parse(pinfo.ModelTSL);
						// console.log("tsl: ", tsl);
						
						if (this.dataType == 'event') {
							this.proEvt = tsl.events;
						} else if (this.dataType == 'attribute') {
							this.proEvt = tsl.properties;
							// console.log(tsl,'tsltsl');
							let rulesAdForm = this.$store.state.rulesAddForm
							// console.log(rulesAdForm, '规则');
							if(rulesAdForm.eventDevice&&rulesAdForm.eventDevice[0]&&rulesAdForm.eventDevice[0].Id===this.deviceInfo.Id){
								if(rulesAdForm.triggerWay===0&&rulesAdForm.eventinfo||rulesAdForm.triggerWay==='0'&&rulesAdForm.eventinfo){
									let needParams=tsl.events.filter(rw=>rw.code===rulesAdForm.eventinfo.code)
									let otherArr=needParams.map(row=>{
										row.outputs=row.outputs.map(ro=>{
											ro.option.type=ro.type
											if(ro.elements){
												ro.option.elements=ro.elements
											}
											ro.isEventParams=true
											this.proEvt.push(ro)
											return ro
										})
										return row
									})
								}
							}
							
							
							// console.log(otherArr,'事件参数处理');
						} else if (this.dataType == 'function') {
							this.proEvt = tsl.functions;
						}
						// console.log("设备事件", this.proEvt);
					}
				}catch(e) {
					console.log('错误', e);
				}

			},
			finishFunChoice(item, inx) {
				try {
					if (this.isDisable) {
						let rulesAdForm = this.$store.state.otherRulesForm
						if (this.dataType == 'attribute') {
							this.proEvt[inx].select = true
							this.proEvt.map((row, inx2) => {
								if (inx2 != inx) {
									row.select = false
								}
							})
							if (this.alReadChoice == item.code) {
								rulesAdForm.attrVal = JSON.parse(JSON.stringify(item))
								this.$store.commit('SET_OTHERRULES_INFO', rulesAdForm)
								this.$nextTick(() => {
									if(this.isOnlyRead){
										uni.navigateTo({
											url: '/pages_rules/attribute?haival=true&inx=' + this.infoIndex +
												'&allInx=' + this.allInx+'&isOnlyRead='+this.isOnlyRead
										})
									}else{
										uni.navigateTo({
											url: '/pages_rules/attribute?haival=true&inx=' + this.infoIndex +
												'&allInx=' + this.allInx
										})
									}
								})
							}
						}
					} else {
						this.proEvt[inx].select = true
						this.proEvt.map((row, inx2) => {
							if (inx2 != inx) {
								row.select = false
							}
						})
						this.$forceUpdate()
						let rulesAdForm = this.$store.state.otherRulesForm
						if (this.rulesType=='condition'&&this.dataType == 'event') {
							rulesAdForm.name = rulesAdForm.name + " " + item.name
							for (let i = 0; i < rulesAdForm.triggerList.length; i++) {
								rulesAdForm.triggerList[i].topicMsg = rulesAdForm.triggerList[i].topicMsg + "#" + item.code
							}
							rulesAdForm.eventinfo = JSON.parse(JSON.stringify(item))
							// this.$store.commit('SET_RULESADD_INFO', rulesAdForm)
							this.$store.commit('SET_OTHERRULES_INFO', rulesAdForm)
							if (this.infoIndex && this.allInx) {
								let obj = {
									infoIndex: this.infoIndex,
									allInx: this.allInx,
									rulesAdForm: rulesAdForm,
									rulesType:this.rulesType
								}
								setPagesParam('finshSelectEvent', obj, 2)
							} else {
								let obj = {
									rulesAdForm: rulesAdForm,
									rulesType:this.rulesType
								}
								setPagesParam('finshSelectEvent', obj, 2)
							}

						} else {
							if (this.dataType == 'attribute') {
								rulesAdForm.attrVal = JSON.parse(JSON.stringify(item))
								this.$store.commit('SET_OTHERRULES_INFO', rulesAdForm)
								if (this.infoIndex && this.allInx) {
									this.$nextTick(() => {
										if(this.isOnlyRead){
											uni.navigateTo({
												url: '/pages_rules/attribute?haival=true&inx=' + this
													.infoIndex + '&allInx=' + this.allInx+'&isOnlyRead='+this.isOnlyRead
											})
										}else{
											uni.navigateTo({
												url: '/pages_rules/attribute?haival=true&inx=' + this
													.infoIndex + '&allInx=' + this.allInx
											})
										}
										
									})
								} else {
									this.$nextTick(() => {
										uni.navigateTo({
											url: '/pages_rules/attribute'
										})
									})
								}
							} else if (this.dataType == 'function') {
								rulesAdForm.funcValue = JSON.parse(JSON.stringify(item))
								if (this.infoIndex && this.allInx) {
									let obj = {
										infoIndex: this.infoIndex,
										allInx: this.allInx,
										rulesAdForm: rulesAdForm
									}
									setPagesParam('finshSelectFunc', obj, 2)
								} else {
									let obj = {
										rulesAdForm: rulesAdForm
									}
									setPagesParam('finshSelectFunc', obj, 2)
								}
							}else if(this.rulesType=='function'&&this.dataType == 'event'){
								rulesAdForm.funcValue = JSON.parse(JSON.stringify(item))
								if (this.infoIndex && this.allInx) {
									let obj = {
										infoIndex: this.infoIndex,
										allInx: this.allInx,
										rulesAdForm: rulesAdForm
									}
									setPagesParam('finshSelectFuncEvent', obj, 2)
								} else {
									let obj = {
										rulesAdForm: rulesAdForm
									}
									setPagesParam('finshSelectFuncEvent', obj, 2)
								}
							}
						}
					}
				} catch (e) {
					//TODO handle the exception
					console.log('错误', e);
				}

			}
		}
	}
</script>

<style lang="less" scoped>
	.fun_con {
		width: 100%;
		padding: 0 20rpx;
		box-sizing: border-box;

		.fun_li {
			margin-top: 20rpx;
			width: 100%;
			height: 126rpx;
			background-color: #ffffff;
			border-radius: 10rpx;
			padding: 0 30rpx;
			box-sizing: border-box;
			display: flex;
			justify-content: space-between;
			align-items: center;

			.name {
				font-size: 32rpx;
				color: #333333;
			}
		}
	}
</style>