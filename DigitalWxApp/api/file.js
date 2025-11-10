import request from '@/common/request.js'
import {
	getToken
} from '../common/auth.js'
//上传文件
export function uploadFile(url) {
	return new Promise((resolve, reject) => {
		uni.uploadFile({
			url: request.config.baseURL + '/AuthService/File/Upload',
			filePath: url,
			header: {
				'Authorization': getToken()
			},
			name: 'file',
			success: (uploadFileRes) => {
				// console.log(uploadFileRes);
				let dataMsg=JSON.parse(uploadFileRes.data)
				if(dataMsg.code==14){
					uni.showToast({
						title: dataMsg.message,
						icon: "none",
						duration: 2000
					});
					return;
				}
				let ret = JSON.parse(uploadFileRes.data);
				if (ret.code == 0) {
					resolve(ret.data);
				} else {
					reject(ret);
				}
			}
		});
	})
}

//修改头像
export function uploadAvatar(url) {
	return new Promise((resolve, reject) => {
		uni.uploadFile({
			url: request.config.baseURL + '/AuthService/Profile/Avatar',
			filePath: url,
			header: {
				'Authorization': getToken()
			},
			name: 'file',
			success: (uploadFileRes) => {
				let ret = JSON.parse(uploadFileRes.data);
				if (ret.code == 0) {
					resolve(ret.data);
				} else {
					reject(ret);
				}
			}
		});
	})
}

//文件删除
export function delFile(id) {
	return request.get({
		url: '/AuthService/File/Delete',
		query: {
			id
		}
	});
}

