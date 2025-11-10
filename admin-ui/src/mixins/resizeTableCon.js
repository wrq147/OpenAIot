export const resizeTableCon = {
    data() {
        return {
            tableConHeight: 0, //设置
        }
    },
    watch: {
        '$store.state.settings.title' () {
            this.setTableCon()
        },
        '$store.state.settings.theme' () {
            this.setTableCon()
        },
        '$store.state.settings.sideTheme' () {
            this.setTableCon()
        },
        '$store.state.app.sidebar' () {
            this.setTableCon()
        },
        '$store.state.app.device' () {
            this.setTableCon()
        },
        '$store.state.settings.tagsView' () {
            this.setTableCon()
        },
        '$store.state.settings.fixedHeader' () {
            this.setTableCon()
        },
    },
    computed: {
        globalFontSize() {
            return this.$store.state.app.size
        }
    },
    destroyed() {
        // 销毁
        window.removeEventListener("resize", this.setTableCon());
    },
    mounted() {
        this.setTableCon();
        window.addEventListener("resize", () => { this.setTableCon() });
    },
    methods: {
        setTableCon() { //设置表格容器的高度
            let div = document.getElementById("from_con");
            let div2 = document.getElementById("big_con");

            if (div && div2) {
                // console.log(div.offsetHeight, div2.offsetHeight);

                let conHei = div.offsetHeight + 40;
                let conHei2 = div2.offsetHeight;
                this.tableConHeight = conHei2 - conHei;
            } else if (div2 && !div) {
                // console.log(div2.offsetHeight);

                let conHei2 = div2.offsetHeight;
                this.tableConHeight = conHei2;
            }

        },
        cellSty({ row, rowIndex }) {
            // console.log(row, rowIndex);
            if (rowIndex == 0) {
                let obj = {
                    'color': '#78829D',
                    'background': '#F9FAFC !important'
                }
                return obj;
            }
        },
    }
}