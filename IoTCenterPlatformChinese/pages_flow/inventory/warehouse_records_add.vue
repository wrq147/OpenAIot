<template>
	<view style="height: 100%; overflow-x: hidden" @click="isSelKd = false">
		<top :title="topTitle" leftWidth="120rpx" :isleftBack="true" leftIcon="icon-fanhui" backgroundColor="#ffffff"
			rightWidth="100rpx"></top>
		<view class="detail_con" style="background-color: rgba(245, 248, 249, 1); padding-bottom: 0">
			<view class="detail_status" v-if="enterId && enterId != null">
				<custom-icons iconsName="icon-daitijiao" iconsSize="36rpx" iconsColor="#333333"></custom-icons>
				<view class="status_text">待提交</view>
			</view>
		</view>
		<uni-forms ref="recordsForm" :modelValue="recordsForm" :rules="rules" labelWidth="80" label-position="top">
			<view class="form_con page_form_con">
				<uni-forms-item label="入库单号" required name="StockNumber" id="StockNumber_form" labelFont="32rpx"
					contentFont="32rpx" :requireOpacity="0.5">
					<view id="StockNumber" class="form_li">
						<uni-easyinput placeholderStyle="color:#C1C1C1;font-size:32rpx" inputHeight="88rpx"
							:styles="styles" type="text" v-model="recordsForm.StockNumber" placeholder="请输入入库单编号"
							contentFontSize="32rpx" primaryColor="rgba(35, 113, 255, 1)" :disabled="true" />
					</view>
				</uni-forms-item>
				<uni-forms-item label="入库时间" required name="InDate" id="InDate_form" labelFont="32rpx"
					contentFont="32rpx" :requireOpacity="0.5">
					<view id="InDate" class="form_li">
						<view class="int_date">
							<view class="date_con long_date" style="color: #fff">
								<uni-datetime-picker ref="dateChoice1" class="date" type="datetime"
									placeholder-style="font-size:32rpx;color:#C1C1C1" :clear-icon="false"
									v-model="recordsForm.InDate" placeholder="请选择入库时间" :isCustom="true" :isDark="false">
									<view class="date_slot" :class="{ has_val: recordsForm.InDate }">
										<custom-icons iconsName="icon-xuanzeshijian" iconsSize="28rpx"
											iconsColor="#C1C1C1"></custom-icons>
										<view class="text">
											{{
                                                recordsForm.InDate
                                                    ? recordsForm.InDate
                                                    : '请选择入库时间'
                                            }}
										</view>
									</view>
								</uni-datetime-picker>
							</view>
						</view>
					</view>
				</uni-forms-item>
				<uni-forms-item label="物流编号" name="ExpressNumber" id="ExpressNumber_form" labelFont="32rpx"
					contentFont="32rpx" :requireOpacity="0.5">
					<view id="ExpressNumber" class="form_li">
						<view class="sel_input">
							<view class="sel" @click.stop="openKDSel">
								<view class="sel_text" :class="{ pal: !recordsForm.ExpressCompanyName }">
									{{
                                        recordsForm.ExpressCompanyName
                                            ? recordsForm.ExpressCompanyName
                                            : '请选择'
                                    }}
								</view>
								<custom-icons iconsName="icon-xialajiantou" iconsSize="12rpx"
									iconsColor="rgba(255, 255, 255, 0.5)"></custom-icons>
							</view>
							<view class="sel_line"></view>
							<view class="input">
								<uni-easyinput placeholderStyle="color:#C1C1C1;font-size:32rpx" inputHeight="88rpx"
									:styles="styles" type="text" v-model="recordsForm.ExpressNumber"
									placeholder="请输入物流编号" contentFontSize="32rpx" primaryColor="rgba(35, 113, 255, 1)"
									@input="expressSelected" @blur="expressSelected" />
							</view>
							<view class="sel_list" v-show="isSelKd">
								<view class="sel_li" v-for="item in kdcompanys" @click.stop="choiceKd(item)">
									{{ item.Name }}
								</view>
							</view>
						</view>
					</view>
				</uni-forms-item>
				<uni-forms-item label="联系手机" required name="ExpressPhone" id="ExpressPhone_form" labelFont="32rpx"
					contentFont="32rpx" :requireOpacity="0.5" v-if="recordsForm.ExpressCompany == 'shunfeng'">
					<view id="ExpressPhone" class="form_li">
						<uni-easyinput placeholderStyle="color:#C1C1C1;font-size:32rpx" inputHeight="88rpx"
							:styles="styles" type="text" v-model="recordsForm.ExpressPhone" placeholder="请输入物流联系手机"
							contentFontSize="32rpx" primaryColor="rgba(35, 113, 255, 1)" />
					</view>
				</uni-forms-item>
				<uni-forms-item label="目标仓库" required name="HouseId" id="HouseId_form" labelFont="32rpx"
					contentFont="32rpx" :requireOpacity="0.5">
					<view id="HouseId" class="form_li" @click="choiceWarehouse">
						<!-- <uni-easyinput placeholderStyle="color:#C1C1C1;font-size:32rpx"
							inputHeight="88rpx" :styles="styles" type="text" v-model="recordsForm.HouseName"
							placeholder="请选择仓库" contentFontSize="32rpx"
							primaryColor="rgba(255, 255, 255, 0.5)" :clearable="false" /> -->
						<view class="form_input" :class="{
                                placeholder_input:
                                    !recordsForm.HouseName || recordsForm.HouseName == '',
                            }">
							{{ recordsForm.HouseName ? recordsForm.HouseName : '请选择仓库' }}
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
							contentFontSize="32rpx" primaryColor="rgba(35, 113, 255, 1)" autoHeight />
					</view>
				</uni-forms-item>
				<uni-forms-item label="入库物品" required name="List" id="List_form" labelFont="32rpx" contentFont="32rpx"
					:requireOpacity="0.5" labelPosition="top">
					<view id="List" class="form_li">
						<view class="form_device_list">
							<view class="form_device_li" v-for="(item, inx) in recordsForm.List" :key="item.Id">
								<view class="li_left">
									<image v-if="item.PhotoUrl + '?wh=500x500'" class="image" :src="item.PhotoUrl" mode=""></image>
									<image v-else class="image" :src="getSerVerUrl()+'/appimg/device_default.png'" mode="aspectFill"></image>
								</view>
								<view class="li_right">
									<view class="name">{{ item.TargetName }}</view>
									<view class="number">{{ item.TargetNumber }}</view>
									<view class="price_quantity" v-if="item.TargetType == 1">
										<view class="input_li mar_input">
											<text class="label">价格:</text>
											<uni-number-box background="#F8F8F8" v-model="item.Price" color="#333333"
												:max="100000000" />
										</view>
										<view class="input_li">
											<text class="label">数量:</text>
											<uni-number-box background="#F8F8F8" v-model="item.Quantity" color="#333333"
												:min="0" :max="1" />
										</view>
									</view>
									<view class="price_quantity" v-if="item.TargetType == 0">
										<view class="input_li mar_input">
											<text class="label">价格:</text>
											<uni-number-box background="#F8F8F8" v-model="item.Price" color="#333333"
												:max="100000000" />
										</view>
										<view class="input_li">
											<text class="label">数量:</text>
											<uni-number-box background="#F8F8F8" v-model="item.Quantity"
												color="#333333" />
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
								<view class="text">添加物品</view>
							</view>
						</view>
					</view>
				</uni-forms-item>
			</view>
		</uni-forms>
		<ProcessCompot ref="flowForm" @isChoiceFlowUserFun="isChoiceFlowUserFun" @setActiveItem="setActiveItem">
			入库审核
		</ProcessCompot>
		<view class="form_con page_form_con">
			<view class="sub_save_con" :class="{ has_btn: enterId && enterId != null }">
				<button class="delete_button" @click="cancelWare" :disabled="isLoading"
					:style="{ opacity: isLoading ? 0.6 : 1 }" :loading="isLoading" v-if="enterId && enterId != null">
					撤销
				</button>
				<button class="submit_button" @click="submitForm(1)" :disabled="isLoading"
					:style="{ opacity: isLoading ? 0.6 : 1 }" :loading="isLoading">
					保存
				</button>
				<button class="jump_button" @click="submitForm(2)" :disabled="isLoading"
					:style="{ opacity: isLoading ? 0.6 : 1 }" :loading="isLoading">
					提交
				</button>
			</view>
		</view>
		<view class="zhanwei" style="width: 100%; height: 40rpx"></view>
		<uni-popup ref="bottomPop" type="bottom" :mask-click="true" :zIndex="999"
			maskBackgroundColor="rgba(0, 0, 0, 0.5)">
			<view class="popup_list">
				<!-- <view class="popup_li borradio" v-if="isCheckPermi(['/ProducerService/Parts/List'])"
					@click.stop="slectConsumables">
					选择耗材
				</view> -->
				<view class="popup_li" @click.stop="slectMachines">
					选择产品
				</view>
				<view class="popup_li" @click.stop="scanAdd">扫码添加</view>
				<view class="popup_li cancel" @click="closePopup">取消</view>
			</view>
		</uni-popup>
		<msg-prompt ref="promptMsg" @confirm="confirmCancel"></msg-prompt>
	</view>
</template>

<script>
	import {
		enterDevList,
		manualPile,
		enterFormData,
		submitManualPile,
		submitManualPileModel,
		generateRKNumber,
		getEnterInfo,
		cancelEnter,
		inStockByKey,
	} from '@/api/stock';
	import {
		houseList
	} from '@/api/house.js';
	import {
		getStockConfig
	} from '@/api/config';
	import ProcessCompot from '@/pages_flow/flow-form/process-compot.vue';
	import {
		autoCompany
	} from '@/api/code.js';
	import {
		setPagesParam
	} from '@/common/utillib.js';
	var dayjs = require('@/common/day.js');

	export default {
		components: {
			ProcessCompot,
		},
		data() {
			return {
				selectDeviceList: [],
				topTitle: '创建入库单',
				isLoading: false,
				styles: {
					color: '#333',
					backgroundColor: '#F8F8F8',
					disableColor: '#F8F8F8',
					borderColor: '#F8F8F8',
				},
				recordsForm: {
					StockNumber: '',
					HouseName: '',
					InDate: '',
					HouseId: undefined,
					Remark: '',
					ExpressNumber: '',
					ExpressCompany: '',
					ExpressPhone: '',
					List: [],
				},
				rules: {
					StockNumber: {
						rules: [{
							required: true,
							errorMessage: '请输入入库单编号',
						}, ],
					},
					HouseName: {
						rules: [{
							required: true,
							errorMessage: '请选择仓库',
						}, ],
					},
					InDate: {
						rules: [{
							required: true,
							errorMessage: '请选择入库时间',
						}, ],
					},
					HouseId: {
						rules: [{
							required: true,
							errorMessage: '请选择仓库',
						}, ],
					},
					// ExpressNumber: {
					// 	rules: [{
					// 		required: true,
					// 		errorMessage: 'Please enter warehouse name',
					// 	}]
					// },
					// ExpressCompany: {
					// 	rules: [{
					// 		required: true,
					// 		errorMessage: 'Please enter warehouse name',
					// 	}]
					// },
					ExpressPhone: {
						rules: [{
							required: true,
							errorMessage: '请输入物流联系手机',
						}, ],
					},
					List: {
						rules: [{
							required: true,
							errorMessage: '请选择入库物品',
						}, ],
					},
				},
				enterId: null,
				enterTemplateId: 0,
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
			};
		},
		computed: {
			FlowParams: function() {
				return {
					'@from': this.recordsForm.StockNumber,
					'@fromtype': '入库单',
				};
			},
		},
		onLoad(options) {
			this.$nextTick(async () => {
				try {
					this.$refs.promptMsg.loadingOpen();
					this.kdcompanys = await this.$store.dispatch('data/kuaiDiList');
					if (options.id) {
						this.enterId = options.id;
						let enterInfo = await getEnterInfo(this.enterId);
						this.recordsForm.StockNumber = enterInfo.data.StockNumber;
						this.recordsForm.HouseName = enterInfo.data.ToHouseName;
						this.recordsForm.InDate = enterInfo.data.InDate;
						this.recordsForm.HouseId = enterInfo.data.ToHouseId;
						this.recordsForm.Remark = enterInfo.data.Remark;
						this.recordsForm.ExpressNumber = enterInfo.data.ExpressNumber;
						this.recordsForm.ExpressCompany = enterInfo.data.ExpressCompany;
						let arr = this.kdcompanys.find(
							(row) => row.Code == this.recordsForm.ExpressCompany
						);
						this.recordsForm.ExpressCompanyName = arr.Name;
						this.recordsForm.ExpressPhone = enterInfo.data.ExpressPhone;
						this.recordsForm.List = enterInfo.data.List;
						if (enterInfo.data.ToHouseEnterTemplateId) {
							this.enterTemplateId = enterInfo.data.ToHouseEnterTemplateId;
						} else {
							this.enterTemplateId = 0;
						}

						this.topTitle = '编辑入库单';
					} else {
						let ssp = await generateRKNumber();
						this.recordsForm.StockNumber = ssp.data;
						this.recordsForm.InDate = dayjs(new Date()).format('YYYY-MM-DD HH:mm:ss');
						this.topTitle = '创建入库单';
						let hslist = (
							await houseList({
								IsSystem: true,
							})
						).data.List;
						if (hslist.length > 0) {
							this.recordsForm.HouseName = hslist[0].StoreName;
							this.recordsForm.HouseId = hslist[0].Id;
							this.enterTemplateId = hslist[0].EnterTemplateId;
						} else {
							this.enterTemplateId = 0;
						}
						this.$forceUpdate();
					}
					setTimeout(async () => {
						try {
							if (this.isCheckPermi(['/FlowService/Flow'])) {
								// let rsp = await getStockConfig(0);
								// this.enterTemplateId = rsp.data.EnterTemplateId;

								if (this.enterTemplateId > 0) {
									let fromInfo = await enterFormData({
										applyNumber: this.recordsForm.StockNumber,
										toHouseId: this.recordsForm.HouseId,
									});
									await this.$refs.flowForm.InitData(
										this.enterTemplateId,
										this.FlowParams,
										this.enterId == null ? null : this.recordsForm
										.StockNumber,
										fromInfo.data
									);
								}
							}
							this.$refs.promptMsg.loadingColse();
						} catch (e) {
							//TODO handle the exception
							this.$refs.promptMsg.loadingColse();
							this.setMsgTop(e);
						}
					}, 100);
				} catch (e) {
					//TODO handle the exception
					console.log(e);
					this.$refs.promptMsg.loadingColse();
					this.setMsgTop(e);
				}
			});
		},
		onShow() {
			//使用onShow生命周期的特点实现对各种页面选择方法
			if (this.isChoiceUser) {
				this.$refs.flowForm.selectEmplee(
					this.isChoiceFlowUser,
					this.choiceUser,
					this.activeItemId
				);
				this.isChoiceUser = false;
			}
			if (this.isChoiceDept) {
				this.$refs.flowForm.selectDept(this.choiceDept, this.activeItemId);
				this.isChoiceDept = false;
			}
			if (this.isChoiceDevice) {
				this.$refs.flowForm.selectDevice(this.choiceDevice, this.activeItemId);
				this.isChoiceDevice = false;
			}
		},
		methods: {
			async confirmCancel() {
				//确认撤销动作
				try {
					this.isLoading = true;
					await cancelEnter({
						Id: this.enterId,
					});
					this.$refs.promptMsg.open('操作成功', 1500);
					setTimeout(() => {
						setPagesParam('loadData', 'load', 1);
					}, 1500);
				} catch (e) {
					//TODO handle the exception
					this.setMsgTop(e);
					this.isLoading = false;
				}
			},
			async cancelWare() {
				try {
					this.$refs.promptMsg.noticeOpen(
						'你是否确定要删除单据编号为 ' +
						this.recordsForm.StockNumber +
						'的入库单? (此操作不可逆)?'
					);
				} catch (e) {
					console.log(e);
				}
			},
			async expressSelected(event) {
				let val = '';
				if (event) {
					if (event.detail) {
						val = event.detail.value;
					} else {
						val = event;
					}
				} else {
					return;
				}
				if (val.length < 8) return;
				this.recordsForm.ExpressCompany = (await autoCompany(val)).data;
				let arr = this.kdcompanys.find((row) => row.Code == this.recordsForm.ExpressCompany);
				this.recordsForm.ExpressCompanyName = arr.Name;
			},
			openKDSel() {
				//打开快递选择列表
				this.isSelKd = !this.isSelKd;
			},
			choiceKd(row) {
				//选择快递
				this.isSelKd = false;
				this.recordsForm.ExpressCompany = row.Code;
				this.recordsForm.ExpressCompanyName = row.Name;
				this.$forceUpdate();
			},
			delSelDev(inx) {
				//移出
				this.recordsForm.List.splice(inx, 1);
			},
			slectMachines() {
				//选择产品批次
				this.isPagesSelectMachines = true;
				this.$refs.bottomPop.close();
				let arr = this.recordsForm.List.filter((x) => x.TargetId);
				uni.navigateTo({
					url: '/pages_device/batch_list?isMulSelect=true&selectDevice=' + JSON.stringify(arr),
				});
			},
			selectDevice(selarr) {
				//选择完设备
				if (this.isPagesSelectMachines) {
					// this.scanAddItems(selarr[0])
					console.log(selarr,'selarr');
					let IdList = this.recordsForm.List.map((item) => {
						return item.TargetId;
					  });
					  selarr.forEach((element) => {
						if (element.Id&&IdList.includes(element.Id)||element.TargetId&&IdList.includes(element.TargetId)) {
						} else {
						  this.recordsForm.List.push({
							PhotoUrl:element.PhotoUrl,
							TargetType: element.ProductLabel == 'U' ? 0 : 1,
							TargetId: element.Id,
							TargetNumber: element.Number,
							Quantity: 1,
							Price: element.Price?element.Price:0,
							TargetName: element.BatchName,
						  });
						}
					  });
					this.isPagesSelectMachines = false;
				} else {
					//选择设备
					// this.$refs.form.selectDept(val,this.activeItemId)
					this.isChoiceDevice = true;
					// this.$refs.flowForm.selectDept(val, this.activeItemId)
					this.choiceDevice = selarr;
				}
			},
			scanAddItems(scanArr) {
				//扫码完获取物品信息
				if (scanArr&&scanArr.Id) {
				  var findix = this.recordsForm.List.findIndex((x) => x.TargetId == scanArr.Id);
				  // console.log(findix,'findixfindix');
				  if (findix!=undefined&&findix>-1) {
					if (scanArr.ProductLabel == 'U') {
						this.recordsForm.List[findix].Quantity += 1;
					}
				  } else {
					this.recordsForm.List.push({
					PhotoUrl:scanArr.PhotoUrl,
					TargetType: scanArr.ProductLabel == 'U' ? 0 : 1,
					TargetId: scanArr.Id,
					TargetNumber: scanArr.Number,
					Quantity: 1,
					Price: scanArr.Price?scanArr.Price:0,
					TargetName: scanArr.BatchName,
					Unit: scanArr.Unit ? scanArr.Unit : '',
					});
				  }
					
				}
			},
			scanAdd() {
				//扫码添加
				let that = this;
				uni.scanCode({
					success: function(res) {
						let currenturl = res.result;
						let targetUID = '';
						let tmpidx = res.result.lastIndexOf('iot');
						if (tmpidx > -1) {
							targetUID = res.result.substring(tmpidx);
						} else if (
							currenturl.indexOf('HD') == 0 ||
							res.result.indexOf('UID') > -1 ||
							res.result.indexOf('Codes') > -1
						) {
							if (currenturl.indexOf('HD') == 0) {
								//为一期设备码
								targetUID = currenturl;
							} else {
								let tmpidx1 = res.result.lastIndexOf('UID');
								targetUID = res.result.substring(tmpidx1);
								if (targetUID == null) {
									this.$refs.promptMsg.open('无效的二维码', 2000);
									return;
								}
							}
							if (res.result.indexOf('Codes') > -1) {
								let paramCodes = res.result.lastIndexOf('Codes');
								targetUID = res.result.substring(paramCodes);
							}
						} else {
							this.$refs.promptMsg.open('无效的二维码', 2000);
						}
						inStockByKey(targetUID)
							.then((res) => {
								if (res.data) {
									this.scanAddItems(res.data);
								}
							})
							.catch((err) => {
								this.setMsgTop(err);
							});
					},
				});
			},

			closePopup() {
				this.$refs.bottomPop.close();
			},
			toChoiceDevice() {
				this.$refs.bottomPop.open();
			},
			submitForm(st) {
				this.$refs.recordsForm
					.validate()
					.then(async (valid) => {
						if (valid) {
							if (this.recordsForm.List.length == 0) {
								this.$refs.promptMsg.open('请选择要入库的物品', 1500);
								return;
							}
							try {
								if (this.enterId && this.enterId != null) {
									this.recordsForm.Id = this.enterId;
								}
								this.isLoading = true;
								let response = await manualPile(this.recordsForm);
								if (this.enterTemplateId > 0) {
									if (st == 2) {
										let tmpmodel = this.$refs.flowForm.getModel();
										tmpmodel['id'] = response.data;
										await submitManualPileModel(tmpmodel);
									} else {
										await this.$refs.flowForm.submitForm(st, (rss) => {});
									}
									this.$refs.promptMsg.open('操作成功', 1500);
									setTimeout(() => {
										setPagesParam('loadData', 'load', 1);
									}, 1500);
									this.$store.commit('SET_INVENTORY_INFO', true);
									// this.isLoading = false
								} else {
									if (st == 2) {
										await submitManualPileModel({
											id: response.data,
										});
									}
									this.$refs.promptMsg.open('操作成功', 1500);
									setTimeout(() => {
										setPagesParam('loadData', 'load', 1);
									}, 1500);
									this.$store.commit('SET_INVENTORY_INFO', true);
								}
							} catch (e) {
								//TODO handle the exception
								this.setMsgTop(e);
								this.isLoading = false;
							}
						}
					})
					.catch((err) => {
						console.log('err', err);
						if (err && err.length > 0) {
							let firstErr = '#' + err[0].key;
							// #ifdef MP-WEIXIN
							const query = uni.createSelectorQuery().in(this);
							query
								.select(firstErr)
								.boundingClientRect((data) => {
									uni.pageScrollTo({
										scrollTop: data.top - 200,
										// selector: firstErr,
										duration: 300,
									});
								})
								.exec();
							// #endif
							// #ifndef MP-WEIXIN
							uni.pageScrollTo({
								selector: firstErr + '_form',
								duration: 300,
							});
							// #endif
						}
					});
			},
			async selectHouse(item) {
				if (item) {
					this.recordsForm.HouseName = item.StoreName;
					this.recordsForm.HouseId = item.Id;
					this.enterTemplateId = item.EnterTemplateId;
					if (this.isCheckPermi(['/FlowService/Flow'])) {
						if (this.enterTemplateId > 0) {
							let fromInfo = await enterFormData({
								applyNumber: this.recordsForm.StockNumber,
								toHouseId: this.recordsForm.HouseId,
							});
							await this.$refs.flowForm.InitData(
								this.enterTemplateId,
								this.FlowParams,
								this.enterId == null ? null : this.recordsForm.StockNumber,
								fromInfo.data
							);
						}
					}
				}
			},
			choiceWarehouse() {
				if (this.recordsForm.HouseId) {
					uni.navigateTo({
						url: '/pages_flow/inventory/warehouse_management?isSelect=true&&selectId=' +
							this.recordsForm.HouseId,
					});
				} else {
					uni.navigateTo({
						url: '/pages_flow/inventory/warehouse_management?isSelect=true',
					});
				}
			},
			//流程组件相关函数
			isChoiceFlowUserFun(val) {
				this.isChoiceFlowUser = val;
			},
			setActiveItem(val) {
				this.activeItemId = val;
				// this.$refs.setActiveItem.selectDept(this.activeItemId)
			},
			selectDept(val) {
				//选择部门

				this.isChoiceDept = true;
				// this.$refs.flowForm.selectDept(val, this.activeItemId)
				this.choiceDept = val;
			},
			selectEmplee(val) {
				//选择人员
				// this.$refs.flowForm.selectEmplee(this.isChoiceFlowUser, val, this.activeItemId)
				this.isChoiceUser = true;
				this.choiceUser = val;
			},
			//流程组件相关函数
		},
	};
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