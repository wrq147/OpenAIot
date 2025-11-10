<template>
	<view class="page_con">
		<top :title="userInfo.name" leftWidth="60rpx" :titleIsLeft="true">
			<template v-slot:top_left>
				<view class="left_top">
					<image class="image" :src="userInfo.avatar" mode="aspectFit" v-if="userInfo.avatar">
					</image>
					<view class="image t-icon-morentouxiang" v-else></view>
				</view>
			</template>
			<!-- <template v-slot:top_right>
				<view class="right_top" @click.stop="onScanWifiConfig">
					<custom-icons iconsName="icon-saoma" iconsSize="36rpx"></custom-icons>
					<view class="dot"></view>
				</view>
			</template> -->
		</top>
		<my-calendar @choiceDate="choiceDate" v-if="isCheckPermi(['/CRMService/Plan/List'])"></my-calendar>
		<follow-swiper :info="planList" v-if="isCheckPermi(['/CRMService/Plan/List'])&&planList.length>0||isLoadPlan"
			:showLoading="isCheckPermi(['/CRMService/Plan/List'])?true:false"></follow-swiper>
		<schedule-reminder
			v-if="isCheckPermi(['/CRMService/Plan/List'])&&planList.length==0&&!isLoadPlan"></schedule-reminder>
		<my-factory v-if="isCheckPermi(['/AgentMan/'])"></my-factory>
		<machines v-if="isCheckPermi(['/CRMService/Dev/List'])"></machines>
		<inventory v-if="isCheckPermi(['/Stock/'])&&isShowInventory"></inventory>
		<my-tab-bar active="home" ref="mytab"></my-tab-bar>
		<msg-prompt ref="promptMsg"></msg-prompt>
	</view>
</template>
<!-- <script src="/common/donut-chart.js"></script> -->

<script>
	import followSwiper from '@/components/home-compt/follow-swiper.vue'
	import inventory from '@/components/home-compt/inventory.vue'
	import myFactory from '@/components/home-compt/my-factory.vue'
	import machines from '@/components/home-compt/machines.vue'
	import scheduleReminder from '@/components/home-compt/schedule-reminder.vue'
	var dayjs = require('@/common/day.js')
	import {
		getSevenPlan
	} from '@/api/home.js'
	export default {
		components: {
			followSwiper, //跟进记录
			scheduleReminder, //计划
			myFactory, //我的工厂
			inventory, //出入库
			machines, //设备
		},
		data() {
			return {
				showHome:true,
				planList: [],
				isLoadPlan: true,
				userInfo: {},
				isShowInventory: true
			}
		},
		async onLoad() {
			// console.log('这是home页面');
			// this.$nextTick(()=>{
			// 	this.$refs.promptMsg.open('提示信息',1000000)//提示信息组件
			// })
			if (this.$store.state.user && this.$store.state.user.uid) {
				this.userInfo = this.$store.state.user
			} else {
				await this.$store.dispatch('GetInfo')
				this.userInfo = this.$store.state.user
			}
			this.$nextTick(() => {
				this.$refs.mytab.loadCheck()
			})
			// console.log("登录用户信息", this.userInfo);
		},
		onPullDownRefresh() {
			uni.reLaunch({
				url:'/pages/index/other2'
			})
			uni.stopPullDownRefresh()
		},
		onShow() {
			if (this.$store.state.isReloadInventory) {
				this.isShowInventory = false
				this.$forceUpdate()
				this.isShowInventory = true
				this.$store.commit('SET_INVENTORY_INFO', false)
			}
		},
		methods: {
			choiceDate(date) {
				//选择日期
				this.isLoadPlan = true
				console.log("选中日期", date);
				let statrt = date + ' 00:00:00'
				statrt = dayjs(statrt).format('YYYY-MM-DD HH:mm:ss')
				let end = date + ' 23:59:59'
				end = dayjs(end).format('YYYY-MM-DD HH:mm:ss')
				getSevenPlan({
					PlanStartTime: statrt,
					PlanEndTime: end,
					pageSize: 10,
					pageNum: 1
				}).then(res => {
					// console.log("跟进计划", res);
					this.planList = res.data.List
					this.$forceUpdate()
					this.isLoadPlan = false
				}).catch(err => {
					this.isLoadPlan = false
					this.setMsgTop(err)
				})
			},
			getFollowPlan() {
				//获取跟进计划

			},
			onScanWifiConfig() {
				//配网扫码总入口
				let that = this
				uni.scanCode({
					success: function(res) {
						console.log("res", res);
						let tmpidx = res.result.lastIndexOf("iot");
						if (tmpidx > -1) {
							let id = res.result.substring(tmpidx);
							uni.navigateTo({
								url: '/pages/wifi-conf/wifi-conf?dtuId=' + id
							});
						} else {
							this.$refs.promptMsg.open('Invalid QR code', 2000)
						}

					}
				});
			},
		}
	}
</script>

<style lang="less" scoped>



</style>