<template>
	<view id="PoolDetails">
		<top leftIcon="icon-fanhui" :isleftBack="true" leftText="Back" backgroundColor="#161A26" title="Details" class="addPool-top"></top>
		<view class="PoolDetails-content">
			<view class="Avatar-information">
				<view class="iconfont t-icon-xiansuochitouxiang">
				
				</view>
				<view class="Avatar-information-title">
					{{arr.CompanyName}}
				</view>
				
				<view class="Avatar-Contacts">
					<view class="Avatar-Contacts-left">
						<text>Contacts: </text>
						<!-- <text class="iconfont icon-a-youjiantoubai"></text> -->
					</view>
					<view class="Avatar-Contacts-right">
						<view class="Avatar-Contacts-right-David">
							<text class="aviter iconfont t-icon-morentouxiang"></text>
							<text class="content">{{arr.RealName}}</text>
						</view>
						<view class="iconfont t-icon-dianhua" @click="handMobile()">
							<!-- {{arr.Mobile}} -->
						</view>
					</view>
				</view>
			</view>
			<view class="Avatar-information-text">
				<!-- <view class="Avatar-information-text-item">
					<view class="Avatar-information-text-item-title">Manager</view>
					<view class="Avatar-information-text-item-header">
						<view class="Avatar-information-text-item-img"></view>
						<view class="content">{{arr.LeaderName}}</view>
					</view>
				</view> -->
				
				<view class="Avatar-information-text-item itemTextBorde">
					<view class="Avatar-information-text-item-title">Department</view>
					<view class="Avatar-information-text-item-header">
						<view class="content">{{arr.DeptName}}</view>
					</view>
				</view>
				<view class="Avatar-information-text-item">
					<view class="Avatar-information-text-item-title">Position</view>
					<view class="Avatar-information-text-item-header">
						<view class="content">{{arr.PostName}}</view>
					</view>
				</view>
				<view class="Avatar-information-text-item">
					<view class="Avatar-information-text-item-title">Customer source</view>
					<view class="Avatar-information-text-item-header">
						<view class="content" v-if="arr.FromType=='weixin'">WeChat Clues</view>
						<view class="content" v-else-if="arr.FromType=='form'">Process Form</view>
						<view class="content" v-else>other</view>
					</view>
				</view>
				<view class="Avatar-information-text-item" v-if="arr.HelperName">
					<view class="Avatar-information-text-item-title">Collaborator</view>
					<view class="Avatar-information-text-item-header">
						<view class="content">{{arr.HelperName}}</view>
					</view>
				</view>
				<view class="Avatar-information-text-item">
					<view class="Avatar-information-text-item-title">Customer details</view>
					<view class="Avatar-information-text-item-header">
						<view class="content">{{arr.Remark}}</view>
					</view>
				</view>
				
				
			</view>
		</view>
		<view class="PoolDetails-Height"></view>
		<view class="PoolDetails-button">
			<view class="button" @click.stop="handReceive()">
				<text class="iconfont icon-lingqu"></text>
				<text >Receive</text>
			</view>
			<view class="button" @click="handEdit()">
				<text class="iconfont icon-bianji"></text>
				<text>Edit</text>
			</view>
			<view class="button" @click.stop="handDelete()">
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
		ClueDataDetails,//线索池详情
		retrieval,//领取
		poolPubRemove//删除
	} from "@/api/crmApi";
	import {
		setPagesParam
	} from '@/common/utillib.js'
	export default {
		data(){
			return{
				arr:[],
				editId:''
			}
		},
		onLoad(option) {
			if(option.id){
				this.list(option.id)
				this.editId=option.id
			}
		},
		methods:{
			handEdit(){
				uni.navigateTo({
					url:'./addCluePool?id='+this.editId
				})
			},
			handMobile(){
				uni.makePhoneCall({
					phoneNumber: this.arr.Mobile,// 这里就是自己要拨打的电话号码
					success: (res) => {
						console.log('调用成功!')
					},
					fail: (res) => {
						console.log('调用失败!')
					}
				})
			},
			confirmUnbind(){
				//删除
				poolPubRemove({id:this.arr.Id}).then((res)=>{
					if(res.code==0){
						uni.showToast({
							title:'Delete successful！',
							icon:'none'
						})
						setTimeout(()=>{
							setPagesParam('list')
						},500)
					}
				}).catch((err) => {
					this.setMsgTop(err)
				})
			},
			handDelete(item){
				//删除
				this.$refs.promptMsg.noticeOpen(
					"Are you sure to delete the customers "+this.arr.CompanyName+"?"
				)
			},
			confireCloseDrawer(){
				//领取
				retrieval({id:this.arr.Id}).then((res)=>{
					if(res.code==0){
						uni.showToast({
							title:'Successfully claimed！',
							icon:'none'
						})
						setTimeout(()=>{
							setPagesParam('list')
						},500)
					}
				}).catch((err) => {
					this.setMsgTop(err)
				})
			},
			handReceive(){
				//领取
				this.$refs.promptReceive.noticeOpen(
					"Are you sure to receive the customer named "+this.arr.CompanyName+"?"
				)
			},
			list(id){
				ClueDataDetails({
					id:id
				}).then((res)=>{
					if(res.code==0){
						//console.log(res)
						this.arr=res.data;
					}
				})
			},
		}
	}
</script>

<style lang="less" scoped>
	page{
		background: #161A26;
		.t-icon-morentouxiang{
			
		}
	}
	
</style>