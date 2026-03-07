import serverUrl from '@/common/constVar.js'
export function retunWeatherImg(val) {
	// console.log("天气值",val);
	if(val=='晴'||val=='热'){
		return serverUrl.getServerUrl()+'/appimg/weather_img/qing.png'
	}else if(val.indexOf('云')>-1){
		return serverUrl.getServerUrl()+'/appimg/weather_img/duoyun.png'
	}else if(val.indexOf('风')>-1){
		if(val.indexOf('暴')>-1||val.indexOf('飓')>-1||val.indexOf('龙卷风')>-1){
			return serverUrl.getServerUrl()+'/appimg/weather_img/baofeng.png'
		}else{
			return serverUrl.getServerUrl()+'/appimg/weather_img/dafeng.png'
		}
	}else if(val.indexOf('霾')>-1||val.indexOf('雾')>-1){
		return serverUrl.getServerUrl()+'/appimg/weather_img/wumai.png'
	}else if(val=='阴'||val=='平静'){
		return serverUrl.getServerUrl()+'/appimg/weather_img/yintian.png'
	}else if(val.indexOf('雨')>-1&&val.indexOf('雪')==-1){
		if(val.indexOf('阵雨')>-1&&val.indexOf('雷')>-1){
			return serverUrl.getServerUrl()+'/appimg/weather_img/leizhenyu.png'
		}else if(val.indexOf('阵雨')>-1&&val.indexOf('雷')==-1){
			return serverUrl.getServerUrl()+'/appimg/weather_img/zhenyu.png'
		}else if(val.indexOf('阵雨')==-1){
			if(val.indexOf('中雨')>-1&&val.indexOf('小雨')==-1){
				return serverUrl.getServerUrl()+'/appimg/weather_img/zhongyu.png'
			}else if(val.indexOf('大雨')>-1&&val.indexOf('中雨')==-1){
				return serverUrl.getServerUrl()+'/appimg/weather_img/dayu.png'
			}else if(val.indexOf('暴雨')>-1||val.indexOf('极端降雨')>-1){
				return serverUrl.getServerUrl()+'/appimg/weather_img/baoyu.png'
			}else{
				return serverUrl.getServerUrl()+'/appimg/weather_img/xiaoyu.png'
			}
		}
	}else if(val.indexOf('雨')>-1&&val.indexOf('雪')>-1){
		return serverUrl.getServerUrl()+'/appimg/weather_img/yuxue.png'
	}else if(val.indexOf('雨')>-1&&val.indexOf('冻')>-1){
		return serverUrl.getServerUrl()+'/appimg/weather_img/yuxue.png'
	}else if(val.indexOf('雪')>-1&&val.indexOf('雨')==-1){
		if(val.indexOf('中雪')>-1&&val.indexOf('小雪')==-1){
			return serverUrl.getServerUrl()+'/appimg/weather_img/zhongxue.png'
		}else if(val.indexOf('大雪')>-1||val.indexOf('暴雪')>-1){
			return serverUrl.getServerUrl()+'/appimg/weather_img/daxue.png'
		}else{
			return serverUrl.getServerUrl()+'/appimg/weather_img/xiaoxue.png'
		}
		// return serverUrl.getServerUrl()+'/appimg/weather_img/yuxue.png'
	}else if(val=='浮尘'||val=='扬沙'){
		return serverUrl.getServerUrl()+'/appimg/weather_img/fuchen.png'
	}else if(val.indexOf('沙尘暴')>-1){
		return serverUrl.getServerUrl()+'/appimg/weather_img/shachenbao.png'
	}
}