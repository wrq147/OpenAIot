import {
	login,
	logout,
	getInfo,
	getUserInfo
} from '@/api/login'
import {
	getJoinOrgList
} from "@/api/user";
import {
	getToken,
	setToken,
	removeToken,
	getRefreshToken,
	setRefreshToken,
	removeRefreshToken
} from '@/common/auth.js'

const user = {
	state: {
		uid: '',
		name: '',
		avatar: '',
		orgId: 0,
		roles: [],
		permissions: [],
	},
	mutations: {
		SET_UID: (state, val) => {
			state.uid = val
		},
		SET_NAME: (state, name) => {
			state.name = name
		},
		SET_AVATAR: (state, avatar) => {
			state.avatar = avatar
		},
		SET_ROLES: (state, roles) => {
			state.roles = roles
		},
		SET_PERMISSIONS: (state, permissions) => {
			state.permissions = permissions
		},
		SET_ORGID: (state, orgId) => {
			state.orgId = orgId
		},
	},

	actions: {
		// 登录
		Login({
			commit
		}, userInfo) {
			const username = userInfo.username.trim()
			const password = userInfo.password
			const code = userInfo.code
			const uuid = userInfo.uuid
			return new Promise((resolve, reject) => {
				login(username, password, code, uuid).then(res => {
					setToken(res.data.token)
					setRefreshToken(res.data.refresh_token)
					resolve(res)
				}).catch(error => {
					console.log(error, 'error');
					reject(error)
				})
			})
		},

		// 获取用户信息
		GetInfo({
			commit,
			state
		}) {
			return new Promise((resolve, reject) => {
				getInfo().then(res => {
					// console.log("res个人信息", res);
					commit('SET_UID', res.data.Id.toString());
					commit('SET_PERMISSIONS', res.data.permissions)
					// commit('SET_ROLES', ['ROLE_DEFAULT'])
					commit('SET_NAME', res.data.name)
					commit('SET_AVATAR', res.data.avatar)
					commit('SET_ORGID', res.data.OrgId)
					resolve(res)
				}).catch(error => {
					reject(error)
				})
			})
		},
		// 获取当前用户用户信息
		getUserInfo({
			commit,
			state
		}) {
			return new Promise((resolve, reject) => {
				getUserInfo({id:0}).then(res => {
					console.log("res个人信息11111", res);
					commit('SET_ROLES', res.data.roles)
					resolve(res)
				}).catch(error => {
					reject(error)
				})
			})
		},
		// 退出系统
		LogOut({
			commit,
			state
		}) {
			return new Promise((resolve, reject) => {
				logout().then(() => {
					commit('SET_ROLES', [])
					commit('SET_PERMISSIONS', [])
					removeToken()
					removeRefreshToken();
					resolve()
				}).catch(error => {
					reject(error)
				})
			})
		},

		// 前端 登出
		FedLogOut({
			commit
		}) {
			return new Promise(resolve => {
				commit('SET_TOKEN', '')
				removeToken()
				resolve()
			})
		}
	}
}

export default user