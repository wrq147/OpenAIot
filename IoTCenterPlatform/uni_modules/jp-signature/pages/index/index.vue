<template>
	<view class="h100">
		<jp-signature-popup v-model="image"  />
		<view>{{image}}</view>
		<view class="but" @click="toPop">自定义样式，弹框调用</view>
		<jp-signature-popup ref="signature" popup v-model="image2" />
		{{image2}}
	</view>
</template>
<script>
	export default {
		data() {
			return {
				image:'',
				image2:''
			}
		},
		methods: {
			toPop(){
				this.$refs.signature.toPop()
			}
		}
	}
</script>

<style lang="scss">
	.h100{
		height: 1150vh;
	}
	.but{
		margin: 25px;
		line-height: 40px;
		text-align: center;
		background-color: #55aaff;
		color: #fff;
	}
</style>
