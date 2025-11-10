import request from '@/common/request.js'
export function getSevenPlan(query) {
	return request.get({
		url: '/CRMService/Plan/List',
		query: query
	});
}

