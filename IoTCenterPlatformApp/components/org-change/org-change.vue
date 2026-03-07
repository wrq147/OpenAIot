<template>
	<uni-popup ref="notification" :mask-click="true" :safe-area="true" mask-background-color="rgba(0, 0, 0, 0.5)"
		type="left" backgroundColor="#fff">
		<view class="notification">
			<!--#ifndef MP-WEIXIN -->
			<top :isLeftSlot="true" :isRightSlot="true" :title="userInfo.name" leftWidth="60rpx" :titleIsLeft="true" backgroundColor="#FFFFFF">
			<!--#endif -->
			<!--#ifdef MP-WEIXIN -->
			<top :hasSeat="true" seatHeight="88rpx" :isLeftSlot="true" :isRightSlot="true" :title="userInfo.name" leftWidth="60rpx" :titleIsLeft="true" backgroundColor="#FFFFFF">
			<!--#endif -->
				<template v-slot:top_left>
					<view class="left_top">
						<view v-if="userInfo.Avatar&&userInfo.Avatar.indexOf('profile.png')>-1" class="image t-icon-morentouxiang1"></view>
						<image class="image" :src="userInfo.avatar" mode="aspectFit" v-else-if="userInfo.avatar">
						</image>
						<view class="image t-icon-morentouxiang1" v-else></view>
					</view>
				</template>
				<template v-slot:top_right>
					<view class="right_top" @click.stop="closeLeftPopup">
						<custom-icons iconsName="icon-crmtianjiaguanbi" iconsSize="36rpx"
							iconsColor="rgba(153, 153, 153, 1)"></custom-icons>
					</view>
				</template>
			</top>
			<view class="fix_org">
				<view class="popup_title">我的企业</view>
				<view class="org_list">
					<view class="org_li" v-for="item in orgList" :class="{'active':item.Id==orgId}" @click.stop="handSelect(item)">
						<view class="li_info">
							<image class="image" :src="item.Logo" mode="" v-if="item.Logo"></image>
							<view class="image t-icon-qiyemorentupian1" v-else></view>
							<view class="text">
								{{item.OrgName}}
							</view>
						</view>
						<view class="li_icon" v-if="item.Id==orgId">
							<custom-icons iconsName="icon-wancheng" iconsSize="28rpx" iconsColor="#2371FF"></custom-icons>
						</view>
					</view>
				</view>
			</view>
		</view>
	</uni-popup>
</template>

<script>
	import {
		SwitchEnterprise //切换企业
	} from "@/api/personalCenter";
	export default {
		name: "orgchange",
		props: {
			orgList: {
				type: [Array],
				default: ()=>{
					return []
				}
			},
			orgId: {
				type:[String,Number],
				default: ''
			},
			userInfo:{
				type: Object,
				default: ()=>{
					return {}
				}
			},
			jumpUrl:{
				type:[String],
				default: '/pages/index/index'
			},
		},
		data() {
			return {
				isShowDraw:false
			};
		},
		methods:{
			handSelect(ite) {
				SwitchEnterprise({
					id: ite.Id
				}).then(async (res) => {
					if (res.code == 0) {
						this.$store.commit('SET_hasLoadVersion',false)
						this.$store.commit('SET_ROLES', [])
						this.$store.commit('SET_PERMISSIONS', [])
						// this.dataInfo();
						this.$store.commit('SET_MESSAGE_INFO', true)
						//消息取消订阅
						// let uid=this.$store.getters.uid
						// this.$store.dispatch("mqttclient/getClient").then((client) => {
						// 	let tkey = "user/" + uid + "/new";
						// 	client.unsubscribe(tkey, (error) => {
						// 		console.info("取消订阅dddd22", error)
						// 		this.$store.commit("mqttclient/Del_Handler", tkey);
						// 	});
						// });
						this.closeLeftPopup()
						uni.showToast({
							title: '切换成功！',
							icon: 'none'
						})
						// this.$refs.promptMsg.loadingOpen('Loading...')
						try { //切换企业后重新设置权限信息
							await this.$store.dispatch("GetInfo");
							this.$store.commit('SET_UID', '');
							// this.$refs.promptMsg.loadingColse()
							// this.$refs.mytab.loadCheck()
							uni.reLaunch({
								url: '/page_register/other?path='+this.jumpUrl
							})
						} catch (e) {
							//TODO handle the exception
							console.log("eeeeeee", e);
							// this.$refs.promptMsg.loadingColse()
						}
			
					}
				}).catch((err) => {
					//console.log(err,'1111111')
					this.setMsgTop(err)
				})
			},
			openLeftPopup() {
				//打开左侧切换企业弹窗
				this.$refs.notification.open()
				this.isShowDraw=true
			},
			closeLeftPopup() {
				//关闭左侧弹出层
				this.$refs.notification.close()
				this.isShowDraw=false
			},
		}
	}
</script>

<style lang="less">
.notification {
		width: 100%;
		min-height: 100vh;
		overflow-y: scroll;
		background-color: #fff;
		position: relative;

		.fix_org {
			position: absolute;
			width: 100%;
		}

		.popup_title {
			font-size: 28rpx;
			color: rgba(153, 153, 153, 1);
			padding: 52rpx 20rpx 30rpx;
			width: 100%;
			box-sizing: border-box;
			line-height: 28rpx;
		}

		.org_list {
			width: 100%;
			padding: 0 20rpx;
			box-sizing: border-box;

			.org_li {
				display: flex;
				justify-content: space-between;
				align-items: center;
				height: 140rpx;
				width: 100%;
				padding: 30rpx;
				box-sizing: border-box;

				&.active {
					background-color: #F8F8F8;
				}

				.li_info {
					display: flex;
					justify-content: flex-start;
					align-items: center;

					.image {
						width: 80rpx;
						height: 80rpx;
						border-radius: 10rpx;
					}

					.text {
						margin-left: 30rpx;
						font-size: 32rpx;
						color: #333333;
						font-weight: bold;
					}
				}
			}
		}
	}
</style>