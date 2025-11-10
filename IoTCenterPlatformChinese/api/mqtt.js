import request from '@/common/request.js'
// #ifdef H5
import {serverUrl} from '@/common/constVar.js'
// #endif
// #ifndef H5
import serverUrl from '@/common/constVar.js'
// #endif
import {
	getToken
} from '@/common/auth.js'
let ser = ''
// #ifdef H5
ser = serverUrl
// #endif
// #ifndef H5
ser = serverUrl.getServerUrl()
// #endif
// 获取Mqtt的web连接地址
export function getWebIp(query) {
	return request.get({
		url: '/MqttService/Server/WebIp',
		query: query
	})
}
export function getClientId1() {
	var requestTask = uni.request({
		url: ser + '/MqttService/User/ClientId',
		method: "GET",
		header: {
			'Authorization': getToken()
		},
		success: (res) => {
			// console.log("resClientId",res);
			// console.log(res,'成功返回');
		},
		fail: function(err) {
			// console.log(err,'错误返回');
		}
	});
	let req = requestTask._xhr
	if (req.status === 200) {
		return JSON.parse(req.responseText).data;
	} else {
		return null;
	}
}

// 获取用户的mqtt连接clientId
export function* getClientId2() {

	// return request.get({
	// 	url: '/MqttService/User/ClientId'
	// })

	let mq = new Promise((resolve, reject) => {
		uni.request({
			url: ser + '/MqttService/User/ClientId',
			method: "GET",
			header: {
				'Authorization': getToken(),
				"Access-Control-Allow-Origin": "*"
			},
			success: (res) => {
				resolve(res.data.data);
			},
			fail: function(err) {
				reject(err)
			}
		});
	})
	mq.then((d) => {
		return d;
	})

	let result = yield mq;
}

export function getClientId() {
	return request.get({
		url: '/MqttService/User/ClientId'
	})
}
// 返回服务端格林威治时间和本地时间之间的时差
export function getTimezoneOffset(ssl) {
	return request.get({
		url: '/MqttService/Server/GetTimezoneOffset',
	})
}
export function asyncClientIdOperation(oldOptions) {
	return new Promise(function(resolve, reject) {
		// 异步操作代码，通过网络获取新的clientId，这里使用setTimeout模拟异步操作
		setTimeout(async function() {
			const newClientId = await getClientId();
			// 使用新的clientId来修改options中的clientId
			const newOptions = Object.assign({}, oldOptions, {
				clientId: newClientId.data
			});
			resolve(newOptions);
			// 或者使用 reject(error) 返回错误信息
		}, 1000); // 假设异步操作耗时1秒
	});
}