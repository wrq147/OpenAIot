import request from '@/common/request.js'

//获取用户关联的企业信息
export function getCompanyMessage(id){
	return request.get({
		url:'/CardService/Org/OrgView',
		query:{id}
	})
}

//解散企业
export function dismissEnterprise(id){
	return request.get({
		url:'/CardService/Org/remove',
		query:{id}
	})
}
//退出企业
export function exitCompany(id){
	return request.get({
		url:'/CardService/Org/Exit',
		query:{id}
	})
}

//移除成员
export function delCompanyMember(id){
	return request.get({
		url:'/CardService/Org/RemoveUser',
		query:{id}
	})
}

//修改企业成员职位信息
export function editMemberInfo(data){
	return request.post({
		url:'/CardService/Org/EditUser',
		data:data
	})
}
//修改信息后保存
export function orgEditSave(data){
	return request.post({
		url:'/CardService/Org/Edit',
		data,
	})
}

// //获取企业信息
// export function getCompanyMes(query){
// 	return request.get({
// 		url:'/CardService/Org/OrgView',
// 		query:query
// 	})
// }