/**
 * url参数转成json对象
 * @param {*} urlParam 
 */
export function parseQueryString(urlParam) {
    let params = {};
    let arr = urlParam.split("&");
    for(let i = 0, l = arr.length; i < l; i++) {
        let a = arr[i].split("=");
        params[a[0]] = a[1];
    }
    return params;
}

/**
 * 校验url是否合法
 * @param {*} sUrl 
 */
export function fIsUrL(sUrl) {
    var sRegex = '^((https|http|ftp|rtsp|mms)?://)' + '?(([0-9a-z_!~*\'().&=+$%-]+: )?[0-9a-z_!~*\'().&=+$%-]+@)?' //ftp的user@ 
        + '(([0-9]{1,3}.){3}[0-9]{1,3}' // IP形式的URL- 199.194.52.184 
        + '|' // 允许IP和DOMAIN（域名） 
        + '([0-9a-z_!~*\'()-]+.)*' // 域名- www. 
        + '([0-9a-z][0-9a-z-]{0,61})?[0-9a-z].' // 二级域名 
        + '[a-z]{2,6})' // first level domain- .com or .museum 
        + '(:[0-9]{1,4})?' // 端口- :80 
        + '((/?)|' // a slash isn't required if there is no file name 
        + '(/[0-9a-z_!~*\'().;?:@&=+$,%#-]+)+/?)$';
    var re = new RegExp(sRegex);
    //re.test() 
    if (re.test(sUrl)) {
        return true;
    }
    return false;
}