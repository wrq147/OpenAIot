<template>
	<view class="calendar_con">
		<view class="mon_year">
			{{monthTrans()+'.'+getDateYear()}}
		</view>
		<view class="dates_ul">
			<view v-for="(item,index) in same_week" :class="same_day==item.date? 'activ dis' :'dis'"
				@click="select(item)" :key='index'>
				<text class="week_name">{{item.week}}</text>
				<view class="dates">
					{{item.name}}
				</view>
				<view class="cot" v-if="isToday(item.date)"></view>
			</view>
		</view>

	</view>
</template>

<script>
	var dayjs = require('@/common/day.js')
	export default {
		name: "my-calendar",//日历组件
		data() {
			return {
				week: [],
				same_week: [],
				same_day: '',
				start: '',
				end: '',

			};
		},
		created() {
			// 默认显示当天前一周的数据
			let data = []
			this.start = this.getDay(+7);
			this.end = this.getDay();
			for (let i = 6; i >= 0; i--) {
				data.push(this.getDay(+i))
			}
			var date = data.reverse()
			this.week = date;
			var date = this.week;
			var pkc = [];
			var weekday = ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"];
			date.forEach((item, index) => { //循坏日期
				var f = new Date(item);
				var week = f.getDay() //计算出星期几
				var str1 = item.split('/');
				var strs = str1[2];

				var weeks = weekday[week]
				var time = Math.round(new Date(item) / 1000)
				var s = {} //用于存储每个日期对象
				s.date = item;
				s.name = strs;
				s.week = weeks;
				s.times = time;
				pkc.push(s)
			})
			this.same_week = pkc;

			this.same_day = pkc[0].date;
			
			this.$emit('choiceDate',dayjs(this.same_day).format('YYYY-MM-DD'))
		},
		methods: {
			isToday(date) {
				//是否是今天的日期
				let nowDate = this.getDay(0)
				// console.log("日期", date, nowDate);
				return nowDate == date
			},
			getDateYear() {
				//获取日期年份
				var today = new Date();
				return today.getFullYear()
			},
			monthTrans() {
				//月份转化成英文
				let monthEnglish = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sept", "Oct", "Nov", "Dec"]

				return monthEnglish[new Date().getMonth()];
			},
			select(item) {
				//选择日期
				this.same_day = item.date;
				this.$emit('choiceDate',dayjs(this.same_day).format('YYYY-MM-DD'))
			},
			getDay(day) {
				var today = new Date();
				var targetday_milliseconds = today.getTime() + 1000 * 60 * 60 * 24 * day;
				today.setTime(targetday_milliseconds);
				var tYear = today.getFullYear();
				var tMonth = today.getMonth();
				var tDate = today.getDate();
				tMonth = this.doHandleMonth(tMonth + 1);
				tDate = this.doHandleMonth(tDate);
				return tYear + "/" + tMonth + "/" + tDate;
			},
			doHandleMonth(month) {
				var m = month;
				if (month.toString().length == 1) {
					m = month;
				}
				return m;

			},
			// 

		},
	}
</script>

<style lang="less">
	.calendar_con {
		width: 100%;
		background-color: #1C2232;
		color: rgba(255, 255, 255, 0.5);
		padding: 20rpx;
		box-sizing: border-box;
		border-radius: 0 0 20rpx 20rpx;
		font-family: Roboto-Regular, Roboto;

		.mon_year {
			font-weight: 550;
			font-size: 28rpx;
			margin-bottom: 16rpx;
		}

		.dates_ul {
			width: 100%;
			display: flex;
			justify-content: space-around;
			align-items: center;

			.dis {
				display: flex;
				flex-direction: column;
				justify-content: flex-start;
				align-items: center;
				width: 80rpx;
				height: 120rpx;
				border-radius: 50%;
				padding-top: 20rpx;
				box-sizing: border-box;

				.dates {
					line-height: 28rpx;
					font-size: 28rpx;
					color: rgba(255, 255, 255, 1);
					margin: 12rpx 0 11rpx 0;
					font-weight: 550;
				}

				.week_name {
					font-size: 24rpx;
					line-height: 24rpx;
				}

				.cot {
					width: 10rpx;
					height: 10rpx;
					border-radius: 100%;
					background-color: #FFFFFF;
				}
			}

			.activ {
				background: linear-gradient(180deg, #FF3535 0%, #FF613D 100%);
				box-shadow: 0px 4px 16px 0px rgba(255, 53, 53, 0.4);
				border-radius: 40px;

				.week_name {
					color: rgba(255, 255, 255, 1);
				}
			}
		}

	}
</style>