<template>
	<view class="initialize_con">
		<view class="initialize_title">{{flowTitle}}</view>
		<view class="initialize_table" v-for="(item,inx) in formInit">
			<view class="form_field">
				<view class="line"></view>
				<view class="text">{{item.title}}</view>
			</view>
			<view class="value_type">
				<view class="title_text">值类型</view>
				<uni-data-select :clear="false" :isCustom="true" v-model="item.way" :localdata="returnValueType(item)"
					type="line" placeholder="请选择" class="addPool-selected" style="color:#333;"
					@change="changeLoadVal($event, item, inx)" :disabled="isViewInfo" :borderColor="isViewInfo?'rgba(234, 234, 234, 1)':'#F8F8F8'"></uni-data-select>
			</view>
			<view class="form_value">
				<view class="title_text">初始值</view>
				<template v-if="item.eltype != 'TableList'">
					<uni-easyinput v-if="item.way == 0" placeholderStyle="color:rgba(193, 193, 193, 1);font-size:32rpx"
						inputHeight="88rpx" :styles="styles" type="text" v-model="item.val" placeholder="请输入内容"
						contentFontSize="32rpx" primaryColor="rgba(255, 255, 255, 0.5)" />
					<uni-data-select v-else :clear="false" :isCustom="true" v-model="item.val"
						:localdata="returnEltypeList(item)" type="line" placeholder="请选择" class="addPool-selected"
						style="color:#333;" :disabled="isViewInfo" :borderColor="isViewInfo?'rgba(234, 234, 234, 1)':'#F8F8F8'"></uni-data-select>
				</template>
				<template v-else>
					<TableList mode="mode" v-model="item.val" :valueModel="{}" :columns="item.props.columns"
						:maxSize="item.props.maxSize" :summaryUnit="item.props.summaryUnit"
						:summaryColumns="item.props.summaryColumns" :deductid="item.props.deductid"
						:showSummary="item.props.showSummary" :IdxColName="item.props.IdxColName"
						:rowLayout="item.props.rowLayout" :showBorder="item.props.showBorder"
						:enablePrint="item.props.enablePrint" :required="item.props.required" :disabled="isViewInfo" />
				</template>
			</view>
		</view>
	</view>
</template>

<script>
	import TableList from '@/pages_flow/flow-form/form-components/TableList.vue'
	export default {
		name: 'AdminUiHouseFlow',
		props: ['formInitVal', 'tbloading', 'flowTitle', 'index'],
		components: {
			TableList
		},
		data() {
			return {
				isViewInfo: false,
				formInit: this.formInitVal,
				valueType: [{
						value: 0,
						text: "自定义"
					},
					{
						value: 1,
						text: "系统值"
					},
				],
				styles: {
					color: '#333',
					backgroundColor: 'rgba(248, 248, 248, 1)',
					disableColor: 'rgba(28, 34, 50, 1)',
					borderColor: 'rgba(255, 255, 255, 0.20)'
				},
			};
		},
		watch: {
			formInit: {
				deep: true,
				handler(newVal) {
					// console.log(newVal,'newValnewVal');
					this.$forceUpdate()
					this.$emit('setformInitData', this.formInit, this.index)
				},
			},
			formInitVal: {
				deep: true,
				handler(newVal) {
					this.formInit = JSON.parse(JSON.stringify(newVal))
				},
				immediate: true
			},
		},
		mounted() {

		},
		methods: {
			wayChange() {
				//数量类型发生改变
				this.$forceUpdate()
			},
			returnEltypeList(row) {
				let eltypeList = []
				if (this.index == 1 && (row.eltype == 'TextInput' || row.eltype == 'TextareaInput')) {
					eltypeList = [{
							value: '出库方式',
							text: "出库方式"
						},
						{
							value: '领用人员',
							text: "领用人员"
						}
					]
				} else if (this.index == 2 && (row.eltype == 'TextInput' || row.eltype == 'TextareaInput')) {
					eltypeList = [{
						value: '申请单编号',
						text: "申请单编号"
					}]
				} else if (this.index == 0 && (row.eltype == 'TextInput' || row.eltype == 'TextareaInput')) {
					eltypeList = [{
						value: '入库方式',
						text: "入库方式"
					}]
				}
				return eltypeList
			},
			returnValueType(row) {
				if (row.eltype != 'DevicPicker' && row.eltype != 'TableList') {
					return [{
							value: 0,
							text: "自定义"
						},
						{
							value: 1,
							text: "系统值"
						},
					]
				} else if (row.eltype == 'DevicPicker') {
					return [{
						value: 1,
						text: "系统值"
					}]
				} else if (row.eltype == 'TableList') {
					return [{
						value: 0,
						text: "自定义"
					}]
				}
			},
			changeLoadVal(event, row, index) {
				//切换流程初始化默认值设置
				console.log(event, 'row.eltype', row.eltype);
				if (event == 1 && row.eltype == "TableList") {
					this.formInit[index].val = {}
				} else {
					this.formInit[index].val = ''
				}
				this.$forceUpdate()
			},
		},
	}
</script>

<style lang="less" scoped>
	.initialize_con {
		width: 100%;
		margin-bottom: 20px;

		.initialize_title {
			width: 100%;
			background-color: rgba(248, 248, 248, 1);
			height: 88rpx;
			border-radius: 10rpx;
			line-height: 88rpx;
			text-align: center;
			font-size: 32rpx;
		}

		.initialize_table {
			margin-top: 40rpx;

			.form_field {
				display: flex;
				align-items: center;
				font-size: 32rpx;

				.line {
					width: 6rpx;
					height: 28rpx;
					margin-right: 24rpx;
					background-color: rgba(35, 113, 255, 1);
				}
			}

			.title_text {
				color: rgba(153, 153, 153, 1);
				font-size: 28rpx;
				margin-top: 40rpx;
				margin-bottom: 20rpx;
			}
		}
	}

	::v-deep .table_con {
		//表格样式
		width: 100%;

		&.disable_table {
			.table_li {
				border: none;
				background-color: rgba(248, 248, 248, 1);
			}
		}

		.table_li {
			width: 100%;
			padding: 30rpx 30rpx 0 30rpx;
			box-sizing: border-box;
			border: 1rpx solid rgba(234, 234, 234, 1);
			border-radius: 10rpx;
			margin-bottom: 20rpx;
			position: relative;
		}

		.table_btn {
			display: flex;
			justify-content: center;
			align-items: center;
			border-top: 1rpx solid rgba(234, 234, 234, 1);

			.btn_li {
				width: calc(50% - 0.5rpx);
				display: flex;
				justify-content: center;
				align-items: center;
				color: rgba(153, 153, 153, 1);
				height: 88rpx;

				&.active {
					color: rgba(35, 113, 255, 1);
				}
			}

			.btn_line {
				width: 1rpx;
				height: 28rpx;
				background: rgba(234, 234, 234, 1);
			}
		}

		.sort_num {
			width: 50rpx;
			height: 40rpx;
			border-radius: 0 10rpx 0 10rpx;
			background-color: #E9F1FF;
			position: absolute;
			right: 0;
			top: 0;
			color: #2371FF;
			display: flex;
			align-items: center;
			justify-content: center;
			line-height: 40rpx;
			font-size: 26rpx;
		}
	}

	::v-deep .form_device_add {
		width: 100%;
		height: 88rpx;
		display: flex;
		justify-content: center;
		align-items: center;
		color: #333333;
		background-color: #F8F8F8;
		border-radius: 10rpx;

		.text {
			margin-left: 10rpx;
		}
	}
</style>