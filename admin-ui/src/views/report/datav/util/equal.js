export function isEqual(objA,objB){
  //判断两个对象是否相等
    //相等
    if(objA === objB) return objA !== 0 || 1/objA === 1/objB;
    //空判断
    if(objA == null || objB == null) return objA === objB;
    //类型判断
    if(Object.prototype.toString.call(objA) !== Object.prototype.toString.call(objB)) return false;

    switch(Object.prototype.toString.call(objA)){
        case '[object RegExp]':
        case '[object String]':
            //字符串转换比较
            return '' + objA ==='' + objB;
        case '[object Number]':
            //数字转换比较,判断是否为NaN
            if(+objA !== +objA){
                return +objB !== +objB;
            }

            return +objA === 0?1/ +objA === 1/objB : +objA === +objB;
        case '[object Date]':
        case '[object Boolean]':
            return +objA === +objB;
        case '[object Array]':
            //判断数组
            for(let i = 0; i < objA.length; i++){
                if (!this.isEqual(objA[i],objB[i])) return false;
            }
            return true;
        case '[object Object]':
            //判断对象
            let keys = Object.keys(objA);
            for(let i = 0; i < keys.length; i++){
                if (!this.isEqual(objA[keys[i]],objB[keys[i]])) return false;
            }

            keys = Object.keys(objB);
            for(let i = 0; i < keys.length; i++){
                if (!this.isEqual(objA[keys[i]],objB[keys[i]])) return false;
            }

            return true;
        default :
            return false;
    }

}