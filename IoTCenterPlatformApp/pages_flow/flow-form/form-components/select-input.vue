<template>
	<view style="width: 100%;">
		<view style="width: 100%;" v-if="disabled">
			<view class="dis_text" v-if="!expanding||_value">
				<text>{{_value}}</text>
			</view>
			<uni-data-checkbox v-model="_value" :localdata="optionsBox" selectedColor="#2371FF" selectedTextColor="#333"
				custextColor="#333" disabledColor="#999999" noselectedColor="#999999" :isColumn="true" textFont="32rpx"
				textLeftMargin="16rpx" :disabled="disabled" v-else></uni-data-checkbox>
		</view>
		<view style="width: 100%;" v-else>
			<uni-data-select v-model="_value" :localdata="optionsBox" width="100%"
				:placeholder="placeholder?placeholder:'请选择选项'"
				:borderColor="disabled?'rgba(234, 234, 234, 1)':'#F8F8F8'" palColor="#c1c1c1" :isCustom="true"
				:isDark="false" v-if="!expanding" :disabled="disabled"></uni-data-select>
			<uni-data-checkbox v-model="_value" :localdata="optionsBox" selectedColor="#2371FF" selectedTextColor="#333"
				custextColor="#333" disabledColor="#999999" noselectedColor="#999999" :isColumn="true" textFont="32rpx"
				textLeftMargin="16rpx" :disabled="disabled" v-else></uni-data-checkbox>
		</view>
	</view>
</template>

<script>
	import componentMinxins from '../ComponentMinxins'

	export default {
		mixins: [componentMinxins],
		name: "SelectInput",
		components: {},
		props: {
			value: {
				type: String,
				default: null
			},
			placeholder: {
				type: String,
				default: '请选择选项'
			},
			expanding: {
				type: Boolean,
				default: false
			},
			options: {
				type: Array,
				default: () => {
					return []
				}
			},
			disabled: {
				default: false,
				type: Boolean,
			},
		},
		data() {
			return {}
		},
		computed: {
			optionsBox() {
				let arr = this.options.map(row => {
					return {
						text: row,
						value: row
					}
				})
				return arr;
			},
		},
		methods: {}
	}
</script>

<style scoped>

</style>