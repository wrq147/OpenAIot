// #ifdef H5
import serverUrl1 from '@/common/constVar.js'
// #endif
// #ifndef H5
import serverUrl from '@/common/constVar.js'
// #endif
import request from '@/common/request.js'
let ser = ''
// #ifdef H5
ser = serverUrl1.getServerUrl()
// #endif
// #ifndef H5
ser = serverUrl.getServerUrl()
// #endif
//判断设备是否在线
export function deviceIsOnline(id) {
	return new Promise((resolve, reject) => {
		uni.request({
			url: ser + '/IoTRulesService/HttpRule/IsOnline',
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