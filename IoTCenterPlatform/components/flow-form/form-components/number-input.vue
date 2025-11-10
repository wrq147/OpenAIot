<template>
	<view style="width: 100%;">
		<!-- <el-input-number style="width: 100%" :disabled="disabled" v-model="_value" size="medium" :placeholder="placeholder" :min="min" :max="max"></el-input-number> -->
		<!-- <input class="li_input input" type="digit" :placeholder="placeholder"
				placeholderStyle="color:rgba(255, 255, 255, 0.2);font-size:32rpx" v-model="_value"
				@input="filterValue2($event,precision)" :disabled="disabled" :maxlength="max.toString().length"/>
				 -->
		<uni-easyinput placeholderStyle="color:rgba(255, 255, 255, 0.2);font-size:32rpx" inputHeight="88rpx"
			:styles="styles" type="number" v-model="_value" :placeholder="placeholder" contentFontSize="32rpx"
			primaryColor="rgba(255, 255, 255, 0.5)" @input="filterValue2($event,precision)" :disabled="disabled" />
		<view class="num_cli">
			<view class="cli_up" @click.stop="numUp()">
				<custom-icons iconsName="icon-shuzijiajiansanjiao" iconsSize="14rpx"
					:iconsColor="!disabled&&_value<max?'rgba(255, 255, 255, 0.5)':'rgba(255, 255, 255, 0.2)'"></custom-icons>
			</view>
			<view class="cli_up rotate-180" @click.stop="numDown()">
				<custom-icons iconsName="icon-shuzijiajiansanjiao" iconsSize="14rpx"
					:iconsColor="!disabled&&_value>min?'rgba(255, 255, 255, 0.5)':'rgba(255, 255, 255, 0.2)'"></custom-icons>
			</view>
		</view>
	</view>
</template>

<script>
	import componentMinxins from '../ComponentMinxins'

	export default {
		mixins: [componentMinxins],
		name: "NumberInput",
		components: {},
		props: {
			value: {
				default: null,
			},
			placeholder: {
				type: String,
				default: '请输入数值'
			},
			disabled: {
				default: false,
				type: Boolean,
			},
			min: {
				type: Number,
				default: 0
			},
			max: {
				type: Number,
				default: 100
			},
			precision: {
				type: Number,
				default: 0
			},
		},
		data() {
			return {}
		},
		beforeMount() {
			console.log(this.value,'this.valuethis.valuethis.valuethis.value');
			if(!this.value||this.value==null){
				this._value=0
			}
		},
		methods: {
			filterValue2(input, n) {
				this.$nextTick(() => {
					let value = this._value
					if (Number(value) >= this.max) {
						this._value = Number(this.max)
						console.log("yyyyuuuu", this._value, this.max);
						this.$forceUpdate()
						return
					}
					if (Number(value) <= this.min) {
						this._value = Number(this.min)
						this.$forceUpdate()
						return
					}
					console.log("222222222222");
					if (n > 0) { //实现保留指定小数位
						value = value.replace(/[^\d.]/g, '');
						value = value.replace(/^\./g, '');
						value = value.replace('.', '$#$').replace(/\./g, '').replace('$#$', '.');
						if (n && Number(n) > 0) { //限制n位
							var d = new Array(Number(n)).fill(`\\d`).join('');
							var reg = new RegExp(`^(\\-)*(\\d+)\\.(${d}).*$`, 'ig');
							value = value.replace(reg, '$1$2.$3')
						}
						if (value && !value.includes('.')) {
							value = Number(value).toString() //去掉开头多个0
						}
						// this.$nextTick(() => {
						this._value = value
						// })
					} else { //不保留小数
						value = value.replace(/\D+/g, '');
						value = value ? Number(value).toString() : value //去掉开头多个0
						// this.$nextTick(() => {
						this._value = Number(value)
						// })
					}
				})

			},
			numUp() {
				if (!this.disabled) {
					// this.$nextTick(() => {
					if (this.value < this.max) {
						if (String(this.value).indexOf('.') > -1) {
							let arr = String(this.value).split('.')
							this._value = Number(String(Number(arr[0]) + 1) + '.' + arr[1])
						} else {
							this._value = Number(this.value) + 1
						}
					}

					// })
				}
			},
			numDown() {
				if (!this.disabled) {
					// this.$nextTick(() => {
					if (this.value > this.min) {
						if (this.value - 1 <= this.min) {
							this._value = this.min
							return
						}
						if (String(this.value).indexOf('.') > -1) {
							let arr = String(this.value).split('.')
							this._value = Number(String(Number(arr[0]) - 1) + '.' + arr[1])
						} else {
							this._value = Number(this.value) - 1
						}
					}

					// })
				}
			},
		}
	}
</script>

<style scoped>

</style>