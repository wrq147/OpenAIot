<template>
	<view style="width: 100%;">
		<uni-easyinput placeholderStyle="color:rgba(255, 255, 255, 0.2);font-size:32rpx" inputHeight="70rpx"
			:styles="styles" type="textarea" v-model="_value" :placeholder="placeholder?placeholder:'Please enter the content'" contentFontSize="32rpx"
			primaryColor="rgba(255, 255, 255, 0.5)" autoHeight :maxlength="255" :disabled="disabled" />
	</view>
</template>

<script>
	import componentMinxins from '../ComponentMinxins'

	export default {
		mixins: [componentMinxins],
		name: "TextareaInput",
		components: {},
		props: {
			value: {
				type: String,
				default: null
			},
			placeholder: {
				type: String,
				default: 'Please enter the content'
			},
			disabled: {
				default: false,
				type: Boolean,
			},
		},
		data() {
			return {}
		},
		methods: {}
	}
</script>

<style scoped>

</style>