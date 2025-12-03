import * as spritejs from "spritejs";
export default {
    props: {
        x: {
            type: Number,
        },
        y: {
            type: Number,
        },
        z: {
            type: Number,
            default: 0
        },
        customId: {
            type: Number,
        },
        activeId: {
            type: [Number, String],
        },
    },
    data() {
        return {
            point1: null,
            point2: null,
            point3: null,
            point4: null,
            spriteimg: null,
            background: null,
            activeId1: 0,
            scene: null,
            layer: null,
        }
    },
    watch: {
        x() {
            this.$nextTick(() => {
                this.changeSpriceImg();
            });
        },
        y() {
            this.$nextTick(() => {
                this.changeSpriceImg();
            });
        },
        width() {
            this.$nextTick(() => {
                this.changeSpriceImg();
            });
        },
        height() {
            this.$nextTick(() => {
                this.changeSpriceImg();
            });
        },
    },
    beforeCreate() {

    },
    beforeDestroy() {

        if (this.background) {
            this.background.remove();
        }
        if (this.point1) {
            this.point1.remove();
        }
        if (this.point2) {
            this.point2.remove();
        }
        if (this.point3) {
            this.point3.remove();
        }
        if (this.point4) {
            this.point4.remove();
        }
        if (this.spriteimg) {
            this.spriteimg.remove();
        }
    },
    mounted() {

        this.$nextTick(() => {
            const { Scene } = spritejs;
            this.container = document.querySelector('.configuration' + this.chartOption.bindingDiv); //容器生成
            let container = this.container;
            this.scene = new Scene({
                container,
                viewport: ['auto', 'auto'],
                resolution: [this.widNum, this.heiNum],
                preserveDrawingBuffer: true,
                // useOffscreen: true,
            });
            if (container) { } else {
                return;
            }
            this.layer = this.scene.layer();
        })
    },
    computed: {
        widNum() {
            if (this.width.indexOf('px') > -1) {
                return Number(this.width.substring(0, this.width.length - 2));
            } else if (this.width.indexOf('%') > -1) {
                return Number(this.width.substring(0, this.width.length - 1));
            } else {
                return Number(this.width.substring(0, this.width.length));
            }
        },
        heiNum() {
            if (this.height.indexOf('px') > -1) {
                return Number(this.height.substring(0, this.height.length - 2));
            } else if (this.height.indexOf('%') > -1) {
                return Number(this.height.substring(0, this.height.length - 1));
            } else {
                return Number(this.height.substring(0, this.height.length));
            }

        },
    },
    methods: {
        domOnMouseUp() {
            //元素上抬起鼠标
            this.$emit('clearSprite')
        },

        async sprite(imgUrl) {
            // console.log(this.layer, 'this.layerthis.layer');
            // console.log("组态相关配置", this.chartOption);
            if (!this.layer) {
                return
            }
            const { Sprite } = spritejs;
            if (this.spriteimg) {
                this.spriteimg.remove();
            }
            if (Array.isArray(imgUrl)) {
                // await spritejs.loadImages(imgUrl).catch((error) => {
                //     console.error('图片加载失败:', error);
                // });

                // 创建动画帧
                this.spriteimg = new spritejs.Sprite();
                this.spriteimg.attr({
                    id: 'content' + this.customId,
                    layer: Number(this.customId),
                    zIndex: this.z + 1,
                    // bgcolor: "#fff",
                    pos: [this.widNum / 2, this.heiNum / 2], // 矩形的位置，设置为中间
                    size: [this.widNum, this.heiNum],
                    borderRadius: "50%",
                    frameRate: 60,
                    frameIndex: 0,
                    animation: 'default',
                    anchor: [0.5, 0.5] //设置中心点
                });
                this.layer.appendChild(this.spriteimg);

                // 实现动画循环
                let textureArr = []
                for (let i = 0; i < imgUrl.length; i++) {
                    textureArr.push({ texture: imgUrl[i] })
                }
                const animation = this.spriteimg.animate(textureArr, { //绘制动画图
                    duration: 500,
                    iterations: Infinity,
                    easing: 'linear'
                });
            } else {
                this.spriteimg = new Sprite(imgUrl);
                this.spriteimg.attr({
                    id: 'content' + this.customId,
                    layer: Number(this.customId),
                    zIndex: this.z + 1,
                    // bgcolor: "#fff",
                    pos: [this.widNum / 2, this.heiNum / 2], // 矩形的位置，设置为中间
                    size: [this.widNum, this.heiNum],
                    borderRadius: "50%",
                    frameRate: 60,
                    frameIndex: 0,
                    animation: 'default',
                    anchor: [0.5, 0.5] //设置中心点
                });
                this.layer.appendChild(this.spriteimg);
            }
        },
        changeSpriceImg() {
            // console.log("更新了", this.widNum, this.heiNum);
            this.scene.resize() //更新容器窗口
            this.$nextTick(() => {
                this.spriteimg.attr({
                    pos: [this.widNum / 2, this.heiNum / 2], // 矩形的位置，设置为中间
                    size: [this.widNum, this.heiNum],
                });
                this.scene.render();
            })
        },
    }
}