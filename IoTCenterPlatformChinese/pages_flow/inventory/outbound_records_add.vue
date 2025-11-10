<template>
	<view style="height: 100%;overflow-x: hidden;" @click="isSelKd=false">
		<top :title="topTitle" leftWidth="120rpx" :isleftBack="true" leftIcon="icon-fanhui" backgroundColor="#ffffff"
			rightWidth="100rpx">
		</top>
		<view class="detail_con" style="background-color: rgba(245, 248, 249, 1);padding-bottom: 0;"
			v-if="leaveId&&leaveId!=null">
			<view class="detail_status">
				<custom-icons iconsName="icon-daitijiao" iconsSize="36rpx" iconsColor="#333"></custom-icons>
				<view class="status_text">
					待提交
				</view>
			</view>
		</view>
		<uni-forms ref="recordsForm" :modelValue="recordsForm" :rules="rules" labelWidth='80' label-position="top"
			v-if="recordsForm.StockNumber">
			<view class="form_con page_form_con" :class="{'no_radius':!leaveId}">
				<uni-forms-item label="出库单号" required name="StockNumber" id="StockNumber_form" labelFont="32rpx"
					contentFont="32rpx" :requireOpacity="0.5">
					<view id="StockNumber" class="form_li">
						<uni-easyinput placeholderStyle="color:#C1C1C1;font-size:32rpx" inputHeight="88rpx"
							:styles="styles" type="text" v-model="recordsForm.StockNumber" placeholder="请输入出库单号"
							contentFontSize="32rpx" :disabled="true" />
					</view>
				</uni-forms-item>
				<uni-forms-item label="出库时间" required name="OutDate" id="OutDate_form" labelFont="32rpx"
					contentFont="32rpx" :requireOpacity="0.5">
					<view id="OutDate" class="form_li">
						<view class="int_date">
							<view class="date_con long_date" style="color: #fff;">
								<uni-datetime-picker ref="dateChoice1" class="date" type="datetime"
									placeholder-style="font-size:32rpx;color:#C1C1C1" :clear-icon="false"
									v-model="recordsForm.OutDate" placeholder='请选择出库时间' :isCustom="true"
									:isDark="false">
									<view class="date_slot" :class="{'has_val':recordsForm.OutDate}">
										<custom-icons iconsName="icon-xuanzeshijian" iconsSize="28rpx"
											iconsColor="#C1C1C1"></custom-icons>
										<view class="text">
											{{recordsForm.OutDate?recordsForm.OutDate:'请选择出库时间'}}
										</view>

									</view>
								</uni-datetime-picker>
							</view>
						</view>
					</view>
				</uni-forms-item>
				<uni-forms-item label="物流单号" name="ExpressNumber" id="ExpressNumber_form" labelFont="32rpx"
					contentFont="32rpx" :requireOpacity="0.5" v-if="!isApplyLeave">
					<view id="ExpressNumber" class="form_li">
						<view class="sel_input">
							<view class="sel" @click.stop="openKDSel">
								<view class="sel_text" :class="{'pal':!recordsForm.ExpressCompanyName}">
									{{recordsForm.ExpressCompanyName?recordsForm.ExpressCompanyName:'请选择'}}
								</view>
								<custom-icons iconsName="icon-xialajiantou" iconsSize="12rpx"
									iconsColor="#999999"></custom-icons>
							</view>
							<view class="sel_line"></view>
							<view class="input">
								<uni-easyinput placeholderStyle="color:#C1C1C1;font-size:32rpx" inputHeight="88rpx"
									:styles="styles" type="text" v-model="recordsForm.ExpressNumber"
									placeholder="请输入物流单号" contentFontSize="32rpx" @input="expressSelected"
									@blur="expressSelected" />
							</view>
							<view class="sel_list" v-show="isSelKd">
								<view class="sel_li" v-for="item in kdcompanys" @click.stop="choiceKd(item)">
									{{item.Name}}
								</view>
							</view>
						</view>

					</view>
				</uni-forms-item>
				<uni-forms-item label="联系手机" required name="ExpressPhone" id="ExpressPhone_form" labelFont="32rpx"
					contentFont="32rpx" :requireOpacity="0.5" v-if="!isApplyLeave&&recordsForm.ExpressCompany == 'shunfeng'">
					<view id="ExpressPhone" class="form_li">
						<uni-easyinput placeholderStyle="color:#C1C1C1;font-size:32rpx" inputHeight="88rpx"
							:styles="styles" type="text" v-model="recordsForm.ExpressPhone" placeholder="请输入联系手机号码"
							contentFontSize="32rpx" primaryColor="rgba(35, 113, 255, 1)" />
					</view>
				</uni-forms-item>
				<uni-forms-item label="所出仓库" required name="FromHouseId" id="FromHouseId_form" labelFont="32rpx"
					contentFont="32rpx" :requireOpacity="0.5">
					<view id="FromHouseId" class="form_li" @click="choiceWarehouse('from')">
						<view class="form_input"
							:class="{'placeholder_input':!recordsForm.FromHouseName||recordsForm.FromHouseName=='','dis_li_input':isApplyLeave}">
							{{recordsForm.FromHouseName?recordsForm.FromHouseName:'请选择所处仓库'}}
						</view>
						<view class="form_sel_icon">
							<custom-icons iconsName="icon-a-youjiantoubai" iconsSize="20rpx"
								iconsColor="#999999"></custom-icons>
						</view>
					</view>
				</uni-forms-item>
				<uni-forms-item label="目标仓库" required name="ToHouseId" id="ToHouseId_form" labelFont="32rpx"
					contentFont="32rpx" :requireOpacity="0.5" v-if="recordsForm.LeaveMethod==2"
					:rules="rules.ToHouseId.rules">
					<view id="ToHouseId" class="form_li" @click="choiceWarehouse('to')">
						<view class="form_input"
							:class="{'placeholder_input':!recordsForm.ToHouseName||recordsForm.ToHouseName==''}">
							{{recordsForm.ToHouseName?recordsForm.ToHouseName:'请选择目标仓库'}}
						</view>
						<view class="form_sel_icon">
							<custom-icons iconsName="icon-a-youjiantoubai" iconsSize="20rpx"
								iconsColor="#999999"></custom-icons>
						</view>
					</view>
				</uni-forms-item>
				<uni-forms-item label="目标客户" required name="ToOrgId" id="ToOrgId_form" labelFont="32rpx"
					contentFont="32rpx" :requireOpacity="0.5" v-if="recordsForm.LeaveMethod==0">
					<view id="ToOrgId" class="form_li" @click="choiceToOrgId">
						<!-- <uni-easyinput placeholderStyle="color:rgba(255, 255, 255, 0.2);font-size:32rpx"
							inputHeight="88rpx" :styles="styles" type="text" v-model="recordsForm.ToName"
							placeholder="Please select the target company" contentFontSize="32rpx"
							primaryColor="rgba(255, 255, 255, 0.5)" :clearable="false" /> -->
						<view class="form_input"
							:class="{'placeholder_input':!recordsForm.ToName||recordsForm.ToName==''}">
							{{recordsForm.ToName?recordsForm.ToName:'请选择目标客户'}}
						</view>
						<view class="form_sel_icon">
							<custom-icons iconsName="icon-a-youjiantoubai" iconsSize="20rpx"
								iconsColor="#999999"></custom-icons>
						</view>
					</view>
				</uni-forms-item>
				<uni-forms-item label="备注" name="Remark" id="Remark_form" labelFont="32rpx" contentFont="32rpx"
					:requireOpacity="0.5">
					<view id="Remark" class="form_li">
						<uni-easyinput placeholderStyle="color:#C1C1C1;font-size:32rpx" inputHeight="70rpx"
							:styles="styles" type="textarea" v-model="recordsForm.Remark" placeholder="请输入备注"
							contentFontSize="32rpx" autoHeight :isCustom="true" />
					</view>
				</uni-forms-item>
				<uni-forms-item label="出库物品" required name="List" id="List_form" labelFont="32rpx" contentFont="32rpx"
					:requireOpacity="0.5" labelPosition="top">
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
										<view class="input_li mar_input" v-if="!isApplyLeave">
											<text class="label">价格:</text>
											<uni-number-box background="#F8F8F8" v-model="item.Price" color="#333333"
												:max="100000000" />
										</view>
										<view class="input_li">
											<text class="label">数量:</text>
											<uni-number-box background="#F8F8F8" v-model="item.Quantity" color="#333333"
												:min="item.Quantity" :max="item.Quantity" :disabled="isApplyLeave"/>
										</view>
									</view>
									<view class="price_quantity" v-if="item.TargetType==0">
										<view class="input_li mar_input" v-if="!isApplyLeave">
											<text class="label">价格:</text>
											<uni-number-box background="#F8F8F8" v-model="item.Price" color="#333333"
												:max="100000000" />
										</view>
										<view class="input_li">
											<text class="label">数量:</text>
											<uni-number-box background="#F8F8F8" v-model="item.Quantity" color="#333333"
												:min="1" :max="item.Quantity" :disabled="isApplyLeave"/>
										</view>
									</view>
								</view>
								<view class="del_icon" @click.stop="delSelDev(inx)" v-if="!isApplyLeave">
									<view class="icons_del t-icon-yichu1"></view>
								</view>
							</view>
							<view class="form_device_add" @click="toChoiceDevice" v-if="!isApplyLeave">
								<custom-icons iconsName="icon-tianjia" iconsSize="24rpx"
									iconsColor="#333333"></custom-icons>
								<view class="text">
									添加物品
								</view>
							</view>
						</view>
					</view>
				</uni-forms-item>
			</view>
		</uni-forms>
		<ProcessCompot ref="flowForm" @isChoiceFlowUserFun="isChoiceFlowUserFun" @setActiveItem="setActiveItem"
			v-if="leaveTemplateId">
			出库审核
		</ProcessCompot>
		<view class="form_con page_form_con no_radius" v-if="recordsForm.StockNumber">
			<view class="sub_save_con" :class="{'has_btn':leaveId&&leaveId!=null}">
				<button class="delete_button" @click="cancelWare" :disabled="isLoading"
					:style="{'opacity':isLoading?0.6:1}" :loading="isLoading" v-if="leaveId&&leaveId!=null">
					撤销
				</button>
				<button class="submit_button" @click="submitForm(1)" :disabled="isLoading"
					:style="{'opacity':isLoading?0.6:1}" :loading="isLoading">
					保存
				</button>
				<button class="jump_button" @click="submitForm(2)" :disabled="isLoading"
					:style="{'opacity':isLoading?0.6:1}" :loading="isLoading">
					提交
				</button>
			</view>
		</view>
		<view class="zhanwei" style="width: 100%;height: 40rpx;">

		</view>
		<uni-popup ref="bottomPop" type="bottom" :mask-click="true" :zIndex="999"
			maskBackgroundColor="rgba(0, 0, 0, 0.5)">
			<view class="popup_list">
				<view class="popup_li borradio" @click.stop="selectItems">
					选择物品
				</view>
				<view class="popup_li" @click.stop="scanAdd">
					扫码添加
				</view>
				<view class="popup_li cancel" @click="closePopup">
					取消
				</view>
			</view>
		</uni-popup>
		<uni-popup ref="customPop" type="bottom" :mask-click="true" :zIndex="999"
			maskBackgroundColor="rgba(0, 0, 0, 0.5)">
			<view class="popup_list">
				<view class="popup_li borradio" @click.stop="selectAgents">
					我的代理商
				</view>
				<view class="popup_li" @click.stop="selectCustoms">
					我的客户
				</view>
				<view class="popup_li cancel" @click="closePopup">
					取消
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
		leaveFormData,
		generateCKNumber,
		leaveSubmit,
		leaveSubmitModel,
		stockList,
		getLeaveInfo,
		OutStockByKey,
		cancelLeave
	} from "@/api/stock";
	import {
		ApplyInfo
	} from "@/api/apply";
	import {
		houseList,
		houseInfo
	} from "@/api/house.js";
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
				topTitle: '创建出库单',
				isLoading: false,
				styles: {
					color: '#333',
					backgroundColor: '#F8F8F8',
					disableColor: '#F8F8F8',
					borderColor: '#F8F8F8'
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
							errorMessage: '请输入出库单号',
						}]
					},
					OutDate: {
						rules: [{
							required: true,
							errorMessage: '请选择出库时间',
						}]
					},
					FromHouseId: {
						rules: [{
							required: true,
							errorMessage: '请选择所处仓库',
						}]
					},
					ToHouseId: {
						rules: [{
							required: true,
							errorMessage: '请选择目标仓库',
						}]
					},
					ToOrgId: {
						rules: [{
							required: true,
							errorMessage: '请选择目标客户',
						}]
					},
					ExpressPhone: {
						rules: [{
							required: true,
							errorMessage: '请输入手机号',
						}]
					},
					List: {
						rules: [{
							required: true,
							errorMessage: '请选择出库物品',
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
				activeHouse: '',
				isApplyLeave:false
			}
		},
		computed: {
			FlowParams: function() {
				return {
					"@from": this.recordsForm.StockNumber,
					"@fromtype": "出库单",
				};
			}
		},
		onLoad(options) {
			this.$nextTick(async () => {
				if(options.applyId){
					this.applyLeaveDialog(options.applyId)
				}else{
					await this.outRecordsLoad(options)
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
			if (this.isShowSelectTips) {
				this.$refs.promptMsg.open("物品 '" + this.wupinSelectArr.map(x => x.TargetName).join() +
					"已经选择'", 2000)
				this.isShowSelectTips = false
			}
		},
		methods: {
			async applyLeaveDialog(id){
			  //申请出库单出库
			  this.$refs.promptMsg.loadingOpen()
			  let info=await ApplyInfo(id)
			  this.isApplyLeave=true
			  this.leaveId =null
			  this.topTitle = "新增领料单";
			  let ssp = await generateCKNumber();
			  this.recordsForm.StockNumber = ssp.data;
			  this.recordsForm.FromHouseName = "";
			  this.recordsForm.LeaveMethod=3
			  this.recordsForm.OutDate = this.parseTime(Date.now());
			  this.recordsForm.FromHouseId = undefined;
			  this.recordsForm.Remark = "";
			  this.recordsForm.ExpressNumber = "";
			  this.recordsForm.ExpressCompany = "";
			  this.recordsForm.ExpressPhone = "";
			  this.recordsForm.ToOrgId = 0;
			  this.recordsForm.ToName = "";
			  this.recordsForm.sourceEnterId=id
			  this.recordsForm.List = info.data.List.map(row=>{
				row.Price=0
				return row
			  });
			  console.log(this.recordsForm.List,'物品列表');
			  let fromHsId=info.data.HouseId
			  if (fromHsId != null) {
				let hsInfo = await houseInfo(fromHsId,false);
				this.recordsForm.FromHouseName = hsInfo.data.StoreName;
				this.recordsForm.FromHouseId = hsInfo.data.Id;
				this.leaveTemplateId = hsInfo.data.LeaveTemplateId;
			  }
			  
			  if (this.leaveTemplateId > 0) {
				setTimeout(async () => {
					try {
						if (this.isCheckPermi(['/FlowService/Flow'])) {
							if (this.leaveTemplateId > 0) {
								let fromInfo=await leaveFormData({
								  "applyNumber": this.recordsForm.StockNumber,
								  "fromHouseId": this.recordsForm.FromHouseId,
								})
								await this.$refs.flowForm.InitData(
								  this.leaveTemplateId,
								  this.FlowParams,
								  this.leaveId == null ? null : this.recordsForm.StockNumber,
								  fromInfo.data
								);
							}
						}
						if (this.isCheckPermi(['/ProducerService/Agent/AllList'])) {
							this.showAgent = true;
						}
						if (this.isCheckPermi(['/CRMService/Customer/List'])) {
							this.showKf = true;
						}
						this.$refs.promptMsg.loadingColse()
					} catch (e) {
						//TODO handle the exception
						this.$refs.promptMsg.loadingColse()
						this.setMsgTop(e)
					}
				}, 100)
			  }
			  this.$refs.promptMsg.loadingColse()
			},
			async outRecordsLoad(options){
				try {
					this.$refs.promptMsg.loadingOpen()
					this.kdcompanys = await this.$store.dispatch("data/kuaiDiList");
					if (options.id) {
						this.leaveId = options.id
						let leaveInfo = await getLeaveInfo(this.leaveId);
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
						this.recordsForm.LeaveMethod=leaveInfo.data.LeaveMethod
						if (leaveInfo.data.FromHouseLeaveTemplateId) {
							this.leaveTemplateId = leaveInfo.data.FromHouseLeaveTemplateId
						} else {
							this.leaveTemplateId = 0
						}
						if (this.recordsForm.LeaveMethod == "0") {
							this.topTitle = '编辑出库单'
						} else {
							if(this.recordsForm.LeaveMethod=='3'){
								this.isApplyLeave=true//出库申请跳转致出库
								this.topTitle = '编辑领料单'
							}else{
								this.topTitle = '编辑调拨单'
							}
							
						}
					} else {
						let ssp = await generateCKNumber();
						this.recordsForm.StockNumber = ssp.data;
						this.recordsForm.OutDate = dayjs(new Date()).format('YYYY-MM-DD HH:mm:ss')
						if (options.LeaveMethod) {
							this.recordsForm.LeaveMethod = options.LeaveMethod
						}
						if (this.recordsForm.LeaveMethod == "0") {
							this.topTitle = '创建出库单'
						} else {
							this.topTitle = '创建调拨单'
						}
				
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
					setTimeout(async () => {
						try {
							if (this.isCheckPermi(['/FlowService/Flow'])) {
								if (this.leaveTemplateId > 0) {
									let fromInfo=await leaveFormData({
									  "applyNumber": this.recordsForm.StockNumber,
									  "fromHouseId": this.recordsForm.FromHouseId,
									})
									await this.$refs.flowForm.InitData(
									  this.leaveTemplateId,
									  this.FlowParams,
									  this.leaveId == null ? null : this.recordsForm.StockNumber,
									  fromInfo.data
									);
								}
							}
							if (this.isCheckPermi(['/ProducerService/Agent/AllList'])) {
								this.showAgent = true;
							}
							if (this.isCheckPermi(['/CRMService/Customer/List'])) {
								this.showKf = true;
							}
							this.$refs.promptMsg.loadingColse()
						} catch (e) {
							//TODO handle the exception
							this.$refs.promptMsg.loadingColse()
							this.setMsgTop(e)
						}
					}, 100)
				} catch (e) {
					//TODO handle the exception
					this.$refs.promptMsg.loadingColse()
					this.setMsgTop(e)
				}
			},
			async confirmCancel() {
				//确认撤销动作
				try {
					this.isLoading = true
					await cancelLeave({
						id: this.leaveId
					});
					this.$refs.promptMsg.open('操作成功', 1500)
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
						'你确定要撤销出库单 ' + this.recordsForm.StockNumber +
						'吗? (此操作是不可逆的)?'
					)
				} catch (e) {
				}
			},
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
			},
			selectItems() {
				//选择物品
				if (this.recordsForm.FromHouseId) {
					this.$refs.bottomPop.close()
					uni.navigateTo({
						url: '/pages_flow/inventory/inventory_list?isSelect=true&isMulSelect=true&HouseId=' + this
							.recordsForm.FromHouseId
					})
				} else {
					this.$refs.promptMsg.open('请先选择要出库的仓库', 2000)
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
									this.$refs.promptMsg.open('无效的二维码', 2000)
									return;
								}
							}
							if (res.result.indexOf("Codes") > -1) {
								let paramCodes = res.result.lastIndexOf("Codes")
								targetUID = res.result.substring(paramCodes);
							}

						} else {
							this.$refs.promptMsg.open('无效的二维码', 2000)
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
				this.$refs.recordsForm.validate().then(async valid => {
					if (valid) {
						// console.log(this.recordsForm, 'this.recordsFormthis.recordsFormthis.recordsForm');
						if (this.recordsForm.List.length == 0) {
							this.$refs.promptMsg.open('请选择要出库的物品', 1500)
							return;
						}
						if (this.recordsForm.ToHouseId && this.recordsForm.FromHouseId == this.recordsForm
							.ToHouseId) {
							this.$refs.promptMsg.open('调拨时请选择不同的出库仓库和目标仓库!',
								1500)
							return;
						}
						this.isLoading = true
						try {
							let response;
							if (this.leaveId && this.leaveId != null) {
								this.recordsForm.Id = this.leaveId;
								response = await leaveEdit(this.recordsForm);
							} else {
								response = await leaveAdd(this.recordsForm);
							}
							//提交审批表单
							if (this.leaveTemplateId > 0) {
								if (st == 2) {
								  let tmpmodel = this.$refs.flowForm.getModel();
								  tmpmodel["id"] = response.data;
								  await leaveSubmitModel(tmpmodel);
								}else{
									await this.$refs.flowForm.submitForm(st, (rss) => {})
								}
								this.$refs.promptMsg.open('操作成功', 1500)
								setTimeout(() => {
									setPagesParam('loadData', 'load', 1)
								}, 1500)
								this.$store.commit('SET_INVENTORY_INFO', true)
								// this.isLoading = false
							} else {
								//提交审批表单
								if (st == 2) {
									await leaveSubmitModel({ id: response.data });
								}
								this.$refs.promptMsg.open('操作成功', 1500)
								setTimeout(() => {
									setPagesParam('loadData', 'load', 1)
								}, 1500)
								this.$store.commit('SET_INVENTORY_INFO', true)
							}

						} catch (e) {
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
				if (item) {
					if (this.activeHouse == 'from') {
						this.recordsForm.FromHouseName = item.StoreName;
						this.recordsForm.FromHouseId = item.Id;
						this.leaveTemplateId = item.LeaveTemplateId;
						if (this.isCheckPermi(['/FlowService/Flow'])) {
							if (this.leaveTemplateId > 0) {
								let fromInfo=await leaveFormData({
								  "applyNumber": this.recordsForm.StockNumber,
								  "fromHouseId": this.recordsForm.FromHouseId,
								})
								await this.$refs.flowForm.InitData(
								  this.leaveTemplateId,
								  this.FlowParams,
								  this.leaveId == null ? null : this.recordsForm.StockNumber,
								  fromInfo.data
								);
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
				if(this.isApplyLeave&&val=='from'){
					return
				}
				if (this.recordsForm.FromHouseId) {
					if (this.activeHouse == 'from') {
						uni.navigateTo({
							url: '/pages_flow/inventory/warehouse_management?isSelect=true&&selectId=' + this
								.recordsForm.FromHouseId
						})
					} else if (this.activeHouse == 'to') {
						uni.navigateTo({
							url: '/pages_flow/inventory/warehouse_management?isSelect=true&&selectId=' + this
								.recordsForm.ToHouseId
						})
					}

				} else {
					uni.navigateTo({
						url: '/pages_flow/inventory/warehouse_management?isSelect=true'
					})
				}
			},
			choiceToOrgId() {
				//选择目标客户
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
				this.$refs.customPop.close()
				uni.navigateTo({
					url: '/pages_factory/agents_list?isSelect=true'
				})
			},
			selectCustoms() {
				this.$refs.customPop.close()
				uni.navigateTo({
					url: '/pages_factory/custom_select?isSelect=true'
				})
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
		background-color: #ffffff;
		border-radius: 20rpx 20rpx 0 0;

		.popup_li {
			height: 120rpx;
			line-height: 120rpx;
			text-align: center;
			color: #333333;
			font-size: 36rpx;

			&.borradio {
				border-radius: 20rpx 20rpx 0 0;
			}

			&.cancel {
				color: #999999;
			}
		}
	}
</style>