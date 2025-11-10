import request from '@/common/request.js'

//获取我的浏览记录
export function getRecordList(data){
	return request.get({
		url: '/CardService/Msg/Record',
		query: data
	});
}

//获取受访记录列表
export function getVisitedList(data){
	return request.get({
		url: '/CardService/Msg/Visited',
		query: data
	});
}

//访问开始时调用
export function visit(data){
	return request.post({
		url: '/CardService/Msg/Visit',
		data: data
	});
}

//访问结束时调用
export function visitEnd(id){
	return request.get({
		url: '/CardService/Msg/VisitEnd',
		query: {id}
	});
}

//设置已读
export function readed(id){
	return request.get({
		url: '/CardService/Msg/Readed',
		query: {id}
	});
}
//查询未读数量
export function getNoRead(){
	return request.get({
		url: '/CardService/Msg/UnreadAmount',
	});
}