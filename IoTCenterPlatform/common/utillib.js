// 表单重置
export function resetForm(refName) {
	if (this.$refs[refName]) {
		this.$refs[refName].resetFields();
	}
}
//调用缓存页面的相关函数
export function setPagesParam(parent_fun_name, params, prev = 1, isJump = true) {
	var pages = getCurrentPages();
	var prevPage = pages[pages.length - (1 + prev)];
	// console.log(pages, prevPage);
	if (!prevPage || prevPage == undefined || prevPage == null) { //如果只有当前页面的时候关闭页面自动回到首页
		if (isJump) {
			uni.switchTab({
				url: '/pages/devices/devices'
			});
		}
		return
	}else{
		parent_fun_name && prevPage.$vm[parent_fun_name] && prevPage.$vm[parent_fun_name](params)
		if (isJump) {
			// #ifndef H5
			uni.navigateBack({
				delta: prev
			})
			//#endif
			// #ifdef H5
			let retNum = -Number(prev)
			history.go(retNum)
			//#endif
		}
	}
	
}
/**
 * 数组转树
 * @param {*} list 
 * @param {*} parentKey 父键
 * @param {*} childrenKey 子数组键
 */
export function totree(list, idKey, parentKey, childrenKey, cb) {
    let parId;
    let obj = {};
    let result = [];
    //将数组中数据转为键值对结构 (这里的数组和obj会相互引用)
    list.map(el => {
            obj[el[idKey]] = el;
        })
        //自动查顶级id
    list.forEach(function(item) {
        if (!obj[item[parentKey]]) {
            parId = item[parentKey];
            return;
        }
    });
    for (let i = 0, len = list.length; i < len; i++) {
        let id = list[i][parentKey];
        if (id == parId) {
            result.push(list[i]);
            continue;
        }

        if (obj[id][childrenKey]) {
            obj[id][childrenKey].push(list[i]);
        } else {
            obj[id][childrenKey] = [list[i]];
            if (cb) {
                cb(obj[id]);
            }
        }
    }
    return result;
}

// 日期格式化
export function parseTime(time, pattern) {
    if (arguments.length === 0 || !time) {
        return null
    }
    const format = pattern || '{y}-{m}-{d} {h}:{i}:{s}'
    let date
    if (typeof time === 'object') {
        date = time
    } else {
        if ((typeof time === 'string') && (/^[0-9]+$/.test(time))) {
            time = parseInt(time)
        } else if (typeof time === 'string') {
            time = time.replace(new RegExp(/-/gm), '/').replace('T', ' ').replace(new RegExp(/\.[\d]{3}/gm), '');
        }
        if ((typeof time === 'number') && (time.toString().length === 10)) {
            time = time * 1000
        }
        date = new Date(time)
    }
    const formatObj = {
        y: date.getFullYear(),
        m: date.getMonth() + 1,
        d: date.getDate(),
        h: date.getHours(),
        i: date.getMinutes(),
        s: date.getSeconds(),
        a: date.getDay()
    }
    const time_str = format.replace(/{(y|m|d|h|i|s|a)+}/g, (result, key) => {
        let value = formatObj[key]
        // Note: getDay() returns 0 on Sunday
        if (key === 'a') { return ['日', '一', '二', '三', '四', '五', '六'][value] }
        if (result.length > 0 && value < 10) {
            value = '0' + value
        }
        return value || 0
    })
    time_str.replace(' ', '+')
    return time_str
}
// 添加日期范围
export function addDateRange(params, dateRange, propName) {
    let search = params;
    dateRange = Array.isArray(dateRange) ? dateRange : [];
    let seccdate = dateRange[1];
    if (dateRange.length > 1) {
        if(typeof dateRange[1] === 'string'){
            if(dateRange[1].indexOf(":") < 0){
                seccdate = dateRange[1] + " 23:59:59";
            }
        }
    }

    if (typeof (propName) === 'undefined') {
        search['beginTime'] = dateRange[0];
        search['endTime'] = seccdate;
    } else {
        search['begin' + propName] = dateRange[0];
        search['end' + propName] = seccdate;
    }
    return search;
}
export function isNotEmpty(obj) {
    return (obj !== undefined && obj !== null && obj !== '' && obj !== 'null')
}
// 转换字符串，undefined,null等转化为""
export function praseStrEmpty(str) {
    if (!str || str == "undefined" || str == "null") {
        return "";
    }
    return str;
}

/**
 * 服务端时间转当前时间
 * @param {*} time 
 * @returns 
 */
import { getTimezoneOffset } from "@/api/mqtt.js";
const tzArray = new Array();
export async function Time2Local(timestr) {
    if (tzArray.length == 0) {
        let rsp = await getTimezoneOffset();
        tzArray.push(rsp.data);
    }
    let serverTime = new Date(Date.parse(timestr.replace(/-/g, "/")));
    let tz = new Date().getTimezoneOffset();
    let localDate = new Date(serverTime.getTime() + tzArray[0] * 60000 - tz * 60000);
    return parseTime(localDate);
}