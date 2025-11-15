import { nanoid } from 'nanoid'
import qrcode from "qrcode";
export default {
    methods: {
        deepCopy(obj, cache = []) {
            if (obj === null || typeof obj !== 'object') {
                return obj
            }
            const objType = Object.prototype.toString.call(obj).slice(8, -1)
                // 考虑 正则对象的copy
            if (objType === 'RegExp') {
                return new RegExp(obj)
            }
            // 考虑 Date 实例 copy
            if (objType === 'Date') {
                return new Date(obj)
            }
            // 考虑 Error 实例 copy
            if (objType === 'Error') {
                return new Error(obj)
            }
            const hit = cache.filter((c) => c.original === obj)[0]
            if (hit) {
                return hit.copy
            }
            const copy = Array.isArray(obj) ? [] : {}
            cache.push({ original: obj, copy })
            Object.keys(obj).forEach((key) => {
                copy[key] = this.deepCopy(obj[key], cache)
            })
            return copy
        },
        getUuid(length = 8) {
            return nanoid(length)
        },
        isBlank(value) {
            return value === undefined || value === null || value === ''
        },
        /**
         * 通过name查找父组件
         * @param {*} vueIns
         * @param {*} name
         */
        findParentComponent(vueIns, name) {
            let parent = vueIns.$parent
            while (parent) {
                let componentName = parent.$options.componentName || parent.$options.name
                if (componentName !== name) {
                    parent = parent.$parent
                } else {
                    return parent
                }
            }
            return false
        },
        createCode(val){//生成二维码
            let codeStr='nodata'
            if(val.srcSetting&&val.srcSetting.length>0){
                for(let i=0;i<val.srcSetting.length;i++){
                    let row=val.srcSetting[i]
                    if(codeStr&&codeStr.indexOf('nodata')==-1){
                        if(row.type=='text'){
                            codeStr=codeStr+row.value
                        }
                        if(row.type=='source'){
                            let findVal=this.dataSet[row.value]
                            codeStr=codeStr+findVal
                        }
                    }else{
                        if(row.type=='text'){
                            codeStr=row.value
                        }
                        if(row.type=='source'){
                            let findVal=this.dataSet[row.value]
                            codeStr=findVal
                        }
                    }
                }
            }
            qrcode.toDataURL(codeStr, {
                version:"",// 二维码版本。如果未指定，将计算更合适的值。
                errorCorrectionLevel:"M", // 纠错级别。low, medium, quartile, high , L, M, Q, H
                maskPattern: 1, // 0、1、2、3、4、5、6、7
                toSJISFunc(){}, // 
                margin:1, // 边距
                scale:40, // 每一个黑点的宽度
                width:4, // 二维码宽
                'color.dark':"#000000ff", // 二维码颜色
                'color.light':"#ffffffff" // 背景色
            })
            .then(url => {
                // console.log('生成二维码图片url：', url)
                // url赋值给img标签即可
                this.codeSrc=url
            })
            .catch((err) => {
                console.log("二维码生成失败",err);
                this.createCode({})
            });
        }
    }
}