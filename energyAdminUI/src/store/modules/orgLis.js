import { getJoinOrgList } from "@/api/system/user";
const orgLis = {
    namespaced: true,
    state: {
        orgList: null,
    },

    mutations: {
        SET_ORG_LIST: (state, val) => {
            state.orgList = val
        },

    },

    actions: {
        //获取指定企业列表
        setOrgList({ commit }) {
            return new Promise(async(resolve, reject) => {
                try {
                    if (orgLis.state.orgList == null) {
                        let res = await getJoinOrgList();
                        // console.log('企业列表', res);
                        commit('SET_ORG_LIST', res.data)
                        orgLis.state.orgList = res.data;
                    }
                    resolve(orgLis.state.orgList);
                } catch (error) {
                    reject(error);
                }
            });
        }
    }
}

export default orgLis