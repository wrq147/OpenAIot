<template>
	<view class="contacts">
		<top :isRightSlot="true" leftIcon="icon-fanhui" :isleftBack="true" backgroundColor="#ffffff"  title="联系人" rightIcon="icon-a-tianjiabantouming"
			class="CRM-header">
			<template v-slot:top_right>
				<view class="right_top" @click="handAdd()">
					<custom-icons iconsName="icon-tianjia" iconsSize="36rpx"></custom-icons>
				</view>
			</template>
			</top>
			<search-compt ref="selectCompt" @openSelect="openSelect" @searching="searching" pal="请输入联系人名称" inputBg="#F8F8F8"></search-compt>
			<select-compt  ref="selectCompt" :haidate="true" :timeQuery="timeQuery" :querydata="queryData"
				@selectFinsh="selectFinsh" fixedHeight="216rpx"></select-compt>
			<view class="contacts-list" v-for="(item,index) in customerData" :key="index" @click="handDetails(item)">
				<view class="contacts-list-item">
					<view class="contacts-list-item-atve" style="text-align: center;">
						{{item.RealName.length>2?item.RealName.slice(1,3):item.RealName}}
					</view>
					<view class="contacts-list-item-content">
						<view class="title">{{item.RealName}}</view>
						<view class="text">{{item.DeptName}}</view>
					</view>
						<view class="iconfont t-icon-dianhua1" @click.stop="dialogConfirm(item.Mobile)"></view>
				</view>
			</view>
			<uni-load-more iconType="circle" :status="status" v-if="status" />
	</view>
</template>

<script>
	import {
		contactData,//联系人列表
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
				ContactsAll:'',
				timeQuery: {
					name: '日期',
					params: ['beginTime', 'endTime'],
					value: [],
				},
			}
		},
		onLoad(option) {
			this.list();//线索池
			if(option.ContactsAll){
				this.ContactsAll=option.ContactsAll
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
		dialogConfirm(mobile) {
			uni.makePhoneCall({
				phoneNumber: mobile,// 这里就是自己要拨打的电话号码
				success: (res) => {
					console.log('调用成功!')
				},
				fail: (res) => {
					console.log('调用失败!')
				}
			})
		},
			handAdd(){
				uni.navigateTo({
					url:'./ContactAddition'
				})
			},
			handDetails(ite){
				if(this.ContactsAll){
					setPagesParam('ContactData',ite,1);
				}else{
					uni.navigateTo({
						url:'./ContactDetails?id='+ite.Id
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
				//客户列表（私海）
				contactData(this.queryData).then((res)=>{
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

<style lang="less" scoped>
	.contacts{
		
		.contacts-list{
			display: flex;
			flex-direction: column;
			margin:20rpx 3%;
			background: #ffffff;
			border-radius: 8rpx;
			.contacts-list-item{
				display: flex;
				align-items: center;
				height: 148rpx;
				padding:0rpx 20rpx;
				
				.t-icon-dianhua1{
					width: 50rpx;
					height: 50rpx;
					line-height: 50rpx;
					text-align: right;
					font-size: 52rpx;
					margin-left:auto;
					color: #E9F1FF;
				}
				.contacts-list-item-content{
					margin-left:30rpx;
					.title{
						color:#333;
						font-size:30rpx;
						font-weight: bold;
					}
					.text{
						color:#999999;
						font-size:24rpx;
					}
				}
				.contacts-list-item-atve{
					width: 100rpx;
					height: 100rpx;
					line-height: 100rpx;
					border-radius: 50%;
					background: #2371FF;
					color:#fff;
				}
			}
		}
		.OpenPool-header{
			display: flex;
			align-items: center;
			margin-bottom: 10rpx;
			.icon-shaixuan{
				color:#fff;
				margin-left:auto;
				margin-right:20rpx;
			}
			.OpenPool-header-input{
				display: flex;
				outline: none;
				border:none;
				flex:1;
				border:1rpx solid rgba(255, 255, 255, .6);
				margin:20rpx;
				height:60rpx;
				border-radius: 8rpx;
				padding:0rpx 20rpx;
				align-items: center;
				.icon-sousuo{
					color:rgba(255,255,255,.6);
					font-size:24rpx;
				}
				.inputArrow{
					height:60rpx;
					font-size:24rpx;
					margin-left:10rpx;
					min-width: 440rpx;
					color:#fff;
				}
			}
		}
	}
</style>