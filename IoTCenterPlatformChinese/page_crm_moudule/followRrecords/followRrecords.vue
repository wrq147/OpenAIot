<template>
	<view id="Customer">
		<top :isRightSlot="true" leftIcon="icon-fanhui" :isleftBack="true" backgroundColor="#ffffff" title="跟进记录" rightIcon="icon-a-tianjiabantouming" class="CRM-header">
			<template v-slot:top_right>
				<view class="right_top" @click="handAdd()">
					<custom-icons iconsName="icon-tianjia" iconsSize="36rpx"></custom-icons>
				</view>
			</template>	
		</top>
			<search-compt inputBg="#F8F8F8" ref="selectCompt" @openSelect="openSelect" @searching="searching" pal="请输入跟进人名称"></search-compt>
			<select-compt  ref="selectCompt" :haidate="true" :timeQuery="timeQuery" :querydata="queryData"
				@selectFinsh="selectFinsh" fixedHeight="216rpx"></select-compt>
		<view class="poolsList" v-for="(item,index) in customerData" :key="index" @click="handDetails(item.Id)">
			<view class="poolsList-item">
				<view class="poolsList-top">
					<view class="poolsList-top-title">
					<view>
						{{item.TargetName}}
					</view>
					<view class="Agent" v-if="item.TargetType==0">
						客户<!--Agent代理0 直销为1-->
					</view>
					<view class="Agent xiansuo" v-if="item.TargetType==1">
						线索
					</view>
					</view>
					<view class="time"><text class="title">执行人:</text><text class="content">{{item.FollowUserInfo.RealName}}</text></view>
					<view class="time"><text class="title">计划时间:</text><text class="content">{{item.FollowTime}}</text></view>
					<view class="poolsList-item-content"><text class="title">计划内容:</text><text class="content" v-html="item.Remark"></text></view>
				
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
				timeQuery: {
					name: '日期',
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
						 //console.log(res,'res')
						
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
<style>
	page{
		background: #F5F8F9;
	}
</style>
<style lang="less" scoped>
	.poolsList{
		// padding:0rpx 20rpx;
	}
	.poolsList-item{
		
	}
	.uni-select__input-text{
		font-size:28rpx;
	}
	.Customer-height{
		height:30rpx;
	}
	.poolsList-top-title{
		display: flex;
		font-size:30rpx!important;
		align-items: center;
	}
	.time,.poolsList-item-content{
		font-size:24rpx!important;
		margin-top:7rpx!important;
	}
	.content{
		font-size:24rpx;
	}
	
	.poolsList-item-content{
		display: flex;
		color:#999999;
		padding:20rpx;
		//justify-content: space-between;
		background: #F8F8F8!important;
		border-radius: 6rpx;
		margin-top:15rpx!important;
	}
	.title{
		//width:400rpx!important;
		margin-right:20rpx!important;
	}
	
	.poolsList-top-Agent{
		position: relative!important;
		width: 56rpx;
		height: 30rpx;
		line-height: 30rpx;
		background: none!important;
		border:1rpx solid #2371FF!important;
		color:#2371FF!important;
		padding:0rpx 10rpx;
		top:0rpx;
		font-size:20rpx;
		border-radius: 6rpx;
		text-align: center;
		margin-left:10rpx;
	}
	.Agent{
		position: relative!important;
		background: none!important;
		border-radius:6rpx!important;
		color:#2371FF!important;
		border:1rpx solid #2371FF!important;
		width: 40rpx;
		height: 30rpx;
		line-height:30rpx;
		font-size:20rpx!important;
		margin-left:10rpx;
	}
	.xiansuo{
		position: relative!important;
		border:1rpx solid #47E2F1!important;
		color:#47E2F1!important;
	}
</style>