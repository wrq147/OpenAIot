<template>
	<view>
		<top :title="topTitle" leftWidth="120rpx" :isleftBack="true" leftIcon="icon-fanhui" backgroundColor="#ffffff"
			rightWidth="100rpx">
		</top>
		<uni-forms ref="taskForm" :modelValue="taskForm" :rules="rules" labelWidth='80' label-position="top">
			<view class="form_con page_form_con">
				<uni-forms-item label="计划名称" required name="planName" id="planName_form" labelFont="32rpx"
					contentFont="32rpx" :requireOpacity="0.5">
					<view id="planName" class="form_li">
						<uni-easyinput placeholderStyle="color:#C1C1C1;font-size:32rpx" inputHeight="88rpx"
							:styles="styles" type="text" v-model="taskForm.planName" placeholder="请输入计划名称"
							contentFontSize="32rpx" primaryColor="rgba(35, 113, 255, 1)" :disabled="isViewInfo" />
					</view>
				</uni-forms-item>
				<uni-forms-item label="任务单号" required name="planeNumber" id="planeNumber_form" labelFont="32rpx"
					contentFont="32rpx" :requireOpacity="0.5">
					<view id="planeNumber" class="form_li">
						<uni-easyinput placeholderStyle="color:#C1C1C1;font-size:32rpx" inputHeight="88rpx"
							:styles="styles" type="text" v-model="taskForm.planeNumber" placeholder="请输入任务单号"
							contentFontSize="32rpx" primaryColor="rgba(35, 113, 255, 1)" :disabled="true" />
					</view>
				</uni-forms-item>
				<uni-forms-item label="目标设备" required name="targetId" id="targetId_form" labelFont="32rpx"
					contentFont="32rpx" :requireOpacity="0.5">
					<!-- <view id="targetId" class="form_li">
						<uni-easyinput placeholderStyle="color:#C1C1C1;font-size:32rpx" inputHeight="88rpx"
							:styles="styles" type="text" v-model="taskForm.targetName" placeholder="请选择目标设备"
							contentFontSize="32rpx" primaryColor="rgba(35, 113, 255, 1)" :disabled="true" />
					</view> -->
					<view id="targetId" class="form_li" @click="toChoiceDevice('from')">
						<view class="form_input"
							:class="{'placeholder_input':!taskForm.targetName||taskForm.targetName=='','dis_li_input':isonlyDevPlane}">
							{{taskForm.targetName?taskForm.targetName:'请选择目标设备'}}
						</view>
						<view class="form_sel_icon">
							<custom-icons iconsName="icon-a-youjiantoubai" iconsSize="20rpx"
								iconsColor="#999999"></custom-icons>
						</view>
					</view>
				</uni-forms-item>
				<!-- <uni-forms-item label="任务备注" name="remark" id="remark_form" labelFont="32rpx" contentFont="32rpx"
					:requireOpacity="0.5">
					<view id="remark" class="form_li">
						<uni-easyinput placeholderStyle="color:#C1C1C1;font-size:32rpx" inputHeight="70rpx"
							:styles="styles" type="textarea" v-model="taskForm.remark" placeholder="请输入备注"
							contentFontSize="32rpx" primaryColor="rgba(35, 113, 255, 1)" autoHeight
							:disabled="isViewInfo" />
					</view>
				</uni-forms-item> -->
				<ProcessCompot ref="flowForm" @isChoiceFlowUserFun="isChoiceFlowUserFun" @setActiveItem="setActiveItem"
					:disabled="isViewInfo" :isNotPadding="true">
				</ProcessCompot>
				
			</view>
		</uni-forms>

		<view class="form_con page_form_con" v-if="!isViewInfo">
			<button class="submit_button" @click="submitForm(2)" :disabled="isLoading"
				:style="{'opacity':isLoading?0.6:1}" :loading="isLoading">
				提交
			</button>
		</view>
		<view class="zhanwei" style="width: 100%;height: 40rpx;">

		</view>
		<msg-prompt ref="promptMsg"></msg-prompt>
		<uni-popup ref="planedevSelect" :mask-click="false" background-color="#ffffff" :safe-area="true"
			mask-background-color="rgba(0, 0, 0, 0.5)" type="top" :zIndex="999">
			<view style="width: 100%;height: 100vh;position: relative;">
				<deviceSelect :isCustomList="true" ref="planedevSel" topTitle="选择设备" :isMulSelect="false" @closeSelect="closeSelect"
					@selectDevice="selectPlaneDevice" :limit="1" :planeId="planeId"></deviceSelect>
			</view>
		</uni-popup>
	</view>
</template>

<script>
	import {
		flowRootRecord
	} from '@/api/process.js'
	import {
		devPlaneInfo,
		devPlaneTaskNumber,
		taskFormData,
		devPlaneTaskAdd
	} from '@/api/devplane.js'
	import ProcessCompot from "@/pages_flow/flow-form/process-compot.vue";
	import {
		setPagesParam
	} from '@/common/utillib.js'
	import deviceSelect from "@/pages_flow/components/planedev-select/device-select.vue"
	export default {
		components: {
			ProcessCompot,
			deviceSelect
		},
		data() {
			return {
				allDeviceList: [],
				isViewInfo: false, //是否时查看信息
				isLoading: false,
				topTitle: '发起任务',
				taskForm: {
					planeNumber: '',
					planName: '',
					planTypeId: '',
					targetId: '',
					targetName: '',
					remark: ''
				},
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
				planeId: '', //计划id
				deviceId: '', //设备id
				deviceName: '', //设备名称
				taskFlowMode: {},
				styles: {
					color: '#333',
					backgroundColor: '#F8F8F8',
					disableColor: '#F8F8F8',
					borderColor: '#F8F8F8',
					disborderColor: 'rgba(234, 234, 234, 1)'
				},
				rules: {
					planeNumber: {
						rules: [{
							required: true,
							errorMessage: '请输入任务编号',
						}]
					},
					planName: {
						rules: [{
							required: true,
							errorMessage: '请输入计划名称',
						}]
					},
					targetId: {
						rules: [{
							required: true,
							errorMessage: '请选择任务目标设备',
						}]
					},
				},
				isonlyDevPlane:false
			}
		},
		onLoad() {
			const eventChannel = this.getOpenerEventChannel();
			// 监听acceptDataFromOpenerPage事件，获取上一页面通过eventChannel传送到当前页面的数据
			eventChannel.on('planeTaskForm', async (data) => {
				// console.log(data)
				if (data.id) {
					this.planeId = data.id
					await this.getPlaneInfo(this.planeId)
				}else{
					uni.navigateBack()
				}
				if (data.deviceId) {
					this.isonlyDevPlane=true
					this.deviceId = data.deviceId
					this.taskForm.targetId = this.deviceId
					await this.loadtaskFormData(this.deviceId, this.planeId)
				}
				if (data.deviceName) {
					this.deviceName = data.deviceName
					this.taskForm.targetName = this.deviceName
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
			toChoiceDevice() {
				if(this.isonlyDevPlane){
					return
				}
				let selected = this._value ? this._value : []
				this.$refs.planedevSelect.open()
				this.$nextTick(() => {
					this.$refs.planedevSel.setSelected(selected)
				})
			},
			closeSelect() {
				this.$refs.planedevSelect.close()
			},
			async selectPlaneDevice(val) {
				//选中设备后
				// console.log("设备", val);
				this.closeSelect()
				if (val&&val.length>0) {
					this.taskForm.targetId = val[0].Id
					this.taskForm.targetName = val[0].Name
					await this.loadtaskFormData(this.taskForm.targetId, this.planeId)
					this.$forceUpdate()
				}
			
			},
			async loadtaskFormData(devId, planeId) {
				try {
					let res = await taskFormData({
						devId: devId,
						planeId: planeId
					})
					this.taskFlowMode = JSON.parse(JSON.stringify(res.data))
					if (this.taskForm.flowId) {
						await this.$refs.flowForm.InitData(
							this.taskForm.flowId, {}, null, res.data
						);
					}
					// console.log("加载res", res);
				} catch (error) {
					this.setMsgTop(error)
				}
			},
			async getPlaneInfo(val) {
				try {
					let infores = await devPlaneInfo({
						id: val
					})
					let info = infores.data
					// console.log(info, 'info');
					this.topTitle = '当前计划类型：' + info.Name
					this.taskForm.planeNumber = await this.getTaskNumber()
					this.taskForm.planName = info.Name
					this.taskForm.flowId = info.FlowTemplateId
					this.taskForm.planTypeId = info.Id
					// this.allDeviceList=JSON.parse(JSON.stringify(info.DeviceList))
				} catch (error) {
					this.setMsgTop(error)
				}
			},
			async getTaskNumber() {
				try {
					let res = await devPlaneTaskNumber()
					return res.data
				} catch (error) {
					this.setMsgTop(error)
				}
			},
			submitForm() {
				this.$refs.taskForm.validate().then(valid => {
					this.$refs.flowForm.setTaskFlow((flowValue) => {
						if (valid) {
							this.isLoading = true
							let submitForm = {}
							submitForm.task = JSON.parse(JSON.stringify(this.taskForm))
							submitForm.model = JSON.parse(JSON.stringify(flowValue))
							devPlaneTaskAdd(submitForm).then(res => {
								// console.log("添加成功",res);
								this.$refs.promptMsg.open('发起任务成功', 1450)
								setTimeout(() => {
									setPagesParam('loadTaskList', 'load', 1)
									if (this.isLoading) {
										this.isLoading = false
									}
								}, 1500)

							}).catch(err => {
								this.setMsgTop(err)
								if (this.isLoading) {
									this.isLoading = false
								}
							})
						}
					})
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
			selectDevice(selarr) {
				//选择完设备
				// this.$refs.form.selectDept(val,this.activeItemId)
				this.isChoiceDevice = true
				// this.$refs.flowForm.selectDept(val, this.activeItemId)
				this.choiceDevice = selarr
			},
		}
	}
</script>

<style>

</style>