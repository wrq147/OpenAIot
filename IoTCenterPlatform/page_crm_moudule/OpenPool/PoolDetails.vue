<template>
	<view id="PoolDetails">
		<top :isleftBack="true" leftIcon="icon-fanhui" leftText="Back" backgroundColor="#161A26" title="Details" class="addPool-top"></top>
		<view class="PoolDetails-content">
			<view class="Avatar-information">
				<view class="iconfont t-icon-gonghaitouxiang">
				
				</view>
				
				<view class="Avatar-information-title">
					{{poolsDetails.CustomerName}}
				</view>
				<view class="Avatar-information-content">
					NO.{{poolsDetails.CustomerNumber}}  
				</view>
				<view class="Agent" v-if="poolsDetails.CustomerType==0">
					Agent
				</view>
				<view class="DirectSales" v-if="poolsDetails.CustomerType==1">
					Direct Sales
				</view>
			</view>
			<view class="Avatar-information-text">
				<view class="Avatar-information-text-item itemTextBorde" v-if="poolsDetails.HelperUsers!=null">
					<view class="Avatar-information-text-item-title">Collaborator</view>
					<view class="Avatar-information-text-flex">
						<view style="margin-right:30rpx;" class="Avatar-information-text-item-header"  v-for="(item,index) in poolsDetails.HelperUsers" :key="index">
							<img :src="item.Avatar" alt="" class="Avatar-information-text-item-img">
							<view class="content">{{item.RealName}}</view>
						</view>
					</view>
				</view>
				
				<view class="Avatar-information-text-item" v-if="poolsDetails.FromType">
					<view class="Avatar-information-text-item-title">Customer source</view>
					<view class="Avatar-information-text-item-header">
						<view class="content" v-if="poolsDetails.FromType=='weixin'">WeChat Clues</view>
						<view class="content" v-if="poolsDetails.FromType=='form'">Process Form</view>
						<view class="content" v-if="poolsDetails.FromType=='other'">other</view>
					</view>
				</view>
				<view class="Avatar-information-text-item" v-if="poolsDetails.CompanyUrl">
					<view class="Avatar-information-text-item-title">Company website</view>
					<view class="Avatar-information-text-item-header">
						<view class="content">{{poolsDetails.CompanyUrl}}</view>
					</view>
				</view>
				<view class="Avatar-information-text-item" v-if="poolsDetails.CompanyTel">
					<view class="Avatar-information-text-item-title">Telephone number</view>
					<view class="Avatar-information-text-item-header">
						<view class="content">{{poolsDetails.CompanyTel}}</view>
					</view>
				</view>
				<view class="Avatar-information-text-item" v-if="IndustryName">
					<view class="Avatar-information-text-item-title">Industry Type</view>
					<view class="Avatar-information-text-item-header">
						<view class="content">{{IndustryName}}</view>
					</view>
				</view>
				<view class="Avatar-information-text-item" v-if="poolsDetails.AddressName">
					<view class="Avatar-information-text-item-title">Company address</view>
					<view class="Avatar-information-text-item-header">
						<view class="content">{{poolsDetails.AddressName}}</view>
					</view>
				</view>
				<view class="Avatar-information-text-item" v-if="poolsDetails.AddressDetail">
					<view class="Avatar-information-text-item-title">Customer details</view>
					<view class="Avatar-information-text-item-header">
						<view class="content">{{poolsDetails.AddressDetail}}</view>
					</view>
				</view>
				<view class="Avatar-information-text-item " v-if="poolsDetails.createTime">
					<view class="Avatar-information-text-item-title">Creation date</view>
					<view class="Avatar-information-text-item-header">
						<view class="content">{{poolsDetails.createTime}}</view>
					</view>
				</view>
			</view>
		</view>
		<view class="PoolDetails-Height"></view>
		<view class="PoolDetails-button">
			<view class="button" @click="handReceive()">
				<text class="iconfont icon-lingqu"></text>
				<text >Receive</text>
			</view>
			<view class="button" @click="handEdit()">
				<text class="iconfont icon-bianji"></text>
				<text>Edit</text>
			</view>
			<view class="button" @click="handDelete()">
				<text class="iconfont icon-shanchu"></text>
				<text>Delete</text>
			</view>
		</view>
		<msg-prompt ref="promptMsg" @confirm="confirmUnbind"></msg-prompt>
		<msg-prompt ref="promptReceive" @confirm="confireCloseDrawer"></msg-prompt>
	</view>
</template>

<script>
	import {
		DataPoolsDetails,//（公海池详情）
		PoolDelete,//公海池删除
		ReceiveDraw,//公海池领取
		IndustryType,//行业类型
	} from "@/api/crmApi";
	import {
		setPagesParam
	} from '@/common/utillib.js'
	export default {
		data(){
			return{
				poolsDetails:[],
				EditId:'',
				IndustryName:''
			}
		},
		onLoad(option) {
			if(option.id){
				this.list(option.id)
				this.EditId=option.id
				
			}
		},
		methods:{
			listIndustry(typeId) {
				//行业类型
			IndustryType().then((res)=>{
				if(res.code==0){
					//console.log(res,typeId,'获取行业列表')
					for(var i=0;i<res.data.length;i++){
						if(res.data[i].Id==typeId){
							//console.log(res.data[i])
							this.IndustryName=res.data[i].Name
						}
					}
					
				}
				
			})
			},
			handDelete(){
				//删除
				this.$refs.promptMsg.noticeOpen(
					"Are you sure to delete the customers "+this.poolsDetails.CustomerName+"?"
				)
			},
			confirmUnbind(){
				//删除
				PoolDelete({id:this.EditId}).then((res)=>{
					if(res.code==0){
						uni.showToast({
							title:'Delete successful！',
							icon:'none'
						})
						setTimeout(() => {
							setPagesParam('list')
						}, 500)
					}
				}).catch((err) => {
					this.setMsgTop(err)
				})
			},
			confireCloseDrawer(){
				//领取
				ReceiveDraw({id:this.EditId}).then((res)=>{
					if(res.code==0){
						uni.showToast({
							title:'Successfully claimed！',
							icon:'none'
						})
					setTimeout(() => {
						setPagesParam('list')
					}, 500)
					}
				}).catch((err) => {
					this.setMsgTop(err)
				})
			},
			handReceive(){
				//领取
				this.$refs.promptReceive.noticeOpen(
					"Are you sure to receive the customer named "+this.poolsDetails.CustomerName+"?"
				)
			},
			handEdit(){
				uni.navigateTo({
					url:'./addPool?id='+this.poolsDetails.Id
				})
			},
			list(id){
				DataPoolsDetails({id:id}).then((res)=>{
					if(res.code==0){
						console.log(res,'res详情')
						this.poolsDetails=res.data;
						this.listIndustry(this.poolsDetails.Industry);
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
	.DirectSales{
						position: absolute;
						right:-0rpx;
						top:-0rpx;
						padding:0rpx 10rpx;
						text-align: center;
						font-size:20rpx;
						border-top-right-radius: 15rpx;
						border-bottom-left-radius: 15rpx;
						background: linear-gradient(180deg,#EFA902,#F4BE3F);
					}
					.Avatar-information-text-flex{
						display: flex;
						align-items: center;
					}
					// .activePadd{
					// 	margin-top:20rpx;
					// }
				
</style>