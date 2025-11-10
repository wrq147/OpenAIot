<template>
	<view style="width: 100%;">
		<!-- <component :ref="'form'+keyId" :is="config.name" :mode="mode" :value="_value" :valueModel="valueModel"
			v-bind="config.props" :keyId="keyId" @setActiveItem="setActiveItem"/> -->
		<TextInput @update:value="updateval" style="width: 100%;" :ref="'form'+keyId" :mode="mode" :value="_value" :valueModel="valueModel" :keyId="keyId"
			v-if="config&&config.name=='TextInput'" :disabled="disabled" :enablePrint="config.props.enablePrint"
			:defaultValue="config.props.defaultValue" :required="config.props.required"></TextInput>
		<NumberInput @update:value="updateval" style="width: 100%;" :ref="'form'+keyId" :mode="mode" :value="_value" :valueModel="valueModel" :keyId="keyId"
			v-if="config&&config.name=='NumberInput'" :disabled="disabled" :min="config.props.min"
			:max="config.props.max" :precision="config.props.precision" :enablePrint="config.props.enablePrint"
			:required="config.props.required"></NumberInput>
		<AmountInput @update:value="updateval" style="width: 100%;" :ref="'form'+keyId" :mode="mode" :value="_value" :valueModel="valueModel" :keyId="keyId"
			v-if="config&&config.name=='AmountInput'" :disabled="disabled" :showChinese="config.props.showChinese"
			:enablePrint="config.props.enablePrint" :required="config.props.required"></AmountInput>
		<TextareaInput @update:value="updateval" style="width: 100%;" :ref="'form'+keyId" :mode="mode" :value="_value" :valueModel="valueModel" :keyId="keyId"
			v-if="config&&config.name=='TextareaInput'" :disabled="disabled" :enablePrint="config.props.enablePrint"
			:required="config.props.required"></TextareaInput>
		<SelectInput @update:value="updateval" style="width: 100%;" :ref="'form'+keyId" :mode="mode" :value="_value" :valueModel="valueModel"
			:options="config.props.options" :expanding="config.props.expanding" :enablePrint="config.props.enablePrint"
			:required="config.props.required" :keyId="keyId" v-if="config&&config.name=='SelectInput'"
			:disabled="disabled"></SelectInput>
		<MultipleSelect @update:value="updateval" style="width: 100%;" :ref="'form'+keyId" :mode="mode" :value="_value" :valueModel="valueModel"
			:options="config.props.options" :expanding="config.props.expanding" :enablePrint="config.props.enablePrint"
			:required="config.props.required" :keyId="keyId" v-if="config&&config.name=='MultipleSelect'"
			:disabled="disabled"></MultipleSelect>
		<DateTime @update:value="updateval" style="width: 100%;" :ref="'form'+keyId" :mode="mode" :value="_value" :valueModel="valueModel"
			:format="config.props.format" :enablePrint="config.props.enablePrint" :required="config.props.required"
			:keyId="keyId" v-if="config&&config.name=='DateTime'" :disabled="disabled"></DateTime>
		<DateTimeRange @update:value="updateval" style="width: 100%;" :ref="'form'+keyId" :mode="mode" :value="_value" :valueModel="valueModel"
			:showLength="config.props.showLength" :placeholder="config.props.placeholder" :format="config.props.format"
			:enablePrint="config.props.enablePrint" :required="config.props.required" :keyId="keyId"
			v-if="config&&config.name=='DateTimeRange'" :disabled="disabled"></DateTimeRange>
		<Description @update:value="updateval" style="width: 100%;" :ref="'form'+keyId" :mode="mode" :value="_value" :valueModel="valueModel"
			:enablePrint="config.props.enablePrint" :required="config.props.required" :keyId="keyId"
			v-if="config&&config.name=='Description'" :disabled="disabled"></Description>
		<ImageUpload @update:value="updateval" style="width: 100%;" :ref="'form'+keyId" :mode="mode" :value="_value" :valueModel="valueModel"
			:enableZip="config.props.enableZip" :maxNumber="config.props.maxNumber" :maxSize="config.props.maxSize"
			:enablePrint="config.props.enablePrint" :required="config.props.required" :keyId="keyId"
			v-if="config&&config.name=='ImageUpload'" :disabled="disabled"></ImageUpload>
		<FileUpload @update:value="updateval" style="width: 100%;" :ref="'form'+keyId" :mode="mode" :value="_value" :valueModel="valueModel"
			:fileTypes="config.props.fileTypes" :onlyRead="config.props.onlyRead" :maxNumber="config.props.maxNumber"
			:maxSize="config.props.maxSize" :enablePrint="config.props.enablePrint" :required="config.props.required"
			:keyId="keyId" v-if="config&&config.name=='FileUpload'" :disabled="disabled"></FileUpload>
		<Location @update:value="updateval" style="width: 100%;" :ref="'form'+keyId" :mode="mode" :value="_value" :valueModel="valueModel" :keyId="keyId"
			v-if="config&&config.name=='Location'" :disabled="disabled"></Location>
		<DeptPicker @update:value="updateval" style="width: 100%;" :ref="'form'+keyId" :mode="mode" :value="_value" :valueModel="valueModel"
			:multiple="config.props.multiple" :enablePrint="config.props.enablePrint" :required="config.props.required"
			:keyId="keyId" @setActiveItem="setActiveItem" v-if="config&&config.name=='DeptPicker'" :disabled="disabled">
		</DeptPicker>
		<UserPicker @update:value="updateval" style="width: 100%;" :ref="'form'+keyId" :mode="mode" :value="_value" :valueModel="valueModel"
			:multiple="config.props.multiple" :enablePrint="config.props.enablePrint" :required="config.props.required"
			:keyId="keyId" @setActiveItem="setActiveItem" v-if="config&&config.name=='UserPicker'" :disabled="disabled">
		</UserPicker>
		<DevicPicker @update:value="updateval" style="width: 100%;" :ref="'form'+keyId" :mode="mode" :value="_value" :valueModel="valueModel"
			:synclist="config.props.synclist" :limit_product="config.props.limit_product" :limit="config.props.limit"
			:enablePrint="config.props.enablePrint" :required="config.props.required" :keyId="keyId"
			@setActiveItem="setActiveItem" v-if="config&&config.name=='DevicPicker'" :disabled="disabled"></DevicPicker>
		<SignPanel @update:value="updateval" style="width: 100%;" :ref="'form'+keyId" :mode="mode" :value="_value" :valueModel="valueModel" :keyId="keyId"
			@setActiveItem="setActiveItem" v-if="config&&config.name=='SignPanel'" :disabled="disabled"></SignPanel>
		<SpanLayout @update:value="updateval" style="width: 100%;" :ref="'form'+keyId" :mode="mode" :value="_value" :valueModel="valueModel" :keyId="keyId"
			@setActiveItem="setActiveItem" v-if="config&&config.name=='SpanLayout'"
			:isShowFirshBorder="isShowFirshBorder" :disabled="disabled" :items="config.props.items"></SpanLayout>
		<TableList @update:value="updateval" style="width: 100%;" :ref="'form'+keyId" :mode="mode" :value="_value" :valueModel="valueModel"
			:columns="config.props.columns" :maxSize="config.props.maxSize" :summaryUnit="config.props.summaryUnit"
			:summaryColumns="config.props.summaryColumns" :deductid="config.props.deductid"
			:showSummary="config.props.showSummary" :IdxColName="config.props.IdxColName"
			:rowLayout="config.props.rowLayout" :showBorder="config.props.showBorder"
			:enablePrint="config.props.enablePrint" :required="config.props.required" :keyId="keyId"
			@setActiveItem="setActiveItem" v-if="config&&config.name=='TableList'" :disabled="disabled"></TableList>
		<ParamInput @update:value="updateval" style="width: 100%;" :ref="'form'+keyId" :mode="mode" :value="_value" :valueModel="valueModel"
			:formId="config.props.formId" :formType="config.props.formType" :enablePrint="config.props.enablePrint"
			:required="config.props.required" :keyId="keyId" @setActiveItem="setActiveItem"
			v-if="config&&config.name=='ParamInput'" :disabled="disabled"></ParamInput>

	</view>
</template>
<script>
	// import components from './ComponentExport'
	import TextInput from './form-components/text-input.vue'
	import NumberInput from './form-components/number-input.vue'
	import AmountInput from './form-components/amount-input.vue'
	import TextareaInput from './form-components/textarea-input.vue'
	import SelectInput from './form-components/select-input.vue'
	import MultipleSelect from './form-components/multiple-select.vue'
	import DateTime from './form-components/DateTime.vue'
	import DateTimeRange from './form-components/DateTimeRange.vue'
	import Description from './form-components/Description.vue'
	import ImageUpload from './form-components/ImageUpload.vue'
	import FileUpload from './form-components/FileUpload.vue'
	import Location from './form-components/Location.vue'
	import DeptPicker from './form-components/DeptPicker.vue'
	import UserPicker from './form-components/UserPicker.vue'
	import DevicPicker from './form-components/DevicPicker.vue'
	import SignPanel from './form-components/SignPannel.vue'
	import SpanLayout from './form-components/SpanLayout.vue'
	import TableList from './form-components/TableList.vue'
	import ParamInput from './form-components/ParamInput.vue'
	export default {
		name: "formDesignRender",
		components: {
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
			isShowFirshBorder: {
				type: Boolean,
				default: false
			},
			mode: {
				type: String,
				default: 'DESIGN'
			},
			value: {
				default: undefined
			},
			valueModel: {
				type: Object,
				default: () => {
					return {}
				}
			},
			config: {
				type: Object,
				default: () => {
					return {}
				}
			},
			keyId: {
				type: [String, Number],
				default: ''
			},
			disabled: {
				default: false,
				type: Boolean,
			},
		},
		computed: {
			_value: {
				get() {
					return this.value;
				},
				set(val) {
					// this.$emit('update:value',val)
					// this.$emit("input", val);
				}
			}
		},
		mounted() {
			// console.log(this.config, 'config');
		},
		data() {
			return {}
		},
		methods: {
			updateval(val){
				this.$emit('update:value',val)
				// this.$emit("input", val);
			},
			validate(call) {
				return this.$refs['form' + this.keyId].validate()
			},
			setActiveItem(val) {
				this.$emit('setActiveItem', val)
			},
			selectDept(val, acitveId) {
				//选择部门
				if (this.$refs[`form${acitveId}`]) {
					this.$refs[`form${acitveId}`].selectDept(val, acitveId)
				} else {

				}

			},
			selectEmplee(val, acitveId) {
				//选择人员
				if (this.$refs[`form${acitveId}`]) {
					this.$refs[`form${acitveId}`].selectEmplee(val, acitveId)
				}

			},
			selectDevice(val, acitveId) {
				//选择设备
				if (this.$refs[`form${acitveId}`]) {
					this.$refs[`form${acitveId}`].selectDevice(val, acitveId)
				}

			},
		}
	}
</script>

<style lang="less" scoped>

</style>