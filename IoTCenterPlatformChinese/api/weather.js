import request from '@/common/request.js'
export function getWeatherInfo(code) {
	return request.get({
		url: '/ThirdPartyService/Weather/Info',
		query: {
			code
		}
	});
}
export function getGeocoder(query) {
	return request.get({
		url: '/AuthService/Code/Geocoder',
		query: query
	});
}
