import request from '@/common/request.js'
//获取省市区列表
export function getAreaList() {
	return request.get({
		url: '/AuthService/Code/AreaList'
	});
}
//获取行业列表
export function getIndustryList() {
	return request.get({
		url: '/AuthService/Code/IndustryList'
	});
}
//根据字典类型查询字典数据信息
export function getDictList(dictType) {
	return request.get({
		url: '/DictService/DictInfo/List',
		query: {
			id: dictType
		}
	});
}


export function industryName(state, code) {
	try {
		let indlist = state.industryData;
		for (let itx = 0; itx < indlist.length; itx++) {
			if (code.indexOf(indlist[itx].Id.toString()) == 0) {
				for (let cix = 0; cix < indlist[itx].children.length; cix++) {
					if (indlist[itx].children[cix].Id == code) {
						return indlist[itx].children[cix].Name;
					}
				}
			}
		}
		return "行业错误";
	} catch (error) {
		return "行业错误";
	}
}
