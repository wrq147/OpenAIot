<template>
	<view style="width: 100%;">
		<view v-if="disabled" style="width: 100%;">
			<view class="dis_text" v-if="!expanding&&_value&&_value.length>0">
				<text v-for="(row,inx) in _value">{{row}}{{inx==_value.length-1?'':', '}}</text>
			</view>
			<view class="view_input" v-else-if="!expanding&&!_value||_value.length==0">
				<view class="pal_col" v-if="!_value||_value.length==0">
					请选择
				</view>
				<view class="view_mask"></view>
				<view class="form_sel_icon">
					<custom-icons iconsName="icon-xialajiantou" iconsSize="14rpx"
						iconsColor="rgba(255, 255, 255, 0.5)"></custom-icons>
				</view>
			</view>
			<uni-data-checkbox v-model="_value" multiple :localdata="optionsBox" selectedColor="#FF3535"
				selectedTextColor="#ffffff" custextColor="#ffffff" disabledColor="rgba(255, 255, 255, 0.5)"
				noselectedColor="rgba(255, 255, 255, 0.5)" :isColumn="true" textFont="32rpx" textLeftMargin="16rpx"
				noSelectBorder="rgba(255, 255, 255, 0.5)" noSelectBg="inherit" :isRadius="true"
				@change="multipleSelectChange('multipleSelect')" :disabled="disabled" v-else></uni-data-checkbox>
		</view>
		<view v-else style="width: 100%;">
			<view class="view_input" @click="toOpen" v-if="!expanding">
				<view class="pal_col" v-if="!_value||_value.length==0">
					请选择
				</view>
				<view class="view_li_con" :id="'view_li_mul_'+keyId" :class="{'shenglue_li':mulArr.includes(keyId)}">
					<view class="view_li_cot" :id="'multipleSelect_li_'+keyId">
						<view class="view_li" v-for="(item,inx) in _value">
							<view class="view_text">
								{{item}}
							</view>
							<view class="view_icon" @click.stop="delMulLi(item,inx)">
								<custom-icons iconsName="icon-guanbidanchuang" iconsSize="20rpx"
									iconsColor="rgba(255, 255, 255, 1)"></custom-icons>
							</view>
						</view>
					</view>
				</view>
				<view class="view_mask"></view>
				<view class="form_sel_icon">
					<custom-icons iconsName="icon-xialajiantou" iconsSize="14rpx"
						iconsColor="rgba(255, 255, 255, 0.5)"></custom-icons>
				</view>
			</view>
			<uni-data-checkbox v-model="_value" multiple :localdata="optionsBox" selectedColor="#FF3535"
				selectedTextColor="#ffffff" custextColor="#ffffff" disabledColor="rgba(255, 255, 255, 0.5)"
				noselectedColor="rgba(255, 255, 255, 0.5)" :isColumn="true" textFont="32rpx" textLeftMargin="16rpx"
				noSelectBorder="rgba(255, 255, 255, 0.5)" noSelectBg="inherit" :isRadius="true"
				@change="multipleSelectChange('multipleSelect')" :disabled="disabled" v-else></uni-data-checkbox>
		</view>
		<view class="select_popup">
			<jp-select-plus ref="selectPlus" label="多选带搜索-拼接展示结果" placeholder="请选择" isJoin checkbox v-model="_value"
				:list="options" keys="value" showName="text" @toConfirm="getMultipleVal" color="#FF3535"
				checkboxColor="#ffffff"></jp-select-plus>
		</view>
	</view>
</template>

<script>
	import componentMinxins from '../ComponentMinxins'

	export default {
		mixins: [componentMinxins],
		name: "MultipleSelect",
		components: {},
		props: {
			placeholder: {
				type: String,
				default: '请选择选项'
			},
			value: {
				type: Array,
				default: () => {
					return []
				}
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
			keyId: {
				type: [String, Number],
				default: ''
			},
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
		data() {
			return {
				checks: [],
				mulArr: [],
				val: []
			}
		},
		created() {
			this.setMulVal()
		},
		methods: {
			delMulLi(row, inx) {
				//多选时删除元素
				// this._value.splice(inx, 1)
				this._value.splice(inx, 1)
				this.$nextTick(() => {
					let conWid = 0
					const query = uni.createSelectorQuery().in(this);
					query.select('#view_li_mul_' + this.keyId).boundingClientRect(data1 => {
						conWid = data1.width
					})
					query.select('#multipleSelect_li_' + this.keyId).boundingClientRect(data => {
						if (data.width > conWid) {
							if (this.mulArr.includes(this.keyId)) {} else {
								this.mulArr.push(this.keyId)
							}
						} else {
							console.log(this.mulArr, this.mulArr.includes(this.keyId));
							if (this.mulArr.includes(this.keyId)) {
								this.mulArr = this.mulArr.filter(item => {
									item != this.keyId
								})
							}
						}
					}).exec();
				})
			},
			setMulVal(label) {
				//设置多选时
				if(!this.value||this.value==null){
					this._value = []
				}
				
			},
			getMultipleVal(val) {
				//获取多选值
				this._value = val
				console.log(this._value, 'this._value');
				// let arr = []
				// val.map(row => {
				// 	arr.push(row.value)
				// })
				// this._value = arr
				this.$nextTick(() => {
					let conWid = 0
					const query = uni.createSelectorQuery().in(this);
					query.select('#view_li_mul_' + this.keyId).boundingClientRect(data1 => {
						conWid = data1.width
					})
					query.select('#multipleSelect_li_' + this.keyId).boundingClientRect(data => {
						if (data.width > conWid) {
							if (this.mulArr.includes(this.keyId)) {} else {
								this.mulArr.push(this.keyId)
							}
						} else {
							console.log(this.mulArr, this.mulArr.includes(this.keyId));
							if (this.mulArr.includes(this.keyId)) {
								this.mulArr = this.mulArr.filter(item => {
									item != this.keyId
								})
							}
						}
					}).exec();
				})
			},
			toOpen() { //打开多选弹窗
				this.$refs.selectPlus.open()
			},
			multipleSelectChange(label) {
				//多选数据改变
				this.$nextTick(() => {
					console.log(this._value, '多选');
				})
			},
		}
	}
</script>

<style lang="less" scoped>
	.select_popup {
		position: fixed;
		left: 100%;
		bottom: 100%;
		z-index: 2;
	}
</style>