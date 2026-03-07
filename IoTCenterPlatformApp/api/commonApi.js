import request from '@/common/request.js'
//经纬度转地址信息
export function reverseGeocoder(lng, lat) {
	return new Promise((resolve, reject) => {
		uni.request({
			url: 'https://apis.map.qq.com/ws/geocoder/v1/?key=CAUBZ-PUFCO-7V4WO-SD2KN-4KYL2-CMB6H&output=json&location=' +
				lat + ',' + lng,
			method: 'GET',
			success(res) {
				// console.log('地址', res);
				if (res.data.status == 0) {
					resolve(res.data.result);
				} else {
					reject(res.data);
				}
			}
		});
	})
}
export function AreaGeocoder(query) {
    return request.get({
        url: '/AuthService/Code/Geocoder?v=2',
		query:query
    });
}
