import request from '@/common/request.js'

export function checkVersion(appVersion,wgtVersion) {
	return request.get({
		url: '/AuthService/Upgrade/CheckVersion',
		query: {"appVersion":appVersion,"wgtVersion":wgtVersion}
	});
}
