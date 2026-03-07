<template>
	<view id="Customer">
		<top leftIcon="icon-fanhui" :isleftBack="true" backgroundColor="#ffffff" title="线索池" rightIcon="icon-a-tianjiabantouming" class="CRM-header">
			<template v-slot:top_right>
				<view class="right_top" @click="handAdd()">
					<custom-icons iconsName="icon-tianjia" iconsSize="36rpx"></custom-icons>
				</view>
			</template>
		</top>
		<search-compt ref="selectCompt" @openSelect="openSelect" @searching="searching" inputBg="#F8F8F8" pal="请输入线索池名称"></search-compt>
		<select-compt  ref="selectCompt" :haidate="true" :timeQuery="timeQuery" :querydata="queryData"
			@selectFinsh="selectFinsh" fixedHeight="216rpx"></select-compt>
		<view class="poolsList" v-for="(item,index) in customerData" :key="index" @click="handDetails(item.Id)">
			<view class="poolsList-item">
				<view class="poolsList-top">
					<view class="poolsList-top-title">{{item.CompanyName}}</view>
					<view class="time">联系人: {{item.RealName}}</view>
				</view>
				<view class="poolsList-item-button">
					<view class="button" @click.stop="handReceive(item)">
						<text class="iconfont icon-lingqu"></text>
						<text >领取</text>
					</view>
					<view class="button" @click.stop="handEditCull(item.Id)">
						<text class="iconfont icon-bianji"></text>
						<text>编辑</text>
					</view>
					<view class="button btnRight" @click.stop="handDelete(item)">
						<text class="iconfont icon-shanchu"></text>
						<text>删除</text>
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
		ClueData,//线索池
		retrieval,//领取
		poolPubRemove//删除
	} from "@/api/crmApi";
	
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
				EditId:'',
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
			confirmUnbind(){
				//删除
				poolPubRemove({id:this.EditId}).then((res)=>{
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
					"你确定要删除名字为 "+item.CompanyName+"的线索吗?"
				)
				this.EditId=item.Id
			},
			confireCloseDrawer(){
				//领取
				retrieval({id:this.EditId}).then((res)=>{
					if(res.code==0){
						uni.showToast({
							title:'已成功领取！',
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
					"您确定要领取名为 "+item.CompanyName+"的线索吗?"
				)
				this.EditId=item.Id
			},
			handEditCull(id){
				uni.navigateTo({
					url:'./addCluePool?id='+id
				})
			},
			handAdd(){
				uni.navigateTo({
					url:'./addCluePool'
				})
			},
			handDetails(id){
				uni.navigateTo({
					url:'./CluePoolDetails?id='+id
				})
			},
		
			handClickDetails(id){
				uni.navigateTo({
					url:'./CustomerDetails?id='+id
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
				//客户列表（私海）
				ClueData(this.queryData).then((res)=>{
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
<style>
	page{
		background: #F5F8F9;
	}
</style>
<style lang="less" scoped>
	
</style>