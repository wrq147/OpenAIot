import {
    totree
} from '@/utils/common.js'

import {
    getAreaList,
    getIndustryList,
    kuaiDiCompanys
} from '@/api/code.js'

const datas = {
    namespaced: true,
    state: {
        areaData: null,
        areaAllData: null,
        industryData: null,
        industry: '',
        kuaiDiList: null
    },
    mutations: {
        SET_INDUSTRY: (state, info) => {

            state.industry = info;
        },

        SET_AREA_DATA: (state, info) => {
            state.areaData = info;
        },
        SET_AREA_ALLDATA: (state, info) => {
            state.areaAllData = info;
        },
        SET_INDUSTRY_DATA: (state, info) => {
            state.industryData = info;
        },
        SET_KUAIDI_List: (state, info) => {
            state.kuaiDiList = info;
        }
    },
    actions: {
        //获取省市区数据
        areaTree({
            commit
        }) {
            return new Promise((resolve, reject) => {
                if (datas.state.areaData == null) {
                    getAreaList().then(res => {
                        let notTreeData = JSON.parse(JSON.stringify(res.data))
                        let treedata = totree(res.data, "Id", "ParentId", "children", (
                            xitem) => {
                            // xitem["children"].splice(0, 0, {
                            //     "Name": "--",
                            //     "Id": "-1"
                            // });
                        });
                        commit('SET_AREA_DATA', treedata);
                        commit('SET_AREA_ALLDATA', notTreeData);
                        resolve(treedata)
                    }).catch(error => {
                        reject(error)
                    })
                } else {
                    resolve(datas.state.areaData)
                }
            })
        },
        //获取行业数据
        industryTree({
            commit
        }) {
            return new Promise((resolve, reject) => {
                if (datas.state.industryData == null) {
                    getIndustryList().then(res => {
                        let treedata = totree(res.data, "Id", "ParentId", "children", (
                            xitem) => {});
                        commit('SET_INDUSTRY_DATA', treedata);
                        resolve(treedata)
                    }).catch(error => {
                        reject(error)
                    });
                } else {
                    resolve(datas.state.industryData)
                }
            })

        },
        industryName({
            commit
        }, code2) {
            return new Promise(async(resolve, reject) => {
                try {
                    let code = datas.state.industry
                    if (code2 && code2 != undefined && code2 != null) {
                        code = code2
                    }
                    let codestr = code.toString();
                    let indlist = [];
                    if (datas.state.industryData == null) {
                        let res = await getIndustryList()
                        indlist = totree(res.data, "Id", "ParentId", "children", (
                            xitem) => {});
                    } else {
                        indlist = datas.state.industryData
                    }
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


        kuaiDiList({ commit }) {
            return new Promise(async(resolve, reject) => {
                if (datas.state.kuaiDiList == null) {
                    kuaiDiCompanys().then(res => {
                        commit('SET_KUAIDI_List', res.data);
                        resolve(res.data)
                    }).catch(error => {
                        reject(error)
                    });
                } else {
                    resolve(datas.state.kuaiDiList)
                }
            });
        },
        kuaiName({ commit }, code) {
            return new Promise(async(resolve, reject) => {
                if (datas.state.kuaiDiList == null) {
                    kuaiDiCompanys().then(res => {
                        commit('SET_KUAIDI_List', res.data);
                        let rsss = res.data.filter(x => x.Code == code);
                        if (rsss.length > 0) {
                            resolve(rsss[0].Name)
                        } else {
                            reject("")
                        }
                    }).catch(error => {
                        reject(error)
                    });
                } else {
                    let rsss = datas.state.kuaiDiList.filter(x => x.Code == code);
                    if (rsss.length > 0) {
                        resolve(rsss[0].Name)
                    } else {
                        reject("")
                    }
                }
            });
        }

    },
}

export default datas