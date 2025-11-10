<template>
	<view id="Customer">
		<top :isleftBack="true" leftIcon="icon-fanhui"  leftText="Back" backgroundColor="#161A26" title="High seas"  class="CRM-header">
			<template v-slot:top_right>
				<view class="right_top" @click="handAddPage">
					<custom-icons iconsName="icon-tianjia" iconsSize="36rpx"></custom-icons>
				</view>
			</template>
		</top>
		<search-compt ref="selectCompt" @openSelect="openSelect" @searching="searching" pal="Please enter the customer name"></search-compt>
		<select-compt  ref="selectCompt" :haidate="true" :timeQuery="timeQuery" :querydata="queryData"
			@selectFinsh="selectFinsh" fixedHeight="216rpx"></select-compt>
			<view style="height:22rpx;width:100%;"></view>
		<view class="poolsList" v-for="(item,index) in poolsList" :key="index" @click="handDetails(item.Id)">
			<view class="poolsList-item">
				<view class="poolsList-top">
					<view class="poolsList-top-title">{{item.CustomerName}}</view>
					<view class="time">{{item.createTime}} created</view>
					<view class="Agent" v-if="item.CustomerType==0">
						Agent<!--Agent代理0 直销为1-->
					</view>
					<view class="DirectSales" v-if="item.CustomerType==1">
						Direct Sales
					</view>
				</view>
				
				<view class="poolsList-item-button">
					<view class="button" @click.stop="handReceive(item)">
						<text class="iconfont icon-lingqu"></text>
						<text >Receive</text>
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
		<msg-prompt ref="promptReceive" @confirm="confireCloseDrawer"></msg-prompt>
	</view>
</template>
<script>
	import {
		DataPoolsList,//客户列表（公海池）
		PoolDelete,//公海池删除
		ReceiveDraw//公海池领取
	} from "@/api/crmApi";
	export default {
		data(){
			return{
				
				key:'',
				status: 'loading',
				queryData:{
					// Belong:1,
					pageNum:1,
					pageSize:30,
				},
				poolsList:[],//公海池列表
				EditId:'',
				timeQuery: {
					name: 'date',
					params: ['beginTime', 'endTime'],
					value: [],
				},
			}
		},
		onLoad() {
			this.list();
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
			confireCloseDrawer(){
				//领取
				ReceiveDraw({id:this.EditId}).then((res)=>{
					if(res.code==0){
						uni.showToast({
							title:'Successfully claimed！',
							icon:'none'
						})
						this.list();
					}
				}).catch((err) => {
					this.setMsgTop(err)
				})
			},
			handReceive(item){
				//领取
				this.$refs.promptReceive.noticeOpen(
					"Are you sure to receive the customer named "+item.CustomerName+"?"
				)
				this.EditId=item.Id
			},
			handDelete(item){
				//删除
				this.$refs.promptMsg.noticeOpen(
					"Are you sure to delete the customers "+item.CustomerName+"?"
				)
				this.EditId=item.Id
			},
			confirmUnbind(){
				//删除
				PoolDelete({id:this.EditId}).then((res)=>{
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
			handEdit(id){
				uni.navigateTo({
					url:'./addPool?id='+id
				})
			},
			handAddPage(){
				uni.navigateTo({
					url:'./addPool'
				})
			},
			searching(val) {
				this.key = val
				if (this.key) {
					this.queryData.Key = this.key
					this.queryData.pageNum = 1
					this.status = 'loading'
					this.poolsList = []
					this.list()
				} else {
					delete this.queryData.Key
					this.queryData.pageNum = 1
					this.status = 'loading'
					this.poolsList = []
					this.list()
				}
			},
			handDetails(id){
				uni.navigateTo({
					url:'./PoolDetails?id='+id
				})
			},
			list(){
				DataPoolsList(this.queryData).then((res)=>{
					if(res.code==0){
						if (this.queryData.pageNum == 1) {
							this.poolsList = []
						}
						this.poolsList = [...this.poolsList, ...res.data.List];
						if (res.data.List.length < this.queryData.pageSize) {
							this.status = 'noMore';
						} else {
							this.status = 'more';
						}
						//console.log(res,'res')
					}
				})
			}
		}
	}
</script>

<style lang="less" scoped>
	page{
		background: #161A26;
		/deep/.search_con .search{
			width: 91%;
		}
	}

</style>

