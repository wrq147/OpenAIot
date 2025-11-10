
/**
 * 版本升级提示
 */
export function verUp(){
	//#ifdef MP-WEIXIN
	//小程序版本检测
	// 获取小程序更新机制兼容
	if (uni.canIUse('getUpdateManager')) {
		const updateManager = uni.getUpdateManager(); //本API返回全局唯一的版本更新管理器对象： updateManager，用于管理小程序更新。
		updateManager.onCheckForUpdate((res) => { //当向小程序后台请求完新版本信息，进行回调方法
			if (res.hasUpdate) {
				updateManager.onUpdateReady((res) => { //当新版本下载完成，会进行回调
					uni.showModal({
						title: '更新提示',
						content: '新版本已经准备好，是否重启应用？',
						success(res) {
							if (res.confirm) {
								// 新的版本已经下载好，调用 applyUpdate 应用新版本并重启
								updateManager.applyUpdate();
							}
						}
					});
				});
				updateManager.onUpdateFailed((res) => {
					// 新的版本下载失败
					uni.showModal({
						title: '更新提示',
						content: '检查到有新版本，但下载失败，请检查网络设置',
						success(res) {
							if (res.confirm) {
								// 新的版本已经下载好，调用 applyUpdate 应用新版本并重启
								updateManager.applyUpdate();
							}
						}
					});
				});
			}
		})
	}
	//#endif
}