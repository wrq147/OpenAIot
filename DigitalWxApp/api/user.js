import request from '@/common/request.js'

//获取登录用户信息
export function getInfo() {
	return request.get({
		url: '/AuthService/User/LoginInfo'
	});
}

//待处理交换请求数量
export function getPendingCount() {
	return request.get({
		url: '/CardService/Exchange/PendingCount'
	});
}

//获取指定用户列表
export function getUserList(query) {
	return request.get({
		url: '/CardService/User/List',
		query: query
	});
}

//编辑本人信息
export function editUser(data) {
	return request.post({
		url: '/AuthService/Profile/Edit',
		data: data
	});
}

//获取本人信息
export function getSelfInfo() {
	return request.get({
		url: '/AuthService/Profile/Info'
	});
}
//获取指定用户的信息
export function getUsersInfo(query) {
	return request.get({
		url: '/AuthService/User/Info',
		query:query
	});
}
//获取指定用户的指定企业的信息
export function getUserCompanyInfo(id){
	return request.get({
		url:'/AuthService/User/UserInfo',
		query:{id}
	})
}
//用户Id转名片Id
export function getId2CardId(id) {
	return request.get({
		url: '/CardService/User/UId2CardId',
		query: {id}
	});
}
