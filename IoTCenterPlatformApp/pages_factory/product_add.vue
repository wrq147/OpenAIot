<template>
	<view style="min-height: 100vh;">
		<top :title="topTitle" leftWidth="120rpx" rightWidth="120rpx" :isleftBack="true" leftIcon="icon-fanhui"
			backgroundColor="#ffffff">
		</top>
		<uni-forms ref="formData" :modelValue="formData" :rules="rules" labelWidth='80' label-position="top">
			<view class="form_con page_form_con">
				<view class="form_group_title">
					<view class="group_line"></view>
					<view class="group_title_text">
						基本信息
					</view>
				</view>
				<uni-forms-item label="产品编码" required name="SkuNumber" id="SkuNumber_form" labelFont="28rpx"
					contentFont="32rpx" :requireOpacity="0.5">
					<view id="SkuNumber" class="form_li">
						<uni-easyinput placeholderStyle="color:#C1C1C1;font-size:32rpx" inputHeight="88rpx"
							:styles="styles" type="text" v-model="formData.SkuNumber" placeholder="请输入产品编码"
							contentFontSize="32rpx" :disabled="true" />
					</view>
				</uni-forms-item>
				<uni-forms-item label="产品名称" required name="ProductName" id="ProductName_form" labelFont="28rpx"
					contentFont="32rpx" :requireOpacity="0.5">
					<view id="ProductName" class="form_li">
						<uni-easyinput placeholderStyle="color:rgba(193, 193, 193, 1);font-size:32rpx"
							inputHeight="88rpx" :styles="styles" type="text" v-model="formData.ProductName"
							placeholder="请输入产品名称" contentFontSize="32rpx" />
					</view>
				</uni-forms-item>
				<uni-forms-item label="生产来源" required name="ProductFrom" id="ProductFrom_form" labelFont="28rpx"
					contentFont="32rpx" :requireOpacity="0.5">
					<view id="ProductFrom" class="form_li">
						<uni-data-select style="width:100%;" v-model="formData.ProductFrom" :localdata="orginList"
							placeholder="请选择生产来源" borderColor="rgba(255, 255, 255, 0.20)"
							palColor="rgba(193, 193, 193, 1)" :isCustom="true" :isDark="false"></uni-data-select>
					</view>
				</uni-forms-item>
				<uni-forms-item label="产品标签" required name="ProductLabel" id="ProductLabel_form" labelFont="28rpx"
					contentFont="32rpx" :requireOpacity="0.5">
					<view id="ProductLabel" class="form_li">
						<uni-data-select style="width:100%;" v-model="formData.ProductLabel" :localdata="labelList"
							placeholder="请选择产品标签" borderColor="rgba(255, 255, 255, 0.20)"
							palColor="rgba(193, 193, 193, 1)" :isCustom="true" :isDark="false"></uni-data-select>
					</view>
				</uni-forms-item>
				<uni-forms-item label="产品分组" name="TypeId" id="TypeId_form" labelFont="28rpx" contentFont="32rpx"
					:requireOpacity="0.5">
					<view id="TypeId" class="form_li">
						<uni-data-select style="width:100%;" @change="productTypeChange" v-model="formData.TypeId"
							:localdata="productTypeList" placeholder="请选择产品分组"
							borderColor="rgba(255, 255, 255, 0.20)" palColor="rgba(193, 193, 193, 1)" :isCustom="true"
							:isDark="false"></uni-data-select>
					</view>
				</uni-forms-item>
				<uni-forms-item label="产品属性" name="Prop" id="Prop_form" labelFont="28rpx" contentFont="32rpx"
					:requireOpacity="0.5">
					<view id="Prop" class="form_li">
						<uni-data-select style="width:100%;" v-model="formData.Prop" :localdata="propertiesData"
							placeholder="请选择产品属性" borderColor="rgba(255, 255, 255, 0.20)"
							palColor="rgba(193, 193, 193, 1)" :isCustom="true" :isDark="false"></uni-data-select>
					</view>
				</uni-forms-item>
				<uni-forms-item label="产品图片" name="PhotoUrl" id="PhotoUrl_form" labelFont="28rpx" contentFont="32rpx"
					:requireOpacity="0.5">
					<view id="PhotoUrl" class="form_li">
						<upload-image ref="formPhotoUrl" v-model="formData.PhotoUrl" :maxNumber="1"></upload-image>
					</view>
				</uni-forms-item>
				<uni-forms-item label="包装单位" name="Unit" id="Unit_form" labelFont="28rpx" contentFont="32rpx"
					:requireOpacity="0.5">
					<view id="Unit" class="form_li">
						<uni-data-select style="width:100%;" v-model="formData.Unit" :localdata="Unitoptions"
							placeholder="请选择产品属性" borderColor="rgba(255, 255, 255, 0.20)"
							palColor="rgba(193, 193, 193, 1)" :isCustom="true" :isDark="false"></uni-data-select>
					</view>
				</uni-forms-item>
				<uni-forms-item label="总计量" name="Total" id="Total_form" labelFont="28rpx" contentFont="32rpx"
					:requireOpacity="0.5">
					<view id="Total" class="form_li">
						<uni-easyinput placeholderStyle="color:rgba(193, 193, 193, 1);font-size:32rpx"
							inputHeight="88rpx" :styles="styles" type="Number" v-model="formData.Total"
							placeholder="请输入总计量" contentFontSize="32rpx" />
					</view>
				</uni-forms-item>
				<uni-forms-item label="计量单位" name="MinUnit" id="MinUnit_form" labelFont="28rpx" contentFont="32rpx"
					:requireOpacity="0.5">
					<view id="MinUnit" class="form_li">
						<uni-data-select style="width:100%;" v-model="formData.MinUnit" :localdata="Unitoptions"
							placeholder="请选择产品属性" borderColor="rgba(255, 255, 255, 0.20)"
							palColor="rgba(193, 193, 193, 1)" :isCustom="true" :isDark="false"></uni-data-select>
					</view>
				</uni-forms-item>
				<uni-forms-item label="物联网产品" name="IOTProductId" id="IOTProductId_form" labelFont="28rpx"
					contentFont="32rpx" :requireOpacity="0.5"
					v-if="isCheckPermi(['/IoTService/IotProduct/ListPage'])&&formData.ProductLabel=='F'">
					<view id="IOTProductId" class="form_li"
						@click="choiceRelatedObject('物联网产品',false,[formData.IOTProductId],'IOTProductId')">
						<view class="form_input"
							:class="{'placeholder_input':!formData.IOTProductIdName||formData.IOTProductIdName==''}">
							{{formData.IOTProductIdName?formData.IOTProductIdName:'请选择物联网产品'}}
						</view>
						<view class="form_sel_icon">
							<custom-icons iconsName="icon-a-youjiantoubai" iconsSize="20rpx"
								iconsColor="#999999"></custom-icons>
						</view>
					</view>
				</uni-forms-item>
				<uni-forms-item label="产品规格" name="Specs" id="Specs_form" labelFont="28rpx" contentFont="32rpx"
					:requireOpacity="0.5">
					<view id="Specs" class="form_li">
						<uni-easyinput placeholderStyle="color:rgba(193, 193, 193, 1);font-size:32rpx"
							inputHeight="88rpx" :styles="styles" type="text" v-model="formData.Specs"
							placeholder="请输入产品规格" contentFontSize="32rpx" />
					</view>
				</uni-forms-item>
				<uni-forms-item label="成本单价" name="Price" id="Price_form" labelFont="28rpx" contentFont="32rpx"
					:requireOpacity="0.5">
					<view id="Price" class="form_li">
						<uni-easyinput placeholderStyle="color:rgba(193, 193, 193, 1);font-size:32rpx"
							inputHeight="88rpx" :styles="styles" type="Number" v-model="formData.Price"
							placeholder="请输入成本单价" contentFontSize="32rpx" />
					</view>
				</uni-forms-item>
				<uni-forms-item label="销售单价" name="SalesPrice" id="SalesPrice_form" labelFont="28rpx"
					contentFont="32rpx" :requireOpacity="0.5">
					<view id="SalesPrice" class="form_li">
						<uni-easyinput placeholderStyle="color:rgba(193, 193, 193, 1);font-size:32rpx"
							inputHeight="88rpx" :styles="styles" type="Number" v-model="formData.SalesPrice"
							placeholder="请输入销售单价" contentFontSize="32rpx" />
					</view>
				</uni-forms-item>
				<uni-forms-item label="工艺路线" name="Route" id="Route_form" labelFont="28rpx" contentFont="32rpx"
					:requireOpacity="0.5">
					<view id="Route" class="form_li">
						<uni-data-select style="width:100%;" v-model="formData.Route" :localdata="processRouteList"
							placeholder="请选择工艺路线" borderColor="rgba(255, 255, 255, 0.20)"
							palColor="rgba(193, 193, 193, 1)" :isCustom="true" :isDark="false"></uni-data-select>
					</view>
				</uni-forms-item>
				<uni-forms-item label="供应商" name="Supplier" id="Supplier_form" labelFont="28rpx" contentFont="32rpx"
					:requireOpacity="0.5">
					<view id="Supplier" class="form_li"
						@click="choiceRelatedObject('供应商',false,[formData.Supplier],'Supplier')">
						<view class="form_input"
							:class="{'placeholder_input':!formData.SupplierName||formData.SupplierName==''}">
							{{formData.SupplierName?formData.SupplierName:'请选择供应商'}}
						</view>
						<view class="form_sel_icon">
							<custom-icons iconsName="icon-a-youjiantoubai" iconsSize="20rpx"
								iconsColor="#999999"></custom-icons>
						</view>
					</view>
				</uni-forms-item>
				<view class="form_group_title" v-if="filedTableList&&filedTableList.length>0">
					<view class="group_line"></view>
					<view class="group_title_text">
						自定义信息
					</view>
				</view>
				<view v-for="(ite,inx) in filedTableList">
					<uni-forms-item :label="ite.name" :required="ite.is_required" :name="ite.mapid"
						:id="ite.mapid+'_form'" labelFont="28rpx" contentFont="32rpx" :requireOpacity="0.5">
						<zxz-uni-data-select :filterable="true" @change="selectDataChange($event,inx)"
							v-if="(ite.type == '单选框' && ite.show_way == '下拉') || (ite.type == '复选框' && ite.show_way == '下拉')"
							v-model="formData[ite.mapid]" :multiple="ite.type == '复选框'" dataKey="text" dataValue="value"
							:localdata="transformData(ite.optionals)" :isCanCustom="ite.is_add" :iscustom="true"
							:placeholder='ite.prompt_text ? ite.prompt_text : "请选择"'></zxz-uni-data-select>
						<uni-data-checkbox @change="customValChange" :multiple="ite.type == '复选框'"
							v-if="ite.type == '单选框' && ite.show_way == '平铺'|| (ite.type == '复选框' && ite.show_way == '平铺')"
							v-model="formData[ite.mapid]" :localdata="transformData(ite.optionals)"></uni-data-checkbox>
						<uni-easyinput v-if="ite.type=='文本'" @input="customValChange"
							placeholderStyle="color:rgba(193, 193, 193, 1);font-size:32rpx" inputHeight="88rpx"
							:styles="styles" type="text" v-model="formData[ite.mapid]"
							:placeholder='ite.prompt_text ? ite.prompt_text : "请输入"' contentFontSize="32rpx" />
						<numberInput @input="customValChange" v-if="ite.type=='数字'" v-model="formData[ite.mapid]"
							:placeholder='ite.prompt_text ? ite.prompt_text : "请输入"' :precision="ite.decimals"
							:isQianfenwei="ite.is_thousandth"></numberInput>
						<view class="int_date" v-if="ite.type=='时间'">
							<view class="date_con long_date" style="color: #fff;">
								<uni-datetime-picker :ref="'dateChoice'+ite.mapid" class="date" type="datetime"
									placeholder-style="font-size:32rpx;color:#999999" :clear-icon="true"
									v-model="formData[ite.mapid]"
									:placeholder='ite.prompt_text ? ite.prompt_text : "请选择"' :isCustom="true"
									:isDark="false" :border="false">
								</uni-datetime-picker>
							</view>
						</view>
						<view v-if="ite.type == '关联对象'" :id="ite.mapid" class="form_li"
							@click="choiceRelatedObject(ite.object_type,false,[formData[ite.mapid]],ite.mapid)">
							<view class="form_input"
								:class="{'placeholder_input':!formData[ite.mapid+'Name']||formData[ite.mapid+'Name']==''}">
								{{formData[ite.mapid+'Name']?formData[ite.mapid+'Name']:'请选择'+ite.object_type}}
							</view>
							<view class="form_sel_icon">
								<custom-icons iconsName="icon-a-youjiantoubai" iconsSize="20rpx"
									iconsColor="#999999"></custom-icons>
							</view>
						</view>
					</uni-forms-item>
				</view>
				<uni-forms-item label="备注" name="Remark" id="Remark_form" labelFont="28rpx" contentFont="32rpx"
					:requireOpacity="0.5">
					<view id="Remark" class="form_li">
						<uni-easyinput placeholderStyle="color:rgba(193, 193, 193, 1);font-size:32rpx"
							inputHeight="70rpx" :styles="styles" type="textarea" v-model="formData.Remark"
							placeholder="请输入备注" contentFontSize="32rpx" 
							autoHeight />
					</view>
				</uni-forms-item>
				<button class="submit_button" @click="submit" :disabled="isLoading"
					:style="{'opacity':isLoading?0.6:1,'margin-top': '60rpx'}" :loading="isLoading">
					保存
				</button>
			</view>
		</uni-forms>
		<msg-prompt ref="promptMsg"></msg-prompt>
		<objectfiledSelect ref="objectfiled_select" @finishFiledChoice="finishFiledChoice"></objectfiledSelect>
	</view>
</template>

<script>
	import {
		factoryProductNumber,
		orgField,
		factoryProductInfo,
		factoryProductTypeListGet,
		factoryProductTypeInfo,
		editProductSave,
		addProductSave
	} from '@/api/product.js'
	import {
		factoryUnitListGet
	} from '@/api/factory/unit.js'
	import objectfiledSelect from '@/pages_factory/cmp/objectfiled-select.vue'
	import numberInput from '@/pages_factory/cmp/number-input.vue'
	import {
		setPagesParam
	} from '@/common/utillib.js'
	var dayjs = require('@/common/day.js')
	export default {
		components: {
			objectfiledSelect,
			numberInput
		},
		data() {
			return {
				topTitle: '添加产品',
				formData: {},
				isLoading: false,
				styles: {
					color: '#333',
					backgroundColor: '#F8F8F8',
					disableColor: 'rgba(248, 248, 248, 1)',
					borderColor: '#F8F8F8'
				},
				rules: {
					ProductName: {
						rules: [{
							required: true,
							errorMessage: "请输入产品名称"
						}]
					},
					ProductFrom: {
						rules: [{
							required: true,
							errorMessage: "请选择产品来源"
						}]
					},
					ProductLabel: {
						rules: [{
							required: true,
							errorMessage: "请选择产品标签"
						}]
					},
				},
				propertiesData: [], //属性
				filedTableList: [], //产品字段
				labelList: [{
					text: "半成品",
					value: "U"
				}, {
					text: "成品",
					value: "F"
				}],
				orginList: [{
					text: "自制",
					value: "自制"
				}, {
					text: "外购",
					value: "外购"
				}, {
					text: "委外",
					value: "委外"
				}],
				productTypeList: [],
				userInfo: {},
				Unitoptions: [],
				processRouteList: [],
				activeChoiceObjectFiled: '',
				id: ''
			};
		},
		async onLoad(options) {
			if (this.$store.state.user && this.$store.state.user.uid) {
				this.userInfo = this.$store.state.user;
			} else {
				await this.$store.dispatch('GetInfo');
				this.userInfo = this.$store.state.user;
			}
			await this.loadProductTypeList()
			await this.getUnitList()
			if (options.id) {
				this.id = options.id
				await this.loadFirstInfo(options.id)
			} else {
				await this.loadFirstInfo()
			}
		},
		methods: {
			selectDataChange(arr, inx) {
				// console.log(arr, 'obj');
				if (arr) {
					if (Array.isArray(arr)) {
						arr.map(rw => {
							let findObj = this.filedTableList[inx].optionals.find(ro => ro == rw.value)
							if (findObj) {} else {
								this.filedTableList[inx].optionals.push(rw.value)
								let tableList = JSON.parse(JSON.stringify(this.filedTableList))
								this.filedTableList = JSON.parse(JSON.stringify(tableList))
							}
						})
					} else {
						let findObj = this.filedTableList[inx].optionals.find(ro => ro == arr.value)
						if (findObj) {} else {
							this.filedTableList[inx].optionals.push(arr.value)
							let tableList = JSON.parse(JSON.stringify(this.filedTableList))
							this.filedTableList = JSON.parse(JSON.stringify(tableList))
						}
					}

				}
				this.customValChange()
			},
			transformData(list) {
				let resList = list.map(rw => {
					let obj = {
						text: rw,
						value: rw
					}
					return obj
				})
				return resList
			},
			finishFiledChoice(arr) {
				//完成关联对象的相关选择
				if (this.activeChoiceObjectFiled) {
					this.formData[this.activeChoiceObjectFiled] = arr[0].enValue
					this.formData[this.activeChoiceObjectFiled + 'Name'] = arr[0].name
					if (this.activeChoiceObjectFiled != '') {
						let finObjFiled = this.filedTableList.find(rw => rw.mapid == this.activeChoiceObjectFiled)
						if (finObjFiled) {
							if (finObjFiled.items && finObjFiled.items.length > 0) {
								// console.log(this.formData[finObjFiled.mapid]);
								// let fieldMapidVal = this.formData[finObjFiled.mapid].split(',')
								// let findObj = this.associationObject[finObjFiled.mapid].find(row => row.Value == fieldMapidVal[0])
								for (let i = 0; i < finObjFiled.items.length; i++) {
									let item = finObjFiled.items[i]
									this.formData[item.field] = arr[0].Obj[item.source_obj]
								}
							}
						}
					}
					this.customValChange()
					this.activeChoiceObjectFiled = ''
					this.$forceUpdate()
				}
			},
			choiceRelatedObject(type, multiple, alChoice, filed) {
				//选择关联对象
				this.activeChoiceObjectFiled = filed
				this.$refs.objectfiled_select.openPopup(type, multiple, alChoice)
			},
			async getUnitList() { //获取单位列表
				let res = await factoryUnitListGet()
				// console.log("查询到单位", res);
				this.Unitoptions = res.data.map(row => {
					let obj = {
						text: row.UnitName,
						value: row.UnitName
					}
					return obj
				})
			},
			async loadProductTypeList() { //产品分组
				try {
					let res = await factoryProductTypeListGet()
					this.productTypeList = res.data.map(row => {
						let obj = {
							text: row.Name,
							value: row.Id
						}
						return obj
					})
				} catch (error) {
					console.log(error, 'error');
				}
			},
			async getFactoryProductNumber() { //生成产品编码
				let res = await factoryProductNumber();
				this.formData.SkuNumber = res.data;
				// console.log("产品编码",res);
			},
			async loadFirstInfo(id) {
				//初始化
				try {
					this.propertiesData = []
					// this.associationObject={}

					await this.getProductCustomFiled();
					if (id) {
						let res = await factoryProductInfo({
							id: id
						});
						let productinfo = res.data;
						this.formData = {
							Id: productinfo.Id,
							ProductName: productinfo.ProductName,
							ProductFrom: productinfo.ProductFrom,
							TypeId: productinfo.TypeId,
							SkuNumber: productinfo.SkuNumber, //产品编码
							IOTProductId: productinfo.IOTProductId, //物联网编码
							ProductLabel: productinfo.ProductLabel, //产品标签
							Prop: productinfo.Prop, //产品属性
							PhotoUrl: productinfo.PhotoUrl ? [productinfo.PhotoUrl] : [], //产品图片
							Unit: productinfo.Unit, //单位
							MinUnit: productinfo.MinUnit, //单位
							Specs: productinfo.Specs, //产品规格
							Price: productinfo.Price, //成本单价
							Total: productinfo.Total, //总计量
							SalesPrice: productinfo.SalesPrice, //销售单价
							Route: productinfo.Route, //工艺路线，
							Supplier: productinfo.Supplier, //供应商，
							Remark: productinfo.Remark, //备注说明
						};
						if (productinfo.Supplier && productinfo.Supplier.indexOf(',') > -1) {
							this.formData.SupplierName = productinfo.Supplier.split(",")[1]
						} else {
							this.formData.SupplierName = ''
						}
						if (productinfo.IOTProductId && productinfo.IOTProductId.indexOf(',') > -1) {
							this.formData.IOTProductIdName = productinfo.IOTProductId.split(",")[1]
						} else {
							this.formData.IOTProductIdName = ''
						}
						this.productTypeChange()
						this.setCustomDefaultValue(productinfo);

					} else {
						this.formData = {
							ProductName: "",
							ProductFrom: '',
							TypeId: "",
							SkuNumber: "", //产品编码
							IOTProductId: "", //物联网编码
							ProductLabel: "", //产品标签
							Prop: "", //产品属性
							PhotoUrl: [], //产品图片
							Unit: "", //单位
							Specs: "", //产品规格
							Price: 0, //成本单价
							Total: 1, //总计量
							SalesPrice: 0, //销售单价
							Route: "", //工艺路线，
							Supplier: "", //供应商，
							Remark: "", //备注说明
						}
						this.setCustomDefaultValue();
						// console.log("表单初始化",this.formData);
						await this.getFactoryProductNumber();
					}

				} catch (error) {
					console.log("出错", error);
				}
			},
			productTypeChange(val) {
				if (val) { //当属于手动切换产品分组时，属性要清空
					this.formData.Prop = ''
				}
				if (this.formData.TypeId) { //产品分组属性
					factoryProductTypeInfo({
						id: this.formData.TypeId
					}).then(res => {
						// console.log('产品分组属性',res);
						if (res.data.PropList) {
							let propArr = res.data.PropList.split(',')
							this.propertiesData = propArr.map(ro => {
								let obj = {
									text: ro,
									value: ro
								}
								return obj
							})
						}
					})
				} else {
					this.propertiesData = []
				}
			},
			setFormItemHide(item) { //判断字段是否隐藏
				if (item.conditions && item.conditions.length > 0) {
					let result = false;
					let conditionsResArr = [];
					for (let i = 0; i < item.conditions.length; i++) {
						let row = item.conditions[i];
						conditionsResArr[i] = this.returnCompareResult(
							row.field,
							row.compare,
							row.val,
							row.valtype
						);
					}
					for (let i = 0; i < conditionsResArr.length; i++) {
						if (i == 0) {
							result = conditionsResArr[i];
						} else {
							if (item.groups && item.groups[i - 1]) {
								if (item.groups[i - 1] == "and") {
									result = result && conditionsResArr[i];
								} else if (item.groups[i - 1] == "or") {
									result = result || conditionsResArr[i];
								}
							} else {
								result = result || conditionsResArr[i];
							}
						}
					}
					return result;
				} else {
					return false;
				}
			},
			returnCompareResult(field, compare, val, type) { //隐藏规则设置方法
				let result = true;
				switch (compare) {
					case "=":
						result = this.formData[field] == val;
						break;
					case "!=":
						result = this.formData[field] != val;
						break;
					case "IN":
						result = this.formData[field] && this.formData[field].indexOf(val) > -1;
						break;
					case "NOTIN":
						result = !this.formData[field] ||
							(this.formData[field] && this.formData[field].indexOf(val) == -1);
						break;
					case "ISNULL":
						result = this.formData[field] == "" || this.formData[field] == null;
						break;
					case "NOTNULL":
						result = this.formData[field] != "" && this.formData[field] != null;
						break;
					case ">":
						if (type && type == "时间") {
							result = val.timeValue && dayjs(this.formData[field]).valueOf() > dayjs(val.timeValue)
								.valueOf();
						} else if (type && type == "数字") {
							result = this.formData[field] > val;
						}
						break;
					case "<":
						if (type && type == "时间") {
							result = val.timeValue && dayjs(this.formData[field]).valueOf() < dayjs(val.timeValue)
								.valueOf();
						} else if (type && type == "数字") {
							result = this.formData[field] < val;
						}
						break;
					case "==":
						if (type && type == "时间") {
							result = val.timeValue && dayjs(this.formData[field]).valueOf() == dayjs(val.timeValue)
								.valueOf();
						} else if (type && type == "数字") {
							result = this.formData[field] == val;
						}
						break;
					case "><":
						if (type && type == "时间") {
							result =
								val.timeValue &&
								dayjs(this.formData[field]).valueOf() !=
								dayjs(val.timeValue).valueOf();
						} else if (type && type == "数字") {
							result = this.formData[field] != val;
						}
						break;
					case ">=":
						if (type && type == "时间") {
							result =
								val.timeValue &&
								dayjs(this.formData[field]).valueOf() >=
								dayjs(val.timeValue).valueOf();
						} else if (type && type == "数字") {
							result = this.formData[field] >= val;
						}
						break;
					case "<=":
						if (type && type == "时间") {
							result = val.timeValue && dayjs(this.formData[field]).valueOf() <= dayjs(val.timeValue)
								.valueOf();
						} else if (type && type == "数字") {
							result = this.formData[field] <= val;
						}
						break;
					case "INRANGE":
						if (type && type == "数字") {

							if (val.min && val.max) {
								if (this.formData[field] >= val.min && this.formData[field] <= val.max) {
									result = true;
								} else {
									result = false;
								}
							}
						}

						break;
					case "NOTINRANGE":
						if (type && type == "数字") {
							if (val.min && val.max) {
								if (this.formData[field] < val.min && this.formData[field] > val.max) {
									result = true;
								} else {
									result = false;
								}
							}
						}
						break;
					case "SELECTRANGE":
						if (type && type == "时间") {
							if (val[0] && val[1]) {
								let max = Math.max(...val);
								let min = Math.min(...val);
								if (this.formData[field] >= min && this.formData[field] <= max) {
									result = true;
								} else {
									result = false;
								}
							}
						}
						break;
					case "DYNAMICS":
						if (type && type == "时间") {
							if (val[0] && val[1]) {
								let max = Math.max(...val);
								let min = Math.min(...val);
								if (this.formData[field] >= min && this.formData[field] <= max) {
									result = true;
								} else {
									result = false;
								}
							}
						}
						break;
				}
				return result;
			},
			customValChange(val, fidItem) { //数据发生变化后刷新，并验证表单
				// console.log("看看关联对象选择后有没有出现",fidItem);
				let formData = JSON.parse(JSON.stringify(this.formData));
				this.formData = JSON.parse(JSON.stringify(formData));

			},
			async getProductCustomFiled() {
				//获取产品的自定义字段
				try {
					this.filedTableList = [];
					let res = await orgField({
						orgId: this.userInfo.orgId,
						field: "产品"
					});
					if (res.data) {
						if (res.data.ExtValue) {
							let filedList = JSON.parse(res.data.ExtValue);
							this.filedTableList = filedList; //排序处理，并且数字字段排前面
						}
					} else {
						this.filedTableList = [];
					}
				} catch (e) {
					console.log("e", e);
				}
			},
			setCustomDefaultValue(afterForm) {
				//设置自定义的变量初始化
				let res = this.filedTableList.map((rw, inx) => {
					if (afterForm) {
						this.formData[rw.mapid] = afterForm[rw.mapid];
						if (rw.type == "时间") {
							// console.log('时间字段的值',afterForm[rw.mapid]);
							if(afterForm[rw.mapid]){
								this.formData[rw.mapid] = dayjs(afterForm[rw.mapid]).format('YYYY-MM-DD HH:mm:ss');
							}
							
						} else {
							if (rw.type == "数字") {
								this.formData[rw.mapid] = Number(afterForm[rw.mapid]);
							} else if (rw.type == "复选框") {
								this.formData[rw.mapid] = afterForm[rw.mapid]?afterForm[rw.mapid].split(","):[];
								this.formData[rw.mapid].map(ro => { //自创建的选项加入选项列表
									if (ro) {
										let find = rw.optionals.find(ow => ow == ro)
										if (find) {} else {
											rw.optionals.push(ro)
										}
									}

								})

							} else if (rw.type == "单选框") {
								this.formData[rw.mapid] = afterForm[rw.mapid]?afterForm[rw.mapid]:'';
								if(this.formData[rw.mapid]){
									let find = rw.optionals.find(ow => ow == this.formData[rw.mapid])
									if (find) {} else { //自创建的选项加入选项列表
										rw.optionals.push(this.formData[rw.mapid])
									}
								}
								
							} else {
								this.formData[rw.mapid] = afterForm[rw.mapid];
								if (rw.type == '关联对象') {
									// console.log(afterForm,'afterForm');
									if (afterForm[rw.mapid] && afterForm[rw.mapid].indexOf(',') > -1) {
										this.formData[rw.mapid + 'Name'] = afterForm[rw.mapid].split(",")[1]
									}

								}
							}
						}
					} else {
						this.formData[rw.mapid] = null;
						if (rw.defval != "" && rw.defval != undefined && rw.defval != null) {
							if (rw.type == "时间") {
								this.formData[rw.mapid] = dayjs(rw.defval).format('YYYY-MM-DD HH:mm:ss');
							} else {
								if (rw.type == "数字") {
									this.formData[rw.mapid] = Number(rw.defval);
								} else {
									this.formData[rw.mapid] = rw.defval;
								}
							}
						} else {
							if (rw.type == "复选框") {
								this.formData[rw.mapid] = [];
							}
						}
					}
					if (rw.is_required) {
						if (rw.type == "单选框" || rw.type == "复选框" || rw.type == "时间") {
							let rowRules = {
								rules: [{
									required: true,
									errorMessage: "请选择" + rw.name,
								}]
							}
							this.rules[rw.mapid] = rowRules;
						} else {
							let rowRules = {
								rules: [{
									required: true,
									errorMessage: "请输入" + rw.name
								}]
							}
							this.rules[rw.mapid] = rowRules;
						}
					}
					return rw
				});
				let rules=JSON.parse((JSON.stringify(this.rules)))
				this.rules=JSON.parse((JSON.stringify(rules)))
				this.filedTableList = JSON.parse(JSON.stringify(res))
				console.log(this.rules,'this.rules');
			},
			submit() {
				this.$refs.formData.validate().then(async valid => {
					if (valid) {
						this.isLoading = true
						try {
							let submitForm = JSON.parse((JSON.stringify(this.formData)))
							for (let i = 0; i < this.filedTableList.length; i++) {
								let row = this.filedTableList[i];
								if (row.type == "数字") {
									if (submitForm[row.mapid]) {} else {
										submitForm[row.mapid] = Number(submitForm[row.mapid]);
									}
								} else if (row.type == "时间") {
									submitForm[row.mapid] = dayjs(submitForm[row.mapid]).valueOf();
								} else if (row.type == "复选框") {
									if (submitForm[row.mapid] && submitForm[row.mapid].length > 0) {
										submitForm[row.mapid] = submitForm[row.mapid].join(",");
									} else {
										submitForm[row.mapid] = "";
									}
								} else if (row.type == "图片") {
									if (submitForm[row.mapid] && submitForm[row.mapid].length > 0) {
										submitForm[row.mapid] = submitForm[row.mapid][0]
									} else {
										submitForm[row.mapid] = "";
									}
								} else {
									if (submitForm[row.mapid]) {} else {
										submitForm[row.mapid] = "";
									}
								}
							}
							if (submitForm.PhotoUrl && submitForm.PhotoUrl.length > 0) {
								submitForm.PhotoUrl = submitForm.PhotoUrl[0]
							} else {
								submitForm.PhotoUrl = ''
							}
							let response;
							if (this.id && this.id != null) {
								submitForm.Id = this.id;
								response = await editProductSave(submitForm);
							} else {
								response = await addProductSave(submitForm);
							}
							this.$refs.promptMsg.open('操作成功', 1500)
							setTimeout(() => {
								setPagesParam('loadList', 'load', 1)
							}, 1500)

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
		}
	}
</script>

<style lang="less" scoped>
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
</style>