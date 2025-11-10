<template>
	<view v-if="isshowTab">
		<view style="height: 112rpx;"></view>
		<view class="uni-tabbar">
			<view class="uni-tabbar__item" v-for="(item,index) in tabbar" :key="index" @tap="changeTab(index)"
				v-if="item.visible">
				<!-- 上面使用的是字体图标，解决切换页面的时候图标会闪的效果，毕竟每切换一个页面都会闪一下不太好看，可以切换使用下面的图片方式 -->
				<view v-if="true" class="uni-tabbar__bd">
					<view class="uni-tabbar__icon">
						<img v-if="index == activeVal" class="uni-w-42 uni-h-42" :src="item.selectedIconPath" />
						<img v-else class="uni-w-42 uni-h-42" :src="item.iconPath" />
					</view>
				</view>
				<view :class="{'uni-tabbar__label':true,'active':index == activeVal}">
					{{item.text}}
				</view>
			</view>
		</view>
	</view>

</template>


<script>
	import {
		getOrg
	} from '@/api/org.js'
	export default {
		name: "card-tab-bar",
		props: {
			active: {
				type: Number,
				default: 0
			},
			orgid: {
				type: Number,
				default: 0
			}
		},
		watch: {
			active: {
				immediate: true,
				handler(newV, oldV) {
					this.activeVal = newV;
				}
			}
		},
		data() {
			return {
				isshowTab: false,
				activeVal: 0,
				showPage: false,
				containerHeight: 400,
				tabbar: [{
						"pagePath": "/pages/card_center/card",
						"iconPath": "/static/tab/mp.png", //未选中tab图标路径
						"selectedIconPath": "/static/tab/lan_mp.png", //选中tab图标路径
						"text": "名片中心",
						"visible": true
					},
					{
						"pagePath": "/pages/card_center/card_pro", //页面路径
						"iconPath": "/static/tab/icon_pro_n.png", //未选中tab图标路径
						"selectedIconPath": "/static/tab/icon_pro_s.png", //选中tab图标路径
						"text": "产品中心", //tab字体显示
						"visible": true
					}, {
						"pagePath": "/pages/card_center/card_case",
						"iconPath": "/static/tab/icon_case_n.png",
						"selectedIconPath": "/static/tab/icon_case_s.png",
						"text": "客户案例",
						"visible": true
					}
				],
				showPro: true, //判断是否展示产品中心模块
				showCaseL: true, //判断是否展示案例中心模块

			};
		},

		async mounted() {
			// console.log("打印tabbar信息",this.tabbar);
			this.tabbar[1].visible = false //设置初始时产品中心时不打开的
			this.tabbar[2].visible = false //设置初始时案例中心时不打开的
			// this.userInfo = await this.$store.dispatch("userInfo");


			let orgInfo = await getOrg(this.orgid);
			// console.log('组织信息', orgInfo);
			if (orgInfo.data) {
				if (orgInfo.data.ProConfig) {
					let ProConfig = orgInfo.data.ProConfig;
					// console.log("产品设置信息", ProConfig);
					let config = JSON.parse(ProConfig);
					
					let config2 = JSON.parse(ProConfig)
					// console.log("uuuu", config2[0]);
					if (config2[0]) {
						// console.log("第一个子级存在");
						config = {
							oldInfo: config2
						}
					}
					// console.log("转化后产品设置信息config", config);
					if (config.oldInfo) {
						config.oldInfo.map((item, index) => {
							// console.log("子级数据", item);
							if (item.type == 'showPro') {
								this.tabbar[1].visible = item.data
							}
							if (item.type == 'title') {
								// console.log("标题存在",item.data);
								if (item.data) {
									this.tabbar[1].text = item.data
								}
							}
						})
					}
					if (config.showPro) {
						this.tabbar[1].visible = config.showPro
					}
					if (config.title) {
						this.tabbar[1].text = config.title
					}
				}

				if (orgInfo.data.CaseConfig) {
					let CaseConfig = orgInfo.data.CaseConfig;
					// console.log("案例设置信息", CaseConfig);
					let config = JSON.parse(CaseConfig);
					let config2 = JSON.parse(CaseConfig)
					// console.log("uuuu", config2[0]);
					if (config2[0]) {
						config = {
							oldInfo: config2
						}
					}
					if (config.oldInfo) {
						config.oldInfo.map((item, index) => {
							// console.log("子级数据", item);
							if (item.type == 'showCase') {
								this.tabbar[2].visible = item.data
							}
							if (item.type == 'title') {
								if (item.data) {
									this.tabbar[2].text = item.data
								}
							}
						})
					}
					if (config.showCase) {
						this.tabbar[2].visible = config.showCase
					}
					if (config.title) {
						this.tabbar[2].text = config.title
					}
					// console.log("转化后案例设置信息this.config", config);
					
				}
			}

			if (this.tabbar[1].visible || this.tabbar[2].visible) {
				this.isshowTab = true
			}


			//获取产品设置信息

		},
		beforeCreate: function() {
			//隐藏底部导航
			uni.hideTabBar();
		},
		methods: {
			changeTab(idx) {
				this.$emit('update:active', idx);
			}
		}
	}
</script>

<style lang="scss" scoped>
	.uni-tabbar {
		position: fixed;
		bottom: 0;
		z-index: 1;
		width: 100vw;
		height: 112rpx;
		display: flex;
		justify-content: space-around;
		align-items: center;
		box-sizing: border-box;
		background-color: #fff;
		box-shadow: 0px -20px 40px 0px rgba(0, 0, 0, 0.0500);

		.uni-tabbar__item {
			display: flex;
			flex-direction: column;

			.uni-tabbar__bd {
				display: flex;
				justify-content: center;

				// tabBar单项
				.uni-tabbar__icon {
					// tabBar图标
					width: 44rpx;
					height: 44rpx;

					img {
						width: 100%;
						height: 100%;
					}
				}
			}

			.uni-tabbar__label {
				// tabBar文字
				font-size: 22rpx;
				font-weight: 400;
				color: #a3a3a3;
				text-align: center;

				&.active {
					color: #50A6FA;
				}
			}
		}

		// .uni-tabbar__icon {
		// 	height: 42upx;
		// 	line-height: 42upx;
		// 	text-align: center;
		// }

		.icon {
			display: inline-block;
		}

		// .uni-tabbar__label {
		// 	line-height: 24upx;
		// 	font-size: 24upx;
		// 	color: #999;

		// 	&.active {
		// 		color: #1ca6ec;
		// 	}
		// }
	}
</style>
