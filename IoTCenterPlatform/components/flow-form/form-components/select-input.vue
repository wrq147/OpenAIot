<template>
	<view style="width: 100%;">
		<uni-data-select v-model="_value" :localdata="optionsBox" width="100%" :placeholder="placeholder?placeholder:'Please select an option'"
			borderColor="rgba(255, 255, 255, 0.20)" palColor="rgba(255, 255, 255, 0.2)" :isCustom="true" :isDark="true"
			v-if="!expanding" :disabled="disabled"></uni-data-select>

		<uni-data-checkbox v-model="_value" :localdata="optionsBox" selectedColor="#FF3535" selectedTextColor="#ffffff"
			custextColor="#ffffff" disabledColor="rgba(255, 255, 255, 0.5)" noselectedColor="rgba(255, 255, 255, 0.5)"
			:isColumn="true" textFont="32rpx" textLeftMargin="16rpx" :disabled="disabled" v-else></uni-data-checkbox>
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
				default: 'Please select an option'
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