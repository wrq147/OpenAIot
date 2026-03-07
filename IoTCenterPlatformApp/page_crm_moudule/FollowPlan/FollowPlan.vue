<template>
	<view id="Customer">
		<top :isRightSlot="true" leftIcon="icon-fanhui" :isleftBack="true" backgroundColor="#ffffff" title="跟进计划" rightIcon="icon-a-tianjiabantouming" class="CRM-header">
			<template v-slot:top_right>
				<view class="right_top" @click="handAdd()">
					<custom-icons iconsName="icon-tianjia" iconsSize="36rpx"></custom-icons>
				</view>
			</template>
		</top>
	<view class="FollowPlanNav">
		<view class="FollowPlanNav-parItem">
			<view class="FollowPlanNav-item" @click="handClickCele(0,'A')">
				<view :class="[selectCele==0?'active':'on']">待完成</view>
				<view v-if="selectCele==0" class="FollowPlanNav-parItem-border"></view>
			</view>
			<view class="FollowPlanNav-item" @click="handClickCele(1,'F')">
				<view :class="[selectCele==1?'active':'on']">已完成</view>
				<view v-if="selectCele==1" class="FollowPlanNav-parItem-border"></view>
			</view>
		</view>
	</view>
		<view class="poolsList" v-for="(item,index) in followPlanData" :key="index" @click="handDetails(item)">
			<view class="poolsList-item">
				<view class="poolsList-top">
					<view class="poolsList-top-title">{{item.CustomerName}}</view>
					<view class="time">执行人：{{item.ExecutorName}}</view>
					<view class="time">计划时间：{{item.PlanTime}}</view>
				</view>
			<view class="poolsList-item-button">
				<view class="button" >
					<text v-if="item.Status=='A'" class="iconfont icon-zhihanggenjin"></text>
					<text v-if="item.Status=='A'" @click.stop="handComplete(item)">去完成</text>
					<text v-if="item.Status=='F'">已完成</text>
				</view>
				<view class="button" @click.stop="handEdit(item.Id)">
					<text class="iconfont icon-bianji"></text>
					<text>编辑</text>
				</view>
				<view class="button btnRight" @click.stop="handDelete(item)">
					<text class="iconfont icon-shanchu"></text>
					<text>删除</text>
				</view>
			</view>
			</view>
			<view class="Agent" v-if="item.CustomerType==0">
				代理<!--Agent代理0 直销为1-->
			</view>
			<view class="DirectSales" v-if="item.CustomerType==1">
				直销
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
				followPlanData:[],
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
			returnExe(val){
				if(val){
					return val.slice(0,val.length-1)
				}
			},
			confirmComple(){
				//完成
				completion({id:this.completionEdit}).then((res)=>{
					if(res.code==0){
						uni.showToast({
							title:'计划完成！',
							icon:'none'
						})
						this.list();
					}
				}).catch((err) => {
					this.setMsgTop(err)
				})
			
			},
			handComplete(item){
				//完成
				this.$refs.promptMsgComple.noticeOpen(
					"确认完成跟进客户为"+item.CustomerName+"的计划吗?"
				)
				this.completionEdit=item.Id
			},
			confirmUnbind(){
				//删除
				FollowPlan({id:this.EditId}).then((res)=>{
					if(res.code==0){
						uni.showToast({
							title:'删除成功！',
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
					"你确定要删除跟进客户为 "+item.CustomerName+"的计划吗?"
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
					this.followPlanData = []
					this.list()
				} else {
					delete this.queryData.Key
					this.queryData.pageNum = 1
					this.status = 'loading'
					this.followPlanData = []
					this.list()
				}
			},
			list(){
				//商机列表（私海）
				if(this.queryData.pageNum==1){
					this.followPlanData=[]
				}
				flowDataList(this.queryData).then((res)=>{
					if(res.code==0){
						res.data.List.map((row) => {
						   row.ExecutorName=(row.ExecutorUsers.map(rs=>rs.RealName)).join(',')
						})
						this.followPlanData = [...this.followPlanData, ...res.data.List];
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
<style>
	page{
		background: #F5F8F9;
	}
</style>
<style lang="less" scoped>
	
	.poolsList-item{
		// padding:0rpx!important;
	}
	
	.Customer-height{
		height:30rpx;
	}
	.poolsList-top-title{
		font-size:30rpx!important;
	}
	.time{
		font-size:24rpx!important;
		margin-top:7rpx!important;
	}
	.FollowPlanNav{
		width:100%;
		background: #fff;
		padding-top:20rpx;
		color:#333;
	}
	.FollowPlanNav-parItem{
		display: flex;
		padding:0px 146rpx;
		justify-content: space-between;
		font-size:28rpx;
	}
	.FollowPlanNav-parItem-border{
		width: 30rpx;
		height:5rpx;
		background: rgba(35, 113, 255, 1);
		border-radius: 3rpx;
		margin:0 auto;
		margin-top:8rpx;
	}
	.active{
		color:rgba(51, 51, 51, 1);
		font-weight: bold;
	}
	.on{
		color:rgba(153, 153, 153, 1);
	}
</style>