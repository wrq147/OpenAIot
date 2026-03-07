// import request from '@/common/request.js'
import request from '@/common/request.js'
// 企业微信绑定登录


export function LoginByWxCorp(data) {
    return request.post({
        url: '/WeiXinService/Login/FromWxCorp',
		header: {
			'Content-Type': 'application/x-www-form-urlencoded'//表单提交
		},
        data: data
    })
}
// 获取指定用户详情
export function wxBaseUrl(query) {
    return request.get({
        url: '/WeiXinService/Login/CreateWxCorpRedirectUrl',
        query: query
    })
}
export function CorpWxConfigJson(appid,url) {
	return request.post({
		url: '/WeiXinService/Login/CorpWxConfigJson',
		data: {
			appid: appid,
			url: url
		}
	})
}