<template>
	<view style="background-color: #161A26;">
		<top title="Alarm" leftText="Back" leftWidth="157rpx" leftIcon="icon-fanhui" rightText="Process all"
			rightWidth="157rpx" :isleftBack="true" backgroundColor="#161A26" @clickRight="clearAllWarn"></top>
		<search-compt @openSelect="openSelect" @searching="searching" pal="Please enter the alarm name or device name"></search-compt>
		<select-compt ref="selectCompt" @selectFinsh="selectFinsh" :haidate="true" :timeQuery="timeQuery"
			:querydata="querydata" fixedHeight="216rpx">
		</select-compt>
		<view class="alarm_con">
			<device-alarm :ararmList="alarmList" :status="status"></device-alarm>
		</view>
        <msg-prompt ref="promptMsg" @confirm="confirm"></msg-prompt>
	</view>
</template>

<script>
	import deviceAlarm from '@/pages_device/device_info/components/device-alarm.vue'
	import {
		warningList,
		clearAllWarning
	} from '@/api/device.js'
	import serverUrl from '@/common/constVar.js'
	export default {
		components: {
			deviceAlarm
		},
		data() {
			return {
				alarmList: ['Ordinary', 'Urgent', 'Warning', 'Ordinary', 'Urgent', 'Warning'],
				timeQuery: {
					name: 'date',
					params: ['beginTime', 'endTime'],
					value: [],
				},
				querydata: {
					pageNum: 1,
					pageSize: 30
				},
				status: 'loading'
			}
		},
		mounted() {
			this.getWarningList()
		},
		onReachBottom() { //上拉触底
			// console.log("现在的状态是什么", this.status);
			if (this.status != 'noMore') {
				this.querydata.pageNum++;
				this.status = "loading";
				this.getWarningList();
				// console.log("现在是第几页", this.page);
			}
		},
		methods: {
			confirm(){
				clearAllWarning().then(res=>{
					// console.log(res,'处理全部报警');
					this.$refs.promptMsg.succossOpen()
					this.querydata.pageNum = 1
					this.status = 'loading'
					this.alarmList = []
					this.getWarningList()
				}).catch((err=>{
					this.setMsgTop(err)
				}))
			},
			clearAllWarn() {
				//清除所有报警
				this.$refs.promptMsg.noticeOpen('Are you sure to handle all alarms?','system prompt',true)
				
			},
			searching(val) {
				this.key = val
				if (this.key) {
					this.querydata.Name = this.key
					this.querydata.pageNum = 1
					this.status = 'loading'
					this.alarmList = []
					this.getWarningList()
				} else {
					delete this.querydata.Name
					this.querydata.pageNum = 1
					this.status = 'loading'
					this.alarmList = []
					this.getWarningList()
				}
			},
			openSelect() {
				this.$refs.selectCompt.openSelect()
			},
			getWarningList(query) {
				// this.querydata.Status = 0;
				// if (this.activeName == "processing") {
				// 	this.querydata.Status = 0;
				// } else if (this.activeName == "processed") {
				// 	this.querydata.Status = 1;
				// }
				if(query){
					this.querydata.pageNum=1
					this.status='loading'
				}
				if (this.querydata.pageNum == 1) {
					this.alarmList = [];
				}
				warningList(this.querydata).then((res) => {
					// console.log("报警列表", res);
					this.alarmList = [...this.alarmList, ...res.data.List];
					if (res.data.List.length < this.querydata.pageSize) {
						this.status = 'noMore';
					} else {
						this.status = 'more';
					}
				});
			},
			selectFinsh(query) {
				this.querydata = JSON.parse(JSON.stringify(query))
				this.querydata.pageNum = 1
				this.status = 'loading'
				this.getWarningList()
			}
		}
	}
</script>

<style lang="less" scoped>
	.alarm_con {
		width: 100%;
		padding: 0 20rpx;
		box-sizing: border-box;
		padding-bottom: 50rpx;
	}
</style>