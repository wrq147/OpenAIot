// #ifdef H5
var jWeixin = require('jweixin-module')
//h5扫码相关方法封装
//链接取值
export function getTargetUrlParam(url, name) { //取链接上的值
	url = url.substr(url.indexOf("?"));
	var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
	var r = url.substr(1).match(reg);
	if (r != null) return unescape(r[2]);
	return null;
}

export function scan(pagetype, cb) {
	jWeixin.scanQRCode({
		needResult: 1, // 默认为0，扫描结果由微信处理，1则直接返回扫描结果，
		scanType: ['qrCode', 'barCode'], // 可以指定扫二维码还是一维码，默认二者都有
		success: function(res) {
			if (res != null) {
				let currenturl = res.resultStr;
				console.log("H5扫码结果", res);
				uni.showToast({
					title: JSON.stringify(res)
				})
				if (currenturl != "") {
					cb(currenturl);
				} else {
					uni.showToast({
						title: "无法识别的二维码",
						icon: "none"
					})
				}
			}
			// 打印结果 使用alert在微信浏览器中是不生效的
		},
		fail: function(res) {
			console.log("扫描失败", res)
		},
	});
}


//微信选择图片
export function chooseima(maxNum, cb, allNum) {
	var ua = navigator.userAgent.toLowerCase();
	if (ua.match(/MicroMessenger/i) != "micromessenger") {
		uni.showToast({
			title: '该功能需要到微信公众号中使用',
			icon: 'none',
			duration: 2000
		})
		return;
	}
	uni.showLoading({
		title: '加载中'
	})
	jWeixin.chooseImage({
		count: maxNum,
		sizeType: ['compressed'], // 可以指定是原图还是压缩图，默认二者都有
		sourceType: ['album', 'camera'], // 可以指定来源是相册还是相机，默认二者都有
		success: function(res) {
			var serverIdList = [];
			var localIds = res.localIds; // 返回选定照片的本地ID列表，localId可以作为img标签的src属性显示图片
			if (res.localIds.length == 0) {
				return;
			}
			if (res.localIds.length > maxNum) {
				if (allNum) {
					uni.showToast({
						title: '最多只能上传' + allNum + '张图片',
						icon: 'none',
						duration: 1000
					})
				} else {
					uni.showToast({
						title: '最多只能上传六张图片',
						icon: 'none',
						duration: 1000
					})
				}

			}
			if (localIds.length > 0) {
				reUploadImg(localIds, serverIdList, cb);
			} else {
				uni.hideLoading()
			}

		},
		fail: (err) => {
			console.log("err", err);
			uni.hideLoading()
		},
		cancel: function() {
			uni.hideLoading()
		}
	});
}

export function reUploadImg(localIds, serverIdList, cb) {
	if (localIds.length == 0) {
		uni.hideLoading()
		cb(serverIdList, localIds);
	} else {
		var localId = localIds.pop();
		jWeixin.uploadImage({
			localId: localId,
			isShowProgressTips: 1,
			success: function(res) {
				serverIdList.push(res.serverId);
				reUploadImg(localIds, serverIdList, cb);
			},
			fail: function(res) {
				uni.hideLoading()
				uni.showToast({
					title: JSON.stringify(res),
					icon: 'none',
					duration: 1500
				})
				return;
			}
		});
	}

}
class wechatUtils {
	//开始录音
	startRecord(callback, errCallback) {
		let that = this
		jWeixin.startRecord({
			success: function() {
				that.timer = setInterval(() => {
					that.time++
				}, 1000)
				callback && callback()
				// that.onVoiceRecordEnd()
			},
			fail: function() {
				// 开始录音失败  
			},
			cancel: function() {
				// 用户拒绝授权录音
			}
		
		});
	}
	
	//停止录音
	stopRecord(callback, errCallback) {
		//停止录音接口
		jWeixin.stopRecord({
			success: function(res) {
		
				// alert(res.localId); 
				// let localId = res.localId;
				callback && callback(res)
			}
		})
	}
	
	//监听到录音停止
	onVoiceRecordEnd(callback, errCallback) {
		let that = this
		//停止录音接口
		jWeixin.onVoiceRecordEnd({
			complete: function(res) {
				// 60秒停止录音
				// that.localId = res.localId
				// this.translate()
				clearInterval(that.timer);
				callback && callback(res)
			}
	
		})
	}
	
	//录音转文字
	translateVoice(localId, callback, errCallback) {
		//停止录音接口
		jWeixin.translateVoice({
			localId: localId, // 需要识别的音频的本地Id，由录音相关接口获得
			isShowProgressTips: 1, // 默认为1，显示进度提示
			success: function(res) {
				// alert(res.localId); 
				// let localId = res.localId;
				callback && callback(res)
			},
			fail: (err) => {
				errCallback && errCallback(err)
			}
		})
	}
}
export default wechatUtils
// #endif