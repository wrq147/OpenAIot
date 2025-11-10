<template>
	<view style="width: 100%;">
		<view v-for="(rows, rsi) in t_items" v-if="t_items&&t_items.length>0">
			<view v-for="(item, index) in rows" :key="item.name + index" v-if="rows&&rows.length>0">
				<!-- :id="'form_'+item.id" -->
				<uni-forms-item v-if="item.name !== 'SpanLayout'" :label="item.title"
					:required="item.props&&item.props.required" :name="item.id" labelFont="28rpx" contentFont="32rpx"
					:requireOpacity="0.5" labelPosition="top" :isDis="item.props&&item.props.disabled"
					:isfirstTop="isShowFirshBorder&&index==0">
					<view :id="'li_'+item.id" class="form_li">
						<!-- <form-design-render :ref="`sub-item_${item.id}`" :value="_value[t_items[rsi][index].id]" :valueModel="value"
							:mode="mode" :item="item" :keyId="item.id" @setActiveItem="setActiveItem"/> -->

						<TextInput @update:value="updateval($event,t_items[rsi][index].id,t_items[rsi][index])"
							style="width: 100%;" :ref="`sub-item_${item.id}`" :mode="mode"
							:value="_value[t_items[rsi][index].id]" :enablePrint="item.props.enablePrint"
							:defaultValue="item.props.defaultValue" :required="item.props.required"
							:keyId="t_items[rsi][index].id" v-if="item&&item.name=='TextInput'">
						</TextInput>
						<NumberInput @update:value="updateval($event,t_items[rsi][index].id,t_items[rsi][index])"
							style="width: 100%;" :ref="`sub-item_${item.id}`" :mode="mode"
							:value="_value[t_items[rsi][index].id]" :min="item.props.min" :max="item.props.max"
							:precision="item.props.precision" :enablePrint="item.props.enablePrint"
							:required="item.props.required" :keyId="item.id" v-if="item&&item.name=='NumberInput'">
						</NumberInput>
						<AmountInput @update:value="updateval($event,t_items[rsi][index].id,t_items[rsi][index])"
							style="width: 100%;" :ref="`sub-item_${item.id}`" :mode="mode"
							:value="_value[t_items[rsi][index].id]" :showChinese="item.props.showChinese"
							:enablePrint="item.props.enablePrint" :required="item.props.required" :keyId="item.id"
							v-if="item&&item.name=='AmountInput'">
						</AmountInput>
						<TextareaInput @update:value="updateval($event,t_items[rsi][index].id,t_items[rsi][index])"
							style="width: 100%;" :ref="`sub-item_${item.id}`" :mode="mode"
							:value="_value[t_items[rsi][index].id]" :enablePrint="item.props.enablePrint"
							:required="item.props.required" :keyId="item.id" v-if="item&&item.name=='TextareaInput'">
						</TextareaInput>
						<SelectInput @update:value="updateval($event,t_items[rsi][index].id,t_items[rsi][index])"
							style="width: 100%;" :ref="`sub-item_${item.id}`" :mode="mode"
							:value="_value[t_items[rsi][index].id]" :options="item.props.options"
							:expanding="item.props.expanding" :enablePrint="item.props.enablePrint"
							:required="item.props.required" :keyId="item.id" v-if="item&&item.name=='SelectInput'">
						</SelectInput>
						<MultipleSelect @update:value="updateval($event,t_items[rsi][index].id,t_items[rsi][index])"
							style="width: 100%;" :ref="`sub-item_${item.id}`" :mode="mode"
							:value="_value[t_items[rsi][index].id]" :options="item.props.options"
							:expanding="item.props.expanding" :enablePrint="item.props.enablePrint"
							:required="item.props.required" :keyId="item.id" v-if="item&&item.name=='MultipleSelect'">
						</MultipleSelect>
						<DateTime @update:value="updateval($event,t_items[rsi][index].id,t_items[rsi][index])"
							style="width: 100%;" :ref="`sub-item_${item.id}`" :mode="mode"
							:value="_value[t_items[rsi][index].id]" :format="item.props.format"
							:enablePrint="item.props.enablePrint" :required="item.props.required" :keyId="item.id"
							v-if="item&&item.name=='DateTime'">
						</DateTime>
						<DateTimeRange @update:value="updateval($event,t_items[rsi][index].id,t_items[rsi][index])"
							style="width: 100%;" :ref="`sub-item_${item.id}`" :mode="mode"
							:value="_value[t_items[rsi][index].id]" :showLength="item.props.showLength"
							:placeholder="item.props.placeholder" :format="item.props.format"
							:enablePrint="item.props.enablePrint" :required="item.props.required" :keyId="item.id"
							v-if="item&&item.name=='DateTimeRange'">
						</DateTimeRange>
						<ImageUpload @update:value="updateval($event,t_items[rsi][index].id,t_items[rsi][index])"
							style="width: 100%;" :ref="`sub-item_${item.id}`" :mode="mode"
							:value="_value[t_items[rsi][index].id]" :enableZip="item.props.enableZip"
							:maxNumber="item.props.maxNumber" :maxSize="item.props.maxSize"
							:enablePrint="item.props.enablePrint" :required="item.props.required" :keyId="item.id"
							v-if="item&&item.name=='ImageUpload'">
						</ImageUpload>
						<FileUpload @update:value="updateval($event,t_items[rsi][index].id,t_items[rsi][index])"
							style="width: 100%;" :ref="`sub-item_${item.id}`" :mode="mode"
							:value="_value[t_items[rsi][index].id]" :fileTypes="item.props.fileTypes"
							:onlyRead="item.props.onlyRead" :maxNumber="item.props.maxNumber"
							:maxSize="item.props.maxSize" :enablePrint="item.props.enablePrint"
							:required="item.props.required" :keyId="item.id" v-if="item&&item.name=='FileUpload'">
						</FileUpload>
						<Location @update:value="updateval($event,t_items[rsi][index].id,t_items[rsi][index])"
							style="width: 100%;" :ref="`sub-item_${item.id}`" :mode="mode"
							:value="_value[t_items[rsi][index].id]" :keyId="item.id" v-if="item&&item.name=='Location'">
						</Location>
						<DeptPicker @update:value="updateval($event,t_items[rsi][index].id,t_items[rsi][index])"
							style="width: 100%;" :ref="`sub-item_${item.id}`" :mode="mode"
							:value="_value[t_items[rsi][index].id]" :multiple="item.props.multiple"
							:enablePrint="item.props.enablePrint" :required="item.props.required" :keyId="item.id"
							@setActiveItem="setActiveItem" v-if="item&&item.name=='DeptPicker'"></DeptPicker>
						<UserPicker @update:value="updateval($event,t_items[rsi][index].id,t_items[rsi][index])"
							style="width: 100%;" :ref="`sub-item_${item.id}`" :mode="mode"
							:value="_value[t_items[rsi][index].id]" :multiple="item.props.multiple"
							:enablePrint="item.props.enablePrint" :required="item.props.required" :keyId="item.id"
							@setActiveItem="setActiveItem" v-if="item&&item.name=='UserPicker'"></UserPicker>
						<DevicPicker @update:value="updateval($event,t_items[rsi][index].id,t_items[rsi][index])"
							style="width: 100%;" :ref="`sub-item_${item.id}`" :mode="mode"
							:value="_value[t_items[rsi][index].id]" :synclist="item.props.synclist"
							:limit_product="item.props.limit_product" :limit="item.props.limit"
							:enablePrint="item.props.enablePrint" :required="item.props.required" :keyId="item.id"
							@setActiveItem="setActiveItem" v-if="item&&item.name=='DevicPicker'"></DevicPicker>
						<SignPanel @update:value="updateval($event,t_items[rsi][index].id,t_items[rsi][index])"
							style="width: 100%;" :ref="`sub-item_${item.id}`" :mode="mode"
							:value="_value[t_items[rsi][index].id]" :keyId="item.id" @setActiveItem="setActiveItem"
							v-if="item&&item.name=='SignPanel'">
						</SignPanel>
						<TableList @update:value="updateval($event,t_items[rsi][index].id,t_items[rsi][index])"
							style="width: 100%;" :ref="`sub-item_${item.id}`" :mode="mode"
							:value="_value[t_items[rsi][index].id]" :columns="item.props.columns"
							:maxSize="item.props.maxSize" :summaryUnit="item.props.summaryUnit"
							:summaryColumns="item.props.summaryColumns" :deductid="item.props.deductid"
							:showSummary="item.props.showSummary" :IdxColName="item.props.IdxColName"
							:rowLayout="item.props.rowLayout" :showBorder="item.props.showBorder"
							:enablePrint="item.props.enablePrint" :required="item.props.required" :keyId="item.id"
							@setActiveItem="setActiveItem" v-if="item&&item.name=='TableList'"></TableList>
						<ParamInput @update:value="updateval($event,t_items[rsi][index].id,t_items[rsi][index])"
							style="width: 100%;" :ref="`sub-item_${item.id}`" :mode="mode"
							:value="_value[t_items[rsi][index].id]" :formId="item.props.formId"
							:formType="item.props.formType" :enablePrint="item.props.enablePrint"
							:required="item.props.required" :keyId="item.id" @setActiveItem="setActiveItem"
							v-if="item&&item.name=='ParamInput'"></ParamInput>
						<Description @update:value="updateval($event,t_items[rsi][index].id,t_items[rsi][index])"
							style="width: 100%;" :mode="mode" :value="_value[t_items[rsi][index].id]"
							:enablePrint="item.props.enablePrint" :required="item.props.required" :keyId="item.id"
							v-if="item&&item.name=='Description'">
						</Description>
					</view>
				</uni-forms-item>
				<view :id="'li_'+item.id" class="form_li" v-else>
					<SpanLayout :name="item.name" @update:value="updateval2" style="width: 100%;" :mode="mode"
						:value="_value" :items="item.props.items" :keyId="item.id" @setActiveItem="setActiveItem"
						v-if="item&&item.name=='SpanLayout'" :isShowFirshBorder="isShowFirshBorder&&index==0">
					</SpanLayout>
				</view>
			</view>
		</view>
	</view>
</template>

<script>
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
			// FormDesignRender,
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
			// _items: {
			// 	get() {
			// 		return this.items;
			// 	},
			// 	set(val) {
			// 		this.items = val;
			// 	},
			// },

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
				t_items: []
			};
		},
		mounted() {
			this.returnItems()
		},
		// beforeCreate: function () {
		//   this.$options.components.TableList = require('./TableList.vue').default
		// },
		methods: {
			returnItems() {
				this.t_items = [];
				for (let i = 0; i < this.items.length; i++) {
					if (i > 0 && i % 2 > 0) {
						this.t_items.push([this.items[i - 1], this.items[i]]);
					}
				}
				if (this.t_items.length * 2 < this.items.length) {
					this.t_items.push([this.items[this.items.length - 1]]);
				}
				// return this.t_items;
			},
			updateval(val, id, item) {
				this._value[id] = val
				this.$emit('update:value', this._value)
				this.$emit("input", this._value);
			},
			updateval2(val) {
				// console.log("span更新到上一级",val);
				this.$emit('update:value', val)
				this.$emit("input", val);
			},
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