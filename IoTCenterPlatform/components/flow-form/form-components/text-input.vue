<template>
	<view style="width: 100%;">
		<uni-easyinput placeholderStyle="color:rgba(255, 255, 255, 0.2);font-size:32rpx" inputHeight="88rpx"
			:styles="styles" type="text" v-model="_value" :placeholder="placeholder" contentFontSize="32rpx"
			primaryColor="rgba(255, 255, 255, 0.5)" :disabled="disabled" :clearable="!disabled" />
	</view>
</template>

<script>
	import componentMinxins from "../ComponentMinxins";
	import { GenerateNumber } from "@/api/process";
	export default {
		mixins: [componentMinxins],
		name: "TextInput",
		components: {},
		props: {
			value: {
				type: String,
				default: null,
			},
			placeholder: {
				type: String,
				default: "请输入内容",
			},
			defaultValue: {
				type: String,
				default: "",
			},
			disabled: {
				default: false,
				type: Boolean,
			},
		},
		data() {
			return {};
		},
		async mounted() {
			if (this.mode != 'DESIGN') {
				if (this.value == null) {
					if (this.defaultValue == "@FlowNumber") {
						//生成工单号
						let rsp = await GenerateNumber();
						this.$emit('input', rsp.data);
						this.valueModel["@FlowNumber"] = rsp.data;
					} else {
						this.$emit('input', this.defaultValue);
					}

				}
			}

		},
		methods: {},
	};
</script>

<style scoped>
</style>