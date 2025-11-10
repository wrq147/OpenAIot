<template>
	<view>
		<view style="height: 112rpx;"></view>
		<view class="uni-tabbar">
			<view class="uni-tabbar__item" v-for="(item,index) in tabbar" :key="index" @tap="changeTab(item,index)" v-show="item.visible">
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
				<view class="noRead" v-if="item.text=='消息'&&countxx>0">
					{{countxx}}
				</view>
			</view>
		</view>
	</view>

</template>


<script>
	import {getNoRead} from '@/api/record.js'
	export default {
		name: "card-tab-bar",
		props: {
			active: {
				type: Number,
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
		computed:{
			countxx(){
				// console.log("数量",this.$store.state.noread);
				return this.$store.state.noread;
			}
		},
		data() {
			return {
				// noRead:0,//未读消息数量
				activeVal: 0,
				showPage: false,
				containerHeight: 400,
				tabbar: [{
						"pagePath": "/pages/card_center/card_center",
						"iconPath": "/static/tab/mp.png", //未选中tab图标路径
						"selectedIconPath": "/static/tab/lan_mp.png", //选中tab图标路径
						"text": "我的名片",
						"visible":true
					},
					{
						"pagePath": "/pages/holder/holder", //页面路径
						"iconPath": "/static/tab/tx.png", //未选中tab图标路径
						"selectedIconPath": "/static/tab/lan_tx.png", //选中tab图标路径
						"text": "通讯录", //tab字体显示
						"visible":true
					}, {
						"pagePath": "/pages/info/info",
						"iconPath": "/static/tab/xx.png",
						"selectedIconPath": "/static/tab/lan_xx.png",
						"text": "消息",
						"visible":true
					},
					{
						"pagePath": "/pages/user/user", //页面路径
						"iconPath": "/static/tab/wd.png", //未选中tab图标路径
						"selectedIconPath": "/static/tab/lan_wd.png", //选中tab图标路径
						"text": "我的", //tab字体显示
						"visible":true
					}
				]
			};
		},
		mounted() {
			//获取未读数量
			this.$store.dispatch("noread");
			
		},
		beforeCreate: function() {
			//隐藏底部导航
			uni.hideTabBar();
		},
		methods: {
			changeTab(item,idx) {
				if(idx==this.activeVal){
					return;
				}
				let currentPage = item.pagePath;
				// console.log("item  Tab",item);
				uni.showLoading({
					title: '正在加载...'
				})
				this.$store.dispatch("noread");
				uni.switchTab({
					url: currentPage,
					success: (e) => {
						uni.hideLoading();
					},
					fail: (e) => {}
				})
			},
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
		box-shadow: 0px -20px 40px 0px rgba(0,0,0,0.0500);
		
		.uni-tabbar__item {
			display: flex;
			flex-direction: column;
			width: 25%;
			position: relative;
			cursor: pointer;
			.noRead{
				position: absolute;
				top: 0;
				right: 30%;
				background-color: red;
				color: #fff;
				font-size: 16rpx;
				width: 30rpx;
				height: 30rpx;
				display: flex;
				align-items: center;
				justify-content: center;
				border-radius: 50%;
			}
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
