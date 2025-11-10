<template>
	<view class="profile" :class="[isShowDraw?'profileActive':'profileOn']">
		<uni-nav-bar :status-bar="true" :fixed="true" :border="false" height="108rpx" backgroundColor="#161A26">
			<view class="profile-header">
				<view class="iconfont icon-qiehuangongsi" @click="showDrawer"></view>
				<!-- <view class="iconfont icon-xiaoxizhongxin"></view> -->
			</view>
		</uni-nav-bar>
		<view class="profile-header-img">
			<!--<view class="profile-header-img-tou">
				 {{userInfo.Avatar}} 
			</view>-->
			<image class="profile-header-img-tou" :src="userInfoImg" alt="">
		</view>
		<view class="profile-header-img-name">
			{{userInfo.RealName}}
		</view>
		<view class="profile-header-img-content">
			<view class="iconfont icon-gongsi"></view>
			<view>
				{{userInfo.dept_name}}
			</view>
		</view>
		<view class="ProcessManagement" v-if="isCheckPermi(['/FlowService/Flow'])">
			<view class="ProcessManagement-title">Process management</view>
			<view class="ProcessManagement-content">
				<view class="ProcessManagement-content-item" @click="toProcessList(0)">
					<view class="num">
						{{Interface.PendingCount}}
					</view>
					<view class="text">
						To-do
					</view>
				</view>

				<view class="ProcessManagement-content-item" @click="toProcessList(1)">
					<view class="num">
						{{Interface.FinishCount}}
					</view>
					<view class="text">
						Completed
					</view>
				</view>

				<view class="ProcessManagement-content-item" @click="toProcessList(2)">
					<view class="num">
						{{Interface.CopyCount}}
					</view>
					<view class="text">
						Cc to me
					</view>
				</view>

				<view class="ProcessManagement-content-item" @click="toProcessList(3)">
					<view class="num">
						{{Interface.MyFlowCount}}
					</view>
					<view class="text">
						Created
					</view>
				</view>
			</view>
			<view class="ProcessManagement-CreateProcess" @click="toCreateProcess">
				+ Create process
			</view>
		</view>
		<view class="InformationData">
			<view class="InformationData-item" @click="handPersonInfo()">
				<view class="InformationData-item-text">
					Personal Information
				</view>
				<view class="iconfont icon-a-youjiantoubai">
				</view>
			</view>

			<view class="InformationData-item" @click="handCompanyInfo()">
				<view class="InformationData-item-text">
					Company information
				</view>
				<view class="iconfont icon-a-youjiantoubai">
				</view>
			</view>

			<view class="InformationData-item" @click="handDearcm()">
				<view class="InformationData-item-text">
					Department management
				</view>
				<view class="iconfont icon-a-youjiantoubai">
				</view>
			</view>

			<view class="InformationData-item" @click="handStaff()">
				<view class="InformationData-item-text">
					Staff management
				</view>
				<view class="iconfont icon-a-youjiantoubai">
				</view>
			</view>

			<view class="InformationData-item" @click="handRoleMent()">
				<view class="InformationData-item-text">
					Role management
				</view>
				<view class="iconfont icon-a-youjiantoubai">
				</view>
			</view>
		</view>

		<uni-drawer ref="showRight" mode="left" :mask-click="false">
			<scroll-view style="height: 100%;" scroll-y="true">
				<view class="uni-popup-alert">
					<uni-nav-bar :status-bar="true" :fixed="true" :border="false" height="108rpx"
						backgroundColor="#161A26">
						<view class="uni-popup-alert-item">
							<view class="uni-popup-alert-item-left">
								<view class="popupNameCutting" v-if="!userName&&!userName.length<=0">
									{{userName.length>2?userName.slice(-2):userName}}
								</view>
								<image v-else class="uni-popup-alert-item-atve" :src="userInfo.Avatar" alt=""></image>
								<view class="title">
									{{userInfo.RealName}}
								</view>
							</view>

							<view class="iconfont icon-guanbidanchuang" @click="handChose">

							</view>
						</view>
					</uni-nav-bar>

					<view class="uni-popup-alert-title">
						My enterprise
					</view>
					<view style="height: 100%;" scroll-y="true">

						<view class="uni-popup-alert-enterprise" :class="[select==item.Id?'active':'on']"
							v-for="(item,index) in OrganData" :key="index" @click="handSelect(index,item.Id,item)">
							<view class="uni-popup-alert-enterprise-parse">
								<view class="uni-popup-alert-enterprise-tou">
									<image style="width: 100%;height:100%;" :src="item.Logo+'?wh=500x500'" alt="" />
								</view>
								<view class="uni-popup-alert-enterprise-text">
									{{item.OrgName}}
								</view>
							</view>
							<view class="iconfont icon-xuanzhongqiye" v-if="select==item.Id">

							</view>
						</view>


						<view class="uni-popup-alert-enterprise" @click="handCreatePage()">
							<view class="uni-popup-alert-enterprise-parse">
								<view class="createAdd">
									+
								</view>
								<view class="uni-popup-alert-enterprise-text">
									Create company
								</view>
							</view>
						</view>
						<view style="height:260rpx;"></view>
						<view class="Positioning">
							<view class="DeleteAccount" @click="handAccount()">Cancel account</view>
							<view class="LogOut" @click="closeDrawer">
								Log out
							</view>
						</view>
					</view>
				</view>

			</scroll-view>
		</uni-drawer>
		<msg-prompt ref="promptMsg" @confirm="confirmUnbind"></msg-prompt>
		<msg-prompt ref="promptClose" @confirm="confireCloseDrawer"></msg-prompt>
		<my-tab-bar ref="mytab" active="Profile"></my-tab-bar>
	</view>
</template>

<script>
	import {
		removeToken,
		removeRefreshToken
	} from "@/common/auth";
	import {
		dataList, //个人中心列表
		ReportInterface, //流程报表接口
		OrganizationList, //组织列表
		SwitchEnterprise, //切换企业
		GetDefaultData, //获取企业信息
		deleteAccountLogin //删除账号
	} from "@/api/personalCenter";
	import {
		checkPermi
	} from '@/common/permission.js';
	export default {
		data() {
			return {
				userName: '',
				isShowDraw: false,
				MorenInfo: [],
				userInfo: [],
				Interface: [],
				OrganData: [],
				infoData: {},
				OrgId: '',
				select: 0,
				arr: [{
						title: 'Fujian Huade Group'
					},
					{
						title: 'Huade'
					}
				],
				userInfoImg: ''
			}
		},

		async onLoad(options) {
			this.$store.commit('orgLis/SET_ORG_LIST', null)
			this.OrganData = await this.$store.dispatch("orgLis/setOrgList");
			// 监听物理返回按钮
			uni.showLoading({
				title: 'loading'
			})
			//await this.$store.dispatch('GetInfo')

			//	await this.$store.dispatch('GetInfo')
			if (this.$store.state.user && this.$store.state.user.uid) {
				this.userInfoImg = this.$store.state.user.avatar
			} else {
				await this.$store.dispatch('GetInfo')
				this.userInfoImg = this.$store.state.user.avatar
			}
			//console.log(this.userInfo,'this.userInfo')
			this.$nextTick(() => {
				this.$refs.mytab.loadCheck()
			})
			uni.hideLoading()
			this.list();
			this.dataInfo();

		},
		async onShow() {
			// console.log(this.$store.state.isReloadReport, 'this.$store.state.isReloadReport');
			if (this.$store.state.isReloadReport) {
				this.reLoadReport()
			}
			if (this.$store.state.orgLis.orgList == null) {
				this.OrganData = await this.$store.dispatch("orgLis/setOrgList");
			}
			if (this.isChangeOrg) {
				if (this.OrganData && this.OrganData.length > 0) {
					let item = this.OrganData[0]
					this.handSelect(0, item.Id, item)
					this.$store.commit('orgLis/SET_CHANGE_ORG', false)
				}

			}
		},
		computed: {
			isChangeOrg() { //是否进行切换企业操作
				return this.$store.state.orgLis.isChangeOrg
			}
		},
		onBackPress(options) {
			if (options.from === 'backbutton') {
				// 来自于导航条返回按钮或者系统返回按钮
				// 返回 `true` 表示拦截操作，不再继续执行返回操作
				// 返回 `false` 表示不拦截返回操作，继续执行默认的返回操作
				if (this.isShowDraw) {
					this.handChose()
					return true; // 根据需要返回true或false
				} else {
					return false; // 根据需要返回true或false
				}
			}
		},
		methods: {
			reLoadReport() {
				//流程报表接口
				ReportInterface().then((res) => {
					if (res.code == 0) {
						//console.log(res,'流程报表接口')
						this.Interface = res.data
						this.$store.commit('SET_UPDATE_REPORT', false)
					}
				})
			},
			handStaff() {
				uni.navigateTo({
					url: '../../personal_center/StaffManagement/index'
				})
			},
			handRoleMent() {
				uni.navigateTo({
					url: '../../personal_center/Roles/index'
				})
			},
			handDearcm() {
				uni.navigateTo({
					url: '../../personal_center/Department/index'
				})
			},
			isCheckPermi(val) {
				return checkPermi(val)
			},
			loadData(query) {
				//更新
				console.log(query, 'query');
			},
			toProcessList(val) {
				//跳转至流程列表
				uni.navigateTo({
					url: '/pages_flow/process_list?item=' + val
				})
			},
			toCreateProcess() {
				//创建流程
				uni.navigateTo({
					url: '/pages_flow/add_process'
				})
			},
			//个人信息
			handPersonInfo() {
				uni.navigateTo({
					url: '../../personal_center/personData/personData?id=' + this.userInfo.Id
				})
			},
			//公司信息
			handCompanyInfo() {
				uni.navigateTo({
					url: '../../personal_center/CompanyInfo/CompanyInfo?id=' + this.OrgId
				})
			},
			confirmUnbind() {
				deleteAccountLogin({

				}).then((res) => {
					if (res.code == 0) {
						removeToken();
						removeRefreshToken();
						this.$store.commit('orgLis/SET_ORG_LIST', null)
						uni.reLaunch({
							url: '/pages/index/login'
						})
					}
				}).catch((err) => {
					this.setMsgTop(err)
				})
			},
			handAccount() {
				this.$refs.promptMsg.noticeOpen(
					'Confirm deleting this account. After logging out, you will not be able to log in?'
				)

			},
			//获取企业信息
			information(id) {
				//console.log(id)
				GetDefaultData({
					id: id
				}).then((res) => {
					if (res.code == 0) {
						//console.log(res.data,'获取企业信息');
						this.MorenInfo = res.data;
					}
				})
			},
			showDrawer() {
				this.$refs.showRight.open();
				this.isShowDraw = true
			},
			handChose() {
				this.$refs.showRight.close();
				this.isShowDraw = false
			},
			closeDrawer() {
				this.$refs.promptClose.noticeOpen(
					'Are you sure you want to log out?'
				)
			},
			confireCloseDrawer() {
				this.$store.dispatch("mqttclient/getClient").then((client) => {
					let tkey = "user/" + this.$store.getters.uid + "/new";
					client.unsubscribe(tkey, (error) => {
						console.info("取消订阅dddd22", error)
						this.$store.commit("mqttclient/Del_Handler", tkey);
					});
				});
				this.$store.commit('SET_ROLES', [])
				this.$store.commit('SET_PERMISSIONS', [])
				this.$store.commit('orgLis/SET_ORG_LIST', null)
				removeToken();
				removeRefreshToken();
				uni.reLaunch({
					url: '/pages/index/login'
				})
			},
			handCreatePage() {
				this.$refs.showRight.close();
				this.isShowDraw = false
				uni.navigateTo({
					url: '/personal_center/createCompany/createCompany'
				})
			},
			dataInfo() {
				//个人信息
				dataList().then((res) => {
					if (res.code == 0) {
						//console.log(res,'个人信息')
						this.userInfo = res.data.user
						this.select = res.data.user.OrgId
						//this.information(res.data.user.OrgId)
						this.OrgId = res.data.user.OrgId
						this.userName = res.data.user.RealName
						//console.log(res.data.user.OrgId,'res.data.user.OrgId')
					}
				})
			},
			list() {
				//流程报表接口
				ReportInterface().then((res) => {
					if (res.code == 0) {
						//console.log(res,'流程报表接口')
						this.Interface = res.data
					}
				})
				//组织列表
				// OrganizationList().then((res) => {
				// 	if (res.code == 0) {
				// 		//	console.log(res,'组织列表')
				// 		this.OrganData = res.data;
				// 	}
				// })
			},
			close() {

			},
			handSelect(inx, id, ite) {
				SwitchEnterprise({
					id: id
				}).then(async (res) => {
					if (res.code == 0) {
						uni.showToast({
							title: 'Switch successful',
							icon: 'none'
						})
						this.$store.commit('SET_ROLES', [])
						this.$store.commit('SET_PERMISSIONS', [])
						this.select = inx;
						this.infoData = ite;
						// this.dataInfo();
						this.$store.commit('SET_MESSAGE_INFO', true)
						//消息取消订阅
						this.$store.dispatch("mqttclient/getClient").then((client) => {
							let tkey = "user/" + this.$store.getters.uid + "/new";
							client.unsubscribe(tkey, (error) => {
								console.info("取消订阅dddd22", error)
								this.$store.commit("mqttclient/Del_Handler", tkey);
							});
						});

						this.$refs.showRight.close();
						this.isShowDraw = false
						// this.$refs.promptMsg.loadingOpen('Loading...')
						try { //切换企业后重新设置权限信息
							await this.$store.dispatch("GetInfo");
							// this.$refs.promptMsg.loadingColse()
							// this.$refs.mytab.loadCheck()
							this.$store.commit('SET_UID', '');
							uni.reLaunch({
								url: '/pages/index/other'
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
			handBread() {
				this.$refs.popup.open()
			}
		}
	}
</script>

<style lang="less">
	.profileActive {
		position: absolute;
	}

	.rules_list_con {
		position: relative;
		z-index: 1;
	}

	.profile {
		position: relative;

		.Positioning {
			position: fixed;
			bottom: 0rpx;
			width: 100%;
			background: rgba(22, 26, 38, 1);
			height: 200rpx;

			.LogOut {
				position: absolute;
				bottom: 20rpx;
				left: 0rpx;
				width: 94%;
				margin: 0rpx 3%;
				height: 100rpx;
				line-height: 100rpx;
				text-align: center;
				background: rgba(28, 34, 50, 1);
				color: rgba(255, 255, 255, .4);
				font-size: 36rpx;
				z-index: 10;

			}
		}

		.DeleteAccount {
			position: absolute;
			width: 94%;
			margin: 0 3%;
			height: 100rpx;
			line-height: 100rpx;
			justify-content: space-evenly;
			align-items: center;
			background: rgba(28, 34, 50, 1);
			color: rgba(255, 255, 255, .4);
			font-size: 32rpx;
			border-radius: 8rpx;
			margin-top: 28rpx;
			margin-bottom: 10rpx;
			text-align: center;
			bottom: 130rpx;
		}

		/deep/.uni-drawer {
			//position: absolute;
		}

		/deep/.uni-drawer__content {
			width: 100vw !important;
			background: rgba(22, 26, 38, 1) !important;

		}

		/deep/.uni-tabbar {
			z-index: 55;
		}

		/deep/.uni-navbar__header-container {
			display: block;
			padding: 0rpx;
		}

		/deep/.uni-navbar__header {
			background: rgba(22, 26, 38, 1) !important;
			color: #fff !important;
			padding: 0rpx;
		}

		/deep/.uni-navbar__header-btns-right,
		/deep/.uni-navbar__header-btns-left {
			display: none;
		}

		.uni-popup-alert {
			position: relative;
			width: 100vw !important;
			background: rgba(22, 26, 38, 1) !important;
			color: #fff;
			z-index: 888;
			// min-height: calc(100vh - 150rpx);
			// padding-bottom: 150rpx;

			/deep/.uni-navbar__header-container {
				display: block;

			}

			/deep/.uni-navbar__header {
				background: rgba(22, 26, 38, 1) !important;
				color: #fff !important;
			}



			.uni-popup-alert-enterprise {
				height: 140rpx;
				// line-height: 140rpx;
				border-radius: 8rpx;
				margin: 0rpx 3%;
				align-items: center;
				display: flex;
				padding: 0rpx 30rpx;

				.icon-xuanzhongqiye {
					margin-left: auto;
				}

				.uni-popup-alert-enterprise-parse {
					display: flex;
					align-items: center;

					.createAdd {
						width: 80rpx;
						height: 80rpx;
						line-height: 80rpx;
						text-align: center;
						font-size: 40rpx;
						background: rgba(28, 34, 50, 1);
						border-radius: 8rpx;
					}

					.uni-popup-alert-enterprise-text {
						color: #fff;
						font-size: 36rpx;
						margin-left: 30rpx;
					}

					.uni-popup-alert-enterprise-tou {
						width: 80rpx;
						height: 80rpx;
						background: #fff;
						border-radius: 8rpx;
					}

				}

			}

			.active {
				background: rgba(28, 34, 50, 1);
			}

			.on {
				background: none;
			}

			.uni-popup-alert-title {
				margin: 20rpx 3%;
				font-size: 32rpx;
				color: rgba(255, 255, 255, .6);
			}

			.uni-popup-alert-item {
				margin: 0rpx 3%;
				padding: 20rpx 0rpx;
				display: flex;
				align-items: center;
				padding-top: 25rpx;

				.uni-popup-alert-item-left {
					display: flex;
					align-items: center;

					.popupNameCutting {
						width: 60rpx;
						height: 60rpx;
						border-radius: 50%;
						line-height: 60rpx;
						text-align: center;
						color: #fff;
						font-size: 20rpx;
						background: rgba(28, 34, 50, 1);
					}

					.title {
						margin-left: 20rpx;
						font-size: 36rpx;
					}

					.uni-popup-alert-item-atve {
						width: 60rpx;
						height: 60rpx;
						//background: red;
						border-radius: 50%;
					}
				}

				.icon-guanbidanchuang {
					margin-left: auto;
					font-size: 38rpx;
				}

			}
		}

		.InformationData {
			display: flex;
			flex-direction: column;
			margin: 0rpx 3%;

			.InformationData-item {
				display: flex;
				color: #fff;
				padding: 0rpx 20rpx;
				height: 110rpx;
				line-height: 110rpx;
				background: rgba(28, 34, 50, 1);
				justify-content: space-between;
				border-radius: 6rpx;
				margin-bottom: 20rpx;

				.InformationData-item-text {
					font-size: 32rpx;
				}

				.icon-a-youjiantoubai {
					font-size: 26rpx;
				}
			}
		}

		.ProcessManagement {
			margin: 30rpx 20rpx;
			border: 1rpx solid rgba(255, 255, 255, .2);
			border-radius: 8rpx;
			padding: 20rpx 25rpx;

			.ProcessManagement-CreateProcess {
				display: flex;
				height: 88rpx;
				line-height: 88rpx;
				justify-content: space-evenly;
				align-items: center;
				background: linear-gradient(180deg, rgba(255, 53, 53, 1), rgba(255, 97, 61, 1));
				color: #fff;
				font-size: 32rpx;
				border-radius: 8rpx;
				margin-top: 28rpx;
				margin-bottom: 10rpx;
			}

			.ProcessManagement-content {
				display: flex;
				text-align: center;
				//padding: 30rpx 0rpx;
				padding-top: 20rpx;
				//border-bottom: 1rpx solid rgba(255, 255, 255, .2);

				.ProcessManagement-content-item {
					width: 25%;

					.num {
						color: #fff;
						font-size: 32rpx;
					}

					.text {
						color: rgba(255, 255, 255, .4);
						margin-top: 10rpx;
						font-size: 28rpx;
					}
				}

			}

			.ProcessManagement-title {
				font-size: 32rpx;
				color: #fff;
			}
		}

		.profile-header-img-name {
			color: #fff;
			text-align: center;
			font-size: 36rpx;
			margin-top: 20rpx;
		}

		.profile-header-img-content {
			display: flex;
			color: rgba(255, 255, 255, .4);
			justify-content: center;
			align-items: center;
			font-size: 28rpx;
			margin-top: 15rpx;
			margin-bottom: 20rpx;

			.icon-gongsi {
				margin-right: 10rpx;
			}
		}

		.profile-header-img {
			width: 180rpx;
			height: 180rpx;
			border-radius: 50%;
			background: url('../../static/headerLogo.png') no-repeat;
			background-size: 100% 100%;
			margin: 0 auto;
			padding: 13rpx;

			.profile-header-img-tou {
				width: 180rpx;
				height: 180rpx;
				// background: green;
				border-radius: 50%;
				margin: 0 auto;
			}
		}

		.profile-header {
			width: 94%;
			margin: 0rpx 3%;
			padding: 20rpx 0rpx;
			display: flex;
			justify-content: space-between;

			.icon-qiehuangongsi {
				font-size: 40rpx;
				color: #fff;
			}

			.icon-xiaoxizhongxin {
				font-size: 40rpx;
				color: #fff;
			}
		}
	}
</style>