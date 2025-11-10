<template>
	<view style="height: 100%;overflow-x: hidden;" @click="isSelKd=false">
		<top :title="topTitle" leftText="Back" leftWidth="120rpx" :isleftBack="true" leftIcon="icon-fanhui"
			backgroundColor="#161A26" rightWidth="100rpx">
		</top>
		<view class="detail_con" style="padding-bottom:0">
			<view class="detail_status" v-if="enterId&&enterId!=null" style="padding-bottom: 0;">
				<custom-icons iconsName="icon-yituihuo" iconsSize="36rpx"
					iconsColor="rgba(255, 255, 255, 1)"></custom-icons>
				<view class="status_text">
					Returned
				</view>
			</view>
		</view>
		<uni-forms ref="outForm" :modelValue="outForm" :rules="rules" labelWidth='80' label-position="top">
			<view class="form_con page_form_con">
				<uni-forms-item label="Outbound number" required name="StockNumber" id="StockNumber_form"
					labelFont="32rpx" contentFont="32rpx" :requireOpacity="0.5">
					<view id="StockNumber" class="form_li">
						<uni-easyinput placeholderStyle="color:rgba(255, 255, 255, 0.2);font-size:32rpx"
							inputHeight="88rpx" :styles="styles" type="text" v-model="outForm.StockNumber"
							placeholder="Please enter warehouse name" contentFontSize="32rpx"
							primaryColor="rgba(255, 255, 255, 0.5)" :disabled="true" />
					</view>
				</uni-forms-item>
				<uni-forms-item label="Outbound time" required name="OutDate" id="OutDate_form" labelFont="32rpx"
					contentFont="32rpx" :requireOpacity="0.5">
					<view id="OutDate" class="form_li">
						<view class="int_date">
							<view class="date_con long_date" style="color: #fff;">
								<uni-datetime-picker ref="dateChoice1" class="date" type="datetime"
									placeholder-style="font-size:32rpx;color:#999999" :clear-icon="false"
									v-model="outForm.OutDate" placeholder='开始日期' :isCustom="true" :isDark="true">
									<view class="date_slot" :class="{'has_val':outForm.OutDate}">
										<custom-icons iconsName="icon-xuanzeshijian" iconsSize="28rpx"
											iconsColor="rgba(255, 255, 255, 0.5)"></custom-icons>
										<view class="text">
											{{outForm.OutDate?outForm.OutDate:'Start date'}}
										</view>

									</view>
								</uni-datetime-picker>
							</view>
						</view>
					</view>
				</uni-forms-item>
				<uni-forms-item label="Logistics tracking number" name="ExpressNumber" id="ExpressNumber_form"
					labelFont="32rpx" contentFont="32rpx" :requireOpacity="0.5">
					<view id="ExpressNumber" class="form_li">
						<view class="sel_input">
							<view class="sel" @click.stop="openKDSel">
								<view class="sel_text" :class="{'pal':!outForm.ExpressCompanyName}">
									{{outForm.ExpressCompanyName?outForm.ExpressCompanyName:'select'}}
								</view>
								<custom-icons iconsName="icon-xialajiantou" iconsSize="12rpx"
									iconsColor="rgba(255, 255, 255, 0.5)"></custom-icons>
							</view>
							<view class="sel_line"></view>
							<view class="input">
								<uni-easyinput placeholderStyle="color:rgba(255, 255, 255, 0.2);font-size:32rpx"
									inputHeight="88rpx" :styles="styles" type="text" v-model="outForm.ExpressNumber"
									placeholder="Please enter warehouse name" contentFontSize="32rpx"
									primaryColor="rgba(255, 255, 255, 0.5)" :inputBorder="false"
									@input="expressSelected" @blur="expressSelected" />
							</view>
							<view class="sel_list" v-show="isSelKd">
								<view class="sel_li" v-for="item in kdcompanys" @click.stop="choiceKd(item)">
									{{item.Name}}
								</view>
							</view>
						</view>

					</view>
				</uni-forms-item>
				<uni-forms-item label="contact phone number" required name="ExpressPhone" id="ExpressPhone_form"
					labelFont="32rpx" contentFont="32rpx" :requireOpacity="0.5"
					v-if="outForm.ExpressCompany == 'shunfeng'">
					<view id="ExpressPhone" class="form_li">
						<uni-easyinput placeholderStyle="color:rgba(255, 255, 255, 0.2);font-size:32rpx"
							inputHeight="88rpx" :styles="styles" type="text" v-model="outForm.ExpressPhone"
							placeholder="Please enter the contact phone number" contentFontSize="32rpx"
							primaryColor="rgba(255, 255, 255, 0.5)" />
					</view>
				</uni-forms-item>
				<uni-forms-item label="Notes" name="Remark" id="Remark_form" labelFont="32rpx" contentFont="32rpx"
					:requireOpacity="0.5">
					<view id="Remark" class="form_li">
						<uni-easyinput placeholderStyle="color:rgba(255, 255, 255, 0.2);font-size:32rpx"
							inputHeight="70rpx" :styles="styles" type="textarea" v-model="outForm.Remark"
							placeholder="Please enter the notes" contentFontSize="32rpx"
							primaryColor="rgba(255, 255, 255, 0.5)" autoHeight />
					</view>
				</uni-forms-item>
				<uni-forms-item label="Warehousing items" name="username" id="username_form" labelFont="32rpx"
					contentFont="32rpx" :requireOpacity="0.5" labelPosition="top">
					<view id="username" class="form_li">
						<view class="form_device_list">
							<view class="form_device_li" v-for="(item,inx) in outForm.List" :key="item.Id">
								<view class="li_left">
									<image class="image" :src="item.PhotoUrl+'?wh=500x500'" mode=""></image>
								</view>
								<view class="li_right">
									<view class="name">{{item.TargetName}}</view>
									<view class="number">{{item.TargetNumber}}</view>
									<view class="price_quantity" v-if="item.TargetType==1">
										<view class="input_li mar_input">
											<text class="label">Price:</text>
											<uni-number-box background="rgba(28, 34, 50, 1)" v-model="item.Price"
												color="rgba(255, 255, 255, 1)" :max="100000000" :disabled="true" />
										</view>
										<view class="input_li">
											<text class="label">Quantity:</text>
											<uni-number-box background="rgba(28, 34, 50, 1)" v-model="item.Quantity"
												color="rgba(255, 255, 255, 1)" :min="0" :max="1" :disabled="true" />
										</view>
									</view>
									<view class="price_quantity" v-if="item.TargetType==0">
										<view class="input_li mar_input">
											<text class="label">Price:</text>
											<uni-number-box background="rgba(28, 34, 50, 1)" v-model="item.Price"
												color="rgba(255, 255, 255, 1)" :max="100000000" :disabled="true" />
										</view>
										<view class="input_li">
											<text class="label">Quantity:</text>
											<uni-number-box background="rgba(28, 34, 50, 1)" v-model="item.Quantity"
												color="rgba(255, 255, 255, 1)" :disabled="true" />
										</view>
									</view>
								</view>
								<!-- <view class="del_icon" @click.stop="delSelDev(inx)">
									<view class="icons_del t-icon-yichu"></view>
								</view> -->
							</view>
							<!-- <view class="form_device_add" @click="toChoiceDevice">
								<custom-icons iconsName="icon-tianjia" iconsSize="24rpx"
									iconsColor="rgba(255, 255, 255, 0.5)"></custom-icons>
								<view class="text">
									Add item
								</view>
							</view> -->
						</view>
					</view>
				</uni-forms-item>

			</view>

		</uni-forms>
		<ProcessCompot ref="flowForm" @isChoiceFlowUserFun="isChoiceFlowUserFun" @setActiveItem="setActiveItem">
			Inventory review
		</ProcessCompot>
		<view class="form_con page_form_con">
			<button class="submit_button" @click="submitForm(2)" :disabled="isLoading"
				:style="{'opacity':isLoading?0.6:1}" :loading="isLoading">
				Submit
			</button>

		</view>
		<view class="zhanwei" style="width: 100%;height: 40rpx;">

		</view>
		<uni-popup ref="bottomPop" type="bottom" :mask-click="true" :zIndex="999"
			maskBackgroundColor="rgba(0, 0, 0, 0.5)">
			<view class="popup_list">
				<view class="popup_li borradio" v-if="isCheckPermi(['/CRMService/Parts/List'])"
					@click.stop="slectConsumables">
					Select consumables
				</view>
				<view class="popup_li" v-if="isCheckPermi(['/IoTService/IotDevice/ListPage'])"
					@click.stop="slectMachines">
					Select machines
				</view>
				<view class="popup_li" @click.stop="scanAdd">
					Scan code to add
				</view>
				<view class="popup_li cancel" @click="closePopup">
					Cancel
				</view>
			</view>
		</uni-popup>
		<msg-prompt ref="promptMsg"></msg-prompt>
	</view>
</template>

<script>
	import {
		enterDevList,
		manualPile,
		submitManualPile,
		generateCKNumber,
		getEnterInfo,
		cancelEnter
	} from "@/api/stock";
	import {
		getStockConfig
	} from "@/api/config";
	import ProcessCompot from "@/components/flow-form/process-compot.vue";
	import {
		autoCompany
	} from '@/api/code.js'
	import {
		setPagesParam
	} from '@/common/utillib.js'
	var dayjs = require('@/common/day.js')

	export default {
		components: {
			ProcessCompot,
		},
		data() {
			return {
				selectDeviceList: [],
				topTitle: 'Return of inbound items',
				isLoading: false,
				styles: {
					color: '#fff',
					backgroundColor: '#161A26',
					disableColor: 'rgba(28, 34, 50, 1)',
					borderColor: 'rgba(255, 255, 255, 0.20)'
				},
				outForm: {
					Id: undefined,
					StockNumber: "",
					OutDate: undefined,
					Remark: "",
					ExpressNumber: "",
					ExpressCompany: "",
					ExpressPhone: "",
					List: []
				},
				rules: {
					OutDate: [{
						required: true,
						message: "退货时间不能为空",
						trigger: "change"
					}]
				},
				enterId: null,
				leaveTemplateId: 0,
				kdcompanys: [], //快递公司列表
				isSelKd: false, //是否展示选择快递列表
				activeItemId: '', //执行跳转页面的组件id
				isChoiceUser: false,
				isChoiceDept: false,
				isChoiceDevice: false,
				choiceUser: '',
				choiceDept: '',
				choiceDevice: '',
				flowInfo: '', //流程信息
				isChoiceFlowUser: false, //是否是流程中的选择人员
				isPagesSelectMachines: false, //是页面内的设备选择还是流程组件的选择
			}
		},
		computed: {
			FlowParams: function() {
				return {
					"@from": this.outForm.StockNumber,
					"@LeaveMethod": "1"
				};
			}
		},
		onLoad(options) {
			this.$nextTick(async () => {
				try {
					this.$refs.promptMsg.loadingOpen()
					this.kdcompanys = await this.$store.dispatch("data/kuaiDiList");
					if (options.id) {
						let ssp = await generateCKNumber();
						this.outForm.StockNumber = ssp.data;
						this.outForm.Id = options.id;
						this.outForm.OutDate = this.parseTime(Date.now());
						this.enterId = options.id
						let enterInfo = await getEnterInfo(this.enterId);
						this.outForm.List = enterInfo.data.List;
						if (enterInfo.data.ToHouseEnterTemplateId) {
							this.leaveTemplateId = enterInfo.data.ToHouseEnterTemplateId
						}
					}

					// let rsp = await getStockConfig(0);
					// this.leaveTemplateId = rsp.data.LeaveTemplateId;

					if (this.isCheckPermi(['/FlowService/Flow'])) {

						if (this.leaveTemplateId > 0) {
							await this.$refs.flowForm.InitData(this.leaveTemplateId, this.FlowParams, null);
						}
					}
					console.log(this.kdcompanys, '快递公司');
					this.$refs.promptMsg.loadingColse()
				} catch (e) {
					//TODO handle the exception
					console.log(e);
				}
			})
		},
		onShow() {
			//使用onShow生命周期的特点实现对各种页面选择方法
			if (this.isChoiceUser) {
				this.$refs.flowForm.selectEmplee(this.isChoiceFlowUser, this.choiceUser, this.activeItemId)
				this.isChoiceUser = false
			}
			if (this.isChoiceDept) {
				console.log(this.activeItemId, 'this.activeItemId');
				this.$refs.flowForm.selectDept(this.choiceDept, this.activeItemId)
				this.isChoiceDept = false
			}
			if (this.isChoiceDevice) {
				this.$refs.flowForm.selectDevice(this.choiceDevice, this.activeItemId)
				this.isChoiceDevice = false
			}
		},
		methods: {
			async expressSelected(event) {
				console.log("快递编号", event);
				let val = ''
				if (event) {
					if (event.detail) {
						val = event.detail.value
					} else {
						val = event
					}
				} else {
					return
				}
				if (val.length < 8) return;
				this.outForm.ExpressCompany = (await autoCompany(val)).data;
				console.log("快递", this.outForm.ExpressCompany);
				let arr = this.kdcompanys.find(row => row.Code == this.outForm.ExpressCompany)
				console.log("arr查找", arr);
				this.outForm.ExpressCompanyName = arr.Name
			},
			openKDSel() { //打开快递选择列表
				this.isSelKd = !this.isSelKd
			},
			choiceKd(row) { //选择快递
				this.isSelKd = false
				this.outForm.ExpressCompany = row.Code
				this.outForm.ExpressCompanyName = row.Name
				this.$forceUpdate()
			},
			submitForm(st) {

				this.$refs.outForm.validate().then(async valid => {
					if (valid) {
						//提交审批表单
						this.isLoading = true
						if (this.leaveTemplateId > 0) {
							this.$refs.flowForm.submitForm(2, (rss) => {
								this.outForm.FlowId = rss.data;
								cancelEnter(this.outForm).then(response => {
									this.$refs.promptMsg.open('Operation successful', 1500)
									setTimeout(() => {
										setPagesParam('loadData', 'load', 1)
									}, 1500)
								}).catch(e => {
									this.setMsgTop(e)
									this.isLoading = false
								});

							})
						} else {
							this.outForm.FlowId = 0;
							cancelEnter(this.outForm).then(response => {
								this.$refs.promptMsg.open('Operation successful', 1500)
								setTimeout(() => {
									setPagesParam('loadData', 'load', 1)
								}, 1500)
							}).catch(e => {
								this.setMsgTop(e)
								this.isLoading = false
							});
						}

						if (this.outForm.List.length == 0) {
							this.$refs.promptMsg.open('Please select the items to be stored in the warehouse',
								1500)
							return;
						}


					}
				}).catch(err => {
					console.log("err", err);
					if (err && err.length > 0) {
						let firstErr = '#' + err[0].key
						// #ifdef MP-WEIXIN
						const query = uni.createSelectorQuery().in(this);
						query.select(firstErr).boundingClientRect(data => {
							uni.pageScrollTo({
								scrollTop: data.top - 200,
								// selector: firstErr,
								duration: 300
							});

						}).exec();
						// #endif
						// #ifndef MP-WEIXIN 
						uni.pageScrollTo({
							selector: firstErr + '_form',
							duration: 300
						});
						// #endif  
					}
				});
			},
			selectHouse(item) {
				console.log(item, 'itemitemitem');
				if (item) {
					this.outForm.HouseId = item.Id
					this.outForm.HouseName = item.StoreName
				}
			},
			choiceWarehouse() {
				if (this.outForm.HouseId) {
					uni.navigateTo({
						url: '/pages_Inventory/warehouse_management?isSelect=true&&selectId=' + this.outForm
							.HouseId
					})
				} else {
					uni.navigateTo({
						url: '/pages_Inventory/warehouse_management?isSelect=true'
					})
				}
			},
			//流程组件相关函数
			isChoiceFlowUserFun(val) {
				this.isChoiceFlowUser = val
			},
			setActiveItem(val) {
				console.log(val, 'val');
				this.activeItemId = val
				// this.$refs.setActiveItem.selectDept(this.activeItemId)
			},
			selectDept(val) {
				//选择部门

				this.isChoiceDept = true
				// this.$refs.flowForm.selectDept(val, this.activeItemId)
				this.choiceDept = val
			},
			selectEmplee(val) {
				//选择人员
				// this.$refs.flowForm.selectEmplee(this.isChoiceFlowUser, val, this.activeItemId)
				this.isChoiceUser = true
				this.choiceUser = val
			},
			//流程组件相关函数
		}
	}
</script>

<style lang="less" scoped>
	.popup_list {
		background-color: rgba(22, 26, 38, 1);
		border-radius: 20rpx 20rpx 0 0;

		.popup_li {
			height: 120rpx;
			line-height: 120rpx;
			text-align: center;
			color: rgba(255, 255, 255, 1);
			font-size: 36rpx;

			&.borradio {
				border-radius: 20rpx 20rpx 0 0;
			}

			&.cancel {
				color: rgba(255, 255, 255, 0.5);
			}
		}
	}
</style>