<template>
    <div>
        <el-dialog title="图形化编程器" :close-on-click-modal="false" top="5vh" :visible.sync="openGraph" width="1400px"
            @close="close" append-to-body>
            <codegraph ref="cdd" @InitNodes="Init"></codegraph>
            <div slot="footer" class="dialog-footer">
                <el-button @click="confirmGraph" type="primary">确 定</el-button>
                <el-button @click="openGraph = false">取 消</el-button>
            </div>
        </el-dialog>
    </div>
</template>

<script>
let codegraph = () => import("@/components/litegraph/code/codegraph.vue")
import { InitStringNodes } from "@/components/litegraph/nodes/strings.js";
import { InitLogicNodes } from "@/components/litegraph/nodes/logic.js";
import { InitBaseNodes } from "@/components/litegraph/nodes/base.js";
import { InitMathNodes } from "@/components/litegraph/nodes/math.js";
import { InitEventsNodes } from "@/components/litegraph/nodes/events.js"
import { InitProgramNodes } from "@/components/litegraph/nodes/program.js"
import {InitFuncNodes} from "@/components/litegraph/nodes/func.js"

export default {
    name: "GraphCodeDialog",
    components: {
        codegraph
    },
    props: {
        initval: {
            type: String,
            default: ""
        }
    },
    data() {
        return {
            openGraph: false,
            graphObj: null,
            graphContainerObj: null
        };
    },
    watch: {
        initval(newval, oldval) {
            this.InitNodes(newval);
        }
    },
    mounted() {
    },
    methods: {
        Init(graph, graphContainer) {
            this.graphObj = graph;
            this.graphContainerObj = graphContainer;
            InitStringNodes(graph);
            InitLogicNodes(graph);
            InitBaseNodes(graph);
            InitMathNodes(graph);
            InitEventsNodes(graph);
            InitProgramNodes(graph);
            InitFuncNodes(graph);

            this.InitNodes(this.initval);
        },
        InitNodes(jsonstr) {
            if (jsonstr != "") {
                try {
                    let jsonobj = JSON.parse(jsonstr);
                    this.graphContainerObj.configure(JSON.parse(jsonstr));
                }
                catch { }
            }
            else {
                this.graphContainerObj.clear();
                var node_watch = this.graphObj.createNode("程序/模拟输出");
                node_watch.pos = [600, 100];
                this.graphContainerObj.add(node_watch);
                var node_obj = this.graphObj.createNode("常用/对象");
                node_obj.pos = [300, 350];
                this.graphContainerObj.add(node_obj);
                var node_func = this.graphObj.createNode("功能/功能回复");
                node_func.pos = [600, 350];
                this.graphContainerObj.add(node_func);
                node_obj.connect(0, node_func, 0);
                node_obj.connect(0, node_watch, 0);
            }
        },
        open() {
            this.openGraph = true;
        },
        close() {
            if (this.graphObj != null) {
                this.graphObj.closeAllContextMenus();
            }
            this.$refs.cdd.debugStop();
        },
        confirmGraph() {
            let jsonstr = JSON.stringify(this.graphContainerObj.serialize());
            this.$emit("confirm", jsonstr);
            this.openGraph = false;
        }
    }
};
</script>

<style lang="scss" scoped>
::v-deep .el-dialog__body {
    padding-top: 0;
}
</style>