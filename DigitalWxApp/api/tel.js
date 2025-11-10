import request from '@/common/request.js'

export function getTel(){
	return request.get({
		url:'/WeiXinService/User/MobileBind?code=200'
	})
}

export function getTelImgCode(){
	return request.get({
		url:'/SMSService/Visitor/GetSMSCaptcha',
	})
}
export function getTelCode(query){
	return request.get({
		url:'/SMSService/Visitor/SendCode',
		query:query
	})
}

export function bindTel(data){
	return request.post({
		url:'/SMSService/Sms/Bind',
		data:data
	})
}