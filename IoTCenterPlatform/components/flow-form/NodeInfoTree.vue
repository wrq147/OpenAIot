<template>
	<view>
		<view class="flow_con" v-if="nodeList&&nodeList.length>0" v-for="(ite,inx) in nodeList">
			<view class="flow_li" :class="{'noborder':!ite.Children || ite.Children.length==0}">
				<view class="name">
					<view class="text">{{ite.StepName}}</view>
				</view>
				<!-- <view class="user_info" v-for="item in ite.ActionUsers">
					<view class="avatar">
						<image class="image" :src="item.Avatar" mode=""></image>
					</view>
					<view class="info_right">
						<view class="user_name">{{item.RealName}}</view>
						<view class="time">{{item.ActionDate}}</view>
					</view>
				</view> -->
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
				<view class="dot finish">
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
		<uni-popup ref="previewPop" type="center" :mask-click="false" :zIndex="999" maskBackgroundColor="rgba(0, 0, 0, 0.9)" @maskClick="closePreview">
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
		name: "NodeInfoTree",
		components: {
			NodeInfo
		},
		props: {
			nodeList: {
				type: Array,
				default: () =>{
					return []
				}
			}
		},
		data() {
			return {
				activeImg:'',
				statusBarHeight: uni.getSystemInfoSync().statusBarHeight*2
			};
		},
		methods: {
			previewImg(img) {
				this.activeImg=img
				this.$refs.previewPop.open()
				this.$forceUpdate()
			},
			closePreview(){
				this.$refs.previewPop.close()
			}
		}
	};
</script>

<style lang="scss">
	.el-tabs__content {
		overflow: visible;
	}

	.times_text {
		color: #bcc1cf;
		font-size: 12px;
		font-weight: normal;
	}

	.name_text {
		color: #333333;
		font-size: 16px;
	}

	.el-timeline-item__timestamp.is-bottom {
		margin-top: 0;
	}

	.last_li>.el-timeline-item__tail {
		border-left: none;
	}

	.el-timeline-item__wrapper {
		padding-left: 20px;
	}
</style>