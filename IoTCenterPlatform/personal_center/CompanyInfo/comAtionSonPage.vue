<template>
	<view id="comAtionSonPage">
		<top  leftIcon="icon-fanhui" :isleftBack="true" leftText="Back" backgroundColor="#161A26" title="Extended operation" class="CRM-header"></top>
		<view class="comAtionSonPage-section">
			<view class="section-name">
				Company ownership
			</view>
			<view class="section-atver">
				<image class="section-atver-tou" :src="userInfo.Avatar"></image>
				<view class="section-atver-txt">{{userInfo.RealName}}</view>
			</view>
			<view class="Transfer" @click="handTransfer">
				Transfer
			</view>
			<view class="section-name">
				Delete Company
			</view>
			<view class="section-details">
				Once you delete the enterprise, all projects, 
				departments, members, content, and associated 
				documents within the enterprise will be 
				permanently deleted. This is an irreversible 
				operation, please treat it with caution!
			</view>
			<view class="Delete" @click="handDelete()">Delete</view>
		</view>
		<msg-prompt ref="promptMsg" @confirm="confirmUnbind"></msg-prompt>
		<uni-popup    ref="activePopup" type="center" :mask-click="true" :zIndex="999" backgroundColor="rgba(22, 26, 38, 1)">
			<view class="notice_con" style="width: 90vw;">
				<view class="notice_con-title">
					<view>Transfer of Company</view>
					<view @click="handChose" class="iconfont icon-crmtianjiaguanbi"></view>
				</view>
				<input v-model="searchValue" type="text" placeholder="Please enter username or phone numb" class="notice_con-input" @input="selectSearchMember">
			</view>
			
				<view v-for="(item,index) in searchMemberList" :key="index" class="notice_con-list" @click="handClickTan(item)">
					<view style="display: flex;padding:20rpx 0rpx;align-items: center;">
						<image style="width: 60rpx;height:60rpx;border-radius: 50%;margin-left:25rpx;"  class="notice_con-list-Avatar" :src="item.Avatar" alt=""></image>
						<view  class="notice_con-list-title" style="margin-left:15rpx;">{{item.RealName}}</view>
					</view>
				</view>
				<view style="height:60rpx;"></view>
		</uni-popup>
		<msg-prompt ref="promptMsg"  @confirm="confirmUnbind"></msg-prompt>
		<msg-prompt ref="promptMsgTitle"  @confirm="confirmTitle"></msg-prompt>
	</view>
	
</template>


<script>
	import {
	removeOrg,
	dataList,
	listMember,
	handoverOrg
	} from "@/api/personalCenter";
import { getTransitionRawChildren } from "vue";
	export default {
		data(){
			return{
				OrgId:'',
				userInfo:'',
				searchMemberList:[],
				searchValue:'',
				selectedMenber:{}
			}
		},
		onLoad(option) {
			if(option.id){
				this.OrgId=option.id
			}
			this.dataInfo();
			
		},
		methods:{
			handChose(){
				this.$refs.activePopup.close()
			},
			confirmTitle(){
				  handoverOrg({ id: this.selectedMenber.OrgId, uid: this.selectedMenber.Id }).then(
				        res => {
				          if (res.code == 0) {
				            uni.showToast({
				            	title:'Transfer successful'
				            })
				            this.$refs.activePopup.close()
				          }
				        }
				      );
			},
			handClickTan(ite){
				this.$refs.promptMsgTitle.noticeOpen(
					'Are you sure to transfer the enterprise to'+ite.RealName+'After handover, your role will become a member?'
					)
					this.selectedMenber=ite
					//console.log(ite)
			},
			    selectSearchMember(query) {
			      //全局搜索指定用户
			      // if (query !== "") {
			      
			      listMember({ key: this.searchValue, showAll: true }).then(res => {
			        // for (let i = 0; i < 10; i++) {
			        //   res.data = [...res.data, ...res.data];
			        // }
			        console.log("查询的员工列表", res);
			
			        setTimeout(() => {
			          
			          this.searchMemberList = res.data.List.filter(item => {
			            console.log("filter", item, this.searchValue);
			            return item;
			          });
			        }, 200);
			      });
			      // } else {
			      //   this.searchMemberList = [];
			      // }
			    },
			dataInfo() {
				//个人信息
				dataList().then((res) => {
					if (res.code == 0) {
						//console.log(res,'个人信息')
						this.userInfo=res.data.user
						//console.log(res.data.user.OrgId,'res.data.user.OrgId')
					}
				})
			},
			handTransfer(){
				this.$refs.activePopup.open()
				this.selectSearchMember("")
			}, 
			handDelete(){
				this.$refs.promptMsg.noticeOpen(
					'Confirm deletion of enterprise?'
					)
			},
			confirmUnbind(){
				removeOrg({
					id:this.OrgId
				}).then((res)=>{
					if(res.code==0){
						uni.showToast({
							icon:'none',
							title:'Delete successful'
						})
						this.$store.commit('orgLis/SET_CHANGE_ORG',true)
						this.$store.commit('orgLis/SET_ORG_LIST', null)
						setTimeout(()=>{
							uni.switchTab({
								url:'/pages/profile/profile'
							})
						},500)
						
					}
				}).catch((err)=>{
					this.setMsgTop(err)
				})
			}
		}
	}
</script>

<style lang="less" scoped>
	#comAtionSonPage{
		color:#fff;
		width: 96%;
		margin:0rpx 4%;
		.notice_con{
			width: 100%;
			padding:0 30rpx!important;
			margin-bottom: 30rpx;
			.notice_con-list{
				color:#fff;
				
					.notice_con-list-Avatar{
						width: 60rpx;
						height: 60rpx;
						border-radius: 50%;
					}
					.notice_con-list-title{
						
					}
				
			
			}
			.notice_con-title{
				color:#fff;
				text-align: center;
				padding:20rpx 0rpx;
				font-size:36rpx;
				margin-top:10rpx;
				justify-items: center;
				.icon-crmtianjiaguanbi{
					position: absolute;
					margin-left:auto;
					color:rgba(250, 250, 250, .2);
					top:30rpx;
					right:30rpx;
				}
			}
			.notice_con-input{
				border:1rpx solid rgba(250, 250, 250, .2);
				padding:20rpx;
				border-radius: 8rpx;
				margin-top:20rpx;
				
			}
		}
		.comAtionSonPage-section{
			.Transfer,.Delete{
				width: 180rpx;
				height:80rpx;
				line-height: 80rpx;
				text-align: center;
				border-radius: 10rpx;
				margin-top:35rpx;
				font-size:32rpx;
				background: linear-gradient(180deg,rgba(255, 53, 53, 1),rgba(255, 97, 61, 1));
			}
			.Delete{
				background: rgba(28, 34, 50, 1);
			}
			.section-atver{
				display: flex;
				align-items: center;
				margin-top:15rpx;
				font-size:32rpx;
				.section-atver-txt{
					margin-left:15rpx;
				}
				.section-atver-tou{
					width:60rpx;
					height:60rpx;
					border-radius: 50%;
				}
			}
			.section-details{
				font-size:32rpx;
			}
			.section-name{
				padding:20rpx 0rpx;
				font-size:32rpx;
				margin-top:25rpx;
				color:rgba(250, 250, 250, .4);
			}
			
		}
	}
	.title{
		color:red;
		background: red;
		color:red;
	}
</style>