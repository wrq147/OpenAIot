import serverUrl from '@/common/constVar.js'
import request from '@/common/request.js'
//判断设备是否在线
export function deviceIsOnline(id) {
	return new Promise((resolve, reject) => {
		uni.request({
			url: serverUrl.getServerUrl() + '/IoTRulesService/HttpRule/IsOnline',
			data: {
				"id": id
			},
			method: "GET",
			header: {
				'token': 'ba6292efbac440dc9a1f42d381375846' //自定义请求头信息
			},
			success: (res) => {
				resolve(res.data);
			},
			fail: function(err) {
				reject(err)
			}
		});
	})
}
export function devicesIsOnline(id) {
	return request.get({
	    url: '/IoTService/IotDevice/IsOnline',
		query:{id}
	});
}