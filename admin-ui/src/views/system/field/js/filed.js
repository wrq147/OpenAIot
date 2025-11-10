export const pickerOptions = {
    shortcuts: [{
        text: '今天',
        onClick(picker) {
            const end = new Date();
            const start = new Date();
            start.setHours(0, 0, 0, 0)
            end.setHours(23, 59, 59, 999)
            picker.$emit('pick', [start, end]);
        }
    }, {
        text: '昨天',
        onClick(picker) {
            let now = new Date();
            const end = new Date();
            const start = new Date();
            start.setDate(now.getDate() - 1); // 昨天的日期
            start.setHours(0, 0, 0, 0); // 设置时间为当天的开始（0点）
            end.setDate(now.getDate() - 1); // 昨天的日期
            end.setHours(23, 59, 59); // 设置时间为当天的结束（23点59分59秒）
            picker.$emit('pick', [start, end]);
        }
    }, {
        text: '上周',
        onClick(picker) {
            const now = new Date();
            const end = new Date();
            const start = new Date();
            // 计算上周周一的日期
            start.setDate(now.getDate() - now.getDay() - 6); // 当前日期减去当前星期几再减去6（周一为0，周日为6）
            start.setHours(0, 0, 0, 0); // 设置时间为0点

            // 计算本周周日的日期
            end.setDate(now.getDate() + (7 - now.getDay())); // 当前日期加上（7 - 当前星期几，周日为0）
            end.setHours(23, 59, 59, 999); // 设置时间为23点59分59秒
            picker.$emit('pick', [start, end]);
        }
    }, {
        text: '本周',
        onClick(picker) {
            const start = new Date(now.setDate(now.getDate() - now.getDay() + 1)); // 本周一
            const end = new Date(now.setDate(now.getDate() - now.getDay() + 7)); // 本周日 23:59:59
            end.setHours(23, 59, 59, 999); // 确保是23:59:59
            picker.$emit('pick', [start, end]);
        }
    }, {
        text: '上月',
        onClick(picker) {
            // const end = new Date();
            // const start = new Date();
            // start.setTime(start.getTime() - 3600 * 1000 * 24 * 30);
            const now = new Date();
            let start = new Date(now.getFullYear(), now.getMonth() - 1, 1); // 上个月的第一天
            let end = new Date(now.getFullYear(), now.getMonth(), 0); // 上个月的最后一天
            // 确保时间部分为00:00:00和23:59:59
            start.setHours(0, 0, 0, 0);
            end.setHours(23, 59, 59);
            picker.$emit('pick', [start, end]);
        }
    }, {
        text: '本月',
        onClick(picker) {
            const now = new Date();
            const year = now.getFullYear();
            const month = now.getMonth(); // 注意：月份是从0开始的，0代表1月，11代表12月
            // 设置月份的第一天（00:00:00）
            const start = new Date(year, month, 1); // 月份是从0开始，所以要用month而不是month+1
            start.setHours(0, 0, 0, 0); // 确保时间是00:00:00.000
            // 设置月份的最后一天（23:59:59）
            const end = new Date(year, month + 1, 0); // 月份加1后设置为下个月的第一天的前一天，即为当前月的最后一天
            end.setHours(23, 59, 59, 999); // 确保时间是23:59:59.999
            picker.$emit('pick', [start, end]);
        }
    }, {
        text: '近三月',
        onClick(picker) {
            const end = new Date();
            const start = new Date();
            start.setTime(start.getTime() - 3600 * 1000 * 24 * 90);
            picker.$emit('pick', [start, end]);
        }
    }]
}