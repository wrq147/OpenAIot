<template>
	<view id="Customer">
		<top leftIcon="icon-fanhui" :isleftBack="true" leftText="Back" backgroundColor="#161A26" title="Followed up" rightIcon="icon-a-tianjiabantouming" class="CRM-header">
			<template v-slot:top_right>
				<view class="right_top" @click="handAdd()">
					<custom-icons iconsName="icon-tianjia" iconsSize="36rpx"></custom-icons>
				</view>
			</template>	
		</top>
			<search-compt ref="selectCompt" @openSelect="openSelect" @searching="searching" pal="Please enter the name of the follow-up record"></search-compt>
			<select-compt  ref="selectCompt" :haidate="true" :timeQuery="timeQuery" :querydata="queryData"
				@selectFinsh="selectFinsh" fixedHeight="216rpx"></select-compt>
				<view style="height:22rpx;width:100%;"></view>
		<view class="poolsList" v-for="(item,index) in customerData" :key="index" @click="handDetails(item.Id)">
			<view class="poolsList-item">
				<view class="poolsList-top">
					<view class="poolsList-top-title">{{item.TargetName}}</view>
					<view class="time"><text class="title">Plan executor:</text><text class="content">{{item.FollowUserInfo.RealName}}</text></view>
					<view class="time"><text class="title">Execution time:</text><text class="content">{{item.FollowTime}}</text></view>
					<view class="poolsList-item-content"><text class="title">Plan content:</text><text class="content" v-html="item.Remark"></text></view>
				<view class="Agent" v-if="item.TargetType==0">
					Customer<!--Agent代理0 直销为1-->
				</view>
				<view class="Agent" v-if="item.TargetType==1">
					Clue
				</view>
				</view>
			</view>
		</view>
		<uni-load-more iconType="circle" :status="status" v-if="status" />
	</view>
</template>

<script>
	import {
		flowDataRecord//跟进记录
	} from "@/api/crmApi";
	import {
		yuanAarngemnt//员工管理
	} from "@/api/personalCenter";
	export default {
		data(){
			return{
				key:'',
				status: 'loading',
				customerData:[],
				queryData:{
					 Belong:2,
					pageNum:1,
					pageSize:30,
				},
				employeeMap:new Map(),
				timeQuery: {
					name: 'date',
					params: ['beginTime', 'endTime'],
					value: [],
				},
			}
		},
		onLoad() {
			this.list();//线索池
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
					url:"./AddFollowRecord"
				})
			},
			handDetails(id){
				uni.navigateTo({
					url:'./followRecordsDetails?id='+id
				})
				
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
				flowDataRecord(this.queryData).then((res)=>{
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
						//this.customerData=res.data.List;
					}
				})
			}
		}
	}
</script>

<style lang="less" scoped>
	page{
		background: #161A26;
		/deep/.uni-select__input-text{
			font-size:32rpx;
		}
		/deep/.uni-forms-item__label{
			font-size:32rpx!important;
		}
		/deep/.uni-textarea-textarea{
			font-size:32rpx;
		}
	}
	.Customer-height{
		height:30rpx;
	}
	.poolsList-top-title{
		font-size:32rpx!important;
		margin-bottom: 10rpx!important;
	}
	.time,.poolsList-item-content{
		font-size:28rpx!important;
		margin-top:7rpx!important;
		
	}
	.content{
		font-size:27rpx;
	}
	
	.poolsList-item-content{
		display: flex;
		color:rgba(255, 255, 255, .6);
		//justify-content: space-between;
		
	}
	.title{
		//width:400rpx!important;
		margin-right:20rpx!important;
	}
	.poolsList-top{
		padding-bottom: 0rpx!important;
	}
	.Agent{
		background: rgba(63, 67, 86, 1)!important;
		
		padding:0rpx 10rpx;
	}
</style>