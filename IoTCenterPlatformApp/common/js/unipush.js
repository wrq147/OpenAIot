// 监听push消息 以及 后台数据回复
import phoneInfo from '@/common/js/phone-info.js';
let timer = null;
let numloop = 0;
import {
	upClientId
} from "@/api/appMessage.js"

// 消息推送 应用配置(这些给后端用的)
const uniPushObj = {
	cid: "",
	AppID: "",
	AppKey: "",
	AppSecret: "",
	MasterSecret: "",
}

export default {
	getInfo() {
		uni.getSystemInfo({
			success: res => {
				phoneInfo.systemInfo = res;
			}
		});
	},
	// 开启监听推送 
	pushListener() {
		const platform = phoneInfo.systemInfo.platform.toLowerCase();
		// 点击推送信息
		plus.push.addEventListener('click', res => {
			// const token = uni.getStorageSync("userid");

			// 其实在这里就可以根据你自己的业务去写了
			plus.push.clear(); //清空通知栏
			// if (token) {
			//     messageClick(res);
			// } else {
			//     // 这里跳登录页了
			//     uni.navigateTo({
			//         url: `/pages/login/index`
			//     })
			// }
		});
		// 接收推送信息  在线
		plus.push.addEventListener('receive', res => {
			// console.log("(receive):" ,res);

			const messageTitle = res.title;
			const messageContent = res.content;
			let payload=res.payload
			if(payload){
				if (payload.click_type == "Approval") {
					let srt = payload.click_url.substring(payload.click_url.lastIndexOf('=') + 1)
					uni.navigateTo({
						url: '/pages_flow/process_detail?id=' + srt + '&isMessage=true'
					})
				}
				if (payload.click_type == "设备告警") {
					let srt = item.click_url.substring(item.click_url.lastIndexOf('=') + 1)
					uni.navigateTo({
						url: '/pages_device/alarm/list'
					})
				}
			}
			
			if (platform == 'android') {
				/***  
				  安卓监听不到  因为安卓这个格式被封装了，做成了通知栏展示
				  换个格式就行(比如里面多个字段，或换个字段名)
				*/
				/***
				  此格式的透传消息由 unipush 做了特殊处理， 会自动展示通知栏 
				  开发者也可自定义其它格式， 在客户端自己处理
				*/
				//   "push_message": {
				//     "transmission": "{
				//       title:\"标题\",
				//       content:\"内容\",
				//       payload:\"自定义数据\"
				//     }"
				//   },
				// Hbulidx 版本大于 ## 3.4.18，安卓不再通知栏展示, 需要自行创建通知
				// plus.push.createMessage(messageContent, res.payload, {
				// 	title: messageTitle
				// });
				// 或者在 onlaunch 写入
				// plus.push.setAutoNotification(true);
			} else {
				const type = res.type
				//【APP离线】收到消息，但没有提醒(发生在一次收到多个离线消息时，只有一个有提醒，但其他的没有提醒)  
				//【APP在线】收到消息，不会触发系统消息,需要创建本地消息，但不能重复创建
				// 必须加msg.type验证去除死循环        
				if (res.aps == null && type == "receive") {
					//创建本地消息,发送的本地消息也会被receive方法接收到，但没有type属性，且aps是null  
					// plus.push.createMessage(messageContent, res.payload, {
					// 	title: messageTitle
					// });
				}
			}
		});

		function messageClick(msg) {
			if (typeof(msg.payload) == 'string') { //如果是字符串，表示是ios创建的  要转换一下
				msg.payload = JSON.parse(msg.payload)
			}
			if (!msg) return false;
			try {
				var page = msg.payload.page;
				switch (msg.payload.type) {
					case 'switchTab':
						uni.switchTab({
							url: page
						})
						break;
					case 'navigateTo':
						uni.navigateTo({
							url: page
						})
						break;
					case 'redirectTo':
						uni.redirectTo({
							url: page
						})
						break;
				}
			} catch (e) {
				console.log(e)
			}
		}
	},
	// 循环获取clientid信息,直到获取到为止
	getClientInfoLoop() {
		if (plus.push.getClientInfo() && plus.push.getClientInfo().clientid) {
			uni.setStorageSync('cid', plus.push.getClientInfo().clientid);
			uniPushObj.cid = plus.push.getClientInfo().clientid
			try {
				passCid(plus.push.getClientInfo().clientid)
			} catch (e) {
				//TODO handle the exception
				console.log("eeeeeeee", e);
			}
		} else {
			plus.push.getClientInfoAsync(info => {
				// 如果info不存在，或者info存在，cid不存在则再次获取cid
				if (!info || !info.clientid) {
					let infoTimer = null;
					infoTimer = setInterval(function() {
						if (cid) {
							clearInterval(infoTimer); //清定时器
							// uni.showModal({
							// 	content: cid
							// })
							uni.setStorageSync('cid', cid);
							uniPushObj.cid = cid
							try {
								passCid(cid)
							} catch (e) {
								//TODO handle the exception
								console.log("eeeeeeee", e);
							}
						}
					}, 50);
				} else if (info && info.clientid) {
					let cid = info.clientid;
					uni.setStorageSync('cid', cid);
					uniPushObj.cid = cid
					try {
						passCid(cid)
					} catch (e) {
						//TODO handle the exception
						console.log("eeeeeeee", e);
					}
				}
			}, (e) => {
				let pinf = plus.push.getClientInfo();
				let cid = pinf.clientid; //客户端标识 
				if (cid) {
					uni.setStorageSync('cid', cid);
					uniPushObj.cid = cid
				}
			})
		}



		async function passCid(cid) {
			// var params = {
			//     action: 'app_bind_cid',
			//     appid: uniPushObj.AppID,
			//     cid: uniPushObj.cid,
			//     userid: uni.getStorageSync('userid')
			// };
			try{
				let response=await upClientId(cid)
				// console.log('----------> cid 绑定别名成功', response);
			}catch(err){
				//TODO handle the exception
				console.log("上报错误", err);
				// this.setMsgTop(err)
			}
		}
	},
	getQuanxian() {
		let platform = uni.getSystemInfoSync().platform; //首先判断app是安卓还是ios
		console.log(platform);
		if (platform == "ios") { //这里是ios的方法
			var UIApplication = plus.ios.import("UIApplication");
			var app = UIApplication.sharedApplication();
			var enabledTypes = 0;
			if (app.currentUserNotificationSettings) {
				var settings = app.currentUserNotificationSettings();
				enabledTypes = settings.plusGetAttribute("types");
				// console.log("enabledTypes1:" + enabledTypes);
				if (enabledTypes == 0) { //如果enabledTypes = 0 就是通知权限没有开启
					uni.showModal({
						title: '提示',
						content: '是否前往打开消息通知权限？',
						success: res => {
							if (res.confirm) {
								openTongZhi()
							} else if (res.cancel) {
								console.log('用户点击取消');
							}
						}
					});
				} else {
					// uni.showToast({
					// 	title: '已开启',
					// 	icon: "none"
					// })
				}
			}
			plus.ios.deleteObject(settings);
		} else if (platform == "android") { //下面是安卓的方法
			// console.log("我是安卓", plus.android);
			var main = plus.android.runtimeMainActivity();
			var pkName = main.getPackageName();
			var uid = main.getApplicationInfo().plusGetAttribute("uid");
			var NotificationManagerCompat = plus.android.importClass(
				"android.support.v4.app.NotificationManagerCompat"
			);
			//android.support.v4升级为androidx
			if (NotificationManagerCompat == null) {
				NotificationManagerCompat = plus.android.importClass(
					"androidx.core.app.NotificationManagerCompat"
				);
			}
			var areNotificationsEnabled =
				NotificationManagerCompat.from(main).areNotificationsEnabled();
			console.log(areNotificationsEnabled);
			// 未开通‘允许通知’权限，则弹窗提醒开通，并点击确认后，跳转到系统设置页面进行设置
			if (!areNotificationsEnabled) {
				this.tongzhi = true; //这里也一样未开启权限，弹出弹窗
			}
			if (areNotificationsEnabled) {
				// uni.showToast({
				// 	title: '已开启',
				// 	icon: "none"
				// })
			} else {
				uni.showModal({
					title: '提示',
					content: '是否前往打开消息通知权限？',
					success: res => {
						if (res.confirm) {
							openTongZhi()
						} else if (res.cancel) {
							console.log('用户点击取消');
						}
					}
				});
			}
		}
	
		function openTongZhi() { //弹窗按钮绑定方法
			let platform = uni.getSystemInfoSync().platform; //获取安卓还是ios
			if (platform == "ios") { //如果机型是ios，ios由于权限问题，可能需要手动开启
				var UIApplication = plus.ios.import("UIApplication");
				var app = UIApplication.sharedApplication();
				var settings = app.currentUserNotificationSettings();
				enabledTypes = settings.plusGetAttribute("types");
				var NSURL2 = plus.ios.import("NSURL");
				var setting2 = NSURL2.URLWithString("app-settings:");
				var application2 = UIApplication.sharedApplication();
				application2.openURL(setting2);
				plus.ios.deleteObject(setting2);
				plus.ios.deleteObject(NSURL2);
				plus.ios.deleteObject(application2);
				plus.ios.deleteObject(settings);
			} else if (platform == "android") { //如果机型是安卓
				var main = plus.android.runtimeMainActivity();
				var pkName = main.getPackageName();
				var uid = main.getApplicationInfo().plusGetAttribute("uid");
				var Intent = plus.android.importClass("android.content.Intent");
				var Build = plus.android.importClass("android.os.Build");
				//android 8.0引导
				if (Build.VERSION.SDK_INT >= 26) { //判断安卓系统版本
					var intent = new Intent("android.settings.APP_NOTIFICATION_SETTINGS");
					intent.putExtra("android.provider.extra.APP_PACKAGE", pkName);
				} else if (Build.VERSION.SDK_INT >= 21) { //判断安卓系统版本
					//android 5.0-7.0
					var intent = new Intent("android.settings.APP_NOTIFICATION_SETTINGS");
					intent.putExtra("app_package", pkName);
					intent.putExtra("app_uid", uid);
				} else {
					//(<21)其他--跳转到该应用管理的详情页
					intent.setAction(Settings.ACTION_APPLICATION_DETAILS_SETTINGS);
					var uri = Uri.fromParts(
						"package",
						mainActivity.getPackageName(),
						null
					);
					intent.setData(uri);
				}
				// 跳转到该应用的系统通知设置页
				main.startActivity(intent);
			}
		}
	},
	/** 
	 * 向后台传送cid，绑定别名
	 */

}