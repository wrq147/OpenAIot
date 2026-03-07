<template>
	<view id="Customer">
		<top :isRightSlot="true" leftIcon="icon-fanhui" :isleftBack="true" backgroundColor="#ffffff" title="商机" rightIcon="icon-a-tianjiabantouming" class="CRM-header">
			<template v-slot:top_right>
				<view class="right_top" @click="handAdd()">
					<custom-icons iconsName="icon-tianjia" iconsSize="36rpx"></custom-icons>
				</view>
			</template>
		</top>
			<search-compt ref="selectCompt" @openSelect="openSelect" @searching="searching" inputBg="#F8F8F8" pal="请输入商机名称"></search-compt>
			<select-compt  ref="selectCompt" :haidate="true" :timeQuery="timeQuery" :querydata="queryData"
				@selectFinsh="selectFinsh" fixedHeight="216rpx"></select-compt>
		<view class="poolsList" v-for="(item,index) in customerData" :key="index" @click="handDetails(item)">
			<view class="poolsList-item">
				<view class="poolsList-top">
					<view class="poolsList-top-title">{{item.OpportName}}</view>
					<view class="time">客户名称:&nbsp;&nbsp;{{item.CustomerName}} </view>
					<view class="time">
						<text>销售阶段:&nbsp;&nbsp;{{item.PeriodName}}</text>
						<text>{{'　'+item.Probability+'%'}}</text>
					</view>
				</view>
			</view>
		</view>
		<uni-load-more iconType="circle" :status="status" v-if="status" />
	</view>
</template>

<script>
	import {
		BusineData//商机列表
	} from "@/api/crmApi";
	import {
		setPagesParam
	} from '@/common/utillib.js'
	export default {
		data(){
			return{
				key:'',
				status: 'loading',
				customerData:[],
				queryData:{
					// Belong:1,
					pageNum:1,
					pageSize:30,
				},
				timeQuery: {
					name: '日期',
					params: ['beginTime', 'endTime'],
					value: [],
				},
				opportHide:''
			}
		},
		onLoad(option) {
			this.list();//线索池
			if (option.opportHide) {
				this.opportHide = true
			}
		},
		methods:{
			openSelect() {
				this.$refs.selectCompt.openSelect()
			},
			selectFinsh(query) {
				//console.log(query,'111')
				this.querydata = JSON.parse(JSON.stringify(query))
				this.querydata.pageNum = 1
				this.status = 'loading'
				this.list();
			},
			searching(val) {
				this.key = val
				if (this.key) {
					this.queryData.Key = this.key
					this.queryData.pageNum = 1
					this.status = 'loading'
					this.customerData = []
					this.list()
				} else {
					delete this.queryData.Key
					this.queryData.pageNum = 1
					this.status = 'loading'
					this.customerData = []
					this.list()
				}
			},
			handAdd(){
				uni.navigateTo({
					url:'./additionOpportunity'
				})
			},
			handDetails(ite){
				if (this.opportHide) {
					setPagesParam('customData', ite, 1);
				} else {
					uni.navigateTo({
						url:'./opportunityDetails?id='+ite.Id
					})
				}
				
				
			},
			searching(val) {
				this.key = val
				if (this.key) {
					this.queryData.Key = this.key
					this.queryData.pageNum = 1
					this.status = 'loading'
					this.customerData = []
					this.list()
				} else {
					delete this.queryData.Key
					this.queryData.pageNum = 1
					this.status = 'loading'
					this.customerData = []
					this.list()
				}
			},
			list(){
				//商机列表（私海）
				BusineData(this.queryData).then((res)=>{
					if(res.code==0){
						if (this.queryData.pageNum == 1) {
							this.customerData = []
						}
						this.customerData = [...this.customerData, ...res.data.List];
						if (res.data.List.length < this.queryData.pageSize) {
							this.status = 'noMore';
						} else {
							this.status = 'more';
						}
						console.log(res,'res')
						//this.customerData=res.data.List;
					}
				})
			}
		}
	}
</script>
<style>
	page{
		background: #F5F8F9;
	}
</style>
<style lang="less" scoped>
	.time{
		font-size:24rpx!important;
		margin-top:0rpx !important;
	}
	.poolsList-top-title{
		margin-bottom: 8rpx;
	}
	.poolsList-top{
		
	}
</style>

