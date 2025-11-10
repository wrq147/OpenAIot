<template>
    <div class="custom-dropdown">
        <!-- 触发按钮 -->
        <button class="dropdown-trigger" @click="toggleMainDropdown">
            <i class="zhongtaiiconfont zhongtai-icon-xinzeng"></i>
            <span style="margin-left: 6px">新增</span>
        </button>

        <!-- 一级下拉菜单 -->
        <div 
            class="dropdown-menu main-dropdown" 
            :class="{ 'show': isMainDropdownShow }"
            @click.stop
        >
            <!-- 固定项：自定义 -->
            <div class="dropdown-item" @click="handleCustomClick">
                自定义
            </div>

            <!-- 分割线 -->
            <div class="dropdown-divider"></div>

            <!-- 动态渲染一级菜单 -->
            <div 
                class="dropdown-item has-children" 
                v-for="(firstLevel, firstIndex) in menuData" 
                :key="firstLevel.Id"
                @click="toggleSecondDropdown(firstIndex)"
            >
                <span class="dropdown-child-trigger">
                    {{ firstLevel.Name }}
                    <i class="el-icon-arrow-right el-icon--right" :class="{ 'rotate': activeSecondDropdownIndex === firstIndex }"></i>
                </span>

                <!-- 二级下拉菜单 -->
                <div 
                    class="dropdown-menu second-dropdown" 
                    :class="{ 'show': activeSecondDropdownIndex === firstIndex }"
                >
                    <div 
                        class="dropdown-item has-children" 
                        v-for="(secondLevel, secondIndex) in firstLevel.Children" 
                        :key="secondLevel.Id"
                        @click.stop="toggleThirdDropdown(firstIndex, secondIndex)"
                    >
                        <span class="dropdown-child-trigger">
                            {{ secondLevel.Name }}
                            <i class="el-icon-arrow-right el-icon--right" :class="{ 'rotate': activeThirdDropdown.first === firstIndex && activeThirdDropdown.second === secondIndex }"></i>
                        </span>

                        <!-- 三级下拉菜单 -->
                        <div 
                            class="dropdown-menu third-dropdown" 
                            :class="{ 'show': activeThirdDropdown.first === firstIndex && activeThirdDropdown.second === secondIndex }"
                        >
                            <!-- 检查三级菜单是否有数据 -->
                            <template v-if="secondLevel.CodeList && secondLevel.CodeList.length > 0">
                                <div 
                                    class="dropdown-item" 
                                    v-for="(codeItem, codeIndex) in secondLevel.CodeList" 
                                    :key="codeItem.Id"
                                    @click="handleCodeItemClick(firstLevel, secondLevel, codeItem)"
                                >
                                    {{ codeItem.Name }}
                                </div>
                            </template>
                            <!-- 没有数据时显示提示 -->
                            <template v-else>
                                <div class="dropdown-empty">
                                    暂无
                                </div>
                            </template>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
import { getCodeListTree } from "@/api/rules/productModel";

export default {
    name: "DynamicAddDropdown",
    props: {
        treeType: {
            type: String,
            default: "attribute"
        }
    },
    data() {
        return {
            menuData: [],
            isMainDropdownShow: false,
            activeSecondDropdownIndex: -1,
            activeThirdDropdown: {
                first: -1,
                second: -1
            }
        };
    },
    watch: {
        treeType: {
            immediate: true,
            handler() {
                this.loadMenuData();
            }
        }
    },
    methods: {
        async loadMenuData() {
            let intTreeType = 0;
            if (this.treeType === "attribute") {
                intTreeType = 0;
            } else if (this.treeType === "function") {
                intTreeType = 1;
            } else if (this.treeType === "event") {
                intTreeType = 2;
            }
            
            try {
                const res = await getCodeListTree(intTreeType);
                this.menuData = res.data || [];
            } catch (error) {
                console.error("Failed to load menu data:", error);
                this.menuData = [];
            }
        },
        
        // 切换主下拉菜单显示/隐藏
        toggleMainDropdown() {
            this.isMainDropdownShow = !this.isMainDropdownShow;
            // 关闭所有子菜单
            if (!this.isMainDropdownShow) {
                this.closeAllSubDropdowns();
            }
        },
        
        // 切换二级下拉菜单显示/隐藏
        toggleSecondDropdown(index) {
            // 如果点击的是当前激活的菜单，则关闭它
            if (this.activeSecondDropdownIndex === index) {
                this.activeSecondDropdownIndex = -1;
                this.closeThirdDropdown();
            } else {
                // 否则打开它，并关闭其他二级菜单和三级菜单
                this.activeSecondDropdownIndex = index;
                this.closeThirdDropdown();
            }
        },
        
        // 切换三级下拉菜单显示/隐藏
        toggleThirdDropdown(firstIndex, secondIndex) {
            // 先关闭其他可能打开的三级菜单
            if (!(this.activeThirdDropdown.first === firstIndex && this.activeThirdDropdown.second === secondIndex)) {
                this.activeThirdDropdown = {
                    first: firstIndex,
                    second: secondIndex
                };
            } else {
                // 如果点击的是当前激活的菜单，则关闭它
                this.closeThirdDropdown();
            }
        },
        
        // 处理自定义项点击
        handleCustomClick() {
            this.$emit("menu-click", {
                type: "custom",
                data: null
            });
            this.closeAllDropdowns();
        },
        
        // 处理三级菜单项点击
        handleCodeItemClick(firstLevel, secondLevel, codeItem) {
            this.$emit("menu-click", {
                type: "codeItem",
                firstLevel,
                secondLevel,
                codeItem
            });
            this.closeAllDropdowns();
        },
        
        // 关闭所有下拉菜单
        closeAllDropdowns() {
            this.isMainDropdownShow = false;
            this.closeAllSubDropdowns();
        },
        
        // 关闭所有子菜单（二级和三级）
        closeAllSubDropdowns() {
            this.activeSecondDropdownIndex = -1;
            this.closeThirdDropdown();
        },
        
        // 关闭三级菜单
        closeThirdDropdown() {
            this.activeThirdDropdown = {
                first: -1,
                second: -1
            };
        }
    },
    // 点击外部关闭下拉菜单
    mounted() {
        document.addEventListener('click', (e) => {
            if (!this.$el.contains(e.target)) {
                this.closeAllDropdowns();
            }
        });
    },
    beforeDestroy() {
        // 移除事件监听器，防止内存泄漏
        document.removeEventListener('click', () => {});
    }
};
</script>

<style scoped>
.custom-dropdown {
    position: relative;
    display: inline-block;
}

/* 触发按钮样式 */
.dropdown-trigger {
    display: inline-flex;
    align-items: center;
    justify-content: center;
    padding: 8px 16px;
    background-color: #ffffff;
    color: #1890ff;
    border: 1px solid #1890ff;
    border-radius: 4px;
    cursor: pointer;
    font-size: 14px;
    transition: all 0.2s ease;
}

.dropdown-trigger:hover {
    background-color: #f0f7ff;
}

/* 下拉菜单通用样式 */
.dropdown-menu {
    position: absolute;
    z-index: 1000;
    display: none;
    min-width: 160px;
    padding: 5px 0;
    margin: 2px 0 0;
    font-size: 14px;
    text-align: left;
    list-style: none;
    background-color: #fff;
    background-clip: padding-box;
    border: 1px solid #e4e7ed;
    border-radius: 4px;
    box-shadow: 0 2px 12px 0 rgba(0, 0, 0, 0.1);
}

.dropdown-menu.show {
    display: block;
}

/* 菜单项样式 */
.dropdown-item {
    display: block;
    width: 100%;
    padding: 5px 16px;
    clear: both;
    font-weight: 400;
    color: #303133;
    text-align: inherit;
    white-space: nowrap;
    background: transparent;
    border: 0;
    cursor: pointer;
    transition: background-color 0.2s ease;
}

.dropdown-item:hover {
    background-color: #f5f7fa;
}

/* 无数据提示样式 */
.dropdown-empty {
    display: block;
    width: 100%;
    padding: 5px 16px;
    color: #909399; /* 灰色文字 */
    text-align: center;
    font-size: 14px;
}

/* 带子菜单的菜单项 */
.dropdown-item.has-children {
    position: relative;
}

.dropdown-child-trigger {
    display: flex;
    align-items: center;
    justify-content: space-between;
    width: 100%;
}

/* 分割线样式 */
.dropdown-divider {
    height: 1px;
    margin: 6px 0;
    overflow: hidden;
    background-color: #e4e7ed;
}

/* 各级菜单位置调整 */
.main-dropdown {
    top: 100%;
    right: 0;
    margin-top: 4px;
    margin-right: -75px;
}

.second-dropdown {
    top: 0;
    left: 100%;
    margin-left: 4px;
    /* 增加z-index确保二级菜单在一级菜单上方 */
    z-index: 1001;
}

.third-dropdown {
    top: 0;
    left: 100%;
    margin-left: 4px;
    /* 增加z-index确保三级菜单在二级菜单上方 */
    z-index: 1002;
}

/* 图标样式 */
.el-icon--right {
    margin-left: 8px;
    font-size: 12px;
    transition: transform 0.2s ease;
}

/* 图标旋转效果，表示菜单已展开 */
.el-icon--right.rotate {
    transform: rotate(90deg);
}
</style>
    