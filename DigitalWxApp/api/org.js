import request from '@/common/request.js'

//获取组织信息
export function getOrg(id) {
	return request.get({
		url: '/CardService/Org/Info',
		query: {id}
	});
}

//创建新企业
export function createOrg(data) {
	return request.post({
		url: '/CardService/Org/Create',
		data: data
	});
}
//二期跳转判断是否加入加入企业
export function isJoinOrg(query) {
	return request.get({
		url: '/CardService/Org/IsJoin',
		query: query
	});
}
//切换企业
export function switchOrg(id) {
	return request.get({
		url: '/CardService/Org/Switch',
		query: {
			id
		}
	});
}

//用户的关联企业列表
export function userOrgList(man) {
	return request.get({
		url: '/CardService/Org/UserOrgList',
		query: {man}
	});
}

//通过邀请码创建或加入企业
export function joinOrg(data) {
	return request.post({
		url: '/CardService/Org/Join',
		data: data
	});
}
//通过直接加入企业
export function directJoinOrg(data) {
	return request.post({
		url: '/CardService/Org/JoinAt',
		data: data
	});
}

//生成邀请码
export function inviteOrg(data) {
	return request.post({
		url: '/CardService/Org/Invite',
		data: data
	});
}

//编辑指定企业的职位
export function editPost(orgId,postName){
	return request.post({
		url: '/CardService/Org/EditPost',
		data: {orgId,postName}
	});
}

//发送邀请短信
export function sendYqSms(data) {
	return request.get({
		url: '/CardService/Org/SendYqSms',
		query: data
	});
}

//解释邀请码
export function parseYq(code) {
	return request.get({
		url: '/CardService/Org/ParseYq',
		query: {
			id: code
		}
	});
}


//解散企业
export function deleteOrg(id) {
	return request.get({
		url: '/CardService/Org/Remove',
		query: {id}
	});
}

//获取指定用户组织信息
export function userInfo(id){
	return request.get({
		url: '/CardService/Org/UserInfo',
		query: {id}
	});
}