<template>
	<view id="comAtionSonPage">
		<top  leftIcon="icon-fanhui" :isleftBack="true" backgroundColor="#ffffff" title="扩展操作" class="CRM-header"></top>
		<view class="comAtionSonPage-section">
			<view class="section-name">
				企业归属
			</view>
			<view class="section-atver">
				<image class="section-atver-tou" :src="userInfo.Avatar"></image>
				<view class="section-atver-txt">{{userInfo.RealName}}</view>
			</view>
			<view class="Transfer" @click="handTransfer">
				移交企业
			</view>
			<view class="section-name">
				删除企业
			</view>
			<view class="section-details">
				一旦你删除了企业，企业内所有项目、部门、成员，
				项目中所有内容以及所关联的所有文档将会被永久删
				除。这是一个不可恢复的操作，请谨慎对待！
			</view>
			<view class="Delete" @click="handDelete()">删除企业</view>
		</view>
		<msg-prompt ref="promptMsg" @confirm="confirmUnbind"></msg-prompt>
		<uni-popup    ref="activePopup" type="center" :mask-click="true" :zIndex="999" backgroundColor="#ffffff">
			<view class="notice_con" style="width: 90vw;">
				<view class="notice_con-title">
					<view style="color:#333;font-weight: bold;">移交企业</view>
					<view @click="handChose" class="iconfont icon-crmtianjiaguanbi"></view>
				</view>
				<input placeholder-style="color:#C1C1C1;" v-model="searchValue" type="text" placeholder="请输入用户名或手机号" class="notice_con-input" @input="selectSearchMember">
			</view>
			
				<view v-for="(item,index) in searchMemberList" :key="index" class="notice_con-list" @click="handClickTan(item)">
					<view style="display: flex;padding:20rpx 0rpx;align-items: center;">
						<image style="width: 60rpx;height:60rpx;border-radius: 50%;margin-left:25rpx;"  class="notice_con-list-Avatar" :src="item.Avatar" alt=""></image>
						<view  class="notice_con-list-title" style="margin-left:15rpx;color:#333;">{{item.RealName}}</view>
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
				            	title:'传输成功'
				            })
				            this.$refs.activePopup.close()
				          }
				        }
				      );
			},
			handClickTan(ite){
				this.$refs.promptMsgTitle.noticeOpen(
					'你确定要将企业转移到'+ite.RealName+'移交后，您的角色将成为成员?'
					)
					this.selectedMenber=ite
					//console.log(ite)
			},
			    selectSearchMember(query) {
			      //全局搜索指定用户
			      // if (query !== "") {
			      
			      listMember({ key: this.searchValue, showAll: true,isPrimaryDept:true }).then(res => {
			        setTimeout(() => {
			          this.searchMemberList = res.data.List.filter(item => {
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
						this.userInfo=res.data.user
					}
				})
			},
			handTransfer(){
				this.$refs.activePopup.open()
				this.selectSearchMember("")
			}, 
			handDelete(){
				this.$refs.promptMsg.noticeOpen(
					'是否确认删除企业？'
					)
			},
			confirmUnbind(){
				removeOrg({
					id:this.OrgId
				}).then((res)=>{
					if(res.code==0){
						uni.showToast({
							icon:'none',
							title:'删除成功'
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
<style>
	page{
		background: #ffffff;
	}
</style>
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
				color:#333;
				
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
					color:#999999;
					top:30rpx;
					right:30rpx;
				}
			}
			.notice_con-input{
				
				padding:20rpx;
				border-radius: 8rpx;
				margin-top:20rpx;
				background: #F8F8F8;
				color:#333;
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
				font-size:28rpx;
				background: linear-gradient(180deg,rgba(255, 53, 53, 1),rgba(255, 97, 61, 1));
			}
			.Transfer{
				background: #E9F1FF;
				color:#2371FF;
				margin-bottom: 60rpx;
			}
			.Delete{
				background: #FFF5F5;
				color:#FF3535;
			}
			.section-atver{
				display: flex;
				align-items: center;
				margin-top:15rpx;
				.section-atver-txt{
					margin-left:15rpx;
					color:#333;
					font-size: 32rpx;
				}
				.section-atver-tou{
					width:60rpx;
					height:60rpx;
					border-radius: 50%;
				}
			}
			.section-details{
				font-size:30rpx;
				color:#333333;
			}
			.section-name{
				padding:20rpx 0rpx;
				font-size:28rpx;
				margin-top:25rpx;
				color:#999999;
			}
			
		}
	}
	.title{
		color:red;
		background: red;
		color:red;
	}
</style>