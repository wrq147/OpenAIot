<template>
	<view>
		<view v-for="(ite,inx) in nodeList">
			<view class="flow_li" :class="{'noborder':!ite.Children || ite.Children.length==0,'flow_li_finish':ite.Active}">
				<view class="name">
					<view class="text">{{ite.StepName}}</view>
					<!-- <view class="tips" :class="{'counter_tips':ite.Tip=='会签'}" v-if="ite.Tip">
						{{ite.Tip}}
					</view> -->
				</view>
				<view class="user_sign" v-for="item in ite.ActionUsers">
					<view class="user_info_icon">
						<view class="user_info">
							<view class="avatar">
								<image class="image" :src="item.Avatar" mode=""></image>
							</view>
							<view class="info_right">
								<view class="user_name">{{item.RealName}}</view>
								<view class="time">{{item.ActionDate}}</view>
							</view>
						</view>
						<view class="type_text" v-if="item.ActionName&&item.ActionName=='同意'">
							同意
						</view>
						<view class="type_text rej" v-else-if="item.ActionName&&item.ActionName=='驳回'">
							驳回
						</view>
						<view class="type_text" v-else-if="item.ActionName">
							{{item.ActionName}}
						</view>
					</view>
					<view class="sign" v-if="item.SignImg">
						<text>签名:</text>
						<image @click="previewImg(item.SignImg)" class="image" :src="item.SignImg" mode=""></image>
					</view>
				</view>
				<template v-if="ite.ActionUsers == null&&ite.WaitUsers != null">
					<view class="user_sign_con">
						<view class="user_sign" v-for="(item,inx) in ite.WaitUsers" v-if="showAllMore||!showAllMore&&inx<=4">
							<view class="user_info_icon">
								<view class="user_info">
									<view class="avatar">
										<image class="image" :src="item.Avatar" mode=""></image>
									</view>
									<view class="info_right">
										<view class="user_name">{{item.RealName}}</view>
										<view class="time">{{item.ActionDate}}</view>
									</view>
								</view>
								<view class="type_text" v-if="item.ActionName&&item.ActionName=='同意'">
									同意
								</view>
								<view class="type_text rej" v-else-if="item.ActionName&&item.ActionName=='驳回'">
									驳回
								</view>
								<view class="type_text" v-else-if="item.ActionName">
									{{item.ActionName}}
								</view>
							</view>
							<view class="sign" v-if="item.SignImg">
								<text>签名:</text>
								<image @click="previewImg(item.SignImg)" class="image" :src="item.SignImg" mode=""></image>
							</view>
						</view>
						<view class="view_more" @click.stop="showAllMore=true" v-if="ite.WaitUsers.length>5&&!showAllMore">
							<text class="text">点击查看更多</text><uni-icons type="down" size="12" color="#bcc1cf"></uni-icons>
						</view>
						<view class="view_more" @click.stop="showAllMore=false" v-if="ite.WaitUsers.length>5&&showAllMore">
							<text class="text">收起</text><uni-icons type="up" size="12" color="#bcc1cf"></uni-icons>
						</view>
					</view>
				 </template>
				<view class="dot" :class="{'finish':ite.Active}">
					<custom-icons iconsName="icon-shenpi" iconsSize="20rpx"
						:iconsColor="ite.Active?'rgba(255, 255, 255, 1)':'rgba(255, 255, 255, 0.5)'"
						v-if="ite.StepName&&ite.StepName.indexOf('审批')>-1"></custom-icons>
					<custom-icons iconsName="icon-chaosong" iconsSize="20rpx"
						:iconsColor="ite.Active?'rgba(255, 255, 255, 1)':'rgba(255, 255, 255, 0.5)'"
						v-else-if="ite.StepName&&ite.StepName.indexOf('抄送')>-1"></custom-icons>
					<custom-icons v-else-if="ite.StepName&&ite.StepName.indexOf('发起')>-1" iconsName="icon-faqi"
						iconsSize="20rpx"
						:iconsColor="ite.Active?'rgba(255, 255, 255, 1)':'rgba(255, 255, 255, 0.5)'"></custom-icons>
					<custom-icons iconsName="icon-qitaliucheng" iconsSize="20rpx"
						:iconsColor="ite.Active?'rgba(255, 255, 255, 1)':'rgba(255, 255, 255, 0.5)'"
						v-else></custom-icons>
				</view>
				<view class="add_icon" v-if="ite.CanAdd">
					<custom-icons iconsName="icon-tianjia" iconsSize="24rpx"></custom-icons>
				</view>
			</view>
			<NodeInfo :nodeList="ite.Children" v-if="ite.Children"></NodeInfo>
		</view>
		<!-- <view v-for="(ite,inx) in nodeList" v-else>
			<view class="flow_li" :class="{'noborder':!ite.Children || ite.Children.length==0}">
				<view class="name">
					<view class="text">{{ite.StepName}}</view>
					<view class="tips">
						{{ite.Tip=='会签'?'Countersign':'eithersign'}}
					</view>
				</view>
				<view class="user_sign" v-for="item in ite.ActionUsers">
					<view class="user_info_icon">
						<view class="user_info">
							<view class="avatar">
								<image class="image" :src="item.Avatar" mode=""></image>
							</view>
							<view class="info_right">
								<view class="user_name">{{item.RealName}}</view>
								<view class="time">{{item.ActionDate}}</view>
							</view>
						</view>
						<view class="type_text" v-if="item.ActionName&&item.ActionName=='同意'">
							Agreed
						</view>
						<view class="type_text rej" v-if="item.ActionName&&item.ActionName=='驳回'">
							Rejected
						</view>
					</view>
					<view class="sign" v-if="item.SignImg">
						<text>Signature:</text>
						<image @click="previewImg(item.SignImg)" class="image" :src="item.SignImg" mode=""></image>
					</view>
				</view>
				<view class="dot" :class="{'finish':ite.Active}">
					<custom-icons iconsName="icon-shenpi" iconsSize="20rpx"
						:iconsColor="ite.Active?'rgba(255, 255, 255, 1)':'rgba(255, 255, 255, 0.5)'"
						v-if="ite.StepName&&ite.StepName.indexOf('审批')>-1"></custom-icons>
					<custom-icons iconsName="icon-chaosong" iconsSize="20rpx"
						:iconsColor="ite.Active?'rgba(255, 255, 255, 1)':'rgba(255, 255, 255, 0.5)'"
						v-else-if="ite.StepName&&ite.StepName.indexOf('抄送')>-1"></custom-icons>
					<custom-icons v-else-if="ite.StepName&&ite.StepName.indexOf('发起')>-1" iconsName="icon-faqi"
						iconsSize="20rpx"
						:iconsColor="ite.Active?'rgba(255, 255, 255, 1)':'rgba(255, 255, 255, 0.5)'"></custom-icons>
					<custom-icons iconsName="icon-qitaliucheng" iconsSize="20rpx"
						:iconsColor="ite.Active?'rgba(255, 255, 255, 1)':'rgba(255, 255, 255, 0.5)'"
						v-else></custom-icons>
				</view>
				<view class="add_icon" v-if="ite.CanAdd">
					<custom-icons iconsName="icon-tianjia" iconsSize="24rpx"></custom-icons>
				</view>
			</view>
			<NodeInfo :nodeList="ite.Children" v-if="ite.Children"></NodeInfo>
		</view> -->
		<uni-popup ref="previewPop" type="center" :mask-click="false" :zIndex="999"
			maskBackgroundColor="rgba(0, 0, 0, 0.7)" @maskClick="closePreview">
			<view class="preView_bg" v-if="activeImg">
				<image class="image" :src="activeImg" mode="aspectFit"></image>
				<view class="pop_close" @click="closePreview" :style="{'top':Number(statusBarHeight+36)+'rpx'}">
					<custom-icons iconsName="icon-guanbidanchuang" iconsSize="36rpx"
						iconsColor="rgba(255, 255, 255, 1)"></custom-icons>
				</view>
			</view>
		</uni-popup>
	</view>
</template>

<script>
	import NodeInfo from "./NodeInfo";
	export default {
		name: "NodeInfo",
		props: {
			nodeList: {
				type: Array,
				default:  () =>{
					return []
				},
			}
		},
		components: {
			NodeInfo
		},
		data() {
			return {
				activeImg: '',
				statusBarHeight: uni.getSystemInfoSync().statusBarHeight*2,
				showAllMore:false
			};
		},
		mounted() {
			// console.log(this.nodeList,'nodeListnodeListnodeListnodeList',this.nodeList.length);
			this.$forceUpdate()
		},
		methods: {
			previewImg(img) {
				this.activeImg = img
				this.$refs.previewPop.open()
				this.$forceUpdate()
			},
			closePreview() {
				this.$refs.previewPop.close()
			}
		}
	};
</script>

<style lang="less" scoped>
	// .el-tabs__content{
	//   overflow:visible;
	// }
	.preView_bg {
		width: 100vw;
		background-color: #fff;
		display: flex;
		justify-content: center;
		align-items: center;

		.pop_close {
			width: 36rpx;
			height: 36rpx;
			position: fixed;
			top: 36rpx;
			right: 36rpx;
		}
	}
</style>