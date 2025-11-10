<template>
	<view style="height: 100%">
		<top :title="topTitle" leftText="Back" leftWidth="157rpx" :isleftBack="true" leftIcon="icon-fanhui"
			backgroundColor="#161A26" rightWidth="157rpx">
		</top>
		<uni-forms ref="warehouseForm" :modelValue="warehouseForm" :rules="rules" labelWidth='80' label-position="top">
			<view class="form_con page_form_con">
				<uni-forms-item label="Warehouse name" required name="StoreName" id="StoreName_form" labelFont="32rpx"
					contentFont="32rpx" :requireOpacity="0.5">
					<view id="StoreName" class="form_li">
						<uni-easyinput placeholderStyle="color:rgba(255, 255, 255, 0.2);font-size:32rpx"
							inputHeight="88rpx" :styles="styles" type="text" v-model="warehouseForm.StoreName"
							placeholder="Please enter warehouse name" contentFontSize="32rpx"
							primaryColor="rgba(255, 255, 255, 0.5)" />
					</view>
				</uni-forms-item>
				<uni-forms-item label="Manager" name="LeaderId" id="LeaderId_form" labelFont="32rpx" contentFont="32rpx"
					:requireOpacity="0.5">
					<view id="LeaderId" class="form_li" @click="choiceManage">
						<!-- <uni-easyinput placeholderStyle="color:rgba(255, 255, 255, 0.2);font-size:32rpx"
							inputHeight="88rpx" :styles="styles" type="text" v-model="warehouseForm.LeaderName"
							placeholder="Please select a manager" contentFontSize="32rpx"
							primaryColor="rgba(255, 255, 255, 0.5)" :clearable="false" /> -->
						<view class="form_input"
							:class="{'placeholder_input':!warehouseForm.LeaderName||warehouseForm.LeaderName==''}">
							{{warehouseForm.LeaderName?warehouseForm.LeaderName:'Please select a manager'}}
						</view>
						<view class="form_sel_icon">
							<custom-icons iconsName="icon-a-youjiantoubai" iconsSize="20rpx"
								iconsColor="rgba(255, 255, 255, 0.5)"></custom-icons>
						</view>
					</view>
				</uni-forms-item>
				<uni-forms-item label="Status" name="Status" id="Status_form" labelFont="32rpx" contentFont="32rpx"
					:requireOpacity="0.5" v-if="warehouseForm.Id!=undefined">
					<view id="Status" class="form_li">
						<view class="sel_label" style="margin-right: 20rpx;" @click.stop="setEnableDis('1')"
							:class="{'active':warehouseForm.Status=='1'}">
							Enable
						</view>
						<view class="sel_label" @click.stop="setEnableDis('0')"
							:class="{'active':warehouseForm.Status=='0'}">
							Disable
						</view>
					</view>
				</uni-forms-item>
				<uni-forms-item label="Notes" name="Remark" id="Remark_form" labelFont="32rpx" contentFont="32rpx"
					:requireOpacity="0.5">
					<view id="Remark" class="form_li">
						<uni-easyinput placeholderStyle="color:rgba(255, 255, 255, 0.2);font-size:32rpx"
							inputHeight="70rpx" :styles="styles" type="textarea" v-model="warehouseForm.Remark"
							placeholder="Please enter the notes" contentFontSize="32rpx"
							primaryColor="rgba(255, 255, 255, 0.5)" autoHeight />
					</view>
				</uni-forms-item>
				<button class="submit_button" @click="submit" :disabled="isLoading"
					:style="{'opacity':isLoading?0.6:1,'margin-top': '60rpx'}" :loading="isLoading"
					v-if="warehouseForm.Id==undefined">
					Save
				</button>
				<view class="btn_con" v-else>
					<button class="jump_button" @click="deleteHouse" :disabled="isLoading"
						:style="{'opacity':isLoading?0.6:1}" :loading="isLoading">
						Delete
					</button>
					<button class="submit_button" @click="submit" :disabled="isLoading"
						:style="{'opacity':isLoading?0.6:1}" :loading="isLoading">
						Save
					</button>
				</view>
			</view>

		</uni-forms>
		<msg-prompt ref="promptMsg" @confirm="confirmDelete"></msg-prompt>
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
	export default {
		data() {
			return {
				topTitle: 'Create warehouse',
				styles: {
					color: '#fff',
					backgroundColor: '#161A26',
					disableColor: 'rgba(28, 34, 50, 1)',
					borderColor: 'rgba(255, 255, 255, 0.20)'
				},
				rules: {
					StoreName: {
						rules: [{
							required: true,
							errorMessage: 'Please enter warehouse name',
						}]
					}
				},
				warehouseForm: {
					Status: '1'
				},
				isLoading: false,
				isDelete: false
			};
		},
		async onLoad(options) {
			if (options.houseId) {
				this.warehouseForm.Id = options.houseId
				this.topTitle = 'Edit Warehouse'
				await this.getHouseInfo()
			}
		},
		methods: {
			setEnableDis(val) {
				//设置停用启用的值
				if (this.warehouseForm.Status != val) {
					this.isDelete = false
					this.warehouseForm.Status = val
					if (this.warehouseForm.Status == '0') {
						this.$refs.promptMsg.noticeOpen(
							'Are you sure to deactivate ' + this.warehouseForm.StoreName + '?', 'prompt', true
						)
					} else {
						this.$refs.promptMsg.noticeOpen(
							'Are you sure to enable ' + this.warehouseForm.StoreName + '?', 'prompt', true
						)
					}
				}
			},
			deleteHouse() {
				//删除仓库
				this.isDelete = true
				this.$refs.promptMsg.noticeOpen(
					'Are you sure to delete the data item with the name ' + this.warehouseForm.StoreName + '?',
					'prompt', true
				)
			},
			confirmDelete() {
				//确认删除
				if (this.isDelete) {
					this.isLoading = true
					delHouse(this.warehouseForm.Id).then(res => {
						console.log("仓库删除", res);
						this.$refs.promptMsg.open('Delete successful', 1500)
						setTimeout(() => {
							setPagesParam('getHouseList', 'load', 1)
						}, 1500)
					}).catch(err => {
						this.isLoading = false
						this.setMsgTop(err)
					})
				} else {
					if (this.warehouseForm.Status == '0') {
						this.$refs.promptMsg.loadingOpen('Deactivating...')
					} else {
						this.$refs.promptMsg.loadingOpen('Enabling...')
					}
					changeStatus(this.warehouseForm.Id, this.warehouseForm.Status).then(res => {
						this.$refs.promptMsg.loadingColse()
						if (this.warehouseForm.Status == '0') {
							this.$refs.promptMsg.open('Warehouse has been deactivated', 1500)
							setPagesParam('getHouseList', 'load', 1, false)
						}
						if (this.warehouseForm.Status == '1') {
							this.$refs.promptMsg.open('Warehouse enabled', 1500)
							setPagesParam('getHouseList', 'load', 1, false)
						}
					}).catch(err => {
						this.$refs.promptMsg.loadingColse()
						this.setMsgTop(err)
					})
				}

			},
			async getHouseInfo() {
				//获取仓库信息
				try {
					let res = await houseInfo(this.warehouseForm.Id)
					console.log("仓库信息", res);
					this.warehouseForm = {
						Id: res.data.Id,
						LeaderId: res.data.LeaderId,
						LeaderName: res.data.LeaderName,
						Remark: res.data.Remark,
						StoreName: res.data.StoreName,
						Status: res.data.Status
					}
					if (this.warehouseForm.LeaderId) {
						let userRsp = await getUser(this.warehouseForm.LeaderId)
						this.warehouseForm.LeaderName = userRsp.data.user.RealName;
						this.warehouseForm.Avatar = userRsp.data.user.avatar
					}
				} catch (e) {
					//TODO handle the exception
					this.setMsgTop(e)
				}
			},
			submit() {
				//保存
				this.$refs.warehouseForm.validate().then(res => {
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
					this.$refs.promptMsg.open('Added successfully', 1500)
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
					this.$refs.promptMsg.open('Modified successfully', 1500)
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
				console.log("负责人", val);
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
					url: '/pages_Inventory/employee_select?selected=' + selected
				})
			}
		}
	}
</script>

<style lang="less">
	// .submit_button {
	// 	margin-top: 60rpx;
	// }
</style>