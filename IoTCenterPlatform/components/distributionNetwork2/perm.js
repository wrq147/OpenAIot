import permision from "@/js_sdk/wa-permission/permission.js"

async function isLocationPermissionGranted() {
	try {
		const {
			authSetting
		} = await promisify(uni.getSetting)();
		console.log(authSetting['scope.userLocation']);
		return Boolean(authSetting['scope.userLocation']);
	} catch (e) {
		//TODO handle the exception
		return false
	}
}

export async function requestLocationPermission() {
	// 用户拒绝权限申请会抛异常, 引导用户进入设置页面授权
	//#ifdef MP-WEIXIN

	if (await isLocationPermissionGranted()) {
		return true;
	} else {
		try {
			await promisify(uni.authorize)({
				scope: 'scope.userLocation',
			});
			return true;
		} catch (error) {
			uni.showModal({
				title: '',
				content: 'Obtaining a WiFi list requires your authorization to use location information',
				confirmText: 'Deauthorization',
				success: async ({
					confirm
				}) => {
					if (confirm) {
						uni.openSetting();
					}
				},
			});
			return false;
		}

	}


	//#endif
	//#ifndef MP-WEIXIN
	if(uni.getSystemInfoSync().platform == 'ios'){
		return false;
	}
	else{
		if (await permision.requestAndroidPermission("android.permission.ACCESS_WIFI_STATE")==1
		&&await permision.requestAndroidPermission("android.permission.CHANGE_WIFI_STATE")==1
		&&await permision.requestAndroidPermission("android.permission.ACCESS_FINE_LOCATION")==1
		&&await permision.requestAndroidPermission("android.permission.ACCESS_COARSE_LOCATION")==1) {
			return true;
		} else {
			uni.showModal({
				title: '',
				content: 'Obtaining the WiFi list requires your authorization to use location information. Please authorize manually',
				success: async ({
					confirm
				}) => {
					if (confirm) {
						permision.gotoAppPermissionSetting();
					}
				},
			});
			return false;
		}

	}

	//#endif

}