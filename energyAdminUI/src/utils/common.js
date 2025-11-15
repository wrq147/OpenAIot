import { getTimezoneOffset } from "@/api/system/mqtt.js";
import ExcelJS from 'exceljs';
import { saveAs } from 'file-saver';

export function exportExcleUtils(tHeader, filterVal, listData, fileName) {
  //设置工作簿属性
  const workbook = new ExcelJS.Workbook();
  workbook.creator = 'Me';
  workbook.lastModifiedBy = 'Her';
  workbook.created = new Date();
  workbook.modified = new Date();
  workbook.lastPrinted = new Date();
  //添加工作薄
  const worksheet = workbook.addWorksheet('sheet1');
  //计算标题宽
  let colWidth = calculateColumnWidth(tHeader);
//   console.log(colWidth);
  let style = { alignment: { vertical: 'middle', horizontal: 'center' } };
  let columns = [];
  for (let i = 0; i < tHeader.length; i++) {
    let header = {
      header: tHeader[i],
      key: filterVal[i] ? filterVal[i] : i + '',
      width: colWidth[i] ? colWidth[i] + 20 : 10,
      style: JSON.parse(JSON.stringify(style)),
    };
    columns.push(header);
  }
  worksheet.columns = columns;

  //添加数据
  listData.forEach((item) => {
    worksheet.addRow(item);
  });

  worksheet.getRow(1).font = { name: '宋体', family: 4, size: 16, bold: true };

  fileName = fileName + '.xlsx';
  workbook.xlsx.writeBuffer().then((data) => {
    let blob = new Blob([data], {
      type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
    });
    saveAs(blob, fileName);
  });
}
export function exportExcleUtils2(tHeader, filterVal, listData, fileName) {
    //设置工作簿属性
    const workbook = new ExcelJS.Workbook();
    workbook.creator = 'Me';
    workbook.lastModifiedBy = 'Her';
    workbook.created = new Date();
    workbook.modified = new Date();
    workbook.lastPrinted = new Date();
    //添加工作薄
    const worksheet = workbook.addWorksheet('sheet1');
     // 添加表头并合并单元格
    // worksheet.mergeCells('A1:D1'); // 假设合并从A1到D1的单元格
    //计算标题宽
    let colWidth = calculateColumnWidth(tHeader);
  //   console.log(colWidth);
    let style = { alignment: { vertical: 'middle', horizontal: 'center' } };
    let columns = [];
    for (let i = 0; i < tHeader.length; i++) {
      let header = {
        header: [tHeader[i].label,tHeader[i].column],
        key: filterVal[i] ? filterVal[i] : i + '',
        width: colWidth[i] ? colWidth[i] + 20 : 10,
        style: JSON.parse(JSON.stringify(style)),
        columns:[]
      };
      columns.push(header);
    }

    worksheet.columns = columns;
    let dataHeadArr=uniqueByField(tHeader,'label')
    for(let j=0;j<dataHeadArr.length;j++){
        worksheet.mergeCells(dataHeadArr[j].range[0],dataHeadArr[j].range[1],dataHeadArr[j].range[2],dataHeadArr[j].range[3])
    }
    // 在表头前插入一行
    // 或者使用header属性定义双层表头
    // worksheet.header = tHeader;
    //添加数据
    listData.forEach((item) => {
      worksheet.addRow(item);
    });
  
    worksheet.getRow(1).font = { name: '宋体', family: 4, size: 16, bold: true };
  
    fileName = fileName + '.xlsx';
    workbook.xlsx.writeBuffer().then((data) => {
      let blob = new Blob([data], {
        type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
      });
      saveAs(blob, fileName);
    });
  }
  export function uniqueByField(arr, field) {//数组去重
    return arr.reduce((acc, current) => {
      const exists = acc.find((item) => item[field] === current[field]);
      if (!exists) {
        acc.push(current);
      }
      return acc;
    }, []);
  }
export function calculateColumnWidth(tHeader) {
  let colWidth = [];
  //计算列宽
  for (let i = 0; i < tHeader.length; i++) {
    if (colWidth[i] && colWidth[i] < tHeader[i].length) {
      colWidth[i] = tHeader[i].length * 2;
    } else if (colWidth[i] === undefined) {
      colWidth[i] = tHeader[i].length * 2;
    }
  }
  return colWidth;
}


/**
 * 通用js方法封装处理
 * Copyright (c) 2019 ruoyi
 */

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

// 表单重置
export function resetForm(refName) {
    if (this.$refs[refName]) {
        this.$refs[refName].resetFields();
    }
}

// 添加日期范围
export function addDateRange(params, dateRange, propName) {
    let search = params;
    dateRange = Array.isArray(dateRange) ? dateRange : [];
    let seccdate = dateRange[1];
    if (dateRange.length > 1) {
        if (typeof dateRange[1] === 'string') {
            if (dateRange[1].indexOf(":") < 0) {
                seccdate = dateRange[1];
            }
        }
    }

    if (typeof (propName) === 'undefined') {
        search['beginTime'] = dateRange[0];
        search['endTime'] = seccdate;
    } else {
        if (typeof (propName) !== 'string' && propName.length == 2) {
            search[propName[0]] = dateRange[0];
            search[propName[1]] = seccdate;
        } else {
            search['begin' + propName] = dateRange[0];
            search['end' + propName] = seccdate;
        }

    }
    return search;
}

// 回显数据字典
export function selectDictLabel(datas, value) {
    var actions = [];
    Object.keys(datas).some((key) => {
        if (datas[key].value == ('' + value)) {
            actions.push(datas[key].label);
            return true;
        }
    })
    return actions.join('');
}

// 回显数据字典（字符串数组）
export function selectDictLabels(datas, value, separator) {
    var actions = [];
    var currentSeparator = undefined === separator ? "," : separator;
    var temp = value.split(currentSeparator);
    Object.keys(value.split(currentSeparator)).some((val) => {
        Object.keys(datas).some((key) => {
            if (datas[key].dictValue == ('' + temp[val])) {
                actions.push(datas[key].dictLabel + currentSeparator);
            }
        })
    })
    return actions.join('').substring(0, actions.join('').length - 1);
}

// 字符串格式化(%s )
export function sprintf(str) {
    var args = arguments,
        flag = true,
        i = 1;
    str = str.replace(/%s/g, function () {
        var arg = args[i++];
        if (typeof arg === 'undefined') {
            flag = false;
            return '';
        }
        return arg;
    });
    return flag ? str : '';
}

// 转换字符串，undefined,null等转化为""
export function praseStrEmpty(str) {
    if (!str || str == "undefined" || str == "null") {
        return "";
    }
    return str;
}

// 数据合并
export function mergeRecursive(source, target) {
    for (var p in target) {
        try {
            if (target[p].constructor == Object) {
                source[p] = mergeRecursive(source[p], target[p]);
            } else {
                source[p] = target[p];
            }
        } catch (e) {
            source[p] = target[p];
        }
    }
    return source;
};

/**
 * 构造树型结构数据
 * @param {*} data 数据源
 * @param {*} id id字段 默认 'id'
 * @param {*} parentId 父节点字段 默认 'parentId'
 * @param {*} children 孩子节点字段 默认 'children'
 */
export function handleTree(data, id, parentId, children) {
    let config = {
        id: id || 'id',
        parentId: parentId || 'parentId',
        childrenList: children || 'children'
    };

    var childrenListMap = {};
    var nodeIds = {};
    var tree = [];

    for (let d of data) {
        let parentId = d[config.parentId];
        if (childrenListMap[parentId] == null) {
            childrenListMap[parentId] = [];
        }
        nodeIds[d[config.id]] = d;
        childrenListMap[parentId].push(d);
    }

    for (let d of data) {
        let parentId = d[config.parentId];
        if (nodeIds[parentId] == null) {
            tree.push(d);
        }
    }

    for (let t of tree) {
        adaptToChildrenList(t);
    }

    function adaptToChildrenList(o) {
        if (childrenListMap[o[config.id]] !== null) {
            o[config.childrenList] = childrenListMap[o[config.id]];
        }
        if (o[config.childrenList]) {
            for (let c of o[config.childrenList]) {
                adaptToChildrenList(c);
            }
        }
    }
    return tree;
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
    list.forEach(function (item) {
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
 * 获取数据源指定字段值，不存在则返回默认值
 * @param {*} obj 数据源
 * @param {*} key 字段
 * @param {*} df 默认值
 * @returns 
 */
export function getDefalut(obj, key, df) {
    return (obj === undefined || key === undefined || !this.$isNotEmpty(obj[key])) ? df : obj[key];
}
/**
 * 判断对象是否不为空
 * @param {*} obj 
 * @returns 
 */
export function isNotEmpty(obj) {
    return (obj !== undefined && obj !== null && obj !== '' && obj !== 'null')
}

const tzArray = new Array();
/**
 * 服务端时间转当前时间
 * @param {*} time 
 * @returns 
 */
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
