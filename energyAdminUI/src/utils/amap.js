import { getConfigKey } from "@/api/system/config.js";
import Vue from 'vue'
export const loadAMapScript = (key, secret) => {
    return new Promise((resolve, reject) => {
        window.init = function() {
            resolve();
        }

        var script = document.createElement("script");
        script.type = "text/javascript";
        let src = `https://webapi.amap.com/maps?v=2.0&key=${key}&style=5&version=1.4.28&rd=1`;

        script.src = src;
        script.onerror = reject;
        document.head.appendChild(script);
    });
};
export async function initMap() {
    if (!sessionStorage.getItem("map.gdsecretkey") || !sessionStorage.getItem("map.gdsecret")) {
        //如果本地没有获取到腾讯地图密钥，重新获取密钥并设置

        // let res = await getConfigKey("map.gdsecretkey");
        // let res2 = await getConfigKey("map.gdsecret");
        // if (res.data == "" || res2.data == '') {
        //     new Vue().$message.warning('高德地图Key参数没有设置，将使用系统自带逆地址解释')

        let tmpkey = "87802b0f37dc5d028ec2402c7d92938d";
        let tmpSecret = "a3c5a12d36d302b0aba08542ae70d340";
        sessionStorage.setItem("map.gdsecretkey", tmpkey);
        sessionStorage.setItem("map.gdsecret", tmpSecret);
        window._AMapSecurityConfig = {
            securityJsCode: tmpSecret,
        };
        await loadAMapScript(tmpkey);
        // } else {
        //     sessionStorage.setItem("map.gdsecretkey", res.data);
        //     sessionStorage.setItem("map.gdsecret", res2.data);
        //     window._AMapSecurityConfig = {
        //         securityJsCode: res2.data,
        //     };
        //     await loadAMapScript(res.data);
        // }
    } else if (sessionStorage.getItem("map.gdsecretkey") && sessionStorage.getItem("map.gdsecret")) {
        window._AMapSecurityConfig = {
            securityJsCode: sessionStorage.getItem("map.gdsecret"),
        };
        await loadAMapScript(sessionStorage.getItem("map.gdsecretkey"), sessionStorage.getItem("map.gdsecret"));
    }
}