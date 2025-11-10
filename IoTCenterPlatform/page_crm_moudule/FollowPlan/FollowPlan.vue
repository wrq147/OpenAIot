<template>
	<view id="Customer">
		<top leftIcon="icon-fanhui" :isleftBack="true" leftText="Back" backgroundColor="#161A26" title="Follow up plan" rightIcon="icon-a-tianjiabantouming" class="CRM-header">
			<template v-slot:top_right>
				<view class="right_top" @click="handAdd()">
					<custom-icons iconsName="icon-tianjia" iconsSize="36rpx"></custom-icons>
				</view>
			</template>
		</top>
		<view class="FollowPlanNav">
			<view class="FollowPlanNav-parItem">
				<view class="FollowPlanNav-item" @click="handClickCele(0,'A')">
					<view :class="[selectCele==0?'active':'on']">Complete</view>
					<view v-if="selectCele==0" class="FollowPlanNav-parItem-border"></view>
				</view>
				<view class="FollowPlanNav-item" @click="handClickCele(1,'F')">
					<view :class="[selectCele==1?'active':'on']">Completed</view>
					<view v-if="selectCele==1" class="FollowPlanNav-parItem-border"></view>
				</view>
			</view>
		</view>
		<view class="Customer-height">
			
		</view>
		<view class="poolsList" v-for="(item,index) in customerData" :key="index" @click="handDetails(item)">
			<view class="poolsList-item">
				<view class="poolsList-top">
					<view class="poolsList-top-title">{{item.CustomerName}}</view>
					<view class="time" v-if="item.ExecutorName">Plan executor:{{item.ExecutorName}}</view>
					<view class="time">Scheduled time: {{item.PlanTime}}</view>
					<!-- <view class="Agent">
						Agent
					</view> -->
					<view class="Agent" v-if="item.CustomerType==0">
						Agent<!--Agent代理0 直销为1-->
					</view>
					<view class="DirectSales" v-if="item.CustomerType==1">
						Direct Sales
					</view>
				</view>
				
			<view class="poolsList-item-button">
				<view class="button" >
					<text v-if="item.Status=='A'" class="iconfont icon-zhihanggenjin"></text>
					<text v-if="item.Status=='A'" @click.stop="handComplete(item)">Complete</text>
					<text v-if="item.Status=='F'">Completed</text>
				</view>
				<view class="button" @click.stop="handEdit(item.Id)">
					<text class="iconfont icon-bianji"></text>
					<text>Edit</text>
				</view>
				<view class="button btnRight" @click.stop="handDelete(item)">
					<text class="iconfont icon-shanchu"></text>
					<text>Delete</text>
				</view>
			</view>
			</view>
		</view>
		<uni-load-more iconType="circle" :status="status" v-if="status" />
		<msg-prompt ref="promptMsg" @confirm="confirmUnbind"></msg-prompt>
		<msg-prompt ref="promptMsgComple" @confirm="confirmComple"></msg-prompt>
	</view>
</template> 

<script>
	import {
		flowDataList,//跟进计划
		FollowPlan,//删除
		completion//完成
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
					// Belong:1,
					pageNum:1,
					pageSize:30,
					Status:'A'
				},
				EditId:'',
				completionEdit:'',
				selectCele:0,
			}
		},
		onLoad() {
			this.list();
		},
		methods:{
			handClickCele(inx,status){
				this.selectCele=inx;
				this.queryData.Status=status;
				this.list();
			},
			handComplete(item){
				//完成
				this.$refs.promptMsgComple.noticeOpen(
					"Are you sure about the client named"+item.CustomerName+"?"
				)
				this.completionEdit=item.Id
			},
			confirmComple(){
				//完成
				completion({id:this.completionEdit}).then((res)=>{
					if(res.code==0){
						uni.showToast({
							title:'Plan completed！',
							icon:'none'
						})
						this.list();
					}
				}).catch((err) => {
					this.setMsgTop(err)
				})
			},
			confirmUnbind(){
				//删除
				FollowPlan({id:this.EditId}).then((res)=>{
					if(res.code==0){
						uni.showToast({
							title:'Delete successful！',
							icon:'none'
						})
						this.list();
					}
				}).catch((err) => {
					this.setMsgTop(err)
				})
			},
			handDelete(item){
				//删除
				this.$refs.promptMsg.noticeOpen(
					"Are you sure to delete the customers "+item.CustomerName+"?"
				)
				this.EditId=item.Id
			},
			handEdit(id){
				uni.navigateTo({
					url:'./addFollowPlan?id='+id
				})
			},
			handAdd(){
				uni.navigateTo({
					url:'./addFollowPlan'
				})
			},
			handDetails(ite){
				uni.navigateTo({
					url:'./FollowDetails?id='+ite.Id+'&CustomerName='+ite.CustomerName
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
				flowDataList(this.queryData).then((res)=>{
					if(res.code==0){
						//console.log(res,'res')
							if (this.queryData.pageNum == 1) {
								this.customerData = []
							}
							res.data.List.map((row)=>{
							 row.ExecutorName=(row.ExecutorUsers.map(rs=>rs.RealName)).join(',')
							})
							//console.log(res.data.List,'listArr')
							this.customerData = [...this.customerData, ...res.data.List];
							if (res.data.List.length < this.queryData.pageSize) {
								this.status = 'noMore';
							} else {
								this.status = 'more';
							}	
					}
				})
			}
		}
	}
</script>

<style lang="less" scoped>
	page{
		background: #161A26;
	}
	.Customer-height{
		height:30rpx;
	}
	.poolsList-top-title{
		font-size:32rpx!important;
		margin-bottom: 10rpx!important;
	}
	.time{
		font-size:28rpx!important;
		margin-top:7rpx!important;
	}
	.FollowPlanNav{
		width:100%;
		background: none;
		padding-top:20rpx;
		color:#fff;
	}
	.FollowPlanNav-parItem{
		display: flex;
		padding:0px 146rpx;
		justify-content: space-between;
	}
	.FollowPlanNav-parItem-border{
		width: 30rpx;
		height:5rpx;
		background: #fff;
		border-radius: 3rpx;
		margin:0 auto;
		margin-top:8rpx;
	}
</style>