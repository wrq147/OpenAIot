export function dealWithData(data,field) {
    let map = {}
    for (let i = 0; i < data.length; i++) {
      let ai = data[i]
      if (!map[ai[field]]) {
          map[ai[field]] = [ai]
      } else {
          map[ai[field]].push(ai)
      }
    }
    let res = []
    Object.keys(map).forEach(key => {
        res.push({
            type: key,
            data: map[key],
        })
    })

    return res;
}

export function sortsFunAsc(arr) {
    return function(a, b) {
        var v1 = a[arr];
        var v2 = b[arr];
        return v1 - v2;
    };
};

export function sortsFunDesc(arr) {
    return function(a, b) {
        var v1 = a[arr];
        var v2 = b[arr];
        return v2 - v1;
    };
};

export const testData = 
[
    {
        "name": "1月",
        "value": 2,
        "series": "蒸发量",
        "type": "bar"
    },
    {
        "name": "2月",
        "value": 4.9,
        "series": "蒸发量",
        "type": "bar"
    },
    {
        "name": "3月",
        "value": 7,
        "series": "蒸发量",
        "type": "bar"
    },
    {
        "name": "4月",
        "value": 23.2,
        "series": "蒸发量",
        "type": "bar"
    },
    {
        "name": "5月",
        "value": 25.6,
        "series": "蒸发量",
        "type": "bar"
    },
    {
        "name": "6月",
        "value": 76.7,
        "series": "蒸发量",
        "type": "bar"
    },
    {
        "name": "7月",
        "value": 135.6,
        "series": "蒸发量",
        "type": "bar"
    },
    {
        "name": "8月",
        "value": 162.2,
        "series": "蒸发量",
        "type": "bar"
    },
    {
        "name": "9月",
        "value": 32.6,
        "series": "蒸发量",
        "type": "bar"
    },
    {
        "name": "10月",
        "value": 20,
        "series": "蒸发量",
        "type": "bar"
    },
    {
        "name": "11月",
        "value": 6.4,
        "series": "蒸发量",
        "type": "bar"
    },
    {
        "name": "12月",
        "value": 3.3,
        "series": "蒸发量",
        "type": "bar"
    },
    {
        "name": "1月",
        "value": 2.6,
        "series": "降水量",
        "type": "bar"
    },
    {
        "name": "2月",
        "value": 5.9,
        "series": "降水量",
        "type": "bar"
    },
    {
        "name": "3月",
        "value": 9,
        "series": "降水量",
        "type": "bar"
    },
    {
        "name": "4月",
        "value": 26.4,
        "series": "降水量",
        "type": "bar"
    },
    {
        "name": "5月",
        "value": 28.7,
        "series": "降水量",
        "type": "bar"
    },
    {
        "name": "6月",
        "value": 70.7,
        "series": "降水量",
        "type": "bar"
    },
    {
        "name": "7月",
        "value": 175.6,
        "series": "降水量",
        "type": "bar"
    },
    {
        "name": "8月",
        "value": 182.2,
        "series": "降水量",
        "type": "bar"
    },
    {
        "name": "9月",
        "value": 48.7,
        "series": "降水量",
        "type": "bar"
    },
    {
        "name": "10月",
        "value": 18.8,
        "series": "降水量",
        "type": "bar"
    },
    {
        "name": "11月",
        "value": 6,
        "series": "降水量",
        "type": "bar"
    },
    {
        "name": "12月",
        "value": 2.3,
        "series": "降水量",
        "type": "bar"
    },
    {
        "name": "1月",
        "value": 2,
        "series": "平均温度",
        "type": "line"
    },
    {
        "name": "2月",
        "value": 2.2,
        "series": "平均温度",
        "type": "line"
    },
    {
        "name": "3月",
        "value": 3.3,
        "series": "平均温度",
        "type": "line"
    },
    {
        "name": "4月",
        "value": 4.5,
        "series": "平均温度",
        "type": "line"
    },
    {
        "name": "5月",
        "value": 6.3,
        "series": "平均温度",
        "type": "line"
    },
    {
        "name": "6月",
        "value": 10.2,
        "series": "平均温度",
        "type": "line"
    },
    {
        "name": "7月",
        "value": 20.3,
        "series": "平均温度",
        "type": "line"
    },
    {
        "name": "8月",
        "value": 23.4,
        "series": "平均温度",
        "type": "line"
    },
    {
        "name": "9月",
        "value": 23,
        "series": "平均温度",
        "type": "line"
    },
    {
        "name": "10月",
        "value": 16.5,
        "series": "平均温度",
        "type": "line"
    },
    {
        "name": "11月",
        "value": 12,
        "series": "平均温度",
        "type": "line"
    },
    {
        "name": "12月",
        "value": 18,
        "series": "平均温度",
        "type": "line"
    }
]