<template>
	<view class="pages_bgcon" style="min-height: 100vh;background-color: #fff;">
		<top :title="topTitle" leftWidth="157rpx" :isleftBack="true" leftIcon="icon-fanhui"
			backgroundColor="rgba(255, 255, 255, 1)" rightWidth="157rpx">
		</top>
		<uni-forms ref="warehouseForm" :modelValue="warehouseForm" :rules="rules" labelWidth='80' label-position="top">
			<view class="form_con page_form_con">
				<uni-forms-item label="仓库名称" required name="StoreName" id="StoreName_form" labelFont="28rpx"
					contentFont="32rpx" :requireOpacity="0.5">
					<view id="StoreName" class="form_li">
						<uni-easyinput placeholderStyle="color:rgba(193, 193, 193, 1);font-size:32rpx"
							inputHeight="88rpx" :styles="styles" type="text" v-model="warehouseForm.StoreName"
							placeholder="请输入仓库名称" contentFontSize="32rpx" />
					</view>
				</uni-forms-item>
				<uni-forms-item label="负责人" name="LeaderId" id="LeaderId_form" labelFont="28rpx" contentFont="32rpx"
					:requireOpacity="0.5">
					<view id="LeaderId" class="form_li" @click="choiceManage">
						<uni-easyinput placeholderStyle="color:rgba(193, 193, 193, 1);font-size:32rpx"
							inputHeight="88rpx" :styles="styles" type="text" v-model="warehouseForm.LeaderName"
							placeholder="请选择负责人" contentFontSize="32rpx"
							:clearable="false" />
						<view class="form_sel_icon">
							<custom-icons iconsName="icon-a-youjiantoubai" iconsSize="20rpx"
								iconsColor="#999999"></custom-icons>
						</view>
					</view>
				</uni-forms-item>
				<view class="flow_initialize_con">
					<view class="initialize_title" @click="setActiveInitialize(1)"
						:class="{'active_title':activeInitialize==1}">出库审核流程</view>
					<view class="initialize_title" @click="setActiveInitialize(2)"
						:class="{'active_title':activeInitialize==2}">入库审核流程</view>
					<view class="initialize_title" @click="setActiveInitialize(3)"
						:class="{'active_title':activeInitialize==3}">出库申请流程</view>
				</view>
				<uni-forms-item label="　" v-show="activeInitialize==1" name="LeaveTemplateName"
					id="LeaveTemplateName_form" labelFont="28rpx" contentFont="32rpx" :requireOpacity="0.5">
					<view id="LeaveTemplateName" class="form_li" @click="openFlowPicker(0)">
						<uni-easyinput placeholderStyle="color:rgba(193, 193, 193, 1);font-size:32rpx"
							inputHeight="88rpx" :styles="styles" type="text" v-model="warehouseForm.LeaveTemplateName"
							placeholder="请选择出库审核流程" contentFontSize="32rpx" 
							:clearable="false" />
						<view class="form_sel_icon" @click.stop="onClearOut">
							<custom-icons iconsName="icon-yichu1" iconsSize="30rpx" iconsColor="#999999"
								v-if="warehouseForm.LeaveTemplateName"></custom-icons>
							<custom-icons iconsName="icon-a-youjiantoubai" iconsSize="20rpx" iconsColor="#999999"
								v-else></custom-icons>
						</view>
					</view>
				</uni-forms-item>
				<uni-forms-item label="　" v-show="activeInitialize==2" name="EnterTemplateName"
					id="EnterTemplateName_form" labelFont="28rpx" contentFont="32rpx" :requireOpacity="0.5">
					<view id="EnterTemplateName" class="form_li" @click="openFlowPicker(1)">
						<uni-easyinput placeholderStyle="color:rgba(193, 193, 193, 1);font-size:32rpx"
							inputHeight="88rpx" :styles="styles" type="text" v-model="warehouseForm.EnterTemplateName"
							placeholder="请选择入库审核流程" contentFontSize="32rpx"
							:clearable="false" />
						<view class="form_sel_icon" @click.stop="onClearIn">

							<custom-icons iconsName="icon-yichu1" iconsSize="30rpx" iconsColor="#999999"
								v-if="warehouseForm.EnterTemplateName"></custom-icons>
							<custom-icons iconsName="icon-a-youjiantoubai" iconsSize="20rpx" iconsColor="#999999"
								v-else></custom-icons>
						</view>
					</view>
				</uni-forms-item>
				<uni-forms-item label="　" v-show="activeInitialize==3" name="LeaveApplyTemplateName"
					id="LeaveApplyTemplateName_form" labelFont="28rpx" contentFont="32rpx" :requireOpacity="0.5">
					<view id="LeaveApplyTemplateName" class="form_li" @click="openFlowPicker(2)">
						<uni-easyinput placeholderStyle="color:rgba(193, 193, 193, 1);font-size:32rpx"
							inputHeight="88rpx" :styles="styles" type="text"
							v-model="warehouseForm.LeaveApplyTemplateName" placeholder="请选择出库申请流程"
							contentFontSize="32rpx" :clearable="false" />
						<view class="form_sel_icon" @click.stop="onClearApply">
							<custom-icons iconsName="icon-yichu1" iconsSize="30rpx" iconsColor="#999999"
								v-if="warehouseForm.LeaveApplyTemplateName"></custom-icons>
							<custom-icons iconsName="icon-a-youjiantoubai" iconsSize="20rpx" iconsColor="#999999"
								v-else></custom-icons>
						</view>
					</view>
				</uni-forms-item>
				<houseFlow v-show="activeInitialize==1&&warehouseForm.LeaveTemplateName" ref="LeaveTemplate"
					:formInitVal="formInit[0]" :tbloading="tbloading[0]" flowTitle="出库审核流程表单初始化" :index="0"
					@setformInitData="setformInitData"></houseFlow>
				<houseFlow v-show="activeInitialize==2&&warehouseForm.EnterTemplateName" ref="EnterTemplate"
					:formInitVal="formInit[1]" :tbloading="tbloading[1]" flowTitle="入库审核流程表单初始化" :index="1"
					@setformInitData="setformInitData"></houseFlow>
				<houseFlow v-show="activeInitialize==3&&warehouseForm.LeaveApplyTemplateName"
					ref="LeaveApplyTemplate" :formInitVal="formInit[2]" :tbloading="tbloading[2]"
					flowTitle="出库申请流程表单初始化" :index="2" @setformInitData="setformInitData"></houseFlow>
				<uni-forms-item label="备注" name="Remark" id="Remark_form" labelFont="28rpx" contentFont="32rpx"
					:requireOpacity="0.5">
					<view id="Remark" class="form_li">
						<uni-easyinput placeholderStyle="color:rgba(193, 193, 193, 1);font-size:32rpx"
							inputHeight="70rpx" :styles="styles" type="textarea" v-model="warehouseForm.Remark"
							placeholder="请输入备注" contentFontSize="32rpx"
							autoHeight />
					</view>
				</uni-forms-item>
				<view class="form_li_flex" v-if="warehouseForm.Id!=undefined">
					<view class="label">
						是否启用
					</view>
					<view class="value">
						<view class="conten_icon t-icon-kai" v-if="warehouseForm.Status=='1'"
							@click.stop="setEnableDis('0')"></view>
						<view class="conten_icon t-icon-guan1" v-else-if="warehouseForm.Status=='0'"
							@click.stop="setEnableDis('1')"></view>
					</view>
				</view>
				<button class="submit_button" @click="submit" :disabled="isLoading"
					:style="{'opacity':isLoading?0.6:1,'margin-top': '60rpx'}" :loading="isLoading"
					v-if="warehouseForm.Id==undefined">
					保存
				</button>
				<view class="btn_con" v-else>
					<button class="jump_button" @click="deleteHouse" :disabled="isLoading"
						:style="{'opacity':isLoading?0.6:1}" :loading="isLoading">
						删除
					</button>
					<button class="submit_button" @click="submit" :disabled="isLoading"
						:style="{'opacity':isLoading?0.6:1}" :loading="isLoading">
						保存
					</button>
				</view>
			</view>

		</uni-forms>
		<msg-prompt ref="promptMsg" @confirm="confirmDelete" @msgClose="msgClose"></msg-prompt>
	</view>
</template>

<script>
	import {
		houseList,
		addHouse,
		editHouse,
		delHouse,
		houseInfo,
		changeStatus
	} from "@/api/house.js";
	import {
		getUser
	} from '@/api/user.js'
	import {
		setPagesParam
	} from '@/common/utillib.js'
	import {
		flowminix
	} from "./flowminix";
	import houseFlow from "./house/house_flow.vue"
	export default {
		name: 'HouseList',
		mixins: [flowminix],
		components: {
			houseFlow
		},
		data() {
			return {
				topTitle: '新增仓库',
				styles: {
					color: '#333',
					backgroundColor: 'rgba(248, 248, 248, 1)',
					disableColor: 'rgba(28, 34, 50, 1)',
					borderColor: 'rgba(255, 255, 255, 0.20)'
				},
				rules: {
					StoreName: {
						rules: [{
							required: true,
							errorMessage: '请输入仓库名称',
						}]
					}
				},
				warehouseForm: {
					Status: '1',
					StoreName: "",
					Remark: "",
					LeaveTemplateId: 0,
					EnterTemplateId: 0,
					LeaveApplyTemplateId: 0,
					LeaveTemplateName: "",
					EnterTemplateName: "",
					LeaveApplyTemplateName: ""
				},
				isLoading: false,
				isDelete: false,
				activeIdx: 0,
				activeInitialize: 1
				// cantloadClose:false
			};
		},

		async onLoad(options) {
			if (options.houseId) {
				this.warehouseForm.Id = options.houseId
				this.topTitle = '编辑仓库'
				await this.getHouseInfo()
			}
		},
		methods: {
			setActiveInitialize(val){
				this.activeInitialize=val
			},
			openFlowPicker(idx) {
				this.activeIdx = idx;
				uni.navigateTo({
					url: '/pages_flow/add_process?isSel=true'
				})
			},
			onClearOut() {
				this.warehouseForm.LeaveTemplateId = 0;
				this.warehouseForm.LeaveTemplateName = "";
				this.warehouseForm.LeaveFlowInitJson = "";
				this.setformInitData([], 0)
			},
			onClearIn() {
				this.warehouseForm.EnterTemplateId = 0;
				this.warehouseForm.EnterTemplateName = "";
				this.warehouseForm.EnterFlowInitJson = "";
				this.setformInitData([], 1)
			},
			onClearApply() {
				this.warehouseForm.LeaveApplyTemplateId = 0;
				this.warehouseForm.LeaveApplyTemplateName = "";
				this.warehouseForm.LeaveApplyFlowInitJson = "";
				this.setformInitData([], 2)
			},
			setEnableDis(val) {
				//设置停用启用的值
				if (this.warehouseForm.Status != val) {
					this.isDelete = false
					this.warehouseForm.Status = val
					if (this.warehouseForm.Status == '0') {
						this.$refs.promptMsg.noticeOpen(
							'是否确定禁用 ' + this.warehouseForm.StoreName + '?', '提示', true
						)
					} else {
						this.$refs.promptMsg.noticeOpen(
							'是否确定启用 ' + this.warehouseForm.StoreName + '?', '提示', true
						)
					}
				}
			},
			deleteHouse() {
				//删除仓库
				this.isDelete = true
				this.$refs.promptMsg.noticeOpen(
					'是否确定要删除名称为 ' + this.warehouseForm.StoreName + '的数据项?',
					'提示', true
				)
			},
			msgClose() {

				if (!this.isDelete) {
					if (this.warehouseForm.Status == '0') {
						this.warehouseForm.Status = '1'
					} else {
						this.warehouseForm.Status = '0'
					}
				}
			},
			confirmDelete() {
				//确认删除
				if (this.isDelete) {
					this.isLoading = true
					delHouse(this.warehouseForm.Id).then(res => {
						this.$refs.promptMsg.open('删除成功', 1500)
						setTimeout(() => {
							setPagesParam('getHouseList', 'load', 1)
						}, 1500)
					}).catch(err => {
						this.isLoading = false
						this.setMsgTop(err)
					})
				} else {
					if (this.warehouseForm.Status == '0') {
						this.$refs.promptMsg.loadingOpen('禁用中...')
					} else {
						this.$refs.promptMsg.loadingOpen('启用中...')
					}
					changeStatus(this.warehouseForm.Id, this.warehouseForm.Status).then(res => {
						this.$refs.promptMsg.loadingColse()
						if (this.warehouseForm.Status == '0') {
							this.$refs.promptMsg.open('仓库已被禁用', 1500)
							setPagesParam('getHouseList', 'load', 1, false)
						}
						if (this.warehouseForm.Status == '1') {
							this.$refs.promptMsg.open('仓库已启用', 1500)
							setPagesParam('getHouseList', 'load', 1, false)
						}
					}).catch(err => {
						this.$refs.promptMsg.loadingColse()
						this.setMsgTop(err)
						if (this.warehouseForm.Status == '0') {
							this.warehouseForm.Status = '1'
						} else {
							this.warehouseForm.Status = '0'
						}
					})
				}

			},
			async getHouseInfo() {
				//获取仓库信息
				uni.showLoading({
					title: '加载中'
				})
				try {
					let res = await houseInfo(this.warehouseForm.Id)
					this.warehouseForm = JSON.parse(JSON.stringify(res.data))
					if(res.data.LeaveTemplateId){
						this.activeIdx=0
						try{
							await this.onSelected({Id:res.data.LeaveTemplateId,Name:res.data.LeaveTemplateName},true,JSON.parse(res.data.LeaveFlowInitJson))
						}catch(e){
							//TODO handle the exception
						}
					}
					if(res.data.EnterTemplateId){
						this.activeIdx=1
						try{
							await this.onSelected({Id:res.data.EnterTemplateId,Name:res.data.EnterTemplateName},true,JSON.parse(res.data.EnterFlowInitJson))
						}catch(e){
							//TODO handle the exception
						}
					}
					if(res.data.LeaveApplyTemplateId){
						this.activeIdx=2
						try{
							await this.onSelected({Id:res.data.LeaveApplyTemplateId,Name:res.data.LeaveApplyTemplateName},true,JSON.parse(res.data.LeaveApplyFlowInitJson))
						}catch(e){
							//TODO handle the exception
						}
					}
					let userRsp = await getUser(this.warehouseForm.LeaderId)
					this.warehouseForm.LeaderName = userRsp.data.user.RealName;
					this.warehouseForm.Avatar = userRsp.data.user.avatar
					uni.hideLoading()
				} catch (e) {
					//TODO handle the exception
					uni.hideLoading()
					this.setMsgTop(e)
				}
			},
			submit() {
				//保存
				this.$refs.warehouseForm.validate().then(res => {
					let leaveFlowInit = this.returnformInit(0)
					for (let i = 0; i < leaveFlowInit.length; i++) {
						if (leaveFlowInit[i].way == 1 && leaveFlowInit[i].val == "") {
							// this.$modal.msgError('出库审核流程' + leaveFlowInit[i].title + "未选择初始值");
							uni.showToast({
								title:'出库审核流程' + leaveFlowInit[i].title + "未选择初始值",
								icon:'error'
							})
							return;
						}
					}
					let enterFlowInit = this.returnformInit(1)
					for (let i = 0; i < enterFlowInit.length; i++) {
						if (enterFlowInit[i].way == 1 && enterFlowInit[i].val == "") {
							// this.$modal.msgError('入库审核流程' + enterFlowInit[i].title + "未选择初始值");
							uni.showToast({
								title:'入库审核流程' + enterFlowInit[i].title + "未选择初始值",
								icon:'error'
							})
							return;
						}
					}
					let leaveApplyFlowInit = this.returnformInit(2)
					for (let i = 0; i < leaveApplyFlowInit.length; i++) {
						if (leaveApplyFlowInit[i].way == 1 && leaveApplyFlowInit[i].val == "") {
							// this.$modal.msgError('出库申请流程' + leaveApplyFlowInit[i].title + "未选择初始值");
							uni.showToast({
								title:'出库申请流程' + leaveApplyFlowInit[i].title + "未选择初始值",
								icon:'error'
							})
							return;
						}
					}
					this.warehouseForm.LeaveFlowInitJson = JSON.stringify(leaveFlowInit)
					this.warehouseForm.EnterFlowInitJson = JSON.stringify(enterFlowInit)
					this.warehouseForm.LeaveApplyFlowInitJson = JSON.stringify(leaveApplyFlowInit)
					if (this.warehouseForm.Id != undefined) {
						this.editHouse()
					} else {
						this.addHouse()
					}
				})
			},
			addHouse() {
				//添加仓库
				this.isLoading = true
				addHouse(this.warehouseForm).then(response => {
					this.$refs.promptMsg.open('添加成功', 1500)
					setTimeout(() => {
						setPagesParam('getHouseList', 'load', 1)
					}, 1500)
				}).catch(err => {
					this.isLoading = false
					this.setMsgTop(err)
				});
			},
			editHouse() {
				//修改仓库
				this.isLoading = true
				if (this.warehouseForm.LeaderId == null) {
					this.warehouseForm.LeaderId = 0;
					this.warehouseForm.LeaderId = 0;
				}
				editHouse(this.warehouseForm).then(response => {
					this.$refs.promptMsg.open('修改成功', 1500)
					setTimeout(() => {
						setPagesParam('getHouseList', 'load', 1)
					}, 1500)
				}).catch(err => {
					this.isLoading = false
					this.setMsgTop(err)
				});
			},
			selectEmplee(val) {
				//选中负责人后
				if (val) {
					let obj = val[0]
					this.warehouseForm.LeaderId = obj.id
					this.warehouseForm.Avatar = obj.avatar
					this.warehouseForm.LeaderName = obj.name
					this.$forceUpdate()
				}

			},
			choiceManage() {
				//选择负责人
				let selected = JSON.stringify([{
					id: this.warehouseForm.LeaderId,
					name: this.warehouseForm.LeaderName,
					avatar: this.warehouseForm.Avatar,
					type: "user"
				}])
				uni.navigateTo({
					url: '/pages_flow/inventory/employee_select?selected=' + selected
				})
			}
		}
	}
</script>

<style lang="less">
	// .submit_button {
	// 	margin-top: 60rpx;
	// }
	.btn_con {
		.jump_button {
			border: none;
			background-color: rgba(248, 248, 248, 1);
			color: rgba(153, 153, 153, 1);
		}

		.jump_button::after {
			border: none;
		}
	}

	.form_li_flex {
		display: flex;
		justify-content: space-between;
		align-items: center;

		.label {
			color: rgba(153, 153, 153, 1);
			font-size: 28rpx;
		}

		.value {
			.conten_icon {
				width: 80rpx;
				height: 44rpx;
			}
		}
	}
	.flow_initialize_con{
		width: 100%;
		display: flex;
		justify-content: flex-start;
		align-items: center;
		border-top: 1rpx solid rgba(234, 234, 234, 1);
		.initialize_title{
			width: 33.3%;
			height: 88rpx;
			background-color: rgba(248, 248, 248, 1);
			color: rgba(153, 153, 153, 1);
			text-align: center;
			text-align: center;
			line-height: 88rpx;
			font-size: 28rpx;
			&.active_title{
				background-color: #ffffff;
				color: #333333;
			}
		}
	}
</style>