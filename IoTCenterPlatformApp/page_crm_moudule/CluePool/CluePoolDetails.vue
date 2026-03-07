<template>
	<view id="PoolDetails">
		<top leftIcon="icon-fanhui" :isleftBack="true" backgroundColor="#ffffff" title="详情" class="addPool-top"></top>
		<view class="PoolDetails-content">
			<view class="PoolDetails-htmlContent-clueDetails">
			<view class="Avatar-information">
				<view class="iconfont t-icon-xiansuochitouxiang1">
				
				</view>
				<view class="Avatar-information-title">
					{{arr.CompanyName}}
				</view>
				
				<view class="Avatar-Contacts">
					<view class="Avatar-Contacts-left">
						<text>联系人: </text>
						<!-- <text class="iconfont icon-a-youjiantoubai"></text> -->
					</view>
					<view class="Avatar-Contacts-right">
						<view class="Avatar-Contacts-right-David">
							<!-- <text class="aviter iconfont t-icon-morentouxiang"></text> -->
							<text class="content">{{arr.RealName}}</text>
						</view>
						
					</view>
					<view class="iconfont t-icon-dianhua1" @click="handMobile()">
						<!-- {{arr.Mobile}} -->
					</view>
				</view>
			</view>
			</view>
			<view class="Avatar-information-parse">
			<view class="Avatar-information-text">
			
				
				<view class="Avatar-information-text-item">
					<view class="Avatar-information-text-item-title">归属部门</view>
					<view class="Avatar-information-text-item-header">
						<view class="content">{{arr.DeptName}}</view>
					</view>
				</view>
				<view class="Avatar-information-text-item">
					<view class="Avatar-information-text-item-title">职务</view>
					<view class="Avatar-information-text-item-header">
						<view class="content">{{arr.PostName}}</view>
					</view>
				</view>
				<view class="Avatar-information-text-item">
					<view class="Avatar-information-text-item-title">线索来源</view>
					<view class="Avatar-information-text-item-header">
						<view class="content" v-if="arr.FromType=='weixin'">微信线索</view>
						<view class="content" v-else-if="arr.FromType=='form'">流程表单</view>
						<view class="content" v-else>其他</view>
					</view>
				</view>
				<view class="Avatar-information-text-item" v-if="arr.HelperName">
					<view class="Avatar-information-text-item-title">协作人</view>
					<view class="Avatar-information-text-item-header">
						<view class="content" v-for="(item,index) in arr.HelperUsers" :key="index" style="margin-top:15rpx;">
							<img  class="contentImg" :src="item.Avatar" alt="" />
							<view class="contentText">
								{{item.RealName}}
							</view>
						</view>
					</view>
				</view>
				<view class="Avatar-information-text-item">
					<view class="Avatar-information-text-item-title">公司电话</view>
					<view class="Avatar-information-text-item-header">
						<view class="content">{{arr.Mobile}}</view>
					</view>
				</view>
				<view class="Avatar-information-text-item itemTextBorde">
					<view class="Avatar-information-text-item-title">线索池详情</view>
					<view class="Avatar-information-text-item-header">
						<view class="content">{{arr.Remark}}</view>
					</view>
				</view>
				
				
			</view>
			</view>
		</view>
		<view class="PoolDetails-Height"></view>
		<view class="PoolDetails-button">
			<view class="button" @click.stop="handReceive()">
				<text class="iconfont icon-lingqu"></text>
				<text >领取</text>
			</view>
			<view class="button" @click="handEdit()">
				<text class="iconfont icon-bianji"></text>
				<text>编辑</text>
			</view>
			<view class="button" @click.stop="handDelete()">
				<text class="iconfont icon-shanchu"></text>
				<text>删除</text>
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
						// console.log('调用成功!')
					},
					fail: (res) => {
						// console.log('调用失败!')
					}
				})
			},
			confirmUnbind(){
				//删除
				poolPubRemove({id:this.arr.Id}).then((res)=>{
					if(res.code==0){
						uni.showToast({
							title:'删除成功！',
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
					"你确定要删除名为 "+this.arr.CompanyName+"的线索吗?"
				)
			},
			confireCloseDrawer(){
				//领取
				retrieval({id:this.arr.Id}).then((res)=>{
					if(res.code==0){
						uni.showToast({
							title:'领取成功！',
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
					"您确定要领取名为 "+this.arr.CompanyName+"的线索吗?"
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
<style>
	page{
		background: #F5F8F9;
	}
</style>
<style lang="less" scoped>
	
		
		.t-icon-morentouxiang{
			
		}
		.PoolDetails-htmlContent-clueDetails{
			padding:0rpx 3%;
			background: #ffffff;
		}
		.Avatar-information-parse{
			width: 94%;
			margin:25rpx 3%;
		}
	
	
</style>