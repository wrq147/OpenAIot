import {
	setToken,
	getToken,
	getRefreshToken,
	setRefreshToken
} from '@/common/auth.js'
import {
	checkPermi
} from '@/common/permission.js';
export default {
	data() {
		return {

		};
	},
	onReady() {
		// #ifdef APP-PLUS
		plus.navigator.setStatusBarStyle('white'); //设置顶部为白色
		// #endif
	},
	onShow() {
		// #ifdef APP-PLUS
		plus.navigator.setStatusBarStyle('white'); //设置顶部为白色
		// #endif
	},
	onHide() {
		// #ifdef APP-PLUS
		plus.navigator.setStatusBarStyle('white');
		// #endif
	},
	async onload() {
		const permissions = this.$store.getters && this.$store.getters.permissions
		console.log(!permissions || permissions.length == 0, 'permissions');
		if (!permissions || permissions.length == 0) {
			await this.$store.dispatch("GetInfo")
		}

	},
	methods: {
		isCheckPermi(val) {
			return checkPermi(val)
		},
		setMsgTop(err) { //访问接口错误提示
			console.log("提示", err);
			if (err.code&&err.code > 9) {
				// if (err.cusMsg) {
				// 	this.$refs.promptMsg.open(err.cusMsg, 2000)
				// } else if (err.message) {
				// 	this.$refs.promptMsg.open(err.message, 2000)
				// }
				if(err.code==400||err.code==50009){
					this.$refs.promptMsg.open(err.cusMsg, 3000)
					var iptcode = this.getUrlParam('code') || "";
					if(iptcode){
						uni.reLaunch({
							url: '/pages/index/login',
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
					if(err.code==604){
						this.$refs.promptMsg.open('There is an abnormality in the system. Please submit it to the administrator for repair！', 3000)
					}else{
						if (err.cusMsg) {
							this.$refs.promptMsg.open(err.cusMsg, 3000)
						} else if (err.message) {
							this.$refs.promptMsg.open(err.message, 3000)
						}
					}
					
				}
			} else {
				if (err.message) {
					this.$refs.promptMsg.open(err.message, 2000)
				} else {
					this.$refs.promptMsg.open(err, 2000)
				}
			}
		},
		iconSubStr(row) {
			//图标名称裁剪
			if (row && row.indexOf('el') > -1) {
				return row.substring(3)
			}
		},
	}

}