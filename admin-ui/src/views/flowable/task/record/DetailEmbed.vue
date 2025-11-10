<template>
    <div :style="curStyle" v-if="node_list.length > 0">
        <slot></slot>
        <node-info-tree :nodeList="node_list"></node-info-tree>
    </div>
</template>
      
<script>
import { flowRootRecord, getQuery } from "@/api/flowable/process.js";
import NodeInfoTree from "../record/NodeInfoTree.vue";
export default {
    name: "DetailEmbed",
    props: ['curStyle'],
    components: {
        NodeInfoTree
    },
    data() {
        return {
            node_list: []
        };
    },
    mounted() { },
    methods: {
        async initNodes(flowId) {
            if (flowId != null && flowId > 0) {
                let rsp = await flowRootRecord(flowId);
                this.node_list = rsp.data.NodeList;
            }
            else {
                this.node_list = [];
            }

        }
    },
};
</script>
<style lang="scss" scoped></style>
      