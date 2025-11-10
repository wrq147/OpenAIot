<template>
	<view>
		<uni-popup ref="selectPopup" :mask-click="true" background-color="#161A26" :safe-area="true"
			mask-background-color="rgba(0, 0, 0, 0.5)" type="top" @maskClick="maskClick">
			<view class="select_con" id="popup_con">
				<uni-nav-bar :status-bar="true" :fixed="true" :border="false" height="176rpx" :zIndex="997"
					backgroundColor="#161A26">
					<template v-slot:allslot>
					</template>
				</uni-nav-bar>
				<view class="select_li" v-if="hasSearch">
					<view class="li_label">{{searchQuery.name}}</view>
					<view class="li_input">
						<uni-easyinput prefixIcon="icon-sousuo" :placeholder="searchQuery.pal" v-model="searchQuery.key"
							clearSize="18" :prefixIconSize="14"
							placeholderStyle="color:rgba(255, 255, 255, 0.2);font-size:32rpx" :styles="customstyles"
							primaryColor="rgba(255, 255, 255, 0.5)" inputHeight="88rpx" :isCustom="true"
							prefixIconFocusColor="rgba(255, 255, 255, 0.5)" prefixIconColor="rgba(255, 255, 255, 0.2)">
						</uni-easyinput>
					</view>
				</view>
				<view class="select_li" v-if="selectListParam&&selectListParam.length>0" v-for="row in selectListParam">
					<view class="li_label">{{row.name}}</view>
					<view class="li_input">
						<uni-data-select v-model="row.value" :localdata="row.localdata" @change="change" width="100%"
							:placeholder="row.pal" borderColor="rgba(255, 255, 255, 0.20)"
							palColor="rgba(255, 255, 255, 0.2)" :isCustom="true" :isDark="true"></uni-data-select>
					</view>
				</view>
				<view class="select_li" v-if="haidate">
					<view class="li_label">{{timeQuery.name}}</view>
					<view class="li_input">
						<view class="int_date">
							<view class="date_con long_date" style="color: #fff;">
								<uni-datetime-picker v-model="timeQuery.value" type="daterange" :isDark="true"
									:changeBottom='changeBottom' :isCustom="true" />
							</view>
						</view>

					</view>
				</view>
				<view class="select_botton_con">
					<view class="reset" @click.stop="resetSelect">
						Reset
					</view>
					<view class="search" @click.stop="selectFinsh">
						Search
					</view>
				</view>
				<view class="zhanwei_con">
				</view>
			</view>
		</uni-popup>
		<uni-nav-bar :status-bar="true" :fixed="true" v-if="cusMask" :border="false" :height="0" :zIndex="999"
			backgroundColor="inherit;">
			<template v-slot:allslot>
				<view class="tanchuceng" :style="{'height':fixedHeight}" @click.stop="timeMask">

				</view>
			</template>
		</uni-nav-bar>

	</view>
</template>

<script>
	export default {
		name: "select-compt", //筛选组件
		props: {
			fixedHeight: {
				type: String,
				default: "88rpx"
			},
			selsectType: {
				type: String,
				default: 'def' //def默认类型；date包含时间
			},
			selectListParam: { //需要筛选的包含
				type: Array,
				default: () => {
					return []
				}
			},
			querydata: { //过滤的参数对象
				type: Object,
				default: () => {
					return {}
				}
			},
			haidate: {
				type: Boolean,
				default: false
			},
			timeQuery: { //日期过滤的参数对象
				type: Object,
				default: () => {
					return {}
				}
			},
			hasSearch: {
				type: Boolean,
				default: false
			},
			searchQuery: { //搜索框过滤的参数对象
				type: Object,
				default: () => {
					return {
						// name:'search',
						// pal:'please enter',
						// key:'',
						//params:''
					}
				}
			},
		},
		data() {
			return {
				isOpen: false,
				changeBottom: '',
				statusBarHeight: uni.getSystemInfoSync().statusBarHeight,
				cusMask: false,
				primaryColor: '#E63F31',
				isDark: true,
				customstyles: {
					color: '#ffffff',
					backgroundColor: '#161A26',
					disableColor: '#F7F6F6',
					borderColor: 'rgba(255, 255, 255, 0.20)'
				}
			};
		},
		destroyed() {
			if (this.$refs.selectPopup) {
				this.$refs.selectPopup.close()
			}

		},
		methods: {
			cusMaskShow(val) { //监听日期选择弹窗是否显示，显示则显示头部遮盖弹窗
				console.log("自定义弹出层", this.cusMask);
				if (val != null && val != undefined) {
					this.cusMask = val
				} else {
					this.cusMask = !this.cusMask
				}

			},
			timeMask() {
				//自定义的时间弹出层
				this.$refs.dateChoice1.close()
				this.$refs.dateChoice2.close()

			},
			dateChange() {
				//开始时间改变
			},
			dateChange2() {
				//结束时间改变
			},
			maskClick() {
				if (this.isOpen) {
					this.isOpen = false
					this.$refs.selectPopup.close()
				}
			},
			openSelect() {
				if (this.isOpen) {
					this.isOpen = false
					this.$refs.selectPopup.close()
				} else {
					this.isOpen = true
					this.$refs.selectPopup.open()
					this.changeBottom = "0"
					this.$nextTick(() => {
						const query = uni.createSelectorQuery().in(this);
						query.select('#popup_con').boundingClientRect(data => {
							console.log("data", data);
							data.top = Math.abs(data.top)
							// #ifdef H5
							this.changeBottom = 'calc(' + data.height * 2 + 'rpx + ' + this
								.statusBarHeight * 2 + 'rpx' + ' - 100vh)'
							//#endif
							// #ifndef H5
							this.changeBottom = 'calc(' + data.height * 2 + 'rpx - ' +
								this.statusBarHeight + 'rpx' + ' - 100vh)'
							//#endif
						}).exec();

					})
				}

			},
			change() {

			},
			selectFinsh() {
				//完成筛选
				console.log(this.querydata, 'this.querydata');
				this.selectListParam.map(row => {
					if (row.value != undefined && row.value != null) {
						this.querydata[row.params] = row.value
					} else {
						delete this.querydata[row.params]
					}
				})
				if (this.haidate && this.timeQuery.value && this.timeQuery.value.length > 0) {
					this.querydata[this.timeQuery.params[0]] = this.timeQuery.value[0]
					this.querydata[this.timeQuery.params[1]] = this.timeQuery.value[1]
				}else if(this.haidate){
					delete this.querydata[this.timeQuery.params[0]]
					delete this.querydata[this.timeQuery.params[1]]
				}
				if (this.hasSearch && this.searchQuery.key && this.searchQuery.key.length > 0) {
					this.querydata[this.searchQuery.params] = this.searchQuery.key
				}else if(this.hasSearch){
					delete this.querydata[this.searchQuery.params]
				}
				let querydata = JSON.parse(JSON.stringify(this.querydata))
				this.$emit('selectFinsh', querydata)
				this.isOpen = false
				this.$refs.selectPopup.close()
			},
			resetSelect() {
				//取消筛选
				if (this.haidate) {
					this.querydata[this.timeQuery.params[0]] = null
					this.querydata[this.timeQuery.params[1]] = null
					delete this.querydata[this.timeQuery.params[0]]
					delete this.querydata[this.timeQuery.params[1]]
				}
				if (this.haidate) {
					this.querydata[this.searchQuery.params] = null
					delete this.querydata[this.searchQuery.params]
				}
				this.selectListParam.map(row => {
					row.value = null
					delete this.querydata[row.params]
					let querydata = JSON.parse(JSON.stringify(this.querydata))
					this.$emit('selectFinsh', querydata)
				})
				if (this.haidate && this.timeQuery.value && this.timeQuery.value.length > 0) {
					this.timeQuery.value = []
					delete this.querydata[this.timeQuery.params[0]]
					delete this.querydata[this.timeQuery.params[1]]
					let querydata = JSON.parse(JSON.stringify(this.querydata))
					this.$emit('selectFinsh', querydata)
				}
				this.isOpen = false
				this.$refs.selectPopup.close()
			}
		}
	}
</script>

<style lang="less">
	.tanchuceng {
		position: fixed;
		top: 0;
		width: 100%;
		z-index: 996;
		background-color: rgba(0, 0, 0, 0.4);
	}
	#popup_con.select_con{
		position: relative !important;
		background-color: #161A26 !important;
		top: 0;
		// top: 0;
		.select_li{
			.li_input{
				padding-left: 0;
				border: none;
			}
		}
	}

	// .int_date {
	// 	// margin-top: 30rpx;
	// 	display: flex;
	// 	align-items: center;
	// 	justify-content: flex-start;

	// 	.date_con {
	// 		width: 345rpx;
	// 		height: 88rpx;
	// 		line-height: 88rpx;
	// 		border-radius: 6rpx;
	// 		// border: 1rpx solid #EAEAEA;
	// 		background-color: rgba(22, 26, 38, 1);

	// 		.date {
	// 			width: 345rpx;
	// 			text-align: center;
	// 		}

	// 		.date_slot {
	// 			width: 345rpx;
	// 			height: 88rpx;
	// 			border: 1rpx solid rgba(255, 255, 255, 0.2);
	// 			border-radius: 6rpx;
	// 			display: flex;
	// 			justify-content: space-around;
	// 			align-items: center;
	// 			color: #999999;
	// 			font-size: 28rpx;
	// 			box-sizing: border-box;
	// 			color: rgba(255, 255, 255, 0.2);

	// 			.icon_date {
	// 				width: 28rpx;
	// 				height: 28rpx;
	// 			}

	// 			&.has_val {
	// 				color: #fff;
	// 			}
	// 		}


	// 	}

	// 	.row_line {
	// 		width: 10rpx;
	// 		height: 2rpx;
	// 		border-radius: 1rpx;
	// 		background-color: rgba(255, 255, 255, 0.2);
	// 		margin: 0 5rpx;
	// 	}
	// }
</style>