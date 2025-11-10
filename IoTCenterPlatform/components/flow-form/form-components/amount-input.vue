<template>
	<view style="width: 100%;">
		<!-- <input class="li_input" type="number" :placeholder="placeholder"
				placeholderStyle="color:rgba(255, 255, 255, 0.2);font-size:32rpx" v-model="_value"
				@input="filterValue($event,precision)" /> -->
		<view class="li_flex">
			<uni-easyinput placeholderStyle="color:rgba(255, 255, 255, 0.2);font-size:32rpx" inputHeight="88rpx"
				:styles="styles" type="number" v-model="_value" :placeholder="placeholder" contentFontSize="32rpx"
				primaryColor="rgba(255, 255, 255, 0.5)" @input="filterValue($event,precision)" :disabled="disabled" />
			<view class="num_cli">
				<view class="cli_up" @click.stop="numUp()">
					<custom-icons iconsName="icon-shuzijiajiansanjiao" iconsSize="14rpx"
						:iconsColor="disabled?'rgba(255, 255, 255, 0.2)':'rgba(255, 255, 255, 0.5)'"></custom-icons>
				</view>
				<view class="cli_up rotate-180" @click.stop="numDown()">
					<custom-icons iconsName="icon-shuzijiajiansanjiao" iconsSize="14rpx"
						:iconsColor="_value>0&&!disabled?'rgba(255, 255, 255, 0.5)':'rgba(255, 255, 255, 0.2)'"></custom-icons>
				</view>
			</view>
		</view>
		<view class="dis_text" style="margin-top: 15rpx" v-show="showChinese">
			<text>大写：</text>
			<text class="chinese">{{ chinese }}</text>
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
		computed: {
			chinese() {
				return this.convertCurrency(this.value);
			},
		},
		data() {
			return {};
		},
		beforeMount() {
			if(!this.value||this.value==null){
				this._value=0
			}
		},
		methods: {
			filterValue(input, n) {
				this.$nextTick(() => {
					let value = this._value;
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
					} else { ////限制只允许输入数字
						value = value.replace(/\D+/g, '');
						value = value ? Number(value).toString() : value //去掉开头多个0
						this._value = value
					}
				})
			},
			numUp() {
				if (!this.disabled) {
					if (String(this.value).indexOf('.') > -1) {
						let arr = String(this.value).split('.')
						this._value = Number(String(Number(arr[0]) + 1) + '.' + arr[1])
					} else {
						this._value = Number(this.value) + 1
					}
				}
			},
			numDown() {
				if (!this.disabled) {
					if (this.value > 0) {
						if (this.value - 1 <= 0) {
							this._value = 0
							return
						}
						if (String(this.value).indexOf('.') > -1) {
							let arr = String(this.value).split('.')
							this._value = Number(String(Number(arr[0]) - 1) + '.' + arr[1])
						} else {
							this._value = Number(this.value) - 1
						}
					}
				}
			},
			convertCurrency(money) {
				//转化

				let chineseStr = "";
				chineseStr = numberToWords(money, "en")
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

	// /deep/ .el-input__inner {
	//   text-align: left;
	// }
</style>