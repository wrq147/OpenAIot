<template>
	<view style="width: 100%;">
		<!-- <input class="li_input" type="number" :placeholder="placeholder"
				placeholderStyle="color:rgba(255, 255, 255, 0.2);font-size:32rpx" v-model="_value"
				@input="filterValue($event,precision)" /> -->
		<view style="width: 100%;" v-if="disabled&&_value!=null">
			<view class="dis_text">
				<text>{{_value}}</text>
			</view>
		</view>
		<view style="width: 100%;" v-else>
			<view class="li_flex">
				<uni-easyinput :focus="ishandleFocus" ref="inputRef" :key="keyStr" :disabled="disabled" :inputBorder="disabled"
					contentFontSize="32rpx" placeholderStyle="color:rgba(255, 255, 255, 0.2);font-size:32rpx"
					inputHeight="88rpx" :type="inputType" v-model="_value" :placeholder="placeholder" :styles="styles"
					@input="handleInput" @blur="handleBlur" @focus="startFocus"/>
			</view>
		</view>
		<view class="dis_text" style="margin-top: 15rpx;font-size: 24rpx;" v-show="showChinese">
			<text>大写：</text>
			<text class="chinese">{{ getchinese() }}</text>
		</view>
	</view>
</template>

<script>
	import componentMinxins from "../ComponentMinxins";
	import {
		numberToWords
	} from '@/common/toenglish.js'
	export default {
		mixins: [componentMinxins],
		name: "AmountInput",
		components: {},
		props: {
			value: {
				default: null,
			},
			placeholder: {
				type: String,
				default: "请输入金额",
			},
			showChinese: {
				type: Boolean,
				default: true,
			},
			precision: {
				type: Number,
				default: 0,
			},
			disabled: {
				default: false,
				type: Boolean,
			},
		},
		data() {
			return {
				chineseVal: '',
				inputType: 'digit',
				// _value: null
				inputKey: 0,
				keyStr: 'int0',
				min:0,
				max:999999999999999.9999,
				ishandleFocus:false
			};
		},
		beforeMount() {
			// console.log(this.value,'this.valuethis.valuethis.valuethis.value');
			// if (!this.value || this.value == null) {
			// 	this._value = 0
			// }
			// console.log(this.value,'11111',this._value,typeof this._value);
		},
		mounted() {
			this._value = this.formatValue(this.value)
		},
		watch: {
			// _value() {
			// 	this.chineseVal = this.getchinese()
			// },
			precision(newVal) {
				this.inputType = newVal === 0 ? 'digit' : 'number'
			},
			value(newVal) {
				this._value = this.formatValue(newVal)
				this.chineseVal = this.getchinese()
			},
		},
		methods: {
			startFocus(){
				this.ishandleFocus=true
			},
			updateValue(val) {
				// this._value = val
				// this.$emit('update:value', val)
				// this.$emit("input", val);
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
				// this.inputKey++
				// this.keyStr = 'int' + this.inputKey
				// this.$nextTick(()=>{
				// 	console.log(this.$refs[this.keyStr],'this.$refs[this.keyStr]');
				// 	this.$refs[this.keyStr].focus()
				// })
				this.$emit('update:value', value)
				// this.$emit("input", value)
				// this.$forceUpdate()
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
			},
			getchinese() {
				if (this.value > 999999999999999.9999) {
					return this.convertCurrency(999999999999999.9999);
				} else {
					return this.convertCurrency(this.value);
				}
			},
			convertCurrency(money) {
				//汉字的数字
				const cnNums = [
					"零",
					"壹",
					"贰",
					"叁",
					"肆",
					"伍",
					"陆",
					"柒",
					"捌",
					"玖",
				];
				//基本单位
				const cnIntRadice = ["", "拾", "佰", "仟"];
				//对应整数部分扩展单位
				const cnIntUnits = ["", "万", "亿", "兆"];
				//对应小数部分单位
				const cnDecUnits = ["角", "分", "毫", "厘"];
				//整数金额时后面跟的字符
				const cnInteger = "整";
				//整型完以后的单位
				const cnIntLast = "元";
				//最大处理的数字
				let maxNum = 999999999999999.9999;
				//金额整数部分
				let integerNum;
				//金额小数部分
				let decimalNum;
				//输出的中文金额字符串
				let chineseStr = "";
				//分离金额后用的数组，预定义
				let parts;
				if (money === "") {
					return "";
				}
				money = parseFloat(money);
				if (money >= maxNum) {
					//超出最大处理数字
					return "";
				}
				if (money === 0) {
					chineseStr = cnNums[0] + cnIntLast + cnInteger;
					return chineseStr;
				}
				//转换为字符串
				money = money.toString();
				if (money.indexOf(".") === -1) {
					integerNum = money;
					decimalNum = "";
				} else {
					parts = money.split(".");
					integerNum = parts[0];
					decimalNum = parts[1].substr(0, 4);
				}
				//获取整型部分转换
				if (parseInt(integerNum, 10) > 0) {
					var zeroCount = 0;
					var IntLen = integerNum.length;
					for (let i = 0; i < IntLen; i++) {
						let n = integerNum.substr(i, 1);
						let p = IntLen - i - 1;
						let q = p / 4;
						let m = p % 4;
						if (n == "0") {
							zeroCount++;
						} else {
							if (zeroCount > 0) {
								chineseStr += cnNums[0];
							}
							//归零
							zeroCount = 0;
							chineseStr += cnNums[parseInt(n)] + cnIntRadice[m];
						}
						if (m == 0 && zeroCount < 4) {
							chineseStr += cnIntUnits[q];
						}
					}
					chineseStr += cnIntLast;
				}
				//小数部分
				if (decimalNum !== "") {
					let decLen = decimalNum.length;
					for (let i = 0; i < decLen; i++) {
						let n = decimalNum.substr(i, 1);
						if (n !== "0") {
							chineseStr += cnNums[Number(n)] + cnDecUnits[i];
						}
					}
				}
				if (chineseStr === "") {
					chineseStr += cnNums[0] + cnIntLast + cnInteger;
				} else if (decimalNum === "") {
					chineseStr += cnInteger;
				}
				return chineseStr;
			},

		},
	};
</script>

<style lang="less" scoped>
	.chinese {
		color: #afadad;
		font-size: smaller;
	}

	// ::v-deep .el-input__inner {
	//   text-align: left;
	// }
</style>