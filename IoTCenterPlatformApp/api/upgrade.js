import request from '@/common/request.js'

export function checkVersion(appVersion,wgtVersion,orgId,styleId) {
	// 'orgId':orgId,
	return request.get({
		url: '/AuthService/Upgrade/CheckVersion',
		query: {"appVersion":appVersion,"wgtVersion":wgtVersion,'styleId':styleId}
	});
}
