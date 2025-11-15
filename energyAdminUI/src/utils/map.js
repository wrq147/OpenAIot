import { getConfigKey } from "@/api/system/config.js";
import Vue from 'vue'
export function qqMap(key) {
    return new Promise(function(resolve, reject) {
        window.init = function() {
            resolve();
        }

        var script = document.createElement("script");
        script.type = "text/javascript";
        script.src = "https://map.qq.com/api/js?v=2.exp&key=" + key;
        script.onerror = reject;
        document.head.appendChild(script);

        var script2 = document.createElement("script");
        script2.type = "text/javascript";
        script2.src = "https://map.qq.com/api/gljs?v=1.exp&libraries=service&key=" + key;
        script2.onerror = reject;
        document.head.appendChild(script2);
    })
}
export async function initMap() {
    if (!sessionStorage.getItem("map.key")) {
        //如果本地没有获取到腾讯地图密钥，重新获取密钥并设置

        let res = await getConfigKey("map.key");
        if (res.data == "") {
            new Vue().$message.warning('腾讯地图Key参数没有设置，将使用系统自带逆地址解释')

            let tmpkey = "R63BZ-X4G6V-NILP7-5TKXR-XTUSQ-OYFRO";
            sessionStorage.setItem("map.key", tmpkey);
            await qqMap(tmpkey);
        } else {
            sessionStorage.setItem("map.key", res.data);
            await qqMap(res.data);
        }
    } else if (sessionStorage.getItem("map.key")) {
        await qqMap(sessionStorage.getItem("map.key"));
    }
}