<template>
	<view>
		<view style="height: calc(112rpx + env(safe-area-inset-bottom));width: 100%;"></view>
		<view class="uni-tabbar">
			<view class="uni-tabbar__item" v-for="(item,index) in tabbar" :key="index" @tap="changeTab(item,index)"
				v-if="item.visible">
				<!-- 上面使用的是字体图标，解决切换页面的时候图标会闪的效果，毕竟每切换一个页面都会闪一下不太好看，可以切换使用下面的图片方式 -->
				<view class="uni-tabbar__bd">
					<view class="uni-tabbar__icon" v-if="item.text!='Network'">
						<custom-icons v-if="item.text == activeVal" :iconsName="item.selectedIconPath" iconsSize="44rpx"
							iconsColor="rgba(255, 53, 53, 1)"></custom-icons>
						<!-- <view v-if="index == activeVal" class="tab-bar-icon active" :class="item.selectedIconPath"></view> -->
						<custom-icons v-else :iconsName="item.iconPath" iconsSize="44rpx"
							iconsColor="rgba(99, 101, 122, 1)"></custom-icons>
						<!-- <view v-else class="tab-bar-icon" :class="item.iconPath" :style="{'height': item.height}">
						</view> -->
					</view>
					<view class="uni-tabbar__icon" v-if="item.text=='Network'">
						<custom-icons :iconsName="item.selectedIconPath" iconsSize="44rpx"
							iconsColor="rgba(99, 101, 122, 1)"></custom-icons>
					</view>
				</view>
				<view :class="{'uni-tabbar__label':true,'active':item.text == activeVal}" v-if="item.text!='Network'">
					{{item.text}}
				</view>
				<view class="uni-tabbar__label" v-if="item.text=='Network'">
					{{item.text}}
				</view>
				<view class="noRead" v-if="item.text=='Messages'&&noReadCountNum>0">
					{{noReadCountNum>99?'99+':noReadCountNum}}
				</view>
			</view>
		</view>
	</view>

</template>


<script>
	import {
		noReadCount
	} from "@/api/message";
	import {
		getUserInfo
	} from '@/api/login'
	// import {
	// 	checkPermi
	// } from '@/common/permission.js';
	export default {
		name: "card-tab-bar",
		props: {
			active: {
				type: [Number, String],
				default: 0
			},
			// noRead: {
			// 	type: Number,
			// 	default: 0
			// }
		},
		watch: {
			active: {
				immediate: true,
				handler(newV, oldV) {
					this.activeVal = newV;
					// #ifdef APP-PLUS
					plus.navigator.setStatusBarStyle('white'); //设置顶部为白色
					// #endif
				}
			}
		},
		computed:{
			noReadCountNum(){
				return this.$store.state.noReadNum
			}
		},
		data() {
			return {
				// noReadCount: 0, //未读消息数量
				activeVal: 'home',
				showPage: false,
				containerHeight: 400,
				tabbar: [{
						"pagePath": "/pages/home/home",
						"iconPath": "icon-a-shouyeweixuanzhong", //未选中tab图标路径
						"selectedIconPath": "icon-a-shouyexuanzhong", //选中tab图标路径
						"text": "home",
						"height": "44rpx",
						"visible": false
					},
					{
						"pagePath": "/pages/devices/devices", //页面路径
						"iconPath": "icon-a-shebeiweixuanzhong", //未选中tab图标路径
						"selectedIconPath": "icon-a-shebeixuanzhong", //选中tab图标路径
						"text": "Machines", //tab字体显示
						"height": "43rpx",
						"visible": false
					},
					// {
					// 	"pagePath": "/pages/devices/devices", //页面路径
					// 	"iconPath": "icon-saoma", //未选中tab图标路径
					// 	"selectedIconPath": "icon-saoma", //选中tab图标路径
					// 	"text": "Network", //tab字体显示
					// 	"height": "43rpx",
					// 	"visible": false
					// },
					{
						"pagePath": "/pages/crm/crm",
						"iconPath": "icon-a-crmweixuanzhong",
						"selectedIconPath": "icon-a-crmxuanzhong",
						"text": "CRM",
						"height": "42rpx",
						"visible": false
					},
					{
						"pagePath": "/pages/message/message",
						"iconPath": "icon-a-xiaoxiweixuanzhong",
						"selectedIconPath": "icon-a-xiaoxixuanzhong",
						"text": "Messages",
						"height": "42rpx",
						"visible": false
					},
					{
						"pagePath": "/pages/profile/profile", //页面路径
						"iconPath": "icon-a-gerenzhongxinweixuanzhong", //未选中tab图标路径
						"selectedIconPath": "icon-a-gerenzhongxinxuanzhong", //选中tab图标路径
						"text": "Profile", //tab字体显示
						"height": "44rpx",
						"visible": true
					}
				]
			};
		},
		async mounted() {
			//获取未读数量
			// console.log(this.$store.state.isReloadMessage,'this.$store.state.isReloadMessage');
			if (this.$store.state.isReloadMessage) {
				this.$nextTick(() => {
					this.msgcount()
					
				})
			}
			await this.loadCheck()

		},
		beforeCreate: function() {
			//隐藏底部导航
			uni.hideTabBar();
		},

		methods: {
			async loadCheck(){
				// console.log(this.$store.state.user.roles);
				
				if(this.$store.state.user.roles&&this.$store.state.user.roles.length>0){
					if(this.$store.state.user.roles.includes(3)||this.$store.state.user.roles.includes(4)){
						this.tabbar[0].visible=true
					}else{
						this.tabbar[0].visible=false
					}
					
				}else{
					let rsp = await getUserInfo({
						id: 0
					})
					if(rsp.data.user.roleIds.includes(3)||rsp.data.user.roleIds.includes(4)){
						this.tabbar[0].visible=true
					}else{
						this.tabbar[0].visible=false
					}
					this.$store.commit('SET_ROLES',rsp.data.user.roleIds)
				}
				if(this.isCheckPermi(["/CRMMan/"])){
					this.tabbar[2].visible=true
				}else{
					this.tabbar[2].visible=false
				}
				if(this.isCheckPermi(["/CRMService/Dev/List"])){
					this.tabbar[1].visible=true
				}else{
					this.tabbar[1].visible=false
				}
				if(this.isCheckPermi(["/MsgSrv/Message/List"])){
					this.tabbar[3].visible=true
				}else{
					this.tabbar[3].visible=false
				}
			},
			onScanWifiConfig() {
				//配网扫码总入口
				let that = this
				uni.scanCode({
					success: function(res) {
						console.log("res", res);
						let tmpidx = res.result.lastIndexOf("iot");
						if (tmpidx > -1) {
							let id = res.result.substring(tmpidx);
							uni.navigateTo({
								url: '/pages/wifi-conf/wifi-conf?dtuId=' + id
							});
						} else {
							this.$refs.promptMsg.open('Invalid QR code', 2000)
						}

					}
				});
			},
			changeTab(item, idx) {
				if (item.text == 'Network') {
					this.onScanWifiConfig()
				} else {
					if (item.text == this.activeVal) {
						return;
					}
					let currentPage = item.pagePath;
					// console.log("item  Tab",item,currentPage);
					// uni.showLoading({
					// 	title: '正在加载...'
					// })
					uni.switchTab({
						url: currentPage,
						success: (e) => {
							// console.log(e);
							// uni.hideLoading();
							// #ifdef APP-PLUS
							plus.navigator.setStatusBarStyle('white'); //设置顶部为白色
							// #endif
						},
						fail: (e) => {
							// console.log(e,'err');
						}
					})
				}

			},
			getNoReadCount() {
				//获取未读数量
				noReadCount().then(res => {
					// console.log("消息未读数量", res);
					this.$store.commit('SET_NOREAD_INFO', res.data)
					
					// this.noReadCount = res.data;
					this.$forceUpdate()
				});
			},
			msgcount() {
				// console.log("yuyuuuuuuu", this.isCheckPermi(["/MsgSrv/Message/List"]));
				if (this.isCheckPermi(["/MsgSrv/Message/List"])) {
					
					this.getNoReadCount();
					this.$store.commit('SET_MESSAGE_INFO', false)
					//订阅刷新消息
					this.$store.dispatch("mqttclient/getClient").then(client => {
						let tkey = "user/" + this.$store.getters.uid + "/new";
						client.subscribe(tkey, error => {
							if (!error) {
								this.$store.commit("mqttclient/Add_Handler", {
									key: tkey,
									func: message => {
										this.getNoReadCount();
										this.$store.commit('SET_MESSAGELIST_INFO', true)
									}
								});
							}
						});
					});
				}

			},
		}
	}
</script>

<style lang="less" scoped>
	.uni-tabbar {
		position: fixed;
		bottom: 0;

		// .bottom {
		// 	bottom: env(safe-area-inset-bottom);
		// }
		padding-bottom: env(safe-area-inset-bottom);
		/*如果要在tabbar之上的话，就在加tabbar的高度，例如tabbar的高度为100rpx*/
		// .bottom{bottom: calc(100rpx + env(safe-area-inset-bottom));
		// }
		left: 0;
		z-index: 1;
		width: 100vw;
		height: 98rpx;
		display: flex;
		justify-content: space-around;
		align-items: center;
		// box-sizing: border-box;
		background-color: #1C2232;
		box-shadow: 0px -20px 40px 0px rgba(0, 0, 0, 0.0500);
		z-index: 99;

		// -webkit-tap-highlight-color: rgba(0, 0, 0, 0);//解决微信小程序点击出现蓝色背景色问题
		.uni-tabbar__item {
			display: flex;
			flex-direction: column;
			width: 20%;
			position: relative;
			cursor: pointer;
			-webkit-tap-highlight-color: rgba(0, 0, 0, 0); //解决微信小程序点击出现蓝色背景色问题
			justify-content: center;
			align-items: center;

			.noRead {
				position: absolute;
				top: -10rpx;
				right: 20rpx;
				line-height: 20rpx;
				padding: 5rpx 10rpx;
				background-color: #F93C3C;
				color: #fff;
				font-size: 20rpx;
				display: flex;
				align-items: center;
				justify-content: center;
				border-radius: 17rpx;
				min-width: 24rpx;
			}

			.uni-tabbar__bd {
				display: flex;
				justify-content: center;

				// tabBar单项
				.uni-tabbar__icon {
					// tabBar图标
					width: 44rpx;
					height: 44rpx;
					display: flex;
					justify-content: center;
					background-repeat: no-repeat;
					align-items: center;
				}
			}

			.uni-tabbar__label {
				// tabBar文字
				font-size: 20rpx;
				font-weight: 400;
				color: #63657A;
				text-align: center;
				margin-top: 5rpx;
				height: 20rpx;
				line-height: 20rpx;

				&.active {
					// color: #50A6FA;
					color: #FF3535;
					font-weight: bold;
				}
			}
		}

		.icon {
			display: inline-block;
		}
	}

	.tab-bar-icon {
		width: 44rpx;
		height: 44rpx;

		&.active {
			width: 44rpx;
			height: 44rpx;

			.images {
				width: 44rpx;
				height: 44rpx;
			}

		}

	}

	.scan_icon {
		width: 74rpx;
		height: 74rpx;


		.scan_icon_img {
			width: 74rpx;
			height: 74rpx;
		}
	}
</style>