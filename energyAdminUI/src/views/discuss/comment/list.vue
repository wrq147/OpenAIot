<template>
    <div style="padding:10px 10px 0 10px;height:100%" id="big_con">
        <div class="elbiaoge_elform pinglun_list_con" :style="{ 'min-height': tableConHeight + 'px' }">
            <el-tabs v-model="activeName" @tab-click="handleClick">
                <el-tab-pane label="我的评论" name="my"></el-tab-pane>
                <el-tab-pane label="回复我的" name="return"></el-tab-pane>
            </el-tabs>
            <div class="pinglun_list">
                <div class="pinglun_list_ul" v-if="pinglunList.length > 0">
                    <div class="pinglun_list_li" v-for="item in pinglunList" :key="item.Id">
                        <div class="pinglun_info">
                            <div class="pinglun_user_heading" v-if="activeName == 'return'">
                                <img class="heading" :src="employeeMap.get(item.UserId).Heading" alt=""
                                    v-if="employeeMap && employeeMap.get(item.UserId) && employeeMap.get(item.UserId).Heading">
                                <div class="heading"
                                    v-else-if="employeeMap && employeeMap.get(item.UserId) && employeeMap.get(item.UserId).RealName">
                                    {{
                                        employeeMap.get(item.UserId).RealName.slice(employeeMap.get(item.UserId).RealName.length
                                            - 2)
                                    }}</div>
                            </div>
                            <div class="pinglun_user">{{ item.UserId == myUserId ? '我' :
                                (employeeMap && employeeMap.get(item.UserId) ? employeeMap.get(item.UserId).RealName : '')
                            }}
                            </div>
                            <div class="pinglun_date" v-if="item.CreateOn">{{ item.CreateOn.slice(0, 10) }}</div>
                            <div class="jingtai">评论了</div>
                            <div class="content_type">{{ item.Subject.TargetType }}</div>
                            <div class="content_title" @click.stop="jumpContent(item)">{{ item.Subject.Title }}</div>
                        </div>
                        <div class="pinglun_content_con">
                            <img src="@/assets/images/yinghao_kaiqi.png" alt="">
                            <div class="pinglun_content" v-html="item.Content"></div>
                            <img src="@/assets/images/yinghao_guanbi.png" alt="">
                        </div>
                        <div class="ping_handle" v-if="activeName=='my'">
                            <span @click.stop="deletePinglun(item)">
                                <i class="zhongtaiiconfont zhongtai-icon-shanchu"></i>删除
                            </span>
                        </div>
                    </div>

                </div>
                <el-empty description="暂无评论" v-else :image-size="200"></el-empty>
                <pagination v-show="total > 0" :total="total" :page.sync="queryParams.pageNum"
                    :limit.sync="queryParams.pageSize" @pagination="loadpinglunList" />
            </div>
        </div>

    </div>
</template>

<script>
import { resizeTableCon } from "@/mixins/resizeTableCon";
import { commentList,removePinglun } from '@/api/crm/comment'
import { listMember } from "@/api/system/Employee";
export default {
    name: 'AdminUiList',
    mixins: [resizeTableCon],
    data() {
        return {
            loading: true,
            activeName: 'my',
            queryParams: {
                pageNum: 1,
                pageSize: 10,
                IsMy: true
            },
            total: 0,//评论总数
            pinglunList: [],
            employeeMap: new Map()
        };
    },
    computed: {
        myUserId() {
            return this.$store.state.user.uid
        }
    },
    async mounted() {
        await this.getMemberList();
        this.loadpinglunList()
    },

    methods: {
        deletePinglun(item){
            //删除评论
            this.$modal.confirm('是否确认删除该条评论？')
            .then(function (res) {
                return removePinglun(item.Id);
            })
            .then(() => {
                this.queryParams.pageNum = 1;
                this.loadpinglunList();
                this.$modal.msgSuccess("删除成功");
            })
            .catch((err) => {
                console.log(err,'errerr',row);
            });
        },
        jumpContent(item) {
            //点击评论实现跳转
            if (item.Subject.TargetType == '客户') {
                this.$router.push({ path: "/crm/customer/customerDetail", query: { id: item.Subject.TargetId, } });
            }
            if (item.Subject.TargetType == '线索') {
                this.$router.push({ path: "/crm/clue/clueDetail", query: { id: item.Subject.TargetId, } });
            }
            if (item.Subject.TargetType == '商机') {

            }
        },
        async getMemberList() {
            try {
                let rsp = await listMember({ deptIdWithChildren: true, showAll: true,isPrimaryDept:true });
                // console.log("员工列表", rsp);
                if (rsp.data && rsp.data.List) {
                    rsp.data.List.map(row => {
                        this.employeeMap.set(row.Id, row);
                    });
                }
            } catch (err) {
                console.log("报错", err);

                this.$message.error(err.data);
            }
        },
        loadpinglunList() {
            //
            this.loading = true
            if (this.activeName == 'my') {//是否是我的评论
                if (this.queryParams.IsMyReply) {
                    delete this.queryParams.IsMyReply
                    this.queryParams.IsMy = true
                }else{
                    this.queryParams.IsMy = true
                }
            } else if (this.activeName == 'return') {//是否是回复我的评论
                if (this.queryParams.IsMy) {
                    delete this.queryParams.IsMy
                    this.queryParams.IsMyReply = true
                }else{
                    this.queryParams.IsMyReply = true
                }
            }
            commentList(this.queryParams).then(res => {
                if (res.data && res.data.List) {
                    this.total = res.data.Total
                    this.pinglunList = res.data.List
                }
                this.loading = false

            }).catch(err => {
                this.loading = false
            })
        },
        handleClick() {
            this.pinglunList = []
            this.queryParams.pageNum = 1
            this.loadpinglunList()
        }
    },
};
</script>

<style lang="less">
.pinglun_list_con {
    .el-tabs .el-tabs__header {
        .el-tabs__nav-wrap::after {
            height: 0;
        }
    }
}

.pinglun_list {
    .pinglun_list_ul {
        .pinglun_list_li {
            padding: 10px 0;
            width: 100%;
            border-bottom: 1px dashed #DDDDDD;
            padding: 0;

            .pinglun_info {
                font-size: 14px;
                display: flex;
                justify-content: flex-start;
                align-items: center;

                .pinglun_user_heading {
                    display: flex;
                    justify-content: center;
                    align-items: center;

                    .heading {
                        width: 30px;
                        height: 30px;
                        border-radius: 15px;
                        background: linear-gradient(90deg, #4c79ff 0%, #6da8ff 100%);
                        color: #fff;
                        font-size: 12px;
                        display: flex;
                        justify-content: center;
                        align-items: center;
                    }
                }

                .pinglun_user {
                    color: #555666;
                }

                .pinglun_date {
                    color: #999;
                    // margin-left: 5px;
                }

                .jingtai {
                    color: #999;
                    // margin-left: 7px;
                }

                .content_type {
                    color: #555666;
                    // margin-left: 7px;
                }

                .content_title {
                    color: #349EDF;
                    // margin-left: 7px;
                }
            }

            .pinglun_content_con {
                padding: 0 20px;
                width: 100%;
                box-sizing: border-box;
                display: flex;
                justify-content: space-between;
                align-items: flex-start;
                margin-top: 10px;

                .pinglun_content {
                    width: calc(100% - 80px);
                }

                img {
                    width: 16px;
                    height: 16px;
                }
            }
            .ping_handle{
                width: 100%;
                display: flex;
                justify-content: flex-end;
                align-items: center;
                margin: 10px 0;
                padding-right: 30px;
                box-sizing: border-box;
                color: #E43F32;
                i{
                    margin-right: 5px;
                }
            }

            .pinglun_info>div:nth-child(n) {
                margin-left: 7px;
                line-height: 16px;
            }
        }
    }
}
</style>