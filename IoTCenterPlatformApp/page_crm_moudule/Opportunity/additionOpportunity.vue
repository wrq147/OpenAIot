<template>
	<view id="CreateCustomer">
		<top leftIcon="icon-fanhui" :isleftBack="true" backgroundColor="#ffffff" title="添加商机"
			class="addPool-top"></top>
		<uni-forms ref="form" :modelValue="formData" label-position="top" class="addPool-form" label-width="100%"
			:rules="rules">
			<uni-forms-item label="  商机名称" name="opportName" required class="addPool-form-item">
				<input placeholder-class="PlaceStyle" type="text" v-model="formData.opportName"
					class="addPool-easyinput" placeholder="请输入商机名称">
			</uni-forms-item>
			<uni-forms-item label="   商机编号" name="opportNumber" required class="addPool-form-item">
				<input placeholder-class="PlaceStyle" disabled type="text" v-model="formData.opportNumber"
					class="addPool-easyinput"
					style="background: #F8F8F8!important;color:#999999;border:2rpx solid #EAEAEA;" placeholder="商机编号">
			</uni-forms-item>
			<uni-forms-item label="  客户名称" name="customerName" required class="addPool-form-item">
				<view class="Collaborator-item" @click="handCustomer" style="display: flex;">

					<view class="Collaborator-item-flex" v-if="formData.customerName">
						{{formData.customerName}}
					</view>
					<view v-else style="color:#C1C1C1;font-size:32rpx;">
						请选择客户名称
					</view>
					<view class="addPoolIcon iconfont icon-a-youjiantoubai">

					</view>
				</view>
			</uni-forms-item>
			<uni-forms-item label=" 客户编号" name="customerNumber" required class="addPool-form-item">
				<input placeholder-class="PlaceStyle" disabled type="text" v-model="formData.customerNumber"
					class="addPool-easyinput"
					style="background: #F8F8F8!important;color:#999999;border:2rpx solid #EAEAEA;" placeholder="客户编号">
			</uni-forms-item>
			<uni-forms-item label="  客户联系人" required name="contactId" class="addPool-form-item">
				<view class="Collaborator-item" @click="handContact" style="display: flex;">
					<view class="Collaborator-item-flex" v-if="formData.contactName">
						{{formData.contactName}}
					</view>
					<view v-else style="color:#C1C1C1;font-size:32rpx;">
						请选择客户联系人
					</view>
					<view class="addPoolIcon iconfont icon-a-youjiantoubai">

					</view>
				</view>
			</uni-forms-item>
			<uni-forms-item label="  销售阶段" name="period" required class="addPool-form-item">
				<uni-data-select :isDark="true" v-model="formData.period" :localdata="periodArr" type="line"
					placeholder=" 请选择销售阶段" class="addPool-selected" @change="handClickPeriod"></uni-data-select>
			</uni-forms-item>
			<uni-forms-item label="  赢率" name="probability" required class="addPool-form-item">
				<input placeholder-class="PlaceStyle" style="background:#f8f8f8!important;" type="text" disabled
					v-model="formData.probability" class="addPool-easyinput" placeholder="请输入预计成交几率">
			</uni-forms-item>
			<uni-forms-item label="  负责人" name="leaderId" required class="addPool-form-item">
				<view class="Collaborator-item" @click="handManager">
					<view class="Collaborator-item-flex">
						<view v-if="ManagerList.length>0">
							<view v-for="(item,index) in ManagerList" :key="index" style="margin-right:10rpx;">
								{{item.name}}
							</view>
						</view>
						<view v-else style="color:#C1C1C1;font-size:32rpx;">
							请输入负责人
						</view>
						<view class="addPoolIcon iconfont icon-a-youjiantoubai">

						</view>
					</view>
				</view>
			</uni-forms-item>
			<!-- 	<uni-forms-item label=" Company website" name="name"  class="addPool-form-item" style="color:red;">
				<input  type="text" class="addPool-easyinput" placeholder="Please enter company website">
			</uni-forms-item> -->

			<uni-forms-item label="协作人" name="helper" class="addPool-form-item">
				<view class="Collaborator-item" @click="handCollAvor">
					<view class="Collaborator-item-flex">
						<view v-if="CollXieData.length>0" style="display: flex;">
							<view v-for="(item,index) in CollXieData" :key="index" style="margin-right:10rpx;">
								{{item.name?item.name:item.RealName}}
							</view>
						</view>
						<view v-else style="color:#C1C1C1;font-size:32rpx;">
							请输入协作人
						</view>
						<view class="addPoolIcon iconfont icon-a-youjiantoubai">

						</view>
					</view>
				</view>
			</uni-forms-item>

			<uni-forms-item label="商机详情" name="remark" required class="addPool-form-item">
				<textarea placeholder-class="PlaceStyle" v-model="formData.remark" placeholder="请输入商机详情"
					class="addPool-textarea" />
			</uni-forms-item>

			<uni-forms-item label="商品" name="name" class="addPool-form-item">
				<view class="product_ul">
					<view class="product_li li_border" v-for="(row,inx) in ProductList" :key="inx">
						<view class="li_cont">
							<view class="cont_title">
								<view class="title_label" :class="{'normal':row.ProductLabel&&row.ProductLabel=='U'}">{{row.ProductLabel&&row.ProductLabel=='U'?'半成品':'成品'}}</view>
								<view class="title_name">
									<text>{{row.ProductName}}</text>
									<text v-if="row.TypeName" class="line">|</text>
									<text v-if="row.TypeName">{{row.TypeName}}</text>
								</view>
							</view>
							<view class="cont_number" v-if="row.SkuNumber">产品编码：{{row.SkuNumber}}</view>
							
							<view class="price_quantity" v-if="row.ProductLabel=='U'">
								<view class="input_li mar_input">
									<text class="label">价格:</text>
									<uni-number-box background="#F8F8F8" v-model="row.Price" color="#333333"
										:max="1000000000000" />
								</view>
								<view class="input_li">
									<text class="label">数量:</text>
									<uni-number-box background="#F8F8F8" v-model="row.Quantity" color="#333333"
										:min="1" :max="1000000000000"/>
								</view>
							</view>
							<view class="price_quantity" v-else>
								<view class="input_li mar_input">
									<text class="label">价格:</text>
									<uni-number-box background="#F8F8F8" v-model="row.Price" color="#333333"
										:max="1000000000000" />
								</view>
								<view class="input_li">
									<text class="label">数量:</text>
									<uni-number-box background="#F8F8F8" v-model="row.Quantity" color="#333333"
										:min="1" :max="1"/>
								</view>
							</view>
						</view>
						<view class="del_icon" @click.stop="handDel(inx)">
							<view class="icons_del t-icon-yichu1"></view>
						</view>
					</view>
				</view>
				<view class="addItem" @click="handTransfer">
					<view class="iconfont icon-tianjia"> <text class="addItem-text">添加物品</text></view>
				</view>
			</uni-forms-item>
		</uni-forms>


		<button v-if="EditId" :style="{'opacity':isSubmit?0.6:1,'margin-top': '60rpx'}" :disabled="isSubmit"
			:loading="isSubmit" class="addPool-button" @click="handEditForm">
			保存
		</button>
		<button v-else :style="{'opacity':isSubmit?0.6:1,'margin-top': '60rpx'}" :disabled="isSubmit"
			:loading="isSubmit" class="addPool-button" @click="handSubmitHigh">
			保存
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
							errorMessage: '负责人不能为空'
						}]
					},
					remark: {
						rules: [{
							required: true,
							errorMessage: '商机详情不能为空'
						}]
					},
					customerNumber: {
						rules: [{
							required: true,
							errorMessage: '客户编号不能为空'
						}]
					},
					opportNumber: {
						rules: [{
							required: true,
							errorMessage: '商机编号不能为空'
						}]
					},
					opportName: {
						rules: [{
							required: true,
							errorMessage: '商机名称不能为空'
						}]
					},
					probability: {
						rules: [{
							required: true,
							errorMessage: '成交几率不能为空'
						}]
					},
					period: {
						rules: [{
							required: true,
							errorMessage: '销售阶段不能为空'
						}]
					},
					customerName: {
						rules: [{
							required: true,
							errorMessage: '客户名称不能为空'
						}]
					},
					contactId: {
						rules: [{
							required: true,
							errorMessage: '联系人不能为空'
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
			handDel(inx) {
				this.ProductList.splice(inx, 1); // 删除元素
				this.formData.detailList.splice(inx, 1);
			},
			
			finishSelectProduct(data) {
				//console.log(data,'finishSelectItems')
				let dataTarget = []
				dataTarget=data.map((item, index) => {
					// dataTarget.push({
					// 	productId: item.TargetId,
					// 	HouseId: item.HouseId ? item.HouseId : '',
					// 	name: item.Name,
					// 	price: item.Price,
					// 	productType: item.TargetType,
					// 	quantity: item.Quantity
					// })
					let obj = {
						label: item.ProductLabel == "U" ? "半成品" : "成品",
						productId: item.Id,
						quantity: 1,
						price: item.Price,
						opportId: this.EditOpportId,
						name: item.ProductName,
						number: item.SkuNumber
					}
					return obj
				})
				this.ProductList = JSON.parse(JSON.stringify(data));
				this.formData.detailList = JSON.parse(JSON.stringify(dataTarget))
			},
			handTransfer() {
				if (this.ProductList && this.ProductList.length > 0) {
					let arrId = []
					arrId = JSON.parse(JSON.stringify(this.ProductList))
					uni.navigateTo({
						url: '/pages_factory/product_select?isSelect=true&isMulSelect=true&selectId=' + JSON
							.stringify(arrId)
					})
				} else {
					this.ProductList = []
					this.formData.detailList = []
					uni.navigateTo({
						url: '/pages_factory/product_select?isSelect=true&isMulSelect=true'
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
								title: '修改成功！',
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
						//console.log(data,'详情！！！')
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
							contactName: data.ContactUser.RealName, //联系人名称（不传）
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
						console.log(data.DetailList,'data.DetailList')
						data.DetailList.map((row, index) => {
							let obj1 = {
								label: row.ProductInfo.ProductLabel == "U" ? "半成品" : "成品",
								productId: row.ProductId,
								quantity: row.Quantity,
								price: row.Price,
								opportId: data.Id,
								name: row.ProductInfo.ProductName,
								number: row.ProductInfo.SkuNumber
							}
							partInfoArr.push(obj1)
							deviceInfoArr.push(row.ProductInfo)
						})
						this.ProductList = deviceInfoArr


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
								title: '新建成功！',
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
					url: '/pages_flow/inventory/employee_select?multiple=true'
				})
			},
			handManager() {
				this.selectText = 'Manager'
				uni.navigateTo({
					url: '/pages_flow/inventory/employee_select?type=user'
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
<style>
	page {
		background: #ffffff;
	}
</style>
<style lang="less" scoped>


	.alert-proup {
		width: 90vw;
		color: #333333;
		padding: 20rpx;
		text-align: center;


		.alert-proup-title {
			margin-bottom: 20rpx;
		}

		.alert_par {}
	}

	.Collaborator-item-flex {
		color: #333;
		display: flex;
	}

	.Collaborator-item {
		height: 90rpx;
		background: #f8f8f8;
		border-radius: 8rpx;
		line-height: 90rpx;
		padding: 0rpx 20rpx;
		font-size: 32rpx;
	}


	.addItem {
		color: #333333;
		height: 88rpx;
		line-height: 88rpx;
		text-align: center;
		background: #f8f8f8;
		margin-top: 30rpx;

		.icon-tianjia {
			font-size: 26rpx;

			.addItem-text {
				margin-left: 10rpx;
				font-size: 32rpx;
			}
		}
	}

	.productList {
		position: relative;
		background: #fff;
		height: 126rpx;
		border-radius: 10rpx;
		display: flex;
		align-items: center;
		color: #333;
		padding: 0rpx 25rpx;
		margin-top: 30rpx;
		border: 1rpx solid rgba(234, 234, 234, 1);

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
			background: rgba(000, 000, 000, .2);

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