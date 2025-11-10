import request from '@/common/request.js'
//获取案例列表
export function getExampleList(query) {
	return request.get({
		url: '/CardService/Case/ListM',
		query:query
	})
}
//获取案例信息
export function getExampleInfo(query) {
	return request.get({
		url: '/CardService/Case/Info',
		query:query
	})
}
//添加案例
export function addExample(data) {
	return request.post({
		url: '/CardService/Case/Add',
		data:data
	})
}
//编辑案例
export function editExample(data) {
	return request.post({
		url: '/CardService/Case/Edit',
		data:data
	})
}
//删除案例
export function delExample(query) {
	return request.get({
		url: '/CardService/Case/Remove',
		query:query
	})
}
//案例设置信息设置
export function addSetting(data){
	return request.post({
		url:'/CardService/Org/Config',
		data:data,
	})
}