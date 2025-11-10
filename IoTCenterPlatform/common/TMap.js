export function TMap(key){
	return new Promise(function(resolve,reject){
		window.init=function(){
			resolve(qq)
		}
		var script=document.createElement('script');
		script.type='text/javascript';
		script.src='https://apis.map.qq.com/ws/geocoder/v1/?v-2.exp&callback=init&key=CAUBZ-PUFCO-7V4WO-SD2KN-4KYL2-CMB6H'
		script.onerror=reject
		document.head.appendChild(script)
	})
}