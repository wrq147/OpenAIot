import axios from 'axios'
import { getToken } from '@/utils/auth'

/**
 * api接口调用
 * @param {*} dataOption 
 * @returns 
 */
export async function chartApi(dataOption) {
    let hdmap = {};
    if (dataOption.requestHeader) {
        dataOption.requestHeader.forEach(element => {
            if (element.value == "$Authorization") {
                hdmap[element.name] = getToken();
            } else {
                hdmap[element.name] = element.value;
            }
        });
    }

    let config = {
        headers: hdmap,
    }
    if (dataOption.requestParamType == "PARAM") {
        if (dataOption.requestParameters) {
            let query = {};
            dataOption.requestParameters.forEach(element => {
                query[element.name] = element.value;
            });
            config["params"] = query;
        }

    } else if (dataOption.requestParamType == "JSON") {
        if (dataOption.requestMethod == "GET") {
            config["params"] = dataOption.requestParameters;
        } else {
            config["data"] = dataOption.requestParameters;
        }

    } else if (dataOption.requestParamType == "FORM") {
        if (dataOption.requestParameters) {
            let postdata = {};
            dataOption.requestParameters.forEach(element => {
                postdata[element.name] = element.value;
            });
            config["data"] = postdata;
        }

    }

    let httpMethod = "get";
    if (dataOption.requestMethod == "GET") {
        httpMethod = "get";
    } else if (dataOption.requestMethod == "POST") {
        httpMethod = "post";
    } else if (dataOption.requestMethod == "PUT") {
        httpMethod = "put";
    } else if (dataOption.requestMethod == "DELETE") {
        httpMethod = "delete";
    }
    config["url"] = dataOption.interfaceURL;
    config["method"] = httpMethod;
    let rss = await axios(config);
    return rss.data;
}