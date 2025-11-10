<template>
	<view id="CreateCustomer">
		<top leftIcon="icon-fanhui" :isleftBack="true" leftText="Back" backgroundColor="#161A26"
			title="Create opportunity" class="addPool-top"></top>
		<uni-forms ref="form" :modelValue="formData" label-position="top" class="addPool-form" label-width="100%"
			:rules="rules">
			<uni-forms-item label=" Opportunity name" name="opportName" required class="addPool-form-item">
				<input placeholder-class="PlaceStyle" type="text" v-model="formData.opportName"
					class="addPool-easyinput" placeholder="Please enter opportunity name">
			</uni-forms-item>
			<uni-forms-item label="  Opportunity number" name="opportNumber" required class="addPool-form-item">
				<input placeholder-class="PlaceStyle" disabled type="text" v-model="formData.opportNumber"
					class="addPool-easyinput" style="background: #1C2232!important;color:rgba(255,255,255,.5);"
					placeholder="KF2309070000017">
			</uni-forms-item>
			<uni-forms-item label=" Customer name" name="customerName" required class="addPool-form-item">
				<view class="Collaborator-item" @click="handCustomer" style="display: flex;">
					<view v-if="formData.customerName">
						<view class="Collaborator-item-flex">
							{{formData.customerName}}
						</view>
					</view>
					<text v-else style="color:rgba(255, 255, 255, .2);font-size:32rpx;">
						Please select a Customer name
					</text>
					<view class="addPoolIcon iconfont icon-a-youjiantoubai">

					</view>
				</view>
			</uni-forms-item>
			<uni-forms-item label=" Customer number" name="customerNumber" required class="addPool-form-item">
				<input placeholder-class="PlaceStyle" disabled type="text" placeholder="Please select a Customer number"
					v-model="formData.customerNumber" class="addPool-easyinput" style="background: #1C2232!important">
			</uni-forms-item>
			<uni-forms-item label=" Contact" required name="contactId" class="addPool-form-item">
				<view class="Collaborator-item" @click="handContact" style="display: flex;">
					<view v-if="formData.contactName">
						<view class="Collaborator-item-flex">
							{{formData.contactName}}
						</view>
					</view>
					<text v-else style="color:rgba(255, 255, 255, .2);font-size:32rpx;">
						Please select a Contact
					</text>
					<view class="addPoolIcon iconfont icon-a-youjiantoubai">

					</view>
				</view>
			</uni-forms-item>
			<uni-forms-item label=" Sale stage" name="period" required class="addPool-form-item">
				<uni-data-select :isDark="true" v-model="formData.period" :localdata="periodArr" type="line"
					placeholder="Please select a sale stage" class="addPool-selected"
					@change="handClickPeriod"></uni-data-select>
			</uni-forms-item>
			<uni-forms-item label=" Win rate" name="probability" required class="addPool-form-item">
				<input placeholder-class="PlaceStyle" style="background:rgb(28, 34, 50) !important;" type="text"
					disabled v-model="formData.probability" class="addPool-easyinput"
					placeholder="Please enter win rate">
			</uni-forms-item>
			<uni-forms-item label=" Manager" name="leaderId" required class="addPool-form-item">
				<view class="Collaborator-item" @click="handManager">
					<view class="Collaborator-item-flex">
						<view v-if="ManagerList.length>0">
							<view v-for="(item,index) in ManagerList" :key="index" style="margin-right:10rpx;">
								{{item.name}}
							</view>
						</view>
						<text v-else style="color:rgba(255, 255, 255, .2);font-size:32rpx;">
							Please select a Manager
						</text>
						<view class="addPoolIcon iconfont icon-a-youjiantoubai">

						</view>
					</view>
				</view>
			</uni-forms-item>
			<!-- 	<uni-forms-item label=" Company website" name="name"  class="addPool-form-item" style="color:red;">
				<input  type="text" class="addPool-easyinput" placeholder="Please enter company website">
			</uni-forms-item> -->

			<uni-forms-item label="Collaborator" name="helper" class="addPool-form-item">
				<view class="Collaborator-item" @click="handCollAvor">
					<view class="Collaborator-item-flex">
						<view v-if="CollXieData.length>0" style="display: flex;">
							<view v-for="(item,index) in CollXieData" :key="index" style="margin-right:10rpx;">
								{{item.name?item.name:item.RealName}}
							</view>
						</view>
						<text v-else style="color:rgba(255, 255, 255, .2);font-size:32rpx;">
							Please select a Collaborator
						</text>
						<view class="addPoolIcon iconfont icon-a-youjiantoubai">

						</view>
					</view>
				</view>
			</uni-forms-item>

			<uni-forms-item label=" Opportunity details" name="remark" required class="addPool-form-item">
				<textarea placeholder-class="PlaceStyle" v-model="formData.remark"
					placeholder="Please enter the details" class="addPool-textarea" />
			</uni-forms-item>
			<uni-forms-item label="商品" name="name" class="addPool-form-item">
				<view class="productList" v-for="(item,index) in ProductList" :key="index">
					<img class="productList-logo" :src="item.PhotoUrl" alt="">
					<view class="productList-content">
						<view class="title">{{item.Name}}</view>
						<view class="content">{{item.DeviceNumber}}</view>
					</view>
					<view class="alert-parse-bgColor" @click="handClose(index)">
						<view class="alert-choose iconfont icon-guanbidanchuang">
						</view>
					</view>
				</view>
				<view class="addItem" @click="handTransfer">
					<view class="iconfont icon-tianjia"> <text class="addItem-text">Add item</text></view>
				</view>
			</uni-forms-item>

			<!-- <uni-forms-item label="Items" name="name" class="addPool-form-item">
				<view class="addItem" @click="handTransfer">
					<view class="iconfont icon-tianjia"> <text class="addItem-text">Add item</text></view>
				</view>
			</uni-forms-item> -->
		</uni-forms>


		<button v-if="EditId" :disabled="isSubmit" :style="{'opacity':isSubmit?0.6:1,'margin-top': '60rpx'}"
			class="addPool-button" :loading="isSubmit" @click="handEditForm">
			Save
		</button>
		<button v-else :disabled="isSubmit" :style="{'opacity':isSubmit?0.6:1,'margin-top': '60rpx'}"
			class="addPool-button" :loading="isSubmit" @click="handSubmitHigh">
			Save
		</button>
		<msg-prompt ref="promptMsg"></msg-prompt>
	</view>
</template>

<script>
	import {
		periodList, //销售阶段
		periodNumber, //商机编号
		OpportunityAdd, //商机新增
		OpportunityInfo, //商机详情
		CustomerList, //客户列表
		contactData, //联系人列表
		OpportunityEdit, //商机编辑
		opportunityDetails, //商机明细
	} from "@/api/crmApi";
	import {
		setPagesParam
	} from '@/common/utillib.js'
	export default {
		data() {
			return {
				isSubmit: false,
				loading: false,
				selectedIndexs: [],
				ProductList: [],
				tableData: [],
				formData: {
					opportNumber: "", //商机编码
					opportName: "", //商机名称
					customerId: "", //客户编码
					customerName: "", //客户编码（不传）
					customerNumber: "", //客户编码（不传）
					contactId: "", //联系人id
					contactName: "", //联系人名称（不传）
					probability: null, //预计成交几率
					period: "", //销售阶段
					helperName: '', //
					helper: "",
					remark: "", //线索详情
					leaderId: '', //负责人
					detailList: [], //商品明细
				},
				periodArr: [],
				selectText: '',
				CollXieData: [],
				ManagerList: [],

				rules: {
					leaderId: {
						rules: [{
							required: true,
							errorMessage: 'The business leaderId name cannot be empty'
						}]
					},
					remark: {
						rules: [{
							required: true,
							errorMessage: 'The business remark name cannot be empty'
						}]
					},
					customerNumber: {
						rules: [{
							required: true,
							errorMessage: 'The business customerNumber name cannot be empty'
						}]
					},
					opportNumber: {
						rules: [{
							required: true,
							errorMessage: 'The business opportNumber name cannot be empty'
						}]
					},
					opportName: {
						rules: [{
							required: true,
							errorMessage: 'The business opportunity name cannot be empty'
						}]
					},
					probability: {
						rules: [{
							required: true,
							errorMessage: 'Expected transaction probability cannot be empty'
						}]
					},
					period: {
						rules: [{
							required: true,
							errorMessage: 'The sales stage cannot be empty'
						}]
					},
					customerName: {
						rules: [{
							required: true,
							errorMessage: 'Customer name cannot be empty'
						}]
					},
					leaderId: {
						rules: [{
							required: true,
							errorMessage: 'The person in charge cannot be empty'
						}]
					},
					contactId: {
						rules: [{
							required: true,
							errorMessage: 'Contact person cannot be empty'
						}]
					},

				},
				EditId: '',
				
			}
		},
		onLoad(option) {
			this.list(); //预加载数据
			if (option.id) {
				this.detailsData(option.id);
				this.EditId = option.id
			} else {
				this.shangNum();
			}
		},
		methods: {
			handClose(inx) {
				this.ProductList.splice(inx, 1); // 删除元素
				this.formData.detailList.splice(inx, 1);
			},
			handDetarMine() {
				if (this.selectedIndexs.length > 0) {
					this.selectedIndexs.sort(function(a, b) {
						return a - b
					})
					//console.log(this.selectedIndexs,'this.selectedIndexs')
					var arrTese = []
					this.selectedIndexs.forEach((item, index) => {
						this.ProductList.push(this.tableData[item])
						arrTese.push({
							productId: this.tableData[item].TargetId,
							HouseId: this.tableData[item].HouseId,
							name: this.tableData[item].Name,
							price: this.tableData[item].Price,
							productType: this.tableData[item].TargetType,
							quantity: this.tableData[item].Quantity,
						})
					})
				} else {
					this.ProductList = []
				}
				this.formData.detailList = arrTese
				console.log(this.ProductList.length, '长度')
				this.$refs.activePopup.close()
			},
			// 多选处理
			selectedItems() {
				return this.selectedIndexs.map(i => this.tableData[i])
			},
			// 多选
			selectionChange(e) {
				//console.log(e.detail.index)
				this.selectedIndexs = e.detail.index

			},
			handChose() {
				this.$refs.activePopup.close()
			},
			finishSelectItems(data) {
				//console.log(data,'finishSelectItems')
				let dataTarget = []
				data.forEach((item, index) => {
					dataTarget.push({
						productId: item.TargetId,
						HouseId: item.HouseId?item.HouseId:'',
						name: item.Name,
						price: item.Price,
						productType: item.TargetType,
						quantity: item.Quantity
					})
				})
				this.ProductList = data;
				this.formData.detailList = dataTarget
			},
			handTransfer() {
				
				if (this.ProductList && this.ProductList.length > 0) {
					let arrId = []
					this.ProductList.map((item, index) => {
						//console.log(item)
						arrId.push(item)
					})
					uni.navigateTo({
						url: '/pages_Inventory/inventory_list?isSelect=true&isMulSelect=true&selectId=' + JSON
							.stringify(arrId)
					})
				} else {
					this.ProductList = []
					this.formData.detailList = []
					uni.navigateTo({
						url: '/pages_Inventory/inventory_list?isSelect=true&isMulSelect=true'
					})
				}
			
			},
			handEditForm() {
				if (!this.isSubmit) {
					this.isSubmit = true;
					setTimeout(() => {
						this.isSubmit = false;
					}, 2000); // 设置 2 秒后可再次点击
				}
				this.$refs.form.validate().then((res) => {
					var helperName = []
					var helper = []
					this.CollXieData.forEach((row) => {
						helperName.push(row.name)
						helper.push(row.id)
					})
					this.formData.helperName = helperName.join(',');
					this.formData.id = this.EditId
					this.formData.helper = helper.join(',');
					if (this.ManagerList.length == 1) {
						this.formData.leaderId = this.ManagerList[0].id
					}
					// console.log(this.formData,'这里是编辑')
					// return
					OpportunityEdit(this.formData).then((data) => {
						if (data.code == 0) {
							uni.showToast({
								title: 'Modified successfully！',
								icon: 'none'
							})
							setTimeout(() => {
								setPagesParam('list', this.EditId)
							}, 1000)
						}
					}).catch((err) => {
						this.setMsgTop(err)
					})
				})
			},
			detailsData(id) {
				OpportunityInfo({
					id: id
				}).then((res) => {
					if (res.code == 0) {
						var data = res.data;
						//this.CollXieData=result.HelperUsers
						if (data.HelperUsers) {
							data.HelperUsers.forEach((ite, inx) => {
								this.CollXieData.push({
									id: ite.Id,
									name: ite.RealName
								})
							})
						}
						
						if (data.LeaderId) {
							//获取责任人相关信息
							this.ManagerList = [{
								id: data.LeaderUser.Id,
								name: data.LeaderUser.RealName,
								avatar: data.LeaderUser.Avatar,
								type: "user",
							}]
						}
								
								this.formData = {
									contactName: data.ContactUser.RealName,//联系人名称（不传）
									customerName: data.CustomerName,
									 customerNumber: data.CustomerNumber,
									opportNumber: data.OpportNumber, //商机编码
									opportName: data.OpportName, //商机名称
									customerId: data.CustomerId, //客户编码
									contactId: data.ContactId, //联系人id
									probability: data.Probability, //预计成交几率
									period: data.Period, //销售阶段
									helper: data.Helper,
									remark: data.Remark, //线索详情
									leaderId: data.LeaderId, //负责人
									id: data.Id,

								};
								
								
								
								let partInfoArr = []
								let deviceInfoArr = []
								//console.log(data.DetailList,'data.DetailList')
								data.DetailList.forEach((row, index) => {
									let obj1 = {
										productType: row.ProductType,
										productId: row.ProductId,
										opportId: data.Id,
										HouseId: row.HouseId,
										quantity: row.Quantity,
										price: row.Price,
										name: row.ProductType == 0 ? row.PartInfo
											.Name : row.DeviceInfo.Name
									}
									let obj2 = {}
									if (row.PartInfo) {
										//partInfoArr.push(item.PartInfo)
										//this.ProductList.push(row)
										obj2 = {
											DeviceNumber: row.PartInfo.DeviceNumber,
											Name: row.PartInfo.Name,
											PhotoUrl: row.PartInfo.PhotoUrl,
											//  Quantity: 1,
											TargetId: row.PartInfo.Id,
											TargetType: 0,
											Unit: row.PartInfo.Unit,
										}
								
									}
									if (row.DeviceInfo) {
										//deviceInfoArr.push(item.DeviceInfo)
										// this.ProductList.push(row)
										obj2 = {
											DeviceNumber: row.DeviceInfo.DeviceNumber,
											Name: row.DeviceInfo.Name,
											PhotoUrl: row.DeviceInfo.PhotoUrl,
											//  Quantity: 1,
											TargetId: row.DeviceInfo.Id,
											TargetType: 1,
											Unit: row.DeviceInfo.Unit,
										}
									}
									partInfoArr.push(obj1)
									deviceInfoArr.push(obj2)
								})
								this.ProductList = deviceInfoArr
								//console.log( partInfoArr,deviceInfoArr,'111111');
								
								this.formData.detailList = partInfoArr
							


						
						//console.log(this.formData,'this.formData')
					}
				})
			},
			handClickPeriod(e) {
				for (let i = 0; i < this.periodArr.length; i++) {
					if (this.periodArr[i].value === e) {
						this.formData.probability = this.periodArr[i].Probability;
						break;
					}
				}
			},
			shangNum() {
				periodNumber().then((res) => {
					//console.log(res,'商机编号');
					this.formData.opportNumber = res.data
				})
			},
			handSubmitHigh() {
				// 部分表单进行校验，接受一个参数，类型为 String 或 Array ，只校验传入 name 表单域的值
				if (!this.isSubmit) {
					this.isSubmit = true;
					setTimeout(() => {
						this.isSubmit = false;
					}, 2000); // 设置 2 秒后可再次点击
				}
				if (this.ManagerList.length == 1) {
					this.formData.leaderId = this.ManagerList[0].id
					this.formData.LeaderName = this.ManagerList[0].name
				}
				this.$refs.form.validate().then((res) => {
					var helperName = []
					var helper = []
					this.CollXieData.forEach((row) => {
						helperName.push(row.name)
						helper.push(row.id)
					})
					this.formData.helperName = helperName.join(',');
					this.formData.helper = helper.join(',');

					// console.log(this.formData,'this.formData')
					// return
					OpportunityAdd(this.formData).then((data) => {
						if (data.code == 0) {
							uni.showToast({
								title: 'New successfully added！',
								icon: 'none'
							})
							setTimeout(() => {
								setPagesParam('list')
							}, 1000)
						}
					}).catch((err) => {
						this.setMsgTop(err)
					})
				})

			},
			selectEmplee(data) {
				//console.log(data,'data');
				if (this.selectText == 'Collaborator') {
					this.CollXieData = data;
				} else {
					//console.log('负责人数据')
					this.ManagerList = data;
				}
			},
			handCollAvor() {
				this.selectText = 'Collaborator'
				uni.navigateTo({
					url: '/pages_Inventory/employee_select?multiple=true'
				})
			},
			handManager() {
				this.selectText = 'Manager'
				uni.navigateTo({
					url: '/pages_Inventory/employee_select?type=user'
				})
			},
			list() {
				periodList().then((res) => {
					//console.log(res,'销售阶段')
					res.data.forEach((row) => {
						this.periodArr.push({
							value: row.Id,
							text: row.PeriodName,
							Probability: row.Probability
						})
					})
				})
			},
			ContactData(data) {
				//console.log(data,'11')
				this.formData.contactId = data.Id
				this.formData.contactName = data.RealName


			},
			customData(data) {
				//console.log(data)
				this.formData.customerName = data.CustomerName
				this.formData.customerId = data.Id
				this.formData.customerNumber = data.CustomerNumber
			},
			handContact() {
				uni.navigateTo({
					url: '../contacts/contacts?ContactsAll=true'
				})
			},
			handCustomer() {
				uni.navigateTo({
					url: '../Customer/Customer?CustomerAll=true'
				})
			},
		}
	}
</script>

<style lang="less" scoped>
	.uni-container {
		/deep/.table--stripe[data-v-6cd49106] .uni-table-tr {
			background: none !important;
		}

		/deep/.uni-table-td {
			color: #fff !important;
		}

		/deep/.uni-table {
			background: none !important;
		}

		/deep/.uni-table[data-v-6cd49106] .uni-table-tr:hover {
			background: none !important;
		}

	}

	page {
		background: #161A26;

		/deep/.uni-select__input-text {
			font-size: 32rpx;
		}

		/deep/.uni-forms-item__label {
			font-size: 32rpx !important;
		}

		/deep/.uni-textarea-textarea {
			font-size: 32rpx;
		}

		.alert-proup {
			width: 90vw;
			color: #fff;
			padding: 20rpx;
			text-align: center;


			.alert-proup-title {
				margin-bottom: 20rpx;
			}

			.alert_par {}
		}

		.Collaborator-item-flex {
			color: #fff;
			display: flex;
			font-size: 32rpx;
		}

		.Collaborator-item {
			color: rgba(255, 255, 255, .4);
			height: 90rpx;
			border: 1rpx solid rgba(255, 255, 255, .2);
			border-radius: 8rpx;
			line-height: 90rpx;
			padding: 0rpx 20rpx;
		}
	}

	.addItem {
		color: rgba(255, 255, 255, .4);
		height: 88rpx;
		line-height: 88rpx;
		text-align: center;
		background: #1C2232;

		.icon-tianjia {
			font-size: 26rpx;

			.addItem-text {
				margin-left: 10rpx;
				font-size: 32rpx;
			}
		}
	}

	.addPoolIcon {
		font-size: .6rem;
		margin-left: auto;
		color: rgb(153, 153, 153);
	}
	.productList {
		position: relative;
		background: #1C2232;
		height: 126rpx;
		border-radius: 10rpx;
		display: flex;
		align-items: center;
		color: #fff;
		padding: 0rpx 25rpx;
		margin-top: 30rpx;
		border: 1rpx solid rgba(234, 234, 234, .2);
		margin-bottom: 30rpx;
		.alert-parse-bgColor {
			position: absolute;
			right: -15rpx;
			top: -15rpx;
			width: 40rpx;
			height: 40rpx;
			line-height: 43rpx;
			margin: 0 auto;
			text-align: center;
			border-radius: 50%;
			background: rgba(000, 000, 000, .6);
	
			.alert-choose {
				color: #fff;
				font-size: 25rpx;
			}
		}
	
		.productList-content {
			.title {
				font-size: 28rpx;
				font-weight: bold;
			}
	
			.content {
				font-size: 24rpx;
				color: #999999;
			}
		}
	
		.Mechines {
			margin-left: auto;
			color: #999999;
			font-size: 24rpx;
		}
	
		.t-icon-yichu {
			position: absolute;
			right: -15rpx;
			top: -20rpx;
			width: 50rpx;
			height: 50rpx;
		}
	
		.productList-logo {
			width: 65rpx;
			height: 65rpx;
			margin-right: 30rpx;
		}
	}
</style>