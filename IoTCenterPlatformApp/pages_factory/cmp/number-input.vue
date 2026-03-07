<template>
	<view style="width: 100%;" class="form_li">
		<!-- <el-input-number style="width: 100%" :disabled="disabled" v-model="_value" size="medium" :placeholder="placeholder" :min="min" :max="max"></el-input-number> -->
		<!-- <input class="li_input input" type="digit" :placeholder="placeholder"
				placeholderStyle="color:rgba(255, 255, 255, 0.2);font-size:32rpx" v-model="_value"
				@input="filterValue2($event,precision)" :disabled="disabled" :maxlength="max.toString().length"/>
				 -->
		<view style="width: 100%;" v-if="disabled&&_value!=null">
			<view class="dis_text">
				<text>{{_value}}</text>
			</view>
		</view>
		<view style="width: 100%;" v-else>
			<uni-easyinput placeholderStyle="color:rgba(255, 255, 255, 0.2);font-size:32rpx" inputHeight="88rpx"
				:styles="styles" type="text" v-model="_value" :placeholder="placeholder" contentFontSize="32rpx"
				@input="filterValue2($event,precision)" :disabled="disabled" :inputBorder="disabled"
				v-if="_value!==undefined&&_value!==null" />
			<view class="num_cli" :style="{'background-image':`url(${getSerVerUrl()}/appimg/images/input_cli.png)`}">
				<view class="cli_up" @click.stop="numUp()">
					<custom-icons iconsName="icon-shuzijiajiansanjiao" iconsSize="14rpx"
						:iconsColor="!disabled&&zhuanhuaNumber(_value)<max?'rgba(153, 153, 153, 1)':'rgba(153, 153, 153, 0.5)'"></custom-icons>
				</view>
				<view class="cli_up rotate-180" @click.stop="numDown()">
					<custom-icons iconsName="icon-shuzijiajiansanjiao" iconsSize="14rpx"
						:iconsColor="!disabled&&zhuanhuaNumber(_value)>min?'rgba(153, 153, 153, 1)':'rgba(153, 153, 153, 0.5)'"></custom-icons>
				</view>
			</view>
		</view>
	</view>
</template>

<script>
	export default {
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
				default: -10000000000000000000
			},
			max: {
				type: Number,
				default: 10000000000000000000
			},
			precision: {
				type: Number,
				default: 0
			},
			isQianfenwei: {
				default: false,
				type: Boolean,
			}
		},
		data() {
			return {
				styles: {
					color: '#333',
					backgroundColor: 'rgba(248, 248, 248, 1)',
					disableColor: '#F8F8F8',
					borderColor: 'rgba(234, 234, 234, 1)'
				},
			}
		},
		beforeMount() {
			// console.log(this.value,'this.valuethis.valuethis.valuethis.value');
			if (!this.value || this.value == null) {
				this._value = 0
			}
		},
		computed: {
			_value: {
				get() {
					return this.value;
				},
				set(val) {
					this.$emit("input", val);
				}
			},

		},
		methods: {
			zhuanhuaNumber(num) {
				if (num && num != 'undefined' && num != 'null') {
					let numS = num;
					numS = numS.toString();
					numS = numS.replace(/,/gi, '');
					return Number(numS);
				} else {
					return num;
				}
			},
			filterValue2(input, n) {
				this.$nextTick(() => {
					let value = input
					if (Number(value) >= this.max) {
						this._value = Number(this.max)
						this.$forceUpdate()
						return
					}
					if (Number(value) <= this.min) {
						this._value = Number(this.min)
						this.$forceUpdate()
						return
					}
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
						if (this.isQianfenwei) {
							this._value = value.toLocaleString()
						} else {
							this._value = value
						}
						this.$forceUpdate()
						// })
					} else { //不保留小数
						value = value.replace(/[^\d]/g, '');
						value = value ? Number(value).toString() : value //去掉开头多个0
						// this.$nextTick(() => {
						if (this.isQianfenwei) {
							this._value = Number(value).toLocaleString()
						} else {
							this._value = Number(value)
						}
						this.$forceUpdate()
						// })
					}
				})

			},
			numUp() {
				console.log("uuuuuuuuuuuu");
				if (!this.disabled) {
					// this.$nextTick(() => {
					if (this.zhuanhuaNumber(this.value) < this.max) {
						if (String(this.value).indexOf('.') > -1) {
							let arr = this.zhuanhuaNumber(this.value).split('.')
							if (this.isQianfenwei) {
								this._value = Number(String(Number(arr[0]) + 1) + '.' + arr[1]).toLocaleString()
							} else {
								this._value = Number(String(Number(arr[0]) + 1) + '.' + arr[1])
							}

						} else {
							if (this.isQianfenwei) {
								this._value = (this.zhuanhuaNumber(this.value) + 1).toLocaleString()
							} else {
								this._value = this.zhuanhuaNumber(this.value) + 1
							}

						}
						console.log(this.value, this._value, 'this._value');
					}
					this.$forceUpdate()
					// })
				}
			},
			numDown() {
				console.log("numDown");
				if (!this.disabled) {
					// this.$nextTick(() => {
					if (this.zhuanhuaNumber(this.value) > this.min) {
						if (this.value - 1 <= this.min) {
							this._value = this.min
							return
						}
						if (String(this.value).indexOf('.') > -1) {
							let arr = this.zhuanhuaNumber(this.value).split('.')
							if (this.isQianfenwei) {
								this._value = Number(String(Number(arr[0]) - 1) + '.' + arr[1]).toLocaleString()
							} else {
								this._value = Number(String(Number(arr[0]) - 1) + '.' + arr[1])
							}

						} else {
							if (this.isQianfenwei) {
								this._value = (this.zhuanhuaNumber(this.value) - 1).toLocaleString()
							} else {
								this._value = this.zhuanhuaNumber(this.value) - 1
							}

						}
					}
					this.$forceUpdate()
					// })
				}
			},
		}
	}
</script>

<style scoped>

</style>