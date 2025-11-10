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
							iconsColor="#2371FF"></custom-icons>
						<!-- <view v-if="index == activeVal" class="tab-bar-icon active" :class="item.selectedIconPath"></view> -->
						<custom-icons v-else :iconsName="item.iconPath" iconsSize="44rpx"
							iconsColor="#C1C1C1"></custom-icons>
						<!-- <view v-else class="tab-bar-icon" :class="item.iconPath" :style="{'height': item.height}">
						</view> -->
					</view>
					<view class="uni-tabbar__icon" v-if="item.text=='Network'">
						<custom-icons :iconsName="item.selectedIconPath" iconsSize="44rpx"
							iconsColor="#C1C1C1"></custom-icons>
					</view>
				</view>
				<view :class="{'uni-tabbar__label':true,'active':item.text == activeVal}" v-if="item.text!='Network'">
					{{item.text_name}}
				</view>
				<view class="uni-tabbar__label" v-if="item.text=='Network'">
					{{item.text_name}}
				</view>
				<view class="noRead" v-if="item.text=='Profile'&&noReadCountNum>0">
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
				}
			}
		},
		computed: {
			noReadCountNum() {
				return this.$store.state.noReadNum
				// return 55
			}
		},
		data() {
			return {
				// noReadCount: 0, //未读消息数量
				activeVal: 'home',
				showPage: false,
				containerHeight: 400,
				tabbar: [
					{
						"pagePath": "/pages/index/index2",
						"iconPath": "icon-a-shouyeweixuanzhong", //未选中tab图标路径
						"selectedIconPath": "icon-a-shouyexuanzhong", //选中tab图标路径
						"text": "home",
						"text_name": "首页",
						"height": "44rpx",
						"visible": false
					},
					{
						"pagePath": "/pages/index/index", //页面路径
						"iconPath": "icon-a-shebeiweixuanzhong", //未选中tab图标路径
						"selectedIconPath": "icon-a-shebeiweixuanzhong", //选中tab图标路径
						"text": "Machines", //tab字体显示
						'text_name': '设备',
						"height": "43rpx",
						"visible": false
					},
					// {
					// 	"pagePath": "/pages/devices/devices", //页面路径
					// 	"iconPath": "icon-saoma", //未选中tab图标路径
					// 	"selectedIconPath": "icon-saoma", //选中tab图标路径
					// 	"text": "Network", //tab字体显示
					// 'text_name':'配网',
					// 	"height": "43rpx",
					// 	"visible": false
					// },
					{
						"pagePath": "/pages/rules/rules",
						"iconPath": "icon-zhineng",
						"selectedIconPath": "icon-zhineng",
						"text": "Rules",
						'text_name': '智能',
						"height": "42rpx",
						"visible": false
					},
					{
						"pagePath": "/pages/crm/crm",
						"iconPath": "icon-a-crmweixuanzhong",
						"selectedIconPath": "icon-a-crmxuanzhong",
						"text": "CRM",
						'text_name': '客户',
						"height": "42rpx",
						"visible": false
					},
					{
						"pagePath": "/pages/profile/profile", //页面路径
						"iconPath": "icon-a-gerenzhongxinweixuanzhong", //未选中tab图标路径
						"selectedIconPath": "icon-a-gerenzhongxinxuanzhong", //选中tab图标路径
						"text": "Profile", //tab字体显示
						'text_name': '我的',
						"height": "44rpx",
						"visible": true
					}
				]
			};
		},
		async mounted() {
			//获取未读数量
			if (this.$store.state.isReloadMessage) {
				this.$nextTick(() => {
					this.msgcount()
					setTimeout(()=>{
						if(this.$store.state.noReadNum){}else{
							this.msgcount()
						}
					},2000)
				})
			}
			await this.loadCheck()

		},
		beforeCreate: function() {
			//隐藏底部导航
			uni.hideTabBar();
		},

		methods: {
			async loadCheck() {
				if (this.isCheckPermi(["/CRMMan/"])) {
					let index = this.tabbar.findIndex(item => item.text === 'CRM');
					if(index){
						this.tabbar[index].visible = true
					}
				} else {
					let index = this.tabbar.findIndex(item => item.text === 'CRM');
					if(index){
						this.tabbar[index].visible = false
					}
				}
				let index = this.tabbar.findIndex(item => item.text === 'home');
				let index2 = this.tabbar.findIndex(item => item.text === 'Rules');
				if(this.$store.state.mobileNavType=='workOrder'){
					this.tabbar[index].visible = true
					this.tabbar[index2].visible = false
				}else{
					this.tabbar[index].visible = false
					this.tabbar[index2].visible = true
				}
				
				if (this.isCheckPermi(["/AfterService/Dev/List"])) {
					let index = this.tabbar.findIndex(item => item.text === 'Machines');
					if(index){
						this.tabbar[index].visible = true
					}
				} else {
					let index = this.tabbar.findIndex(item => item.text === 'Machines');
					if(index){
						this.tabbar[index].visible = false
					}
				}
			},
			onScanWifiConfig() {
				//配网扫码总入口
				let that = this
				uni.scanCode({
					success: function(res) {
						let tmpidx = res.result.lastIndexOf("iot");
						if (tmpidx > -1) {
							let id = res.result.substring(tmpidx);
							uni.navigateTo({
								url: '/pages_device/wifi-conf/wifi-conf?dtuId=' + id
							});
						} else {
							this.$refs.promptMsg.open('二维码无效', 2000)
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
							// plus.navigator.setStatusBarStyle('white'); //设置顶部为白色
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
					this.$store.commit('SET_NOREAD_INFO', res.data)
					// console.log("未读消息",res);
					// this.noReadCount = res.data;
					this.$emit('messageChange')
					this.$forceUpdate()
				});
			},
			msgcount() {
				if (this.isCheckPermi(["/MsgSrv/Message/List"])) {

					this.getNoReadCount();
					
					//订阅刷新消息
					if(this.$store.state.isReloadMessage){
						this.$store.commit('SET_MESSAGE_INFO', false)
						this.$store.dispatch("mqttclient/getClient").then(client => {
							let tkey = "user/" + this.$store.getters.uid + "/new";
							client.subscribe(tkey, error => {
								// console.log("订阅成功",client, error,tkey);
								if (!error) {
									this.$store.commit("mqttclient/Add_Handler", {
										key: tkey,
										func: message => {
											// console.log("消息中心",message);
											this.getNoReadCount();
											this.$store.commit('SET_MESSAGELIST_INFO', true)
										}
									});
								}
							});
						});
					}
					
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
		background-color: #FFFFFF;
		box-shadow: 0rpx -6rpx 12rpx 0rpx rgba(0, 0, 0, 0.02);
		z-index: 8;

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
				right: 30rpx;
				height: 30rpx;
				line-height: 30rpx;
				padding: 0 9rpx;
				background-color: #F93C3C;
				color: #fff;
				font-size: 20rpx;
				display: flex;
				align-items: center;
				justify-content: center;
				border-radius: 50rpx;
				min-width: 20rpx;
				-webkit-user-select: none !important;
				/* Safari */
				-moz-user-select: none !important;
				/* Firefox */
				-ms-user-select: none !important;
				/* IE/Edge */
				user-select: none !important;
				/* 标准语法 */
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
				color: #C1C1C1;
				text-align: center;
				margin-top: 5rpx;
				height: 20rpx;
				line-height: 20rpx;
				-webkit-user-select: none !important;
				/* Safari */
				-moz-user-select: none !important;
				/* Firefox */
				-ms-user-select: none !important;
				/* IE/Edge */
				user-select: none !important;
				/* 标准语法 */

				&.active {
					// color: #50A6FA;
					color: #2371FF;
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