<template>
	<view style="width: 100%;">
		<!-- <component :ref="'form'+keyId" :is="config.name" :mode="mode" v-model="_value" :valueModel="valueModel"
			v-bind="config.props" :keyId="keyId" @setActiveItem="setActiveItem"/> -->
			<TextInput :ref="'form'+keyId" :is="config.name" :mode="mode" v-model="_value" :valueModel="valueModel"
			v-bind="config.props" :keyId="keyId" v-if="config&&config.name=='TextInput'"></TextInput>
			<NumberInput :ref="'form'+keyId" :is="config.name" :mode="mode" v-model="_value" :valueModel="valueModel"
			v-bind="config.props" :keyId="keyId" v-if="config&&config.name=='NumberInput'"></NumberInput>
			<AmountInput :ref="'form'+keyId" :is="config.name" :mode="mode" v-model="_value" :valueModel="valueModel"
			v-bind="config.props" :keyId="keyId" v-if="config&&config.name=='AmountInput'"></AmountInput>
			<TextareaInput :ref="'form'+keyId" :is="config.name" :mode="mode" v-model="_value" :valueModel="valueModel"
			v-bind="config.props" :keyId="keyId" v-if="config&&config.name=='TextareaInput'"></TextareaInput>
			<SelectInput :ref="'form'+keyId" :is="config.name" :mode="mode" v-model="_value" :valueModel="valueModel"
			v-bind="config.props" :keyId="keyId" v-if="config&&config.name=='SelectInput'"></SelectInput>
			<MultipleSelect :ref="'form'+keyId" :is="config.name" :mode="mode" v-model="_value" :valueModel="valueModel"
			v-bind="config.props" :keyId="keyId" v-if="config&&config.name=='MultipleSelect'"></MultipleSelect>
			<DateTime :ref="'form'+keyId" :is="config.name" :mode="mode" v-model="_value" :valueModel="valueModel"
			v-bind="config.props" :keyId="keyId" v-if="config&&config.name=='DateTime'"></DateTime>
			<DateTimeRange :ref="'form'+keyId" :is="config.name" :mode="mode" v-model="_value" :valueModel="valueModel"
			v-bind="config.props" :keyId="keyId" v-if="config&&config.name=='DateTimeRange'"></DateTimeRange>
			<Description :ref="'form'+keyId" :is="config.name" :mode="mode" v-model="_value" :valueModel="valueModel"
			v-bind="config.props" :keyId="keyId" v-if="config&&config.name=='Description'"></Description>
			<ImageUpload :ref="'form'+keyId" :is="config.name" :mode="mode" v-model="_value" :valueModel="valueModel"
			v-bind="config.props" :keyId="keyId" v-if="config&&config.name=='ImageUpload'"></ImageUpload>
			<FileUpload :ref="'form'+keyId" :is="config.name" :mode="mode" v-model="_value" :valueModel="valueModel"
			v-bind="config.props" :keyId="keyId" v-if="config&&config.name=='FileUpload'"></FileUpload>
			<Location :ref="'form'+keyId" :is="config.name" :mode="mode" v-model="_value" :valueModel="valueModel"
			v-bind="config.props" :keyId="keyId" v-if="config&&config.name=='Location'"></Location>
			<DeptPicker :ref="'form'+keyId" :is="config.name" :mode="mode" v-model="_value" :valueModel="valueModel"
			v-bind="config.props" :keyId="keyId" @setActiveItem="setActiveItem" v-if="config&&config.name=='DeptPicker'"></DeptPicker>
			<UserPicker :ref="'form'+keyId" :is="config.name" :mode="mode" v-model="_value" :valueModel="valueModel"
			v-bind="config.props" :keyId="keyId" @setActiveItem="setActiveItem" v-if="config&&config.name=='UserPicker'"></UserPicker>
			<DevicPicker :ref="'form'+keyId" :is="config.name" :mode="mode" v-model="_value" :valueModel="valueModel"
			v-bind="config.props" :keyId="keyId" @setActiveItem="setActiveItem" v-if="config&&config.name=='DevicPicker'"></DevicPicker>
			<SignPanel :ref="'form'+keyId" :is="config.name" :mode="mode" v-model="_value" :valueModel="valueModel"
			v-bind="config.props" :keyId="keyId" @setActiveItem="setActiveItem" v-if="config&&config.name=='SignPanel'"></SignPanel>
			<SpanLayout :ref="'form'+keyId" :is="config.name" :mode="mode" v-model="_value" :valueModel="valueModel"
			v-bind="config.props" :keyId="keyId" @setActiveItem="setActiveItem" v-if="config&&config.name=='SpanLayout'"></SpanLayout>
			<TableList :ref="'form'+keyId" :is="config.name" :mode="mode" v-model="_value" :valueModel="valueModel"
			v-bind="config.props" :keyId="keyId" @setActiveItem="setActiveItem" v-if="config&&config.name=='TableList'"></TableList>
			<ParamInput :ref="'form'+keyId" :is="config.name" :mode="mode" v-model="_value" :valueModel="valueModel"
			v-bind="config.props" :keyId="keyId" @setActiveItem="setActiveItem" v-if="config&&config.name=='ParamInput'"></ParamInput>
			
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
		name: "formDesignRender2",
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
				type: [String,Number],
				default: ''
			},
		},
		computed: {
			_value: {
				get() {
					return this.value;
				},
				set(val) {
					this.$emit("input", val);
				}
			}
		},
		mounted() {
			console.log(this.config,'11111configconfigconfigconfig');
		},
		data() {
			return {}
		},
		methods: {
			validate(call) {
				return this.$refs['form'+this.keyId].validate()
			},
			setActiveItem(val){
				this.$emit('setActiveItem',val)
			},
			selectDept(val,acitveId){
				//选择部门
				this.$refs[`form${acitveId}`].selectDept(val,acitveId)
			},
			selectEmplee(val,acitveId){
				//选择人员
				this.$refs[`form${acitveId}`].selectEmplee(val,acitveId)
			},
			selectDevice(val,acitveId){
				//选择设备
				this.$refs[`form${acitveId}`].selectDevice(val,acitveId)
			},
		}
	}
</script>

<style lang="less" scoped>

</style>