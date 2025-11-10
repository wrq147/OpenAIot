<template>
	<view style="height: 100%;overflow-x: hidden;" @click="isSelKd=false">
		<top :title="topTitle" leftText="Back" leftWidth="120rpx" :isleftBack="true" leftIcon="icon-fanhui"
			backgroundColor="#161A26" rightWidth="100rpx">
		</top>
		<view class="detail_con" style="padding-bottom:0">
			<view class="detail_status" v-if="leaveId&&leaveId!=null" style="padding-bottom: 0;">
				<custom-icons iconsName="icon-daitijiao" iconsSize="36rpx"
					iconsColor="rgba(255, 255, 255, 1)"></custom-icons>
				<view class="status_text">
					Pending submission
				</view>
			</view>
		</view>
		<uni-forms ref="recordsForm" :modelValue="recordsForm" :rules="rules" labelWidth='80' label-position="top"
			v-if="recordsForm.StockNumber">
			<view class="form_con page_form_con">
				<uni-forms-item label="outbound tracking number" required name="StockNumber" id="StockNumber_form"
					labelFont="32rpx" contentFont="32rpx" :requireOpacity="0.5">
					<view id="StockNumber" class="form_li">
						<uni-easyinput placeholderStyle="color:rgba(255, 255, 255, 0.2);font-size:32rpx"
							inputHeight="88rpx" :styles="styles" type="text" v-model="recordsForm.StockNumber"
							placeholder="Please enter the outbound tracking number" contentFontSize="32rpx"
							primaryColor="rgba(255, 255, 255, 0.5)" :disabled="true" />
					</view>
				</uni-forms-item>
				<uni-forms-item label="Warehousing time" required name="OutDate" id="OutDate_form" labelFont="32rpx"
					contentFont="32rpx" :requireOpacity="0.5">
					<view id="OutDate" class="form_li">
						<view class="int_date">
							<view class="date_con long_date" style="color: #fff;">
								<uni-datetime-picker ref="dateChoice1" class="date" type="datetime"
									placeholder-style="font-size:32rpx;color:#999999" :clear-icon="false"
									v-model="recordsForm.OutDate" placeholder='Please select the outbound time'
									:isCustom="true" :isDark="true">
									<view class="date_slot" :class="{'has_val':recordsForm.OutDate}">
										<custom-icons iconsName="icon-xuanzeshijian" iconsSize="28rpx"
											iconsColor="rgba(255, 255, 255, 0.5)"></custom-icons>
										<view class="text">
											{{recordsForm.OutDate?recordsForm.OutDate:'Please select the outbound time'}}
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
								<view class="sel_text" :class="{'pal':!recordsForm.ExpressCompanyName}">
									{{recordsForm.ExpressCompanyName?recordsForm.ExpressCompanyName:'select'}}
								</view>
								<custom-icons iconsName="icon-xialajiantou" iconsSize="12rpx"
									iconsColor="rgba(255, 255, 255, 0.5)"></custom-icons>
							</view>
							<view class="sel_line"></view>
							<view class="input">
								<uni-easyinput placeholderStyle="color:rgba(255, 255, 255, 0.2);font-size:32rpx"
									inputHeight="88rpx" :styles="styles" type="text" v-model="recordsForm.ExpressNumber"
									placeholder="Please enter logistics tracking number" contentFontSize="32rpx"
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
					v-if="recordsForm.ExpressCompany == 'shunfeng'">
					<view id="ExpressPhone" class="form_li">
						<uni-easyinput placeholderStyle="color:rgba(255, 255, 255, 0.2);font-size:32rpx"
							inputHeight="88rpx" :styles="styles" type="text" v-model="recordsForm.ExpressPhone"
							placeholder="Please enter the contact phone number" contentFontSize="32rpx"
							primaryColor="rgba(255, 255, 255, 0.5)" />
					</view>
				</uni-forms-item>
				<uni-forms-item label="Out of warehouse" required name="FromHouseId" id="FromHouseId_form"
					labelFont="32rpx" contentFont="32rpx" :requireOpacity="0.5">
					<view id="FromHouseId" class="form_li" @click="choiceWarehouse('from')">
						<!-- <uni-easyinput placeholderStyle="color:rgba(255, 255, 255, 0.2);font-size:32rpx"
							inputHeight="88rpx" :styles="styles" type="text" v-model="recordsForm.FromHouseName"
							placeholder="Please select a Warehouse" contentFontSize="32rpx"
							primaryColor="rgba(255, 255, 255, 0.5)" :clearable="false" :disabled="true"/> -->
						<view class="form_input"
							:class="{'placeholder_input':!recordsForm.FromHouseName||recordsForm.FromHouseName==''}">
							{{recordsForm.FromHouseName?recordsForm.FromHouseName:'Please select a Warehouse'}}
						</view>
						<view class="form_sel_icon">
							<custom-icons iconsName="icon-a-youjiantoubai" iconsSize="20rpx"
								iconsColor="rgba(255, 255, 255, 0.5)"></custom-icons>
						</view>
					</view>
				</uni-forms-item>
				<uni-forms-item label="Target Warehouse" required name="ToHouseId" id="ToHouseId_form" labelFont="32rpx"
					contentFont="32rpx" :requireOpacity="0.5" v-if="recordsForm.LeaveMethod==2"
					:rules="rules.ToHouseId.rules">
					<view id="ToHouseId" class="form_li" @click="choiceWarehouse('to')">
						<!-- <uni-easyinput placeholderStyle="color:rgba(255, 255, 255, 0.2);font-size:32rpx"
							inputHeight="88rpx" :styles="styles" type="text" v-model="recordsForm.ToHouseName"
							placeholder="Please select a Warehouse" contentFontSize="32rpx"
							primaryColor="rgba(255, 255, 255, 0.5)" :clearable="false" :disabled="true"/> -->
						<view class="form_input"
							:class="{'placeholder_input':!recordsForm.ToHouseName||recordsForm.ToHouseName==''}">
							{{recordsForm.ToHouseName?recordsForm.ToHouseName:'Please select a Warehouse'}}
						</view>
						<view class="form_sel_icon">
							<custom-icons iconsName="icon-a-youjiantoubai" iconsSize="20rpx"
								iconsColor="rgba(255, 255, 255, 0.5)"></custom-icons>
						</view>
					</view>
				</uni-forms-item>
				<uni-forms-item label="Target customer" required name="ToOrgId" id="ToOrgId_form" labelFont="32rpx"
					contentFont="32rpx" :requireOpacity="0.5" v-if="recordsForm.LeaveMethod==0">
					<view id="ToOrgId" class="form_li" @click="choiceToOrgId">
						<!-- <uni-easyinput placeholderStyle="color:rgba(255, 255, 255, 0.2);font-size:32rpx"
							inputHeight="88rpx" :styles="styles" type="text" v-model="recordsForm.ToName"
							placeholder="Please select the target company" contentFontSize="32rpx"
							primaryColor="rgba(255, 255, 255, 0.5)" :clearable="false" :disabled="true" /> -->
						<view class="form_input"
							:class="{'placeholder_input':!recordsForm.ToName||recordsForm.ToName==''}">
							{{recordsForm.ToName?recordsForm.ToName:'Please select the target company'}}
						</view>
						<view class="form_sel_icon">
							<custom-icons iconsName="icon-a-youjiantoubai" iconsSize="20rpx"
								iconsColor="rgba(255, 255, 255, 0.5)"></custom-icons>
						</view>
					</view>
				</uni-forms-item>
				<uni-forms-item label="Notes" name="Remark" id="Remark_form" labelFont="32rpx" contentFont="32rpx"
					:requireOpacity="0.5">
					<view id="Remark" class="form_li">
						<uni-easyinput placeholderStyle="color:rgba(255, 255, 255, 0.2);font-size:32rpx"
							inputHeight="70rpx" :styles="styles" type="textarea" v-model="recordsForm.Remark"
							placeholder="Please enter the notes" contentFontSize="32rpx"
							primaryColor="rgba(255, 255, 255, 0.5)" autoHeight />
					</view>
				</uni-forms-item>
				<uni-forms-item label="Warehousing items" required name="List" id="List_form" labelFont="32rpx"
					contentFont="32rpx" :requireOpacity="0.5" labelPosition="top">
					<view id="List" class="form_li">
						<view class="form_device_list">
							<view class="form_device_li" v-for="(item,inx) in recordsForm.List" :key="item.Id">
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
												color="rgba(255, 255, 255, 1)" :max="100000000" :isminCustom="true"/>
										</view>
										<view class="input_li">
											<text class="label">Quantity:</text>
											<uni-number-box background="rgba(28, 34, 50, 1)" v-model="item.Quantity"
												color="rgba(255, 255, 255, 1)" :min="item.Quantity"
												:max="item.Quantity" :isminCustom="true" />
										</view>
									</view>
									<view class="price_quantity" v-if="item.TargetType==0">
										<view class="input_li mar_input">
											<text class="label">Price:</text>
											<uni-number-box background="rgba(28, 34, 50, 1)" v-model="item.Price"
												color="rgba(255, 255, 255, 1)" :max="100000000" :isminCustom="true" />
										</view>
										<view class="input_li">
											<text class="label">Quantity:</text>
											<uni-number-box background="rgba(28, 34, 50, 1)" v-model="item.Quantity"
												color="rgba(255, 255, 255, 1)" :min="1" :max="item.Quantity" :isminCustom="true" />
										</view>
									</view>
								</view>
								<view class="del_icon" @click.stop="delSelDev(inx)">
									<view class="icons_del t-icon-yichu"></view>
								</view>
							</view>
							<view class="form_device_add" @click="toChoiceDevice">
								<custom-icons iconsName="icon-tianjia" iconsSize="24rpx"
									iconsColor="rgba(255, 255, 255, 0.5)"></custom-icons>
								<view class="text">
									Add item
								</view>
							</view>
						</view>
					</view>
				</uni-forms-item>

			</view>

		</uni-forms>
		<ProcessCompot ref="flowForm" @isChoiceFlowUserFun="isChoiceFlowUserFun" @setActiveItem="setActiveItem"
			v-if="recordsForm.StockNumber">
			Inventory review
		</ProcessCompot>
		<view class="form_con page_form_con" v-if="recordsForm.StockNumber">
			<view class="sub_save_con" :class="{'has_btn':leaveId&&leaveId!=null}">
				<button class="delete_button" @click="cancelWare" :disabled="isLoading"
					:style="{'opacity':isLoading?0.6:1}" :loading="isLoading" v-if="leaveId&&leaveId!=null">
					revoke
				</button>
				<button class="submit_button" @click="submitForm(1)" :disabled="isLoading"
					:style="{'opacity':isLoading?0.6:1}" :loading="isLoading">
					Save
				</button>
				<button class="jump_button" @click="submitForm(2)" :disabled="isLoading"
					:style="{'opacity':isLoading?0.6:1}" :loading="isLoading">
					Submit
				</button>
			</view>
		</view>
		<view class="zhanwei" style="width: 100%;height: 40rpx;">

		</view>
		<uni-popup ref="bottomPop" type="bottom" :mask-click="true" :zIndex="999"
			maskBackgroundColor="rgba(0, 0, 0, 0.5)">
			<view class="popup_list">
				<view class="popup_li borradio" @click.stop="selectItems">
					Select items
				</view>
				<view class="popup_li" @click.stop="scanAdd">
					Scan code to add
				</view>
				<view class="popup_li cancel" @click="closePopup">
					Cancel
				</view>
			</view>
		</uni-popup>
		<uni-popup ref="customPop" type="bottom" :mask-click="true" :zIndex="999"
			maskBackgroundColor="rgba(0, 0, 0, 0.5)">
			<view class="popup_list">
				<view class="popup_li borradio" @click.stop="selectAgents">
					My agents
				</view>
				<view class="popup_li" @click.stop="selectCustoms">
					My customers
				</view>
				<view class="popup_li cancel" @click="closePopup">
					Cancel
				</view>
			</view>
		</uni-popup>
		<msg-prompt ref="promptMsg" @confirm="confirmCancel"></msg-prompt>
	</view>
</template>

<script>
	import {
		leaveAdd,
		leaveEdit,
		generateCKNumber,
		leaveSubmit,
		stockList,
		getLeaveInfo,
		OutStockByKey,
		cancelLeave
	} from "@/api/stock";
	import {
		houseList,
	} from "@/api/house.js";
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
				topTitle: 'Create outbound records',
				isLoading: false,
				styles: {
					color: '#fff',
					backgroundColor: '#161A26',
					disableColor: 'rgba(28, 34, 50, 1)',
					borderColor: 'rgba(255, 255, 255, 0.20)'
				},
				recordsForm: {
					LeaveMethod: 0,
					StockNumber: "",
					ToOrgId: undefined,
					ToName: "",
					FromHouseId: undefined,
					FromHouseName: "",
					ToHouseId: undefined,
					ToHouseName: "",
					OutDate: undefined,
					Remark: "",
					ExpressNumber: "",
					ExpressCompany: "",
					ExpressPhone: "",
					List: []
				},
				rules: {
					StockNumber: {
						rules: [{
							required: true,
							errorMessage: 'Please enter the outbound tracking number',
						}]
					},
					OutDate: {
						rules: [{
							required: true,
							errorMessage: 'Please select the outbound time',
						}]
					},
					FromHouseId: {
						rules: [{
							required: true,
							errorMessage: 'Please select the outbound warehouse',
						}]
					},
					ToHouseId: {
						rules: [{
							required: true,
							errorMessage: 'Please select the target warehouse',
						}]
					},
					ToOrgId: {
						rules: [{
							required: true,
							errorMessage: 'Please select the target company',
						}]
					},
					ExpressPhone: {
						rules: [{
							required: true,
							errorMessage: 'Please enter the contact phone number',
						}]
					},
					List: {
						rules: [{
							required: true,
							errorMessage: 'Please select the outbound item',
						}]
					},
				},
				leaveId: null,
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
				showAgent: false, //是否显示选择代理
				showKf: false, //是否显示选择客户
				wupinSelectArr: [], //刚选择完重复的物品
				isShowSelectTips: false, //是否显示选择提示
				activeHouse: ''
			}
		},
		computed: {
			FlowParams: function() {
				return {
					"@from": this.recordsForm.StockNumber,
					"@LeaveMethod": this.recordsForm.LeaveMethod == 0 ? "0" : "2"
				};
			}
		},
		onLoad(options) {
			this.$nextTick(async () => {
				try {
					this.$refs.promptMsg.loadingOpen()
					this.kdcompanys = await this.$store.dispatch("data/kuaiDiList");
					if (options.id) {
						this.leaveId = options.id
						let leaveInfo = await getLeaveInfo(this.leaveId);
						console.log(leaveInfo, 'leaveInfo出库单信息');
						this.recordsForm.StockNumber = leaveInfo.data.StockNumber;
						this.recordsForm.FromHouseName = leaveInfo.data.FromHouseName;
						this.recordsForm.OutDate = leaveInfo.data.OutDate;
						this.recordsForm.FromHouseId = leaveInfo.data.FromHouseId;
						this.recordsForm.ToHouseId = leaveInfo.data.ToHouseId;
						this.recordsForm.ToHouseName = leaveInfo.data.ToHouseName;
						this.recordsForm.ToOrgId = leaveInfo.data.ToOrgId;
						this.recordsForm.ToName = leaveInfo.data.ToName;
						this.recordsForm.Remark = leaveInfo.data.Remark;
						this.recordsForm.ExpressNumber = leaveInfo.data.ExpressNumber;
						this.recordsForm.ExpressCompany = leaveInfo.data.ExpressCompany;
						let arr = this.kdcompanys.find(row => row.Code == this.recordsForm.ExpressCompany)
						this.recordsForm.ExpressCompanyName = arr.Name
						this.recordsForm.ExpressPhone = leaveInfo.data.ExpressPhone;
						this.recordsForm.List = leaveInfo.data.List;
						if(leaveInfo.data.FromHouseLeaveTemplateId){
							this.leaveTemplateId=leaveInfo.data.FromHouseLeaveTemplateId
						}else{
							this.leaveTemplateId=0
						}
						this.topTitle = 'Edit outbound records'
					} else {
						let ssp = await generateCKNumber();
						this.recordsForm.StockNumber = ssp.data;
						this.recordsForm.OutDate = dayjs(new Date()).format('YYYY-MM-DD HH:mm:ss')
						if (options.LeaveMethod) {
							this.recordsForm.LeaveMethod = options.LeaveMethod
						}
						this.topTitle = 'Create outbound records'
						let hslist = (await houseList({
							IsSystem: true
						})).data.List;
						if (hslist.length > 0) {
							this.recordsForm.FromHouseName = hslist[0].StoreName;
							this.recordsForm.FromHouseId = hslist[0].Id;
							this.leaveTemplateId = hslist[0].LeaveTemplateId;
						} else {
							this.leaveTemplateId = 0;
						}
						this.$forceUpdate()
					}
					setTimeout(async ()=>{
						try{
							if (this.isCheckPermi(['/FlowService/Flow'])) {
							
								if (this.leaveTemplateId > 0) {
									await this.$refs.flowForm.InitData(this.leaveTemplateId, this.FlowParams, this.leaveId == null ? null : this.recordsForm.StockNumber);
								}
							}
							
							console.log(this.kdcompanys, '快递公司');
							if (this.isCheckPermi(['/CRMService/Agent/AllList'])) {
								this.showAgent = true;
							}
							if (this.isCheckPermi(['/CRMService/Customer/List'])) {
								this.showKf = true;
							}
							this.$refs.promptMsg.loadingColse()
						}catch(e){
							//TODO handle the exception
							this.$refs.promptMsg.loadingColse()
							this.setMsgTop(e)
						}
					},100)
				} catch (e) {
					//TODO handle the exception
					console.log(e);
					this.$refs.promptMsg.loadingColse()
					this.setMsgTop(e)
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
			if (this.isShowSelectTips) {
				this.$refs.promptMsg.open("Cannot add items '" + this.wupinSelectArr.map(x => x.TargetName).join() +
					"repeatedly'", 2000)
				this.isShowSelectTips = false
			}
		},
		methods: {
			async confirmCancel() {
				//确认撤销动作
				try {
					this.isLoading = true
					await cancelLeave({
						id: this.leaveId
					});
					this.$refs.promptMsg.open('Operation successful', 1500)
					setTimeout(() => {
						setPagesParam('loadData', 'load', 1)
					}, 1500)
				} catch (e) {
					//TODO handle the exception
					this.setMsgTop(e)
					this.isLoading = false
				}
			},
			async cancelWare() {
				try {
					this.$refs.promptMsg.noticeOpen(
						'Are you sure you want to cancel the outbound order ' + this.recordsForm.StockNumber +
						'? (This operation is irreversible)?'
					)
				} catch (e) {
					console.log(e);
				}
			},
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
				this.recordsForm.ExpressCompany = (await autoCompany(val)).data;
				let arr = this.kdcompanys.find(row => row.Code == this.recordsForm.ExpressCompany)
				this.recordsForm.ExpressCompanyName = arr.Name
			},
			openKDSel() { //打开快递选择列表
				this.isSelKd = !this.isSelKd
			},
			choiceKd(row) { //选择快递
				this.isSelKd = false
				this.recordsForm.ExpressCompany = row.Code
				this.recordsForm.ExpressCompanyName = row.Name
				this.$forceUpdate()
			},
			delSelDev(inx) {
				//移出
				this.recordsForm.List.splice(inx, 1)
				console.log(this.recordsForm.List, 'this.recordsForm.Listthis.recordsForm.List');
			},
			finishSelectItems(arr) {
				if (arr.length > 0) {

					let filteritems = this.recordsForm.List.filter(x => arr.some(w => w.TargetId == x.TargetId));
					if (filteritems.length > 0) {
						this.wupinSelectArr = JSON.parse(JSON.stringify(filteritems))
						this.isShowSelectTips = true
						// this.$message.error("不可以重复添加物品 '" + filteritems.map(x => x.TargetName).join() + "'");
						return;
					}
					arr.map(element => {
						this.recordsForm.List.push({
							TargetType: element.TargetType,
							TargetId: element.TargetId,
							TargetNumber: element.DeviceNumber,
							Quantity: element.Quantity,
							Price: element.Price,
							TargetName: element.Name,
							PhotoUrl: element.PhotoUrl,
							Unit: element.Unit ? element.Unit : ''
						});
					});
				}
				console.log(this.recordsForm.List, '设备列表');
			},
			selectItems() {
				//选择物品
				if (this.recordsForm.FromHouseId) {
					this.$refs.bottomPop.close()
					uni.navigateTo({
						url: '/pages_Inventory/inventory_list?isSelect=true&isMulSelect=true&HouseId=' + this
							.recordsForm.FromHouseId
					})
				} else {
					this.$refs.promptMsg.open('Please select the warehouse to be shipped first', 2000)
				}

			},
			selectDevice(selarr) {
				//选择完设备
				if (this.isPagesSelectMachines) {
					this.isPagesSelectMachines = false
				} else {
					//选择设备
					// this.$refs.form.selectDept(val,this.activeItemId)
					this.isChoiceDevice = true
					// this.$refs.flowForm.selectDept(val, this.activeItemId)
					this.choiceDevice = selarr
				}
			},
			scanAddItems(scanArr) {
				//扫码完获取物品信息
				if (scanArr && scanArr.TargetId) {
					var tmpitems = this.recordsForm.List.filter(x => x.TargetId == scanArr.TargetId);
					if (tmpitems.length > 0) {
						if (scanArr.TargetType == 0) {
							tmpitems[0].Quantity += 1;
						}
					} else {
						this.recordsForm.List.push({
							TargetType: scanArr.TargetType,
							TargetId: scanArr.TargetId,
							TargetNumber: scanArr.DeviceNumber,
							Quantity: 1,
							Price: 0,
							TargetName: scanArr.TargetName,
							PhotoUrl: scanArr.PhotoUrl,
							Unit: scanArr.Unit ? scanArr.Unit : ''
						});
					}
				}
			},
			scanAdd() {
				//扫码添加
				let that = this
				uni.scanCode({
					success: function(res) {
						console.log("res", res);
						let currenturl = res.result
						let targetUID = ''
						let tmpidx = res.result.lastIndexOf("iot");
						if (tmpidx > -1) {
							targetUID = res.result.substring(tmpidx);

						} else if (currenturl.indexOf("HD") == 0 || res.result.indexOf("UID") > -1 || res
							.result.indexOf("Codes") > -1) {
							if (currenturl.indexOf("HD") == 0) {
								//为一期设备码
								targetUID = currenturl;
							} else {
								let tmpidx1 = res.result.lastIndexOf("UID");
								targetUID = res.result.substring(tmpidx1);
								if (targetUID == null) {
									this.$refs.promptMsg.open('Invalid QR code', 2000)
									return;
								}
							}
							if (res.result.indexOf("Codes") > -1) {
								let paramCodes = res.result.lastIndexOf("Codes")
								targetUID = res.result.substring(paramCodes);
							}

						} else {
							this.$refs.promptMsg.open('Invalid QR code', 2000)
						}
						OutStockByKey(targetUID).then(res => {
							if (res.data) {
								this.scanAddItems(res.data)
							}
						}).catch(err => {
							this.setMsgTop(err)
						})
					}
				});
			},

			closePopup() {
				this.$refs.bottomPop.close()
			},
			toChoiceDevice() {
				this.$refs.bottomPop.open()
			},
			submitForm(st) {
				console.log("出库", st);
				this.$refs.recordsForm.validate().then(async valid => {
					if (valid) {
						console.log(this.recordsForm, 'this.recordsFormthis.recordsFormthis.recordsForm');
						if (this.recordsForm.List.length == 0) {
							this.$refs.promptMsg.open('Please select the items to be stored in the warehouse',
								1500)
							return;
						}
						if (this.recordsForm.ToHouseId && this.recordsForm.FromHouseId == this.recordsForm
							.ToHouseId) {
							this.$refs.promptMsg.open(
								'Please select different outgoing and target warehouses!',
								1500)
							return;
						}
						this.isLoading = true
						try {

							//提交审批表单
							if (this.leaveTemplateId > 0) {
								await this.$refs.flowForm.submitForm(st, async (rss) => {
									if (rss) {
										let response;
										if (this.leaveId && this.leaveId != null) {
											this.recordsForm.Id = this.leaveId;
											response = await leaveEdit(this.recordsForm);
										} else {
											response = await leaveAdd(this.recordsForm);
										}
										//提交审批表单
										if (st == 2) {
											await leaveSubmit({
												id: response.data,
												flowId: rss
											});
										}
										this.$refs.promptMsg.open('Operation successful', 1500)
										setTimeout(() => {
											setPagesParam('loadData', 'load', 1)
										}, 1500)
										this.$store.commit('SET_INVENTORY_INFO', true)

									} else {
										this.isLoading = false
									}

								});
							} else {
								let response;
								if (this.leaveId && this.leaveId != null) {
									this.recordsForm.Id = this.leaveId;
									response = await leaveEdit(this.recordsForm);
								} else {
									response = await leaveAdd(this.recordsForm);
								}
								//提交审批表单
								if (st == 2) {
									await leaveSubmit({
										id: response.data,
										flowId: 0
									});
								}
								this.$refs.promptMsg.open('Operation successful', 1500)
								setTimeout(() => {
									setPagesParam('loadData', 'load', 1)
								}, 1500)
								this.$store.commit('SET_INVENTORY_INFO', true)
							}


						} catch (e) {
							console.log(e);
							//TODO handle the exception
							this.setMsgTop(e)
							this.isLoading = false
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
			async selectHouse(item) {
				console.log(item, 'itemitemitem');
				if (item) {
					if (this.activeHouse == 'from') {
						this.recordsForm.FromHouseId = item.Id
						this.recordsForm.FromHouseName = item.StoreName
						this.leaveTemplateId = item.LeaveTemplateId;
						if (this.isCheckPermi(['/FlowService/Flow'])) {
							if (this.leaveTemplateId > 0) {
								await this.$refs.flowForm.InitData(this.leaveTemplateId, this.FlowParams, this.leaveId ==null ? null : this.recordsForm.StockNumber);
							}
						}
					} else if (this.activeHouse == 'to') {
						this.recordsForm.ToHouseId = item.Id
						this.recordsForm.ToHouseName = item.StoreName
					}

				}
			},
			choiceWarehouse(val) {
				this.activeHouse = val
				if (this.recordsForm.FromHouseId) {
					if (this.activeHouse == 'from') {
						uni.navigateTo({
							url: '/pages_Inventory/warehouse_management?isSelect=true&&selectId=' + this
								.recordsForm.FromHouseId
						})
					} else if (this.activeHouse == 'to') {
						uni.navigateTo({
							url: '/pages_Inventory/warehouse_management?isSelect=true&&selectId=' + this
								.recordsForm.ToHouseId
						})
					}

				} else {
					uni.navigateTo({
						url: '/pages_Inventory/warehouse_management?isSelect=true'
					})
				}
			},
			choiceToOrgId() {
				//选择目标客户
				console.log("yyyy", this.showAgent, this.showKf);
				if (this.showAgent && this.showKf) {
					this.$refs.customPop.open()
				} else if (this.showAgent && !this.showKf) {
					this.selectAgents()
				} else if (!this.showAgent && this.showKf) {
					this.selectCustoms()
				}

			},
			finishSelectAgents(arr) {
				if (!arr || arr.length == 0) return
				this.recordsForm.ToOrgId = arr[0].OrgId;
				this.recordsForm.ToName = arr[0].OrgName;
			},
			finishSelectCustom(arr) {
				if (!arr || arr.length == 0) return
				this.recordsForm.ToOrgId = arr[0].BindOrgId;
				this.recordsForm.ToName = arr[0].CustomerName;
			},
			selectAgents() {
				uni.navigateTo({
					url: '/pages_factory/agents_list?isSelect=true'
				})
			},
			selectCustoms() {
				uni.navigateTo({
					url: '/pages_factory/custom_select?isSelect=true'
				})
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