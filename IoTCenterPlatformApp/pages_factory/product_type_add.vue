<template>
	<view>
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
				<uni-forms-item label="分组名称" required name="Name" id="Name_form" labelFont="28rpx" contentFont="32rpx"
					:requireOpacity="0.5">
					<view id="Name" class="form_li">
						<uni-easyinput @input="typeNameChange"
							placeholderStyle="color:rgba(193, 193, 193, 1);font-size:32rpx" inputHeight="88rpx"
							:styles="styles" type="text" v-model="formData.Name" placeholder="请输入分组名称"
							contentFontSize="32rpx" />
					</view>
				</uni-forms-item>
				<uni-forms-item label="产品图片" required name="PhotoUrl" id="PhotoUrl_form" labelFont="28rpx"
					contentFont="32rpx" :requireOpacity="0.5">
					<view id="PhotoUrl" class="form_li">
						<upload-image ref="formPhotoUrl" v-model="formData.PhotoUrl" :maxNumber="1"></upload-image>
					</view>
				</uni-forms-item>
				<uni-forms-item label="分组属性" name="PropList" id="PropList_form" labelFont="28rpx" contentFont="32rpx"
					:requireOpacity="0.5">
					<view id="PropList" class="form_li" style="flex-direction: column;">
						<view class="prop_ul" v-if="formData.PropList&&formData.PropList.length>0">
							<view class="prop_li" v-for="(it,inx) in formData.PropList">
								<uni-easyinput placeholderStyle="color:rgba(193, 193, 193, 1);font-size:32rpx"
									inputHeight="88rpx" :styles="styles" type="text" v-model="formData.PropList[inx]"
									placeholder="请输入属性名称" contentFontSize="32rpx" />
								<view class="icon_del t-icon-shouqi1" @click="delTypeProps(inx)"></view>
							</view>
						</view>
						<view class="filter_add" @click.stop="addTypeProps">
							<view class="icon_con">
								<custom-icons iconsName="icon-tianjia" iconsSize="24rpx"
									iconsColor="#333333"></custom-icons>
							</view>
							<view class="text">添加分组属性</view>
						</view>
					</view>
				</uni-forms-item>

				<view class="form_group_title">
					<view class="group_line"></view>
					<view class="group_title_text">
						数据过滤
					</view>
				</view>
				<product_filter :isPopup="false" @filterListChange="filterListChange" :filterKey="filterKey"
					ref="product_filter" @choiceFiled="choiceFiled" :productFiledList="productFiledList">
				</product_filter>
				<button class="submit_button" @click="submit" :disabled="isLoading"
					:style="{'opacity':isLoading?0.6:1,'margin-top': '60rpx'}" :loading="isLoading">
					保存
				</button>
			</view>
		</uni-forms>
		<msg-prompt ref="promptMsg" @confirm="confirmDelete" @msgClose="msgClose"></msg-prompt>

		<filed_select ref="filed_select" @finishFiledChoice="finishFiledChoice"></filed_select>
	</view>
</template>

<script>
	import {
		addProductTypeSave,
		editProductTypeSave,
		factoryProductTypeInfo,
		orgFormFields,
	} from '@/api/product.js'
	import product_filter from '@/pages_factory/cmp/product_filter.vue'
	import filed_select from '@/pages_factory/cmp/filed_select.vue'
	import {
		setPagesParam
	} from '@/common/utillib.js'
	var dayjs = require('@/common/day.js')
	export default {
		components: {
			product_filter,
			filed_select
		},
		data() {
			return {
				topTitle: '添加分组',
				formData: {},
				isLoading: false,
				filterKey: 1,
				productFiledList: [], //产品字段
				filterList: [], //数据过滤列表
				fieldTable: [], //字段设置列表（显示、固定、序号）
				styles: {
					color: '#333',
					backgroundColor: '#F8F8F8',
					disableColor: 'rgba(248, 248, 248, 1)',
					borderColor: '#F8F8F8'
				},
				rules: {
					Name: {
						rules: [{
							required: true,
							errorMessage: "请输入分组名称"
						}]
					},
					PhotoUrl: {
						rules: [{
							required: true,
							errorMessage: "请上传分组图标"
						}]
					},
				},
				id: ''
			};
		},
		async onLoad(options) {
			this.fieldTable = []
			if (options.id) {
				await this.loadTypeInfo(options.id)
			} else {
				await this.loadOrgFormFields()
				const eventChannel = this.getOpenerEventChannel();
				if (eventChannel && eventChannel.emitCache && eventChannel.emitCache.acceptFilterData) {
					// 监听acceptDataFromOpenerPage事件，获取上一页面通过eventChannel传送到当前页面的数据
					eventChannel.on('acceptFilterData', (data) => {
						// console.log(data)
						if (data && data.filterList) {
							this.filterList = JSON.parse(JSON.stringify(data.filterList))
							this.$refs.product_filter.openPopup(this.filterList)
						}
						this.$forceUpdate()
					})
				} else {
					//默认设置过滤分组
					this.filterList = [{
						field: 'TypeName',
						compare: '等于', //比较符号：大于、小于、大于等于、小于等于、不等于、等于、包含、不包含
						val: '', //字符串
						val_num: null, //数字
						val_arr: [], //字符串数组
						isdef: true
					}]
					this.$refs.product_filter.openPopup(this.filterList)
				}

			}
		},
		computed: {
			filterProductFiledList() {
				//过滤文本，时间，数字类型的字段
				let list = this.productFiledList.filter(row => row.type == '文本' || row.type == '时间' || row.type == '数字' ||
					row.type == '关联对象')
				return list
			}
		},
		methods: {
			typeNameChange(val) {
				this.$nextTick(()=>{
					this.$refs.product_filter.changeTypeFilter(this.formData.Name)
				})
			},
			async loadTypeInfo(id) {
				this.topTitle = '修改产品分组'
				let res = await factoryProductTypeInfo({
					id: id
				})
				this.id = id
				// console.log(res,'resres');
				this.formData = {
					id: id,
					Sort: res.data.Sort,
					PhotoUrl: res.data.PhotoUrl ? [res.data.PhotoUrl] : [],
					Name: res.data.Name,
					PropList: res.data.PropList ? res.data.PropList.split(',') : []
				}
				let fieldTable = res.data.ListFieldsJson ? JSON.parse(res.data.ListFieldsJson) : []
				await this.loadOrgFormFields(fieldTable)
				let filterList = res.data.ConditionJson ? JSON.parse(res.data.ConditionJson) : []
				// console.log(filterList, 'filterListfilterList');
				this.filterList = JSON.parse(JSON.stringify(filterList))
				this.$refs.product_filter.openPopup(this.filterList)
			},
			addTypeProps() {
				if (this.formData.PropList) {} else {
					this.formData.PropList = []
				}
				this.formData.PropList.push('')
				this.$forceUpdate()
			},
			delTypeProps(inx){
				this.formData.PropList.splice(inx,1)
			},
			filterListChange(list) { //数据过滤字段数据发生变化
				if (list) {
					this.filterList = JSON.parse(JSON.stringify(list))
				}
				// console.log('this.filterList',this.filterList);
			},
			choiceFiled(filterProductFiledList) { //选择过滤字段
				this.$refs.filed_select.openPopup(filterProductFiledList, this.filterList)
			},
			submit() {
				this.$refs.formData.validate().then(async valid => {
					if (valid) {
						this.isLoading = true
						try {
							let filterList = JSON.parse(JSON.stringify(this.filterList))
							filterList = filterList.map(rw => {
								let rowObj = this.filterProductFiledList.find(row => row.mapid == rw
									.field)
								if (rowObj && rowObj.type == '时间') {
									rw.val_num = dayjs(rw.val).valueOf()
									rw.val = ''
								}
								if (rw.val_num) {} else {
									delete rw.val_num
								}
								if (rw.val_arr && rw.val_arr.length > 0) {} else {
									delete rw.val_arr
								}
								return rw
							})
							let submitForm = JSON.parse((JSON.stringify(this.formData)))
							if (submitForm.PhotoUrl && submitForm.PhotoUrl.length > 0) {
								submitForm.PhotoUrl = submitForm.PhotoUrl[0]
							} else {
								submitForm.PhotoUrl = ''
							}
							if (submitForm.PropList && submitForm.PropList.length > 0) {
								submitForm.PropList = submitForm.PropList.join(',')
							} else {
								submitForm.PropList = ''
							}
							submitForm.ConditionJson = filterList && filterList.length > 0 ? JSON.stringify(
								filterList) : ''
							submitForm.ListFieldsJson = this.fieldTable && this.fieldTable.length > 0 ? JSON
								.stringify(this.fieldTable) : ''
							let response;
							if (this.id && this.id != null) {
								submitForm.Id = this.id;
								response = await editProductTypeSave(submitForm);
							} else {
								response = await addProductTypeSave(submitForm);
							}
							this.$refs.promptMsg.open('操作成功', 1500)
							setTimeout(() => {
								setPagesParam('loadTypeList', 'load', 1)
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
			finishFiledChoice(list) { //选择过滤字段后
				if (list) {
					list.map(row => {
						let findObj = this.filterList.find(ro => ro.field == row.mapid)
						if (findObj) {} else {
							let obj = {
								field: row.mapid,
								compare: '', //比较符号：大于、小于、大于等于、小于等于、不等于、等于、包含、不包含
								val: '', //字符串
								val_num: null, //数字
								val_arr: [] //字符串数组
							}
							this.filterList.push(obj)
						}
					})
					this.filterKey++
					this.$refs.product_filter.setNewFilterData(this.filterList)
				}
			},
			async loadOrgFormFields(list) {
				try {
					this.fieldTable = []
					let res = await orgFormFields({
						field: '产品',
						ext: true
					})
					// console.log("产品所有字段",res);
					let data = res.data
					this.productFiledList = JSON.parse(JSON.stringify(res.data))
					if (list) {
						this.setDefaultFiledSetting(list)
					} else {
						this.setDefaultFiledSetting(this.getDefaultfiledsetting())
					}

					// this.originalField = JSON.parse(JSON.stringify(this.fieldTable))

				} catch (error) {
					console.log(error, 'error');
				}
			},
			setDefaultFiledSetting(list) {

				if (list && list.length > 0) {
					list.map(row => {
						// console.log("字段",row);
						let obj = null
						if (list && list.length > 0) {
							obj = list.find(rw => row.mapid == rw.field)
						}
						if (obj) {} else {
							obj = {
								field: row.mapid, //字段
								fieldName: row.name, //字段名称
								type: row.type,
								isShow: true, //是否显示
								isFixed: false, //是否固定
							}
						}
						this.fieldTable.push(obj)
					})
				}
			},
			getDefaultfiledsetting() { //获取默认的字段设置
				return [{
						"field": "SkuNumber",
						"fieldName": "产品编号",
						"type": "文本",
						"isShow": true,
						"isFixed": false
					},
					{
						"field": "ProductName",
						"fieldName": "产品名称",
						"type": "文本",
						"isShow": true,
						"isFixed": false
					},
					{
						"field": "ProductLabel",
						"fieldName": "产品标签",
						"type": "文本",
						"isShow": true,
						"isFixed": false
					},
					{
						"field": "TypeName",
						"fieldName": "产品分组",
						"type": "文本",
						"isShow": true,
						"isFixed": false
					},
					{
						"field": "Prop",
						"fieldName": "产品属性",
						"type": "文本",
						"isShow": true,
						"isFixed": false
					},
					{
						"field": "Total",
						"fieldName": "总计量",
						"type": "数字",
						"isShow": true,
						"isFixed": false
					},
					{
						"field": "Price",
						"fieldName": "成本单价",
						"type": "数字",
						"isShow": true,
						"isFixed": false
					},
					{
						"field": "SalesPrice",
						"fieldName": "销售单价",
						"type": "数字",
						"isShow": true,
						"isFixed": false
					}
				]
			}
		}
	}
</script>

<style lang="less" scoped>
	.prop_ul {
		width: 100%;

		.prop_li {
			width: 100%;
			margin-bottom: 20rpx;
		}

		.prop_li:last-child {
			margin-bottom: 0;
		}
	}

	.filter_add {
		display: flex;
		justify-content: center;
		align-items: center;
		width: 100%;
		height: 88rpx;
		font-size: 28rpx;
		color: rgba(51, 51, 51, 1);
		background: rgba(248, 248, 248, 1);
		margin-top: 30rpx;

		.icon_con {
			margin-right: 10rpx;
			line-height: 24rpx;
		}

		.text {
			line-height: 28rpx;
		}
	}
</style>