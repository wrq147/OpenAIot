import {
	checkVersion
} from '@/api/upgrade.js'
import store from '../store'
let that = this
// 推荐再App.vue中使用
const PACKAGE_INFO_KEY = '__package_info__'

export default function() {
	// #ifdef APP-PLUS
	return checkFunc();
	// #endif
}

function checkFunc() {
	return new Promise((resolve, reject) => {
		plus.runtime.getProperty(plus.runtime.appid, function(widgetInfo) {
			store.commit('SET_VERSION_NUMBER', widgetInfo.version)
			let loginOrg = store.state.loginOrg
			let loginCheckTheme = store.state.checkUpdatesThemeId
			// console.log("loginOrg",loginOrg,widgetInfo.version);
			checkVersion(plus.runtime.version, widgetInfo.version,loginOrg,loginCheckTheme).then(async (e) => {
				// console.log("e.data",e.data);
				if (e.data == null) {
					return resolve(e)
				}

				// 静默更新，只有wgt有
				if (e.data.IsSilently == true) {
					uni.downloadFile({
						url: e.data.UpUrl,
						success: res => {
							if (res.statusCode == 200) {
								// 下载好直接安装，下次启动生效
								plus.runtime.install(res.tempFilePath, {
									force: false
								}, () => {
									uni.showModal({
										title: '更新成功后是否重新启动？',
										success: res => {
											if (res.confirm) {
												//更新完重启app
												plus.runtime.restart();
											}
										}
									})
								}, err => {
									uni.showToast({
										title: '应用程序安装失败',
										icon: "none",
										duration: 2000
									});
									setTimeout(checkFunc, 2000);
								});
							}
						}
					});
					return resolve(e);
				}

				/**
				 * 提示升级
				 * 使用 uni.showModal
				 */

				updateUseModal(e.data);
				return resolve(e);
			}).catch(err => {
				console.info(err)
				// TODO 云函数报错处理
				uni.switchTab({
					url: store.state.homejumpurl?store.state.homejumpurl:'/pages/index/index',
					// #ifdef APP-PLUS
					success: () => {
						plus.navigator.closeSplashscreen();
					},
					// #endif
				})
				reject(err)
			})
		})
	});
}
/**
 * 使用 uni.showModal 升级
 */
function updateUseModal(packageInfo) {
	const {
		Title, // 标题
		UpContent, // 升级内容
		IsMandatory, // 是否强制更新
		UpUrl, // 安装包下载地址
		Platform, // 安装包平台
		PackageType // 安装包类型
	} = packageInfo;

	let isWGT = PackageType === 1
	let confirmText = !isWGT ? '确认更新' : '下载'

	return uni.showModal({
		title: Title,
		content: UpContent,
		showCancel: IsMandatory == false,
		confirmText: confirmText,
		success: res => {
			if (res.cancel) return;
			// 安装包下载
			if (!isWGT) {
				plus.runtime.openURL(UpUrl);
				if (IsMandatory == true) {
					checkFunc();
					switch (uni.getSystemInfoSync().platform) {
						case 'android':
							plus.runtime.quit();
							break;
						case 'ios':
							plus.ios.import('UIApplication').sharedApplication().performSelector('exit');
							break;
					}
				}
				return;
			}

			uni.showLoading({
				title: '下载中……'
			});

			// wgt下载更新
			uni.downloadFile({
				url: UpUrl,
				success: res => {
					uni.hideLoading();

					// 下载好直接安装，下次启动生效
					plus.runtime.install(res.tempFilePath, {
						force: false
					}, () => {
						if (IsMandatory == true) {
							//更新完重启app
							plus.runtime.restart();
							return;
						}
						uni.showModal({
							title: '安装成功后是否重新启动？',
							success: res => {
								if (res.confirm) {
									//更新完重启app
									plus.runtime.restart();
								}
							}
						});
					}, err => {
						uni.showToast({
							title: '应用程序安装失败',
							icon: "none",
							duration: 2000
						});
						setTimeout(checkFunc, 2000);
					});
				},
				fail: (err) => {
					uni.hideLoading();
					uni.showToast({
						title: '网络访问失败',
						icon: "none",
						duration: 2000
					});
					setTimeout(checkFunc, 2000);
				}
			});
		}
	});
}