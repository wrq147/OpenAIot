<template>
	<view id="Customer">
		<top leftIcon="icon-fanhui" :isleftBack="true" leftText="Back" backgroundColor="#161A26" title="Clues" rightIcon="icon-a-tianjiabantouming" class="CRM-header">
			<template v-slot:top_right>
				<view class="right_top" @click="handAdd()">
					<custom-icons iconsName="icon-tianjia" iconsSize="36rpx"></custom-icons>
				</view>
			</template>
		</top>
		<search-compt ref="selectCompt" @openSelect="openSelect" @searching="searching" pal="Please enter the clue name"></search-compt>
		<select-compt  ref="selectCompt" :haidate="true" :timeQuery="timeQuery" :querydata="queryData"
			@selectFinsh="selectFinsh" fixedHeight="216rpx"></select-compt>
			<view style="height:22rpx;width:100%;"></view>
	<view class="poolsList" v-for="(item,index) in customerData" :key="index" @click="handDetails(item.Id)">
		<view class="poolsList-item">
			<view class="poolsList-top">
				<view class="poolsList-top-title">{{item.CompanyName}}</view>
				<view class="time">Contact: {{item.RealName}}</view>
			</view>
			<view class="poolsList-item-button">
				<view class="button" @click.stop="handReturn(item.Id)">
					<text class="iconfont icon-lingqu"></text>
					<text >return</text>
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
	<view class="move" v-if="alertHide"></view>
	<uni-load-more iconType="circle" :status="status" v-if="status" />
		<msg-prompt ref="promptMsg" @confirm="confirmUnbind"></msg-prompt>
		<msg-prompt ref="promptReceive" @confirm="confireCloseDrawer"></msg-prompt>
		<!--添加弹窗-->
		<view class="AddPopUps" v-if="alertHide">
			<view class="AddPopUps-item">
				<view >
					<view class="AddPopUps-title" style="margin-top:25rpx;">
						<view class="title">
							Return to customer
						</view>
						<view class="iconfont t-icon-guanbidanchuang" @click="handClose">
				
						</view>
					</view>
					<view>
						<textarea value="" v-model="reason" placeholder="Please enter the reason for return"
							class="addPool-textarea" :rows="4" />
						<view class="addPool-button" @click="handConfirm">
							confirm
						</view>
					</view>
				</view>
			
			
		
			</view>
		</view>
	</view>
</template>

<script>
	import {
		xianSuo,//线索
		culeRemove,//删除
		ClueReturn//线索退回
	} from "@/api/crmApi";
	
	export default {
		data(){
			return{
				reason:'',
				alertHide: false,
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
			handConfirm() {
				if (this.reason == '') {
					uni.showToast({
						title: 'Please enter the reason for return',
						icon: 'none'
					})
					return
				}
				ClueReturn({
					id: this.EditId,
					reason:this.reason
				}).then((res) => {
					if (res.code == 0) {
						uni.showToast({
							title: 'Return successful！',
							icon: 'none'
						})
						this.alertHide = false
						this.reason=''
					     this.list();
					  
					}
				}).catch((err) => {
					this.setMsgTop(err)
				})
			},
			handClose() {
				this.alertHide = false;
			},
			handReturn(id) {
				this.alertHide = true
				this.EditId=id
			},
			handDelete(item){
				//删除
				this.$refs.promptMsg.noticeOpen(
					"Are you sure to delete the customers "+item.CompanyName+"?"
				)
				this.EditId=item.Id
			},
			confirmUnbind(){
				//删除
				culeRemove({id:this.EditId}).then((res)=>{
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
					url:'./addClue?id='+id
				})
			},
			handAdd(){
				uni.navigateTo({
					url:'./addClue'
				})
			},
			handDetails(id){
				uni.navigateTo({
					url:'./clueDeails?id='+id
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
				xianSuo(this.queryData).then((res)=>{
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
	page{
		background: #161A26;
		.addPool-button {
			width: 100%;
			height: 80rpx;
			line-height:80rpx;
			text-align: center;
			border-radius: 10rpx;
			color: #fff;
			margin:0 auto;
			background: linear-gradient(180deg, #ff3535, #ff613d);
		}
		
		.addPool-textarea {
			height: 290rpx;
			border: 1rpx solid rgba(255, 255, 255, .2);
			border-radius: 10rpx;
			margin: 20rpx auto;
			padding: 20rpx;
		}
		.move {
			position: fixed;
			height: 100%;
			width: 100%;
			background: #000;
			opacity: .7;
			top: 0px;
			left: 0px;
			z-index: 222;
		}
		.AddPopUps {
			position: fixed;
			background: #161A26;
			z-index: 333;
			color: #fff;
			width: 90%;
			margin: 0px 5%;
			border-radius: 8rpx;
			top: 250rpx;
		
			.addPool-selected {
				/deep/.uni-select__input-text {
					color: #fff !important;
				}
		
				/deep/.uni-select {
					height: 88rpx !important;
					border: 1rpx solid rgba(255, 255, 255, .2);
				}
		
				/deep/.uni-select__input-placeholder {
					font-size: 30rpx !important;
					color: rgba(255, 255, 255, .4) !important;
				}
			}
		
			.link {
				color: rgba(255, 255, 255, .4);
				margin: 0rpx 3%;
				margin-top: 30rpx;
			}
		
			.GenerateLink {
				text-align: center;
				height: 90rpx;
				border-radius: 6px;
				line-height: 90rpx;
				width: 94%;
				margin: 0 auto;
				margin-top: 20px;
				background: linear-gradient(180deg, #ff3535, #ff613d);
			}
		
			.lianUrlInput {
				height: 88rpx;
				line-height: 88rpx;
				padding: 0rpx 20rpx;
				// border:2rpx solid rgba(255, 255, 255, .2);
				border-radius: 4rpx;
				margin-top: 15rpx;
				display: flex;
				align-items: center;
				border-radius: 10rpx;
		
				.lianUrl-input {
					width: 100%;
				}
			}
		
			.AddPopUps-title {
				position: relative;
				// margin:20rpx 30rpx;
				align-items: center;
		
				.title {
					text-align: center;
					font-size: 34rpx;
				}
		
				.t-icon-guanbidanchuang {
					position: absolute;
					margin-left: auto;
					width: 40rpx;
					height: 40rpx;
					right: 0rpx;
					top: 5rpx;
				}
			}
		
			.AddPopUps-item {
				padding: 30rpx;
		
			}
		}
	}
</style>