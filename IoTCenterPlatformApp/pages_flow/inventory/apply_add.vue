<template>
	<view style="height: 100%;overflow-x: hidden;">
		<top :title="topTitle" leftWidth="120rpx" :isleftBack="true" leftIcon="icon-fanhui" backgroundColor="#ffffff"
			rightWidth="100rpx">
		</top>
		<view class="detail_con" style="background-color: rgba(245, 248, 249, 1);padding-bottom: 0;"
			v-if="applyId&&applyId!=null">
			<view class="detail_status">
				<custom-icons iconsName="icon-daitijiao" iconsSize="36rpx" iconsColor="#333"></custom-icons>
				<view class="status_text">
					待提交
				</view>
			</view>
		</view>
		<uni-forms ref="applyForm" :modelValue="applyForm" :rules="rules" labelWidth='80' label-position="top"
			v-if="applyForm.ApplyNumber">
			<view class="form_con page_form_con" :class="{'no_radius':!applyId}">
				<uni-forms-item label="出库申请单号" required name="ApplyNumber" id="ApplyNumber_form" labelFont="32rpx"
					contentFont="32rpx" :requireOpacity="0.5">
					<view id="ApplyNumber" class="form_li">
						<uni-easyinput placeholderStyle="color:#C1C1C1;font-size:32rpx" inputHeight="88rpx"
							:styles="styles" type="text" v-model="applyForm.ApplyNumber" placeholder="请输入出库申请单号"
							contentFontSize="32rpx" :disabled="true" />
					</view>
				</uni-forms-item>
				<uni-forms-item label="申请时间" required name="ApplyOn" id="ApplyOn_form" labelFont="32rpx"
					contentFont="32rpx" :requireOpacity="0.5">
					<view id="ApplyOn" class="form_li">
						<view class="int_date">
							<view class="date_con long_date" style="color: #fff;">
								<uni-datetime-picker ref="dateChoice1" class="date" type="datetime"
									placeholder-style="font-size:32rpx;color:#C1C1C1" :clear-icon="false"
									v-model="applyForm.ApplyOn" placeholder='请选择申请时间' :isCustom="true"
									:isDark="false">
									<view class="date_slot" :class="{'has_val':applyForm.ApplyOn}">
										<custom-icons iconsName="icon-xuanzeshijian" iconsSize="28rpx"
											iconsColor="#C1C1C1"></custom-icons>
										<view class="text">
											{{applyForm.ApplyOn?applyForm.ApplyOn:'请选择申请时间'}}
										</view>

									</view>
								</uni-datetime-picker>
							</view>
						</view>
					</view>
				</uni-forms-item>
				<uni-forms-item label="申请类型" required name="ApplyType" id="ApplyType_form" labelFont="32rpx"
					contentFont="32rpx" :requireOpacity="0.5">
					<view id="ApplyType" class="form_li">
						<uni-data-select v-model="applyForm.ApplyType" :localdata="ApplyTypeList" width="100%" placeholder="请选择申请类型"
							borderColor="#F8F8F8" palColor="#c1c1c1" :isCustom="true" :isDark="false" ></uni-data-select>
					</view>
				</uni-forms-item>
				<uni-forms-item label="申请人"  required name="ApplyUserId" id="ApplyUserId_form" labelFont="32rpx"
					contentFont="32rpx" :requireOpacity="0.5">
					<view id="ApplyUserId" class="form_li" @click="choiceApplyUser()">
						<view class="form_input"
							:class="{'placeholder_input':!applyForm.ApplyUserId||applyForm.ApplyUserId==''}">
							{{applyForm.ApplyUserName?applyForm.ApplyUserName:'请选择申请人'}}
						</view>
						<view class="form_sel_icon">
							<custom-icons iconsName="icon-a-youjiantoubai" iconsSize="20rpx"
								iconsColor="#999999"></custom-icons>
						</view>
					</view>
				</uni-forms-item>
				<uni-forms-item label="仓库" required name="HouseId" id="HouseId_form" labelFont="32rpx"
					contentFont="32rpx" :requireOpacity="0.5" :rules="rules.HouseId.rules">
					<view id="HouseId" class="form_li" @click="choiceWarehouse('to')">
						<view class="form_input"
							:class="{'placeholder_input':!applyForm.HouseName||applyForm.HouseName==''}">
							{{applyForm.HouseName?applyForm.HouseName:'请选择仓库'}}
						</view>
						<view class="form_sel_icon">
							<custom-icons iconsName="icon-a-youjiantoubai" iconsSize="20rpx"
								iconsColor="#999999"></custom-icons>
						</view>
					</view>
				</uni-forms-item>
				<uni-forms-item label="备注" name="Reason" id="Reason_form" labelFont="32rpx" contentFont="32rpx"
					:requireOpacity="0.5">
					<view id="Reason" class="form_li">
						<uni-easyinput placeholderStyle="color:#C1C1C1;font-size:32rpx" inputHeight="70rpx"
							:styles="styles" type="textarea" v-model="applyForm.Reason" placeholder="请输入备注"
							contentFontSize="32rpx" autoHeight :isCustom="true" />
					</view>
				</uni-forms-item>
				<uni-forms-item label="申请物品" required name="List" id="List_form" labelFont="32rpx" contentFont="32rpx"
					:requireOpacity="0.5" labelPosition="top">
					<view id="List" class="form_li">
						<view class="form_device_list">
							<view class="form_device_li" v-for="(item,inx) in applyForm.List" :key="item.Id">
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
												:max="100000000" />
										</view>
										<view class="input_li">
											<text class="label">数量:</text>
											<uni-number-box background="#F8F8F8" v-model="item.Quantity" color="#333333"
												:min="item.Quantity" :max="item.Quantity" />
										</view>
									</view>
									<view class="price_quantity" v-if="item.TargetType==0">
										<view class="input_li mar_input">
											<text class="label">价格:</text>
											<uni-number-box background="#F8F8F8" v-model="item.Price" color="#333333"
												:max="100000000" />
										</view>
										<view class="input_li">
											<text class="label">数量:</text>
											<uni-number-box background="#F8F8F8" v-model="item.Quantity" color="#333333"
												:min="1" :max="item.Quantity" />
										</view>
									</view>
								</view>
								<view class="del_icon" @click.stop="delSelDev(inx)">
									<view class="icons_del t-icon-yichu1"></view>
								</view>
							</view>
							<view class="form_device_add" @click="toChoiceDevice">
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
			v-if="LeaveApplyTemplateId">
			出库申请
		</ProcessCompot>
		<view class="form_con page_form_con no_radius" v-if="applyForm.ApplyNumber">
			<view class="sub_save_con" :class="{'has_btn':applyId&&applyId!=null}">
				<button class="delete_button" @click="cancelWare" :disabled="isLoading"
					:style="{'opacity':isLoading?0.6:1}" :loading="isLoading" v-if="applyId&&applyId!=null">
					删除
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
		<msg-prompt ref="promptMsg" @confirm="confirmCancel"></msg-prompt>
	</view>
</template>

<script>
	import {
		applyAdd,
		applyEdit,
		applyFormData,
		generateCKSQNumber,
		applySubmitModel,
		ApplyInfo,
		deleteApply
	} from "@/api/apply";
	import {stockList,OutStockByKey} from "@/api/stock.js"
	import {
		houseList,
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
				topTitle: '创建出库申请单',
				isLoading: false,
				styles: {
					color: '#333',
					backgroundColor: '#F8F8F8',
					disableColor: '#F8F8F8',
					borderColor: '#F8F8F8'
				},
				applyForm: {
					ApplyNumber: "",
					HouseId: undefined,
					HouseName: "",
					ApplyOn: undefined,
					ApplyType:'0',
					Reason: "",
					List: []
				},
				rules: {
					ApplyNumber: {
						rules: [{
							required: true,
							errorMessage: '请输入出库申请单号',
						}]
					},
					ApplyOn: {
						rules: [{
							required: true,
							errorMessage: '请选择申请时间',
						}]
					},
					ApplyType: {
						rules: [{
							required: true,
							errorMessage: '请选择申请类型',
						}]
					},
					HouseId: {
						rules: [{
							required: true,
							errorMessage: '请选择仓库',
						}]
					},
					List: {
						rules: [{
							required: true,
							errorMessage: '请选择出库物品',
						}]
					},
				},
				applyId: null,
				LeaveApplyTemplateId: 0,
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
				wupinSelectArr: [], //刚选择完重复的物品
				isShowSelectTips: false, //是否显示选择提示
				activeHouse: '',
				ApplyTypeList:[],
				selectText:''
			}
		},
		computed: {
			FlowParams: function() {
				return {
					"@from": this.applyForm.ApplyNumber,
					"@fromtype": "出库申请单",
				};
			}
		},
		onLoad(options) {
			this.$nextTick(async () => {
				try {
					this.$refs.promptMsg.loadingOpen()
					let typeList=await this.$store.dispatch("data/dictList", 'apply_type');
					this.ApplyTypeList=typeList.map(row=>{
						return{text: row.label,value: row.value}
					})
					console.log(this.ApplyTypeList,'申请类型列表');
					if (options.id) {
						this.applyId = options.id
						let applyInfo = await ApplyInfo(this.applyId);
						this.applyForm.ApplyNumber = applyInfo.data.ApplyNumber;
						this.applyForm.ApplyOn = applyInfo.data.ApplyOn;
						this.applyForm.ApplyType= applyInfo.data.ApplyType;
						this.applyForm.HouseId = applyInfo.data.HouseId;
						this.applyForm.HouseName = applyInfo.data.House.HouseName;
						this.applyForm.ApplyUserId=applyInfo.data.ApplyUserId.RealName
						this.applyForm.ApplyUserName=applyInfo.data.ApplyUserInfo
						this.applyForm.Reason = applyInfo.data.Reason;
						this.applyForm.List = applyInfo.data.List;
						if (applyInfo.data.LeaveApplyTemplateId) {
							this.LeaveApplyTemplateId = applyInfo.data.LeaveApplyTemplateId
						} else {
							this.LeaveApplyTemplateId = 0
						}
						this.topTitle = '编辑出库申请单'
					} else {
						let ssp = await generateCKSQNumber();
						this.applyForm.ApplyNumber = ssp.data;
						this.applyForm.ApplyOn = dayjs(new Date()).format('YYYY-MM-DD HH:mm:ss')
						if (options.LeaveMethod) {
							this.applyForm.LeaveMethod = options.LeaveMethod
						}
						this.topTitle = '创建出库申请单'

						let hslist = (await houseList({
							IsSystem: true
						})).data.List;
						if (hslist.length > 0) {
							this.applyForm.HouseName = hslist[0].StoreName;
							this.applyForm.HouseId = hslist[0].Id;
							this.LeaveApplyTemplateId = hslist[0].LeaveApplyTemplateId;
						} else {
							this.LeaveApplyTemplateId = 0;
						}
						this.applyForm.ApplyUserId=this.$store.state.user.uid
						this.applyForm.ApplyUserName=this.$store.state.user.name
						this.applyForm.ApplyAvatar=this.$store.state.user.avatar
						this.$forceUpdate()
					}
					setTimeout(async () => {
						try {
							if (this.isCheckPermi(['/FlowService/Flow'])) {
								if (this.LeaveApplyTemplateId > 0) {
									let fromInfo=await applyFormData({
									  "applyNumber": this.applyForm.ApplyNumber,
									  "houseId": this.applyForm.HouseId,
									})
									await this.$refs.flowForm.InitData(
									  this.LeaveApplyTemplateId,
									  this.FlowParams,
									  this.applyId == null ? null : this.applyForm.ApplyNumber,
									  fromInfo.data
									);
								}
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
			choiceApplyUser(){
				this.selectText='ApplyUser'
				let select=[]
				if(this.applyForm.ApplyUserId){
					let obj={
						avatar:this.applyForm.ApplyAvatar,
						id:this.applyForm.ApplyUserId,
						name:this.applyForm.ApplyUserName,
						type:'user',
						selected:true
					}
					select.push(obj)
				}
				if(select&&select.length>0){
					uni.navigateTo({
						url: '/pages_flow/inventory/employee_select?type=user&selected='+JSON.stringify(select)
					})
				}else{
					uni.navigateTo({
						url: '/pages_flow/inventory/employee_select?type=user'
					})
				}
				
			},
			getApplyUserName(userId,row){
			  //显示申请人名称
			  if(userId==this.$store.state.user.uid){
				return this.$store.state.user.name
			  }else{
				if(row.ApplyUserInfo){
				  return row.ApplyUserInfo.RealName
				}else{
				  return ''
				}
				
			  }
			},
			async confirmCancel() {
				//确认撤销动作
				try {
					this.isLoading = true
					await deleteApply({
						id: this.applyId
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
						'你确定要删除出库申请单 ' + this.applyForm.ApplyNumber +
						'吗? (此操作是不可逆的)?'
					)
				} catch (e) {
				}
			},
			delSelDev(inx) {
				//移出
				this.applyForm.List.splice(inx, 1)
			},
			finishSelectItems(arr) {
				if (arr.length > 0) {

					let filteritems = this.applyForm.List.filter(x => arr.some(w => w.TargetId == x.TargetId));
					if (filteritems.length > 0) {
						this.wupinSelectArr = JSON.parse(JSON.stringify(filteritems))
						this.isShowSelectTips = true
						// this.$message.error("不可以重复添加物品 '" + filteritems.map(x => x.TargetName).join() + "'");
						return;
					}
					arr.map(element => {
						this.applyForm.List.push({
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
				if (this.applyForm.HouseId) {
					this.$refs.bottomPop.close()
					uni.navigateTo({
						url: '/pages_flow/inventory/inventory_list?isSelect=true&isMulSelect=true&HouseId=' + this
							.applyForm.HouseId
					})
				} else {
					this.$refs.promptMsg.open('请先选择仓库', 2000)
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
					var tmpitems = this.applyForm.List.filter(x => x.TargetId == scanArr.TargetId);
					if (tmpitems.length > 0) {
						if (scanArr.TargetType == 0) {
							tmpitems[0].Quantity += 1;
						}
					} else {
						this.applyForm.List.push({
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
				this.$refs.applyForm.validate().then(async valid => {
					if (valid) {
						// console.log(this.applyForm, 'this.recordsFormthis.recordsFormthis.applyForm');
						if (this.applyForm.List.length == 0) {
							this.$refs.promptMsg.open('请选择要申请出库的物品', 1500)
							return;
						}
						this.isLoading = true
						try {
							this.applyForm.FlowId=this.LeaveApplyTemplateId
							let response;
							if (this.applyId && this.applyId != null) {
								this.applyForm.Id = this.applyId;
								response = await applyEdit(this.applyForm);
							} else {
								response = await applyAdd(this.applyForm);
							}
							//提交审批表单
							if (this.LeaveApplyTemplateId > 0) {
								if (st == 2) {
								  let tmpmodel = this.$refs.flowForm.getModel();
								  tmpmodel["id"] = response.data;
								  await applySubmitModel(tmpmodel);
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
									await applySubmitModel({ id: response.data });
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
					this.applyForm.HouseName = item.StoreName;
					this.applyForm.HouseId = item.Id;
					this.LeaveApplyTemplateId = item.LeaveTemplateId;
					if (this.isCheckPermi(['/FlowService/Flow'])) {
						if (this.LeaveApplyTemplateId > 0) {
							let fromInfo=await applyFormData({
							  "applyNumber": this.applyForm.ApplyNumber,
							  "HouseId": this.applyForm.HouseId,
							})
							await this.$refs.flowForm.InitData(
							  this.LeaveApplyTemplateId,
							  this.FlowParams,
							  this.applyId == null ? null : this.applyForm.ApplyNumber,
							  fromInfo.data
							);
						}
					}

				}
			},
			choiceWarehouse(val) {
				this.activeHouse = val
				if (this.applyForm.HouseId) {
					uni.navigateTo({
						url: '/pages_flow/inventory/warehouse_management?isSelect=true&&selectId=' + this.applyForm.HouseId
					})

				} else {
					uni.navigateTo({
						url: '/pages_flow/inventory/warehouse_management?isSelect=true'
					})
				}
			},
			finishSelectAgents(arr) {
				if (!arr || arr.length == 0) return
				this.applyForm.ToOrgId = arr[0].OrgId;
				this.applyForm.ToName = arr[0].OrgName;
			},
			finishSelectCustom(arr) {
				if (!arr || arr.length == 0) return
				this.applyForm.ToOrgId = arr[0].BindOrgId;
				this.applyForm.ToName = arr[0].CustomerName;
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
				console.log(val,'valval');
				if(this.selectText=='ApplyUser'){
					this.selectText=''
					this.applyForm.ApplyUserId=val[0].id
					this.applyForm.ApplyUserName=val[0].name
					this.applyForm.ApplyAvatar=val[0].avatar
					this.$forceUpdate()
				}else{
					this.isChoiceUser = true
					this.choiceUser = val
				}
				
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