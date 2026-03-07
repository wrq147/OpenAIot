import request from '@/common/request.js'
// #ifdef H5
import {serverUrl} from '@/common/constVar.js'
// #endif
// #ifndef H5
import serverUrl from '@/common/constVar.js'
// #endif
import {
	getToken
} from '@/common/auth.js'
import {
    praseStrEmpty
} from "@/common/utillib.js";
let ser = ''
// #ifdef H5
ser = serverUrl=='/'?'':serverUrl
// #endif
// #ifndef H5
ser = serverUrl.getServerUrl()
// #endif
//查询当前用户加入的组织列表
export function getJoinOrgList() {
    return request.get({
        url: '/AuthService/Profile/OrgList',
    })
}

//切换企业
export function switchOrg(query) {
    return request.get({
        url: '/AuthService/Profile/Switch',
        query: query
    })
}

// 加入邀请
export function joinInvite(data) {
    return request.post({
        url: '/ProducerService/Agent/JoinInvite',
        data: data
    })
}
//上传文件
// export function uploadPhoto(data) {
//     return request.post({
//         url: '/AuthService/File/Upload?withDomain=true',
//         data: data
//     });
// }
export function uploadPhoto(url) {
	return new Promise((resolve, reject) => {
		uni.uploadFile({
			header:{
				'Authorization':getToken()
			},
			url: ser + '/AuthService/File/Upload?withDomain=true', 
			filePath: url,
			name: 'file',
			success: (uploadFileRes) => {
				let ret = JSON.parse(uploadFileRes.data);
				if (ret.code==0) {
					resolve(ret.data);
				} else {
					reject(ret);
				}
			},
			fail:(err)=>{
				console.log("图片上传失败",err);
			}
		});
	})
}


export function uploadPhotoImg(url) {
	return new Promise((resolve, reject) => {
		uni.uploadFile({
			header:{
				'Authorization':getToken()
			},
			url: ser + '/AuthService/File/Upload?withDomain=true', 
			filePath: url,
			name: 'wangeditor-uploaded-image',
			success: (uploadFileRes) => {
				let ret = JSON.parse(uploadFileRes.data);
				if (ret.code==0) {
					resolve(ret.data);
				} else {
					reject(ret);
				}
			},
			fail:(err)=>{
				console.log("图片上传失败",err);
			}
		});
	})
}
//删除文件
export function delPhoto(url) {
    return request.get({
        url: '/AuthService/File/Delete?id='+url,
        // query: query
    });
}
// 创建新企业
export function creatCompany(data) {
    return request.post({
        url: '/AuthService/Org/Create',
		data:data
    })
}
// 查询组织架构树
export function getOrgTree(query) {
  return request.get({
    url: '/FlowService/Org/Tree',
    query: query
  })
}

// 搜索人员
export function getUserByName(query) {
  return request.get({
    url: '/FlowService/Org/Search',
    query: query
  })
}
// 查询用户详细
export function getUser(userId) {
    return request.get({
        url: '/AuthService/User/Info/' + praseStrEmpty(userId),
    })
}
