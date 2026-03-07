<template>
	<view style="height: 100%;overflow-x: hidden;" @click="isSelKd=false">
		<top :title="topTitle" leftWidth="120rpx" :isleftBack="true" leftIcon="icon-fanhui" backgroundColor="#ffffff"
			rightWidth="100rpx">
		</top>
		<view class="detail_con" style="background-color: rgba(245, 248, 249, 1);padding-bottom:0">
			<view class="detail_status" v-if="enterId&&enterId!=null">
				<custom-icons iconsName="icon-yituihuo" iconsSize="36rpx" iconsColor="#333333"></custom-icons>
				<view class="status_text">
					退货
				</view>
			</view>
		</view>
		<uni-forms ref="outForm" :modelValue="outForm" :rules="rules" labelWidth='80' label-position="top">
			<view class="form_con page_form_con" style="background-color: #fff;">
				<uni-forms-item label="出库单号" required name="StockNumber" id="StockNumber_form" labelFont="32rpx"
					contentFont="32rpx" :requireOpacity="0.5">
					<view id="StockNumber" class="form_li">
						<uni-easyinput placeholderStyle="color:rgba(255, 255, 255, 0.2);font-size:32rpx"
							inputHeight="88rpx" :styles="styles" type="text" v-model="outForm.StockNumber"
							placeholder="请输入出库单号" contentFontSize="32rpx"
							primaryColor="rgba(35, 113, 255, 1)" :disabled="true" />
					</view>
				</uni-forms-item>
				<uni-forms-item label="退货时间" required name="OutDate" id="OutDate_form" labelFont="32rpx"
					contentFont="32rpx" :requireOpacity="0.5">
					<view id="OutDate" class="form_li">
						<view class="int_date">
							<view class="date_con long_date" style="color: #fff;">
								<uni-datetime-picker ref="dateChoice1" class="date" type="datetime"
									placeholder-style="font-size:32rpx;color:#999999" :clear-icon="false"
									v-model="outForm.OutDate" placeholder='开始日期' :isCustom="true" :isDark="false">
									<view class="date_slot" :class="{'has_val':outForm.OutDate}">
										<custom-icons iconsName="icon-xuanzeshijian" iconsSize="28rpx"
											iconsColor="#999999"></custom-icons>
										<view class="text">
											{{outForm.OutDate?outForm.OutDate:'退货时间'}}
										</view>
									</view>
								</uni-datetime-picker>
							</view>
						</view>
					</view>
				</uni-forms-item>
				<uni-forms-item label="物流单号" name="ExpressNumber" id="ExpressNumber_form" labelFont="32rpx"
					contentFont="32rpx" :requireOpacity="0.5">
					<view id="ExpressNumber" class="form_li">
						<view class="sel_input">
							<view class="sel" @click.stop="openKDSel">
								<view class="sel_text" :class="{'pal':!outForm.ExpressCompanyName}">
									{{outForm.ExpressCompanyName?outForm.ExpressCompanyName:'请选择'}}
								</view>
								<custom-icons iconsName="icon-xialajiantou" iconsSize="12rpx"
									iconsColor="#999999"></custom-icons>
							</view>
							<view class="sel_line"></view>
							<view class="input">
								<uni-easyinput placeholderStyle="color:#C1C1C1;font-size:32rpx" inputHeight="88rpx"
									:styles="styles" type="text" v-model="outForm.ExpressNumber" placeholder="请输入物流编号"
									contentFontSize="32rpx" primaryColor="rgba(35, 113, 255, 1)"
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
				<uni-forms-item label="联系手机号" required name="ExpressPhone" id="ExpressPhone_form" labelFont="32rpx"
					contentFont="32rpx" :requireOpacity="0.5" v-if="outForm.ExpressCompany == 'shunfeng'">
					<view id="ExpressPhone" class="form_li">
						<uni-easyinput placeholderStyle="color:#C1C1C1;font-size:32rpx" inputHeight="88rpx"
							:styles="styles" type="text" v-model="outForm.ExpressPhone" placeholder="请输入物流联系手机"
							contentFontSize="32rpx" primaryColor="rgba(35, 113, 255, 1)" />
					</view>
				</uni-forms-item>
				<uni-forms-item label="备注说明" name="Remark" id="Remark_form" labelFont="32rpx" contentFont="32rpx"
					:requireOpacity="0.5">
					<view id="Remark" class="form_li">
						<uni-easyinput placeholderStyle="color:#C1C1C1;font-size:32rpx" inputHeight="70rpx"
							:styles="styles" type="textarea" v-model="outForm.Remark" placeholder="请输入备注"
							contentFontSize="32rpx" primaryColor="rgba(35, 113, 255, 1)" autoHeight :isCustom="true" />
					</view>
				</uni-forms-item>
				<uni-forms-item label="退货物品" name="username" id="username_form" labelFont="32rpx" contentFont="32rpx"
					:requireOpacity="0.5" labelPosition="top">
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
											<text class="label">价格:</text>
											<uni-number-box background="#F8F8F8" v-model="item.Price" color="#333333"
												:max="100000000" :disabled="true" />
										</view>
										<view class="input_li">
											<text class="label">数量:</text>
											<uni-number-box background="#F8F8F8" v-model="item.Quantity" color="#333333"
												:min="0" :max="1" :disabled="true" />
										</view>
									</view>
									<view class="price_quantity" v-if="item.TargetType==0">
										<view class="input_li mar_input">
											<text class="label">价格:</text>
											<uni-number-box background="#F8F8F8" v-model="item.Price" color="#333333"
												:max="100000000" :disabled="true" />
										</view>
										<view class="input_li">
											<text class="label">数量:</text>
											<uni-number-box background="#F8F8F8" v-model="item.Quantity" color="#333333"
												:disabled="true" />
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
			出库审核
		</ProcessCompot>
		<view class="form_con page_form_con" style="background-color: #fff;">
			<button class="submit_button" @click="submitForm(2)" :disabled="isLoading"
				:style="{'opacity':isLoading?0.6:1}" :loading="isLoading">
				提交
			</button>
		</view>
		<view class="zhanwei" style="width: 100%;height: 40rpx;">

		</view>
		<uni-popup ref="bottomPop" type="bottom" :mask-click="true" :zIndex="999"
			maskBackgroundColor="rgba(0, 0, 0, 0.5)">
			<view class="popup_list">
				<view class="popup_li borradio" v-if="isCheckPermi(['/ProducerService/Parts/List'])"
					@click.stop="slectConsumables">
					选择耗材
				</view>
				<view class="popup_li" v-if="isCheckPermi(['/IoTService/IotDevice/ListPage'])"
					@click.stop="slectMachines">
					选择设备
				</view>
				<view class="popup_li" @click.stop="scanAdd">
					扫码添加
				</view>
				<view class="popup_li cancel" @click="closePopup">
					取消
				</view>
			</view>
		</uni-popup>
		<msg-prompt ref="promptMsg" @confirm="confirmDelete"></msg-prompt>
	</view>
</template>

<script>
	import {
		enterDevList,
		manualPile,
		leaveFormData,
		submitManualPile,
		generateCKNumber,
		getEnterInfo,
		cancelEnter,
		cancelEnterModel
	} from "@/api/stock";
	import {
		getStockConfig
	} from "@/api/config";
	import ProcessCompot from "@/pages_flow/flow-form/process-compot.vue";
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
				topTitle: '退货',
				isLoading: false,
				styles: {
					color: '#333',
					backgroundColor: '#F8F8F8',
					disableColor: '#F8F8F8',
					borderColor: '#F8F8F8'
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
				cancelHouseId:0
			}
		},
		computed: {
			FlowParams: function() {
				return {
					"@from": this.outForm.StockNumber,
					"@fromtype": "出库单",
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
						console.log("入库单详情",enterInfo);
						this.cancelHouseId=enterInfo.data.ToHouseId
						if (enterInfo.data.FromHouseLeaveTemplateId) {
							this.leaveTemplateId = enterInfo.data.FromHouseLeaveTemplateId
						}
					}
					if (this.isCheckPermi(['/FlowService/Flow'])) {

						if (this.leaveTemplateId > 0) {
							let fromInfo=await leaveFormData({
							  "applyNumber": this.outForm.StockNumber,
							  "fromHouseId": this.cancelHouseId,
							})
							await this.$refs.flowForm.InitData(
							  this.leaveTemplateId,
							  this.FlowParams,
							  this.leaveId == null ? null : this.outForm.StockNumber,
							  fromInfo.data
							);
						}
					}
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
				let arr = this.kdcompanys.find(row => row.Code == this.outForm.ExpressCompany)
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
						if (this.outForm.List.length == 0) {
							this.$refs.promptMsg.open('请选择要出库的物品',
								1500)
							return;
						}
						this.isLoading = true
						let tmpmodel = this.$refs.flowForm.getModel();
						let mergedObj1 = Object.assign({}, tmpmodel, this.outForm);
						cancelEnterModel(mergedObj1).then(response => {
							this.$refs.promptMsg.open('操作成功', 1500)
							setTimeout(() => {
								setPagesParam('loadData', 'load', 1)
							}, 1500)
						}).catch(e => {
							this.setMsgTop(e)
							this.isLoading = false
						});
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
				if (item) {
					this.outForm.HouseId = item.Id
					this.outForm.HouseName = item.StoreName
				}
			},
			choiceWarehouse() {
				if (this.outForm.HouseId) {
					uni.navigateTo({
						url: '/pages_flow/inventory/warehouse_management?isSelect=true&&selectId=' + this.outForm
							.HouseId
					})
				} else {
					uni.navigateTo({
						url: '/pages_flow/inventory/warehouse_management?isSelect=true'
					})
				}
			},
			//流程组件相关函数
			isChoiceFlowUserFun(val) {
				this.isChoiceFlowUser = val
			},
			setActiveItem(val) {
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