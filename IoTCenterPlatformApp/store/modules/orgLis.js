import { getJoinOrgList } from "@/api/user";
const orgLis = {
    namespaced: true,
    state: {
        orgList: null,
		isChangeOrg:false,//是否在我的页面切换企业
    },

    mutations: {
        SET_ORG_LIST: (state, val) => {
            state.orgList = val
        },
		SET_CHANGE_ORG:(state,val)=>{
			state.isChangeOrg=val
		}

    },

    actions: {
        //获取指定企业列表
        setOrgList() {
            return new Promise(async(resolve, reject) => {
                try {
                    if (orgLis.state.orgList == null) {
                        let res = await getJoinOrgList();

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