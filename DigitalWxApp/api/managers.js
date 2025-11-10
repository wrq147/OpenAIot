import request from '@/common/request.js'

//获取管理员列表
export function getManagersList(orgId) {
	return request.get({
		url: '/CardService/Man/List',
		query:{orgId}
	})
}

//获取用户列表
export function getUserList() {
	return request.get({
		url: '/AuthService/User/List',
	})
}

//添加管理员
export function setManager(query) {
	return request.get({
		url: '/CardService/Man/Add',
		query:query
	})
}

//删除管理员
export function removeManager(query) {
	return request.get({
		url: '/CardService/Man/Remove',
		query:query
	})
}
//判断是否有管理员权限
export function existManager(orgId){
	return request.get({
		url: '/CardService/Man/Exist',
		query:{"id":orgId}
	})
}