import request from '@/utils/request'
//获取省市区列表
export function getAreaList() {
    return request({
        url: '/AuthService/Code/AreaList',
        method: 'get'
    });
}
//获取行业列表
export function getIndustryList() {
    return request({
        url: '/AuthService/Code/IndustryList',
        method: 'get'
    });
}

//根据经纬度获取区域代码等信息
export function geocoder(params) {
    return request({
        url: '/AuthService/Code/Geocoder',
        method: 'get',
        params: params
    });
}


//自动识别快递单号的对应快递企业
export function autoCompany(number) {
    return request({
        url: '/AuthService/Code/AutoCompany',
        method: 'get',
        params: { number }
    });
}

//获取所有快递企业
export function kuaiDiCompanys(number) {
    return request({
        url: '/AuthService/Code/KuaiDiCompanys',
        method: 'get'
    });
}
//获取下载地址
export function getAppDown() {
    return request({
        url: '/UpgradeDown',
        method: 'get',
    });
}
// export function industryName(state, code) {
//     try {
//         let indlist = state.industryData;
//         for (let itx = 0; itx < indlist.length; itx++) {
//             if (code.indexOf(indlist[itx].Id.toString()) == 0) {
//                 for (let cix = 0; cix < indlist[itx].children.length; cix++) {
//                     if (indlist[itx].children[cix].Id == code) {
//                         return indlist[itx].children[cix].Name;
//                     }
//                 }
//             }
//         }
//         return "行业错误";
//     } catch (error) {
//         return "行业错误";
//     }
// }