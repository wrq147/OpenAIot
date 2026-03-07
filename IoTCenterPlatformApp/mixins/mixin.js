import {
	setToken,
	getToken,
	getRefreshToken,
	setRefreshToken
} from '@/common/auth.js'
import {
	checkPermi
} from '@/common/permission.js';
import {jumpLogin} from "@/common/utillib.js"
import serverUrl from '@/common/constVar.js'
export default {
	data() {
		return {

		};
	},
	async onload() {
		const permissions = this.$store.getters && this.$store.getters.permissions
		if (!permissions || permissions.length == 0) {
			await this.$store.dispatch("GetInfo")
		}

	},
	methods: {
		getSerVerUrl(){
			return serverUrl.getServerUrl()
		},
		getUrlParam(name) {
			let reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
			let r = window.location.search.substr(1).match(reg);
			// console.log(r, 'rrrrr');
			if (r != null) return unescape(r[2]);
			return null;
		},
		isCheckPermi(val) {
			return checkPermi(val)
		},
		setMsgTop(err) { //访问接口错误提示
			if (err.code&&err.code > 9) {
				if(err.code==400||err.code==50009||err.code==600001){
					this.$refs.promptMsg.open(err.cusMsg, 3000)
					var iptcode = this.getUrlParam('code') || "";
					if(iptcode){
						uni.reLaunch({
							url: '/page_register/login',
							// #ifdef APP-PLUS
							success: () => {
								plus.navigator.closeSplashscreen();
							},
							// #endif
						})
					}else{
						jumpLogin()
					}
					
				}else{
					// if(err.code==604){
					// 	this.$refs.promptMsg.open('系统有异常，请提交管理员修复！', 3000)
					// }else{
					// 	if (err.cusMsg) {
					// 		this.$refs.promptMsg.open(err.cusMsg, 3000)
					// 	} else if (err.message) {
					// 		this.$refs.promptMsg.open(err.message, 3000)
					// 	}
					// }
					if (err.cusMsg) {
						this.$refs.promptMsg.open(err.cusMsg, 3000)
					} else if (err.message) {
						this.$refs.promptMsg.open(err.message, 3000)
					}
					
				}
				
			} else {
				if (err.message) {
					this.$refs.promptMsg.open(err.message, 3000)
				}else if (err.errMsg) {
					if(err.errMsg.indexOf('request')>-1&&err.errMsg.indexOf('fail')>-1){
						// this.$refs.promptMsg.open('网络异常', 3000)
					}else{
						this.$refs.promptMsg.open(err.errMsg, 3000)
					}
					
				} else {
					// this.$refs.promptMsg.open(JSON.stringify(err), 3000)
					this.$refs.promptMsg.open('异常待修复', 3000)
				}
			}
		},
		iconSubStr(row) {
			//图标名称裁剪
			// console.log("图标",row);
			if (row && row.indexOf('el') > -1) {
				return row.substring(3)
			}
		},
	}

}