<script>
	import store from '@/store/index.js'
	export default {
		globalData: { //全局数据
			jt: '', //域名，便于调用接口
			info: '',
			cid:0
		},
		onLaunch: function(options) {
			//获取设备信息
			let pla = uni.getSystemInfoSync().platform;
			// console.log("设备类型",pla);
			store.commit("SET_SYSTYPE_DATA", pla);
			//加载省市区数据
			store.dispatch("areaTree");
			//加载行业数据
			store.dispatch("industryTree");
			//隐藏底部导航
			uni.hideTabBar();
			// #ifdef APP-PLUS
			// 锁定屏幕方向
			plus.screen.lockOrientation('portrait-primary');
			// App平台检测升级，服务端代码是通过uniCloud的云函数实现的，详情可参考：https://ext.dcloud.net.cn/plugin?id=4542
			if (plus.runtime.appid !== 'HBuilder') { // 真机运行不需要检查更新，真机运行时appid固定为'HBuilder'，这是调试基座的appid
				checkUpdate()
			}
			// #endif

			// #ifdef APP-PLUS
			//token标志来判断
			let token = getToken();
			if (token) {
				//存在则进入首页
				plus.navigator.closeSplashscreen();
			}
			// #endif

			// #ifdef MP-WEIXIN
			console.log("onLaunch接收的传的值", options);
			if (options.query && options.query.jt && options.query.info) {
				console.log("info", options.info);
				this.$options.globalData.jt=options.query.jt
				this.$options.globalData.info=options.query.info
			} else if (options.query && options.query.jt && options.query.cid) {
				console.log('options.cid', options.cid);
				this.$options.globalData.jt=options.query.jt
				this.$options.globalData.cid=options.query.cid
			}
			// #endif
		},
		onShow: function() {

		},
		onHide: function() {

		}
	}
</script>


<style lang="scss">
	/*每个页面公共css */
	page {
		font-size: 28rpx;
		font-family: -apple-system, BlinkMacSystemFont, "PingFang SC", "Helvetica Neue", STHeiti, "Microsoft Yahei", Tahoma, Simsun, sans-serif;
		background-color: #F6F6F6;
		color: #333333;
	}

	input {
		display: block;
		outline: none;
	}


	/****半像素边框***/
	/*#ifndef APP-PLUS-NVUE*/
	.border-top,
	.border-bottom,
	.border-left,
	.border-right,
	.border-all {
		position: relative;
	}

	.border-top:after,
	.border-bottom:after,
	.border-left:after,
	.border-right:after,
	.border-all:after {
		border: 0 solid #E5E5E5;
		bottom: -50%;
		box-sizing: border-box;
		content: " ";
		left: -50%;
		pointer-events: none;
		position: absolute;
		right: -50%;
		top: -50%;
		transform: scale(.5);
		transform-origin: center;
	}

	.border-top:after {
		border-top-width: 1px;
	}

	.border-left:after {
		border-left-width: 1px;
	}

	.border-right:after {
		border-right-width: 1px;
	}

	.border-bottom:after {
		border-bottom-width: 1px;
	}

	.border-all:after {
		border-width: 1px;
	}


	/*#endif*/
	/* #ifdef APP-PLUS-NVUE */
	.border-top {
		border-top-width: 1px;
		border-top-style: solid;
		border-top-color: #E5E5E5;
	}

	.border-bottom {
		border-bottom-width: 1px;
		border-bottom-style: solid;
		border-bottom-color: #E5E5E5;
	}

	.border-left {
		border-left-width: 1px;
		border-left-style: solid;
		border-left-color: #E5E5E5;
	}

	.border-right {
		border-right-width: 1px;
		border-right-style: solid;
		border-right-color: #E5E5E5;
	}

	.border-all {
		border-width: 1px;
		border-style: solid;
		border-color: #E5E5E5;
	}

	/* #endif */

	/****清除按钮默认样式***/
	button::after {
		border: none;
	}

	.button-hover {
		opacity: 0.8 !important;
	}


	/* 公共样式 */
	.common_wrap {
		width: 100%;

		.row>text {
			color: #333333;
		}

		.row {
			width: 100%;
			height: 120rpx;
			display: flex;
			justify-content: space-between;
			align-items: center;
			padding: 0 30rpx;
			box-sizing: border-box;
			font-size: 30rpx;
			background-color: #FFFFFF;
			border-bottom: 1rpx solid #F6F6F6;

			.left {
				color: #333333;
				width: 200rpx;
			}

			.right {
				display: flex;
				align-items: center;

				text {
					color: #999999;
					margin-right: 12rpx;
				}

				input {
					text-align: right;
					width: 100%;
				}
			}
		}

		.row_gap {
			margin: 30rpx auto;
		}
	}

	.common_btn {
		width: 690rpx;
		height: 90rpx;
		line-height: 90rpx;
		text-align: center;
		background: linear-gradient(270deg, #2CC2FB 0%, #50A6FA 100%);
		color: #fff;
		font-size: 30rpx;
		margin: 0 auto;
		border-radius: 8rpx;
		letter-spacing: 3rpx;
	}

	.common_title {
		height: 120rpx;
		line-height: 120rpx;
		text-align: left;
		padding: 0 30rpx;
		box-sizing: border-box;
		background-color: #FFFFFF;
		font-size: 34rpx;
		font-family: pfzho;
	}

	.border-btn {
		height: 140rpx;
		width: 100%;
		border-top: 1rpx solid #F6F6F6;
		padding: 20rpx 30rpx;
		box-sizing: border-box;
		position: fixed;
		bottom: 0;
		left: 0;

		.btn {
			width: 100%;
			height: 100rpx;
			line-height: 100rpx;
			text-align: center;
			color: #FFFFFF;
			background: linear-gradient(270deg, #2CC2FB 0%, #50A6FA 100%);
			font-size: 30rpx;
			border-radius: 8rpx;

		}
	}

	.bot-btn {
		width: 690rpx;
		height: 90rpx;
		line-height: 90rpx;
		text-align: center;
		color: #FFFFFF;
		background: linear-gradient(270deg, #2CC2FB 0%, #50A6FA 100%);
		border-radius: 8rpx;
		font-size: 30rpx;
		position: fixed;
		bottom: 39rpx;
		left: 30rpx;
	}
</style>