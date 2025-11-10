import request from '@/common/request.js'

//获取产品列表
export function getProductList(query) {
	return request.get({
		url: '/CardService/Pro/List',
		query:query
	})
}
//获取产品信息
export function getProductMessage(query) {
	return request.get({
		url: '/CardService/Pro/Info',
		query:query
	})
}
//添加产品
export function addProduct(data) {
	return request.post({
		url: '/CardService/Pro/Add',
		data:data
	})
}

//删除产品
export function deletePro(id) {
	return request.get({
		url: '/CardService/Pro/Remove',
		query:{id}
	})
}
//恢复产品
export function recoverPro(query) {
	return request.get({
		url: '/CardService/Pro/Recover',
		query:query
	})
}
//产品编辑
export function editProduct(data) {
	return request.post({
		url: '/CardService/Pro/Edit',
		data:data
	})
}
//获取产品分类列表
export function getProClassList(query){
	return request.get({
		url:'/CardService/Pro/CategoryList',
		query:query,
	})
}

//添加产品分类
export function addProClass(data){
	return request.post({
		url:'/CardService/Pro/AddCategory',
		data:data,
	})
}
//获取树状产品分类列表
export function getClassTree(query){
	return request.get({
		url:'/CardService/Pro/CategoryTreeSelect',
		query:query,
	})
}
//修改产品分类
export function editProClass(data){
	return request.post({
		url:'/CardService/Pro/EditCategory',
		data:data,
	})
}
//产品分类排序
export function sortProClass(data){
	return request.post({
		url:'/CardService/Pro/SortCategory',
		data:data,
	})
}
//删除产品分类
export function deleteProClass(query){
	return request.get({
		url:'/CardService/Pro/RemoveCategory',
		query:query,
	})
}

//产品设置信息设置
export function addSetting(data){
	return request.post({
		url:'/CardService/Org/Config',
		data:data,
	})
}

//产品设置信息添加
