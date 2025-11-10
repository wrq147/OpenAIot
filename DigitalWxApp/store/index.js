import Vue from 'vue'
import Vuex from 'vuex'

import submitMod from './submit.js'

Vue.use(Vuex)
import {
	wxLogin
} from '@/api/login'
import {
	getNoRead
} from '@/api/record.js'
import {
	getToken,
	setToken,
	setRefreshToken,
	removeToken
} from '@/common/auth'

import {
	totree
} from '@/common/util.js'

import {
	getInfo,
	getSelfInfo
} from '@/api/user.js'
import {
	getAreaList,
	getIndustryList,
	getDictList
} from '@/api/code.js'

import {
	getProClassList
} from '@/api/product.js'
import {
	getDeptList
} from '@/api/dept.js'
const store = new Vuex.Store({
	state: {
		userInfo: null,
		areaData: null,
		industryData: null,
		dictObj: {},
		holderReloadQueue: [],
		noread: null,
		isNewCard:true,//定义一个全局变量，专门用于验证名片是否有更新，控制是否要重新绘制名片
		sysType:'',
		proListTitle:"",
		shouleShow:false//用于新用户修改名片信息携带修改用户信息时做判断使用
	},
	mutations: {
		CARRY_USER_SHOW(state, info) {
			state.shouleShow = info;
		},
		SET_USER_INFO(state, info) {
			state.userInfo = info;
		},
		SET_AREA_DATA(state, info) {
			state.areaData = info;
		},
		SET_INDUSTRY_DATA(state, info) {
			state.industryData = info;
		},
		SET_NOREAD_DATA(state, info) {
			state.noread = info;
		},
		SET_ISNEWCARD_DATA(state, info) {
			state.isNewCard = info;
		},
		SET_SYSTYPE_DATA(state, info) {
			state.sysType = info;
		},
		SET_PROLISTTITLE_DATA(state, info) {
			state.proListTitle = info;
		}
	},
	actions: {
		
		//获取消息未读数量
		noread: function({
			commit,
			state
		}) {
			return new Promise((resolve, reject) => {
				getNoRead().then(res => {
					// console.log("未读数量设置", res);
					commit('SET_NOREAD_DATA', res.data);
					resolve(res.data)

				}).catch(error => {
					reject(error)
				})
			})
		},
		//微信登录
		weiXinLogin: function({
			commit,
			state
		}, userInfo) {
			return new Promise((resolve, reject) => {
				wxLogin(userInfo).then(res => {
					setToken(res.data.token);
					setRefreshToken(res.data.refresh_token);
					commit('SET_USER_INFO', res.data.ext_info);
					resolve(res.data)
				}).catch(error => {
					reject(error)
				});
			})
		},
		// 获取当前用户信息
		userInfo: function({
			commit,
			state
		}) {
			return new Promise((resolve, reject) => {
				if (state.userInfo == null) {
					getInfo().then(res => {
						getSelfInfo().then((result) => {
							if (result.data.user.Avatar) {
								res.data.avatar = result.data.user.Avatar
							}
							// console.log("用户信息ttt", res, result);
							commit('SET_USER_INFO', res.data);
							resolve(res.data)
						})


					}).catch(error => {
						reject(error)
					})
				} else {
					resolve(state.userInfo)
				}
			})
		},

		//获取省市区数据
		areaTree: function({
			commit,
			state
		}) {
			return new Promise((resolve, reject) => {
				if (state.areaData == null) {
					getAreaList().then(res => {
						let treedata = totree(res.data, "Id", "ParentId", "children", (
							xitem) => {
							xitem["children"].splice(0, 0, {
								"Name": "--",
								"Id": "-1"
							});
						});
						commit('SET_AREA_DATA', treedata);
						resolve(treedata)
					}).catch(error => {
						reject(error)
					})
				} else {
					resolve(state.areaData)
				}
			})
		},
		//获取行业数据
		industryTree: function({
			commit,
			state
		}) {
			return new Promise((resolve, reject) => {
				if (state.industryData == null) {
					getIndustryList().then(res => {
						let treedata = totree(res.data, "Id", "ParentId", "children", (
							xitem) => {});
						commit('SET_INDUSTRY_DATA', treedata);
						resolve(treedata)
					}).catch(error => {
						reject(error)
					})
				} else {
					resolve(state.industryData)
				}
			})
		},
		industryName: function({
			commit,
			state
		}, code) {
			return new Promise(async (resolve, reject) => {
				try {
					let codestr = code.toString();
					let indlist = state.industryData;
					for (let itx = 0; itx < indlist.length; itx++) {
						if (codestr.indexOf(indlist[itx].Id.toString()) == 0) {
							for (let cix = 0; cix < indlist[itx].children.length; cix++) {
								if (indlist[itx].children[cix].Id == code) {
									resolve(indlist[itx].children[cix].Name);
									return;
								}
							}
						}
					}
					reject("行业错误");
				} catch (error) {
					reject(error);
				}
			});

		},
		//获取产品分类id对应的名称
		flClassName: function({
			commit,
			state
		}, code) {
			return new Promise(async (resolve, reject) => {
				getProClassList({
					OrgId: state.userInfo.OrgId
				}).then(res => {
					let codestr = code.toString();
					let indlist = res.data;
					// console.log("state这边查询的产品分类",res);
					// console.log('分类id', code);
					// console.log("列表正常吗", indlist);
					for (let itx = 0; itx < indlist.length; itx++) {
						if (codestr.indexOf(indlist[itx].Id.toString()) == 0) {
							// for (let cix = 0; cix < indlist[itx].children
							// .length; cix++) {
							if (indlist[itx].Id == code) {
								resolve(indlist[itx].CategoryName);
								// return;
							}
							// }
						}
					}
				}).catch(error => {
					reject(error)
				})
			});

		},
		//获取部门id对应的名称
		deptName: function({
			commit,
			state
		}, code) {
			return new Promise(async (resolve, reject) => {
				getDeptList({
					OrgId: state.userInfo.OrgId
				}).then(res => {
					let codestr = code.toString();
					let indlist = res.data;
					// console.log("state这边查询的部门",res);
					// console.log('部门id', code);
					// console.log("部门列表正常吗", indlist);
					for (let itx = 0; itx < indlist.length; itx++) {
						if (codestr.indexOf(indlist[itx].deptId.toString()) == 0) {
							// for (let cix = 0; cix < indlist[itx].children
							// .length; cix++) {
							if (indlist[itx].deptId == code) {
								resolve(indlist[itx].deptName);
								// return;
							}
							// }
						}
					}
				}).catch(error => {
					reject(error)
				})
			});

		},
		//获取指定字典名称
		dictName: function({
			commit,
			state
		}, obj) {
			return new Promise(async (resolve, reject) => {
				try {
					let dictType = obj.name;
					let dictVal = obj.value.toString();

					if (!state.dictObj.hasOwnProperty(dictType)) {
						let res = await getDictList(dictType);
						state.dictObj[dictType] = res.data;
					}
					let rtarr = state.dictObj[dictType].filter(v => {
						return v.value == dictVal;
					});

					if (rtarr.length > 0) {
						resolve(rtarr[0].label);
					} else {
						resolve("");
					}
				} catch (error) {
					reject(error);
				}

			});
		},


		//获取指定字典列表
		dictList: function({
			commit,
			state
		}, dictType) {
			return new Promise(async (resolve, reject) => {
				try {
					if (!state.dictObj.hasOwnProperty(dictType)) {
						let res = await getDictList(dictType);
						state.dictObj[dictType] = res.data;
					}
					resolve(state.dictObj[dictType]);
				} catch (error) {
					reject(error);
				}
			});
		}
	},
	modules: {
		submitMod
	}
})

export default store
