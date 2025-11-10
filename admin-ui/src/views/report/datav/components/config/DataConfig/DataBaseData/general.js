export const jslist = [{
    type: 'timeConversion',
    text: '时间转化',
}, {
    type: 'arrayGroupBy',
    text: '数组分组',
}, {
    type: 'objToArray',
    text: '对象转数组'
}]
export const jsObject = {
    timeConversion: `function parseTime(time, pattern) {
        //time:传入的时间
        //pattern:转换的时间类型,y表示年,m表示月,d表示日,h表示小时,i表示分钟,s表示秒
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
      let nowtime=parseTime(new Date(),'yyyy-mm-dd')
      `,
    arrayGroupBy: `function groupBy(array, key) {
        //array表示传入的需要分组的数组
        //key表示分组字段
            return array.reduce((result, currentItem) => {
                // 使用 key 函数提取分组键，如果未定义则直接使用属性名
                const groupKey = typeof key === 'function' ? key(currentItem) : currentItem[key];
                // console.log("分组",groupKey);
                // 初始化分组数组
                if (!result[groupKey]) {
                    result[groupKey] = [];
                }
                // 将当前项添加到分组数组中
                result[groupKey].push(currentItem);

                return result;

            }, {});
        }`,
    objToArray: `function objToArr(obj,keyName,valueName){
        let resArray=[]
        for(let key in obj){
          let rowObj={}
          rowObj[keyName]=key
          rowObj[valueName]=obj[key]
          resArray.push(rowObj)
        }
        return resArray
      }
      let array=objToArr(result[0],'text','value')`

}