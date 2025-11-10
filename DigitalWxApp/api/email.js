import request from '@/common/request.js'

export function getEmailCode(query){
	return request.get({
		url:'/EmailService/Email/SendCode',
		query:query
	})
}

export function bindMailbox(query){
	return request.get({
		url:'/EmailService/Email/BindEmail',
		query:query
	})
}