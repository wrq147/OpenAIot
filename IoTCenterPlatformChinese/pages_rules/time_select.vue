<template>
	<view class="pages_bgcon">
		<top :title="topTitle" leftWidth="157rpx" :isleftBack="true" leftIcon="icon-fanhui" rightWidth="157rpx"
			:rightIcon="isOnlyRead?'':'icon-wancheng'" rightIconColor="#2371FF" backgroundColor="#ffffff"
			@clickRight="finishTimeChoice">
		</top>
		<view class="time_listcon" v-if="timeType==1">
			<view class="time_li" @click="choiceRepeat">
				<view class="title">
					重复
				</view>
				<view class="picker_li">
					<view class="li_text">
						{{getText()}}
					</view>
					<custom-icons iconsName="icon-a-youjiantoubai" iconsSize="20rpx"
						iconsColor="#999999"></custom-icons>
				</view>
			</view>
			<view class="time_li">
				<view class="title">
					指定时间点
				</view>
				<view class="picker_li">
					<view class="time_font" v-if="isOnlyRead&&repeatVal!=0">{{ timeValue }}</view>
					<view class="time_font" v-else-if="isOnlyRead&&repeatVal==0">{{ datetimeValue }}</view>
					<uni-datetime-picker type="datetime" v-model="datetimeValue" @change="changeStartTime"
						v-else-if="!isOnlyRead&&repeatVal==0" :border="false" />
					<picker v-else-if="!isOnlyRead&&repeatVal!=0" mode="time" :value="timeValue" start="00:00"
						end="23:59" @change="onTimeChange">
						<view class="time_font">{{ timeValue }}</view>
					</picker>
					<custom-icons iconsName="icon-a-youjiantoubai" iconsSize="20rpx"
						iconsColor="#999999"></custom-icons>
				</view>
			</view>
		</view>
		<view class="time_listcon" v-if="timeType==2">
			<view class="time_li" @click="choiceInterval">
				<view class="title">
					按时间间隔
				</view>
				<view class="picker_li">
					<view class="li_text">
						{{getIntervalText()}}
					</view>
					<custom-icons iconsName="icon-a-youjiantoubai" iconsSize="20rpx"
						iconsColor="#999999"></custom-icons>
				</view>
			</view>
		</view>
		<uni-popup ref="intervalPopup" type="bottom" :mask-click="true" :zIndex="998">
			<view class="repeat_list">
				<view class="title">
					间隔类型
				</view>
				<view class="repeat_con">
					<view class="title_block"></view>
					<view class="repeat_li" @click="setIntervalVal(2)" :class="{'active':intervalType==2}">
						<view class="text">
							时
						</view>
						<custom-icons iconsName="icon-wancheng" iconsSize="28rpx" iconsColor="#2371FF"
							v-if="intervalType==2"></custom-icons>
					</view>
					<view class="repeat_li" @click="setIntervalVal(1)" :class="{'active':intervalType==1}">
						<view class="text">
							分
						</view>
						<custom-icons iconsName="icon-wancheng" iconsSize="28rpx" iconsColor="#2371FF"
							v-if="intervalType==1"></custom-icons>
					</view>
					<view class="repeat_li" @click="setIntervalVal(0)" :class="{'active':intervalType==0}">
						<view class="text">
							秒
						</view>
						<custom-icons iconsName="icon-wancheng" iconsSize="28rpx" iconsColor="#2371FF"
							v-if="intervalType==0"></custom-icons>
					</view>
					<!-- <view class="interval_num">
						<view class="num_label">
							间隔数
						</view>
						<view class="num_input">
							<uni-number-box background="#F8F8F8" v-model="intervalNum" color="#333333" :min="1"
								:max="intervalMax" width="180rpx" :isCusBtn="true" :disabled="isOnlyRead" />
						</view>
					</view> -->
				</view>
				<view class="repeat_btn">
					<view class="btn_left btn_li" @click="intervalFun">
						取消
					</view>
					<view class="btn_right btn_li" @click="intervalFun">
						确认
					</view>
				</view>
			</view>
		</uni-popup>
		<uni-popup ref="repeatPopup" type="bottom" :mask-click="true" :zIndex="998">
			<view class="repeat_list">
				<view class="title">
					重复
				</view>
				<view class="repeat_con">
					<view class="title_block"></view>
					<view class="repeat_li" @click="setRepeatVal(0)" :class="{'active':repeatVal==0}">
						<view class="text">
							只执行一次
						</view>
						<custom-icons iconsName="icon-wancheng" iconsSize="28rpx" iconsColor="#2371FF"
							v-if="repeatVal==0"></custom-icons>
					</view>
					<view class="repeat_li" @click="setRepeatVal(1)" :class="{'active':repeatVal==1}">
						<view class="text">
							每天
						</view>
						<custom-icons iconsName="icon-wancheng" iconsSize="28rpx" iconsColor="#2371FF"
							v-if="repeatVal==1"></custom-icons>
					</view>
					<view class="repeat_li" @click="setRepeatVal(2)" :class="{'active':repeatVal==2}">
						<view class="text">
							每月
						</view>
						<custom-icons iconsName="icon-wancheng" iconsSize="28rpx" iconsColor="#2371FF"
							v-if="repeatVal==2"></custom-icons>
					</view>
					<view class="repeat_li" @click="setRepeatVal(3)" :class="{'active':repeatVal==3}">
						<view class="text">
							自定义
						</view>
						<custom-icons iconsName="icon-wancheng" iconsSize="28rpx" iconsColor="#2371FF"
							v-if="repeatVal==3"></custom-icons>
					</view>
				</view>
				<view class="repeat_btn">
					<view class="btn_left btn_li" @click="repeatCancel">
						取消
					</view>
					<view class="btn_right btn_li" @click="repeatConfirm">
						确认
					</view>
				</view>
			</view>
		</uni-popup>
		<uni-popup ref="weekPopup" type="bottom" :mask-click="true" :zIndex="998">
			<view class="repeat_list">
				<view class="title">
					重复
				</view>
				<view class="repeat_con">
					<view class="title_block"></view>
					<view class="repeat_li" @click="setCustomVal(row.val)" v-for="row in daylist">
						<view class="text">
							{{row.text}}
						</view>
						<custom-icons iconsName="icon-wancheng" iconsSize="28rpx" iconsColor="#2371FF"
							v-if="weekVal.includes(row.val)"></custom-icons>
					</view>
				</view>
				<view class="repeat_btn">
					<view class="btn_left btn_li" @click="weekCancel">
						取消
					</view>
					<view class="btn_right btn_li" @click="weekConfirm">
						确认
					</view>
				</view>
			</view>
		</uni-popup>
		<uni-popup ref="dayPopup" type="bottom" :mask-click="true" :zIndex="998" :isFixedBottomHeight="true"
			fixedBottomHeight="60%">
			<view class="repeat_list">
				<view class="title">
					重复
				</view>
				<view class="repeat_con">
					<view class="title_block"></view>
					<view class="repeat_li" @click="setDayVal(row)" v-for="row in dateList">
						<view class="text">
							{{row}}
						</view>
						<custom-icons iconsName="icon-wancheng" iconsSize="28rpx" iconsColor="#2371FF"
							v-if="dayVal&&dayVal==row"></custom-icons>
					</view>
				</view>
				<view class="repeat_btn"></view>
				<view class="repeat_btn day_repeat">
					<view class="btn_left btn_li" @click="dayCancel">
						取消
					</view>
					<view class="btn_right btn_li" @click="dayConfirm">
						确认
					</view>
				</view>
			</view>
		</uni-popup>
		<uni-popup ref="intervalValuePopup" type="bottom" :mask-click="true" :zIndex="998" :isFixedBottomHeight="true"
			fixedBottomHeight="60%">
			<view class="repeat_list">
				<view class="title">
					间隔时长
				</view>
				<view class="repeat_con">
					<view class="title_block"></view>
					<view class="repeat_li" @click="setIntervalValue(row)" v-for="row in activeIntervalList">
						<view class="text">
							{{row}} {{activeIntervalUnit}}
						</view>
						<custom-icons iconsName="icon-wancheng" iconsSize="28rpx" iconsColor="#2371FF"
							v-if="intervalNum&&intervalNum==row"></custom-icons>
					</view>
				</view>
				<view class="repeat_btn"></view>
				<view class="repeat_btn day_repeat">
					<view class="btn_left btn_li" @click="intervalValueCancel">
						取消
					</view>
					<view class="btn_right btn_li" @click="intervalValueConfirm">
						确认
					</view>
				</view>
			</view>
		</uni-popup>
	</view>
</template>

<script>
	var dayjs = require('@/common/day.js')
	import {
		setPagesParam
	} from '@/common/utillib.js'
	export default {
		data() {
			return {
				dateList: [],
				daylist: [],
				timeValue: '08:00',
				datetimeValue: '', //指定日期时间点
				topTitle: '定时触发',
				repeatVal: 0,
				weekVal: [],
				dayVal: '',
				infoIndex: 0,
				allInx: 0,
				timeType: 0, //时间表达式重复方式
				intervalType: 0, //间隔类型
				intervalNum: 1, //间隔数
				intervalMax: 59, //间隔的最大值
				defaultCronStr: '',
				isOnlyRead: false, //是否只读
				//时分秒对应间隔数组
				secondsList: [1, 2, 5, 10, 15, 20, 30],
				minutesList: [1, 2, 5, 10, 15, 20, 30],
				hoursList: [1, 2, 3, 4, 6, 12],
				activeIntervalUnit: ''
			};
		},
		computed: {
			activeIntervalList() {
				if (this.intervalType == 0) {
					this.activeIntervalUnit = 's'
					return this.secondsList;
				} else if (this.intervalType == 1) {
					this.activeIntervalUnit = 'm'
					return this.minutesList;
				} else if (this.intervalType == 2) {
					this.activeIntervalUnit = 'h'
					return this.hoursList;
				}
			},
		},
		onLoad(options) {
			this.dateList = []
			for (let i = 0; i < 31; i++) {
				this.dateList.push(i + 1)
			}
			this.daylist = []
			let arr = ['周日', '周一', '周二', '周三', '周四', '周五', '周六']
			arr.map((row, inx) => {
				// console.log(row, inx);
				let obj = {
					text: row,
					val: inx + 1
				}
				this.daylist.push(obj)
			})
			if (options.type) {
				this.timeType = options.type
			}
			if (options.isOnlyRead) {
				this.isOnlyRead = true
			}
			if (options.cronStr) {
				this.defaultCronStr = options.cronStr
				let arr = options.cronStr.split(' ')
				if (options.cronStr.indexOf('/') > -1) {
					if (this.timeType == 2) {
						this.timeType = 2;
						if (arr[0].indexOf("/") > -1) {
							this.intervalType = 0;
							this.intervalMax = 59;
							let numarr = arr[0].split("/");
							this.intervalNum = numarr[1];
						} else if (arr[1].indexOf("/") > -1) {
							this.intervalType = 1;
							this.intervalMax = 59;
							let numarr = arr[1].split("/");
							this.intervalNum = numarr[1];
						} else if (arr[2].indexOf("/") > -1) {
							this.intervalType = 2;
							this.intervalMax = 23;
							let numarr = arr[2].split("/");
							this.intervalNum = numarr[1];
						}
					}

				} else {
					if (this.timeType == 1) {
						this.timeType = 1
						let h = arr[2].length == 2 ? arr[2] : '0' + arr[2]
						let m = arr[1].length == 2 ? arr[1] : '0' + arr[1]
						this.timeValue = h + ':' + m
						if (arr.length == 7) {
							this.repeatVal = 0
							this.timeValue = ''
							let dateStr = arr[6] + '-' + arr[4] + '-' + arr[3] + ' ' + arr[2] + ':' + arr[1] + ':' + arr[0]
							this.datetimeValue = dayjs(dateStr).format('YYYY-MM-DD HH:mm:ss')
						} else if (arr.length == 6 && arr[3] == '*' && arr[5] == '?') {
							this.repeatVal = 1
						} else if (arr.length == 6 && arr[4] == "*" && arr[5] == "?") {
							this.repeatVal = 2;
							this.dayVal = Number(arr[3]);
						} else if (arr.length == 6 && arr[3] == "?") {
							this.repeatVal = 3
							this.weekVal = arr[5].split(',')
							let arr2 = []
							this.weekVal.map(row => {
								arr2.push(Number(row))
							})
							this.weekVal = JSON.parse(JSON.stringify(arr2))
						}
					}
				}
			}
			if (options.inx) {
				this.infoIndex = options.inx
			}
			if (options.allInx) {
				this.allInx = options.allInx
			}
		},
		methods: {
			changeStartTime(val) {
				//只执行一次切换时间
				if (dayjs().isAfter(dayjs(val))) {
					this.$nextTick(() => {
						this.datetimeValue = ''
						// this.$forceUpdate()
					})
					// this.$message.warning("所选时间需大于当前时间");
					uni.showToast({
						title: '所选时间需大于当前时间',
						icon: 'none'
					})
				}
			},
			intervalFun() {
				this.$refs.intervalPopup.close()
			},
			repeatCancel() {
				this.$refs.repeatPopup.close()
			},
			repeatConfirm() {
				if (this.repeatVal == 3 && this.weekVal.length == 0) {
					this.$refs.weekPopup.open()
				} else {
					this.$refs.repeatPopup.close()
				}
			},
			weekCancel() {
				this.$refs.weekPopup.close()
			},
			weekConfirm() {
				if (this.weekVal.length > 0) {
					this.$refs.weekPopup.close()
				}
			},
			dayCancel() {
				this.$refs.dayPopup.close()
			},
			dayConfirm() {
				if (this.dayVal) {
					this.$refs.dayPopup.close()
				}
			},
			intervalValueCancel() {
				this.$refs.intervalValuePopup.close()
			},
			intervalValueConfirm() {
				if (this.intervalNum) {
					this.$refs.intervalValuePopup.close()
				}

			},
			setIntervalValue(val) {
				this.intervalNum = val
			},
			setDayVal(val) {
				//设置日期的值
				if (this.isOnlyRead) {
					return
				}
				this.dayVal = val
			},
			setCustomVal(val) {
				if (this.isOnlyRead) {
					return
				}
				if (this.weekVal.includes(val)) {
					let ix = this.weekVal.indexOf(val)
					this.weekVal.splice(ix, 1)
				} else {
					this.weekVal.push(val)
				}
			},
			setRepeatVal(val) {
				if (this.isOnlyRead && this.repeatVal != val) {
					return
				}
				if (val !== 0) {
					this.timeValue = '08:00'
				}
				this.repeatVal = val
				if (val == 2) {
					// this.$refs.repeatPopup.close()
					this.$refs.dayPopup.open()
				} else if (val == 3) {
					// this.$refs.repeatPopup.close()
					this.$refs.weekPopup.open()
				}
			},
			setIntervalVal(val) {
				if (this.isOnlyRead) {
					return
				}
				this.intervalType = val
				if (this.intervalType == 0) {
					if (this.secondsList.includes(Number(this.intervalNum))) {} else {
						this.intervalNum = 1;
					}
				} else if (this.intervalType == 1) {
					if (this.minutesList.includes(Number(this.intervalNum))) {} else {
						this.intervalNum = 1;
					}
				} else if (this.intervalType == 2) {
					if (this.hoursList.includes(Number(this.intervalNum))) {} else {
						this.intervalNum = 1;
					}
				}
				this.$refs.intervalValuePopup.open()
			},
			getIntervalText() {
				if (this.intervalType == 0) {
					return this.intervalNum + '秒'
				} else if (this.intervalType == 1) {
					return this.intervalNum + '分'
				} else if (this.intervalType == 2) {
					return this.intervalNum + '时'
				}
			},
			getText() {
				if (this.repeatVal == 0) {
					return '只执行一次'
				}
				if (this.repeatVal == 1) {
					return '每天'
				}
				if (this.repeatVal == 2) {
					let str = ''
					if (this.dayVal) {
						str = this.dayVal + '号'
					}
					return '每月' + str
				}
				if (this.repeatVal == 3) {
					let textarr = []
					this.daylist.map(row => {
						if (this.weekVal.includes(row.val)) {
							textarr.push(row.text)
						}
					})
					return textarr.join(',')
				}
			},
			choiceInterval() {
				//时间间隔
				this.$refs.intervalPopup.open()
			},
			choiceRepeat() {
				this.$refs.repeatPopup.open()
			},
			onTimeChange(e) {
				this.timeValue = e.mp.detail.value;
				// console.log(this.timeValue, 'timeValuetimeValue');
			},
			finishTimeChoice() {
				if (this.isOnlyRead) {
					return
				}
				if (this.timeType == 1) {
					this.finishTimeType()
				} else if (this.timeType == 2) {
					this.finishIntervalType()
				}
			},
			finishTimeType() {
				let str = ''
				let arr = []
				if (this.timeValue) {
					arr = this.timeValue.split(':')
				} else {
					arr = [0, 0, 0]
				}

				if (this.repeatVal == 0) {
					// let nowDate = new Date()
					// var hour = nowDate.getHours() + 1;
					// var minutes = nowDate.getMinutes();
					// if ((Number(arr[0]) < hour) || (Number(arr[0]) == hour && Number(arr[1]) < minutes)) {
					// 	nowDate.setDate(nowDate.getDate() + 1);
					// }

					// let day = dayjs(nowDate).date()
					// let mon = dayjs(nowDate).month()
					// let year = dayjs(nowDate).year()
					// str = '0' + ' ' + Number(arr[1]) + ' ' + Number(arr[0]) + ' ' + day + ' ' + mon + ' ? ' + year
					if (this.datetimeValue) {} else {
						this.datetimeValue = new Date()
					}
					let second = dayjs(this.datetimeValue).second();
					let minuttes = dayjs(this.datetimeValue).minute();
					let hour = dayjs(this.datetimeValue).hour();
					let day = dayjs(this.datetimeValue).date();
					let mon = dayjs(this.datetimeValue).month() + 1;
					let year = dayjs(this.datetimeValue).year();

					str = second + " " + minuttes + " " + hour + " " + day + " " + mon + " ? " + year;

				} else if (this.repeatVal == 1) {
					str = '0' + ' ' + Number(arr[1]) + ' ' + Number(arr[0]) + ' * * ?'
				} else if (this.repeatVal == 2) {
					str = "0" + " " + Number(arr[1]) + " " + Number(arr[0]) + " " + this.dayVal + " * ?";
				} else if (this.repeatVal == 3) {
					if (this.weekVal.length == 0) {
						uni.showToast({
							title: '请选择重复周期',
							icon: 'none'
						})
						return
					}
					let weekStr = this.weekVal.join(',')
					str = '0' + ' ' + Number(arr[1]) + ' ' + Number(arr[0]) + ' ? * ' + weekStr
				}
				let obj = {}
				if (this.infoIndex && this.allInx) {
					obj = {
						infoIndex: this.infoIndex,
						allInx: this.allInx,
						str: str
					}
				} else {
					obj = {
						str: str
					}
				}
				setPagesParam('finishFixedTime', obj, 2)

			},
			finishIntervalType() {
				let str = "";

				if (this.intervalType == 0) {
					str = "0/" + this.intervalNum + " * * * * ?";
				} else if (this.intervalType == 1) {
					str = "0 0/" + this.intervalNum + " * * * ?";
				} else if (this.intervalType == 2) {
					str = "0 0 0/" + this.intervalNum + " * * ?";
				}
				let obj = {}
				if (this.infoIndex && this.allInx) {
					obj = {
						infoIndex: this.infoIndex,
						allInx: this.allInx,
						str: str
					}
				} else {
					obj = {
						str: str
					}
				}
				setPagesParam('finishFixedTime', obj, 2)
			}
		}
	}
</script>

<style lang="less" scoped>
	.repeat_list {
		background-color: #fff;
		border-radius: 20rpx 20rpx 0rpx 0rpx;
		width: 100%;
		padding: 0 20rpx;
		box-sizing: border-box;
		position: relative;
		height: 100%;
		overflow-y: scroll;

		// .repeat_con {

		// }

		.title {
			font-size: 34rpx;
			color: #333333;
			text-align: center;
			padding: 50rpx 0;
			position: fixed;
			top: 0;
			left: 0;
			text-align: center;
			width: 100%;
			background: #ffffff;
			border-radius: 20rpx 20rpx 0 0;
		}

		.title_block {
			height: 134rpx;
			width: 100%;
		}

		.repeat_li {
			display: flex;
			justify-content: space-between;
			align-items: center;
			width: 100%;
			height: 120rpx;
			color: #333333;
			font-size: 32rpx;
			padding: 0 30rpx;
			box-sizing: border-box;
			border-radius: 10rpx;

			&.active {
				background-color: #F8F8F8;
			}
		}

		.interval_num {
			display: flex;
			justify-content: flex-start;
			align-items: center;
			width: 100%;
			padding: 0 30rpx;
			box-sizing: border-box;
			height: 140rpx;
			margin-top: 30rpx;

			.num_label {
				font-size: 28rpx;
				color: #999999;
			}

			.num_input {
				margin-left: 73rpx;
			}
		}

		.repeat_btn {
			display: flex;
			justify-content: space-between;
			align-items: center;
			padding: 50rpx 0 40rpx 0;
			height: 100rpx;

			&.day_repeat {
				width: 100%;
				position: fixed;
				bottom: 0;
				left: 0;
				background-color: #fff;
			}

			.btn_li {
				width: calc(50% - 10rpx);
				text-align: center;
				font-size: 32rpx;
				border-radius: 10rpx;
				height: 100rpx;
				line-height: 100rpx;

				&.btn_left {
					background-color: #F8F8F8;
					color: #999999;
				}

				&.btn_right {
					background-color: #2371FF;
					color: #fff;
				}
			}
		}
	}

	.time_listcon {
		width: 100%;
		padding: 0 20rpx;
		box-sizing: border-box;
		font-size: 32rpx;
		color: #333333;

		.time_li {
			padding: 0 30rpx;
			box-sizing: border-box;
			width: 100%;
			height: 126rpx;
			border-radius: 10rpx;
			line-height: 126rpx;
			background-color: #fff;
			margin-top: 20rpx;
			display: flex;
			justify-content: space-between;
			align-items: center;

			.picker_li {
				display: flex;
				justify-content: flex-end;
				align-items: center;

				.li_text {
					font-size: 28rpx;
					color: #999999;
					margin-right: 20rpx;
				}

				.time_font {
					font-size: 28rpx;
					color: #999999;
					margin-right: 20rpx;
				}
			}
		}
	}
</style>