<template>
	<view style="width: 100%;">
		<view v-for="(rows, rsi) in __items" v-if="_items&&__items.length>0">
			<view v-for="(item, index) in rows" :key="item.name + index" v-if="rows&&rows.length>0">
				<uni-forms-item v-if="item.name !== 'SpanLayout' && item.name !== 'Description'" :label="item.title"
					required :name="item.id" :id="'form_'+item.id" labelFont="32rpx" contentFont="32rpx"
					:requireOpacity="0.5" labelPosition="top" :isDis="item.props&&item.props.disabled"
					:isfirstTop="index==0">
					<view :id="'li_'+item.id" class="form_li">
						<!-- <form-design-render :ref="`sub-item_${item.id}`" v-model="_value[item.id]" :valueModel="value"
							:mode="mode" :config="item" :keyId="item.id" @setActiveItem="setActiveItem"/> -->

						<TextInput :ref="`sub-item_${item.id}`" :is="item.name" :mode="mode" v-model="_value[item.id]"
							v-bind="item.props" :keyId="item.id" v-if="item&&item.name=='TextInput'">
						</TextInput>
						<NumberInput :ref="`sub-item_${item.id}`" :is="item.name" :mode="mode" v-model="_value[item.id]"
							v-bind="item.props" :keyId="item.id" v-if="item&&item.name=='NumberInput'">
						</NumberInput>
						<AmountInput :ref="`sub-item_${item.id}`" :is="item.name" :mode="mode" v-model="_value[item.id]"
							v-bind="item.props" :keyId="item.id" v-if="item&&item.name=='AmountInput'">
						</AmountInput>
						<TextareaInput :ref="`sub-item_${item.id}`" :is="item.name" :mode="mode"
							v-model="_value[item.id]" v-bind="item.props" :keyId="item.id"
							v-if="item&&item.name=='TextareaInput'"></TextareaInput>
						<SelectInput :ref="`sub-item_${item.id}`" :is="item.name" :mode="mode" v-model="_value[item.id]"
							v-bind="item.props" :keyId="item.id" v-if="item&&item.name=='SelectInput'">
						</SelectInput>
						<MultipleSelect :ref="`sub-item_${item.id}`" :is="item.name" :mode="mode"
							v-model="_value[item.id]" v-bind="item.props" :keyId="item.id"
							v-if="item&&item.name=='MultipleSelect'"></MultipleSelect>
						<DateTime :ref="`sub-item_${item.id}`" :is="item.name" :mode="mode" v-model="_value[item.id]"
							v-bind="item.props" :keyId="item.id" v-if="item&&item.name=='DateTime'">
						</DateTime>
						<DateTimeRange :ref="`sub-item_${item.id}`" :is="item.name" :mode="mode"
							v-model="_value[item.id]" v-bind="item.props" :keyId="item.id"
							v-if="item&&item.name=='DateTimeRange'"></DateTimeRange>
						<ImageUpload :ref="`sub-item_${item.id}`" :is="item.name" :mode="mode" v-model="_value[item.id]"
							v-bind="item.props" :keyId="item.id" v-if="item&&item.name=='ImageUpload'">
						</ImageUpload>
						<FileUpload :ref="`sub-item_${item.id}`" :is="item.name" :mode="mode" v-model="_value[item.id]"
							v-bind="item.props" :keyId="item.id" v-if="item&&item.name=='FileUpload'">
						</FileUpload>
						<Location :ref="`sub-item_${item.id}`" :is="item.name" :mode="mode" v-model="_value[item.id]"
							v-bind="item.props" :keyId="item.id" v-if="item&&item.name=='Location'">
						</Location>
						<DeptPicker :ref="`sub-item_${item.id}`" :is="item.name" :mode="mode" v-model="_value[item.id]"
							v-bind="item.props" :keyId="item.id" @setActiveItem="setActiveItem"
							v-if="item&&item.name=='DeptPicker'"></DeptPicker>
						<UserPicker :ref="`sub-item_${item.id}`" :is="item.name" :mode="mode" v-model="_value[item.id]"
							v-bind="item.props" :keyId="item.id" @setActiveItem="setActiveItem"
							v-if="item&&item.name=='UserPicker'"></UserPicker>
						<DevicPicker :ref="`sub-item_${item.id}`" :is="item.name" :mode="mode" v-model="_value[item.id]"
							v-bind="item.props" :keyId="item.id" @setActiveItem="setActiveItem"
							v-if="item&&item.name=='DevicPicker'"></DevicPicker>
						<SignPanel :ref="`sub-item_${item.id}`" :is="item.name" :mode="mode" v-model="_value[item.id]"
							v-bind="item.props" :keyId="item.id" @setActiveItem="setActiveItem"
							v-if="item&&item.name=='SignPanel'"></SignPanel>
						<TableList :ref="`sub-item_${item.id}`" :is="item.name" :mode="mode" v-model="_value[item.id]"
							v-bind="item.props" :keyId="item.id" @setActiveItem="setActiveItem"
							v-if="item&&item.name=='TableList'"></TableList>
						<ParamInput :ref="`sub-item_${item.id}`" :is="item.name" :mode="mode" v-model="_value[item.id]"
							v-bind="item.props" :keyId="item.id" @setActiveItem="setActiveItem"
							v-if="item&&item.name=='ParamInput'"></ParamInput>
					</view>
				</uni-forms-item>
				<view :id="'li_'+item.id" class="form_li" v-else>

					<Description :mode="mode" v-model="_value" v-bind="item.props" :keyId="item.id"
						v-if="item&&item.name=='Description'">
					</Description>
					<SpanLayout :mode="mode" v-model="_value" v-bind="item.props" :keyId="item.id"
						@setActiveItem="setActiveItem" v-if="item&&item.name=='SpanLayout'"></SpanLayout>
				</view>
			</view>
		</view>
	</view>
</template>

<script>
	import FormDesignRender from "../form-design-render2";
	import TextInput from './text-input.vue'
	import NumberInput from './number-input.vue'
	import AmountInput from './amount-input.vue'
	import TextareaInput from './textarea-input.vue'
	import SelectInput from './select-input.vue'
	import MultipleSelect from './multiple-select.vue'
	import DateTime from './DateTime.vue'
	import DateTimeRange from './DateTimeRange.vue'
	import Description from './Description.vue'
	import ImageUpload from './ImageUpload.vue'
	import FileUpload from './FileUpload.vue'
	import Location from './Location.vue'
	import DeptPicker from './DeptPicker.vue'
	import UserPicker from './UserPicker.vue'
	import DevicPicker from './DevicPicker.vue'
	import SignPanel from './SignPannel.vue'
	import SpanLayout from './SpanLayout.vue'
	import TableList from './TableList.vue'
	import ParamInput from './ParamInput.vue'
	import componentMinxins from "../ComponentMinxins";
	import "@/pages_flow/utlity.js";
	export default {
		mixins: [componentMinxins],
		name: "SpanLayout",
		components: {
			FormDesignRender,
			TextInput,
			NumberInput,
			AmountInput,
			TextareaInput,
			SelectInput,
			MultipleSelect,
			DateTime,
			DateTimeRange,
			Description,
			ImageUpload,
			FileUpload,
			Location,
			DeptPicker,
			UserPicker,
			DevicPicker,
			SignPanel,
			SpanLayout,
			TableList,
			ParamInput
		},
		props: {
			value: {
				default: null,
			},
			items: {
				type: Array,
				default: () => {
					return [];
				},
			},
			disabled: {
				default: false,
				type: Boolean,
			},
		},
		computed: {
			_items: {
				get() {
					return this.items;
				},
				set(val) {
					this.items = val;
				},
			},
			__items() {
				let result = [];
				for (let i = 0; i < this.items.length; i++) {
					if (i > 0 && i % 2 > 0) {
						result.push([this.items[i - 1], this.items[i]]);
					}
				}
				if (result.length * 2 < this.items.length) {
					result.push([this.items[this.items.length - 1]]);
				}
				return result;
			},
		},
		data() {
			return {
				select: null,
				drag: false,
				formConfig: {
					//数据字段
					data: {},
					//校验规则
					rules: {},
				},
				form: {
					formId: "",
					formName: "",
					logo: {},
					formItems: [],
					process: {},
					remark: "",
				},
			};
		},
		// beforeCreate: function () {
		//   this.$options.components.TableList = require('./TableList.vue').default
		// },
		methods: {
			setActiveItem(val) {
				this.$emit('setActiveItem', val)
			},
			selectDept(val, acitveId) {
				//选择部门
				this.$refs[`sub-item_${acitveId}`][0].selectDept(val, acitveId)
			},
			selectEmplee(val, acitveId) {
				//选择人员
				this.$refs[`sub-item_${acitveId}`][0].selectEmplee(val, acitveId)
			},
			selectDevice(val, acitveId) {
				//选择设备
				this.$refs[`sub-item_${acitveId}`][0].selectDevice(val, acitveId)
			},
		},
	}
</script>

<style lang="less" scoped>

</style>