function getUrlParam(url, name) {
	url = url.substr(url.indexOf("?"));
	var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
	var r = url.substr(1).match(reg);
	if (r != null) return unescape(r[2]);
	return null;
}

function formatTime(time) {
	if (typeof time !== 'number' || time < 0) {
		return time
	}

	var hour = parseInt(time / 3600)
	time = time % 3600
	var minute = parseInt(time / 60)
	time = time % 60
	var second = time

	return ([hour, minute, second]).map(function(n) {
		n = n.toString()
		return n[1] ? n : '0' + n
	}).join(':')
}

function formatLocation(longitude, latitude) {
	if (typeof longitude === 'string' && typeof latitude === 'string') {
		longitude = parseFloat(longitude)
		latitude = parseFloat(latitude)
	}

	longitude = longitude.toFixed(2)
	latitude = latitude.toFixed(2)

	return {
		longitude: longitude.toString().split('.'),
		latitude: latitude.toString().split('.')
	}
}


var dateUtils = {
	UNITS: {
		'年': 31557600000,
		'月': 2629800000,
		'天': 86400000,
		'小时': 3600000,
		'分钟': 60000,
		'秒': 1000
	},
	humanize: function(milliseconds) {
		var humanize = '';
		for (var key in this.UNITS) {
			if (milliseconds >= this.UNITS[key]) {
				humanize = Math.floor(milliseconds / this.UNITS[key]) + key + '前';
				break;
			}
		}
		return humanize || '刚刚';
	},
	format: function(dateStr) {
		var date = this.parse(dateStr)
		var diff = Date.now() - date.getTime();
		if (diff < this.UNITS['天']) {
			return this.humanize(diff);
		}
		var _format = function(number) {
			return (number < 10 ? ('0' + number) : number);
		};
		return date.getFullYear() + '/' + _format(date.getMonth() + 1) + '/' + _format(date.getDate()) + '-' +
			_format(date.getHours()) + ':' + _format(date.getMinutes());
	},
	parse: function(str) { //将"yyyy-mm-dd HH:MM:ss"格式的字符串，转化为一个Date对象
		return new Date(Date.parse(str.replace(/-/g, "/")));
	}
};



/**
 * 数组转树
 * @param {*} list 
 * @param {*} parentKey 父键
 * @param {*} childrenKey 子数组键
 */
function totree(list, idKey, parentKey, childrenKey, cb) {
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

/**
 * 随机密码
 */
function randomStr() {
	var datetime = new Date();
	var year = datetime.getFullYear().toString().substr(2, 2);
	var month = datetime.getMonth() + 1 < 10 ? "0" + (datetime.getMonth() + 1) : datetime.getMonth() + 1;
	var date = datetime.getDate() < 10 ? "0" + datetime.getDate() : datetime.getDate();
	return year + month + date;
}

/**
 * 执行上一个页面的reloadpage方法
 */
function reloadPrePage(num, name) {
	if (num == null) {
		num = 1;
	}
	var pages = getCurrentPages();
	// console.log("页面数量",pages);
	if (pages.length > num) {
		var page = pages[pages.length - (num + 1)];
		if (page == undefined || page == null) return;
		if (typeof page.$vm.reloadpage != 'undefined') {
			page.$vm.reloadpage(name);
		}
	}
}

//实现在本页设置上一页的数据
//需要  传递参数的那一页的页码，传递的参数，
/** 关闭当前页面传参到上一个页面
 * parent_fun_name 上一个页面对应函数名称  async 方法旋转不到
 * params 传递的参数
 * prev 前第几个页面
 * delay 延迟关闭当前页面的时间
 */
export function setPagesParam(parent_fun_name, params, prev = 1, delay = 1000) {
	var pages = getCurrentPages();
	var prevPage = pages[pages.length - (1 + prev)];
	parent_fun_name && prevPage.$vm[parent_fun_name] && prevPage.$vm[parent_fun_name](params)
		uni.navigateBack({
			delta: prev
		})
}

/**
 * 判断字符串是否为null或空
 */
function isNullOrEmpty(str) {
	return str == null || str == "";
}


export {
	formatTime,
	formatLocation,
	dateUtils,
	totree,
	randomStr,
	reloadPrePage,
	getUrlParam,
	isNullOrEmpty
}
