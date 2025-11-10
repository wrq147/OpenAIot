import { saveAs } from 'file-saver'
import axios from 'axios'
import { getToken } from '@/utils/auth'
import qs from 'qs';
export default {
    resource(url, query) {
        let paramsSerializer = function(params) {
            for (const propName of Object.keys(params)) {
                const value = params[propName];
                // console.log(value, 'value');

                if (value === null || typeof(value) === "undefined") {
                    delete params[propName]
                        // Vue.delete(params, propName)
                        // console.log("get方法传参dddd", params, value);
                }
            }
            // console.log("get方法传参", params);

            return qs.stringify(params, { arrayFormat: 'repeat' })
        }
        axios({
            method: 'get',
            baseURL: process.env.VUE_APP_BASE_API,
            url: url,
            responseType: 'blob',
            params: query,
            paramsSerializer: paramsSerializer,
            headers: { 'Authorization': getToken() }
        }).then(res => {
            const blob = new Blob([res.data])
            this.saveAs(blob, decodeURI(res.headers['download-filename']))
        })
    },
    zip(url, name) {
        axios({
            baseURL: process.env.VUE_APP_BASE_API,
            method: 'get',
            url: url,
            responseType: 'blob',
            headers: { 'Authorization': getToken() }
        }).then(res => {
            const blob = new Blob([res.data], { type: 'application/zip' })
            this.saveAs(blob, name)
        })
    },
    saveAs(text, name, opts) {
        saveAs(text, name, opts);
    }
}