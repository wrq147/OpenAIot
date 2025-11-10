import request from '@/common/request.js'

//获取名片信息
export function getCardInfo(id) {
	return request.get({
		url: '/CardService/Card/Info',
		query: {
			id
		}
	});
}


//获取我的名片列表
export function getCardList(data){
	return request.get({
		url: '/CardService/Card/List',
		query: data
	});
}

//切换名片
export function switchCard(id){
	return request.get({
		url: '/CardService/Card/Switch',
		query: {id}
	});
}

//获取指定名片
export function getCard(id){
	return request.get({
		url: '/CardService/Card/Info',
		query: {id}
	});
}

//更新名片
export function editCard(data) {
	return request.post({
		url: '/CardService/Card/Edit',
		data: data
	});
}

//添加名片
export function addCard(data) {
	return request.post({
		url: '/CardService/Card/Add',
		data: data
	});
}

//删除名片
export function deleteCard(id) {
	return request.get({
		url: '/CardService/Card/Remove',
		query: {id}
	});
}

//获取指定名片的交换信息
export function getExchangeInfo(tuid, tcid, ucid){
	return request.get({
		url: '/CardService/Exchange/Info',
		query: {tuid,tcid,ucid}
	});
}

//存入通讯录
export function addHolder(id) {
	return request.post({
		url: '/CardService/Holder/Add',
		data: {id}
	});
}

//递名片
export function addExchange(id) {
	return request.post({
		url: '/CardService/Exchange/Add',
		data: {id}
	});
}

//获取通讯录列表
export function getHolderList(data){
	return request.get({
		url: '/CardService/Holder/List',
		query: data
	});
}

//获取名片交换请求列表
export function getExchangeList(data){
	return request.get({
		url: '/CardService/Exchange/List',
		query: data
	});
}

//同意交换
export function agreeExchange(id){
	return request.get({
		url: '/CardService/Exchange/Agree',
		query: {id}
	});
}

//忽略交换
export function refuseExchange(id){
	return request.get({
		url: '/CardService/Exchange/Refuse',
		query: {id}
	});
}