import request from '@/common/request.js'

export function getDeptList(query) {
	return request.get({
		url: '/CardService/Dept/List',
		query: query
	});
}

//获取部门树
export function getDeptTree(data) {
	return request.get({
		url: '/CardService/Dept/TreeSelect',
		query: data
	});
}
//获取部门信息
export function getDeptInfo(id) {
	return request.get({
		url: '/CardService/Dept/Info',
		query: {
			id
		}
	});
}


//排序
export function sortDept(data) {
	return request.post({
		url: '/CardService/Dept/Sort',
		data: data
	});
}

//编辑部门
export function editDept(data) {
	return request.post({
		url: '/CardService/Dept/Edit',
		data: data
	});
}
//添加部门
export function addDept(data) {
	return request.post({
		url: '/CardService/Dept/Add',
		data: data
	});
}

//删除部门
export function delDept(id) {
	return request.get({
		url: '/CardService/Dept/Remove',
		query: {
			id
		}
	});
}

//移动部门
export function moveDept(ids, uids, parentId) {
	return request.post({
		url: '/CardService/Dept/Move',
		data: {
			ids,
			uids,
			parentId
		}
	});
}
