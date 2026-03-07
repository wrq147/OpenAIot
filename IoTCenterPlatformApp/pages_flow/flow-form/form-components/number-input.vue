<template>
	<view style="width: 100%;">
		<view style="width: 100%;" v-if="disabled && _value !== null && _value !== ''">
			<view class="dis_text">
				<text>{{ formatDisplay(_value) }}</text>
			</view>
		</view>
		<view style="width: 100%;" v-else>
			<uni-easyinput :focus="ishandleFocus" ref="inputRef2" :key="keyStr" :disabled="disabled" :inputBorder="disabled"
				contentFontSize="32rpx" placeholderStyle="color:rgba(255, 255, 255, 0.2);font-size:32rpx"
				inputHeight="88rpx" :type="inputType" v-model="_value" :placeholder="placeholder" :styles="styles"
				@input="handleInput" @blur="handleBlur" @focus="startFocus"/>
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
			min: { // 最小值
				type: Number,
				default: 0
			},
			max: { // 最大值
				type: Number,
				default: 100
			},
			precision: { // 小数位
				type: Number,
				default: 0
			},
		},
		data() {
			return {
				inputType: 'digit',
				// _value: null
				inputKey: 0,
				keyStr: 'int0',
				ishandleFocus:false
			}
		},
		watch: {
			precision(newVal) {
				this.inputType = newVal === 0 ? 'digit' : 'number'
			},
			value(newVal) {
				this._value = this.formatValue(newVal)
			},
		},
		mounted() {
			this._value = this.formatValue(this.value)
		},
		methods: {
			startFocus(){
				this.ishandleFocus=true
			},
			// 格式化显示值
			formatDisplay(value) {
				if (value === '' || value === null) return ''
				return Number(value).toFixed(this.precision)
			},

			// 格式化输入值
			formatValue(value) {
				if (value === '' || value === null) return ''

				// 确保在最大最小值范围内
				let num = Number(value)
				if (isNaN(num)) return ''

				num = Math.max(this.min, Math.min(this.max, num))

				// 根据精度格式化
				return this.precision === 0 ?
					Math.round(num).toString() :
					num.toFixed(this.precision)
			},

			// 输入事件处理
			handleInput(e) {
				// console.log(e, 'eeeee');
				let value = this.formatValue(e)

				// 处理整数模式
				if (this.precision === 0) {
					// 禁止输入小数点
					value = value.replace(/[^\d]/g, '')

					// 去除前导零
					value = value.replace(/^0+/, '')

					// 至少保留一位数字
					if (value === '') value = '0'
				} else {
					// 处理小数模式
					// 只允许一个小数点
					value = value.replace(/^0+(\d)/, '$1') // 去除前导零但保留0.xx

					// 限制小数点后的位数
					if (value.includes('.')) {
						const parts = value.split('.')
						if (parts[1].length > this.precision) {
							value = parts[0] + '.' + parts[1].substring(0, this.precision)
						}
					}
				}

				// 更新值并触发事件
				this._value = value
				
				// this.$nextTick(()=>{
				// 	console.log(this.$refs[this.keyStr],'this.$refs[this.keyStr]');
				// 	this.$refs[this.keyStr].focus()
				// })
				this.$emit('update:value', value)
				// this.$emit("input", value)
				this.$forceUpdate()
			},

			// 失焦事件处理
			handleBlur() {
				if (this._value === '' || this._value === null) return

				// 格式化最终值
				this._value = this.formatValue(this._value)
				if(this.ishandleFocus){
					this.inputKey++
					this.keyStr = 'int' + this.inputKey
					this.ishandleFocus=false
				}
				this.$emit('update:value', this._value)
				// this.$emit("input", this._value)
				this.$forceUpdate()
			}
		}
	}
</script>

<style scoped>
	.dis_text {
		padding: 10px 0;
		color: #333;
	}
</style>